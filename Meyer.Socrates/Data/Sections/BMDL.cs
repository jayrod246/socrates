namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.Data.Volatile;
    using Meyer.Socrates.IO;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    [SectionKey("BMDL")]
    public sealed class BMDL: VirtualSection
    {
        private readonly VolatileCollection<int> triangles;
        private readonly VolatileCollection<BrVector2> texCoords;
        private readonly VolatileCollection<BrVector3> verts;
        private readonly VolatileCollection<TriangleUnks> unks;

        public IList<int> Triangles
        {
            get
            {
                using (Lock())
                {
                    return triangles;
                }
            }
        }
        public IList<BrVector2> TextureCoordinates
        {
            get
            {
                using (Lock())
                {
                    return texCoords;
                }
            }
        }
        public IList<BrVector3> Vertices
        {
            get
            {
                using (Lock())
                {
                    return verts;
                }
            }
        }
        public IList<TriangleUnks> TriangleUnks
        {
            get
            {
                using (Lock())
                {
                    return unks;
                }
            }
        }

        public BMDL()
        {
            triangles = new VolatileCollection<int>(NotifyDataChanged);
            texCoords = new VolatileCollection<BrVector2>(NotifyDataChanged);
            verts = new VolatileCollection<BrVector3>(NotifyDataChanged);
            unks = new VolatileCollection<TriangleUnks>(NotifyDataChanged);
        }

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.Read<uint>();
            var vertcount = c.Read<UInt16>();
            var tricount = c.Read<UInt16>();
            c.Position = 48;
            Vertices.Clear();
            TextureCoordinates.Clear();
            Triangles.Clear();
            TriangleUnks.Clear();

            for (int k = 0;k < vertcount;k++)
            {
                Vertices.Add(c.Read<BrVector3>());
                var uv = c.Read<BrVector2>();
                uv.Y = 1 - uv.Y;
                TextureCoordinates.Add(uv);
                c.Position += 12;
            }

            for (int k = 0;k < tricount;k++)
            {
                Triangles.Add(c.Read<UInt16>());
                Triangles.Add(c.Read<UInt16>());
                Triangles.Add(c.Read<UInt16>());
                TriangleUnks.Add(c.Read<TriangleUnks>());
            }
        }

        protected override void Write(IDataWriteContext c)
        {
            if (Triangles.Count % 3 != 0) throw new InvalidOperationException("Triangles.Count must be divisible by 3");
            c.Write(MagicNumber);
            c.Write((ushort)Vertices.Count);
            c.Write((ushort)(Triangles.Count / 3));
            c.WriteArray(new byte[40]); // Junk

            for (int k = 0;k < Vertices.Count;k++)
            {
                c.Write(Vertices[k].X.RawValue);
                c.Write(Vertices[k].Y.RawValue);
                c.Write(Vertices[k].Z.RawValue);
                c.Write(TextureCoordinates[k].X.RawValue);
                c.Write((1 - TextureCoordinates[k].Y).RawValue);
                c.WriteArray(new byte[12]);
            }

            int num = 0;

            while (num < Triangles.Count)
            {
                c.Write((ushort)Triangles[num++]);
                c.Write((ushort)Triangles[num++]);
                c.Write((ushort)Triangles[num++]);
                c.Write(TriangleUnks[(num / 3) - 1]);
            }
        }

        private void NotifyDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ClearCache();
        }
    }
}
