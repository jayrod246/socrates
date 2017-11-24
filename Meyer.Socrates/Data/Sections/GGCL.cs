namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.IO;

    [SectionKey("GGCL")]
    public sealed class GGCL: IndexableSection<ActionFrame>
    {
        protected override void Read(IDataReadContext c)
        {
            if (c.Length < 20)
                throw new InvalidDataException("GGCL section is too small");
            MagicNumber = c.Read<uint>();
            var frameCount = c.Read<UInt32>();
            var offsetDirectory = c.Read<UInt32>() + 20;
            c.Assert(0xFFFFFFFF);
            c.Assert(8);

            for (int i = 0;i < frameCount;i++)
            {
                c.Position = offsetDirectory + (8 * i);
                var offset = c.Read<UInt32>();
                var len = c.Read<UInt32>();
                c.Position = offset + 20;

                if (c.Read<UInt32>() != 0)
                    throw new InvalidDataException("Invalid entry in GGCL.");

                var frameData = new ActionFrame()
                {
                    PathUnits = c.Read<BrScalar>(),
                    Cells = c.ReadArray<ActionCell>((int)(len / 4))
                };

                Add(frameData);
            }
        }

        protected override void Write(IDataWriteContext c)
        {
            c.WriteArray(new byte[20]);
            var offsets = new uint[Count];
            var lengths = new uint[Count];

            // Write data sections
            for (int i = 0;i < Count;i++)
            {
                offsets[i] = (uint)c.Position;
                c.Write(0);
                c.Write(this[i].PathUnits);
                for (int j = 0;j < this[i].Cells.Length;j++)
                {
                    c.Write(this[i].Cells[j].BMDL);
                    c.Write(this[i].Cells[j].TransformIndex);
                }
                lengths[i] = (uint)c.Position - offsets[i];
            }

            uint toplen = (uint)c.Length - 20;

            // Write offsets and lengths
            for (int i = 0;i < Count;i++)
            {
                c.Write(offsets[i] - 20);
                c.Write(lengths[i]);
            }

            c.Position = 0;
            c.Write(MagicNumber);
            c.Write((uint)Count);
            c.Write(toplen);
            c.Write(0xFFFFFFFF);
            c.Write(8);
        }
    }
}
