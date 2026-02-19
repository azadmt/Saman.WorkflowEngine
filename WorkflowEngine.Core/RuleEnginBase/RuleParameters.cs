namespace RuleEngine.Base;
public class RuleParameters : Dictionary<string, object>
{
    public RuleParameters WithParam(string key, object value)
    {
        this[key] = value;
        return this;
    }

    /// <summary>
    /// خواندن پارامتر با تبدیل نوع (Type Safe)
    /// </summary>
    public T Get<T>(string key)
    {
        if (TryGetValue(key, out var value))
            return (T)value;
        throw new KeyNotFoundException($"Parameter '{key}' not found in rule parameters");
    }
}
