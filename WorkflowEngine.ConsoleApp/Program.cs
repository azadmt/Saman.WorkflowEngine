using System.Reflection;
using System.Runtime.CompilerServices;
using WorkflowBase;
using WorkflowEngine.Core.Common;
using WorkflowEngine.Core.Persistence;
using WrokflowDefinition.HealthInsuranceIssue;
using WrokflowDefinition.HealthInsuranceIssue.DataContract;

namespace WorkflowEngine.ConsoleApp;

class Program
{

    static void Main()
    {
        Console.WriteLine("=== Health Insurance Workflow Demo ===\n");

       
        // 1️⃣ Load workflow definition (normally from DB / JSON)
        var workflowDefinitions = GetAllWorkflowDefinitions();

        // 2️⃣ Create engine
        var dbContext = new LiteDbContext("WorkFlowHost.db");
        var workflowTaskRepo = new WorkflowTaskService(dbContext);
        var workfloeRepo = new WorkflowRepository();
        var engine = new WorkflowBase.WorkflowEngine(workfloeRepo, workflowTaskRepo, dbContext);
        engine.RegisterWorkflow(workflowDefinitions);
        // 3️⃣ Start workflow instance with initial variables
        var poicyRequest = HealthPolicyRequest.GenerateSample(underlyingDiseaseCount: 1);
        var instance = engine.Start(
              workflowName: "health-underwriting",
              workflowVersion: 1,
              input: new Dictionary<string, object>
              {
                  ["PolicyRequest"] = poicyRequest,
              }
          );


        var doctor = new { Role = "Doctor", UserName = "Dr.Ahmadi" };

        var opentasks = workflowTaskRepo.GetAvailableTasks(doctor.Role, doctor.UserName);
        foreach (var item in opentasks)
        {
            //TODO 
            workflowTaskRepo.AssigneTask(item.Id, doctor.Role, doctor.UserName);
        }
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

        Console.WriteLine($"State after doctor action: {instance.CurrentStateName}\n");

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

        Console.WriteLine($"=========================================");
        foreach (var item in instance.WorkflowHistories)
        {
            Console.WriteLine($" {item.Timestamp.DateTime.ToString("dddd, yyyy MMMM dd  HH:mm:ss")} - go to  {item.StateName}- by {item.User}");

        }
    }

    private static IEnumerable<WorkflowDefinition> GetAllWorkflowDefinitions()
    {
        var workflowDefiniotionType = typeof(IWorkflowDefinitionFactory);
        var workflowDefinitions = typeof(HealthInsuranceWorkflow).Assembly
    .GetTypes()
    .Where(type => workflowDefiniotionType.IsAssignableFrom(type) && !type.IsInterface);

        foreach (var definition in workflowDefinitions)
        {
            var instance = (IWorkflowDefinitionFactory)Activator.CreateInstance(definition);

            yield return instance.GetDefinition();
        }
    }
}

public class Activity
{
    public Input<int> Id { get; set; } = new();

    public void run()
    {
        Id.Get<int>(null);
    }
}
public class Input<T>
{
    public T Get<T>( IDataContext dataContext, [CallerMemberName] string memberName = "")
    {
        return default(T);
    }
}
