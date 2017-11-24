using System;

namespace Meyer.Socrates.Data
{
    public struct BrAngle
    {
        public const int Size = 2;
        public static readonly BrAngle MaxValue = FromRaw(ushort.MaxValue);
        public static readonly BrAngle MinValue = FromRaw(ushort.MinValue);

        public ushort RawValue { get; set; }
        public float Value { get => AsSingle(); set => this = new BrAngle(value); }

        public BrAngle(float value)
        {
            value %= 360f;
            if (value < 0)
                value += 360f;
            RawValue = checked((ushort)((value / 360f) * 65536f));
        }

        public float AsSingle(bool radians = false)
        {
            return radians ? (BrScalar.PI / 180f) * BrScalar.FromRaw(RawValue) * 360f : BrScalar.FromRaw(RawValue) * 360f;
        }

        public static BrAngle FromRaw(ushort rawValue)
        {
            return new BrAngle() { RawValue = rawValue };
        }

        public static BrAngle FromDegrees(float deg)
        {
            return new BrAngle(deg);
        }

        public static BrAngle FromRadians(float rad)
        {
            return new BrAngle((180f / BrScalar.PI) * rad);
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
