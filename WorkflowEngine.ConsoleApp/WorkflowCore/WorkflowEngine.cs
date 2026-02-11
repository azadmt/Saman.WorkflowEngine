using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace WorkflowCore;

public class WorkflowEngine
{
    private Dictionary<string, WorkflowDefinition> _definitions = new();
    private readonly WorkflowRepository _workflowEngineRepository;
    private readonly WorkflowTaskRepository _workflowTaskRepository;

    public WorkflowEngine(WorkflowRepository workflowEngineRepository, WorkflowTaskRepository workflowTaskRepository)
    {
        _workflowEngineRepository = workflowEngineRepository;
        _workflowTaskRepository = workflowTaskRepository;
    }

    public void RegisterWorkflow(IEnumerable<WorkflowDefinition> definitions)
    {
        foreach (var d in definitions)
        {
            _definitions[d.GetId()] = d;
        }
    }

    public WorkflowInstance Start(string workflowName, int workflowVersion, Dictionary<string, object> input)
    {
        var def = _definitions[$"{workflowName}-V{workflowVersion}"];
        var instance = new WorkflowInstance
        {
            WorkflowDefinitionId = def.GetId(),
            CurrentStateId = def.StartState
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
        var workflowInstance = _workflowEngineRepository.Get(instanceId);
        var state = GetCurrentStateDefinition(workflowInstance);

        foreach (var kv in data)
            workflowInstance.Context.SetData(kv.Key, kv.Value);
        //instance.Context.SetData(kv.Key, JsonSerializer.SerializeToElement(kv.Value));

        var transition = state.Transitions.Find(t => t.Event == eventName)!;
        workflowInstance.CurrentStateId = transition.To;
        workflowInstance.Status = WorkflowInstanceStatus.Running;

        Execute(workflowInstance);
    }

    private void Execute(WorkflowInstance instance)
    {
        var state = GetCurrentStateDefinition(instance);

        if (state.Type == StateType.Automatic)
        {
            foreach (var activity in state.Activities)
            {
                activity.ExecuteAsync(instance.Context);
            }

            var transition = state.Transitions[0];
            instance.CurrentStateId = transition.To;
            Execute(instance);
        }
        else if (state.Type == StateType.HumanTask)
        {
            instance.Status = WorkflowInstanceStatus.Waiting;
            _workflowTaskRepository.Add(new WorkflowTask
            {
                Role = state.HumanTask.Role,
                WorkflowInstanceId = instance.Id,
                Inputs= state.HumanTask.Inputs
            });
        
        }
        else if (state.Type == StateType.End)
        {
            foreach (var activity in state.Activities)
                activity.ExecuteAsync(instance.Context);

            instance.Status = WorkflowInstanceStatus.Completed;
        }

        _workflowEngineRepository.Save(instance);
    }

    private async Task RunActivity(WorkflowActivity act, WorkflowInstance instance)
    {
        var activity = (WorkflowActivity)Activator.CreateInstance(Type.GetType(act.Name));
        await activity.ExecuteAsync(instance.Context);
    }

    private StateDefinition GetCurrentStateDefinition(WorkflowInstance instance)
    {
        return _definitions[instance.WorkflowDefinitionId].States[instance.CurrentStateId];
    }
}
