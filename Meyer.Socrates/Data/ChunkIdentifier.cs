namespace Meyer.Socrates.Data
{
    internal struct ChunkIdentifier
    {
        internal ChunkIdentifier(Quad quad, uint id) : this()
        {
            Quad = quad;
            ID = id;
        }

        internal uint ID { get; }
        internal Quad Quad { get; }

        public override int GetHashCode()
        {
            return unchecked((ID.GetHashCode() & 0xFFFF) | (Quad.GetHashCode() << 16));
        }

        public override bool Equals(object obj)
        {
            if (obj is ChunkIdentifier other) return ID == other.ID && Quad == other.Quad;
            return base.Equals(obj);
        }
    }
}
