using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.ConsoleApp
{
    public static class WorkflowFactory
    {
        public static WorkflowDefinition CreateHealthInsuranceWorkflow()
        {
            return new WorkflowDefinition
            {
                Id = "health-underwriting",
                Version = 1,
                StartStateId = "AutoMedicalCheck",
                States = new Dictionary<string, StateDefinition>
                {
                    ["AutoMedicalCheck"] = new StateDefinition
                    {
                        Id = "AutoMedicalCheck",
                        Type = StateType.Automatic,
                        Activities =
{
new ActivityDefinition { Type = "rule", Reference = "EvaluateMedicalRisk" }
},
                        Transitions =
{
new TransitionDefinition { To = "WaitingForDoctor" }
}
                    },

                    ["WaitingForDoctor"] = new StateDefinition
                    {
                        Id = "WaitingForDoctor",
                        Type = StateType.HumanTask,
                        HumanTask = new HumanTaskDefinition
                        {
                            Role = "Doctor",
                            UiContract = "DoctorMedicalReview"
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
                            UiContract = "UploadLabResult"
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
new ActivityDefinition { Type = "apiCall", Reference = "UpdatePolicy" },
new ActivityDefinition { Type = "event", Reference = "PolicyApproved" }
}
                    },

                    ["Rejected"] = new StateDefinition
                    {
                        Id = "Rejected",
                        Type = StateType.End
                    }
                }
            };
        }
    }
}