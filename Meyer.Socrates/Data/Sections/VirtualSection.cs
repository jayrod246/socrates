namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.Runtime.CompilerServices;

    public abstract class VirtualSection: Section
    {
        protected uint MagicNumber
        {
            get
            {
                if (magicNumber != Ms3dmm.MAGIC_NUM_US && magicNumber != Ms3dmm.MAGIC_NUM_JP)
                    magicNumber = Ms3dmm.MAGIC_NUM_US;
                return GetValue(ref magicNumber);
            }

            set
            {
                if (value != Ms3dmm.MAGIC_NUM_US && value != Ms3dmm.MAGIC_NUM_JP) throw new ArgumentException("Bad MagicNumber", "value");
                SetValue(ref magicNumber, value);
            }
        }

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

        internal sealed override void EnsureLoadedInternal()
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

        // TODO: Remove these RequireLoad() methods and use the "using(Lock()) { ... }"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RequireLoad(Action action)
        {
            using (Lock())
            {
                action.Invoke();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T RequireLoad<T>(Func<T> func)
        {
            using (Lock())
            {
                return func.Invoke();
            }
        }

        protected abstract void Read(IDataReadContext c);
        protected abstract void Write(IDataWriteContext c);

        private bool isLoaded = true;
        private uint magicNumber;
    }
}