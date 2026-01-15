namespace archerly.core;

public interface IDetailProvider
{
    public IDictionary<string, object?> Details { get; }
}