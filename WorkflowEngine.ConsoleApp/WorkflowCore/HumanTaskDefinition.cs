public class HumanTaskDefinition
{
    public string Role { get; set; } = default!;
    public string UiContract { get; set; } = default!;
    public List<TaskInput> Inputs{ get; set; } = new();
}

public class TaskInput
{
    public string Name { get; set; }
    public string Lable { get; set; }
    public InputType Type { get; set; }
    public List<KeyValuePair<string, string>> Options { get; set; } = new();

}

public enum InputType
{
    Number,
    Text,
    Date,
    CheckBox,
    Radio,
    Dropdown
}

