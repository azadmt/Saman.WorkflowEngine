using WorkflowEngine.Core.Common;
namespace WorkflowBase;

public interface IWorkflowContext : IDataContext
{
    IReadOnlyList<TransitionDefinition> GetTransitions();
}
