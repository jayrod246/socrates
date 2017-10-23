namespace Meyer.Socrates.Data
{
    using Meyer.Socrates.IO;
    using System;

    public struct SourceReferenceEx: IChunkIdentifier
    {
        private UInt32 collectionID;
        private Ms3dmmCollection collection;

        internal ISourceReferencedEx owner;
        public Quad Quad { get; set; }
        public UInt32 ID { get; set; }
        public Int32 Unk { get; set; }
        public UInt32 CollectionID
        {
            get => collectionID;
            set
            {
                if (value != collectionID)
                {
                    collectionID = value;
                    if (value == 0 || !Ms3dmmCollection.TryOpen(value, out collection))
                        collection = null;
                }
            }
        }
        public Ms3dmmCollection Collection
        {
            get => collection;
            set
            {
                if (value != collection)
                {
                    collection = value;
                    collectionID = collection == null ? 0 : collection.CollectionID;
                }
            }
        }

        public Chunk Resolve()
        {
            if (CollectionID == 0) return ResolveInternal(owner?.Container);
            if (Ms3dmmCollection.TryOpen(CollectionID, out var coll))
                return ResolveInternal(coll);
            return null;
        }

        public Chunk Resolve(Ms3dmmFile sourceFile)
        {
            if (sourceFile == null) throw new ArgumentNullException("sourceFile");
            return ResolveInternal(sourceFile);
        }

        internal Chunk ResolveInternal(IResolver<IChunkIdentifier, Chunk> resolver)
        {
            return resolver.Resolve(this);
        }

        ChunkIdentity IChunkIdentifier.GetChunkIdentity()
        {
            return new ChunkIdentity(Quad, ID);
        }
    }
}