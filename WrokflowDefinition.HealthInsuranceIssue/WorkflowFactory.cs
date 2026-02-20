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
                    Title = "بررسی سیستمی شرایط پزشکی نفرات بیمه شده",
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
                        Inputs = new List<TaskInput>() {
                            new TaskInput { Name = "ExtraRate", Type = InputType.Number }
                        }
                    },
                    Transitions =
                    {
                        new TransitionDefinition { Event = "APPROVE", To = "Approved" },
                        new TransitionDefinition { Event = "REQUEST_CompletingMedicalDocuments", To = "CompletingMedicalDocuments" },
                        new TransitionDefinition { Event = "REJECT", To = "Rejected" }
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
                            new TaskInput { Name = "DocUrl", Type = InputType.Text }
                        },
                        AutoAssigne = (p) =>
                        {
                            return p.GetData<HealthPolicyRequest>("PolicyRequest").PolicyHodler.ToString();
                        }
                    },
                    Transitions =
                    {
                        new TransitionDefinition { Event = "MedicalDocuments_UPLOADED", To = "WaitingForDoctor" }
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