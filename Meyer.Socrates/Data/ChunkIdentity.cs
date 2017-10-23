namespace Meyer.Socrates.Data
{
    public struct ChunkIdentity
    {
        public ChunkIdentity(Quad quad, uint id) : this()
        {
            Quad = quad;
            ID = id;
        }

        public uint ID { get; }
        public Quad Quad { get; }

        public override int GetHashCode()
        {
            return ID.GetHashCode() ^ Quad.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ChunkIdentity other) return ID == other.ID && Quad == other.Quad;
            return base.Equals(obj);
        }
    }
}
