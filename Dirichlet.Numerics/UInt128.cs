using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace Dirichlet.Numerics;

public struct UInt128 : IFormattable, IComparable, IComparable<UInt128>, IEquatable<UInt128>, IUnsignedNumber<UInt128> {
    private struct UInt256 {
        public ulong S0;
        public ulong S1;
        public ulong S2;
        public ulong S3;

        public uint R0 => (uint)S0;
        public uint R1 => (uint)(S0 >> 32);
        public uint R2 => (uint)S1;
        public uint R3 => (uint)(S1 >> 32);
        public uint R4 => (uint)S2;
        public uint R5 => (uint)(S2 >> 32);
        public uint R6 => (uint)S3;
        public uint R7 => (uint)(S3 >> 32);

        public UInt128 T0 {
            get {
                Create(out var result, S0, S1);
                return result;
            }
        }

        public UInt128 T1 {
            get {
                Create(out var result, S2, S3);
                return result;
            }
        }

        public static implicit operator BigInteger(UInt256 a) {
            return ((BigInteger)a.S3 << 192) | ((BigInteger)a.S2 << 128) | ((BigInteger)a.S1 << 64) | a.S0;
        }
    }

#pragma warning disable IDE0032 // Use auto property
    internal ulong _s0;
    internal ulong _s1;
#pragma warning restore IDE0032 // Use auto property

    public static UInt128 MinValue => Zero;
    public static UInt128 MaxValue { get; } = ~(UInt128)0;
    public static UInt128 AdditiveIdentity { get; } = (UInt128)0;
    public static UInt128 MultiplicativeIdentity { get; } = (UInt128)1;
    public static UInt128 Zero { get; } = (UInt128)0;
    public static UInt128 One { get; } = (UInt128)1;

    private uint R0 => (uint)_s0;
    private uint R1 => (uint)(_s0 >> 32);
    private uint R2 => (uint)_s1;
    private uint R3 => (uint)(_s1 >> 32);

    //  public ulong _s0 => _s0;
    //  public ulong _s1 => _s1;

    public bool IsZero => (_s0 | _s1) == 0;
    public bool IsOne => _s1 == 0 && _s0 == 1;
    public bool IsPowerOfTwo => (this & (this - 1)).IsZero;
    public bool IsEven => (_s0 & 1) == 0;
    public int Sign => IsZero ? 0 : 1;

    // static
    UInt128 INumber<UInt128>.Sign(UInt128 value) {
        return value.IsZero ? Zero : One;
    }

    public override string ToString() {
        return ((BigInteger)this).ToString();
    }

    public string ToString(string format) {
        return ((BigInteger)this).ToString(format);
    }

    public string ToString(IFormatProvider provider) {
        return ToString(null, provider);
    }

    public string ToString(string? format, IFormatProvider? provider) {
        return ((BigInteger)this).ToString(format, provider);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider) {
        return ((BigInteger)this).TryFormat(destination, out charsWritten, format, provider);
    }


    #region Parsing

    private const NumberStyles _StyleUnsignedInteger = NumberStyles.Integer & ~NumberStyles.AllowLeadingSign;

    public static UInt128 Parse(string s) {
        return TryParse(s, _StyleUnsignedInteger, NumberFormatInfo.CurrentInfo, out var c)
            ? c
            : throw new FormatException();
    }

    public static UInt128 Parse(string s, IFormatProvider? provider) {
        return TryParse(s, _StyleUnsignedInteger, provider, out var c) ? c : throw new FormatException();
    }

    public static UInt128 Parse(string s, NumberStyles style, IFormatProvider? provider) {
        return TryParse(s, style, provider, out var c) ? c : throw new FormatException();
    }

