using Meyer.Socrates.IO;
using System;

namespace Meyer.Socrates.Data.ActorEvents
{
    /// <summary>
    /// Actor Event that changes the "costume" of the object.
    /// </summary>
    /// TODO: Fix the SetCostumeAE class.
    [ActorEventType(ActorEventType.SetCostume)]
    public class SetCostumeAE: VirtualActorEvent, ISourceReferencedEx
    {
        /// <summary>
        /// The BodySet of this object to look in.
        /// </summary>
        public UInt32 SourceBodySet { get => GetValue<UInt32>(); set => SetValue(value); }

        /// <summary>
        /// The reference ID of a Cmtl.
        /// </summary>
        public UInt32 SourceCMTL { get => GetValue<UInt32>(); set => SetValue(value); }

        public Int32 Unk1 { get => GetValue<Int32>(); set => SetValue(value); }

        /// <summary>
        /// A reference to a MTRL.
        /// </summary>
        public SourceReferenceEx SourceReference
        {
            get => GetValue<SourceReferenceEx>();
            set
            {
                value.owner = this;
                SetValue(value);
            }
        }

        Ms3dmmFile ISourceReferencedEx.Container => Owner?.Owner?.container as Ms3dmmFile;

        protected override void Read(IDataReadContext c)
        {
            SourceBodySet = c.Read<UInt32>();
            SourceCMTL = c.Read<UInt32>();
            Unk1 = c.AssertAny(0, 1);
            //Unk1 = c.Read<Int32>();
            SourceReference = new SourceReferenceEx()
            {
                CollectionID = c.Read<UInt32>(),
                Unk = c.Read<Int32>(),
                Quad = c.Read<Quad>(),
                ID = c.Read<UInt32>()
            };
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(SourceBodySet);
            c.Write(SourceCMTL);
            c.Write(Unk1);
            c.Write(SourceReference.CollectionID);
            c.Write(SourceReference.Unk);
            c.Write(SourceReference.Quad);
            c.Write(SourceReference.ID);
        }
    }
}
