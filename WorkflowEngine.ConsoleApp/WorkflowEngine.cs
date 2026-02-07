using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WorkflowEngine.ConsoleApp;

public class WorkflowEngine
{
    private readonly IDictionary<string, WorkflowDefinition> _definitions;


    public WorkflowEngine(IEnumerable<WorkflowDefinition> definitions)
    {
        _definitions = new Dictionary<string, WorkflowDefinition>();
        foreach (var d in definitions)
            _definitions[d.Id] = d;
    }


    public WorkflowInstance Start(string workflowId, Dictionary<string, object> input)
    {
        var def = _definitions[workflowId];
        var instance = new WorkflowInstance
        {
            WorkflowDefinitionId = def.Id,
            WorkflowVersion = def.Version,
            CurrentStateId = def.StartStateId
        };


        foreach (var kv in input)
            instance.Variables[kv.Key] = JsonSerializer.SerializeToElement(kv.Value);


        Execute(instance);
        return instance;
    }
    public void Execute(WorkflowInstance instance)
    {
        var def = _definitions[instance.WorkflowDefinitionId];
        var state = def.States[instance.CurrentStateId];


        if (state.Type == StateType.Automatic)
        {
            foreach (var act in state.Activities)
                RunActivity(act, instance);


            var transition = state.Transitions[0];
            instance.CurrentStateId = transition.To;
            Execute(instance);
        }
        else if (state.Type == StateType.HumanTask)
        {
            instance.Status = WorkflowInstanceStatus.Waiting;
            Console.WriteLine($"Waiting for role: {state.HumanTask!.Role}");
        }
        else if (state.Type == StateType.End)
        {
            foreach (var act in state.Activities)
                RunActivity(act, instance);


            instance.Status = WorkflowInstanceStatus.Completed;
        }
    }
    public void Resume(Guid instanceId, string eventName, Dictionary<string, object> data, WorkflowInstance instance)
    {
        var def = _definitions[instance.WorkflowDefinitionId];
        var state = def.States[instance.CurrentStateId];


        foreach (var kv in data)
            instance.Variables[kv.Key] = JsonSerializer.SerializeToElement(kv.Value);


        var transition = state.Transitions.Find(t => t.Event == eventName)!;
        instance.CurrentStateId = transition.To;
        instance.Status = WorkflowInstanceStatus.Running;


        Execute(instance);
    }
    private void RunActivity(ActivityDefinition act, WorkflowInstance instance)
    {
        Console.WriteLine($"Activity executed: {act.Type} ({act.Reference})");
    }
}
