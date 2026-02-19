
namespace RuleEngine.Base;

public static class RuleSetRegistry
{
    private static readonly Dictionary<string, RuleSet> _ruleSets = new();

    public static void RegisterRuleSet(RuleSet ruleSet)
    {
        _ruleSets[ruleSet.Id] = ruleSet;
    }

    public static RuleSet GetRuleSet(string ruleSetId) 
    {
        if (_ruleSets.TryGetValue(ruleSetId, out var ruleSet))
            return ruleSet;
        return null;
    }
}
