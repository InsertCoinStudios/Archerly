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

    public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> predicate)
            where TKey : notnull
    {
        var keysToRemove = dict.Where(predicate).Select(kvp => kvp.Key).ToList();
        foreach (var key in keysToRemove)
        {
            dict.Remove(key);
        }
    }
}