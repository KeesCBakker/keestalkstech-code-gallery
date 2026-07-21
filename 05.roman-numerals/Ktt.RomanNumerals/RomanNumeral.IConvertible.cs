using System.Globalization;

namespace Ktt.RomanNumerals;

public partial class RomanNumeral : IConvertible
{
    TypeCode IConvertible.GetTypeCode() => TypeCode.Int32;

    bool IConvertible.ToBoolean(IFormatProvider? provider) => Convert.ToBoolean(Number, provider);

    byte IConvertible.ToByte(IFormatProvider? provider) => Convert.ToByte(Number, provider);

    char IConvertible.ToChar(IFormatProvider? provider) => Convert.ToChar(Number, provider);

    DateTime IConvertible.ToDateTime(IFormatProvider? provider) => Convert.ToDateTime(Number, provider);

    decimal IConvertible.ToDecimal(IFormatProvider? provider) => Number;

    double IConvertible.ToDouble(IFormatProvider? provider) => Number;

    short IConvertible.ToInt16(IFormatProvider? provider) => Convert.ToInt16(Number, provider);

    int IConvertible.ToInt32(IFormatProvider? provider) => Number;

    long IConvertible.ToInt64(IFormatProvider? provider) => Number;

    sbyte IConvertible.ToSByte(IFormatProvider? provider) => Convert.ToSByte(Number, provider);

    float IConvertible.ToSingle(IFormatProvider? provider) => Number;

    string IConvertible.ToString(IFormatProvider? provider) => ToString();

    object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
    {
        if (conversionType == typeof(RomanNumeral))
        {
            return this;
        }

        return Convert.ChangeType(Number, conversionType, provider ?? CultureInfo.CurrentCulture);
    }

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => Convert.ToUInt16(Number, provider);

    uint IConvertible.ToUInt32(IFormatProvider? provider) => Convert.ToUInt32(Number, provider);

    ulong IConvertible.ToUInt64(IFormatProvider? provider) => Convert.ToUInt64(Number, provider);
}
