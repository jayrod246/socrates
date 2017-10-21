namespace Meyer.Socrates.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class SocDataObject: SocObjectBase
    {
        public bool IsReadOnly => isReadOnly;

        [SocProperty(SocPropertyFlags.KeepCache)]
        public byte[] Data
        {
            get
            {
                lock (Synchronized) // Not using Lock() because EnsureLoaded is not necessary.
                {
                    EnsureCached();
                    Contract.Assert(cache != null);
                    return (byte[])cache.Clone();
                }
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (SetDataInternal(value))
                    NeedsReload();
            }
        }

        public bool HasCache
        {
            get
            {
                using (Lock())
                    return cache != null;
            }
        }

        internal bool SetDataInternal(byte[] buffer)
        {
            EnsureCanWrite();
            // This way an event is always raised when set to null, even when already equal to null. (as is the special case for re-caching)
            if (cache != null && buffer == cache) return false;
            var oldCache = cache;
            OnPropertyChanging("Data", oldCache, buffer);
            cache = buffer;
            OnPropertyChanged("Data", oldCache, buffer);
            return true;
        }

        private void EnsureCanWrite()
        {
            if (!isLoading && isReadOnly) throw new NotSupportedException("The SocDataObject is in a read-only state.");
        }

        /// <summary>
        /// Ensures that the data is cached.
        /// </summary>
        protected void EnsureCached()
        {
            Contract.Ensures(cache != null);
            Contract.Assert(Monitor.IsEntered(Synchronized));
            EnsureCachedInternal();
        }

        /// <summary>
        /// Sets the value of a property using a backing field.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="field">The backing field.</param>
        /// <param name="value">The value to assign to the property.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>True if the property was assigned a different value than it had previously.</returns>
        protected override bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            using (Lock())
            {
                EnsureCanWrite();
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;
                var propertyFlags = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetCustomAttribute<SocPropertyAttribute>()?.PropertyFlags ?? default(SocPropertyFlags);
                var oldValue = field;
                if (!propertyFlags.HasFlag(SocPropertyFlags.SilenceChangingNotifications)) OnPropertyChanging(propertyName, oldValue, value);
                field = value;
                if (!propertyFlags.HasFlag(SocPropertyFlags.SilenceChangedNotifications)) OnPropertyChanged(propertyName, oldValue, value);
                if (!isLoading && !propertyFlags.HasFlag(SocPropertyFlags.KeepCache)) ClearCache();
                return true;
            }
        }

        /// <summary>
        /// Gets the value of a property using a backing field.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="field">The backing field.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The value of the property.</returns>
        protected override T GetValue<T>(ref T field, [CallerMemberName] string propertyName = null)
        {
            using (Lock())
            {
                return field;
            }
        }

        /// <summary>
        /// Ensures that the object has been loaded since the data was last updated.
        /// </summary>
        protected internal void EnsureLoaded()
        {
            Contract.Assert(Monitor.IsEntered(Synchronized));
            if (!isLoading)
            {
                isLoading = true;
                try
                {
                    EnsureLoadedInternal();
                }
                finally
                {
                    isLoading = false;
                }
            }
        }

        /// <summary>
        /// Clears the cache, forcing a refresh the next time the data is accessed.
        /// </summary>
        protected void ClearCache()
        {
            using (Lock())
            {
                SetDataInternal(null);
            }
        }

        internal virtual void NeedsReload()
        {

        }

        internal virtual void EnsureLoadedInternal()
        {

        }

        internal virtual void EnsureCachedInternal()
        {
            if (cache == null) cache = Array.Empty<byte>();
        }

        /// <summary>
        /// Locks the data object for editing, and ensures that it has been loaded before the edits begin.
        /// </summary>
        /// <param name="clearCache">Determines if the cache needs to be cleared after disposing of the lock.</param>
        protected IDisposable Lock(bool clearCache = false)
        {
            Monitor.Enter(Synchronized);
            try
            {
                EnsureLoaded();
                if (clearCache) EnsureCanWrite();
                return new LockHelper(this, clearCache);
            }
            catch
            {
                // Make sure we exit sync, in case of an exception!
                Monitor.Exit(Synchronized);
                throw;
            }
        }

        internal void Unlock(bool clearCache)
        {
            try
            {
                if (clearCache) ClearCache();
            }
            finally
            {
                // Make sure we exit sync no matter what!
                Monitor.Exit(Synchronized);
            }
        }

        public SocDataObject AsReadOnly()
        {
            using (Lock())
            {
                var clone = (SocDataObject)MemberwiseClone();
                clone.isReadOnly = true;
                return clone;
            }
        }

        internal void MakeReadOnly()
        {
            lock (Synchronized)
                isReadOnly = true;
        }

        private bool isReadOnly;
        private bool isLoading;
        internal byte[] cache;

        private class LockHelper: IDisposable
        {
            public LockHelper(SocDataObject dataObject, bool clearCache)
            {
                this.dataObject = dataObject;
                this.clearCache = clearCache;
            }

            public void Dispose()
            {
                if (dataObject != null)
                {
                    dataObject.Unlock(clearCache);
                    dataObject = null;
                }
            }

            private SocDataObject dataObject;
            private bool clearCache;
        }
    }

    public class SocDataObject<TOwner>: SocDataObject
    {
        public TOwner Owner
        {
            get
            {
                using (Lock())
                {
                    if (owner is TOwner) return (TOwner)owner;
                    if (owner is IResolvable<TOwner> resolvable) return resolvable.Resolve();
                    return default(TOwner);
                }
            }
        }

        internal volatile object owner = null;
    }
}
