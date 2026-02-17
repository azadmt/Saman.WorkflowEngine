
using WorkflowEngine.ConsoleApp.RuleEngineCore;
using WorkflowEngine.ConsoleApp.WorkflowDefinitions.HealthInsuranceIssueWorkflow.RuleDefinitions;

namespace WorkflowEngine.ConsoleApp.WorkflowDefinitions.HealthInsuranceIssueWorkflow;

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
            RuleSetId= rulsetId,
            StartState = "AutoMedicalCheck",
            States = new Dictionary<string, StateDefinition>
            {
                ["AutoMedicalCheck"] = new StateDefinition
                {
                    Name = "AutoMedicalCheck",
                    RulesetId= rulsetId,
                    Type = StateType.Automatic,
                    Activities =
                    {
                        //call api for policy
                        new EvaluateMedicalRiskActivity()

                    },
                    Transitions = {
                        new TransitionDefinition {
                            To = "WaitingForDoctor",
                         Condition=(p)=> p.GetData<string>("RiskLevel")=="medium"},
                        new TransitionDefinition {
                            To = "Rejected" ,
                            Condition=(p)=> p.GetData<string>("RiskLevel")=="high",

                        }

                    }
                },
                ["WaitingForDoctor"] = new StateDefinition
                {
                    Name = "WaitingForDoctor",
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
                        new TransitionDefinition { Event = "REQUEST_LAB", To = "WaitingForLab" },
                        new TransitionDefinition { Event = "REJECT", To = "Rejected" }
                    }
                },
                ["WaitingForLab"] = new StateDefinition
                {
                    Name = "WaitingForLab",
                    Type = StateType.HumanTask,
                    HumanTask = new HumanTaskDefinition
                    {
                        Role = "Customer",
                        UiContract = "UploadLabResult",
                        Inputs = new List<TaskInput>() {
                            new TaskInput { Name = "DocUrl", Type = InputType.Text }
                        }
                    },
                    Transitions =
                    {
                        new TransitionDefinition { Event = "LAB_UPLOADED", To = "WaitingForDoctor" }
                    }
                },
                ["Approved"] = new StateDefinition
                {
                    Name = "Approved",
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
                    Name = "Rejected",
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
