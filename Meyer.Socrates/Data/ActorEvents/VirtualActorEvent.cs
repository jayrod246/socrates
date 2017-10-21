namespace Meyer.Socrates.Data.ActorEvents
{
    using Meyer.Socrates.IO;
    using System;
    using System.Runtime.CompilerServices;

    public abstract class VirtualActorEvent: ActorEvent
    {
        public sealed override bool IsVirtual => true;

        public VirtualActorEvent() : base()
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
                }
            }
        }

        internal sealed override void EnsureLoadedInternal()
        {
            if (!isLoaded)
            {
                if (cache != null)
                {
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
    }
}
