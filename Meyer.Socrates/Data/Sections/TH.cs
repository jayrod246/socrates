namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;

    /// <summary>
    /// The base class for TH-suffixed chunks. This class is abstract.
    /// </summary>
    public abstract class TH: VirtualSection, ISourceReferenced
    {
        internal abstract Quad DefaultQuad { get; }

        public SourceReference SourceReference
        {
            get => GetValue<SourceReference>();
            set
            {
                value.owner = this;
                SetValue(value);
            }
        }

        Ms3dmmFile ISourceReferenced.Container => Owner?.container as Ms3dmmFile;

        /// <summary>
        /// Prevent externals from inheriting.
        /// </summary>
        internal TH()
        {
        }

        public TH(uint sourceID)
        {
            SourceReference = new SourceReference() { Quad = DefaultQuad, ID = sourceID };
        }

        public TH(Quad sourceQuad, uint sourceID)
        {
            SourceReference = new SourceReference() { Quad = sourceQuad, ID = sourceID };
        }

        protected sealed override void Read(IDataReadContext c)
        {
            MagicNumber = c.AssertAny(Ms3dmm.MAGIC_NUM_US, Ms3dmm.MAGIC_NUM_JP);
            SourceReference = new SourceReference()
            {
                Quad = c.Read<Quad>(),
                ID = c.Read<UInt32>()
            };
        }

        protected sealed override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(SourceReference.Quad);
            c.Write(SourceReference.ID);
        }
    }

    [SectionKey("SMTH")]
    public sealed class SMTH: TH
    {
        public SMTH(uint sourceID) : base(sourceID)
        {
        }

        public SMTH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public SMTH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"MSND";
    }

    [SectionKey("SVTH")]
    public sealed class SVTH: TH
    {
        public SVTH(uint sourceID) : base(sourceID)
        {
        }

        public SVTH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public SVTH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"MSND";
    }

    [SectionKey("SFTH")]
    public sealed class SFTH: TH
    {
        public SFTH(uint sourceID) : base(sourceID)
        {
        }

        public SFTH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public SFTH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"MSND";
    }

    [SectionKey("BKTH")]
    public sealed class BKTH: TH
    {
        public BKTH(uint sourceID) : base(sourceID)
        {
        }

        public BKTH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public BKTH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"BKGD";
    }

    [SectionKey("CATH")]
    public sealed class CATH: TH
    {
        public CATH(uint sourceID) : base(sourceID)
        {
        }

        public CATH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public CATH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"CAM ";
    }

    [SectionKey("MTTH")]
    public sealed class MTTH: TH
    {
        public MTTH(uint sourceID) : base(sourceID)
        {
        }

        public MTTH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public MTTH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"MTRL";
    }

    [SectionKey("PRTH")]
    public sealed class PRTH: TH
    {
        public PRTH(uint sourceID) : base(sourceID)
        {
        }

        public PRTH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public PRTH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"TMPL";
    }

    [SectionKey("TMTH")]
    public sealed class TMTH: TH
    {
        public TMTH(uint sourceID) : base(sourceID)
        {
        }

        public TMTH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public TMTH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"TMPL";
    }

    [SectionKey("TCTH")]
    public sealed class TCTH: TH
    {
        public TCTH(uint sourceID) : base(sourceID)
        {
        }

        public TCTH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public TCTH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"....";
    }

    [SectionKey("TBTH")]
    public sealed class TBTH: TH
    {
        public TBTH(uint sourceID) : base(sourceID)
        {
        }

        public TBTH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public TBTH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"....";
    }

    [SectionKey("TZTH")]
    public sealed class TZTH: TH
    {
        public TZTH(uint sourceID) : base(sourceID)
        {
        }

        public TZTH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public TZTH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"....";
    }

    [SectionKey("TSTH")]
    public sealed class TSTH: TH
    {
        public TSTH(uint sourceID) : base(sourceID)
        {
        }

        public TSTH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public TSTH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"....";
    }

    [SectionKey("TFTH")]
    public sealed class TFTH: TH
    {
        public TFTH(uint sourceID) : base(sourceID)
        {
        }

        public TFTH(Quad sourceQuad, uint sourceID) : base(sourceQuad, sourceID)
        {
        }

        public TFTH() : base()
        {
        }

        internal override Quad DefaultQuad { get; } = (Quad)"TDF ";
    }
}
