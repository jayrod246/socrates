namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;

    [SectionKey("FILL")]
    public sealed class FILL: VirtualSection
    {
        public int X1 { get => GetValue<int>(); set => SetValue(value); }
        public int Y1 { get => GetValue<int>(); set => SetValue(value); }
        public int X2 { get => GetValue<int>(); set => SetValue(value); }
        public int Y2 { get => GetValue<int>(); set => SetValue(value); }
        public byte[] Extra { get => GetValue<byte[]>(); set => SetValue(value); }

        public int Width
        {
            get => Math.Abs(X2 - X1);
            set
            {
                if (X1 < X2)
                    X2 = X1 + value;
                else
                    X1 = X2 + value;
            }
        }

        public int Height
        {
            get => Math.Abs(Y2 - Y1);
            set
            {
                if (Y1 < Y2)
                    Y2 = Y1 + value;
                else
                    Y1 = Y2 + value;
            }
        }

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);
            X1 = c.Read<Int32>();
            Y1 = c.Read<Int32>();
            X2 = c.Read<Int32>();
            Y2 = c.Read<Int32>();
            Extra = c.ReadArray<Byte>((int)(c.Length - c.Position));
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(X1);
            c.Write(Y1);
            c.Write(X2);
            c.Write(Y2);
            c.WriteArray(Extra);
        }
    }
}
