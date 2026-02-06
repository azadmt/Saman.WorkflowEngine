namespace WorkflowEngine.Core
{
    // 3. Define the Workflow class
    public class Workflow
    {
        public string Name { get; set; }
        public List<IWorkflowStep> Steps { get; set; } = new List<IWorkflowStep>();

        public void AddStep(IWorkflowStep step)
        {
            Steps.Add(step);
        }
    }
}
