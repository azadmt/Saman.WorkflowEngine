public class HumanTaskDefinition
{
    public string Role { get; set; } = default!;
    public string UiContract { get; set; } = default!;
    public List<TaskInput> Inputs{ get; set; } = new();
}

public class TaskInput
{
    public string Name { get; set; }
    public string Type { get; set; }
}
