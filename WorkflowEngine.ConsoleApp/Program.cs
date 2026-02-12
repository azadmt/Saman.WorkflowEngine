using System.Reflection;
using WorkflowCore;
using WorkflowEngine.ConsoleApp.WorkflowDefinitions.HealthInsuranceIssueWorkflow;

namespace WorkflowEngine.ConsoleApp;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Health Insurance Workflow Demo ===\n");


        // 1️⃣ Load workflow definition (normally from DB / JSON)
        var workflowDefinitions = GetAllWorkflowDefinitions();

        // 2️⃣ Create engine
        var workflowTaskRepo = new WorkflowTaskRepository();
        var engine = new WorkflowCore.WorkflowEngine(new WorkflowRepository(), workflowTaskRepo);
        engine.RegisterWorkflow(workflowDefinitions);
        // 3️⃣ Start workflow instance with initial variables
       
      var instance=  engine.Start(
            workflowName: "health-underwriting",
            workflowVersion: 1,
            input: new Dictionary<string, object>
            {
                ["Smoker"] = true,
                ["HasSurgeryHistory"] = false
            }
        );
     var opentasks=   workflowTaskRepo.GetAvailableTasks("Doctor", null);
        // 4️⃣ Doctor opens task
        if (instance.Status == WorkflowInstanceStatus.Waiting)
        {
            Console.WriteLine("--- Doctor reviewing case ---");

            engine.Resume(
                instanceId: instance.Id,
                eventName: "REQUEST_LAB",
                data: new Dictionary<string, object> { ["RequestedLab"] = "Blood Test" }
            );
        }

        Console.WriteLine($"State after doctor action: {instance.CurrentStateId}\n");

        // 5️⃣ User uploads lab result
        if (instance.Status == WorkflowInstanceStatus.Waiting)
        {
            Console.WriteLine("--- User uploads lab result ---");

            engine.Resume(
                instanceId: instance.Id,
                eventName: "LAB_UPLOADED",
                data: new Dictionary<string, object> { ["BloodTestResult"] = "NORMAL" }
            );
        }

     //   Console.WriteLine($"State after lab upload: {instance.CurrentStateId}\n");
        // 6️⃣ Doctor final decision
        if (instance.Status == WorkflowInstanceStatus.Waiting)
        {
            Console.WriteLine("--- Doctor final decision ---");

            engine.Resume(
                instanceId: instance.Id,
                eventName: "APPROVE",
                data: new Dictionary<string, object> { ["ExtraPremium"] = 15 }
            );
        }

      //  Console.WriteLine($"Final State: {instance.CurrentStateId}");
        Console.WriteLine($"Workflow Status: {instance.Status}");
    }

    private static IEnumerable<WorkflowDefinition> GetAllWorkflowDefinitions()
    {
        var workflowDefiniotionType = typeof(IWorkflowDefinition);
        var workflowDefinitions = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(type => workflowDefiniotionType.IsAssignableFrom(type) && !type.IsInterface);

        foreach (var definition in workflowDefinitions)
        {
           var instance= (IWorkflowDefinition)Activator.CreateInstance(definition);

            yield return instance.GetDefinition();
        }
    }
}
