using System;

namespace Meyer.Socrates.Data
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BrVector3
    {
        internal int v0;
        internal int v1;
        internal int v2;

        public const int Size = 12;

        public BrVector3(int rawX, int rawY, int rawZ)
        {
            v0 = rawX;
            v1 = rawY;
            v2 = rawZ;
        }

        public BrVector3(BrScalar x, BrScalar y, BrScalar z)
        {
            v0 = x.RawValue;
            v1 = y.RawValue;
            v2 = z.RawValue;
        }

        public BrScalar X
        {
            get
            {
                return new BrScalar(v0);
            }
            set
            {
                v0 = value.RawValue;
            }
        }

        public BrScalar Y
        {
            get
            {
                return new BrScalar(v1);
            }
            set
            {
                v1 = value.RawValue;
            }
        }

        public BrScalar Z
        {
            get
            {
                return new BrScalar(v2);
            }
            set
            {
                v2 = value.RawValue;
            }
        }

        public BrScalar this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return new BrScalar(v0);
                    case 1:
                        return new BrScalar(v1);
                    case 2:
                        return new BrScalar(v2);
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
                    default:
                        throw new ArgumentOutOfRangeException("index");
                }
            }
        }

        public static implicit operator BrVector4(BrVector3 v)
        {
            return new BrVector4(v.v0, v.v1, v.v2, 0);
        }

        public static explicit operator BrVector2(BrVector3 v)
        {
            return new BrVector2(v.v0, v.v1);
        }

        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z}";
        }
    }
}
