using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Dirichlet.Numerics;

public struct Int128 : IFormattable, IComparable, IComparable<Int128>, IEquatable<Int128>, ISignedNumber<Int128>
{
    private UInt128 v;

    public static Int128 MinValue { get; } = (Int128)((UInt128)1 << 127);
    public static Int128 MaxValue { get; } = (Int128)(((UInt128)1 << 127) - 1);
    public static Int128 AdditiveIdentity { get; } = (Int128)0;
    public static Int128 MultiplicativeIdentity { get; } = (Int128)1;
    public static Int128 Zero { get; } = (Int128)0;
    public static Int128 One { get; } = (Int128)1;
    public static Int128 NegativeOne { get; } = (Int128)(-1);

    public ulong S0 => v.S0;
    public ulong S1 => v.S1;

    public bool IsZero => v.IsZero;
    public bool IsOne => v.IsOne;
    public bool IsPowerOfTwo => v.IsPowerOfTwo;
    public bool IsEven => v.IsEven;
    public bool IsNegative => v.S1 > long.MaxValue;
    public int Sign => IsNegative ? -1 : v.Sign;
    static Int128 INumber<Int128>.Sign(Int128 value) => (Int128)value.Sign;


    public override string ToString() => ((BigInteger)this).ToString();
    public string ToString(string format) => ((BigInteger)this).ToString(format);
    public string ToString(IFormatProvider provider) => ToString(null, provider);
    public string ToString(string? format, IFormatProvider? provider) => ((BigInteger)this).ToString(format, provider);
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => ((BigInteger)this).TryFormat(destination, out charsWritten, format, provider);


