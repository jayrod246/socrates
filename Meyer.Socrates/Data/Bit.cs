namespace Meyer.Socrates.Data
{
    public struct Bit
    {
        private byte value;
        public static readonly Bit Zero = new Bit(0);
        public static readonly Bit One = new Bit(1);

        internal Bit(int value)
        {
            this.value = (byte)(value & 1);
        }

        public Bit(bool value) : this(value ? 1 : 0) { }

        public bool IsSet => this == One;

        public static implicit operator byte(Bit bit) => bit.value;

        public static explicit operator Bit(ulong value) => (value & 1) == 0 ? Zero : One;
        public static explicit operator Bit(uint value) => (value & 1) == 0 ? Zero : One;
        public static explicit operator Bit(ushort value) => (value & 1) == 0 ? Zero : One;
        public static explicit operator Bit(byte value) => (value & 1) == 0 ? Zero : One;
        public static explicit operator Bit(long value) => (value & 1) == 0 ? Zero : One;
        public static explicit operator Bit(int value) => (value & 1) == 0 ? Zero : One;
        public static explicit operator Bit(short value) => (value & 1) == 0 ? Zero : One;
        public static explicit operator Bit(sbyte value) => (value & 1) == 0 ? Zero : One;

        public static bool operator ==(Bit a, Bit b)
        {
            return a.value == b.value;
        }

        public static bool operator !=(Bit a, Bit b)
        {
            return a.value != b.value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Bit) return value.Equals(((Bit)obj).value);
            return value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
