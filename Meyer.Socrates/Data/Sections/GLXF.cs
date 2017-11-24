namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;

    [SectionKey("GLXF")]
    public sealed class GLXF: IndexableSection<BrMatrix3x4>
    {
        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.Read<uint>();
            c.Assert(0x30);
            var count = c.Read<UInt32>();

            for (int i = 0;i < count;i++) Add(c.Read<BrMatrix3x4>());
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(0x30);
            c.Write((uint)Count);
            foreach (var item in this) c.Write(item);
        }
    }
}
