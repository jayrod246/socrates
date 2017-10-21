using System;
using System.Collections;
using System.Collections.Generic;

namespace Meyer.Socrates.Collections.Generic
{
    public abstract class CollectionBase<T>: IList<T>, IList, IReadOnlyList<T>
    {
        IList<T> items;

        protected CollectionBase() : this(new List<T>())
        {
        }

        protected CollectionBase(IList<T> collection)
        {
            items = collection;
        }

        protected virtual void InsertItem(int index, T item)
        {
            this.items.Insert(index, item);
        }

        protected virtual void RemoveItem(int index, T item)
        {
            this.items.RemoveAt(index);
        }

        protected virtual void SetItem(int index, T oldItem, T newItem)
        {
            this.items[index] = newItem;
        }

        protected virtual void ClearItems()
        {
            this.items.Clear();
        }

        protected virtual void ValidateItem(T item)
        { }

        public void AddRange(IEnumerable<T> collection)
        {
            InsertRange(items.Count, collection);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (index < 0) throw new ArgumentOutOfRangeException("index less than zero", "index");
            if (index > items.Count) throw new ArgumentOutOfRangeException("index greater than item count", "index");

            foreach (var item in collection)
                InsertItem(index++, item);
        }

        public int IndexOf(T item)
        {
            return this.items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ValidateItem(item);
            InsertItem(index, item);
        }

        public void RemoveAt(int index)
        {
            RemoveItem(index, items[index]);
        }

        public T this[int index]
        {
            get => this.items[index];
            set
            {
                ValidateItem(value);
                SetItem(index, this.items[index], value);
            }
        }

        public void Add(T item)
        {
            ValidateItem(item);
            InsertItem(items.Count, item);
        }

        public void Clear()
        {
            ClearItems();
        }

        public bool Contains(T item)
        {
            return this.items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            int index = items.IndexOf(item);
            if (index < 0) return false;
            RemoveItem(index, item);
            return true;
        }

        public int Count => this.items.Count;

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        int IList.Add(object value)
        {
            if (value == null) return -1;
            if (!(value is T)) return -1;
            var item = (T)value;
            try
            {
                ValidateItem(item);
            }
            catch (ValidationException)
            {
                return -1;
            }
            ValidateItem(item);
            int index = items.Count;
            InsertItem(index, item);
            return index;
        }

        bool IList.Contains(object value)
        {
            if (value == null) return false;
            if (!(value is T)) return false;
            return items.Contains((T)value);
        }

        void IList.Clear()
        {
            Clear();
        }

        int IList.IndexOf(object value)
        {
            if (value == null) return -1;
            if (!(value is T)) return -1;
            return items.IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (!(value is T)) throw new InvalidCastException($"Invalid cast from {value.GetType().Name} to {typeof(T).Name}.");
            var item = (T)value;
            ValidateItem(item);
            InsertItem(index, item);
        }

        void IList.Remove(object value)
        {
            if (value == null || !(value is T)) return;
            var item = (T)value;
            int index = items.IndexOf(item);
            if (index < 0) return;
            RemoveItem(index, item);
        }

        void IList.RemoveAt(int index)
        {
            RemoveItem(index, items[index]);
        }

        object IList.this[int index]
        {
            get => items[index];
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (!(value is T)) throw new InvalidCastException($"Invalid cast from {value.GetType().Name} to {typeof(T).Name}.");
                var item = (T)value;
                ValidateItem(item);
                SetItem(index, this.items[index], item);
            }
        }

        bool IList.IsReadOnly => false;

        bool IList.IsFixedSize => false;

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)items).CopyTo(array, index);
        }

        int ICollection.Count => items.Count;

        object ICollection.SyncRoot => ((ICollection)items).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)items).IsSynchronized;

        internal void DoValidateItem(T item)
        {
            ValidateItem(item);
        }
    }
}
