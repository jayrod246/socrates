namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public abstract class SharedMBMP: BitmapSection, IIndexed8Image
    {
        internal SharedMBMP(int width, int height, bool isThum)
        {
            Width = width;
            Height = height;
            IsTHUM = isThum;
        }

        #region Properties
        public int OffsetX { get => GetValue<int>(); set => SetValue(value); }
        public int OffsetY { get => GetValue<int>(); set => SetValue(value); }
        public bool UsesTransparency { get => GetValue<bool>(); set => SetValue(value); }
        public sealed override PixelFormat PixelFormat => PixelFormat.Indexed8;

        public byte this[int x, int y]
        {
            get
            {
                using (Lock())
                {
                    return (byte)GetPixelCore(x, y);
                }
            }
            set
            {
                using (Lock())
                {
                    SetPixelCore(x, y, value);
                }
            }
        }

        internal bool IsTHUM { get; }
        #endregion

        public void SetPixel(int x, int y, byte value)
        {
            using (Lock()) SetPixelCore(x, y, value);
        }

        public void Clear(byte fill)
        {
            using (Lock()) ClearCore(fill);
        }

        protected sealed override void Read(IDataReadContext c)
        {
            MagicNumber = c.Read<uint>();

            if (c.Read<Int32>() != 0)
                throw new InvalidDataException("Invalid MBMP header.");

            OffsetX = c.Read<Int32>();
            OffsetY = c.Read<Int32>();
            Width = c.Read<Int32>() - OffsetX;
            Height = c.Read<Int32>() - OffsetY;

            if (c.Read<UInt32>() != c.Length)
                throw new InvalidDataException("Filelengths don't match.");

            var lineLengths = new short[Height];
            for (int line = 0;line < Height;line++)
            {
                lineLengths[line] = c.Read<Int16>();
            }

            for (int line = 0;line < Height;line++)
            {
                int linePos = 0;
                int x = 0;

                while (linePos < lineLengths[line])
                {
                    int skip = c.Read<Byte>();

                    if (skip > 0)
                    {
                        UsesTransparency = true;
                    }

                    int lineSize = c.Read<Byte>();
                    x += skip;
                    linePos += 2;

                    c.ReadArray(PixelBuffer, x + line * Stride, lineSize);
                    x += lineSize;

                    linePos += lineSize;
                }
            }
        }

        protected sealed override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(0x0); // Junk
            c.Write(OffsetX);
            c.Write(OffsetY);
            c.Write(Width + OffsetX);
            c.Write(Height + OffsetY);

            c.Write(0); // FileSize

            var lineLengths = new short[Height];
            c.Position += Height * 2;

            const byte t = 0x0;

            if (UsesTransparency)
            {
                LineSegment line;
                for (int y = 0;y < Height;y++)
                {
                    line = new LineSegment();
                    int x = 0;
                    while (x < Width)
                    {
                        if (PixelBuffer[Width * y + x] == t && line.Skip < 255)
                        {
                            line.Skip++;
                            x++;
                        }
                        else
                        {
                            int i = 0;
                            while (i < Width - x && PixelBuffer[Width * y + x + i] != t && i < 255)
                                i++;
                            if (i > 0)
                            {
                                line.Data = new byte[i];
                                Array.Copy(PixelBuffer, Width * y + x, line.Data, 0, i);
                                x += i;
                            }
                            c.Write(line.Skip);
                            c.Write((byte)line.Data.Length);
                            c.WriteArray(line.Data);
                            lineLengths[y] += (short)(line.Data.Length + 2);
                            line = new LineSegment();
                        }
                    }
                    if (!line.IsEmpty())
                    {
                        c.Write(line.Skip);
                        c.Write((byte)line.Data.Length);
                        c.WriteArray(line.Data);
                        lineLengths[y] += (short)(line.Data.Length + 2);
                        line = new LineSegment();
                    }
                }
            }
            else
            {
                var lineData = new List<byte>();
                for (int y = 0;y < Height;y++)
                {
                    for (int x = 0;x < Width;x++)
                    {
                        lineData.Add(PixelBuffer[y * Width + x]);
                        if (lineData.Count == 255 || x == Width - 1)
                        {
                            c.Write((byte)0);
                            c.Write((byte)lineData.Count);
                            c.WriteArray(lineData.ToArray());
                            lineLengths[y] += (short)(lineData.Count + 2);
                            lineData.Clear();
                        }
                    }
                }
            }

            c.Position = 28;
            foreach (var linelength in lineLengths)
                c.Write(linelength);
            c.Position = 24;
            c.Write((int)c.Length);
        }

        private struct LineSegment
        {
            private byte[] data;
            public byte Skip;
            public byte[] Data
            {
                get
                {
                    return data ?? new byte[0];
                }
                set
                {
                    data = value;
                }
            }

            public bool IsEmpty()
            {
                return Skip == 0 && Data.Length == 0;
            }
        }
    }
}
