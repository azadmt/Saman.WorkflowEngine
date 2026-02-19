namespace RuleEngine.Base;
public class RuleBinding
{
    public IRule Rule { get; set; }
    public RuleParameters Parameters { get; set; } = new();

    public int Priority { get; set; } = 0;
}