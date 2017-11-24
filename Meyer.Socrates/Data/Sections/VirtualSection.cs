namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;

    public abstract class VirtualSection: Section
    {
        public VirtualSection() : base()
        {

        }

        internal sealed override void EnsureCachedInternal()
        {
            if (cache == null)
            {
                using (var d = new DataStream())
                {
                    Write(d);
                    cache = d.ToArray();
                    SetCompressionTypeInternal(CompressionType.Uncompressed);
                }
            }
        }

        internal override void EnsureLoadedInternal()
        {
            if (!isLoaded)
            {
                if (cache != null)
                {
                    SetCompressionTypeInternal(Compression.GetCompressionType(cache));
                    using (var d = new DataStream(Compression.Decompress(cache)))
                        Read(d);
                }
                isLoaded = true;
            }
        }

        internal sealed override void NeedsReload()
        {
            using (Lock())
            {
                base.NeedsReload();
                isLoaded = false;
            }
        }

        protected abstract void Read(IDataReadContext c);
        protected abstract void Write(IDataWriteContext c);

        internal bool isLoaded = true;
    }
}