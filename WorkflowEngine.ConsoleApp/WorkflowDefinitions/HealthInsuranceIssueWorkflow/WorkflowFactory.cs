
namespace WorkflowEngine.ConsoleApp.WorkflowDefinitions.HealthInsuranceIssueWorkflow;

public class HealthInsuranceWorkflow : IWorkflowDefinition
{
    public WorkflowDefinition GetDefinition()
    {
        return new WorkflowDefinition
        {
            Name = "health-underwriting",
            Version = 1,
            StartState = "AutoMedicalCheck",
            States = new Dictionary<string, StateDefinition>
            {
                ["AutoMedicalCheck"] = new StateDefinition
                {
                    Id = "AutoMedicalCheck",
                    Type = StateType.Automatic,
                    Activities =
                    {
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
                    Id = "WaitingForDoctor",
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
                            new TaskInput { Name = "ExtraRate", Type = typeof(int).Name }
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
                    Id = "WaitingForLab",
                    Type = StateType.HumanTask,
                    HumanTask = new HumanTaskDefinition
                    {
                        Role = "Customer",
                        UiContract = "UploadLabResult",
                        Inputs = new List<TaskInput>() {
                            new TaskInput { Name = "DocUrl", Type = typeof(string).Name }
                        }
                    },
                    Transitions =
                    {
                        new TransitionDefinition { Event = "LAB_UPLOADED", To = "WaitingForDoctor" }
                    }
                },
                ["Approved"] = new StateDefinition
                {
                    Id = "Approved",
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
                    Id = "Rejected",
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


}
