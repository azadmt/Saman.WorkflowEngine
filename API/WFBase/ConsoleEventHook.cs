using Newtonsoft.Json;

namespace API.WFBase
{
    public class ConsoleEventHook : IWorkflowHook
    {
        public async Task ExecuteAsync(WorkflowContext context)
        {
            Console.WriteLine(" EVENT PUBLISHED: " + JsonConvert.SerializeObject(context.Data));
    
        }
    }
}