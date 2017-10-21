namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;

    [SectionKey("MASK")]
    public sealed class MASK: BitmapSection, IIndexed1Image
    {
        public int OffsetX { get => GetValue<int>(); set => SetValue(value); }
        public int OffsetY { get => GetValue<int>(); set => SetValue(value); }
        public int Unk { get => GetValue<int>(); set => SetValue(value); }
        public override PixelFormat PixelFormat => PixelFormat.BlackWhite;

        public bool this[int x, int y]
        {
            get => GetPixel(x, y);
            set => SetPixel(x, y, value);
        }

        public MASK() : this(1, 1)
        { }

        public MASK(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void SetPixel(int x, int y, bool value)
        {
            RequireLoad(() => SetPixelCore(x, y, value ? 1 : 0));
        }

        public bool GetPixel(int x, int y)
        {
            return RequireLoad(() => GetPixelCore(x, y) == 1);
        }

        public void Clear(bool fill)
        {
            RequireLoad(() =>
            {
                if (fill)
                {
                    for (int i = 0;i < PixelBuffer.Length;i++)
                        PixelBuffer[i] = 0xFF;
                }
                else Array.Clear(PixelBuffer, 0, PixelBuffer.Length);
            });
        }

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);
            Unk = c.Read<Int32>();
            OffsetX = c.Read<Int32>();
            OffsetY = c.Read<Int32>();
            Width = c.Read<Int32>() - OffsetX;
            Height = c.Read<Int32>() - OffsetY;
            c.Assert((uint)c.Length);
            int w = Width;
            int h = Height;
            var lineLengths = new short[h];

            for (int i = 0;i < lineLengths.Length;i++)
            {
                lineLengths[i] = c.Read<Int16>();
            }

            for (int y = 0;y < lineLengths.Length;y++)
            {
                int xpos = 0;

                for (int j = 0;j < lineLengths[y];j += 2)
                {
                    int masked = c.Read<Byte>();
                    int nonmasked = c.Read<Byte>();
                    xpos += masked;

                    while (nonmasked > 0)
                    {
                        nonmasked--;
                        SetPixel(xpos, y, true);
                        xpos++;
                    }
                }
            }
        }

        protected override void Write(IDataWriteContext c)
        {
            int w = Width;
            int h = Height;
            var lineLengths = new ushort[h];

            c.Write(MagicNumber);
            c.Write(Unk);
            c.Write(OffsetX);
            c.Write(OffsetY);
            c.Write(OffsetX + w);
            c.Write(OffsetY + h);
            c.Position = 28 + (h * 2);

            for (int y = 0;y < lineLengths.Length;y++)
            {
                int xpos = 0;
                int len = 0;

                while (xpos < w)
                {
                    int masked = 0;
                    int nonmasked = 0;

                    while (xpos < w && GetPixel(xpos, y) && masked < byte.MaxValue)
                    {
                        masked++;
                        xpos++;
                    }

                    while (xpos < w && GetPixel(xpos, y) && nonmasked < byte.MaxValue)
                    {
                        nonmasked++;
                        xpos++;
                    }

                    c.Write((byte)masked);
                    c.Write((byte)nonmasked);
                    len += 2;
                }

                lineLengths[y] = (ushort)len;
            }

            c.Position = 28;
            c.WriteArray(lineLengths);
            c.Position = 24;
            c.Write((uint)c.Length);
        }
    }
}
