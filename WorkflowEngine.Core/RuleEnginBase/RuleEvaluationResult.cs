namespace RuleEngine.Base;
public class RuleEvaluationResult
{
    public bool AllPassed { get; set; }
    public bool HasBlockers { get; set; }
    public bool HasWarnings { get; set; }
    public List<RuleResult> Results { get; set; }
}
