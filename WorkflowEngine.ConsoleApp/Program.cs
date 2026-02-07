

namespace WorkflowEngine.ConsoleApp;
class Program
{
    static void Main()
    {
        Console.WriteLine("=== Health Insurance Workflow Demo ===\n");


        // 1️⃣ Load workflow definition (normally from DB / JSON)
        var workflowDefinition = WorkflowFactory.CreateHealthInsuranceWorkflow();


        // 2️⃣ Create engine
        var engine = new WorkflowEngine(new[] { workflowDefinition });


        // 3️⃣ Start workflow instance with initial variables
        var instance = engine.Start(
        workflowId: "health-underwriting",
        input: new Dictionary<string, object>
        {
            ["Smoker"] = true,
            ["HasSurgeryHistory"] = false
        }
        );


        Console.WriteLine($"Workflow started. InstanceId: {instance.Id}");
        Console.WriteLine($"Current State: {instance.CurrentStateId}\n");
        // 4️⃣ Doctor opens task
        if (instance.Status == WorkflowInstanceStatus.Waiting)
        {
            Console.WriteLine("--- Doctor reviewing case ---");


            engine.Resume(
            instanceId: instance.Id,
            eventName: "REQUEST_LAB",
            data: new Dictionary<string, object>
            {
                ["RequestedLab"] = "Blood Test"
            },
            instance: instance
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
            data: new Dictionary<string, object>
            {
                ["BloodTestResult"] = "NORMAL"
            },
            instance: instance
            );
        }


        Console.WriteLine($"State after lab upload: {instance.CurrentStateId}\n");
        // 6️⃣ Doctor final decision
        if (instance.Status == WorkflowInstanceStatus.Waiting)
        {
            Console.WriteLine("--- Doctor final decision ---");


            engine.Resume(
            instanceId: instance.Id,
            eventName: "APPROVE",
            data: new Dictionary<string, object>
            {
                ["ExtraPremium"] = 15
            },
            instance: instance
            );
        }


        Console.WriteLine($"Final State: {instance.CurrentStateId}");
        Console.WriteLine($"Workflow Status: {instance.Status}");
    }
}