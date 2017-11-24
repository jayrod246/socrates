namespace Meyer.Socrates.Data
{
    using Meyer.Socrates.Collections;
    using Meyer.Socrates.Data.Sections;
    using Meyer.Socrates.Data.Volatile;
    using System;
    using System.Runtime.CompilerServices;

    public class ReadOnlyReference: Reference
    {
        internal ReadOnlyReference()
        {

        }

        protected override bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            throw new NotSupportedException("Reference is read-only");
        }

        internal bool SetValueInternal<T>(ref T field, T value, string propertyName)
        {
            return base.SetValue(ref field, value, propertyName);
        }
    }

    public class Reference: VolatileObject, IChunkIdentifier, IResolvable<Chunk>, IResolvable<Section>, ICloneable
    {
        public uint ID { get => GetValue(ref id); set => SetValue(ref id, value); }
        public uint ReferenceID { get => GetValue(ref reference_id); set => SetValue(ref reference_id, value); }
        public Quad Quad { get => GetValue(ref quad); set => SetValue(ref quad, value); }

        public Chunk Resolve()
        {
            if (container == null || container.owner == null || container.owner.container == null) return null;
            return Resolve(container.owner.container);
        }

        Section IResolvable<Section>.Resolve()
        {
            return Resolve()?.Section;
        }

        internal Chunk Resolve(IResolver<IChunkIdentifier, Chunk> resolver)
        {
            return resolver.Resolve(this);
        }

        public Reference Clone()
        {
            return new Reference()
            {
                Quad = Quad,
                ID = ID,
                ReferenceID = ReferenceID
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public ReadOnlyReference AsReadOnly()
        {
            lock (Synchronized)
            {
                var result = new ReadOnlyReference();
                result.SetValueInternal(ref result.quad, quad, nameof(Quad));
                result.SetValueInternal(ref result.id, id, nameof(ID));
                result.SetValueInternal(ref result.reference_id, reference_id, nameof(ReferenceID));
                return result;
            }
        }

        protected override bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            lock (Synchronized)
            {
                if (propertyName == nameof(Quad) || propertyName == nameof(ID))
                {
#pragma warning disable CS0420 // A reference to a volatile field will not be treated as volatile
                    CommonItemHelper.ValidateValue(this, ref container, ref field, value);
#pragma warning restore CS0420 // A reference to a volatile field will not be treated as volatile
                }
                return base.SetValue(ref field, value, propertyName);
            }
        }

        protected override T GetValue<T>(ref T field, [CallerMemberName] string propertyName = null)
        {
            lock (Synchronized)
            {
                return base.GetValue(ref field, propertyName);
            }
        }

        ChunkIdentity IChunkIdentifier.GetChunkIdentity()
        {
            return new ChunkIdentity(Quad, ID);
        }

        internal volatile ReferenceCollection container;

        private uint id;
        private uint reference_id;
        private Quad quad;
    }
}
