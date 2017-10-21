namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;

    [SectionKey("ZBMP")]
    public sealed class ZBMP: BitmapSection
    {
        public ZBMP() : this(1, 1, 0)
        { }

        public ZBMP(int width, int height) : this(width, height, 0)
        { }

        public ZBMP(int width, int height, ushort fill)
        {
            Width = width;
            Height = height;

            for (int y = 0;y < Height;y++)
                for (int x = 0;x < Width;x++)
                    SetPixel(x, y, fill);
        }

        public override PixelFormat PixelFormat => PixelFormat.Gray16;

        public ushort this[int x, int y]
        {
            get => RequireLoad(() => (ushort)GetPixelCore(x, y));
            set => RequireLoad(() => SetPixelCore(x, y, value));
        }

        public void SetPixel(int x, int y, ushort value)
        {
            RequireLoad(() => SetPixelCore(x, y, value));
        }

        public ushort GetPixel(int x, int y)
        {
            return RequireLoad(() => (ushort)GetPixelCore(x, y));
        }

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);
            c.Assert(0x00000000);
            Width = c.Read<UInt16>();
            Height = c.Read<UInt16>();
            Buffer.BlockCopy(c.ReadArray<UInt16>(Width * Height), 0, PixelBuffer, 0, Stride * Height);
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write((uint)0x00000000);
            c.Write((ushort)Width);
            c.Write((ushort)Height);
            c.WriteArray(PixelBuffer);
        }
    }
}
