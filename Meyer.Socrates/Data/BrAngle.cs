using System;

namespace Meyer.Socrates.Data
{
    public struct BrAngle
    {
        public const int Size = 2;
        public static readonly BrAngle MaxValue = new BrAngle(ushort.MaxValue);
        public static readonly BrAngle MinValue = new BrAngle(ushort.MinValue);

        public ushort RawValue { get; set; }

        internal BrAngle(ushort raw)
        {
            RawValue = raw;
        }

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

        public float AsSingle() => new BrScalar(RawValue) * 360f;

        public static BrAngle FromDegrees(float deg)
        {
            return new BrAngle(deg);
        }

        public static BrAngle FromRadians(float deg)
        {
            return new BrAngle((BrScalar.PI / 180f) * deg);
        }
    }
}