    #region Parsing
    public static Int128 Parse(string? value) => TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out Int128 c) ? c : throw new FormatException();
    public static Int128 Parse(string? value, IFormatProvider? format) => TryParse(value, NumberStyles.Integer, format, out Int128 c) ? c : throw new FormatException();
    public static Int128 Parse(string? value, NumberStyles style, IFormatProvider? format) => TryParse(value, style, format, out Int128 c) ? c : throw new FormatException();
    public static Int128 Parse(ReadOnlySpan<char> value, IFormatProvider? format) => TryParse(value, NumberStyles.Integer, format, out Int128 c) ? c : throw new FormatException();
    public static Int128 Parse(ReadOnlySpan<char> value, NumberStyles style, IFormatProvider? format) => TryParse(value, style, format, out Int128 c) ? c : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? value, out Int128 result) => TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
    public static bool TryParse([NotNullWhen(true)] string? value, IFormatProvider? format, out Int128 result) => TryParse(value, NumberStyles.Integer, format, out result);
    public static bool TryParse([NotNullWhen(true)] string? value, NumberStyles style, IFormatProvider? format, out Int128 result)
    {
        if (BigInteger.TryParse(value, style, format, out BigInteger a))
        {
            UInt128.Create(out result.v, a);
            return true;
        }
        result = Zero;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> value, IFormatProvider? format, out Int128 result) => TryParse(value, NumberStyles.Integer, format, out result);
    public static bool TryParse(ReadOnlySpan<char> value, NumberStyles style, IFormatProvider? format, out Int128 result)
    {
        if (BigInteger.TryParse(value, style, format, out BigInteger a))
        {
            UInt128.Create(out result.v, a);
            return true;
        }
        result = Zero;
        return false;
    }
    #endregion Parsing


    public Int128(long value) => UInt128.Create(out v, value);
    public Int128(ulong value) => UInt128.Create(out v, value);
    public Int128(double value) => UInt128.Create(out v, value);
    public Int128(decimal value) => UInt128.Create(out v, value);
    public Int128(BigInteger value) => UInt128.Create(out v, value);


    public static Int128 Create<TOther>(TOther value) where TOther : INumber<TOther> => new() { v = UInt128.Create(value) };
    public static bool TryCreate<TOther>(TOther value, out Int128 result) where TOther : INumber<TOther> => UInt128.TryCreate(value, out result.v);

    //TODO
    public static Int128 CreateSaturating<TOther>(TOther value) where TOther : INumber<TOther> => throw new NotImplementedException();
    //TODO
    public static Int128 CreateTruncating<TOther>(TOther value) where TOther : INumber<TOther> => throw new NotImplementedException();


    public static explicit operator Int128(double a)
    {
        Int128 c;
        UInt128.Create(out c.v, a);
        return c;
    }

    public static implicit operator Int128(sbyte a)
    {
        Int128 c;
        UInt128.Create(out c.v, a);
        return c;
    }

    public static implicit operator Int128(byte a)
    {
        Int128 c;
        UInt128.Create(out c.v, a);
        return c;
    }

    public static implicit operator Int128(short a)
    {
        Int128 c;
        UInt128.Create(out c.v, a);
        return c;
    }

    public static implicit operator Int128(ushort a)
    {
        Int128 c;
        UInt128.Create(out c.v, a);
        return c;
    }

    public static implicit operator Int128(int a)
    {
        Int128 c;
        UInt128.Create(out c.v, a);
        return c;
    }

    public static implicit operator Int128(uint a)
    {
        Int128 c;
        UInt128.Create(out c.v, (ulong)a);
        return c;
    }

    public static implicit operator Int128(long a)
    {
        Int128 c;
        UInt128.Create(out c.v, a);
        return c;
    }

    public static implicit operator Int128(ulong a)
    {
        Int128 c;
        UInt128.Create(out c.v, a);
        return c;
    }

    public static explicit operator Int128(decimal a)
    {
        Int128 c;
        UInt128.Create(out c.v, a);
        return c;
    }

    public static explicit operator Int128(UInt128 a)
    {
        Int128 c;
        c.v = a;
        return c;
    }

    public static explicit operator UInt128(Int128 a) => a.v;

    public static explicit operator Int128(BigInteger a)
    {
        Int128 c;
        UInt128.Create(out c.v, a);
        return c;
    }

    public static explicit operator sbyte(Int128 a) => (sbyte)a.v.S0;

    public static explicit operator byte(Int128 a) => (byte)a.v.S0;

    public static explicit operator short(Int128 a) => (short)a.v.S0;

    public static explicit operator ushort(Int128 a) => (ushort)a.v.S0;

    public static explicit operator int(Int128 a) => (int)a.v.S0;

    public static explicit operator uint(Int128 a) => (uint)a.v.S0;

    public static explicit operator long(Int128 a) => (long)a.v.S0;

    public static explicit operator ulong(Int128 a) => a.v.S0;

    public static explicit operator decimal(Int128 a)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 c, ref a.v);
            return -(decimal)c;
        }
        return (decimal)a.v;
    }

    public static implicit operator BigInteger(Int128 a)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 c, ref a.v);
            return -(BigInteger)c;
        }
        return (BigInteger)a.v;
    }

    public static explicit operator float(Int128 a)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 c, ref a.v);
            return -UInt128.ConvertToFloat(ref c);
        }
        return UInt128.ConvertToFloat(ref a.v);
    }

    public static explicit operator double(Int128 a)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 c, ref a.v);
            return -UInt128.ConvertToDouble(ref c);
        }
        return UInt128.ConvertToDouble(ref a.v);
    }

    public static Int128 operator <<(Int128 a, int b)
    {
        Int128 c;
        UInt128.LeftShift(out c.v, ref a.v, b);
        return c;
    }

    public static Int128 operator >>(Int128 a, int b)
    {
        Int128 c;
        UInt128.ArithmeticRightShift(out c.v, ref a.v, b);
        return c;
    }

    public static Int128 operator &(Int128 a, Int128 b)
    {
        Int128 c;
        UInt128.And(out c.v, ref a.v, ref b.v);
        return c;
    }

    public static int operator &(Int128 a, int b) => (int)(a.v & (uint)b);

    public static int operator &(int a, Int128 b) => (int)(b.v & (uint)a);

    public static long operator &(Int128 a, long b) => (long)(a.v & (ulong)b);

    public static long operator &(long a, Int128 b) => (long)(b.v & (ulong)a);

    public static Int128 operator |(Int128 a, Int128 b)
    {
        Int128 c;
        UInt128.Or(out c.v, ref a.v, ref b.v);
        return c;
    }

    public static Int128 operator ^(Int128 a, Int128 b)
    {
        Int128 c;
        UInt128.ExclusiveOr(out c.v, ref a.v, ref b.v);
        return c;
    }

    public static Int128 operator ~(Int128 a)
    {
        Int128 c;
        UInt128.Not(out c.v, ref a.v);
        return c;
    }

    public static Int128 operator +(Int128 a, long b)
    {
        Int128 c;
        if (b < 0)
            UInt128.Subtract(out c.v, ref a.v, (ulong)(-b));
        else
            UInt128.Add(out c.v, ref a.v, (ulong)b);
        return c;
    }

    public static Int128 operator +(long a, Int128 b)
    {
        Int128 c;
        if (a < 0)
            UInt128.Subtract(out c.v, ref b.v, (ulong)(-a));
        else
            UInt128.Add(out c.v, ref b.v, (ulong)a);
        return c;
    }

    public static Int128 operator +(Int128 a, Int128 b)
    {
        Int128 c;
        UInt128.Add(out c.v, ref a.v, ref b.v);
        return c;
    }

    public static Int128 operator ++(Int128 a)
    {
        Int128 c;
        UInt128.Add(out c.v, ref a.v, 1);
        return c;
    }

    public static Int128 operator -(Int128 a, long b)
    {
        Int128 c;
        if (b < 0)
            UInt128.Add(out c.v, ref a.v, (ulong)(-b));
        else
            UInt128.Subtract(out c.v, ref a.v, (ulong)b);
        return c;
    }

    public static Int128 operator -(Int128 a, Int128 b)
    {
        Int128 c;
        UInt128.Subtract(out c.v, ref a.v, ref b.v);
        return c;
    }

    public static Int128 operator +(Int128 a) => a;

    public static Int128 operator -(Int128 a)
    {
        Int128 c;
        UInt128.Negate(out c.v, ref a.v);
        return c;
    }

    public static Int128 operator --(Int128 a)
    {
        Int128 c;
        UInt128.Subtract(out c.v, ref a.v, 1);
        return c;
    }

    public static Int128 operator *(Int128 a, int b)
    {
        Multiply(out Int128 c, ref a, b);
        return c;
    }

    public static Int128 operator *(int a, Int128 b)
    {
        Multiply(out Int128 c, ref b, a);
        return c;
    }

    public static Int128 operator *(Int128 a, uint b)
    {
        Multiply(out Int128 c, ref a, b);
        return c;
    }

    public static Int128 operator *(uint a, Int128 b)
    {
        Multiply(out Int128 c, ref b, a);
        return c;
    }

    public static Int128 operator *(Int128 a, long b)
    {
        Multiply(out Int128 c, ref a, b);
        return c;
    }

    public static Int128 operator *(long a, Int128 b)
    {
        Multiply(out Int128 c, ref b, a);
        return c;
    }

    public static Int128 operator *(Int128 a, ulong b)
    {
        Multiply(out Int128 c, ref a, b);
        return c;
    }

    public static Int128 operator *(ulong a, Int128 b)
    {
        Multiply(out Int128 c, ref b, a);
        return c;
    }

    public static Int128 operator *(Int128 a, Int128 b)
    {
        Multiply(out Int128 c, ref a, ref b);
        return c;
    }

    public static Int128 operator /(Int128 a, int b)
    {
        Divide(out Int128 c, ref a, b);
        return c;
    }

    public static Int128 operator /(Int128 a, uint b)
    {
        Divide(out Int128 c, ref a, b);
        return c;
    }

    public static Int128 operator /(Int128 a, long b)
    {
        Divide(out Int128 c, ref a, b);
        return c;
    }

    public static Int128 operator /(Int128 a, ulong b)
    {
        Divide(out Int128 c, ref a, b);
        return c;
    }

    public static Int128 operator /(Int128 a, Int128 b)
    {
        Divide(out Int128 c, ref a, ref b);
        return c;
    }

    public static int operator %(Int128 a, int b) => Remainder(ref a, b);
    public static int operator %(Int128 a, uint b) => Remainder(ref a, b);
    public static long operator %(Int128 a, long b) => Remainder(ref a, b);
    public static long operator %(Int128 a, ulong b) => Remainder(ref a, b);
    public static Int128 operator %(Int128 a, Int128 b)
    {
        Remainder(out Int128 c, ref a, ref b);
        return c;
    }

    #region Comparison operators
    public static bool operator <(Int128 a, UInt128 b) => a.CompareTo(b) < 0;
    public static bool operator <(UInt128 a, Int128 b) => b.CompareTo(a) > 0;
    public static bool operator <(Int128 a, Int128 b) => LessThan(ref a.v, ref b.v);
    public static bool operator <(Int128 a, int b) => LessThan(ref a.v, b);
    public static bool operator <(int a, Int128 b) => LessThan(a, ref b.v);
    public static bool operator <(Int128 a, uint b) => LessThan(ref a.v, b);
    public static bool operator <(uint a, Int128 b) => LessThan(a, ref b.v);
    public static bool operator <(Int128 a, long b) => LessThan(ref a.v, b);
    public static bool operator <(long a, Int128 b) => LessThan(a, ref b.v);
    public static bool operator <(Int128 a, ulong b) => LessThan(ref a.v, b);
    public static bool operator <(ulong a, Int128 b) => LessThan(a, ref b.v);
    public static bool operator <=(Int128 a, UInt128 b) => a.CompareTo(b) <= 0;
    public static bool operator <=(UInt128 a, Int128 b) => b.CompareTo(a) >= 0;
    public static bool operator <=(Int128 a, Int128 b) => !LessThan(ref b.v, ref a.v);
    public static bool operator <=(Int128 a, int b) => !LessThan(b, ref a.v);
    public static bool operator <=(int a, Int128 b) => !LessThan(ref b.v, a);
    public static bool operator <=(Int128 a, uint b) => !LessThan(b, ref a.v);
    public static bool operator <=(uint a, Int128 b) => !LessThan(ref b.v, a);
    public static bool operator <=(Int128 a, long b) => !LessThan(b, ref a.v);
    public static bool operator <=(long a, Int128 b) => !LessThan(ref b.v, a);
    public static bool operator <=(Int128 a, ulong b) => !LessThan(b, ref a.v);
    public static bool operator <=(ulong a, Int128 b) => !LessThan(ref b.v, a);
    public static bool operator >(Int128 a, UInt128 b) => a.CompareTo(b) > 0;
    public static bool operator >(UInt128 a, Int128 b) => b.CompareTo(a) < 0;
    public static bool operator >(Int128 a, Int128 b) => LessThan(ref b.v, ref a.v);
    public static bool operator >(Int128 a, int b) => LessThan(b, ref a.v);
    public static bool operator >(int a, Int128 b) => LessThan(ref b.v, a);
    public static bool operator >(Int128 a, uint b) => LessThan(b, ref a.v);
    public static bool operator >(uint a, Int128 b) => LessThan(ref b.v, a);
    public static bool operator >(Int128 a, long b) => LessThan(b, ref a.v);
    public static bool operator >(long a, Int128 b) => LessThan(ref b.v, a);
    public static bool operator >(Int128 a, ulong b) => LessThan(b, ref a.v);
    public static bool operator >(ulong a, Int128 b) => LessThan(ref b.v, a);
    public static bool operator >=(Int128 a, UInt128 b) => a.CompareTo(b) >= 0;
    public static bool operator >=(UInt128 a, Int128 b) => b.CompareTo(a) <= 0;
    public static bool operator >=(Int128 a, Int128 b) => !LessThan(ref a.v, ref b.v);
    public static bool operator >=(Int128 a, int b) => !LessThan(ref a.v, b);
    public static bool operator >=(int a, Int128 b) => !LessThan(a, ref b.v);
    public static bool operator >=(Int128 a, uint b) => !LessThan(ref a.v, b);
    public static bool operator >=(uint a, Int128 b) => !LessThan(a, ref b.v);
    public static bool operator >=(Int128 a, long b) => !LessThan(ref a.v, b);
    public static bool operator >=(long a, Int128 b) => !LessThan(a, ref b.v);
    public static bool operator >=(Int128 a, ulong b) => !LessThan(ref a.v, b);
    public static bool operator >=(ulong a, Int128 b) => !LessThan(a, ref b.v);
    public static bool operator ==(UInt128 a, Int128 b) => b.Equals(a);
    public static bool operator ==(Int128 a, UInt128 b) => a.Equals(b);
    public static bool operator ==(Int128 a, Int128 b) => a.v.Equals(b.v);
    public static bool operator ==(Int128 a, int b) => a.Equals(b);
    public static bool operator ==(int a, Int128 b) => b.Equals(a);
    public static bool operator ==(Int128 a, uint b) => a.Equals(b);
    public static bool operator ==(uint a, Int128 b) => b.Equals(a);
    public static bool operator ==(Int128 a, long b) => a.Equals(b);
    public static bool operator ==(long a, Int128 b) => b.Equals(a);
    public static bool operator ==(Int128 a, ulong b) => a.Equals(b);
    public static bool operator ==(ulong a, Int128 b) => b.Equals(a);
    public static bool operator !=(UInt128 a, Int128 b) => !b.Equals(a);
    public static bool operator !=(Int128 a, UInt128 b) => !a.Equals(b);
    public static bool operator !=(Int128 a, Int128 b) => !a.v.Equals(b.v);
    public static bool operator !=(Int128 a, int b) => !a.Equals(b);
    public static bool operator !=(int a, Int128 b) => !b.Equals(a);
    public static bool operator !=(Int128 a, uint b) => !a.Equals(b);
    public static bool operator !=(uint a, Int128 b) => !b.Equals(a);
    public static bool operator !=(Int128 a, long b) => !a.Equals(b);
    public static bool operator !=(long a, Int128 b) => !b.Equals(a);
    public static bool operator !=(Int128 a, ulong b) => !a.Equals(b);
    public static bool operator !=(ulong a, Int128 b) => !b.Equals(a);
    #endregion Comparison operators

    public int CompareTo(UInt128 other) => IsNegative ? -1 : v.CompareTo(other);

    public int CompareTo(Int128 other) => SignedCompare(ref v, other.S0, other.S1);
    public int CompareTo(int other) => SignedCompare(ref v, (ulong)other, (ulong)(other >> 31));
    public int CompareTo(uint other) => SignedCompare(ref v, other, 0);
    public int CompareTo(long other) => SignedCompare(ref v, (ulong)other, (ulong)(other >> 63));
    public int CompareTo(ulong other) => SignedCompare(ref v, other, 0);
    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        Int128 => CompareTo((Int128)obj),
        _ => throw new ArgumentException(null, nameof(obj))
    };

    private static bool LessThan(ref UInt128 a, ref UInt128 b)
    {
        long as1 = (long)a.S1;
        long bs1 = (long)b.S1;
        return as1 != bs1 ? as1 < bs1 : a.S0 < b.S0;
    }

    private static bool LessThan(ref UInt128 a, long b)
    {
        long as1 = (long)a.S1;
        long bs1 = b >> 63;
        return as1 != bs1 ? as1 < bs1 : a.S0 < (ulong)b;
    }

    private static bool LessThan(long a, ref UInt128 b)
    {
        long as1 = a >> 63;
        long bs1 = (long)b.S1;
        return as1 != bs1 ? as1 < bs1 : (ulong)a < b.S0;
    }

    private static bool LessThan(ref UInt128 a, ulong b)
    {
        long as1 = (long)a.S1;
        return as1 != 0 ? as1 < 0 : a.S0 < b;
    }

    private static bool LessThan(ulong a, ref UInt128 b)
    {
        long bs1 = (long)b.S1;
        return 0 != bs1 ? 0 < bs1 : a < b.S0;
    }

    private static int SignedCompare(ref UInt128 a, ulong bs0, ulong bs1)
    {
        ulong as1 = a.S1;
        return as1 != bs1 ? ((long)as1).CompareTo((long)bs1) : a.S0.CompareTo(bs0);
    }

    public bool Equals(UInt128 other) => !IsNegative && v.Equals(other);

    public bool Equals(Int128 other) => v.Equals(other.v);

    public bool Equals(int other)
    {
        if (other < 0)
            return v.S1 == ulong.MaxValue && v.S0 == (uint)other;
        return v.S1 == 0 && v.S0 == (uint)other;
    }

    public bool Equals(uint other) => v.S1 == 0 && v.S0 == other;

    public bool Equals(long other)
    {
        if (other < 0)
            return v.S1 == ulong.MaxValue && v.S0 == (ulong)other;
        return v.S1 == 0 && v.S0 == (ulong)other;
    }

    public bool Equals(ulong other) => v.S1 == 0 && v.S0 == other;

    public override bool Equals(object? obj) => obj is Int128 n && Equals(n);

    public override int GetHashCode() => v.GetHashCode();

    public static void Multiply(out Int128 c, ref Int128 a, int b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            if (b < 0)
                UInt128.Multiply(out c.v, ref aneg, (uint)(-b));
            else
            {
                UInt128.Multiply(out c.v, ref aneg, (uint)b);
                UInt128.Negate(ref c.v);
            }
        }
        else
        {
            if (b < 0)
            {
                UInt128.Multiply(out c.v, ref a.v, (uint)(-b));
                UInt128.Negate(ref c.v);
            }
            else
                UInt128.Multiply(out c.v, ref a.v, (uint)b);
        }
        Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
    }

    public static void Multiply(out Int128 c, ref Int128 a, uint b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            UInt128.Multiply(out c.v, ref aneg, b);
            UInt128.Negate(ref c.v);
        }
        else
            UInt128.Multiply(out c.v, ref a.v, b);
        Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
    }

    public static void Multiply(out Int128 c, ref Int128 a, long b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            if (b < 0)
                UInt128.Multiply(out c.v, ref aneg, (ulong)(-b));
            else
            {
                UInt128.Multiply(out c.v, ref aneg, (ulong)b);
                UInt128.Negate(ref c.v);
            }
        }
        else
        {
            if (b < 0)
            {
                UInt128.Multiply(out c.v, ref a.v, (ulong)(-b));
                UInt128.Negate(ref c.v);
            }
            else
                UInt128.Multiply(out c.v, ref a.v, (ulong)b);
        }
        Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
    }

    public static void Multiply(out Int128 c, ref Int128 a, ulong b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            UInt128.Multiply(out c.v, ref aneg, b);
            UInt128.Negate(ref c.v);
        }
        else
            UInt128.Multiply(out c.v, ref a.v, b);
        Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
    }

    public static void Multiply(out Int128 c, ref Int128 a, ref Int128 b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            if (b.IsNegative)
            {
                UInt128.Negate(out UInt128 bneg, ref b.v);
                UInt128.Multiply(out c.v, ref aneg, ref bneg);
            }
            else
            {
                UInt128.Multiply(out c.v, ref aneg, ref b.v);
                UInt128.Negate(ref c.v);
            }
        }
        else
        {
            if (b.IsNegative)
            {
                UInt128.Negate(out UInt128 bneg, ref b.v);
                UInt128.Multiply(out c.v, ref a.v, ref bneg);
                UInt128.Negate(ref c.v);
            }
            else
                UInt128.Multiply(out c.v, ref a.v, ref b.v);
        }
        Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
    }

    public static void Divide(out Int128 c, ref Int128 a, int b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            if (b < 0)
                UInt128.Multiply(out c.v, ref aneg, (uint)(-b));
            else
            {
                UInt128.Multiply(out c.v, ref aneg, (uint)b);
                UInt128.Negate(ref c.v);
            }
        }
        else
        {
            if (b < 0)
            {
                UInt128.Multiply(out c.v, ref a.v, (uint)(-b));
                UInt128.Negate(ref c.v);
            }
            else
                UInt128.Multiply(out c.v, ref a.v, (uint)b);
        }
        Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
    }

    public static void Divide(out Int128 c, ref Int128 a, uint b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            UInt128.Divide(out c.v, ref aneg, b);
            UInt128.Negate(ref c.v);
        }
        else
            UInt128.Divide(out c.v, ref a.v, b);
        Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
    }

    public static void Divide(out Int128 c, ref Int128 a, long b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            if (b < 0)
                UInt128.Divide(out c.v, ref aneg, (ulong)(-b));
            else
            {
                UInt128.Divide(out c.v, ref aneg, (ulong)b);
                UInt128.Negate(ref c.v);
            }
        }
        else
        {
            if (b < 0)
            {
                UInt128.Divide(out c.v, ref a.v, (ulong)(-b));
                UInt128.Negate(ref c.v);
            }
            else
                UInt128.Divide(out c.v, ref a.v, (ulong)b);
        }
        Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
    }

    public static void Divide(out Int128 c, ref Int128 a, ulong b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            UInt128.Divide(out c.v, ref aneg, b);
            UInt128.Negate(ref c.v);
        }
        else
            UInt128.Divide(out c.v, ref a.v, b);
        Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
    }

    public static void Divide(out Int128 c, ref Int128 a, ref Int128 b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            if (b.IsNegative)
            {
                UInt128.Negate(out UInt128 bneg, ref b.v);
                UInt128.Divide(out c.v, ref aneg, ref bneg);
            }
            else
            {
                UInt128.Divide(out c.v, ref aneg, ref b.v);
                UInt128.Negate(ref c.v);
            }
        }
        else
        {
            if (b.IsNegative)
            {
                UInt128.Negate(out UInt128 bneg, ref b.v);
                UInt128.Divide(out c.v, ref a.v, ref bneg);
                UInt128.Negate(ref c.v);
            }
            else
                UInt128.Divide(out c.v, ref a.v, ref b.v);
        }
        Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
    }

    public static int Remainder(ref Int128 a, int b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            return b < 0 ? (int)UInt128.Remainder(ref aneg, (uint)-b) : -(int)UInt128.Remainder(ref aneg, (uint)b);
        }
        else
        {
            return b < 0 ? -(int)UInt128.Remainder(ref a.v, (uint)-b) : (int)UInt128.Remainder(ref a.v, (uint)b);
        }
    }

    public static int Remainder(ref Int128 a, uint b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            return -(int)UInt128.Remainder(ref aneg, b);
        }
        else
            return (int)UInt128.Remainder(ref a.v, b);
    }

    public static long Remainder(ref Int128 a, long b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            return b < 0 ? (long)UInt128.Remainder(ref aneg, (ulong)-b) : -(long)UInt128.Remainder(ref aneg, (ulong)b);
        }
        else
        {
            return b < 0 ? -(long)UInt128.Remainder(ref a.v, (ulong)-b) : (long)UInt128.Remainder(ref a.v, (ulong)b);
        }
    }

    public static long Remainder(ref Int128 a, ulong b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            return -(long)UInt128.Remainder(ref aneg, b);
        }
        else
            return (long)UInt128.Remainder(ref a.v, b);
    }

    public static void Remainder(out Int128 c, ref Int128 a, ref Int128 b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            if (b.IsNegative)
            {
                UInt128.Negate(out UInt128 bneg, ref b.v);
                UInt128.Remainder(out c.v, ref aneg, ref bneg);
            }
            else
            {
                UInt128.Remainder(out c.v, ref aneg, ref b.v);
                UInt128.Negate(ref c.v);
            }
        }
        else
        {
            if (b.IsNegative)
            {
                UInt128.Negate(out UInt128 bneg, ref b.v);
                UInt128.Remainder(out c.v, ref a.v, ref bneg);
                UInt128.Negate(ref c.v);
            }
            else
                UInt128.Remainder(out c.v, ref a.v, ref b.v);
        }
        Debug.Assert((BigInteger)c == (BigInteger)a % (BigInteger)b);
    }

    public static Int128 Abs(Int128 a)
    {
        if (!a.IsNegative)
            return a;
        Int128 c;
        UInt128.Negate(out c.v, ref a.v);
        return c;
    }

    public static Int128 Square(long a)
    {
        if (a < 0)
            a = -a;
        Int128 c;
        UInt128.Square(out c.v, (ulong)a);
        return c;
    }

    public static Int128 Square(Int128 a)
    {
        Int128 c;
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            UInt128.Square(out c.v, ref aneg);
        }
        else
            UInt128.Square(out c.v, ref a.v);
        return c;
    }

    public static Int128 Cube(long a)
    {
        Int128 c;
        if (a < 0)
        {
            UInt128.Cube(out c.v, (ulong)-a);
            UInt128.Negate(ref c.v);
        }
        else
            UInt128.Cube(out c.v, (ulong)a);
        return c;
    }

    public static Int128 Cube(Int128 a)
    {
        Int128 c;
        if (a < 0)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            UInt128.Cube(out c.v, ref aneg);
            UInt128.Negate(ref c.v);
        }
        else
            UInt128.Cube(out c.v, ref a.v);
        return c;
    }

    public static void Add(ref Int128 a, long b)
    {
        if (b < 0)
            UInt128.Subtract(ref a.v, (ulong)-b);
        else
            UInt128.Add(ref a.v, (ulong)b);
    }

    public static void Add(ref Int128 a, ref Int128 b) => UInt128.Add(ref a.v, ref b.v);

    public static void Subtract(ref Int128 a, long b)
    {
        if (b < 0)
            UInt128.Add(ref a.v, (ulong)-b);
        else
            UInt128.Subtract(ref a.v, (ulong)b);
    }

    public static void Subtract(ref Int128 a, ref Int128 b) => UInt128.Subtract(ref a.v, ref b.v);

    public static void Add(ref Int128 a, Int128 b) => UInt128.Add(ref a.v, ref b.v);

    public static void Subtract(ref Int128 a, Int128 b) => UInt128.Subtract(ref a.v, ref b.v);

    public static void AddProduct(ref Int128 a, ref UInt128 b, int c)
    {
        if (c < 0)
        {
            UInt128.Multiply(out UInt128 product, ref b, (uint)-c);
            UInt128.Subtract(ref a.v, ref product);
        }
        else
        {
            UInt128.Multiply(out UInt128 product, ref b, (uint)c);
            UInt128.Add(ref a.v, ref product);
        }
    }

    public static void AddProduct(ref Int128 a, ref UInt128 b, long c)
    {
        if (c < 0)
        {
            UInt128.Multiply(out UInt128 product, ref b, (ulong)-c);
            UInt128.Subtract(ref a.v, ref product);
        }
        else
        {
            UInt128.Multiply(out UInt128 product, ref b, (ulong)c);
            UInt128.Add(ref a.v, ref product);
        }
    }

    public static void SubtractProduct(ref Int128 a, ref UInt128 b, int c)
    {
        if (c < 0)
        {
            UInt128.Multiply(out UInt128 d, ref b, (uint)-c);
            UInt128.Add(ref a.v, ref d);
        }
        else
        {
            UInt128.Multiply(out UInt128 d, ref b, (uint)c);
            UInt128.Subtract(ref a.v, ref d);
        }
    }

    public static void SubtractProduct(ref Int128 a, ref UInt128 b, long c)
    {
        if (c < 0)
        {
            UInt128.Multiply(out UInt128 d, ref b, (ulong)-c);
            UInt128.Add(ref a.v, ref d);
        }
        else
        {
            UInt128.Multiply(out UInt128 d, ref b, (ulong)c);
            UInt128.Subtract(ref a.v, ref d);
        }
    }

    public static void AddProduct(ref Int128 a, UInt128 b, int c) => AddProduct(ref a, ref b, c);

    public static void AddProduct(ref Int128 a, UInt128 b, long c) => AddProduct(ref a, ref b, c);

    public static void SubtractProduct(ref Int128 a, UInt128 b, int c) => SubtractProduct(ref a, ref b, c);

    public static void SubtractProduct(ref Int128 a, UInt128 b, long c) => SubtractProduct(ref a, ref b, c);

    public static void Pow(out Int128 result, ref Int128 value, int exponent)
    {
        if (exponent < 0)
            throw new ArgumentException("exponent must not be negative");
        if (value.IsNegative)
        {
            UInt128.Negate(out UInt128 valueneg, ref value.v);
            if ((exponent & 1) == 0)
                UInt128.Pow(out result.v, ref valueneg, (uint)exponent);
            else
            {
                UInt128.Pow(out result.v, ref valueneg, (uint)exponent);
                UInt128.Negate(ref result.v);
            }
        }
        else
            UInt128.Pow(out result.v, ref value.v, (uint)exponent);
    }

    public static Int128 Pow(Int128 value, int exponent)
    {
        Pow(out Int128 result, ref value, exponent);
        return result;
    }

    public static ulong FloorSqrt(Int128 a) => a.IsNegative ? throw new ArgumentException("argument must not be negative", nameof(a)) : UInt128.FloorSqrt(a.v);

    public static ulong CeilingSqrt(Int128 a) => a.IsNegative ? throw new ArgumentException("argument must not be negative", nameof(a)) : UInt128.CeilingSqrt(a.v);

    public static long FloorCbrt(Int128 a)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            return -(long)UInt128.FloorCbrt(aneg);
        }
        return (long)UInt128.FloorCbrt(a.v);
    }

    public static long CeilingCbrt(Int128 a)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            return -(long)UInt128.CeilingCbrt(aneg);
        }
        return (long)UInt128.CeilingCbrt(a.v);
    }

    public static Int128 Min(Int128 a, Int128 b) => LessThan(ref a.v, ref b.v) ? a : b;

    public static Int128 Max(Int128 a, Int128 b) => LessThan(ref b.v, ref a.v) ? a : b;

    public static Int128 Clamp(Int128 a, Int128 min, Int128 max) => LessThan(ref a.v, ref min.v) ? min : (LessThan(ref max.v, ref a.v) ? max : a);

    public static double Log(Int128 a) => Log(a, Math.E);

    public static double Log10(Int128 a) => Log(a, 10);

    public static double Log(Int128 a, double b) => a.IsNegative || a.IsZero
            ? throw new ArgumentException("argument must be positive", nameof(a))
            : Math.Log(UInt128.ConvertToDouble(ref a.v), b);

    public static Int128 Add(Int128 a, Int128 b)
    {
        Int128 c;
        UInt128.Add(out c.v, ref a.v, ref b.v);
        return c;
    }

    public static Int128 Subtract(Int128 a, Int128 b)
    {
        Int128 c;
        UInt128.Subtract(out c.v, ref a.v, ref b.v);
        return c;
    }

    public static Int128 Multiply(Int128 a, Int128 b)
    {
        Multiply(out Int128 c, ref a, ref b);
        return c;
    }

    public static Int128 Divide(Int128 a, Int128 b)
    {
        Divide(out Int128 c, ref a, ref b);
        return c;
    }

    public static Int128 Remainder(Int128 a, Int128 b)
    {
        Remainder(out Int128 c, ref a, ref b);
        return c;
    }

    public static Int128 DivRem(Int128 a, Int128 b, out Int128 remainder)
    {
        //TODO: Yuck!
        Divide(out Int128 c, ref a, ref b);
        Remainder(out remainder, ref a, ref b);
        return c;
    }

    public static (Int128 Quotient, Int128 Remainder) DivRem(Int128 a, Int128 b)
    {
        Int128 quotient = DivRem(a, b, out Int128 remainder);
        return (quotient, remainder);
    }

    public static Int128 Negate(Int128 a)
    {
        Int128 c;
        UInt128.Negate(out c.v, ref a.v);
        return c;
    }

    public static Int128 GreatestCommonDivisor(Int128 a, Int128 b)
    {
        GreatestCommonDivisor(out Int128 c, ref a, ref b);
        return c;
    }

    public static void GreatestCommonDivisor(out Int128 c, ref Int128 a, ref Int128 b)
    {
        if (a.IsNegative)
        {
            UInt128.Negate(out UInt128 aneg, ref a.v);
            if (b.IsNegative)
            {
                UInt128.Negate(out UInt128 bneg, ref b.v);
                UInt128.GreatestCommonDivisor(out c.v, ref aneg, ref bneg);
            }
            else
                UInt128.GreatestCommonDivisor(out c.v, ref aneg, ref b.v);
        }
        else
        {
            if (b.IsNegative)
            {
                UInt128.Negate(out UInt128 bneg, ref b.v);
                UInt128.GreatestCommonDivisor(out c.v, ref a.v, ref bneg);
            }
            else
                UInt128.GreatestCommonDivisor(out c.v, ref a.v, ref b.v);
        }
    }

    public static void LeftShift(ref Int128 c, int d) => UInt128.LeftShift(ref c.v, d);

    public static void LeftShift(ref Int128 c) => UInt128.LeftShift(ref c.v);

    public static void RightShift(ref Int128 c, int d) => UInt128.ArithmeticRightShift(ref c.v, d);

    public static void RightShift(ref Int128 c) => UInt128.ArithmeticRightShift(ref c.v);

    public static void Swap(ref Int128 a, ref Int128 b) => UInt128.Swap(ref a.v, ref b.v);

    public static int Compare(Int128 a, Int128 b) => a.CompareTo(b);

    public static void Shift(out Int128 c, ref Int128 a, int d) => UInt128.ArithmeticShift(out c.v, ref a.v, d);

    public static void Shift(ref Int128 c, int d) => UInt128.ArithmeticShift(ref c.v, d);

    public static Int128 ModAdd(Int128 a, Int128 b, Int128 modulus)
    {
        Int128 c;
        UInt128.ModAdd(out c.v, ref a.v, ref b.v, ref modulus.v);
        return c;
    }

    public static Int128 ModSub(Int128 a, Int128 b, Int128 modulus)
    {
        Int128 c;
        UInt128.ModSub(out c.v, ref a.v, ref b.v, ref modulus.v);
        return c;
    }

    public static Int128 ModMul(Int128 a, Int128 b, Int128 modulus)
    {
        Int128 c;
        UInt128.ModMul(out c.v, ref a.v, ref b.v, ref modulus.v);
        return c;
    }

    public static Int128 ModPow(Int128 value, Int128 exponent, Int128 modulus)
    {
        Int128 result;
        UInt128.ModPow(out result.v, ref value.v, ref exponent.v, ref modulus.v);
        return result;
    }



}
