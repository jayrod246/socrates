namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;

    /// <summary>
    /// Image section used for texture maps.
    /// </summary>
    [SectionKey("TMAP")]
    public sealed class TMAP: BitmapSection, IIndexed8Image
    {
        public TMAP() : this(1, 1, 0)
        { }

        public TMAP(int width, int height) : this(width, height, 0)
        {

        }

        public TMAP(int width, int height, byte fill)
        {
            Width = width;
            Height = height;
            Unk1 = 0x0203;
            if (fill > byte.MinValue) Clear(fill);
        }

        #region Properties
        public short Unk1 { get => GetValue<short>(); set => SetValue(value); }
        public int Reserved1 { get => GetValue<int>(); set => SetValue(value); }
        public int Reserved2 { get => GetValue<int>(); set => SetValue(value); }
        public override PixelFormat PixelFormat => PixelFormat.Indexed8;

        public byte this[int x, int y]
        {
            get => RequireLoad(() => (byte)GetPixelCore(x, y));
            set => RequireLoad(() => SetPixelCore(x, y, value));
        }

        #endregion

        public void SetPixel(int x, int y, byte value)
        {
            RequireLoad(() => SetPixelCore(x, y, value));
        }

        public void Clear(byte fill)
        {
            RequireLoad(() => ClearCore(fill));
        }

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);
            Width = c.Read<Int16>();
            Unk1 = c.AssertAny<Int16>(0x0203, 0x0003);
            Reserved1 = c.Read<Int32>();
            c.Assert((Int16)Width);
            Height = c.Read<Int16>();
            Reserved2 = c.Read<Int32>();
            for (int y = Height - 1;y >= 0;y--)
            {
                c.ReadArray(PixelBuffer, y * Width, Width);
            }
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write((Int16)Width);
            c.Write(Unk1);
            c.Write(Reserved1);
            c.Write((Int16)Width);
            c.Write((Int16)Height);
            c.Write(Reserved2);
            for (int y = Height - 1;y >= 0;y--)
            {
                c.WriteArray(PixelBuffer, y * Width, Width);
            }
        }
    }
}
