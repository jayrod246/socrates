using System;

namespace Meyer.Socrates.Data
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BrColorRGB
    {
        internal byte b;
        internal byte g;
        internal byte r;
        internal byte a;

        public BrColorRGB(byte r, byte g, byte b) : this(r, g, b, 0xFF)
        {
        }

        public BrColorRGB(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public byte R
        {
            get => r;
            set => r = value;
        }

        public byte G
        {
            get => g;
            set => g = value;
        }

        public byte B
        {
            get => b;
            set => b = value;
        }

        public byte A
        {
            get => a;
            set => a = value;
        }

        public BrScalar ScR
        {
            get => new BrScalar(r / 255f);
            set => r = (byte)(value.Clamped(0f, 1f) * 255f);
        }

        public BrScalar ScG
        {
            get => new BrScalar(g / 255f);
            set => g = (byte)(value.Clamped(0f, 1f) * 255f);
        }

        public BrScalar ScB
        {
            get => new BrScalar(b / 255f);
            set => b = (byte)(value.Clamped(0f, 1f) * 255f);
        }

        public BrScalar ScA
        {
            get => new BrScalar(a / 255f);
            set => a = (byte)(value.Clamped(0f, 1f) * 255f);
        }

        public static explicit operator BrVector3(BrColorRGB c)
        {
            return new BrVector3(c.R / 255f, c.G / 255f, c.B / 255f);
        }

        public static explicit operator BrVector4(BrColorRGB c)
        {
            return new BrVector4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
        }
    }
}
