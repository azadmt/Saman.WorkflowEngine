using Newtonsoft.Json;

namespace API.WFBase
{
    public class ConsoleEventHook : IWorkflowHook
    {
        public Task ExecuteAsync(WorkflowContext context)
        {
            Console.WriteLine("📢 EVENT PUBLISHED: " + JsonConvert.SerializeObject(context.Data));
            return Task.CompletedTask;
        }
    }
