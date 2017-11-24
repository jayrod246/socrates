using Meyer.Socrates.IO;
using System;

namespace Meyer.Socrates.Data.Sections
{
    /// <summary>
    /// Sets the default costume. (3D Movie Maker doesn't actually use this)
    /// </summary>
    [SectionKey("GLDC")]
    public class GLDC: VirtualSection
    {
        public UInt32 CMTLRefID { get => GetValue<UInt32>(); set => SetValue(value); }
        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.Read<uint>();
            c.Assert(4);
            c.Assert(1);
            CMTLRefID = c.Read<UInt32>();
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(4);
            c.Write(1);
            c.Write(CMTLRefID);
        }
    }
}
