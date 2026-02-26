using Newtonsoft.Json.Serialization;
using RuleEngine.Base;
using WorkflowBase;
using WorkflowEngine.Core.Common.Builder;
using WrokflowDefinition.HealthInsuranceIssue.Activity;
using WrokflowDefinition.HealthInsuranceIssue.DataContract;
using WrokflowDefinition.HealthInsuranceIssue.RuleDefinitions;

namespace WrokflowDefinition.HealthInsuranceIssue;

public class HealthInsuranceWorkflow : IWorkflowDefinitionFactory
{
    public WorkflowDefinition GetDefinition()
    {
        var rulsetId = "health-underwriting";
        RegisterRuleSet(rulsetId);
        return new WorkflowDefinition
        {
            Name = "health-underwriting",
            Version = 1,
            RuleSetId = rulsetId,
            StartState = "AutoMedicalCheck",
            States = new Dictionary<string, StateDefinition>
            {
                ["Start"] = new StateDefinition
                {
                    Type = StateType.Start,
                    Transitions =
                    {
                        new TransitionDefinition { To = "AutoMedicalCheck"},
                    }
                },
                ["AutoMedicalCheck"] = new StateDefinition
                {
                    Title = "بررسی سیستمی شرایط بیمه نامه",
                    RulesetId = rulsetId,
                    Type = StateType.Automatic,
                    Activities =
                    {
                        //call api for policy
                        new EvaluateMedicalRiskActivity()
                    },
                    Transitions = {
                        new TransitionDefinition {
                            To = "WaitingForDoctor",
                        // Condition=(p)=> p.GetData<string>("RiskLevel")=="medium"},
                         ConditionExpression="GetData<string>(\"RiskLevel\") == \"medium\"",
                        },
                        new TransitionDefinition {
                            To = "Rejected" ,
                           // Condition=(p)=> p.GetData<string>("RiskLevel")=="high",
                            ConditionExpression="GetData<string>(\"RiskLevel\") == \"high\"",
                        },
                          new TransitionDefinition {
                            To = "Approved",

                         ConditionExpression="GetData<string>(\"RiskLevel\") == \"low\"",
                    }
                }
                },
                ["WaitingForDoctor"] = new StateDefinition
                {
                    Title = "بررسی توسط پزشک",
                    Type = StateType.HumanTask,
                    Activities =
                    {
                        new DoctorReviewActivity()
                    },
                    HumanTask = new HumanTaskDefinition
                    {
                        Role = "Doctor",
                        UiContract = "DoctorMedicalReview",
                        Title = "بررسی بیمه نامه درمان",
                        Inputs = new List<TaskInput>
                         {
                      
                        new() { Name = "Doctor_ExtraRate", Lable = "نرخ اضافی پیشنهادی (%)", Type = InputType.Number },
                        new() { Name = "Doctor_Notes", Lable = "یادداشت‌های پزشکی", Type = InputType.Textarea },
                        new() { Name = "Doctor_RemovedCovers",
                            Lable = "پوشش ها",
                            Type = InputType.MultiSelect,
                            OptionsDataProvider= ctx =>
                                    {
                                        var req = ctx.GetData<HealthPolicyRequest>("PolicyRequest");
                                        return req?
                                        .Covers?
                                        .Select(x=> new KeyValuePair<string,string>(x.Id.ToString(),x.Name))
                                        .ToList();
                                    }

                        },
                        new() {
                            Name = "Doctor_ApprovalStatus",
                            Lable = "وضعیت تأیید",
                            Type = InputType.Dropdown,
                            Options = new() {
                                new("APPROVE", "تأیید"), 
                                new("REJECT", "رد") ,
                                new("REQUEST_CompletingMedicalDocuments", "نیاز به تکمیل مدارک پزشکی")
                            }
                               }
                          },
                        ContextDisplayFields = new List<DisplayField> {
                            new()
                                {
                                    Label = "نام بیمه‌گذار",
                                    Order = 1,
                                    ValueProvider = ctx =>
                                    {
                                        var req = ctx.GetData<HealthPolicyRequest>("PolicyRequest");
                                        return req?.Insureds
                                        .First(x=> x.Id==req.PolicyHodler)
                                        .Name ?? "نامشخص";
                                    }
                                },
                            new()
                                {
                                    Label = "مبلغ بیمه نامه",
                                    Order = 2,
                                    ValueProvider = ctx =>
                                    {
                                        var req = ctx.GetData<HealthPolicyRequest>("PolicyRequest");
                                        return req?.Ammount;
                                    }
                                },
                            new()
                                {
                                    Label = "پوشش ها",
                                    Order = 3,
                                    GridColumns = new List<GridColumn>
                                    {
                                        new GridColumn { Header = "کد", Field = "Id" },
                                        new GridColumn { Header = "نام", Field = "Name" }
                                    },
                                    ValueProvider = ctx =>
                                    {
                                        var req = ctx.GetData<HealthPolicyRequest>("PolicyRequest");
                                        return req?.Covers;
                                    }
                                },                            
                            new()
                                {
                                    Label = "بیمه شدگان",
                                    Order = 3,
                                      GridColumns = new List<GridColumn>
                                    {
                                       
                                        new GridColumn { Header = "نام", Field = "Name" },
                                        new GridColumn { Header = "تاریخ تولد", Field = "BirthDate", Format = "yyyy/MM/dd" },
                                        new GridColumn { Header = "بیماری زمینه‌ای", Field = "HasUnderlyingDisease" }
                                    },
                                    ValueProvider = ctx =>
                                    {
                                        var req = ctx.GetData<HealthPolicyRequest>("PolicyRequest");
                                        return req?.Insureds;
                                    }
                                }


                        }
                    },
                    Transitions =
                    {
                        new  () { Event = "APPROVE", To = "Approved",Title="تایید" },
                        new  (){
                                    Event = "REQUEST_CompletingMedicalDocuments",
                                    To = "CompletingMedicalDocuments",
                                    Title="تکمیل مدارک" },
                        new  (){ Event = "REJECT", To = "Rejected",Title="رد" }
                    }
                },
                ["CompletingMedicalDocuments"] = new StateDefinition
                {
                    Title = "تکمیل مدارک پزشکی توسط بیمه گذار",
                    Type = StateType.HumanTask,

                    HumanTask = new HumanTaskDefinition
                    {
                        Role = "Customer",
                        UiContract = "UploadLabResult",
                        Inputs = new List<TaskInput>() {
                            new TaskInput { Name = "DocUrl", Type = InputType.FileUpload }
                        },
                        ContextDisplayFields = new List<DisplayField> {
                            new()
                                {
                                    Label = "کامنت پزشک",
                                    Order = 1,
                                    ValueProvider = ctx =>
                                    {
                                        var req = ctx.GetData<string>("Doctor_Notes");
                                        return req;
                                    }
                                },
                        },
                        AutoAssigne = (p) =>
                        {
                            var policy= p.GetData<HealthPolicyRequest>("PolicyRequest");
                            return policy.Insureds[0].Name;
                        }
                    },
                    Transitions =
                    {
                        new TransitionDefinition { Event = "MedicalDocuments_UPLOADED", To = "WaitingForDoctor",Title = "ارسال" }
                    }
                },
                ["Approved"] = new StateDefinition
                {
                    Title = "تایید شده",
                    Type = StateType.End,
                    Activities =
                    {
                        new ApproveProposalActivity()
                        //new ActivityDefinition { Type = "apiCall", Reference = "UpdatePolicy" },
                        //new ActivityDefinition { Type = "event", Reference = "PolicyApproved" }
                    }
                },
                ["Rejected"] = new StateDefinition
                {
                    Title = "ردشده",
                    Type = StateType.End,

                    Activities =
                    {
                        new RejectProposalActivity()
                        //new ActivityDefinition { Type = "apiCall", Reference = "UpdatePolicy" },
                        //new ActivityDefinition { Type = "event", Reference = "PolicyApproved" }
                    }
                }
            }
        };
    }

