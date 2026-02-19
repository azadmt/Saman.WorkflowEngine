namespace WorkflowEngine.Core.Common
{
    public interface IDataContext
    {
        T? GetData<T>(string key);
        void SetData(string key, object value);
    }
}
