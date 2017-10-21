namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    [SectionKey("PATH")]
    public class PATH: IndexableSection<PathNode>
    {
        protected override void Read(IDataReadContext c, IList<PathNode> items)
        {
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);

            if (c.Read<UInt32>() != 16)
                throw new InvalidDataException();

            var count = c.Read<UInt32>();
            for (int i = 0;i < count;i++)
                items.Add(c.Read<PathNode>());
        }

        protected override void Write(IDataWriteContext c, IList<PathNode> items)
        {
            c.Write(MagicNumber);
            c.Write<UInt32>(16);
            c.Write((UInt32)items.Count);
            c.WriteArray(items.ToArray());
        }
    }
}