    public WorkflowDefinition GetFluentDefinition()
    {
        var ruleSetId = "health-underwriting";
        RegisterRuleSet(ruleSetId);

        return Workflow.Define("health-underwriting", version: 1)
            .WithRuleSet(ruleSetId)
            .StartsAt("AutoMedicalCheck")

            // ────────────── State: Start ──────────────
            .State("Start",StateType.Start)               
                .On("start")  
                .GoTo("AutoMedicalCheck")
                .Done()

            // ────────────── State: AutoMedicalCheck ──────────────
            .State("AutoMedicalCheck", StateType.Automatic)
                .Title("بررسی سیستمی شرایط بیمه‌نامه")                
                .Activity<EvaluateMedicalRiskActivity>()
                .WhenExpression("GetData<string>(\"RiskLevel\") == \"low\"")
                    .GoTo("Approved")
                .WhenExpression("GetData<string>(\"RiskLevel\") == \"medium\"")
                    .GoTo("WaitingForDoctor")
                .WhenExpression("GetData<string>(\"RiskLevel\") == \"high\"")
                    .GoTo("Rejected")
                .Done()

            // ────────────── State: WaitingForDoctor ──────────────
            .State("WaitingForDoctor", StateType.HumanTask)
                .Title("بررسی توسط پزشک")
                .HumanTask(role: "Doctor", uiContract: "DoctorMedicalReview")
                    .Title("بررسی بیمه‌نامه درمان")

                    // Inputs
                    .InputNumber("Doctor_ExtraRate", "نرخ اضافی پیشنهادی (%)")
                    .InputTextarea("Doctor_Notes", "یادداشت‌های پزشکی")

                    .InputMultiSelectFromContext(
                        name: "Doctor_RemovedCovers",
                        label: "پوشش ها",
                        optionsProvider: ctx =>
                        {
                            var req = ctx.GetData<HealthPolicyRequest>("PolicyRequest");
                            return req?.Covers?
                                .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name))
                                .ToList() ?? new List<KeyValuePair<string, string>>();
                        })

                    .WithDecisionField(configureOptions: dropdown =>
                    {
                        dropdown.Options.Add(new("APPROVE", "تأیید"));
                        dropdown.Options.Add(new("REJECT", "رد"));
                        dropdown.Options.Add(new("REQUEST_CompletingMedicalDocuments", "نیاز به تکمیل مدارک پزشکی"));
                    })

                    // Display Fields
                    .Display("نام بیمه‌گذار", ctx =>
                    {
                        var req = ctx.GetData<HealthPolicyRequest>("PolicyRequest");
                        return req?.Insureds
                            .FirstOrDefault(x => x.Id == req.PolicyHodler)?
                            .Name ?? "نامشخص";
                    }, order: 1)

                    .Display("مبلغ بیمه‌نامه", ctx =>
                    {
                        var req = ctx.GetData<HealthPolicyRequest>("PolicyRequest");
                        return req?.Ammount;
                    }, order: 2)

                    .DisplayGrid("پوشش ها", ctx => ctx.GetData<HealthPolicyRequest>("PolicyRequest")?.Covers, order: 3)
                        .Column("کد", "Id")
                        .Column("نام", "Name")
                        .Done()

                    .DisplayGrid("بیمه‌شدگان", ctx => ctx.GetData<HealthPolicyRequest>("PolicyRequest")?.Insureds, order: 4)
                        .Column("نام", "Name")
                        .Column("تاریخ تولد", "BirthDate", format: "yyyy/MM/dd")
                        .Column("بیماری زمینه‌ای", "HasUnderlyingDisease")
                        .Done()

                // Transitions
                .On("APPROVE")
                    .GoTo("Approved", title: "تایید")
                .On("REQUEST_CompletingMedicalDocuments")
                    .GoTo("CompletingMedicalDocuments", title: "تکمیل مدارک")
                .On("REJECT")
                    .GoTo("Rejected", title: "رد")
                .Done()

            // ────────────── State: CompletingMedicalDocuments ──────────────
            //.State("CompletingMedicalDocuments", StateType.HumanTask)
            //    .Title("تکمیل مدارک پزشکی توسط بیمه‌گذار")
            //    .HumanTask(role: "Customer", uiContract: "UploadLabResult")
            //        .InputFileUpload("DocUrl", "آپلود مدرک")  // اگر InputFileUpload دارید، یا از InputText با نوع File استفاده کنید
            //        .Display("کامنت پزشک", ctx => ctx.GetData<string>("Doctor_Notes"), order: 1)

            //        // AutoAssign
            //        .AutoAssign(ctx =>
            //        {
            //            var policy = ctx.GetData<HealthPolicyRequest>("PolicyRequest");
            //            return policy?.Insureds.FirstOrDefault()?.Name;
            //        })
            //    .On("MedicalDocuments_UPLOADED")
            //        .GoTo("WaitingForDoctor", title: "ارسال")
            //    .Done()

            // ────────────── State: Approved ──────────────
            .State("Approved",StateType.End)
                .Title("تایید شده")                
                .Activity<ApproveProposalActivity>()
                .Done()

            // ────────────── State: Rejected ──────────────
            .State("Rejected",StateType.End)
                .Title("ردشده")                
                .Activity<RejectProposalActivity>()
                .Done()

            .Build();
    }

    private void RegisterRuleSet(string rulesetId)
    {
        // کانفیگ قوانین برای این ورک‌فلو خاص
        var ruleSet = new RuleSet
        {
            Id = rulesetId,
            Rules = new List<RuleBinding>
            {
                // قانون سن: برای بیمه سلامت معمولی
                new()
                {
                    Rule = new AgeLimitRule(),
                    Parameters = new RuleParameters()
                    .WithParam("minAge",18)
                    .WithParam("maxAge",79)
                },

                new()
                {
                    Rule = new UnderlyingDiseaseRule()
                }
                //,

                //// قانون سابقه بیماری
                //new()
                //{
                //    Rule = new ChronicDiseaseRule(),
                //    Parameters = new RuleParameters
                //    {
                //        ["MaxAllowedChronicDiseases"] = 2
                //    }
                //}
            }
        };

        RuleSetRegistry.RegisterRuleSet(ruleSet);
    }
}