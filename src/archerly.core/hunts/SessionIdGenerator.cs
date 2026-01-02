namespace archerly.core.hunts;

public class SessionIdGenerator
{
    private static readonly char[] _chars =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_".ToCharArray();

    private readonly int _length = 4;
    private int[] _indices; // holds base-64 digits

    // Constructor: start from beginning
    public SessionIdGenerator()
    {
        _indices = new int[_length];
        // all zeros, corresponds to "AAAA"
    }

    /// <summary>
    /// Restart generator from a specific last session ID.
    /// </summary>
    public void RestartFrom(string lastSessionId)
    {
        if (lastSessionId.Length != _length)
        {
            throw new ArgumentException($"Session ID must be {_length} chars long.", nameof(lastSessionId));
        }

        _indices = lastSessionId.Select(c =>
        {
            int index = Array.IndexOf(_chars, c);
            if (index < 0)
            {
                throw new ArgumentException($"Invalid character '{c}' in session ID.");
            }
            return index;
        }).ToArray();
    }

    /// <summary>
    /// Returns the next session ID.
    /// </summary>
    public string Next()
    {
        // Increment base-64 number
        for (int i = _length - 1; i >= 0; i--)
        {
            _indices[i]++;
            if (_indices[i] < 64)
            {
                break;
            }
            _indices[i] = 0; // carry
        }

        return new string(_indices.Select(i => _chars[i]).ToArray());
    }
}