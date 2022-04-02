using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace Dirichlet.Numerics;

public struct UInt128 : IFormattable, IComparable, IComparable<UInt128>, IEquatable<UInt128>, IUnsignedNumber<UInt128>
{
    private struct UInt256
    {
        public ulong s0;
        public ulong s1;
        public ulong s2;
        public ulong s3;

        public uint R0 => (uint)s0;
        public uint R1 => (uint)(s0 >> 32);
        public uint R2 => (uint)s1;
        public uint R3 => (uint)(s1 >> 32);
        public uint R4 => (uint)s2;
        public uint R5 => (uint)(s2 >> 32);
        public uint R6 => (uint)s3;
        public uint R7 => (uint)(s3 >> 32);

        public UInt128 T0 { get { Create(out var result, s0, s1); return result; } }
        public UInt128 T1 { get { Create(out var result, s2, s3); return result; } }

        public static implicit operator BigInteger(UInt256 a) => (BigInteger)a.s3 << 192 | (BigInteger)a.s2 << 128 | (BigInteger)a.s1 << 64 | a.s0;

    }

#pragma warning disable IDE0032 // Use auto property
    private ulong s0;
    private ulong s1;
#pragma warning restore IDE0032 // Use auto property

    public static UInt128 MinValue => Zero;
    public static UInt128 MaxValue { get; } = ~(UInt128)0;
    public static UInt128 AdditiveIdentity { get; } = (UInt128)0;
    public static UInt128 MultiplicativeIdentity { get; } = (UInt128)1;
    public static UInt128 Zero { get; } = (UInt128)0;
    public static UInt128 One { get; } = (UInt128)1;

    private uint R0 => (uint)s0;
    private uint R1 => (uint)(s0 >> 32);
    private uint R2 => (uint)s1;
    private uint R3 => (uint)(s1 >> 32);

    public ulong S0 => s0;
    public ulong S1 => s1;

    public bool IsZero => (s0 | s1) == 0;
    public bool IsOne => s1 == 0 && s0 == 1;
    public bool IsPowerOfTwo => (this & (this - 1)).IsZero;
    public bool IsEven => (s0 & 1) == 0;
    public int Sign => IsZero ? 0 : 1;

   // static
    UInt128 INumber<UInt128>.Sign(UInt128 value) => value.IsZero ? Zero : One;

    public override string ToString() => ((BigInteger)this).ToString();
    public string ToString(string format) => ((BigInteger)this).ToString(format);
    public string ToString(IFormatProvider provider) => ToString(null, provider);
    public string ToString(string? format, IFormatProvider? provider) => ((BigInteger)this).ToString(format, provider);
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => ((BigInteger)this).TryFormat(destination, out charsWritten, format, provider);


    #region Parsing
    const NumberStyles StyleUnsignedInteger = NumberStyles.Integer & ~NumberStyles.AllowLeadingSign;
    public static UInt128 Parse(string s) => TryParse(s, StyleUnsignedInteger, NumberFormatInfo.CurrentInfo, out var c) ? c : throw new FormatException();
    public static UInt128 Parse(string s, IFormatProvider? provider) => TryParse(s, StyleUnsignedInteger, provider, out var c) ? c : throw new FormatException();
    public static UInt128 Parse(string s, NumberStyles style, IFormatProvider? provider) => TryParse(s, style, provider, out var c) ? c : throw new FormatException();
    public static UInt128 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => TryParse(s, StyleUnsignedInteger, provider, out var c) ? c : throw new FormatException();
    public static UInt128 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => TryParse(s, style, provider, out var c) ? c : throw new FormatException();

