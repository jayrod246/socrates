namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.Collections.Generic;

    [SectionKey("GLXF")]
    public sealed class GLXF: IndexableSection<BrMatrix3x4>
    {
        protected override void Read(IDataReadContext c, IList<BrMatrix3x4> items)
        {
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);
            c.Assert(0x30);
            var count = c.Read<UInt32>();

            for (int i = 0;i < count;i++) items.Add(c.Read<BrMatrix3x4>());
        }

        protected override void Write(IDataWriteContext c, IList<BrMatrix3x4> items)
        {
            c.Write(MagicNumber);
            c.Write(0x30);
            c.Write((uint)items.Count);
            foreach (var item in items) c.Write(item);
        }
    }
}
