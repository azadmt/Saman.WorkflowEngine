using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WorkflowEngine.Core
{
    public class WorkflowEngine
    {
        public async Task RunWorkflowAsync(Workflow workflow, WorkflowContext context)
        {

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            Console.WriteLine($"Starting workflow: {workflow.Name}");
            foreach (var step in workflow.Steps)
            {
                Console.WriteLine($"Executing step: {step.Name}");
                try
                {
                    var contextJson = JsonConvert.SerializeObject(context,settings);
                    Console.WriteLine(contextJson);

                    context = JsonConvert.DeserializeObject<WorkflowContext>(contextJson, settings);
                    await step.ExecuteAsync(context);
        
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in step {step.Name}: {ex.Message}");
                   
                }
            }
            Console.WriteLine($"Workflow {workflow.Name} completed.");
        }
    }
}
