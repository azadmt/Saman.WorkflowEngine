using WorkflowEngine.Core.Common;
namespace WorkflowBase;

public interface IWorkflowContex : IDataContext
{
    IReadOnlyList<TransitionDefinition> GetTransitions();
}
