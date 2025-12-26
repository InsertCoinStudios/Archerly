using System.Numerics;

namespace archerly.core.extensions;

public static class DictionaryExtensions
{
    public static void AddToCount<TKey, TCounter>(this IDictionary<TKey, TCounter> dictionary, TKey key, TCounter value)
            where TKey : notnull
            where TCounter : INumber<TCounter>
    {
        if (dictionary.TryGetValue(key, out var current))
        {
            dictionary[key] = current + value;
        }
        else
        {
            dictionary[key] = value;
        }
    }
}