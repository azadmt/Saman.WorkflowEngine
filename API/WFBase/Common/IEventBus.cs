namespace API.WFBase.Common
{
    public interface IEventBus
    {
        void Publish(string name, object data);
    }


    public class InMemoryEventBus : IEventBus
    {
        public void Publish(string name, object data)
        => Console.WriteLine($"Event: {name}");
    }
}
