namespace archerly.core.extensions;

public static class ArgumentExceptionExtension
{
    extension(ArgumentException)
    {
        public static void ThrowIfEmpty(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException(
                    "Value cannot be an empty GUID.",
                    nameof(value)
                );
            }
        }
    }
}