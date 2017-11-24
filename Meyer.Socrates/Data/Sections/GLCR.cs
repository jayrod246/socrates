namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    [SectionKey("GLCR")]
    public sealed class GLCR: VirtualSection, IEnumerable<BrColorRGB>
    {
        private readonly BrColorRGB[] colors = new BrColorRGB[256];

        public BrColorRGB this[int index]
        {
            get
            {
                using (Lock())
                {
                    return colors[index];
                }
            }
            set
            {
                using (Lock())
                {
                    colors[index] = value;
                }
            }
        }

        public bool IsPartial { get => GetValue<bool>(); set => SetValue(value); }

        public IEnumerator<BrColorRGB> GetEnumerator()
        {
            using (Lock()) return ((IEnumerable<BrColorRGB>)colors).GetEnumerator();
        }

        protected override void Read(IDataReadContext c)
        {
            if (c.Length != 1036 && c.Length != 956)
                throw new InvalidDataException();

            MagicNumber = c.Read<uint>();

            if (c.Read<Int32>() != 4)
                throw new InvalidDataException();

            var colCount = c.Read<UInt32>();

            if (colCount != 256 && colCount != 236)
                throw new InvalidDataException();

            IsPartial = colCount != 256;

            for (int i = 0;i < 256;i++)
            {
                if (IsPartial && (i < 10 || i > 245))
                {
                    colors[i] = new BrColorRGB(255, 0, 255);
                    continue;
                }
                //colors[i] = new BrColorRGB()
                //{
                //    B = c.Read<Byte>(),
                //    G = c.Read<Byte>(),
                //    R = c.Read<Byte>(),
                //    A = c.Read<Byte>() // Reserved.
                //};
                colors[i] = c.Read<BrColorRGB>();
            }
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(4);
            c.Write(IsPartial ? 236 : 256);

            //var tmp = colors;

            //if (IsPartial)
            //    tmp = tmp.Skip(10).Take(236).ToArray();

            //foreach (var entry in tmp)
            //{
            //    c.Write((byte)(entry.ScB * byte.MaxValue));
            //    c.Write((byte)(entry.ScG * byte.MaxValue));
            //    c.Write((byte)(entry.ScR * byte.MaxValue));
            //    c.Write((byte)0); // Reserved
            //}

            int colCount = IsPartial ? 236 : 256;
            int colOffset = IsPartial ? 10 : 0;

            for (int i = 0;i < colCount;i++)
            {
                c.Write(colors[colOffset + i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (Lock()) return ((IEnumerable<BrColorRGB>)colors).GetEnumerator();
        }
    }
}
