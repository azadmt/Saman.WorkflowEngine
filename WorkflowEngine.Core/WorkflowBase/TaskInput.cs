namespace WorkflowBase;

public class TaskInput
{
    public string Name { get; set; }
    public string Lable { get; set; }//عنوان فارسی
    public InputType Type { get; set; }
    public List<KeyValuePair<string, string>> Options { get; set; } = new();

}

