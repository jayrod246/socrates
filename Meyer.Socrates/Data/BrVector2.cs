using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meyer.Socrates.Data
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BrVector2
    {
        internal int v0;
        internal int v1;

        public static BrVector2 FromRaw(int rawX, int rawY)
        {
            return new BrVector2(BrScalar.FromRaw(rawX), BrScalar.FromRaw(rawY));
        }

        public BrVector2(BrScalar x, BrScalar y)
        {
            v0 = x.RawValue;
            v1 = y.RawValue;
        }

        public int RawX { get => v0; set => v0 = value; }

        public int RawY { get => v1; set => v1 = value; }

        public BrScalar X { get => BrScalar.FromRaw(v0); set => v0 = value.RawValue; }

        public BrScalar Y { get => BrScalar.FromRaw(v1); set => v1 = value.RawValue; }

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
                    default:
                        throw new ArgumentOutOfRangeException("index");
                }
            }
        }

        public static implicit operator BrVector3(BrVector2 v)
        {
            return new BrVector3(v.v0, v.v1, 0);
        }

        public static implicit operator BrVector4(BrVector2 v)
        {
            return new BrVector4(v.v0, v.v1, 0, 0);
        }

        public override string ToString()
        {
            return $"X: {X} Y: {Y}";
        }
    }
}
