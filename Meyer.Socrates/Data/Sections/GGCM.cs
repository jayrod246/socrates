namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.Collections.Generic;
    using System.IO;

    [SectionKey("GGCM")]
    public sealed class GGCM: IndexableSection<CostumeDefinition>
    {
        protected override void Read(IDataReadContext c, IList<CostumeDefinition> items)
        {
            if (c.Length < 20)
                throw new InvalidDataException("GGCM section is too small");

            MagicNumber = c.Read<UInt32>();
            var costumeCount = c.Read<UInt32>();
            var offsetDirectory = c.Read<UInt32>() + 20;

            c.Assert(0xFFFFFFFF);
            c.Assert(4);

            items.Clear();
            for (int i = 0;i < costumeCount;i++)
            {
                c.Position = offsetDirectory + (8 * i);
                var offset = c.Read<UInt32>();
                var len = c.Read<UInt32>();
                c.Position = offset + 20;
                items.Add(new CostumeDefinition(c.ReadArray<UInt32>(checked((int)c.Read<UInt32>()))));
            }
        }

        protected override void Write(IDataWriteContext c, IList<CostumeDefinition> items)
        {
            c.WriteArray(new byte[20]);
            var offsets = new uint[items.Count];
            var lengths = new uint[items.Count];

            // Write data sections
            for (int i = 0;i < items.Count;i++)
            {
                var costumes = items[i];

                offsets[i] = (uint)c.Position;
                uint length = (uint)costumes.Count;
                c.Write(length);

                for (int j = 0;j < length;j++)
                    c.Write(costumes[j]);

                lengths[i] = (uint)c.Position - offsets[i];
            }

            uint toplen = (uint)c.Length - 20;

            // Write offsets and lengths
            for (int i = 0;i < items.Count;i++)
            {
                c.Write(offsets[i] - 20);
                c.Write(lengths[i]);
            }

            c.Position = 0;
            c.Write(MagicNumber);
            c.Write((uint)items.Count);
            c.Write(toplen);
            c.Write(0xFFFFFFFF);
            c.Write(4);
        }
    }
}
