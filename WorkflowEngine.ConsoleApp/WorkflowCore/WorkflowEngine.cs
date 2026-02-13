using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace WorkflowCore;

public class WorkflowEngine
{
    private Dictionary<string, WorkflowDefinition> _workflowRegistry = new();
    private readonly WorkflowRepository _workflowRepository;
    private readonly WorkflowTaskRepository _workflowTaskRepository;

    public WorkflowEngine(WorkflowRepository workflowRepository, WorkflowTaskRepository workflowTaskRepository)
    {
        _workflowRepository = workflowRepository;
        _workflowTaskRepository = workflowTaskRepository;
    }

    public void RegisterWorkflow(IEnumerable<WorkflowDefinition> definitions)
    {
        foreach (var d in definitions)
        {
            _workflowRegistry[d.GetId()] = d;
        }
    }

    public WorkflowInstance Start(string workflowName, int workflowVersion, Dictionary<string, object> input)
    {
        var def = _workflowRegistry[$"{workflowName}-V{workflowVersion}"];
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
        return instance;
    }

    public void Resume(
       Guid instanceId,
       string eventName,
       Dictionary<string, object> data
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

        Execute(workflowInstance);
    }

    private void Execute(WorkflowInstance instance)
    {
        SetCurrentStateDefinition(instance);

        var currentState = instance.Context.CurrentStateDefinition;

        if (instance.Context.CurrentStateDefinition.Type == StateType.Automatic)
        {
            foreach (var activity in currentState.Activities)
            {
                activity.ExecuteAsync(instance.Context);
            }
            
            var transition = currentState.Transitions.Where(x => x.Condition(instance.Context)).Single();
            instance.CurrentStateName = transition.To;
            Execute(instance);
        }
        else if (currentState.Type == StateType.HumanTask)
        {
            instance.Status = WorkflowInstanceStatus.Waiting;
            _workflowTaskRepository.Add(new WorkflowTask
            {
                Role = currentState.HumanTask.Role,
                WorkflowInstanceId = instance.Id,
                Inputs = currentState.HumanTask.Inputs
            });

        }
        else if (currentState.Type == StateType.End)
        {
            foreach (var activity in currentState.Activities)
                activity.ExecuteAsync(instance.Context);

            instance.Status = WorkflowInstanceStatus.Completed;
        }

        _workflowRepository.Save(instance);
    }

    private async Task RunActivity(IWorkflowActivity act, WorkflowInstance instance)
    {
        var activity = (IWorkflowActivity)Activator.CreateInstance(Type.GetType(act.Name));
        await activity.ExecuteAsync(instance.Context);
    }

    private void SetCurrentStateDefinition(WorkflowInstance instance)
    {
        instance.Context.CurrentStateDefinition = _workflowRegistry[instance.WorkflowDefinitionId]
            .States[instance.CurrentStateName]
            ;
    }
}
