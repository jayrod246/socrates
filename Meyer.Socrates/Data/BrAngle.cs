using System;

namespace Meyer.Socrates.Data
{
    public struct BrAngle
    {
        public const int Size = 2;
        public static readonly BrAngle MaxValue = FromRaw(ushort.MaxValue);
        public static readonly BrAngle MinValue = FromRaw(ushort.MinValue);

        public ushort RawValue { get; set; }

        public BrAngle(float value)
        {
            try
            {
                RawValue = checked((ushort)((value / 360f) * 65536f));
            }
            catch (OverflowException)
            {
                if (System.Math.Ceiling(value) != 32768f) throw;
                this = MaxValue;
            }
        }

        public float AsSingle(bool radians = false) => radians ? (BrScalar.PI / 180f) * BrScalar.FromRaw(RawValue) * 360f : BrScalar.FromRaw(RawValue) * 360f;

        public static BrAngle FromRaw(ushort rawValue)
        {
            return new BrAngle() { RawValue = rawValue };
        }

        public static BrAngle FromDegrees(float deg)
        {
            return new BrAngle(deg);
        }

        public static BrAngle FromRadians(float deg)
        {
            return new BrAngle((BrScalar.PI / 180f) * deg);
        }

        public static implicit operator float(BrAngle x)
        {
            return x.AsSingle();
        }

        public static implicit operator BrAngle(float x)
        {
            return FromDegrees(x);
        }
    }
}
