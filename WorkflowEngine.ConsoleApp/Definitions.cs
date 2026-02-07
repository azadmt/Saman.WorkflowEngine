using System.Text.Json;

public enum WorkflowInstanceStatus { Running, Waiting, Completed, Rejected }
public enum StateType { Start, Automatic, HumanTask, End }


public class WorkflowDefinition
{
    public string Id { get; set; } = default!;
    public int Version { get; set; }
    public string StartStateId { get; set; } = default!;
    public Dictionary<string, StateDefinition> States { get; set; } = new();
}


public class StateDefinition
{
    public string Id { get; set; } = default!;
    public StateType Type { get; set; }
    public List<ActivityDefinition> Activities { get; set; } = new();
    public List<TransitionDefinition> Transitions { get; set; } = new();
    public HumanTaskDefinition? HumanTask { get; set; }
}


public class ActivityDefinition
{
    public string Type { get; set; } = default!;
    public string? Reference { get; set; }
}

public class TransitionDefinition
{
    public string? Event { get; set; }
    public string To { get; set; } = default!;
}


public class HumanTaskDefinition
{
    public string Role { get; set; } = default!;
    public string UiContract { get; set; } = default!;
}


public class WorkflowInstance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string WorkflowDefinitionId { get; set; } = default!;
    public int WorkflowVersion { get; set; }
    public string CurrentStateId { get; set; } = default!;
    public WorkflowInstanceStatus Status { get; set; } = WorkflowInstanceStatus.Running;
    public Dictionary<string, JsonElement> Variables { get; set; } = new();
}