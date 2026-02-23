using Newtonsoft.Json.Serialization;
using RuleEngine.Base;
using WorkflowBase;
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
                        new() { Name = "ExtraRate", Lable = "نرخ اضافی پیشنهادی (%)", Type = InputType.Number },
                        new() { Name = "MedicalNotes", Lable = "یادداشت‌های پزشکی", Type = InputType.Text },
                        new() { Name = "RemovedCovers",
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
                            Name = "ApprovalStatus",
                            Lable = "وضعیت تأیید",
                            Type = InputType.Dropdown,
                            Options = new() { new("Approved", "تأیید"), new("Rejected", "رد") }
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
                        AutoAssigne = (p) =>
                        {
                            return p.GetData<HealthPolicyRequest>("PolicyRequest").PolicyHodler.ToString();
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