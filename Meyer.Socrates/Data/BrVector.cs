using System;

namespace Meyer.Socrates.Data
{
    public static class BrVector
    {
        public static BrVector2 Create(BrScalar x, BrScalar y) => new BrVector2(x, y);
        public static BrVector3 Create(BrScalar x, BrScalar y, BrScalar z) => new BrVector3(x, y, z);
        public static BrVector4 Create(BrScalar x, BrScalar y, BrScalar z, BrScalar w) => new BrVector4(x, y, z, w);

        public static BrScalar GetMagnitude(BrVector2 vec)
        {
            return (BrScalar)Math.Sqrt(Math.Pow(vec.X, 2) + Math.Pow(vec.Y, 2));
        }

        public static BrScalar GetMagnitude(BrVector3 vec)
        {
            return (BrScalar)Math.Sqrt(Math.Pow(vec.X, 2) + Math.Pow(vec.Y, 2) + Math.Pow(vec.Z, 2));
        }

        public static BrScalar GetMagnitude(BrVector4 vec)
        {
            return (BrScalar)Math.Sqrt(Math.Pow(vec.X, 2) + Math.Pow(vec.Y, 2) + Math.Pow(vec.Z, 2) + Math.Pow(vec.W, 2));
        }
    }
}
