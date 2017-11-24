using System;

namespace Meyer.Socrates.Data
{
    public partial struct BrScalar
    {
        public static readonly BrScalar Epsilon = FromRaw(0x0001);
        public static readonly BrScalar MaxValue = FromRaw(int.MaxValue);
        public static readonly BrScalar MinValue = FromRaw(int.MinValue);
        public static readonly BrScalar PI = 3.14159265358979323846f;

        public int RawValue { get; set; }
        public float AsSingle() => RawValue / 65536f;

        public static BrScalar FromRaw(int rawValue)
        {
            return new BrScalar() { RawValue = rawValue };
        }

        public BrScalar(float value)
        {
            try
            {
                RawValue = checked((int)(value * 65536f));
            }
            catch (OverflowException)
            {
                if (System.Math.Ceiling(value) != 32768f) throw;
                this = MaxValue;
            }
        }

        public override string ToString()
        {
            return AsSingle().ToString();
        }

        public BrScalar Clamped(BrScalar min, BrScalar max)
        {
            if (this < min) return min;
            if (this > max) return max;
            return this;
        }

        public static implicit operator BrScalar(float fl)
        {
            return new BrScalar(fl);
        }
        public static implicit operator float(BrScalar sc)
        {
            return sc.AsSingle();
        }
        public static implicit operator double(BrScalar sc)
        {
            return sc.AsSingle();
        }
        public static explicit operator long(BrScalar sc)
        {
            return (long)sc.AsSingle();
        }
        public static explicit operator ulong(BrScalar sc)
        {
            return (ulong)sc.AsSingle();
        }
        public static explicit operator int(BrScalar sc)
        {
            return (int)sc.AsSingle();
        }
        public static explicit operator uint(BrScalar sc)
        {
            return (uint)sc.AsSingle();
        }
        public static explicit operator short(BrScalar sc)
        {
            return (short)sc.AsSingle();
        }
        public static explicit operator ushort(BrScalar sc)
        {
            return (ushort)sc.AsSingle();
        }
        public static explicit operator byte(BrScalar sc)
        {
            return (byte)sc.AsSingle();
        }
        public static explicit operator sbyte(BrScalar sc)
        {
            return (sbyte)sc.AsSingle();
        }
        public static BrScalar operator *(BrScalar a, BrScalar b)
        {
            return Multiply(a, b);
        }
        public static BrScalar operator /(BrScalar a, BrScalar b)
        {
            return Divide(a, b);
        }
        public static bool operator <(BrScalar a, BrScalar b) => a.RawValue < b.RawValue;
        public static bool operator >(BrScalar a, BrScalar b) => a.RawValue > b.RawValue;
        public static BrScalar operator +(BrScalar a, BrScalar b) => FromRaw(a.RawValue + b.RawValue);
        public static BrScalar operator -(BrScalar a, BrScalar b) => FromRaw(a.RawValue - b.RawValue);
    }
}
