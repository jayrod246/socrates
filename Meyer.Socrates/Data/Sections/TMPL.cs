using Meyer.Socrates.IO;
using System;

namespace Meyer.Socrates.Data.Sections
{
    /// <summary>
    /// Main section for Actors, Props and 3D Words.
    /// </summary>
    [SectionKey("TMPL")]
    public sealed class TMPL: VirtualSection
    {
        /// <summary>
        /// Sets the default orientation of the object.
        /// </summary>
        public BrEuler Euler { get => GetValue<BrEuler>(); set => SetValue(value); }

        /// <summary>
        /// Determines type of object.
        /// </summary>
        public ObjectType ObjectType { get => GetValue<ObjectType>(); set => SetValue(value); }

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.Read<UInt32>();
            Euler = c.Read<BrEuler>();
            ObjectType = c.Read<ObjectType>();
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write(Euler);
            c.Write(ObjectType);
        }
    }
}
