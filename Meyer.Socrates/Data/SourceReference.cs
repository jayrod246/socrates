using Meyer.Socrates.IO;
using System;

namespace Meyer.Socrates.Data
{
    public struct SourceReference: IChunkIdentifier, IResolvable<Chunk>, IResolver<Ms3dmmFile, Chunk>
    {
        internal ISourceReferenced owner;
        public Quad Quad { get; set; }
        public uint ID { get; set; }

        public Chunk Resolve()
        {
            if (owner == null || owner.Container == null) return null;
            return ResolveInternal(owner.Container) ?? ResolveInternal(owner.Container.container);
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

        Chunk IResolver<Ms3dmmFile, Chunk>.Resolve(Ms3dmmFile input)
        {
            return Resolve(input);
        }

        ChunkIdentity IChunkIdentifier.GetChunkIdentity()
        {
            return new ChunkIdentity(Quad, ID);
        }
    }
}