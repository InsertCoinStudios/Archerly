using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace archerly.core.extensions;

public static class EnumExtensions
{
    public static bool TryToEnum<TEnum, TValue>(this TValue value, [NotNullWhen(true)] out TEnum result)
            where TEnum : struct, Enum
            where TValue : INumber<TValue>
    {
        try
        {
            Type underlying = Enum.GetUnderlyingType(typeof(TEnum));

            // Convert value to the underlying enum type
            object converted = Convert.ChangeType(value, underlying);

            if (Enum.IsDefined(typeof(TEnum), converted))
            {
                result = (TEnum)converted;
                return true;
            }

            throw new Exception();
        }
        catch
        {
            result = default;
            return false;
        }
    }

    public static TEnum ToEnum<TEnum, TValue>(this TValue value)
        where TEnum : struct, Enum
        where TValue : INumber<TValue>
    {
        // TODO: Upgrade to dotnet 10
        ArgumentNullExceptionExtension.ThrowIfInvalidEnum<TEnum, TValue>(value, nameof(value));
        Type underlying = Enum.GetUnderlyingType(typeof(TEnum));
        object converted = Convert.ChangeType(value, underlying);
        return (TEnum)converted;
    }
}