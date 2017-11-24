namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.Data;
    using Meyer.Socrates.IO;
    using System.IO;

    [SectionKey("TXXF")]
    public sealed class TXXF: VirtualSection
    {
        #region Constructors
        public TXXF() : this(new BrVector2(), new BrVector2(1, 1), BrAngle.MinValue)
        { }

        public TXXF(BrVector2 position, BrVector2 scale) : this(position, scale, BrAngle.MinValue)
        { }

        public TXXF(BrVector2 position, BrVector2 scale, BrAngle rotation)
        {
            TransformMatrix = BrMatrix2x3.Identity;
            Position = position;
            Scale = scale;
            Rotation = rotation;
        }
        #endregion

        public BrVector2 Position
        {
            get => GetValue<BrVector2>();
            set
            {
                if (SetValue(value))
                {
                    transformMatrix = null;
                }
            }
        }

        public BrAngle Rotation
        {
            get => GetValue<BrAngle>();
            set
            {
                if (SetValue(value))
                {
                    transformMatrix = null;
                }
            }
        }

        public BrVector2 Scale
        {
            get => GetValue<BrVector2>();
            set
            {
                if (SetValue(value))
                {
                    transformMatrix = null;
                }
            }
        }

        public BrMatrix2x3 TransformMatrix
        {
            get
            {
                using (Lock())
                {
                    return (transformMatrix ?? (transformMatrix = BrMatrix2x3.FromTRS(Position, Rotation, Scale))).Value;
                }
            }

            set
            {
                using (Lock())
                {
                    value.TRS(out var t, out var r, out var s);
                    Position = t;
                    Rotation = r;
                    Scale = s;
                    transformMatrix = value;
                }
            }
        }

        protected override void Read(IDataReadContext c)
        {
            if (c.Length != 28) throw new InvalidDataException("TXXF section length must be 28.");
            MagicNumber = c.Read<uint>();
            TransformMatrix = c.Read<BrMatrix2x3>();
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(TransformMatrix);
        }

        private BrMatrix2x3? transformMatrix;
    }
}
