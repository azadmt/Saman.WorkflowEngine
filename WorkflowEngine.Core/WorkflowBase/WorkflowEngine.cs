using WorkflowEngine.Core.Common;
using WorkflowEngine.Core.Persistence;

namespace WorkflowBase;
public class WorkflowEngine
{

    private readonly IWorkflowRepository _workflowRepository;
    private readonly WorkflowTaskService _workflowTaskRepository;
    private readonly LiteDbContext _dbContext;

    public WorkflowEngine(IWorkflowRepository workflowRepository, WorkflowTaskService workflowTaskRepository, LiteDbContext dbContext)
    {
        _workflowRepository = workflowRepository;
        _workflowTaskRepository = workflowTaskRepository;
        _dbContext = dbContext;
    }

    public void RegisterWorkflow(IEnumerable<WorkflowDefinition> definitions)
    {
        WorkflowDefinitionRegistry.RegisterWorkflow(definitions);
    }

    public WorkflowInstance Start(string workflowName, int workflowVersion, Dictionary<string, object> input)
    {
        var def = WorkflowDefinitionRegistry.Get(workflowName, workflowVersion);
        var instance = new WorkflowInstance
        {
            WorkflowDefinitionId = def.GetId(),
            CurrentStateName = def.StartState
        };

        foreach (var kv in input)
        {
            instance.Context.SetData(kv.Key, kv.Value);
        }

        Execute(instance);
        _workflowRepository.Add(instance);
        _dbContext.SaveChanges();
        return instance;
    }

    public void Resume(
       Guid instanceId,
       string eventName,
       Dictionary<string, object> data,
       WorkflowTask workflowTask = null
   )
    {
        var workflowInstance = _workflowRepository.Get(instanceId);
        SetCurrentStateDefinition(workflowInstance);

        foreach (var kv in data)
            workflowInstance.Context.SetData(kv.Key, kv.Value);

        var state = workflowInstance.Context.CurrentStateDefinition;//TO DO : Just check in Execute
        var transition = state.Transitions.Find(t => t.Event == eventName)!;
        workflowInstance.CurrentStateName = transition.To;
        workflowInstance.Status = WorkflowInstanceStatus.Running;

        if (workflowTask != null)
        {
            _workflowTaskRepository.CompleteTask(workflowTask.Id);
        }
        Execute(workflowInstance);
        _workflowRepository.Update(workflowInstance);
        _dbContext.SaveChanges();
    }

    private void Execute(WorkflowInstance instance)
    {
        SetCurrentStateDefinition(instance);

        var currentState = instance.Context.CurrentStateDefinition;

        instance.AddHistoryEntry();
        if (instance.Context.CurrentStateDefinition.Type == StateType.Automatic)
        {
            foreach (var activity in currentState.Activities)
            {
                activity.ExecuteAsync(instance.Context);
            }

            //var transition = currentState.Transitions.Where(x => x.Condition(instance.Context)).Single();
            //instance.CurrentStateName = transition.To;

            TransitionDefinition transition = null;

            foreach (var t in currentState.Transitions)
            {
                bool conditionMet = false;

                if (t.ConditionFunc != null)
                {

                    conditionMet = t.ConditionFunc(instance.Context);
                }
                else if (!string.IsNullOrEmpty(t.ConditionExpression))
                {
                    try
                    {
                        conditionMet = ExpressionEvaluator
                            .EvaluateConditionAsync(t.ConditionExpression, instance.Context)
                            .GetAwaiter()
                            .GetResult();
                    }
                    catch (Exception ex)
                    {
                        // لاگ و ادامه یا throw
                        continue;
                    }
                }
                else
                {

                    conditionMet = true;
                }

                if (conditionMet)
                {
                    transition = t;
                    break;  // اولین درست
                }
            }
            if (transition == null)
            {
                throw new InvalidOperationException("هیچ transition شرطی برقرار نیست");
            }

            instance.CurrentStateName = transition.To;

            Execute(instance);

        }
        else if (currentState.Type == StateType.HumanTask)
        {
            instance.Status = WorkflowInstanceStatus.Waiting;
            _workflowTaskRepository.Add(GetWorkflowTask(instance));

        }
        else if (currentState.Type == StateType.End)
        {
            foreach (var activity in currentState.Activities)
                activity.ExecuteAsync(instance.Context);

            instance.Status = WorkflowInstanceStatus.Completed;
        }


    }

    private async Task RunActivity(IWorkflowActivity act, WorkflowInstance instance)
    {
        var activity = (IWorkflowActivity)Activator.CreateInstance(Type.GetType(act.Name));
        await activity.ExecuteAsync(instance.Context);
    }

    private void SetCurrentStateDefinition(WorkflowInstance instance)
    {
        instance.Context.CurrentStateDefinition = WorkflowDefinitionRegistry
            .Get(instance.WorkflowDefinitionId)
            .States[instance.CurrentStateName]
            ;
    }


    private static WorkflowTask GetWorkflowTask(WorkflowInstance workflowInstance)
    {
        var stateDefinition = workflowInstance.Context.CurrentStateDefinition;
        var taskAssignee = stateDefinition.HumanTask.AutoAssigne != null
                ? stateDefinition?.HumanTask.AutoAssigne.Invoke(workflowInstance.Context)
                : null;

        stateDefinition
            .HumanTask
            .ContextDisplayFields
            .ForEach(x => { x.Value = x.ValueProvider.Invoke(workflowInstance.Context); });

        stateDefinition
            .HumanTask
            .Inputs
            .Where(x => x.OptionsDataProvider != null)
            .ToList()
            .ForEach(x => x.Options = x.OptionsDataProvider.Invoke(workflowInstance.Context));
        return new WorkflowTask
        {
            Role = stateDefinition.HumanTask.Role,
            Title = stateDefinition.Title,
            WorkflowInstanceId = workflowInstance.Id,
            WorkflowDefinitionId = workflowInstance.WorkflowDefinitionId,
            Inputs = stateDefinition.HumanTask.Inputs,
            Assignee = taskAssignee,
            ContextDisplayFields = stateDefinition
            .HumanTask
            .ContextDisplayFields
        };
    }
}


public static class WorkflowDefinitionRegistry
{
    private static Dictionary<string, WorkflowDefinition> _workflowRegistry = new();

    public static void RegisterWorkflow(IEnumerable<WorkflowDefinition> definitions)
    {
        foreach (var d in definitions)
        {
            _workflowRegistry[d.GetId()] = d;
        }
    }

    public static WorkflowDefinition Get(string workflowName, int workflowVersion)
    {
        return _workflowRegistry[$"{workflowName}-V{workflowVersion}"];
    }

    public static WorkflowDefinition Get(string workflowDefinitionId)
    {
        return _workflowRegistry[workflowDefinitionId];
    }
}