using System;

namespace Meyer.Socrates.Data
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BrVector4
    {
        internal int v0;
        internal int v1;
        internal int v2;
        internal int v3;

        public static BrVector4 FromRaw(int rawX, int rawY, int rawZ, int rawW)
        {
            return new BrVector4(BrScalar.FromRaw(rawX), BrScalar.FromRaw(rawY), BrScalar.FromRaw(rawZ), BrScalar.FromRaw(rawW));
        }

        public BrVector4(BrScalar x, BrScalar y, BrScalar z, BrScalar w)
        {
            v0 = x.RawValue;
            v1 = y.RawValue;
            v2 = z.RawValue;
            v3 = w.RawValue;
        }

        public BrScalar X
        {
            get => BrScalar.FromRaw(v0);
            set => v0 = value.RawValue;
        }

        public BrScalar Y
        {
            get => BrScalar.FromRaw(v1);
            set => v1 = value.RawValue;
        }

        public BrScalar Z
        {
            get => BrScalar.FromRaw(v2);
            set => v2 = value.RawValue;
        }

        public BrScalar W
        {
            get => BrScalar.FromRaw(v3);
            set => v3 = value.RawValue;
        }

        public BrScalar this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return BrScalar.FromRaw(v0);
                    case 1:
                        return BrScalar.FromRaw(v1);
                    case 2:
                        return BrScalar.FromRaw(v2);
                    case 3:
                        return BrScalar.FromRaw(v3);
                    default:
                        throw new ArgumentOutOfRangeException("index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        v0 = value.RawValue;
                        break;
                    case 1:
                        v1 = value.RawValue;
                        break;
                    case 2:
                        v2 = value.RawValue;
                        break;
                    case 3:
                        v3 = value.RawValue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("index");
                }
            }
        }

        public static implicit operator BrVector3(BrVector4 v)
        {
            return new BrVector3(v.v0, v.v1, v.v2);
        }

        public static explicit operator BrVector2(BrVector4 v)
        {
            return new BrVector2(v.v0, v.v1);
        }

        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z} W: {W}";
        }
    }
}
