namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.IO;

    [SectionKey("MTRL")]
    public sealed class MTRL: VirtualSection
    {
        public byte PaletteStart { get => GetValue<byte>(); set => SetValue(value); }
        public int PaletteCount { get => GetValue<int>(); set => SetValue(value); }

        public MTRL()
        {
            PaletteCount = 1;
        }

        protected override void OnPropertyChanging(string propertyName, object currentValue, object newValue)
        {
            if (propertyName == nameof(PaletteCount))
            {
                var newInt = (int)newValue;
                if (newInt <= 0)
                {
                    throw new ArgumentOutOfRangeException("newValue less than zero", "newValue");
                }
                if (newInt > byte.MaxValue)
                {
                    throw new ArgumentOutOfRangeException($"newValue greater than {byte.MaxValue}", "newValue");
                }
            }
            base.OnPropertyChanging(propertyName, currentValue, newValue);
        }

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);

            if (c.Read<UInt32>() != 0 || c.Read<UInt32>() != 0xFFFF0000 || c.Read<UInt16>() != 0)
                throw new InvalidDataException("MTRL header is invalid");

            PaletteStart = c.Read<Byte>();
            PaletteCount = c.Read<Byte>() + 1;

            if (c.Read<UInt32>() != 0x320000)
                throw new InvalidDataException("MTRL header is invalid");
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(0);
            c.Write(0xFFFF0000);
            c.Write((ushort)0);
            c.Write(PaletteStart);
            c.Write(checked((byte)(PaletteCount - 1)));
            c.Write((uint)0x320000);
        }
    }
}
