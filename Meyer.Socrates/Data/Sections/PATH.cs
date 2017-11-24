namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.IO;
    using System.Linq;

    [SectionKey("PATH")]
    public class PATH: IndexableSection<PathNode>
    {
        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.Read<uint>();

            if (c.Read<UInt32>() != 16)
                throw new InvalidDataException();

            var count = c.Read<UInt32>();
            for (int i = 0;i < count;i++)
                Add(c.Read<PathNode>());
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write<UInt32>(16);
            c.Write((UInt32)Count);
            c.WriteArray(this.ToArray());
        }
    }
}
