using System.Numerics;

namespace archerly.core.extensions;

// TODO: Upgrade to dotnet 10 and make this a static extension of the exception
public static class ArgumentNullExceptionExtension
{
    public static void ThrowIfInvalidEnum<TEnum, TValue>(TValue value, string paramName)
        where TEnum : struct, Enum
        where TValue : INumber<TValue>
    {
        Type underlying = Enum.GetUnderlyingType(typeof(TEnum));

        // Convert the value once to the underlying type
        object convertedValue = Convert.ChangeType(value, underlying);

        // Check if the converted value is defined in the enum
        if (!Enum.IsDefined(typeof(TEnum), convertedValue))
        {
            throw new ArgumentOutOfRangeException(
                paramName,
                value,
                $"Value {value} is not valid for enum {typeof(TEnum).Name}"
            );
        }
    }
}