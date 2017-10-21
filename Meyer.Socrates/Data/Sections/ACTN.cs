namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;

    [SectionKey("ACTN")]
    public sealed class ACTN: VirtualSection
    {
        public ActionBehavior ActionBehavior { get => GetValue<ActionBehavior>(); set => SetValue(value); }

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);
            ActionBehavior = c.Read<ActionBehavior>();
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(ActionBehavior);
        }
    }
}
