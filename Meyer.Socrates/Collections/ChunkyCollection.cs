namespace Meyer.Socrates.Collections
{
    using Meyer.Socrates.Data;
    using Meyer.Socrates.Data.Volatile;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class ChunkyCollection: VolatileCollection<Chunk>, IResolver<IChunkIdentifier, Chunk>
    {
        internal ChunkyCollection() : base()
        {
            lock (Synchronized) dictionary = new Dictionary<ChunkIdentity, Entry>();
        }

        internal ChunkyCollection(int capacity) : base(new List<Chunk>(capacity))
        {
            lock (Synchronized) dictionary = new Dictionary<ChunkIdentity, Entry>(capacity);
        }

        internal ChunkyCollection(IList<Chunk> collection) : base(collection)
        {
            lock (Synchronized)
            {
                dictionary = new Dictionary<ChunkIdentity, Entry>(collection.Count);
                foreach (var item in collection)
                {
                    ValidateItem(item);
                    Attach(item, true);
                }
            }
        }

        protected override void InsertItem(int index, Chunk item)
        {
            lock (Synchronized)
            {
                Attach(item, true);
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index, Chunk item)
        {
            lock (Synchronized)
            {
                Detach(item, true);
                base.RemoveItem(index, item);
            }
        }

        protected override void SetItem(int index, Chunk oldItem, Chunk newItem)
        {
            lock (Synchronized)
            {
                Detach(oldItem, true);
                Attach(newItem, true);
                base.SetItem(index, oldItem, newItem);
            }
        }

        protected override void ClearItems()
        {
            foreach (var c in this)
                Detach(c, true);
            base.ClearItems();
        }

        protected override void ValidateItem(Chunk item)
        {
            if (item == null) throw new ValidationException(new ArgumentNullException("item"));
            if (item.container != null) throw new ValidationException("Chunk is already attached.");
            if (dictionary.TryGetValue(new ChunkIdentity(item.Quad, item.ID), out var entry) && entry.chunk != null) throw new ValidationException("Chunk with identifier already exists inside collection.");
            base.ValidateItem(item);
        }

        private void Attach(Chunk item, bool subscribe)
        {
            item.container = this;
            if (subscribe)
            {
                item.PropertyChanging += Chunk_PropertyChanging;
                item.PropertyChanged += Chunk_PropertyChanged;
                item.References.CollectionChanged += References_CollectionChanged;
            }
            GetOrAddEntry(item).chunk = item;

            foreach (var r in item.References)
                AttachReference(r, subscribe);
        }

        private void Detach(Chunk item, bool unsubscribe)
        {
            item.container = null;
            if (unsubscribe)
            {
                item.PropertyChanging -= Chunk_PropertyChanging;
                item.PropertyChanged -= Chunk_PropertyChanged;
                item.References.CollectionChanged -= References_CollectionChanged;
            }
            var key = new ChunkIdentity(item.Quad, item.ID);
            var entry = dictionary[key];
            if (entry.references_to_this.Count > 0)
            {
                entry.chunk = null;
                dictionary[key] = entry;
            }
            else dictionary.Remove(key);
            foreach (var r in item.References)
                DetachReference(r, unsubscribe);
        }

        private void Chunk_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null || e.PropertyName == nameof(Chunk.Quad) || e.PropertyName == nameof(Chunk.ID))
            {
                Attach((Chunk)sender, false);
                Monitor.Exit(Synchronized);
            }
        }

        private void Chunk_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            if (e.PropertyName == null || e.PropertyName == nameof(Chunk.Quad) || e.PropertyName == nameof(Chunk.ID))
            {
                Monitor.Enter(Synchronized);
                Detach((Chunk)sender, false);
            }
        }

        private void References_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var r in e.OldItems.Cast<Reference>())
                    DetachReference(r, true);
            }

            if (e.NewItems != null)
            {
                foreach (var r in e.NewItems.Cast<Reference>())
                    AttachReference(r, true);
            }
        }

        private void DetachReference(Reference reference, bool unsubscribe)
        {
            if (unsubscribe)
            {
                reference.PropertyChanging -= Reference_PropertyChanging;
                reference.PropertyChanged -= Reference_PropertyChanged;
            }

            if (!GetOrAddEntry(reference).references_to_this.Remove(reference))
                throw new InvalidOperationException("Failed to detach a reference.");
        }

        private void AttachReference(Reference reference, bool subscribe)
        {
            if (subscribe)
            {
                reference.PropertyChanging += Reference_PropertyChanging;
                reference.PropertyChanged += Reference_PropertyChanged;
            }

            GetOrAddEntry(reference).references_to_this.Add(reference);
        }

        private void Reference_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            if (e.PropertyName == null || e.PropertyName == nameof(Reference.Quad) || e.PropertyName == nameof(Reference.ID))
            {
                Monitor.Enter(Synchronized);
                DetachReference((Reference)sender, false);
            }
            else if (e.PropertyName == nameof(Reference.ReferenceID))
            {
                Monitor.Enter(Synchronized);

                // Do something
            }
        }

        private void Reference_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null || e.PropertyName == nameof(Reference.Quad) || e.PropertyName == nameof(Reference.ID))
            {
                AttachReference((Reference)sender, false);
                Monitor.Exit(Synchronized);
            }
            else if (e.PropertyName == nameof(Reference.ReferenceID))
            {
                // Do something

                Monitor.Exit(Synchronized);
            }
        }

        private Entry GetOrAddEntry(IChunkIdentifier identifier)
        {
            var key = identifier.GetChunkIdentity();
            if (!dictionary.TryGetValue(key, out var entry))
                dictionary[key] = entry = new Entry();
            return entry;
        }

        private class Entry
        {
            public Chunk chunk;
            public ICollection<Reference> references_to_this = new ReferenceSet();

            private class ReferenceSet: ICollection<Reference>
            {
                ISet<Reference> set = new HashSet<Reference>();
                readonly object Synchronized = new object();

                public void Add(Reference item)
                {
                    lock (Synchronized)
                        this.set.Add(item);
                }

                public void Clear()
                {
                    lock (Synchronized)
                        this.set.Clear();
                }

                public bool Contains(Reference item)
                {
                    lock (Synchronized)
                        return this.set.Contains(item);
                }

                public void CopyTo(Reference[] array, int arrayIndex)
                {
                    lock (Synchronized)
                        this.set.CopyTo(array, arrayIndex);
                }

                public bool Remove(Reference item)
                {
                    lock (Synchronized)
                        return this.set.Remove(item);
                }

                public int Count
                {
                    get
                    {
                        lock (Synchronized)
                            return this.set.Count;
                    }
                }

                public bool IsReadOnly
                {
                    get
                    {
                        lock (Synchronized)
                            return this.set.IsReadOnly;
                    }
                }

                public IEnumerator<Reference> GetEnumerator()
                {
                    lock (Synchronized)
                        return this.set.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    lock (Synchronized)
                        return this.set.GetEnumerator();
                }
            }
        }

        private readonly Dictionary<ChunkIdentity, Entry> dictionary;

        Chunk IResolver<IChunkIdentifier, Chunk>.Resolve(IChunkIdentifier input)
        {
            return dictionary.TryGetValue(input.GetChunkIdentity(), out var entry) ? entry.chunk : null;
        }
    }
}