    public static UInt128 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) {
        return TryParse(s, _StyleUnsignedInteger, provider, out var c) ? c : throw new FormatException();
    }

    public static UInt128 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) {
        return TryParse(s, style, provider, out var c) ? c : throw new FormatException();
    }

    public static bool TryParse(string s, out UInt128 result) {
        return TryParse(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out UInt128 result) {
        return TryParse(s, _StyleUnsignedInteger, provider, out result);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider,
        out UInt128 result) {
        if (BigInteger.TryParse(s, style, provider, out var a)) {
            Create(out result, a);
            return true;
        }

        result = Zero;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out UInt128 result) {
        return TryParse(s, _StyleUnsignedInteger, provider, out result);
    }

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
        out UInt128 result) {
        if (BigInteger.TryParse(s, style, provider, out var a)) {
            Create(out result, a);
            return true;
        }

        result = Zero;
        return false;
    }

    #endregion Parsing


    #region Creation and Casting

    public UInt128(long value) {
        Create(out this, value);
    }

    public UInt128(ulong value) {
        Create(out this, value);
    }

    public UInt128(decimal value) {
        Create(out this, value);
    }

    public UInt128(double value) {
        Create(out this, value);
    }

    public UInt128(BigInteger value) {
        Create(out this, value);
    }

    public static void Create(out UInt128 c, uint r0, uint r1, uint r2, uint r3) {
        c._s0 = ((ulong)r1 << 32) | r0;
        c._s1 = ((ulong)r3 << 32) | r2;
    }

    public static void Create(out UInt128 c, ulong s0, ulong s1) {
        c._s0 = s0;
        c._s1 = s1;
    }

    public static void Create(out UInt128 c, long a) {
        c._s0 = (ulong)a;
        c._s1 = a < 0 ? ulong.MaxValue : 0;
    }

    public static void Create(out UInt128 c, ulong a) {
        c._s0 = a;
        c._s1 = 0;
    }

    public static void Create(out UInt128 c, decimal a) {
        var bits = decimal.GetBits(decimal.Truncate(a));
        Create(out c, (uint)bits[0], (uint)bits[1], (uint)bits[2], 0);
        if (a < 0)
            Negate(ref c);
    }

    public static void Create(out UInt128 c, BigInteger a) {
        var sign = a.Sign;
        if (sign == -1)
            a = -a;
        c._s0 = (ulong)(a & ulong.MaxValue);
        c._s1 = (ulong)(a >> 64);
        if (sign == -1)
            Negate(ref c);
    }

    public static void Create(out UInt128 c, double a) {
        var negate = false;
        if (a < 0) {
            negate = true;
            a = -a;
        }

        if (a <= ulong.MaxValue) {
            c._s0 = (ulong)a;
            c._s1 = 0;
        }
        else {
            var shift = Math.Max((int)Math.Ceiling(Math.Log(a, 2)) - 63, 0);
            c._s0 = (ulong)(a / Math.Pow(2, shift));
            c._s1 = 0;
            LeftShift(ref c, shift);
        }

        if (negate)
            Negate(ref c);
    }

    public static UInt128 Create<TOther>(TOther value) where TOther : INumber<TOther> {
        if (TryCreate(value, out var result))
            return result;
        throw new NotSupportedException();
    }


    public static bool TryCreate<TOther>(TOther value, out UInt128 result) where TOther : INumber<TOther> {
        var success = true;

        UInt128 Fail() {
            success = false;
            return default;
        }

        result = value switch {
            long a => new UInt128(a),
            ulong a => new UInt128(a),
            double a => new UInt128(a),
            decimal a => new UInt128(a),
            BigInteger a => new UInt128(a),
            Int128 a => new UInt128(a),
            float a => new UInt128(a),
            int a => new UInt128(a),
            uint a => new UInt128(a),
            short a => new UInt128(a),
            ushort a => new UInt128(a),
            char a => new UInt128(a),
            byte a => new UInt128(a),
            _ => Fail()
        };
        return success;
    }

    //TODO
    public static UInt128 CreateSaturating<TOther>(TOther value) where TOther : INumber<TOther> {
        throw new NotImplementedException();
    }

    //TODO
    public static UInt128 CreateTruncating<TOther>(TOther value) where TOther : INumber<TOther> {
        throw new NotImplementedException();
    }


    public static explicit operator UInt128(double a) {
        Create(out var c, a);
        return c;
    }

    public static explicit operator UInt128(sbyte a) {
        Create(out var c, a);
        return c;
    }

    public static implicit operator UInt128(byte a) {
        Create(out var c, a);
        return c;
    }

    public static explicit operator UInt128(short a) {
        Create(out var c, a);
        return c;
    }

    public static implicit operator UInt128(ushort a) {
        Create(out var c, a);
        return c;
    }

    public static explicit operator UInt128(int a) {
        Create(out var c, a);
        return c;
    }

    public static implicit operator UInt128(uint a) {
        Create(out var c, a);
        return c;
    }

    public static explicit operator UInt128(long a) {
        Create(out var c, a);
        return c;
    }

    public static implicit operator UInt128(ulong a) {
        Create(out var c, a);
        return c;
    }

    public static explicit operator UInt128(decimal a) {
        Create(out var c, a);
        return c;
    }

    public static explicit operator UInt128(BigInteger a) {
        Create(out var c, a);
        return c;
    }

    public static explicit operator float(UInt128 a) {
        return ConvertToFloat(ref a);
    }

    public static explicit operator double(UInt128 a) {
        return ConvertToDouble(ref a);
    }

    public static float ConvertToFloat(ref UInt128 a) {
        return a._s1 == 0 ? a._s0 : (a._s1 * (float)ulong.MaxValue) + a._s0;
    }

    public static double ConvertToDouble(ref UInt128 a) {
        return a._s1 == 0 ? a._s0 : (a._s1 * (double)ulong.MaxValue) + a._s0;
    }

    public static explicit operator sbyte(UInt128 a) {
        return (sbyte)a._s0;
    }

    public static explicit operator byte(UInt128 a) {
        return (byte)a._s0;
    }

    public static explicit operator short(UInt128 a) {
        return (short)a._s0;
    }

    public static explicit operator ushort(UInt128 a) {
        return (ushort)a._s0;
    }

    public static explicit operator int(UInt128 a) {
        return (int)a._s0;
    }

    public static explicit operator uint(UInt128 a) {
        return (uint)a._s0;
    }

    public static explicit operator long(UInt128 a) {
        return (long)a._s0;
    }

    public static explicit operator ulong(UInt128 a) {
        return a._s0;
    }

    public static explicit operator decimal(UInt128 a) {
        if (a._s1 == 0)
            return a._s0;
        var shift = Math.Max(0, 32 - GetBitLength(a._s1));

        RightShift(out _, ref a, shift);
        return new decimal((int)a.R0, (int)a.R1, (int)a.R2, false, (byte)shift);
    }

    public static implicit operator BigInteger(UInt128 a) {
        return a._s1 == 0 ? a._s0 : ((BigInteger)a._s1 << 64) | a._s0;
    }

    #endregion Creation and Casting


    public static UInt128 operator <<(UInt128 a, int b) {
        LeftShift(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator >> (UInt128 a, int b) {
        RightShift(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator &(UInt128 a, UInt128 b) {
        And(out var c, ref a, ref b);
        return c;
    }

    public static uint operator &(UInt128 a, uint b) {
        return (uint)a._s0 & b;
    }

    public static uint operator &(uint a, UInt128 b) {
        return a & (uint)b._s0;
    }

    public static ulong operator &(UInt128 a, ulong b) {
        return a._s0 & b;
    }

    public static ulong operator &(ulong a, UInt128 b) {
        return a & b._s0;
    }

    public static UInt128 operator |(UInt128 a, UInt128 b) {
        Or(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 operator ^(UInt128 a, UInt128 b) {
        ExclusiveOr(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 operator ~(UInt128 a) {
        Not(out var c, ref a);
        return c;
    }

    public static UInt128 operator +(UInt128 a, UInt128 b) {
        Add(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 operator +(UInt128 a, ulong b) {
        Add(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator +(ulong a, UInt128 b) {
        Add(out var c, ref b, a);
        return c;
    }

    public static UInt128 operator ++(UInt128 a) {
        Add(out var c, ref a, 1);
        return c;
    }

    public static UInt128 operator -(UInt128 a, UInt128 b) {
        Subtract(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 operator -(UInt128 a, ulong b) {
        Subtract(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator -(ulong a, UInt128 b) {
        Subtract(out var c, a, ref b);
        return c;
    }

    public static UInt128 operator --(UInt128 a) {
        Subtract(out var c, ref a, 1);
        return c;
    }

    public static UInt128 operator +(UInt128 a) {
        return a;
    }

    public static UInt128 operator -(UInt128 a) {
        Negate(ref a);
        return a;
    }

    public static UInt128 operator *(UInt128 a, uint b) {
        Multiply(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator *(uint a, UInt128 b) {
        Multiply(out var c, ref b, a);
        return c;
    }

    public static UInt128 operator *(UInt128 a, ulong b) {
        Multiply(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator *(ulong a, UInt128 b) {
        Multiply(out var c, ref b, a);
        return c;
    }

    public static UInt128 operator *(UInt128 a, UInt128 b) {
        Multiply(out UInt128 c, ref a, ref b);
        return c;
    }

    public static UInt128 operator /(UInt128 a, ulong b) {
        Divide(out var c, ref a, b);
        return c;
    }

    public static UInt128 operator /(UInt128 a, UInt128 b) {
        Divide(out var c, ref a, ref b);
        return c;
    }

    public static ulong operator %(UInt128 a, uint b) {
        return Remainder(ref a, b);
    }

    public static ulong operator %(UInt128 a, ulong b) {
        return Remainder(ref a, b);
    }

    public static UInt128 operator %(UInt128 a, UInt128 b) {
        Remainder(out var c, ref a, ref b);
        return c;
    }

    #region Comparison operators

    public static bool operator <(UInt128 a, UInt128 b) {
        return LessThan(ref a, ref b);
    }

    public static bool operator <(UInt128 a, int b) {
        return LessThan(ref a, b);
    }

    public static bool operator <(int a, UInt128 b) {
        return LessThan(a, ref b);
    }

    public static bool operator <(UInt128 a, uint b) {
        return LessThan(ref a, b);
    }

    public static bool operator <(uint a, UInt128 b) {
        return LessThan(a, ref b);
    }

    public static bool operator <(UInt128 a, long b) {
        return LessThan(ref a, b);
    }

    public static bool operator <(long a, UInt128 b) {
        return LessThan(a, ref b);
    }

    public static bool operator <(UInt128 a, ulong b) {
        return LessThan(ref a, b);
    }

    public static bool operator <(ulong a, UInt128 b) {
        return LessThan(a, ref b);
    }

    public static bool operator <=(UInt128 a, UInt128 b) {
        return !LessThan(ref b, ref a);
    }

    public static bool operator <=(UInt128 a, int b) {
        return !LessThan(b, ref a);
    }

    public static bool operator <=(int a, UInt128 b) {
        return !LessThan(ref b, a);
    }

    public static bool operator <=(UInt128 a, uint b) {
        return !LessThan(b, ref a);
    }

    public static bool operator <=(uint a, UInt128 b) {
        return !LessThan(ref b, a);
    }

    public static bool operator <=(UInt128 a, long b) {
        return !LessThan(b, ref a);
    }

    public static bool operator <=(long a, UInt128 b) {
        return !LessThan(ref b, a);
    }

    public static bool operator <=(UInt128 a, ulong b) {
        return !LessThan(b, ref a);
    }

    public static bool operator <=(ulong a, UInt128 b) {
        return !LessThan(ref b, a);
    }

    public static bool operator >(UInt128 a, UInt128 b) {
        return LessThan(ref b, ref a);
    }

    public static bool operator >(UInt128 a, int b) {
        return LessThan(b, ref a);
    }

    public static bool operator >(int a, UInt128 b) {
        return LessThan(ref b, a);
    }

    public static bool operator >(UInt128 a, uint b) {
        return LessThan(b, ref a);
    }

    public static bool operator >(uint a, UInt128 b) {
        return LessThan(ref b, a);
    }

    public static bool operator >(UInt128 a, long b) {
        return LessThan(b, ref a);
    }

    public static bool operator >(long a, UInt128 b) {
        return LessThan(ref b, a);
    }

    public static bool operator >(UInt128 a, ulong b) {
        return LessThan(b, ref a);
    }

    public static bool operator >(ulong a, UInt128 b) {
        return LessThan(ref b, a);
    }

    public static bool operator >=(UInt128 a, UInt128 b) {
        return !LessThan(ref a, ref b);
    }

    public static bool operator >=(UInt128 a, int b) {
        return !LessThan(ref a, b);
    }

    public static bool operator >=(int a, UInt128 b) {
        return !LessThan(a, ref b);
    }

    public static bool operator >=(UInt128 a, uint b) {
        return !LessThan(ref a, b);
    }

    public static bool operator >=(uint a, UInt128 b) {
        return !LessThan(a, ref b);
    }

    public static bool operator >=(UInt128 a, long b) {
        return !LessThan(ref a, b);
    }

    public static bool operator >=(long a, UInt128 b) {
        return !LessThan(a, ref b);
    }

    public static bool operator >=(UInt128 a, ulong b) {
        return !LessThan(ref a, b);
    }

    public static bool operator >=(ulong a, UInt128 b) {
        return !LessThan(a, ref b);
    }

    public static bool operator ==(UInt128 a, UInt128 b) {
        return a.Equals(b);
    }

    public static bool operator ==(UInt128 a, int b) {
        return a.Equals(b);
    }

    public static bool operator ==(int a, UInt128 b) {
        return b.Equals(a);
    }

    public static bool operator ==(UInt128 a, uint b) {
        return a.Equals(b);
    }

    public static bool operator ==(uint a, UInt128 b) {
        return b.Equals(a);
    }

    public static bool operator ==(UInt128 a, long b) {
        return a.Equals(b);
    }

    public static bool operator ==(long a, UInt128 b) {
        return b.Equals(a);
    }

    public static bool operator ==(UInt128 a, ulong b) {
        return a.Equals(b);
    }

    public static bool operator ==(ulong a, UInt128 b) {
        return b.Equals(a);
    }

    public static bool operator !=(UInt128 a, UInt128 b) {
        return !a.Equals(b);
    }

    public static bool operator !=(UInt128 a, int b) {
        return !a.Equals(b);
    }

    public static bool operator !=(int a, UInt128 b) {
        return !b.Equals(a);
    }

    public static bool operator !=(UInt128 a, uint b) {
        return !a.Equals(b);
    }

    public static bool operator !=(uint a, UInt128 b) {
        return !b.Equals(a);
    }

    public static bool operator !=(UInt128 a, long b) {
        return !a.Equals(b);
    }

    public static bool operator !=(long a, UInt128 b) {
        return !b.Equals(a);
    }

    public static bool operator !=(UInt128 a, ulong b) {
        return !a.Equals(b);
    }

    public static bool operator !=(ulong a, UInt128 b) {
        return !b.Equals(a);
    }

    #endregion

    public int CompareTo(UInt128 other) {
        return _s1 != other._s1 ? _s1.CompareTo(other._s1) : _s0.CompareTo(other._s0);
    }

    public int CompareTo(int other) {
        return _s1 != 0 || other < 0 ? 1 : _s0.CompareTo((ulong)other);
    }

    public int CompareTo(uint other) {
        return _s1 != 0 ? 1 : _s0.CompareTo(other);
    }

    public int CompareTo(long other) {
        return _s1 != 0 || other < 0 ? 1 : _s0.CompareTo((ulong)other);
    }

    public int CompareTo(ulong other) {
        return _s1 != 0 ? 1 : _s0.CompareTo(other);
    }

    public int CompareTo(object? obj) {
        return obj switch {
            null => 1,
            UInt128 => CompareTo((UInt128)obj),
            _ => throw new ArgumentException(null, nameof(obj))
        };
    }

    private static bool LessThan(ref UInt128 a, long b) {
        return b >= 0 && a._s1 == 0 && a._s0 < (ulong)b;
    }

    private static bool LessThan(long a, ref UInt128 b) {
        return a < 0 || b._s1 != 0 || (ulong)a < b._s0;
    }

    private static bool LessThan(ref UInt128 a, ulong b) {
        return a._s1 == 0 && a._s0 < b;
    }

    private static bool LessThan(ulong a, ref UInt128 b) {
        return b._s1 != 0 || a < b._s0;
    }

    private static bool LessThan(ref UInt128 a, ref UInt128 b) {
        return a._s1 != b._s1 ? a._s1 < b._s1 : a._s0 < b._s0;
    }

    public static bool Equals(ref UInt128 a, ref UInt128 b) {
        return a._s0 == b._s0 && a._s1 == b._s1;
    }

    public bool Equals(UInt128 other) {
        return _s0 == other._s0 && _s1 == other._s1;
    }

    public bool Equals(int other) {
        return other >= 0 && _s0 == (uint)other && _s1 == 0;
    }

    public bool Equals(uint other) {
        return _s0 == other && _s1 == 0;
    }

    public bool Equals(long other) {
        return other >= 0 && _s0 == (ulong)other && _s1 == 0;
    }

    public bool Equals(ulong other) {
        return _s0 == other && _s1 == 0;
    }

    public override bool Equals(object? obj) {
        return obj is UInt128 n && Equals(n);
    }

    public override int GetHashCode() {
        return _s0.GetHashCode() ^ _s1.GetHashCode();
    }

    public static void Multiply(out UInt128 c, ulong a, ulong b) {
        Multiply64(out c, a, b);
        Debug.Assert(c == a * (BigInteger)b);
    }

    public static void Multiply(out UInt128 c, ref UInt128 a, uint b) {
        if (a._s1 == 0)
            Multiply64(out c, a._s0, b);
        else
            Multiply128(out c, ref a, b);
        Debug.Assert(c == a * (BigInteger)b % ((BigInteger)1 << 128));
    }

    public static void Multiply(out UInt128 c, ref UInt128 a, ulong b) {
        if (a._s1 == 0)
            Multiply64(out c, a._s0, b);
        else
            Multiply128(out c, ref a, b);
        Debug.Assert(c == a * (BigInteger)b % ((BigInteger)1 << 128));
    }

    public static void Multiply(out UInt128 c, ref UInt128 a, ref UInt128 b) {
        if ((a._s1 | b._s1) == 0)
            Multiply64(out c, a._s0, b._s0);
        else if (a._s1 == 0)
            Multiply128(out c, ref b, a._s0);
        else if (b._s1 == 0)
            Multiply128(out c, ref a, b._s0);
        else
            Multiply128(out c, ref a, ref b);
        Debug.Assert(c == a * (BigInteger)b % ((BigInteger)1 << 128));
    }

    private static void Multiply(out UInt256 c, ref UInt128 a, ref UInt128 b) {
        Multiply64(out var c00, a._s0, b._s0);
        Multiply64(out var c01, a._s0, b._s1);
        Multiply64(out var c10, a._s1, b._s0);
        Multiply64(out var c11, a._s1, b._s1);
        uint carry1 = 0;
        uint carry2 = 0;
        c.S0 = c00._s0;
        c.S1 = Add(Add(c00._s1, c01._s0, ref carry1), c10._s0, ref carry1);
        c.S2 = Add(Add(Add(c01._s1, c10._s1, ref carry2), c11._s0, ref carry2), carry1, ref carry2);
        c.S3 = c11._s1 + carry2;
        Debug.Assert(c == a * (BigInteger)b);
    }

    public static UInt128 Abs(UInt128 a) {
        return a;
    }

    public static UInt128 Square(ulong a) {
        Square(out var c, a);
        return c;
    }

    public static UInt128 Square(UInt128 a) {
        Square(out var c, ref a);
        return c;
    }

    public static void Square(out UInt128 c, ulong a) {
        Square64(out c, a);
    }

    public static void Square(out UInt128 c, ref UInt128 a) {
        if (a._s1 == 0)
            Square64(out c, a._s0);
        else
            Multiply128(out c, ref a, ref a);
    }

    public static UInt128 Cube(ulong a) {
        Cube(out var c, a);
        return c;
    }

    public static UInt128 Cube(UInt128 a) {
        Cube(out var c, ref a);
        return c;
    }

    public static void Cube(out UInt128 c, ulong a) {
        Square(out var square, a);
        Multiply(out c, ref square, a);
    }

    public static void Cube(out UInt128 c, ref UInt128 a) {
        if (a._s1 == 0) {
            Square64(out var square, a._s0);
            Multiply(out c, ref square, a._s0);
        }
        else {
            Multiply128(out var square, ref a, ref a);
            Multiply128(out c, ref square, ref a);
        }
    }

    public static void Add(out UInt128 c, ulong a, ulong b) {
        c._s0 = a + b;
        c._s1 = 0;
        if (c._s0 < a && c._s0 < b)
            ++c._s1;
        Debug.Assert(c == a + (BigInteger)b);
    }

    public static void Add(out UInt128 c, ref UInt128 a, ulong b) {
        c._s0 = a._s0 + b;
        c._s1 = a._s1;
        if (c._s0 < a._s0 && c._s0 < b)
            ++c._s1;
        Debug.Assert(c == (a + (BigInteger)b) % ((BigInteger)1 << 128));
    }

    public static void Add(out UInt128 c, ref UInt128 a, ref UInt128 b) {
        c._s0 = a._s0 + b._s0;
        c._s1 = a._s1 + b._s1;
        if (c._s0 < a._s0 && c._s0 < b._s0)
            ++c._s1;
        Debug.Assert(c == (a + (BigInteger)b) % ((BigInteger)1 << 128));
    }

    private static ulong Add(ulong a, ulong b, ref uint carry) {
        var c = a + b;
        if (c < a && c < b)
            ++carry;
        return c;
    }

    public static void Add(ref UInt128 a, ulong b) {
        var sum = a._s0 + b;
        if (sum < a._s0 && sum < b)
            ++a._s1;
        a._s0 = sum;
    }

    public static void Add(ref UInt128 a, ref UInt128 b) {
        var sum = a._s0 + b._s0;
        if (sum < a._s0 && sum < b._s0)
            ++a._s1;
        a._s0 = sum;
        a._s1 += b._s1;
    }

    public static void Add(ref UInt128 a, UInt128 b) {
        Add(ref a, ref b);
    }

    public static void Subtract(out UInt128 c, ref UInt128 a, ulong b) {
        c._s0 = a._s0 - b;
        c._s1 = a._s1;
        if (a._s0 < b)
            --c._s1;
        Debug.Assert(c == (a - (BigInteger)b + ((BigInteger)1 << 128)) % ((BigInteger)1 << 128));
    }

    public static void Subtract(out UInt128 c, ulong a, ref UInt128 b) {
        c._s0 = a - b._s0;
        c._s1 = 0 - b._s1;
        if (a < b._s0)
            --c._s1;
        Debug.Assert(c == (a - (BigInteger)b + ((BigInteger)1 << 128)) % ((BigInteger)1 << 128));
    }

    public static void Subtract(out UInt128 c, ref UInt128 a, ref UInt128 b) {
        c._s0 = a._s0 - b._s0;
        c._s1 = a._s1 - b._s1;
        if (a._s0 < b._s0)
            --c._s1;
        Debug.Assert(c == (a - (BigInteger)b + ((BigInteger)1 << 128)) % ((BigInteger)1 << 128));
    }

    public static void Subtract(ref UInt128 a, ulong b) {
        if (a._s0 < b)
            --a._s1;
        a._s0 -= b;
    }

    public static void Subtract(ref UInt128 a, ref UInt128 b) {
        if (a._s0 < b._s0)
            --a._s1;
        a._s0 -= b._s0;
        a._s1 -= b._s1;
    }

    public static void Subtract(ref UInt128 a, UInt128 b) {
        Subtract(ref a, ref b);
    }

    private static void Square64(out UInt128 w, ulong u) {
        ulong u0 = (uint)u;
        var u1 = u >> 32;
        var carry = u0 * u0;
        var r0 = (uint)carry;
        var u0U1 = u0 * u1;
        carry = (carry >> 32) + u0U1;
        var r2 = carry >> 32;
        carry = (uint)carry + u0U1;
        w._s0 = (carry << 32) | r0;
        w._s1 = (carry >> 32) + r2 + (u1 * u1);
        Debug.Assert(w == (BigInteger)u * u);
    }

    private static void Multiply64(out UInt128 w, uint u, uint v) {
        w._s0 = (ulong)u * v;
        w._s1 = 0;
        Debug.Assert(w == (BigInteger)u * v);
    }

    private static void Multiply64(out UInt128 w, ulong u, uint v) {
        ulong u0 = (uint)u;
        var u1 = u >> 32;
        var carry = u0 * v;
        var r0 = (uint)carry;
        carry = (carry >> 32) + (u1 * v);
        w._s0 = (carry << 32) | r0;
        w._s1 = carry >> 32;
        Debug.Assert(w == (BigInteger)u * v);
    }

    private static void Multiply64(out UInt128 w, ulong u, ulong v) {
        ulong u0 = (uint)u;
        var u1 = u >> 32;
        ulong v0 = (uint)v;
        var v1 = v >> 32;
        var carry = u0 * v0;
        var r0 = (uint)carry;
        carry = (carry >> 32) + (u0 * v1);
        var r2 = carry >> 32;
        carry = (uint)carry + (u1 * v0);
        w._s0 = (carry << 32) | r0;
        w._s1 = (carry >> 32) + r2 + (u1 * v1);
        Debug.Assert(w == (BigInteger)u * v);
    }

    private static void Multiply64(out UInt128 w, ulong u, ulong v, ulong c) {
        ulong u0 = (uint)u;
        var u1 = u >> 32;
        ulong v0 = (uint)v;
        var v1 = v >> 32;
        var carry = (u0 * v0) + (uint)c;
        var r0 = (uint)carry;
        carry = (carry >> 32) + (u0 * v1) + (c >> 32);
        var r2 = carry >> 32;
        carry = (uint)carry + (u1 * v0);
        w._s0 = (carry << 32) | r0;
        w._s1 = (carry >> 32) + r2 + (u1 * v1);
        Debug.Assert(w == ((BigInteger)u * v) + c);
    }

    private static ulong MultiplyHigh64(ulong u, ulong v, ulong c) {
        ulong u0 = (uint)u;
        var u1 = u >> 32;
        ulong v0 = (uint)v;
        var v1 = v >> 32;
        var carry = (((u0 * v0) + (uint)c) >> 32) + (u0 * v1) + (c >> 32);
        var r2 = carry >> 32;
        carry = (uint)carry + (u1 * v0);
        return (carry >> 32) + r2 + (u1 * v1);
    }

    private static void Multiply128(out UInt128 w, ref UInt128 u, uint v) {
        Multiply64(out w, u._s0, v);
        w._s1 += u._s1 * v;
        Debug.Assert(w == (BigInteger)u * v % ((BigInteger)1 << 128));
    }

    private static void Multiply128(out UInt128 w, ref UInt128 u, ulong v) {
        Multiply64(out w, u._s0, v);
        w._s1 += u._s1 * v;
        Debug.Assert(w == (BigInteger)u * v % ((BigInteger)1 << 128));
    }

    private static void Multiply128(out UInt128 w, ref UInt128 u, ref UInt128 v) {
        Multiply64(out w, u._s0, v._s0);
        w._s1 += (u._s1 * v._s0) + (u._s0 * v._s1);
        Debug.Assert(w == (BigInteger)u * v % ((BigInteger)1 << 128));
    }

    public static void Divide(out UInt128 w, ref UInt128 u, uint v) {
        if (u._s1 == 0)
            Divide64(out w, u._s0, v);
        else if (u._s1 <= uint.MaxValue)
            Divide96(out w, ref u, v);
        else
            Divide128(out w, ref u, v);
    }

    public static void Divide(out UInt128 w, ref UInt128 u, ulong v) {
        if (u._s1 == 0) {
            Divide64(out w, u._s0, v);
        }
        else {
            var v0 = (uint)v;
            if (v == v0) {
                if (u._s1 <= uint.MaxValue)
                    Divide96(out w, ref u, v0);
                else
                    Divide128(out w, ref u, v0);
            }
            else {
                if (u._s1 <= uint.MaxValue)
                    Divide96(out w, ref u, v);
                else
                    Divide128(out w, ref u, v);
            }
        }
    }

    public static void Divide(out UInt128 c, ref UInt128 a, ref UInt128 b) {
        if (LessThan(ref a, ref b))
            c = Zero;
        else if (b._s1 == 0)
            Divide(out c, ref a, b._s0);
        else if (b._s1 <= uint.MaxValue)
            Create(out c, DivRem96(out _, ref a, ref b));
        else
            Create(out c, DivRem128(out _, ref a, ref b));
    }

    public static uint Remainder(ref UInt128 u, uint v) {
        return u._s1 switch {
            0 => (uint)(u._s0 % v),
            <= uint.MaxValue => Remainder96(ref u, v),
            _ => Remainder128(ref u, v)
        };
    }

    public static ulong Remainder(ref UInt128 u, ulong v) {
        if (u._s1 == 0)
            return u._s0 % v;
        var v0 = (uint)v;
        return v == v0
            ? u._s1 <= uint.MaxValue ? Remainder96(ref u, v0) : Remainder128(ref u, v0)
            : u._s1 <= uint.MaxValue
                ? Remainder96(ref u, v)
                : Remainder128(ref u, v);
    }

    public static void Remainder(out UInt128 c, ref UInt128 a, ref UInt128 b) {
        if (LessThan(ref a, ref b))
            c = a;
        else if (b._s1 == 0)
            Create(out c, Remainder(ref a, b._s0));
        else if (b._s1 <= uint.MaxValue)
            DivRem96(out c, ref a, ref b);
        else
            DivRem128(out c, ref a, ref b);
    }

    public static void Remainder(ref UInt128 a, ref UInt128 b) {
        var a2 = a;
        Remainder(out a, ref a2, ref b);
    }

    private static void Remainder(out UInt128 c, ref UInt256 a, ref UInt128 b) {
        if (b.R3 == 0)
            Remainder192(out c, ref a, ref b);
        else
            Remainder256(out c, ref a, ref b);
    }

    private static void Divide64(out UInt128 w, ulong u, ulong v) {
        w._s1 = 0;
        w._s0 = u / v;
        Debug.Assert(w == (BigInteger)u / v);
    }

    private static void Divide96(out UInt128 w, ref UInt128 u, uint v) {
        var r2 = u.R2;
        var w2 = r2 / v;
        ulong u0 = r2 - (w2 * v);
        var u0U1 = (u0 << 32) | u.R1;
        var w1 = (uint)(u0U1 / v);
        u0 = u0U1 - (w1 * v);
        u0U1 = (u0 << 32) | u.R0;
        var w0 = (uint)(u0U1 / v);
        w._s1 = w2;
        w._s0 = ((ulong)w1 << 32) | w0;
        Debug.Assert(w == (BigInteger)u / v);
    }

    private static void Divide128(out UInt128 w, ref UInt128 u, uint v) {
        var r3 = u.R3;
        var w3 = r3 / v;
        ulong u0 = r3 - (w3 * v);
        var u0U1 = (u0 << 32) | u.R2;
        var w2 = (uint)(u0U1 / v);
        u0 = u0U1 - (w2 * v);
        u0U1 = (u0 << 32) | u.R1;
        var w1 = (uint)(u0U1 / v);
        u0 = u0U1 - (w1 * v);
        u0U1 = (u0 << 32) | u.R0;
        var w0 = (uint)(u0U1 / v);
        w._s1 = ((ulong)w3 << 32) | w2;
        w._s0 = ((ulong)w1 << 32) | w0;
        Debug.Assert(w == (BigInteger)u / v);
    }

    private static void Divide96(out UInt128 w, ref UInt128 u, ulong v) {
        w._s0 = w._s1 = 0;
        var dneg = GetBitLength((uint)(v >> 32));
        var d = 32 - dneg;
        var vPrime = v << d;
        var v1 = (uint)(vPrime >> 32);
        var v2 = (uint)vPrime;
        var r0 = u.R0;
        var r1 = u.R1;
        var r2 = u.R2;
        uint r3 = 0;
        if (d != 0) {
            r3 = r2 >> dneg;
            r2 = (r2 << d) | (r1 >> dneg);
            r1 = (r1 << d) | (r0 >> dneg);
            r0 <<= d;
        }

        var q1 = DivRem(r3, ref r2, ref r1, v1, v2);
        var q0 = DivRem(r2, ref r1, ref r0, v1, v2);
        w._s0 = ((ulong)q1 << 32) | q0;
        w._s1 = 0;
        Debug.Assert(w == (BigInteger)u / v);
    }

    private static void Divide128(out UInt128 w, ref UInt128 u, ulong v) {
        w._s0 = w._s1 = 0;
        var dneg = GetBitLength((uint)(v >> 32));
        var d = 32 - dneg;
        var vPrime = v << d;
        var v1 = (uint)(vPrime >> 32);
        var v2 = (uint)vPrime;
        var r0 = u.R0;
        var r1 = u.R1;
        var r2 = u.R2;
        var r3 = u.R3;
        uint r4 = 0;
        if (d != 0) {
            r4 = r3 >> dneg;
            r3 = (r3 << d) | (r2 >> dneg);
            r2 = (r2 << d) | (r1 >> dneg);
            r1 = (r1 << d) | (r0 >> dneg);
            r0 <<= d;
        }

        w._s1 = DivRem(r4, ref r3, ref r2, v1, v2);
        var q1 = DivRem(r3, ref r2, ref r1, v1, v2);
        var q0 = DivRem(r2, ref r1, ref r0, v1, v2);
        w._s0 = ((ulong)q1 << 32) | q0;
        Debug.Assert(w == (BigInteger)u / v);
    }

    private static uint Remainder96(ref UInt128 u, uint v) {
        ulong u0 = u.R2 % v;
        var u0U1 = (u0 << 32) | u.R1;
        u0 = u0U1 % v;
        u0U1 = (u0 << 32) | u.R0;
        return (uint)(u0U1 % v);
    }

    private static uint Remainder128(ref UInt128 u, uint v) {
        ulong u0 = u.R3 % v;
        var u0U1 = (u0 << 32) | u.R2;
        u0 = u0U1 % v;
        u0U1 = (u0 << 32) | u.R1;
        u0 = u0U1 % v;
        u0U1 = (u0 << 32) | u.R0;
        return (uint)(u0U1 % v);
    }

    private static ulong Remainder96(ref UInt128 u, ulong v) {
        var dneg = GetBitLength((uint)(v >> 32));
        var d = 32 - dneg;
        var vPrime = v << d;
        var v1 = (uint)(vPrime >> 32);
        var v2 = (uint)vPrime;
        var r0 = u.R0;
        var r1 = u.R1;
        var r2 = u.R2;
        uint r3 = 0;
        if (d != 0) {
            r3 = r2 >> dneg;
            r2 = (r2 << d) | (r1 >> dneg);
            r1 = (r1 << d) | (r0 >> dneg);
            r0 <<= d;
        }

        DivRem(r3, ref r2, ref r1, v1, v2);
        DivRem(r2, ref r1, ref r0, v1, v2);
        return (((ulong)r1 << 32) | r0) >> d;
    }

    private static ulong Remainder128(ref UInt128 u, ulong v) {
        var dneg = GetBitLength((uint)(v >> 32));
        var d = 32 - dneg;
        var vPrime = v << d;
        var v1 = (uint)(vPrime >> 32);
        var v2 = (uint)vPrime;
        var r0 = u.R0;
        var r1 = u.R1;
        var r2 = u.R2;
        var r3 = u.R3;
        uint r4 = 0;
        if (d != 0) {
            r4 = r3 >> dneg;
            r3 = (r3 << d) | (r2 >> dneg);
            r2 = (r2 << d) | (r1 >> dneg);
            r1 = (r1 << d) | (r0 >> dneg);
            r0 <<= d;
        }

        DivRem(r4, ref r3, ref r2, v1, v2);
        DivRem(r3, ref r2, ref r1, v1, v2);
        DivRem(r2, ref r1, ref r0, v1, v2);
        return (((ulong)r1 << 32) | r0) >> d;
    }

    private static ulong DivRem96(out UInt128 rem, ref UInt128 a, ref UInt128 b) {
        var d = 32 - GetBitLength(b.R2);
        LeftShift64(out var v, ref b, d);
        var r4 = (uint)LeftShift64(out rem, ref a, d);
        var v1 = v.R2;
        var v2 = v.R1;
        var v3 = v.R0;
        var r3 = rem.R3;
        var r2 = rem.R2;
        var r1 = rem.R1;
        var r0 = rem.R0;
        var q1 = DivRem(r4, ref r3, ref r2, ref r1, v1, v2, v3);
        var q0 = DivRem(r3, ref r2, ref r1, ref r0, v1, v2, v3);
        Create(out rem, r0, r1, r2, 0);
        var div = ((ulong)q1 << 32) | q0;
        RightShift64(ref rem, d);
        Debug.Assert((BigInteger)div == a / (BigInteger)b);
        Debug.Assert(rem == a % (BigInteger)b);
        return div;
    }

    private static uint DivRem128(out UInt128 rem, ref UInt128 a, ref UInt128 b) {
        var d = 32 - GetBitLength(b.R3);
        LeftShift64(out var v, ref b, d);
        var r4 = (uint)LeftShift64(out rem, ref a, d);
        var r3 = rem.R3;
        var r2 = rem.R2;
        var r1 = rem.R1;
        var r0 = rem.R0;
        var div = DivRem(r4, ref r3, ref r2, ref r1, ref r0, v.R3, v.R2, v.R1, v.R0);
        Create(out rem, r0, r1, r2, r3);
        RightShift64(ref rem, d);
        Debug.Assert((BigInteger)div == a / (BigInteger)b);
        Debug.Assert(rem == a % (BigInteger)b);
        return div;
    }

    private static void Remainder192(out UInt128 c, ref UInt256 a, ref UInt128 b) {
        var d = 32 - GetBitLength(b.R2);
        LeftShift64(out var v, ref b, d);
        var v1 = v.R2;
        var v2 = v.R1;
        var v3 = v.R0;
        LeftShift64(out var rem, ref a, d);
        var r6 = rem.R6;
        var r5 = rem.R5;
        var r4 = rem.R4;
        var r3 = rem.R3;
        var r2 = rem.R2;
        var r1 = rem.R1;
        var r0 = rem.R0;
        DivRem(r6, ref r5, ref r4, ref r3, v1, v2, v3);
        DivRem(r5, ref r4, ref r3, ref r2, v1, v2, v3);
        DivRem(r4, ref r3, ref r2, ref r1, v1, v2, v3);
        DivRem(r3, ref r2, ref r1, ref r0, v1, v2, v3);
        Create(out c, r0, r1, r2, 0);
        RightShift64(ref c, d);
        Debug.Assert(c == a % (BigInteger)b);
    }

    private static void Remainder256(out UInt128 c, ref UInt256 a, ref UInt128 b) {
        var d = 32 - GetBitLength(b.R3);
        LeftShift64(out var v, ref b, d);
        var v1 = v.R3;
        var v2 = v.R2;
        var v3 = v.R1;
        var v4 = v.R0;
        var r8 = (uint)LeftShift64(out var rem, ref a, d);
        var r7 = rem.R7;
        var r6 = rem.R6;
        var r5 = rem.R5;
        var r4 = rem.R4;
        var r3 = rem.R3;
        var r2 = rem.R2;
        var r1 = rem.R1;
        var r0 = rem.R0;
        DivRem(r8, ref r7, ref r6, ref r5, ref r4, v1, v2, v3, v4);
        DivRem(r7, ref r6, ref r5, ref r4, ref r3, v1, v2, v3, v4);
        DivRem(r6, ref r5, ref r4, ref r3, ref r2, v1, v2, v3, v4);
        DivRem(r5, ref r4, ref r3, ref r2, ref r1, v1, v2, v3, v4);
        DivRem(r4, ref r3, ref r2, ref r1, ref r0, v1, v2, v3, v4);
        Create(out c, r0, r1, r2, r3);
        RightShift64(ref c, d);
        Debug.Assert(c == a % (BigInteger)b);
    }

    private static ulong Q(uint u0, uint u1, uint u2, uint v1, uint v2) {
        var u0U1 = ((ulong)u0 << 32) | u1;
        var qhat = u0 == v1 ? uint.MaxValue : u0U1 / v1;
        var r = u0U1 - (qhat * v1);
        if (r == (uint)r && v2 * qhat > ((r << 32) | u2)) {
            --qhat;
            r += v1;
            if (r == (uint)r && v2 * qhat > ((r << 32) | u2))
                --qhat;
            //r += v1; //redundant
        }

        return qhat;
    }

    private static uint DivRem(uint u0, ref uint u1, ref uint u2, uint v1, uint v2) {
        var qhat = Q(u0, u1, u2, v1, v2);
        var carry = qhat * v2;
        var borrow = (long)u2 - (uint)carry;
        carry >>= 32;
        u2 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v1;
        borrow += (long)u1 - (uint)carry;
        carry >>= 32;
        u1 = (uint)borrow;
        borrow >>= 32;
        borrow += (long)u0 - (uint)carry;
        if (borrow != 0) {
            --qhat;
            carry = (ulong)u2 + v2;
            u2 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u1 + v1;
            u1 = (uint)carry;
        }

        return (uint)qhat;
    }

    private static uint DivRem(uint u0, ref uint u1, ref uint u2, ref uint u3, uint v1, uint v2, uint v3) {
        var qhat = Q(u0, u1, u2, v1, v2);
        var carry = qhat * v3;
        var borrow = (long)u3 - (uint)carry;
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
        if (borrow != 0) {
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

    private static uint DivRem(uint u0, ref uint u1, ref uint u2, ref uint u3, ref uint u4, uint v1, uint v2, uint v3,
        uint v4) {
        var qhat = Q(u0, u1, u2, v1, v2);
        var carry = qhat * v4;
        var borrow = (long)u4 - (uint)carry;
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
        if (borrow != 0) {
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

    public static void ModAdd(out UInt128 c, ref UInt128 a, ref UInt128 b, ref UInt128 modulus) {
        Add(out c, ref a, ref b);
        if (!LessThan(ref c, ref modulus) || (LessThan(ref c, ref a) && LessThan(ref c, ref b)))
            Subtract(ref c, ref modulus);
    }

    public static void ModSub(out UInt128 c, ref UInt128 a, ref UInt128 b, ref UInt128 modulus) {
        Subtract(out c, ref a, ref b);
        if (LessThan(ref a, ref b))
            Add(ref c, ref modulus);
    }

    public static void ModMul(out UInt128 c, ref UInt128 a, ref UInt128 b, ref UInt128 modulus) {
        if (modulus._s1 == 0) {
            Multiply64(out var product, a._s0, b._s0);
            Create(out c, Remainder(ref product, modulus._s0));
        }
        else {
            Multiply(out UInt256 product, ref a, ref b);
            Remainder(out c, ref product, ref modulus);
        }
    }

    public static void ModMul(ref UInt128 a, ref UInt128 b, ref UInt128 modulus) {
        if (modulus._s1 == 0) {
            Multiply64(out var product, a._s0, b._s0);
            Create(out a, Remainder(ref product, modulus._s0));
        }
        else {
            Multiply(out UInt256 product, ref a, ref b);
            Remainder(out a, ref product, ref modulus);
        }
    }

    public static void ModPow(out UInt128 result, ref UInt128 value, ref UInt128 exponent, ref UInt128 modulus) {
        result = One;
        var v = value;
        var e = exponent._s0;
        if (exponent._s1 != 0) {
            for (var i = 0; i < 64; i++) {
                if ((e & 1) != 0)
                    ModMul(ref result, ref v, ref modulus);
                ModMul(ref v, ref v, ref modulus);
                e >>= 1;
            }

            e = exponent._s1;
        }

        while (e != 0) {
            if ((e & 1) != 0)
                ModMul(ref result, ref v, ref modulus);
            if (e != 1)
                ModMul(ref v, ref v, ref modulus);
            e >>= 1;
        }

        Debug.Assert(BigInteger.ModPow(value, exponent, modulus) == result);
    }

    public static void Shift(out UInt128 c, ref UInt128 a, int d) {
        if (d < 0)
            RightShift(out c, ref a, -d);
        else
            LeftShift(out c, ref a, d);
    }

    public static void ArithmeticShift(out UInt128 c, ref UInt128 a, int d) {
        if (d < 0)
            ArithmeticRightShift(out c, ref a, -d);
        else
            LeftShift(out c, ref a, d);
    }

    public static ulong LeftShift64(out UInt128 c, ref UInt128 a, int d) {
        if (d == 0) {
            c = a;
            return 0;
        }

        var dneg = 64 - d;
        c._s1 = (a._s1 << d) | (a._s0 >> dneg);
        c._s0 = a._s0 << d;
        return a._s1 >> dneg;
    }

    private static ulong LeftShift64(out UInt256 c, ref UInt256 a, int d) {
        if (d == 0) {
            c = a;
            return 0;
        }

        var dneg = 64 - d;
        c.S3 = (a.S3 << d) | (a.S2 >> dneg);
        c.S2 = (a.S2 << d) | (a.S1 >> dneg);
        c.S1 = (a.S1 << d) | (a.S0 >> dneg);
        c.S0 = a.S0 << d;
        return a.S3 >> dneg;
    }

    public static void LeftShift(out UInt128 c, ref UInt128 a, int b) {
        if (b < 64) {
            LeftShift64(out c, ref a, b);
        }
        else if (b == 64) {
            c._s0 = 0;
            c._s1 = a._s0;
        }
        else {
            c._s0 = 0;
            c._s1 = a._s0 << (b - 64);
        }
    }

    public static void RightShift64(out UInt128 c, ref UInt128 a, int b) {
        if (b == 0) {
            c = a;
        }
        else {
            c._s0 = (a._s0 >> b) | (a._s1 << (64 - b));
            c._s1 = a._s1 >> b;
        }
    }

    public static void RightShift(out UInt128 c, ref UInt128 a, int b) {
        if (b < 64) {
            RightShift64(out c, ref a, b);
        }
        else if (b == 64) {
            c._s0 = a._s1;
            c._s1 = 0;
        }
        else {
            c._s0 = a._s1 >> (b - 64);
            c._s1 = 0;
        }
    }

    public static void ArithmeticRightShift64(out UInt128 c, ref UInt128 a, int b) {
        if (b == 0) {
            c = a;
        }
        else {
            c._s0 = (a._s0 >> b) | (a._s1 << (64 - b));
            c._s1 = (ulong)((long)a._s1 >> b);
        }
    }

    public static void ArithmeticRightShift(out UInt128 c, ref UInt128 a, int b) {
        if (b < 64) {
            ArithmeticRightShift64(out c, ref a, b);
        }
        else if (b == 64) {
            c._s0 = a._s1;
            c._s1 = (ulong)((long)a._s1 >> 63);
        }
        else {
            c._s0 = a._s1 >> (b - 64);
            c._s1 = (ulong)((long)a._s1 >> 63);
        }
    }

    public static void And(out UInt128 c, ref UInt128 a, ref UInt128 b) {
        c._s0 = a._s0 & b._s0;
        c._s1 = a._s1 & b._s1;
    }

    public static void Or(out UInt128 c, ref UInt128 a, ref UInt128 b) {
        c._s0 = a._s0 | b._s0;
        c._s1 = a._s1 | b._s1;
    }

    public static void ExclusiveOr(out UInt128 c, ref UInt128 a, ref UInt128 b) {
        c._s0 = a._s0 ^ b._s0;
        c._s1 = a._s1 ^ b._s1;
    }

    public static void Not(out UInt128 c, ref UInt128 a) {
        c._s0 = ~a._s0;
        c._s1 = ~a._s1;
    }

    public static void Negate(ref UInt128 a) {
        var s0 = a._s0;
        a._s0 = 0 - s0;
        a._s1 = 0 - a._s1;
        if (s0 > 0)
            --a._s1;
    }


    public static void Negate(out UInt128 c, ref UInt128 a) {
        c._s0 = 0 - a._s0;
        c._s1 = 0 - a._s1;
        if (a._s0 > 0)
            --c._s1;
        Debug.Assert(c == (BigInteger)(~a + 1));
    }

    public static void Pow(out UInt128 result, ref UInt128 value, uint exponent) {
        result = One;
        while (exponent != 0) {
            if ((exponent & 1) != 0) {
                var previous = result;
                Multiply(out result, ref previous, ref value);
            }

            if (exponent != 1) {
                var previous = value;
                Square(out value, ref previous);
            }

            exponent >>= 1;
        }
    }

    public static UInt128 Pow(UInt128 value, uint exponent) {
        Pow(out var result, ref value, exponent);
        return result;
    }

    private const int _MaxRepShift = 53;
    private static readonly ulong _MaxRep = (ulong)1 << _MaxRepShift;
    private static readonly UInt128 _MaxRepSquaredHigh = (ulong)1 << ((2 * _MaxRepShift) - 64);

    public static ulong FloorSqrt(UInt128 a) {
        if (a._s1 == 0 && a._s0 <= _MaxRep)
            return (ulong)Math.Sqrt(a._s0);

        var s = (ulong)Math.Sqrt(ConvertToDouble(ref a));
        if (a._s1 < _MaxRepSquaredHigh) {
            Square(out var s2, s);
            var r = a._s0 - s2._s0;
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

    public static ulong CeilingSqrt(UInt128 a) {
        if (a._s1 == 0 && a._s0 <= _MaxRep)
            return (ulong)Math.Ceiling(Math.Sqrt(a._s0));
        var s = (ulong)Math.Ceiling(Math.Sqrt(ConvertToDouble(ref a)));
        if (a._s1 < _MaxRepSquaredHigh) {
            Square(out var s2, s);
            var r = s2._s0 - a._s0;
            if (r > long.MaxValue)
                ++s;
            else if (r - (s << 1) <= long.MaxValue)
                --s;
            Debug.Assert((BigInteger)(s - 1) * (s - 1) < a && (BigInteger)s * s >= a);
            return s;
        }

        s = FloorSqrt(ref a, s);
        Square(out var square, s);
        if (square._s0 != a._s0 || square._s1 != a._s1)
            ++s;
        Debug.Assert((BigInteger)(s - 1) * (s - 1) < a && (BigInteger)s * s >= a);
        return s;
    }

    private static ulong FloorSqrt(ref UInt128 a, ulong s) {
        ulong sprev = 0;
        while (true) {
            // Equivalent to:
            // snext = (a / s + s) / 2;
            Divide(out var div, ref a, s);
            Add(out var sum, ref div, s);
            var snext = sum._s0 >> 1;
            if (sum._s1 != 0)
                snext |= (ulong)1 << 63;
            if (snext == sprev) {
                if (snext < s)
                    s = snext;
                break;
            }

            sprev = s;
            s = snext;
        }

        return s;
    }

    public static ulong FloorCbrt(UInt128 a) {
        var s = (ulong)Math.Pow(ConvertToDouble(ref a), (double)1 / 3);
        Cube(out var s3, s);
        if (a < s3) {
            --s;
        }
        else {
            Multiply(out var sum, 3 * s, s + 1);
            Subtract(out var diff, ref a, ref s3);
            if (LessThan(ref sum, ref diff))
                ++s;
        }

        Debug.Assert((BigInteger)s * s * s <= a && (BigInteger)(s + 1) * (s + 1) * (s + 1) > a);
        return s;
    }

    public static ulong CeilingCbrt(UInt128 a) {
        var s = (ulong)Math.Ceiling(Math.Pow(ConvertToDouble(ref a), (double)1 / 3));
        Cube(out var s3, s);
        if (s3 < a) {
            ++s;
        }
        else {
            Multiply(out var sum, 3 * s, s + 1);
            Subtract(out var diff, ref s3, ref a);
            if (LessThan(ref sum, ref diff))
                --s;
        }

        Debug.Assert((BigInteger)(s - 1) * (s - 1) * (s - 1) < a && (BigInteger)s * s * s >= a);
        return s;
    }

    public static UInt128 Min(UInt128 a, UInt128 b) {
        return LessThan(ref a, ref b) ? a : b;
    }

    public static UInt128 Max(UInt128 a, UInt128 b) {
        return LessThan(ref b, ref a) ? a : b;
    }

    public static UInt128 Clamp(UInt128 a, UInt128 min, UInt128 max) {
        return LessThan(ref a, ref min) ? min : LessThan(ref max, ref a) ? max : a;
    }


    public static double Log(UInt128 a) {
        return Log(a, Math.E);
    }

    public static double Log10(UInt128 a) {
        return Log(a, 10);
    }

    public static double Log(UInt128 a, double b) {
        return Math.Log(ConvertToDouble(ref a), b);
    }

    public static UInt128 Add(UInt128 a, UInt128 b) {
        Add(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 Subtract(UInt128 a, UInt128 b) {
        Subtract(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 Multiply(UInt128 a, UInt128 b) {
        Multiply(out UInt128 c, ref a, ref b);
        return c;
    }

    public static UInt128 Divide(UInt128 a, UInt128 b) {
        Divide(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 Remainder(UInt128 a, UInt128 b) {
        Remainder(out var c, ref a, ref b);
        return c;
    }

    public static UInt128 DivRem(UInt128 a, UInt128 b, out UInt128 remainder) {
        //TODO: Yuck!
        Divide(out var c, ref a, ref b);
        Remainder(out remainder, ref a, ref b);
        return c;
    }

    public static (UInt128 Quotient, UInt128 Remainder) DivRem(UInt128 a, UInt128 b) {
        var quotient = DivRem(a, b, out var remainder);
        return (quotient, remainder);
    }

    public static UInt128 ModAdd(UInt128 a, UInt128 b, UInt128 modulus) {
        ModAdd(out var c, ref a, ref b, ref modulus);
        return c;
    }

    public static UInt128 ModSub(UInt128 a, UInt128 b, UInt128 modulus) {
        ModSub(out var c, ref a, ref b, ref modulus);
        return c;
    }

    public static UInt128 ModMul(UInt128 a, UInt128 b, UInt128 modulus) {
        ModMul(out var c, ref a, ref b, ref modulus);
        return c;
    }

    public static UInt128 ModPow(UInt128 value, UInt128 exponent, UInt128 modulus) {
        ModPow(out var result, ref value, ref exponent, ref modulus);
        return result;
    }

    public static UInt128 Negate(UInt128 a) {
        Negate(out var c, ref a);
        return c;
    }

    public static UInt128 GreatestCommonDivisor(UInt128 a, UInt128 b) {
        GreatestCommonDivisor(out var c, ref a, ref b);
        return c;
    }

    private static void RightShift64(ref UInt128 c, int d) {
        if (d == 0)
            return;
        c._s0 = (c._s1 << (64 - d)) | (c._s0 >> d);
        c._s1 >>= d;
    }

    public static void RightShift(ref UInt128 c, int d) {
        if (d < 64) {
            RightShift64(ref c, d);
        }
        else {
            c._s0 = c._s1 >> (d - 64);
            c._s1 = 0;
        }
    }

    public static void Shift(ref UInt128 c, int d) {
        if (d < 0)
            RightShift(ref c, -d);
        else
            LeftShift(ref c, d);
    }

    public static void ArithmeticShift(ref UInt128 c, int d) {
        if (d < 0)
            ArithmeticRightShift(ref c, -d);
        else
            LeftShift(ref c, d);
    }

    public static void RightShift(ref UInt128 c) {
        c._s0 = (c._s1 << 63) | (c._s0 >> 1);
        c._s1 >>= 1;
    }

    private static void ArithmeticRightShift64(ref UInt128 c, int d) {
        if (d == 0)
            return;
        c._s0 = (c._s1 << (64 - d)) | (c._s0 >> d);
        c._s1 = (ulong)((long)c._s1 >> d);
    }

    public static void ArithmeticRightShift(ref UInt128 c, int d) {
        if (d < 64) {
            ArithmeticRightShift64(ref c, d);
        }
        else {
            c._s0 = (ulong)((long)c._s1 >> (d - 64));
            c._s1 = 0;
        }
    }

    public static void ArithmeticRightShift(ref UInt128 c) {
        c._s0 = (c._s1 << 63) | (c._s0 >> 1);
        c._s1 = (ulong)((long)c._s1 >> 1);
    }

    private static ulong LeftShift64(ref UInt128 c, int d) {
        if (d == 0)
            return 0;
        var dneg = 64 - d;
        var result = c._s1 >> dneg;
        c._s1 = (c._s1 << d) | (c._s0 >> dneg);
        c._s0 <<= d;
        return result;
    }

    public static void LeftShift(ref UInt128 c, int d) {
        if (d < 64) {
            LeftShift64(ref c, d);
        }
        else {
            c._s1 = c._s0 << (d - 64);
            c._s0 = 0;
        }
    }

    public static void LeftShift(ref UInt128 c) {
        c._s1 = (c._s1 << 1) | (c._s0 >> 63);
        c._s0 <<= 1;
    }

    public static void Swap(ref UInt128 a, ref UInt128 b) {
        var as0 = a._s0;
        var as1 = a._s1;
        a._s0 = b._s0;
        a._s1 = b._s1;
        b._s0 = as0;
        b._s1 = as1;
    }

    public static void GreatestCommonDivisor(out UInt128 c, ref UInt128 a, ref UInt128 b) {
        // Check whether one number is > 64 bits and the other is <= 64 bits and both are non-zero.
        UInt128 a1, b1;
        if (a._s1 == 0 == (b._s1 == 0) || a.IsZero || b.IsZero) {
            a1 = a;
            b1 = b;
        }
        else {
            // Perform a normal step so that both a and b are <= 64 bits.
            if (LessThan(ref a, ref b)) {
                a1 = a;
                Remainder(out b1, ref b, ref a);
            }
            else {
                b1 = b;
                Remainder(out a1, ref a, ref b);
            }
        }

        // Make sure neither is zero.
        if (a1.IsZero) {
            c = b1;
            return;
        }

        if (b1.IsZero) {
            c = a1;
            return;
        }

        // Ensure a >= b.
        if (LessThan(ref a1, ref b1))
            Swap(ref a1, ref b1);

        // Lehmer-Euclid algorithm.
        // See: http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.31.693
        while (a1._s1 != 0 && !b.IsZero) {
            // Extract the high 63 bits of a and b.
            var norm = 63 - GetBitLength(a1._s1);
            Shift(out var ahat, ref a1, norm);
            Shift(out var bhat, ref b1, norm);
            var uhat = (long)ahat._s1;
            var vhat = (long)bhat._s1;

            // Check whether q exceeds single-precision.
            if (vhat == 0) {
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
            var even = true;
            while (true) {
                // Calculate quotient, cosquence pair, and update uhat and vhat.
                var q = uhat / vhat;
                var x2 = x0 - (q * x1);
                var y2 = y0 - (q * y1);
                var t = uhat;
                uhat = vhat;
                vhat = t - (q * vhat);
                even = !even;

                // Apply Jebelean's termination condition
                // to check whether q is valid.
                if (even) {
                    if (vhat < -x2 || uhat - vhat < y2 - y1)
                        break;
                }
                else {
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
            if (x0 == 1 && y0 == 0) {
                Remainder(out var rem, ref a1, ref b1);
                a1 = b1;
                b1 = rem;
                continue;
            }

            // Back calculate a and b from the last valid cosequence pairs.
            UInt128 anew, bnew;
            if (even) {
                AddProducts(out anew, y0, ref b1, x0, ref a1);
                AddProducts(out bnew, x1, ref a1, y1, ref b1);
            }
            else {
                AddProducts(out anew, x0, ref a1, y0, ref b1);
                AddProducts(out bnew, y1, ref b1, x1, ref a1);
            }

            a1 = anew;
            b1 = bnew;
        }

        // Check whether we have any 64 bit work left.
        if (!b1.IsZero) {
            var a2 = a1._s0;
            var b2 = b1._s0;

            // Perform 64 bit steps.
            while (a2 > uint.MaxValue && b2 != 0) {
                var t = a2 % b2;
                a2 = b2;
                b2 = t;
            }

            // Check whether we have any 32 bit work left.
            if (b2 != 0) {
                var a3 = (uint)a2;
                var b3 = (uint)b2;

                // Perform 32 bit steps.
                while (b3 != 0) {
                    var t = a3 % b3;
                    a3 = b3;
                    b3 = t;
                }

                Create(out c, a3);
            }
            else {
                Create(out c, a2);
            }
        }
        else {
            c = a1;
        }
    }

    private static void AddProducts(out UInt128 result, long x, ref UInt128 u, long y, ref UInt128 v) {
        // Compute x * u + y * v assuming y is negative and the result is positive and fits in 128 bits.
        Multiply(out var product1, ref u, (ulong)x);
        Multiply(out var product2, ref v, (ulong)-y);
        Subtract(out result, ref product1, ref product2);
    }

    public static int Compare(UInt128 a, UInt128 b) {
        return a.CompareTo(b);
    }

    private static readonly byte[] _BitLength = Enumerable.Range(0, byte.MaxValue + 1)
        .Select(value => {
            int count;
            for (count = 0; value != 0; count++)
                value >>= 1;
            return (byte)count;
        }).ToArray();

    private static int GetBitLength(uint value) {
        var tt = value >> 16;
        if (tt != 0) {
            var t = tt >> 8;
            if (t != 0)
                return _BitLength[t] + 24;
            return _BitLength[tt] + 16;
        }
        else {
            var t = value >> 8;
            if (t != 0)
                return _BitLength[t] + 8;
            return _BitLength[value];
        }
    }

    private static int GetBitLength(ulong value) {
        var r1 = value >> 32;
        return r1 != 0 ? GetBitLength((uint)r1) + 32 : GetBitLength((uint)value);
    }

    public static void Reduce(out UInt128 w, ref UInt128 u, ref UInt128 v, ref UInt128 n, ulong k0) {
        Multiply64(out var carry, u._s0, v._s0);
        var t0 = carry._s0;
        Multiply64(out carry, u._s1, v._s0, carry._s1);
        var t1 = carry._s0;
        var t2 = carry._s1;

        var m = t0 * k0;
        Multiply64(out carry, m, n._s1, MultiplyHigh64(m, n._s0, t0));
        Add(ref carry, t1);
        t0 = carry._s0;
        Add(out carry, carry._s1, t2);
        t1 = carry._s0;
        t2 = carry._s1;

        Multiply64(out carry, u._s0, v._s1, t0);
        t0 = carry._s0;
        Multiply64(out carry, u._s1, v._s1, carry._s1);
        Add(ref carry, t1);
        t1 = carry._s0;
        Add(out carry, carry._s1, t2);
        t2 = carry._s0;
        var t3 = carry._s1;

        m = t0 * k0;
        Multiply64(out carry, m, n._s1, MultiplyHigh64(m, n._s0, t0));
        Add(ref carry, t1);
        t0 = carry._s0;
        Add(out carry, carry._s1, t2);
        t1 = carry._s0;
        t2 = t3 + carry._s1;

        Create(out w, t0, t1);
        if (t2 != 0 || !LessThan(ref w, ref n))
            Subtract(ref w, ref n);
    }

    public static void Reduce(out UInt128 w, ref UInt128 t, ref UInt128 n, ulong k0) {
        var t0 = t._s0;
        var t1 = t._s1;
        ulong t2 = 0;

        for (var i = 0; i < 2; i++) {
            var m = t0 * k0;
            Multiply64(out var carry, m, n._s1, MultiplyHigh64(m, n._s0, t0));
            Add(ref carry, t1);
            t0 = carry._s0;
            Add(out carry, carry._s1, t2);
            t1 = carry._s0;
            t2 = carry._s1;
        }

        Create(out w, t0, t1);
        if (t2 != 0 || !LessThan(ref w, ref n))
            Subtract(ref w, ref n);
    }

    public static UInt128 Reduce(UInt128 u, UInt128 v, UInt128 n, ulong k0) {
        Reduce(out var w, ref u, ref v, ref n, k0);
        return w;
    }

    public static UInt128 Reduce(UInt128 t, UInt128 n, ulong k0) {
        Reduce(out var w, ref t, ref n, k0);
        return w;
    }
}