    public static bool TryParse(string s, out UInt128 result) => TryParse(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out UInt128 result) => TryParse(s, StyleUnsignedInteger, provider, out result);
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out UInt128 result)
    {
        if (BigInteger.TryParse(s, style, provider, out BigInteger a))
        {
            Create(out result, a);
            return true;
        }
        result = Zero;
        return false;
    }
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out UInt128 result) => TryParse(s, StyleUnsignedInteger, provider, out result);
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out UInt128 result)
    {
        if (BigInteger.TryParse(s, style, provider, out BigInteger a))
        {
            Create(out result, a);
            return true;
        }
        result = Zero;
        return false;
    }
    #endregion Parsing


    #region Creation and Casting
    public UInt128(long value) => Create(out this, value);

    public UInt128(ulong value) => Create(out this, value);

    public UInt128(decimal value) => Create(out this, value);

    public UInt128(double value) => Create(out this, value);

    public UInt128(BigInteger value) => Create(out this, value);

    public static void Create(out UInt128 c, uint r0, uint r1, uint r2, uint r3)
    {
        c.s0 = (ulong)r1 << 32 | r0;
        c.s1 = (ulong)r3 << 32 | r2;
    }
    public static void Create(out UInt128 c, ulong s0, ulong s1)
    {
        c.s0 = s0;
        c.s1 = s1;
    }
    public static void Create(out UInt128 c, long a)
    {
        c.s0 = (ulong)a;
        c.s1 = a < 0 ? ulong.MaxValue : 0;
    }
    public static void Create(out UInt128 c, ulong a)
    {
        c.s0 = a;
        c.s1 = 0;
    }
    public static void Create(out UInt128 c, decimal a)
    {
        int[] bits = decimal.GetBits(decimal.Truncate(a));
        Create(out c, (uint)bits[0], (uint)bits[1], (uint)bits[2], 0);
        if (a < 0)
            Negate(ref c);
    }

    public static void Create(out UInt128 c, BigInteger a)
    {
        int sign = a.Sign;
        if (sign == -1)
            a = -a;
        c.s0 = (ulong)(a & ulong.MaxValue);
        c.s1 = (ulong)(a >> 64);
        if (sign == -1)
            Negate(ref c);
    }

    public static void Create(out UInt128 c, double a)
    {
        bool negate = false;
        if (a < 0)
        {
            negate = true;
            a = -a;
        }
        if (a <= ulong.MaxValue)
        {
            c.s0 = (ulong)a;
            c.s1 = 0;
        }
        else
        {
            int shift = Math.Max((int)Math.Ceiling(Math.Log(a, 2)) - 63, 0);
            c.s0 = (ulong)(a / Math.Pow(2, shift));
            c.s1 = 0;
            LeftShift(ref c, shift);
        }
        if (negate)
            Negate(ref c);
    }

    public static UInt128 Create<TOther>(TOther value) where TOther : INumber<TOther>
    {
        if (TryCreate(value, out var result))
            return result;
        throw new NotSupportedException();
    }


    public static bool TryCreate<TOther>(TOther value, out UInt128 result) where TOther : INumber<TOther>
    {
        bool success = true;
        UInt128 fail()
        {
            success = false;
            return default;
        }
        result = value switch
        {
            long a => new(a),
            ulong a => new(a),
            double a => new(a),
            decimal a => new(a),
            BigInteger a => new(a),
            Int128 a => new(a),
            float a => new(a),
            int a => new(a),
            uint a => new(a),
            short a => new(a),
            ushort a => new(a),
            char a => new(a),
            byte a => new(a),
            _ => fail(),
        };
        return success;
    }

    //TODO
    public static UInt128 CreateSaturating<TOther>(TOther value) where TOther : INumber<TOther> => throw new NotImplementedException();
    //TODO
    public static UInt128 CreateTruncating<TOther>(TOther value) where TOther : INumber<TOther> => throw new NotImplementedException();


    public static explicit operator UInt128(double a)
    {
        Create(out var c, a);
        return c;
    }

    public static explicit operator UInt128(sbyte a)
    {
        Create(out UInt128 c, a);
        return c;
    }

    public static implicit operator UInt128(byte a)
    {
        Create(out UInt128 c, a);
        return c;
    }

    public static explicit operator UInt128(short a)
    {
        Create(out UInt128 c, a);
        return c;
    }

    public static implicit operator UInt128(ushort a)
    {
        Create(out UInt128 c, a);
        return c;
    }

    public static explicit operator UInt128(int a)
    {
        Create(out UInt128 c, a);
        return c;
    }

    public static implicit operator UInt128(uint a)
    {
        Create(out UInt128 c, a);
        return c;
    }

    public static explicit operator UInt128(long a)
    {
        Create(out var c, a);
        return c;
    }

    public static implicit operator UInt128(ulong a)
    {
        Create(out var c, a);
        return c;
    }

    public static explicit operator UInt128(decimal a)
    {
        Create(out var c, a);
        return c;
    }

    public static explicit operator UInt128(BigInteger a)
    {
        Create(out var c, a);
        return c;
    }

    public static explicit operator float(UInt128 a) => ConvertToFloat(ref a);

    public static explicit operator double(UInt128 a) => ConvertToDouble(ref a);

    public static float ConvertToFloat(ref UInt128 a) => a.s1 == 0 ? a.s0 : a.s1 * (float)ulong.MaxValue + a.s0;

    public static double ConvertToDouble(ref UInt128 a) => a.s1 == 0 ? a.s0 : a.s1 * (double)ulong.MaxValue + a.s0;

    public static explicit operator sbyte(UInt128 a) => (sbyte)a.s0;

    public static explicit operator byte(UInt128 a) => (byte)a.s0;

    public static explicit operator short(UInt128 a) => (short)a.s0;

    public static explicit operator ushort(UInt128 a) => (ushort)a.s0;

    public static explicit operator int(UInt128 a) => (int)a.s0;

    public static explicit operator uint(UInt128 a) => (uint)a.s0;

    public static explicit operator long(UInt128 a) => (long)a.s0;

    public static explicit operator ulong(UInt128 a) => a.s0;

    public static explicit operator decimal(UInt128 a)
    {
        if (a.s1 == 0)
            return a.s0;
        int shift = Math.Max(0, 32 - GetBitLength(a.s1));

        RightShift(out _, ref a, shift);
        return new decimal((int)a.R0, (int)a.R1, (int)a.R2, false, (byte)shift);
    }

    public static implicit operator BigInteger(UInt128 a) => a.s1 == 0 ? (BigInteger)a.s0 : (BigInteger)a.s1 << 64 | a.s0;
    #endregion Creation and Casting


    public static UInt128 operator <<(UInt128 a, int b)
    {
        LeftShift(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator >>(UInt128 a, int b)
    {
        RightShift(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator &(UInt128 a, UInt128 b)
    {
        And(out var c, ref a, ref b);
        return c;
    }

    public static uint operator &(UInt128 a, uint b) => (uint)a.s0 & b;

    public static uint operator &(uint a, UInt128 b) => a & (uint)b.s0;

    public static ulong operator &(UInt128 a, ulong b) => a.s0 & b;

    public static ulong operator &(ulong a, UInt128 b) => a & b.s0;

    public static UInt128 operator |(UInt128 a, UInt128 b)
    {
        Or(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 operator ^(UInt128 a, UInt128 b)
    {
        ExclusiveOr(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 operator ~(UInt128 a)
    {
        Not(out var c, ref a);
        return c;
    }

    public static UInt128 operator +(UInt128 a, UInt128 b)
    {
        Add(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 operator +(UInt128 a, ulong b)
    {
        Add(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator +(ulong a, UInt128 b)
    {
        Add(out var c, ref b, a);
        return c;
    }

    public static UInt128 operator ++(UInt128 a)
    {
        Add(out UInt128 c, ref a, 1);
        return c;
    }

    public static UInt128 operator -(UInt128 a, UInt128 b)
    {
        Subtract(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 operator -(UInt128 a, ulong b)
    {
        Subtract(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator -(ulong a, UInt128 b)
    {
        Subtract(out var c, a, ref b);
        return c;
    }

    public static UInt128 operator --(UInt128 a)
    {
        Subtract(out UInt128 c, ref a, 1);
        return c;
    }

    public static UInt128 operator +(UInt128 a) => a;

    public static UInt128 operator -(UInt128 a)
    {
        Negate(ref a);
        return a;
    }

    public static UInt128 operator *(UInt128 a, uint b)
    {
        Multiply(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator *(uint a, UInt128 b)
    {
        Multiply(out var c, ref b, a);
        return c;
    }

    public static UInt128 operator *(UInt128 a, ulong b)
    {
        Multiply(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator *(ulong a, UInt128 b)
    {
        Multiply(out var c, ref b, a);
        return c;
    }

    public static UInt128 operator *(UInt128 a, UInt128 b)
    {
        Multiply(out UInt128 c, ref a, ref b);
        return c;
    }

    public static UInt128 operator /(UInt128 a, ulong b)
    {
        Divide(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator /(UInt128 a, UInt128 b)
    {
        Divide(out var c, ref a, ref b);
        return c;
    }

    public static ulong operator %(UInt128 a, uint b) => Remainder(ref a, b);

    public static ulong operator %(UInt128 a, ulong b) => Remainder(ref a, b);

    public static UInt128 operator %(UInt128 a, UInt128 b)
    {
        Remainder(out var c, ref a, ref b);
        return c;
    }

    #region Comparison operators
    public static bool operator <(UInt128 a, UInt128 b) => LessThan(ref a, ref b);
    public static bool operator <(UInt128 a, int b) => LessThan(ref a, b);
    public static bool operator <(int a, UInt128 b) => LessThan(a, ref b);
    public static bool operator <(UInt128 a, uint b) => LessThan(ref a, b);
    public static bool operator <(uint a, UInt128 b) => LessThan(a, ref b);
    public static bool operator <(UInt128 a, long b) => LessThan(ref a, b);
    public static bool operator <(long a, UInt128 b) => LessThan(a, ref b);
    public static bool operator <(UInt128 a, ulong b) => LessThan(ref a, b);
    public static bool operator <(ulong a, UInt128 b) => LessThan(a, ref b);
    public static bool operator <=(UInt128 a, UInt128 b) => !LessThan(ref b, ref a);
    public static bool operator <=(UInt128 a, int b) => !LessThan(b, ref a);
    public static bool operator <=(int a, UInt128 b) => !LessThan(ref b, a);
    public static bool operator <=(UInt128 a, uint b) => !LessThan(b, ref a);
    public static bool operator <=(uint a, UInt128 b) => !LessThan(ref b, a);
    public static bool operator <=(UInt128 a, long b) => !LessThan(b, ref a);
    public static bool operator <=(long a, UInt128 b) => !LessThan(ref b, a);
    public static bool operator <=(UInt128 a, ulong b) => !LessThan(b, ref a);
    public static bool operator <=(ulong a, UInt128 b) => !LessThan(ref b, a);
    public static bool operator >(UInt128 a, UInt128 b) => LessThan(ref b, ref a);
    public static bool operator >(UInt128 a, int b) => LessThan(b, ref a);
    public static bool operator >(int a, UInt128 b) => LessThan(ref b, a);
    public static bool operator >(UInt128 a, uint b) => LessThan(b, ref a);
    public static bool operator >(uint a, UInt128 b) => LessThan(ref b, a);
    public static bool operator >(UInt128 a, long b) => LessThan(b, ref a);
    public static bool operator >(long a, UInt128 b) => LessThan(ref b, a);
    public static bool operator >(UInt128 a, ulong b) => LessThan(b, ref a);
    public static bool operator >(ulong a, UInt128 b) => LessThan(ref b, a);
    public static bool operator >=(UInt128 a, UInt128 b) => !LessThan(ref a, ref b);
    public static bool operator >=(UInt128 a, int b) => !LessThan(ref a, b);
    public static bool operator >=(int a, UInt128 b) => !LessThan(a, ref b);
    public static bool operator >=(UInt128 a, uint b) => !LessThan(ref a, b);
    public static bool operator >=(uint a, UInt128 b) => !LessThan(a, ref b);
    public static bool operator >=(UInt128 a, long b) => !LessThan(ref a, b);
    public static bool operator >=(long a, UInt128 b) => !LessThan(a, ref b);
    public static bool operator >=(UInt128 a, ulong b) => !LessThan(ref a, b);
    public static bool operator >=(ulong a, UInt128 b) => !LessThan(a, ref b);
    public static bool operator ==(UInt128 a, UInt128 b) => a.Equals(b);
    public static bool operator ==(UInt128 a, int b) => a.Equals(b);
    public static bool operator ==(int a, UInt128 b) => b.Equals(a);
    public static bool operator ==(UInt128 a, uint b) => a.Equals(b);
    public static bool operator ==(uint a, UInt128 b) => b.Equals(a);
    public static bool operator ==(UInt128 a, long b) => a.Equals(b);
    public static bool operator ==(long a, UInt128 b) => b.Equals(a);
    public static bool operator ==(UInt128 a, ulong b) => a.Equals(b);
    public static bool operator ==(ulong a, UInt128 b) => b.Equals(a);
    public static bool operator !=(UInt128 a, UInt128 b) => !a.Equals(b);
    public static bool operator !=(UInt128 a, int b) => !a.Equals(b);
    public static bool operator !=(int a, UInt128 b) => !b.Equals(a);
    public static bool operator !=(UInt128 a, uint b) => !a.Equals(b);
    public static bool operator !=(uint a, UInt128 b) => !b.Equals(a);
    public static bool operator !=(UInt128 a, long b) => !a.Equals(b);
    public static bool operator !=(long a, UInt128 b) => !b.Equals(a);
    public static bool operator !=(UInt128 a, ulong b) => !a.Equals(b);
    public static bool operator !=(ulong a, UInt128 b) => !b.Equals(a);
    #endregion

    public int CompareTo(UInt128 other) => s1 != other.s1 ? s1.CompareTo(other.s1) : s0.CompareTo(other.s0);
    public int CompareTo(int other) => s1 != 0 || other < 0 ? 1 : s0.CompareTo((ulong)other);
    public int CompareTo(uint other) => s1 != 0 ? 1 : s0.CompareTo(other);
    public int CompareTo(long other) => s1 != 0 || other < 0 ? 1 : s0.CompareTo((ulong)other);
    public int CompareTo(ulong other) => s1 != 0 ? 1 : s0.CompareTo(other);
    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        UInt128 => CompareTo((UInt128)obj),
        _ => throw new ArgumentException(null, nameof(obj))
    };

    private static bool LessThan(ref UInt128 a, long b) => b >= 0 && a.s1 == 0 && a.s0 < (ulong)b;
    private static bool LessThan(long a, ref UInt128 b) => a < 0 || b.s1 != 0 || (ulong)a < b.s0;
    private static bool LessThan(ref UInt128 a, ulong b) => a.s1 == 0 && a.s0 < b;
    private static bool LessThan(ulong a, ref UInt128 b) => b.s1 != 0 || a < b.s0;
    private static bool LessThan(ref UInt128 a, ref UInt128 b) => a.s1 != b.s1 ? a.s1 < b.s1 : a.s0 < b.s0;

    public static bool Equals(ref UInt128 a, ref UInt128 b) => a.s0 == b.s0 && a.s1 == b.s1;
    public bool Equals(UInt128 other) => s0 == other.s0 && s1 == other.s1;
    public bool Equals(int other) => other >= 0 && s0 == (uint)other && s1 == 0;
    public bool Equals(uint other) => s0 == other && s1 == 0;
    public bool Equals(long other) => other >= 0 && s0 == (ulong)other && s1 == 0;
    public bool Equals(ulong other) => s0 == other && s1 == 0;
    public override bool Equals(object? obj) => obj is UInt128 n && Equals(n);

    public override int GetHashCode() => s0.GetHashCode() ^ s1.GetHashCode();

    public static void Multiply(out UInt128 c, ulong a, ulong b)
    {
        Multiply64(out c, a, b);
        Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
    }

    public static void Multiply(out UInt128 c, ref UInt128 a, uint b)
    {
        if (a.s1 == 0)
            Multiply64(out c, a.s0, b);
        else
            Multiply128(out c, ref a, b);
        Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b % ((BigInteger)1 << 128));
    }

    public static void Multiply(out UInt128 c, ref UInt128 a, ulong b)
    {
        if (a.s1 == 0)
            Multiply64(out c, a.s0, b);
        else
            Multiply128(out c, ref a, b);
        Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b % ((BigInteger)1 << 128));
    }

    public static void Multiply(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        if ((a.s1 | b.s1) == 0)
            Multiply64(out c, a.s0, b.s0);
        else if (a.s1 == 0)
            Multiply128(out c, ref b, a.s0);
        else if (b.s1 == 0)
            Multiply128(out c, ref a, b.s0);
        else
            Multiply128(out c, ref a, ref b);
        Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b % ((BigInteger)1 << 128));
    }

    private static void Multiply(out UInt256 c, ref UInt128 a, ref UInt128 b)
    {
        Multiply64(out var c00, a.s0, b.s0);
        Multiply64(out var c01, a.s0, b.s1);
        Multiply64(out var c10, a.s1, b.s0);
        Multiply64(out var c11, a.s1, b.s1);
        uint carry1 = 0;
        uint carry2 = 0;
        c.s0 = c00.S0;
        c.s1 = Add(Add(c00.s1, c01.s0, ref carry1), c10.s0, ref carry1);
        c.s2 = Add(Add(Add(c01.s1, c10.s1, ref carry2), c11.s0, ref carry2), carry1, ref carry2);
        c.s3 = c11.s1 + carry2;
        Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
    }

    public static UInt128 Abs(UInt128 a) => a;

    public static UInt128 Square(ulong a)
    {
        Square(out var c, a);
        return c;
    }

    public static UInt128 Square(UInt128 a)
    {
        Square(out var c, ref a);
        return c;
    }

    public static void Square(out UInt128 c, ulong a) => Square64(out c, a);

    public static void Square(out UInt128 c, ref UInt128 a)
    {
        if (a.s1 == 0)
            Square64(out c, a.s0);
        else
            Multiply128(out c, ref a, ref a);
    }

    public static UInt128 Cube(ulong a)
    {
        Cube(out var c, a);
        return c;
    }

    public static UInt128 Cube(UInt128 a)
    {
        Cube(out var c, ref a);
        return c;
    }

    public static void Cube(out UInt128 c, ulong a)
    {
        Square(out var square, a);
        Multiply(out c, ref square, a);
    }

    public static void Cube(out UInt128 c, ref UInt128 a)
    {
        if (a.s1 == 0)
        {
            Square64(out var square, a.s0);
            Multiply(out c, ref square, a.s0);
        }
        else
        {
            Multiply128(out var square, ref a, ref a);
            Multiply128(out c, ref square, ref a);
        }
    }

    public static void Add(out UInt128 c, ulong a, ulong b)
    {
        c.s0 = a + b;
        c.s1 = 0;
        if (c.s0 < a && c.s0 < b)
            ++c.s1;
        Debug.Assert((BigInteger)c == ((BigInteger)a + (BigInteger)b));
    }

    public static void Add(out UInt128 c, ref UInt128 a, ulong b)
    {
        c.s0 = a.s0 + b;
        c.s1 = a.s1;
        if (c.s0 < a.s0 && c.s0 < b)
            ++c.s1;
        Debug.Assert((BigInteger)c == ((BigInteger)a + (BigInteger)b) % ((BigInteger)1 << 128));
    }

    public static void Add(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        c.s0 = a.s0 + b.s0;
        c.s1 = a.s1 + b.s1;
        if (c.s0 < a.s0 && c.s0 < b.s0)
            ++c.s1;
        Debug.Assert((BigInteger)c == ((BigInteger)a + (BigInteger)b) % ((BigInteger)1 << 128));
    }

    private static ulong Add(ulong a, ulong b, ref uint carry)
    {
        ulong c = a + b;
        if (c < a && c < b)
            ++carry;
        return c;
    }

    public static void Add(ref UInt128 a, ulong b)
    {
        ulong sum = a.s0 + b;
        if (sum < a.s0 && sum < b)
            ++a.s1;
        a.s0 = sum;
    }

    public static void Add(ref UInt128 a, ref UInt128 b)
    {
        ulong sum = a.s0 + b.s0;
        if (sum < a.s0 && sum < b.s0)
            ++a.s1;
        a.s0 = sum;
        a.s1 += b.s1;
    }

    public static void Add(ref UInt128 a, UInt128 b) => Add(ref a, ref b);

    public static void Subtract(out UInt128 c, ref UInt128 a, ulong b)
    {
        c.s0 = a.s0 - b;
        c.s1 = a.s1;
        if (a.s0 < b)
            --c.s1;
        Debug.Assert((BigInteger)c == ((BigInteger)a - (BigInteger)b + ((BigInteger)1 << 128)) % ((BigInteger)1 << 128));
    }

    public static void Subtract(out UInt128 c, ulong a, ref UInt128 b)
    {
        c.s0 = a - b.s0;
        c.s1 = 0 - b.s1;
        if (a < b.s0)
            --c.s1;
        Debug.Assert((BigInteger)c == ((BigInteger)a - (BigInteger)b + ((BigInteger)1 << 128)) % ((BigInteger)1 << 128));
    }

    public static void Subtract(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        c.s0 = a.s0 - b.s0;
        c.s1 = a.s1 - b.s1;
        if (a.s0 < b.s0)
            --c.s1;
        Debug.Assert((BigInteger)c == ((BigInteger)a - (BigInteger)b + ((BigInteger)1 << 128)) % ((BigInteger)1 << 128));
    }

    public static void Subtract(ref UInt128 a, ulong b)
    {
        if (a.s0 < b)
            --a.s1;
        a.s0 -= b;
    }

    public static void Subtract(ref UInt128 a, ref UInt128 b)
    {
        if (a.s0 < b.s0)
            --a.s1;
        a.s0 -= b.s0;
        a.s1 -= b.s1;
    }

    public static void Subtract(ref UInt128 a, UInt128 b) => Subtract(ref a, ref b);

    private static void Square64(out UInt128 w, ulong u)
    {
        ulong u0 = (uint)u;
        ulong u1 = u >> 32;
        ulong carry = u0 * u0;
        uint r0 = (uint)carry;
        ulong u0u1 = u0 * u1;
        carry = (carry >> 32) + u0u1;
        ulong r2 = carry >> 32;
        carry = (uint)carry + u0u1;
        w.s0 = carry << 32 | r0;
        w.s1 = (carry >> 32) + r2 + u1 * u1;
        Debug.Assert((BigInteger)w == (BigInteger)u * u);
    }

    private static void Multiply64(out UInt128 w, uint u, uint v)
    {
        w.s0 = (ulong)u * v;
        w.s1 = 0;
        Debug.Assert((BigInteger)w == (BigInteger)u * v);
    }

    private static void Multiply64(out UInt128 w, ulong u, uint v)
    {
        ulong u0 = (uint)u;
        ulong u1 = u >> 32;
        ulong carry = u0 * v;
        uint r0 = (uint)carry;
        carry = (carry >> 32) + u1 * v;
        w.s0 = carry << 32 | r0;
        w.s1 = carry >> 32;
        Debug.Assert((BigInteger)w == (BigInteger)u * v);
    }

    private static void Multiply64(out UInt128 w, ulong u, ulong v)
    {
        ulong u0 = (uint)u;
        ulong u1 = u >> 32;
        ulong v0 = (uint)v;
        ulong v1 = v >> 32;
        ulong carry = u0 * v0;
        uint r0 = (uint)carry;
        carry = (carry >> 32) + u0 * v1;
        ulong r2 = carry >> 32;
        carry = (uint)carry + u1 * v0;
        w.s0 = carry << 32 | r0;
        w.s1 = (carry >> 32) + r2 + u1 * v1;
        Debug.Assert((BigInteger)w == (BigInteger)u * v);
    }

    private static void Multiply64(out UInt128 w, ulong u, ulong v, ulong c)
    {
        ulong u0 = (uint)u;
        ulong u1 = u >> 32;
        ulong v0 = (uint)v;
        ulong v1 = v >> 32;
        ulong carry = u0 * v0 + (uint)c;
        uint r0 = (uint)carry;
        carry = (carry >> 32) + u0 * v1 + (c >> 32);
        ulong r2 = carry >> 32;
        carry = (uint)carry + u1 * v0;
        w.s0 = carry << 32 | r0;
        w.s1 = (carry >> 32) + r2 + u1 * v1;
        Debug.Assert((BigInteger)w == (BigInteger)u * v + c);
    }

    private static ulong MultiplyHigh64(ulong u, ulong v, ulong c)
    {
        ulong u0 = (uint)u;
        ulong u1 = u >> 32;
        ulong v0 = (uint)v;
        ulong v1 = v >> 32;
        ulong carry = ((u0 * v0 + (uint)c) >> 32) + u0 * v1 + (c >> 32);
        ulong r2 = carry >> 32;
        carry = (uint)carry + u1 * v0;
        return (carry >> 32) + r2 + u1 * v1;
    }

    private static void Multiply128(out UInt128 w, ref UInt128 u, uint v)
    {
        Multiply64(out w, u.s0, v);
        w.s1 += u.s1 * v;
        Debug.Assert((BigInteger)w == (BigInteger)u * v % ((BigInteger)1 << 128));
    }

    private static void Multiply128(out UInt128 w, ref UInt128 u, ulong v)
    {
        Multiply64(out w, u.s0, v);
        w.s1 += u.s1 * v;
        Debug.Assert((BigInteger)w == (BigInteger)u * v % ((BigInteger)1 << 128));
    }

    private static void Multiply128(out UInt128 w, ref UInt128 u, ref UInt128 v)
    {
        Multiply64(out w, u.s0, v.s0);
        w.s1 += u.s1 * v.s0 + u.s0 * v.s1;
        Debug.Assert((BigInteger)w == (BigInteger)u * v % ((BigInteger)1 << 128));
    }

    public static void Divide(out UInt128 w, ref UInt128 u, uint v)
    {
        if (u.s1 == 0)
            Divide64(out w, u.s0, v);
        else if (u.s1 <= uint.MaxValue)
            Divide96(out w, ref u, v);
        else
            Divide128(out w, ref u, v);
    }

    public static void Divide(out UInt128 w, ref UInt128 u, ulong v)
    {
        if (u.s1 == 0)
            Divide64(out w, u.s0, v);
        else
        {
            uint v0 = (uint)v;
            if (v == v0)
            {
                if (u.s1 <= uint.MaxValue)
                    Divide96(out w, ref u, v0);
                else
                    Divide128(out w, ref u, v0);
            }
            else
            {
                if (u.s1 <= uint.MaxValue)
                    Divide96(out w, ref u, v);
                else
                    Divide128(out w, ref u, v);
            }
        }
    }

    public static void Divide(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        if (LessThan(ref a, ref b))
            c = Zero;
        else if (b.s1 == 0)
            Divide(out c, ref a, b.s0);
        else if (b.s1 <= uint.MaxValue)
            Create(out c, DivRem96(out _, ref a, ref b));
        else
            Create(out c, DivRem128(out _, ref a, ref b));
    }

    public static uint Remainder(ref UInt128 u, uint v) => u.s1 switch
    {
        0 => (uint)(u.s0 % v),
        <= uint.MaxValue => Remainder96(ref u, v),
        _ => Remainder128(ref u, v)
    };

    public static ulong Remainder(ref UInt128 u, ulong v)
    {
        if (u.s1 == 0)
            return u.s0 % v;
        uint v0 = (uint)v;
        return v == v0
            ? u.s1 <= uint.MaxValue ? Remainder96(ref u, v0) : Remainder128(ref u, v0)
            : u.s1 <= uint.MaxValue ? Remainder96(ref u, v) : Remainder128(ref u, v);
    }

    public static void Remainder(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        if (LessThan(ref a, ref b))
            c = a;
        else if (b.s1 == 0)
            Create(out c, Remainder(ref a, b.s0));
        else if (b.s1 <= uint.MaxValue)
            DivRem96(out c, ref a, ref b);
        else
            DivRem128(out c, ref a, ref b);
    }

    public static void Remainder(ref UInt128 a, ref UInt128 b)
    {
        var a2 = a;
        Remainder(out a, ref a2, ref b);
    }

    private static void Remainder(out UInt128 c, ref UInt256 a, ref UInt128 b)
    {
        if (b.R3 == 0)
            Remainder192(out c, ref a, ref b);
        else
            Remainder256(out c, ref a, ref b);
    }

    private static void Divide64(out UInt128 w, ulong u, ulong v)
    {
        w.s1 = 0;
        w.s0 = u / v;
        Debug.Assert((BigInteger)w == (BigInteger)u / v);
    }

    private static void Divide96(out UInt128 w, ref UInt128 u, uint v)
    {
        uint r2 = u.R2;
        uint w2 = r2 / v;
        ulong u0 = r2 - w2 * v;
        ulong u0u1 = u0 << 32 | u.R1;
        uint w1 = (uint)(u0u1 / v);
        u0 = u0u1 - w1 * v;
        u0u1 = u0 << 32 | u.R0;
        uint w0 = (uint)(u0u1 / v);
        w.s1 = w2;
        w.s0 = (ulong)w1 << 32 | w0;
        Debug.Assert((BigInteger)w == (BigInteger)u / v);
    }

    private static void Divide128(out UInt128 w, ref UInt128 u, uint v)
    {
        uint r3 = u.R3;
        uint w3 = r3 / v;
        ulong u0 = r3 - w3 * v;
        ulong u0u1 = u0 << 32 | u.R2;
        uint w2 = (uint)(u0u1 / v);
        u0 = u0u1 - w2 * v;
        u0u1 = u0 << 32 | u.R1;
        uint w1 = (uint)(u0u1 / v);
        u0 = u0u1 - w1 * v;
        u0u1 = u0 << 32 | u.R0;
        uint w0 = (uint)(u0u1 / v);
        w.s1 = (ulong)w3 << 32 | w2;
        w.s0 = (ulong)w1 << 32 | w0;
        Debug.Assert((BigInteger)w == (BigInteger)u / v);
    }

    private static void Divide96(out UInt128 w, ref UInt128 u, ulong v)
    {
        w.s0 = w.s1 = 0;
        int dneg = GetBitLength((uint)(v >> 32));
        int d = 32 - dneg;
        ulong vPrime = v << d;
        uint v1 = (uint)(vPrime >> 32);
        uint v2 = (uint)vPrime;
        uint r0 = u.R0;
        uint r1 = u.R1;
        uint r2 = u.R2;
        uint r3 = 0;
        if (d != 0)
        {
            r3 = r2 >> dneg;
            r2 = r2 << d | r1 >> dneg;
            r1 = r1 << d | r0 >> dneg;
            r0 <<= d;
        }
        uint q1 = DivRem(r3, ref r2, ref r1, v1, v2);
        uint q0 = DivRem(r2, ref r1, ref r0, v1, v2);
        w.s0 = (ulong)q1 << 32 | q0;
        w.s1 = 0;
        Debug.Assert((BigInteger)w == (BigInteger)u / v);
    }

    private static void Divide128(out UInt128 w, ref UInt128 u, ulong v)
    {
        w.s0 = w.s1 = 0;
        int dneg = GetBitLength((uint)(v >> 32));
        int d = 32 - dneg;
        ulong vPrime = v << d;
        uint v1 = (uint)(vPrime >> 32);
        uint v2 = (uint)vPrime;
        uint r0 = u.R0;
        uint r1 = u.R1;
        uint r2 = u.R2;
        uint r3 = u.R3;
        uint r4 = 0;
        if (d != 0)
        {
            r4 = r3 >> dneg;
            r3 = r3 << d | r2 >> dneg;
            r2 = r2 << d | r1 >> dneg;
            r1 = r1 << d | r0 >> dneg;
            r0 <<= d;
        }
        w.s1 = DivRem(r4, ref r3, ref r2, v1, v2);
        uint q1 = DivRem(r3, ref r2, ref r1, v1, v2);
        uint q0 = DivRem(r2, ref r1, ref r0, v1, v2);
        w.s0 = (ulong)q1 << 32 | q0;
        Debug.Assert((BigInteger)w == (BigInteger)u / v);
    }

    private static uint Remainder96(ref UInt128 u, uint v)
    {
        ulong u0 = u.R2 % v;
        ulong u0u1 = u0 << 32 | u.R1;
        u0 = u0u1 % v;
        u0u1 = u0 << 32 | u.R0;
        return (uint)(u0u1 % v);
    }

    private static uint Remainder128(ref UInt128 u, uint v)
    {
        ulong u0 = u.R3 % v;
        ulong u0u1 = u0 << 32 | u.R2;
        u0 = u0u1 % v;
        u0u1 = u0 << 32 | u.R1;
        u0 = u0u1 % v;
        u0u1 = u0 << 32 | u.R0;
        return (uint)(u0u1 % v);
    }

    private static ulong Remainder96(ref UInt128 u, ulong v)
    {
        int dneg = GetBitLength((uint)(v >> 32));
        int d = 32 - dneg;
        ulong vPrime = v << d;
        uint v1 = (uint)(vPrime >> 32);
        uint v2 = (uint)vPrime;
        uint r0 = u.R0;
        uint r1 = u.R1;
        uint r2 = u.R2;
        uint r3 = 0;
        if (d != 0)
        {
            r3 = r2 >> dneg;
            r2 = r2 << d | r1 >> dneg;
            r1 = r1 << d | r0 >> dneg;
            r0 <<= d;
        }
        DivRem(r3, ref r2, ref r1, v1, v2);
        DivRem(r2, ref r1, ref r0, v1, v2);
        return ((ulong)r1 << 32 | r0) >> d;
    }

    private static ulong Remainder128(ref UInt128 u, ulong v)
    {
        int dneg = GetBitLength((uint)(v >> 32));
        int d = 32 - dneg;
        ulong vPrime = v << d;
        uint v1 = (uint)(vPrime >> 32);
        uint v2 = (uint)vPrime;
        uint r0 = u.R0;
        uint r1 = u.R1;
        uint r2 = u.R2;
        uint r3 = u.R3;
        uint r4 = 0;
        if (d != 0)
        {
            r4 = r3 >> dneg;
            r3 = r3 << d | r2 >> dneg;
            r2 = r2 << d | r1 >> dneg;
            r1 = r1 << d | r0 >> dneg;
            r0 <<= d;
        }
        DivRem(r4, ref r3, ref r2, v1, v2);
        DivRem(r3, ref r2, ref r1, v1, v2);
        DivRem(r2, ref r1, ref r0, v1, v2);
        return ((ulong)r1 << 32 | r0) >> d;
    }

    private static ulong DivRem96(out UInt128 rem, ref UInt128 a, ref UInt128 b)
    {
        int d = 32 - GetBitLength(b.R2);
        LeftShift64(out var v, ref b, d);
        uint r4 = (uint)LeftShift64(out rem, ref a, d);
        uint v1 = v.R2;
        uint v2 = v.R1;
        uint v3 = v.R0;
        uint r3 = rem.R3;
        uint r2 = rem.R2;
        uint r1 = rem.R1;
        uint r0 = rem.R0;
        uint q1 = DivRem(r4, ref r3, ref r2, ref r1, v1, v2, v3);
        uint q0 = DivRem(r3, ref r2, ref r1, ref r0, v1, v2, v3);
        Create(out rem, r0, r1, r2, 0);
        ulong div = (ulong)q1 << 32 | q0;
        RightShift64(ref rem, d);
        Debug.Assert((BigInteger)div == (BigInteger)a / (BigInteger)b);
        Debug.Assert((BigInteger)rem == (BigInteger)a % (BigInteger)b);
        return div;
    }

    private static uint DivRem128(out UInt128 rem, ref UInt128 a, ref UInt128 b)
    {
        int d = 32 - GetBitLength(b.R3);
        LeftShift64(out var v, ref b, d);
        uint r4 = (uint)LeftShift64(out rem, ref a, d);
        uint r3 = rem.R3;
        uint r2 = rem.R2;
        uint r1 = rem.R1;
        uint r0 = rem.R0;
        uint div = DivRem(r4, ref r3, ref r2, ref r1, ref r0, v.R3, v.R2, v.R1, v.R0);
        Create(out rem, r0, r1, r2, r3);
        RightShift64(ref rem, d);
        Debug.Assert((BigInteger)div == (BigInteger)a / (BigInteger)b);
        Debug.Assert((BigInteger)rem == (BigInteger)a % (BigInteger)b);
        return div;
    }

    private static void Remainder192(out UInt128 c, ref UInt256 a, ref UInt128 b)
    {
        int d = 32 - GetBitLength(b.R2);
        LeftShift64(out var v, ref b, d);
        uint v1 = v.R2;
        uint v2 = v.R1;
        uint v3 = v.R0;
        LeftShift64(out var rem, ref a, d);
        uint r6 = rem.R6;
        uint r5 = rem.R5;
        uint r4 = rem.R4;
        uint r3 = rem.R3;
        uint r2 = rem.R2;
        uint r1 = rem.R1;
        uint r0 = rem.R0;
        DivRem(r6, ref r5, ref r4, ref r3, v1, v2, v3);
        DivRem(r5, ref r4, ref r3, ref r2, v1, v2, v3);
        DivRem(r4, ref r3, ref r2, ref r1, v1, v2, v3);
        DivRem(r3, ref r2, ref r1, ref r0, v1, v2, v3);
        Create(out c, r0, r1, r2, 0);
        RightShift64(ref c, d);
        Debug.Assert((BigInteger)c == (BigInteger)a % (BigInteger)b);
    }

    private static void Remainder256(out UInt128 c, ref UInt256 a, ref UInt128 b)
    {
        int d = 32 - GetBitLength(b.R3);
        LeftShift64(out var v, ref b, d);
        uint v1 = v.R3;
        uint v2 = v.R2;
        uint v3 = v.R1;
        uint v4 = v.R0;
        uint r8 = (uint)LeftShift64(out var rem, ref a, d);
        uint r7 = rem.R7;
        uint r6 = rem.R6;
        uint r5 = rem.R5;
        uint r4 = rem.R4;
        uint r3 = rem.R3;
        uint r2 = rem.R2;
        uint r1 = rem.R1;
        uint r0 = rem.R0;
        DivRem(r8, ref r7, ref r6, ref r5, ref r4, v1, v2, v3, v4);
        DivRem(r7, ref r6, ref r5, ref r4, ref r3, v1, v2, v3, v4);
        DivRem(r6, ref r5, ref r4, ref r3, ref r2, v1, v2, v3, v4);
        DivRem(r5, ref r4, ref r3, ref r2, ref r1, v1, v2, v3, v4);
        DivRem(r4, ref r3, ref r2, ref r1, ref r0, v1, v2, v3, v4);
        Create(out c, r0, r1, r2, r3);
        RightShift64(ref c, d);
        Debug.Assert((BigInteger)c == (BigInteger)a % (BigInteger)b);
    }

    private static ulong Q(uint u0, uint u1, uint u2, uint v1, uint v2)
    {
        ulong u0u1 = (ulong)u0 << 32 | u1;
        ulong qhat = u0 == v1 ? uint.MaxValue : u0u1 / v1;
        ulong r = u0u1 - qhat * v1;
        if (r == (uint)r && v2 * qhat > (r << 32 | u2))
        {
            --qhat;
            r += v1;
            if (r == (uint)r && v2 * qhat > (r << 32 | u2))
            {
                --qhat;
                //r += v1; //redundant
            }
        }
        return qhat;
    }

    private static uint DivRem(uint u0, ref uint u1, ref uint u2, uint v1, uint v2)
    {
        ulong qhat = Q(u0, u1, u2, v1, v2);
        ulong carry = qhat * v2;
        long borrow = (long)u2 - (uint)carry;
        carry >>= 32;
        u2 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v1;
        borrow += (long)u1 - (uint)carry;
        carry >>= 32;
        u1 = (uint)borrow;
        borrow >>= 32;
        borrow += (long)u0 - (uint)carry;
        if (borrow != 0)
        {
            --qhat;
            carry = (ulong)u2 + v2;
            u2 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u1 + v1;
            u1 = (uint)carry;
        }
        return (uint)qhat;
    }

    private static uint DivRem(uint u0, ref uint u1, ref uint u2, ref uint u3, uint v1, uint v2, uint v3)
    {
        ulong qhat = Q(u0, u1, u2, v1, v2);
        ulong carry = qhat * v3;
        long borrow = (long)u3 - (uint)carry;
        carry >>= 32;
        u3 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v2;
        borrow += (long)u2 - (uint)carry;
        carry >>= 32;
        u2 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v1;
        borrow += (long)u1 - (uint)carry;
        carry >>= 32;
        u1 = (uint)borrow;
        borrow >>= 32;
        borrow += (long)u0 - (uint)carry;
        if (borrow != 0)
        {
            --qhat;
            carry = (ulong)u3 + v3;
            u3 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u2 + v2;
            u2 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u1 + v1;
            u1 = (uint)carry;
        }
        return (uint)qhat;
    }

    private static uint DivRem(uint u0, ref uint u1, ref uint u2, ref uint u3, ref uint u4, uint v1, uint v2, uint v3, uint v4)
    {
        ulong qhat = Q(u0, u1, u2, v1, v2);
        ulong carry = qhat * v4;
        long borrow = (long)u4 - (uint)carry;
        carry >>= 32;
        u4 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v3;
        borrow += (long)u3 - (uint)carry;
        carry >>= 32;
        u3 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v2;
        borrow += (long)u2 - (uint)carry;
        carry >>= 32;
        u2 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v1;
        borrow += (long)u1 - (uint)carry;
        carry >>= 32;
        u1 = (uint)borrow;
        borrow >>= 32;
        borrow += (long)u0 - (uint)carry;
        if (borrow != 0)
        {
            --qhat;
            carry = (ulong)u4 + v4;
            u4 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u3 + v3;
            u3 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u2 + v2;
            u2 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u1 + v1;
            u1 = (uint)carry;
        }
        return (uint)qhat;
    }

    public static void ModAdd(out UInt128 c, ref UInt128 a, ref UInt128 b, ref UInt128 modulus)
    {
        Add(out c, ref a, ref b);
        if (!LessThan(ref c, ref modulus) || LessThan(ref c, ref a) && LessThan(ref c, ref b))
            Subtract(ref c, ref modulus);
    }

    public static void ModSub(out UInt128 c, ref UInt128 a, ref UInt128 b, ref UInt128 modulus)
    {
        Subtract(out c, ref a, ref b);
        if (LessThan(ref a, ref b))
            Add(ref c, ref modulus);
    }

    public static void ModMul(out UInt128 c, ref UInt128 a, ref UInt128 b, ref UInt128 modulus)
    {
        if (modulus.s1 == 0)
        {
            Multiply64(out var product, a.s0, b.s0);
            Create(out c, Remainder(ref product, modulus.s0));
        }
        else
        {
            Multiply(out UInt256 product, ref a, ref b);
            Remainder(out c, ref product, ref modulus);
        }
    }

    public static void ModMul(ref UInt128 a, ref UInt128 b, ref UInt128 modulus)
    {
        if (modulus.s1 == 0)
        {
            Multiply64(out var product, a.s0, b.s0);
            Create(out a, Remainder(ref product, modulus.s0));
        }
        else
        {
            Multiply(out UInt256 product, ref a, ref b);
            Remainder(out a, ref product, ref modulus);
        }
    }

    public static void ModPow(out UInt128 result, ref UInt128 value, ref UInt128 exponent, ref UInt128 modulus)
    {
        result = One;
        var v = value;
        ulong e = exponent.s0;
        if (exponent.s1 != 0)
        {
            for (int i = 0; i < 64; i++)
            {
                if ((e & 1) != 0)
                    ModMul(ref result, ref v, ref modulus);
                ModMul(ref v, ref v, ref modulus);
                e >>= 1;
            }
            e = exponent.s1;
        }
        while (e != 0)
        {
            if ((e & 1) != 0)
                ModMul(ref result, ref v, ref modulus);
            if (e != 1)
                ModMul(ref v, ref v, ref modulus);
            e >>= 1;
        }
        Debug.Assert(BigInteger.ModPow(value, exponent, modulus) == result);
    }

    public static void Shift(out UInt128 c, ref UInt128 a, int d)
    {
        if (d < 0)
            RightShift(out c, ref a, -d);
        else
            LeftShift(out c, ref a, d);
    }

    public static void ArithmeticShift(out UInt128 c, ref UInt128 a, int d)
    {
        if (d < 0)
            ArithmeticRightShift(out c, ref a, -d);
        else
            LeftShift(out c, ref a, d);
    }

    public static ulong LeftShift64(out UInt128 c, ref UInt128 a, int d)
    {
        if (d == 0)
        {
            c = a;
            return 0;
        }
        int dneg = 64 - d;
        c.s1 = a.s1 << d | a.s0 >> dneg;
        c.s0 = a.s0 << d;
        return a.s1 >> dneg;
    }

    private static ulong LeftShift64(out UInt256 c, ref UInt256 a, int d)
    {
        if (d == 0)
        {
            c = a;
            return 0;
        }
        int dneg = 64 - d;
        c.s3 = a.s3 << d | a.s2 >> dneg;
        c.s2 = a.s2 << d | a.s1 >> dneg;
        c.s1 = a.s1 << d | a.s0 >> dneg;
        c.s0 = a.s0 << d;
        return a.s3 >> dneg;
    }

    public static void LeftShift(out UInt128 c, ref UInt128 a, int b)
    {
        if (b < 64)
            LeftShift64(out c, ref a, b);
        else if (b == 64)
        {
            c.s0 = 0;
            c.s1 = a.s0;
            return;
        }
        else
        {
            c.s0 = 0;
            c.s1 = a.s0 << (b - 64);
        }
    }

    public static void RightShift64(out UInt128 c, ref UInt128 a, int b)
    {
        if (b == 0)
            c = a;
        else
        {
            c.s0 = a.s0 >> b | a.s1 << (64 - b);
            c.s1 = a.s1 >> b;
        }
    }

    public static void RightShift(out UInt128 c, ref UInt128 a, int b)
    {
        if (b < 64)
            RightShift64(out c, ref a, b);
        else if (b == 64)
        {
            c.s0 = a.s1;
            c.s1 = 0;
        }
        else
        {
            c.s0 = a.s1 >> (b - 64);
            c.s1 = 0;
        }
    }

    public static void ArithmeticRightShift64(out UInt128 c, ref UInt128 a, int b)
    {
        if (b == 0)
            c = a;
        else
        {
            c.s0 = a.s0 >> b | a.s1 << (64 - b);
            c.s1 = (ulong)((long)a.s1 >> b);
        }
    }

    public static void ArithmeticRightShift(out UInt128 c, ref UInt128 a, int b)
    {
        if (b < 64)
            ArithmeticRightShift64(out c, ref a, b);
        else if (b == 64)
        {
            c.s0 = a.s1;
            c.s1 = (ulong)((long)a.s1 >> 63);
        }
        else
        {
            c.s0 = a.s1 >> (b - 64);
            c.s1 = (ulong)((long)a.s1 >> 63);
        }
    }

    public static void And(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        c.s0 = a.s0 & b.s0;
        c.s1 = a.s1 & b.s1;
    }

    public static void Or(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        c.s0 = a.s0 | b.s0;
        c.s1 = a.s1 | b.s1;
    }

    public static void ExclusiveOr(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        c.s0 = a.s0 ^ b.s0;
        c.s1 = a.s1 ^ b.s1;
    }

    public static void Not(out UInt128 c, ref UInt128 a)
    {
        c.s0 = ~a.s0;
        c.s1 = ~a.s1;
    }

    public static void Negate(ref UInt128 a)
    {
        ulong s0 = a.s0;
        a.s0 = 0 - s0;
        a.s1 = 0 - a.s1;
        if (s0 > 0)
            --a.s1;
    }


    public static void Negate(out UInt128 c, ref UInt128 a)
    {
        c.s0 = 0 - a.s0;
        c.s1 = 0 - a.s1;
        if (a.s0 > 0)
            --c.s1;
        Debug.Assert((BigInteger)c == (BigInteger)(~a + 1));
    }

    public static void Pow(out UInt128 result, ref UInt128 value, uint exponent)
    {
        result = One;
        while (exponent != 0)
        {

            if ((exponent & 1) != 0)
            {
                var previous = result;
                Multiply(out result, ref previous, ref value);
            }
            if (exponent != 1)
            {
                var previous = value;
                Square(out value, ref previous);
            }
            exponent >>= 1;
        }
    }

    public static UInt128 Pow(UInt128 value, uint exponent)
    {
        Pow(out var result, ref value, exponent);
        return result;
    }

    private const int maxRepShift = 53;
    private static readonly ulong maxRep = (ulong)1 << maxRepShift;
    private static readonly UInt128 maxRepSquaredHigh = (ulong)1 << (2 * maxRepShift - 64);

    public static ulong FloorSqrt(UInt128 a)
    {
        if (a.s1 == 0 && a.s0 <= maxRep)
            return (ulong)Math.Sqrt(a.s0);

        ulong s = (ulong)Math.Sqrt(ConvertToDouble(ref a));
        if (a.s1 < maxRepSquaredHigh)
        {
            Square(out var s2, s);
            ulong r = a.s0 - s2.s0;
            if (r > long.MaxValue)
                --s;
            else if (r - (s << 1) <= long.MaxValue)
                ++s;
            Debug.Assert((BigInteger)s * s <= a && (BigInteger)(s + 1) * (s + 1) > a);
            return s;
        }
        s = FloorSqrt(ref a, s);
        Debug.Assert((BigInteger)s * s <= a && (BigInteger)(s + 1) * (s + 1) > a);
        return s;
    }

    public static ulong CeilingSqrt(UInt128 a)
    {
        if (a.s1 == 0 && a.s0 <= maxRep)
            return (ulong)Math.Ceiling(Math.Sqrt(a.s0));
        ulong s = (ulong)Math.Ceiling(Math.Sqrt(ConvertToDouble(ref a)));
        if (a.s1 < maxRepSquaredHigh)
        {
            Square(out var s2, s);
            ulong r = s2.s0 - a.s0;
            if (r > long.MaxValue)
                ++s;
            else if (r - (s << 1) <= long.MaxValue)
                --s;
            Debug.Assert((BigInteger)(s - 1) * (s - 1) < a && (BigInteger)s * s >= a);
            return s;
        }
        s = FloorSqrt(ref a, s);
        Square(out var square, s);
        if (square.S0 != a.S0 || square.S1 != a.S1)
            ++s;
        Debug.Assert((BigInteger)(s - 1) * (s - 1) < a && (BigInteger)s * s >= a);
        return s;
    }

    private static ulong FloorSqrt(ref UInt128 a, ulong s)
    {
        ulong sprev = 0;
        while (true)
        {
            // Equivalent to:
            // snext = (a / s + s) / 2;
            Divide(out var div, ref a, s);
            Add(out var sum, ref div, s);
            ulong snext = sum.S0 >> 1;
            if (sum.S1 != 0)
                snext |= (ulong)1 << 63;
            if (snext == sprev)
            {
                if (snext < s)
                    s = snext;
                break;
            }
            sprev = s;
            s = snext;
        }
        return s;
    }

    public static ulong FloorCbrt(UInt128 a)
    {
        ulong s = (ulong)Math.Pow(ConvertToDouble(ref a), (double)1 / 3);
        Cube(out var s3, s);
        if (a < s3)
            --s;
        else
        {
            Multiply(out var sum, 3 * s, s + 1);
            Subtract(out var diff, ref a, ref s3);
            if (LessThan(ref sum, ref diff))
                ++s;
        }
        Debug.Assert((BigInteger)s * s * s <= a && (BigInteger)(s + 1) * (s + 1) * (s + 1) > a);
        return s;
    }

    public static ulong CeilingCbrt(UInt128 a)
    {
        ulong s = (ulong)Math.Ceiling(Math.Pow(ConvertToDouble(ref a), (double)1 / 3));
        Cube(out var s3, s);
        if (s3 < a)
            ++s;
        else
        {
            Multiply(out var sum, 3 * s, s + 1);
            Subtract(out var diff, ref s3, ref a);
            if (LessThan(ref sum, ref diff))
                --s;
        }
        Debug.Assert((BigInteger)(s - 1) * (s - 1) * (s - 1) < a && (BigInteger)s * s * s >= a);
        return s;
    }

    public static UInt128 Min(UInt128 a, UInt128 b) => LessThan(ref a, ref b) ? a : b;

    public static UInt128 Max(UInt128 a, UInt128 b) => LessThan(ref b, ref a) ? a : b;

    public static UInt128 Clamp(UInt128 a, UInt128 min, UInt128 max) => LessThan(ref a, ref min) ? min : (LessThan(ref max, ref a) ? max : a);


    public static double Log(UInt128 a) => Log(a, Math.E);

    public static double Log10(UInt128 a) => Log(a, 10);

    public static double Log(UInt128 a, double b) => Math.Log(ConvertToDouble(ref a), b);

    public static UInt128 Add(UInt128 a, UInt128 b)
    {
        Add(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 Subtract(UInt128 a, UInt128 b)
    {
        Subtract(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 Multiply(UInt128 a, UInt128 b)
    {
        Multiply(out UInt128 c, ref a, ref b);
        return c;
    }

    public static UInt128 Divide(UInt128 a, UInt128 b)
    {
        Divide(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 Remainder(UInt128 a, UInt128 b)
    {
        Remainder(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 DivRem(UInt128 a, UInt128 b, out UInt128 remainder)
    {
        //TODO: Yuck!
        Divide(out var c, ref a, ref b);
        Remainder(out remainder, ref a, ref b);
        return c;
    }

    public static (UInt128 Quotient, UInt128 Remainder) DivRem(UInt128 a, UInt128 b)
    {
        var quotient = DivRem(a, b, out var remainder);
        return (quotient, remainder);
    }

    public static UInt128 ModAdd(UInt128 a, UInt128 b, UInt128 modulus)
    {
        ModAdd(out var c, ref a, ref b, ref modulus);
        return c;
    }

    public static UInt128 ModSub(UInt128 a, UInt128 b, UInt128 modulus)
    {
        ModSub(out var c, ref a, ref b, ref modulus);
        return c;
    }

    public static UInt128 ModMul(UInt128 a, UInt128 b, UInt128 modulus)
    {
        ModMul(out var c, ref a, ref b, ref modulus);
        return c;
    }

    public static UInt128 ModPow(UInt128 value, UInt128 exponent, UInt128 modulus)
    {
        ModPow(out var result, ref value, ref exponent, ref modulus);
        return result;
    }

    public static UInt128 Negate(UInt128 a)
    {
        Negate(out var c, ref a);
        return c;
    }

    public static UInt128 GreatestCommonDivisor(UInt128 a, UInt128 b)
    {
        GreatestCommonDivisor(out var c, ref a, ref b);
        return c;
    }

    private static void RightShift64(ref UInt128 c, int d)
    {
        if (d == 0)
            return;
        c.s0 = c.s1 << (64 - d) | c.s0 >> d;
        c.s1 >>= d;
    }

    public static void RightShift(ref UInt128 c, int d)
    {
        if (d < 64)
            RightShift64(ref c, d);
        else
        {
            c.s0 = c.s1 >> (d - 64);
            c.s1 = 0;
        }
    }

    public static void Shift(ref UInt128 c, int d)
    {
        if (d < 0)
            RightShift(ref c, -d);
        else
            LeftShift(ref c, d);
    }

    public static void ArithmeticShift(ref UInt128 c, int d)
    {
        if (d < 0)
            ArithmeticRightShift(ref c, -d);
        else
            LeftShift(ref c, d);
    }

    public static void RightShift(ref UInt128 c)
    {
        c.s0 = c.s1 << 63 | c.s0 >> 1;
        c.s1 >>= 1;
    }

    private static void ArithmeticRightShift64(ref UInt128 c, int d)
    {
        if (d == 0)
            return;
        c.s0 = c.s1 << (64 - d) | c.s0 >> d;
        c.s1 = (ulong)((long)c.s1 >> d);
    }

    public static void ArithmeticRightShift(ref UInt128 c, int d)
    {
        if (d < 64)
            ArithmeticRightShift64(ref c, d);
        else
        {
            c.s0 = (ulong)((long)c.s1 >> (d - 64));
            c.s1 = 0;
        }
    }

    public static void ArithmeticRightShift(ref UInt128 c)
    {
        c.s0 = c.s1 << 63 | c.s0 >> 1;
        c.s1 = (ulong)((long)c.s1 >> 1);
    }

    private static ulong LeftShift64(ref UInt128 c, int d)
    {
        if (d == 0)
            return 0;
        int dneg = 64 - d;
        ulong result = c.s1 >> dneg;
        c.s1 = c.s1 << d | c.s0 >> dneg;
        c.s0 <<= d;
        return result;
    }

    public static void LeftShift(ref UInt128 c, int d)
    {
        if (d < 64)
            LeftShift64(ref c, d);
        else
        {
            c.s1 = c.s0 << (d - 64);
            c.s0 = 0;
        }
    }

    public static void LeftShift(ref UInt128 c)
    {
        c.s1 = c.s1 << 1 | c.s0 >> 63;
        c.s0 <<= 1;
    }

    public static void Swap(ref UInt128 a, ref UInt128 b)
    {
        ulong as0 = a.s0;
        ulong as1 = a.s1;
        a.s0 = b.s0;
        a.s1 = b.s1;
        b.s0 = as0;
        b.s1 = as1;
    }

    public static void GreatestCommonDivisor(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        // Check whether one number is > 64 bits and the other is <= 64 bits and both are non-zero.
        UInt128 a1, b1;
        if ((a.s1 == 0) == (b.s1 == 0) || a.IsZero || b.IsZero)
        {
            a1 = a;
            b1 = b;
        }
        else
        {
            // Perform a normal step so that both a and b are <= 64 bits.
            if (LessThan(ref a, ref b))
            {
                a1 = a;
                Remainder(out b1, ref b, ref a);
            }
            else
            {
                b1 = b;
                Remainder(out a1, ref a, ref b);
            }
        }

        // Make sure neither is zero.
        if (a1.IsZero)
        {
            c = b1;
            return;
        }
        if (b1.IsZero)
        {
            c = a1;
            return;
        }

        // Ensure a >= b.
        if (LessThan(ref a1, ref b1))
            Swap(ref a1, ref b1);

        // Lehmer-Euclid algorithm.
        // See: http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.31.693
        while (a1.s1 != 0 && !b.IsZero)
        {
            // Extract the high 63 bits of a and b.
            int norm = 63 - GetBitLength(a1.s1);
            Shift(out var ahat, ref a1, norm);
            Shift(out var bhat, ref b1, norm);
            long uhat = (long)ahat.s1;
            long vhat = (long)bhat.s1;

            // Check whether q exceeds single-precision.
            if (vhat == 0)
            {
                // Perform a normal step and try again.
                Remainder(out var rem, ref a1, ref b1);
                a1 = b1;
                b1 = rem;
                continue;
            }

            // Perform steps using signed single-precision arithmetic.
            long x0 = 1;
            long y0 = 0;
            long x1 = 0;
            long y1 = 1;
            bool even = true;
            while (true)
            {
                // Calculate quotient, cosquence pair, and update uhat and vhat.
                long q = uhat / vhat;
                long x2 = x0 - q * x1;
                long y2 = y0 - q * y1;
                long t = uhat;
                uhat = vhat;
                vhat = t - q * vhat;
                even = !even;

                // Apply Jebelean's termination condition
                // to check whether q is valid.
                if (even)
                {
                    if (vhat < -x2 || uhat - vhat < y2 - y1)
                        break;
                }
                else
                {
                    if (vhat < -y2 || uhat - vhat < x2 - x1)
                        break;
                }

                // Adjust cosequence history.
                x0 = x1;
                y0 = y1;
                x1 = x2;
                y1 = y2;
            }

            // Check whether a normal step is necessary.
            if (x0 == 1 && y0 == 0)
            {
                Remainder(out var rem, ref a1, ref b1);
                a1 = b1;
                b1 = rem;
                continue;
            }

            // Back calculate a and b from the last valid cosequence pairs.
            UInt128 anew, bnew;
            if (even)
            {
                AddProducts(out anew, y0, ref b1, x0, ref a1);
                AddProducts(out bnew, x1, ref a1, y1, ref b1);
            }
            else
            {
                AddProducts(out anew, x0, ref a1, y0, ref b1);
                AddProducts(out bnew, y1, ref b1, x1, ref a1);
            }
            a1 = anew;
            b1 = bnew;
        }

        // Check whether we have any 64 bit work left.
        if (!b1.IsZero)
        {
            ulong a2 = a1.s0;
            ulong b2 = b1.s0;

            // Perform 64 bit steps.
            while (a2 > uint.MaxValue && b2 != 0)
            {
                ulong t = a2 % b2;
                a2 = b2;
                b2 = t;
            }

            // Check whether we have any 32 bit work left.
            if (b2 != 0)
            {
                uint a3 = (uint)a2;
                uint b3 = (uint)b2;

                // Perform 32 bit steps.
                while (b3 != 0)
                {
                    uint t = a3 % b3;
                    a3 = b3;
                    b3 = t;
                }

                Create(out c, a3);
            }
            else
                Create(out c, a2);
        }
        else
            c = a1;
    }

    private static void AddProducts(out UInt128 result, long x, ref UInt128 u, long y, ref UInt128 v)
    {
        // Compute x * u + y * v assuming y is negative and the result is positive and fits in 128 bits.
        Multiply(out var product1, ref u, (ulong)x);
        Multiply(out var product2, ref v, (ulong)(-y));
        Subtract(out result, ref product1, ref product2);
    }

    public static int Compare(UInt128 a, UInt128 b) => a.CompareTo(b);

    private static readonly byte[] bitLength = Enumerable.Range(0, byte.MaxValue + 1)
        .Select(value =>
        {
            int count;
            for (count = 0; value != 0; count++)
                value >>= 1;
            return (byte)count;
        }).ToArray();

    private static int GetBitLength(uint value)
    {
        uint tt = value >> 16;
        if (tt != 0)
        {
            uint t = tt >> 8;
            if (t != 0)
                return bitLength[t] + 24;
            return bitLength[tt] + 16;
        }
        else
        {
            uint t = value >> 8;
            if (t != 0)
                return bitLength[t] + 8;
            return bitLength[value];
        }
    }

    private static int GetBitLength(ulong value)
    {
        ulong r1 = value >> 32;
        return r1 != 0 ? GetBitLength((uint)r1) + 32 : GetBitLength((uint)value);
    }

    public static void Reduce(out UInt128 w, ref UInt128 u, ref UInt128 v, ref UInt128 n, ulong k0)
    {
        Multiply64(out var carry, u.s0, v.s0);
        ulong t0 = carry.s0;
        Multiply64(out carry, u.s1, v.s0, carry.s1);
        ulong t1 = carry.s0;
        ulong t2 = carry.s1;

        ulong m = t0 * k0;
        Multiply64(out carry, m, n.s1, MultiplyHigh64(m, n.s0, t0));
        Add(ref carry, t1);
        t0 = carry.s0;
        Add(out carry, carry.s1, t2);
        t1 = carry.s0;
        t2 = carry.s1;

        Multiply64(out carry, u.s0, v.s1, t0);
        t0 = carry.s0;
        Multiply64(out carry, u.s1, v.s1, carry.s1);
        Add(ref carry, t1);
        t1 = carry.s0;
        Add(out carry, carry.s1, t2);
        t2 = carry.s0;
        ulong t3 = carry.s1;

        m = t0 * k0;
        Multiply64(out carry, m, n.s1, MultiplyHigh64(m, n.s0, t0));
        Add(ref carry, t1);
        t0 = carry.s0;
        Add(out carry, carry.s1, t2);
        t1 = carry.s0;
        t2 = t3 + carry.s1;

        Create(out w, t0, t1);
        if (t2 != 0 || !LessThan(ref w, ref n))
            Subtract(ref w, ref n);
    }

    public static void Reduce(out UInt128 w, ref UInt128 t, ref UInt128 n, ulong k0)
    {
        ulong t0 = t.s0;
        ulong t1 = t.s1;
        ulong t2 = 0;

        for (int i = 0; i < 2; i++)
        {
            ulong m = t0 * k0;
            Multiply64(out var carry, m, n.s1, MultiplyHigh64(m, n.s0, t0));
            Add(ref carry, t1);
            t0 = carry.s0;
            Add(out carry, carry.s1, t2);
            t1 = carry.s0;
            t2 = carry.s1;
        }

        Create(out w, t0, t1);
        if (t2 != 0 || !LessThan(ref w, ref n))
            Subtract(ref w, ref n);
    }

    public static UInt128 Reduce(UInt128 u, UInt128 v, UInt128 n, ulong k0)
    {
        Reduce(out var w, ref u, ref v, ref n, k0);
        return w;
    }

    public static UInt128 Reduce(UInt128 t, UInt128 n, ulong k0)
    {
        Reduce(out var w, ref t, ref n, k0);
        return w;
    }

}
