namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.Linq;

    /// <summary>
    /// Defines the lighting in the scene.
    /// </summary>
    [SectionKey("GLLT")]
    public sealed class GLLT: IndexableSection<BrLight>
    {
        private const int bytesPerLight = 56;

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.Read<uint>();
            c.Assert(bytesPerLight);
            var count = c.Read<Int32>();
            AddRange(c.ReadArray<BrLight>(count));
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(bytesPerLight);
            c.Write(Count);
            c.WriteArray(this.ToArray());
        }
    }
}
