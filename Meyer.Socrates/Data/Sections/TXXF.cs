namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.Data;
    using Meyer.Socrates.IO;
    using System;
    using System.IO;

    [SectionKey("TXXF")]
    public sealed class TXXF: VirtualSection
    {
        #region Constructors
        public TXXF() : this(new BrVector2(), new BrVector2(1, 1))
        { }

        public TXXF(BrVector2 position, BrVector2 scale)
        {
            Position = position;
            Scale = scale;
        }
        #endregion

        public BrVector2 Position { get => GetValue<BrVector2>(); set => SetValue(value); }
        public BrVector2 Scale { get => GetValue<BrVector2>(); set => SetValue(value); }
        public int Unk1 { get => GetValue<int>(); set => SetValue(value); }
        public int Unk2 { get => GetValue<int>(); set => SetValue(value); }

        protected override void Read(IDataReadContext c)
        {
            if (c.Length != 28) throw new InvalidDataException("TXXF section length must be 28.");
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);
            var scale = new BrVector2() { X = c.Read<BrScalar>() };
            Unk1 = c.Read<Int32>();
            Unk2 = c.Read<Int32>();
            scale.Y = c.Read<BrScalar>();
            Scale = scale;
            Position = c.Read<BrVector2>();
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(Scale.X);
            c.Write(Unk1);
            c.Write(Unk2);
            c.Write(Scale.Y);
            c.Write(Position);
        }
    }
}
