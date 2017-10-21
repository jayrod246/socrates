namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;

    [SectionKey("GOKD")]
    public sealed class GOKD: VirtualSection
    {
        public int Unk1 { get => GetValue<int>(); set => SetValue(value); }
        public int Unk2 { get => GetValue<int>(); set => SetValue(value); }
        public int X { get => GetValue<int>(); set => SetValue(value); }
        public int Y { get => GetValue<int>(); set => SetValue(value); }
        public int Z { get => GetValue<int>(); set => SetValue(value); }
        public byte[] Extra { get => GetValue<byte[]>(); set => SetValue(value); }

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);
            Unk1 = c.Read<Int32>();
            Unk2 = c.Read<Int32>();
            X = c.Read<Int32>();
            Y = c.Read<Int32>();
            Z = c.Read<Int32>();
            Extra = c.ReadArray<Byte>((int)(c.Length - c.Position));
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(Unk1);
            c.Write(Unk2);
            c.Write(X);
            c.Write(Y);
            c.Write(Z);
            c.WriteArray(Extra ?? new byte[0]);
        }
    }
}
