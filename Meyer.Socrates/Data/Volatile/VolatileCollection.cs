namespace Meyer.Socrates.Data.Volatile
{
    using Meyer.Socrates.Collections.Generic;
    using System.Collections.Specialized;
    using System.Collections.Generic;

    public class VolatileCollection<T>: CollectionBase<T>, INotifyCollectionChanged
    {
        protected VolatileCollection()
        {
        }

        protected VolatileCollection(IList<T> collection) : base(collection)
        {
        }

        internal VolatileCollection(NotifyCollectionChangedEventHandler collectionChangedEventHandler)
        {
            CollectionChanged += collectionChangedEventHandler;
        }

        protected override void InsertItem(int index, T item)
        {
            lock (Synchronized)
            {
                base.InsertItem(index, item);
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }

        protected override void RemoveItem(int index, T item)
        {
            lock (Synchronized)
            {
                base.RemoveItem(index, item);
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }
        }

        protected override void SetItem(int index, T oldItem, T newItem)
        {
            lock (Synchronized)
            {
                base.SetItem(index, oldItem, newItem);
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, oldItem, index));
            }
        }

        protected override void ClearItems()
        {
            lock (Synchronized)
            {
                base.ClearItems();
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        internal void NotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected readonly object Synchronized = new object();
    }
}
