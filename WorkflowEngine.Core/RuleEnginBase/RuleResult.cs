namespace RuleEngine.Base;
public class RuleResult
{
    public string Name { get; set; }
    public bool Passed { get; set; }
    public RuleSeverity Severity { get; set; }
    public List<string> Messages { get; set; } = new();

}
