namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;

    [SectionKey("CMTL")]
    public sealed class CMTL: VirtualSection
    {
        public uint BodySetID { get => GetValue<uint>(); set => SetValue(value); }

        public CMTL() : this(0)
        {
        }

        public CMTL(uint bodySetID)
        {
            BodySetID = bodySetID;
        }

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);
            BodySetID = c.Read<UInt32>();
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(BodySetID);
        }
    }
}
