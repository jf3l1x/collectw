namespace CollectW.Sinks.StatsD
{
    public interface IResolveCounterType
    {
        StatsDTypes Resolve(string counterIdentifier);
    }
}