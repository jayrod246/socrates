namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the lighting in the scene.
    /// </summary>
    [SectionKey("GLLT")]
    public sealed class GLLT: IndexableSection<BrLight>
    {
        private const int bytesPerLight = 56;

        protected override void Read(IDataReadContext c, IList<BrLight> items)
        {
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);
            c.Assert(bytesPerLight);
            var count = c.Read<Int32>();
            (items as dynamic).AddRange(c.ReadArray<BrLight>(count));
        }

        protected override void Write(IDataWriteContext c, IList<BrLight> items)
        {
            c.Write(MagicNumber);
            c.Write(bytesPerLight);
            c.Write(items.Count);
            c.WriteArray(items.ToArray());
        }
    }
}
