namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    public abstract class IndexableSection<T>: VirtualSection, IList<T>
    {
        List<T> items = new List<T>();

        public int Count
        {
            get => RequireLoad(() => this.items.Count);
        }

        public T this[int index]
        {
            get => RequireLoad(() => this.items[index]);
            set
            {
                using (Lock(true))
                {
                    SetItem(index, value);
                }
            }
        }

        protected virtual void SetItem(int index, T item)
        {
            if (this.items[0] is INotifyPropertyChanged) ((INotifyPropertyChanged)this.items[0]).PropertyChanged -= OnItemPropertyChanged;
            this.items[index] = item;
            if (item is INotifyPropertyChanged) ((INotifyPropertyChanged)item).PropertyChanged += OnItemPropertyChanged;
        }

        private void OnItemPropertyChanged(object sender, EventArgs e)
        {
            ClearCache();
        }

        protected virtual void InsertItem(int index, T item)
        {
            this.items.Insert(index, item);
            if (item is INotifyPropertyChanged) ((INotifyPropertyChanged)item).PropertyChanged += OnItemPropertyChanged;
        }

        protected virtual void RemoveItem(int index, T item)
        {
            this.items.RemoveAt(index);
            if (item is INotifyPropertyChanged) ((INotifyPropertyChanged)item).PropertyChanged -= OnItemPropertyChanged;
        }

        protected override void Read(IDataReadContext c)
        {
            var items = new List<T>();
            Read(c, items);
            Clear();
            AddRange(items);
        }

        protected override void Write(IDataWriteContext c)
        {
            Write(c, this.items.AsReadOnly());
        }

        protected abstract void Read(IDataReadContext c, IList<T> items);
        protected abstract void Write(IDataWriteContext c, IList<T> items);

        public void AddRange(IEnumerable<T> collection)
        {
            using (Lock(true))
            {
                foreach (var item in collection)
                    InsertItem(this.items.Count, item);
            }
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            using (Lock(true))
            {
                int cnt = this.items.Count;

                foreach (var item in collection)
                {
                    InsertItem(index, item);

                    if (cnt < this.items.Count)
                        index++;
                }
            }
        }

        public int IndexOf(T item)
        {
            return RequireLoad(() => this.items.IndexOf(item));
        }

        public void Insert(int index, T item)
        {
            using (Lock(true))
            {
                InsertItem(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            using (Lock())
            {
                if (index < 0 || index >= this.items.Count)
                    throw new ArgumentOutOfRangeException("index");

                RemoveItem(index, this.items[index]);
                ClearCache();
            }
        }

        public void Add(T item)
        {
            using (Lock(true))
            {
                InsertItem(this.items.Count, item);
            }
        }

        public void Clear()
        {
            using (Lock(true))
            {
                for (int i = this.items.Count - 1;i >= 0;i--)
                    RemoveItem(i, this.items[i]);
            }
        }

        public bool Contains(T item)
        {
            return RequireLoad(() => this.items.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            RequireLoad(() => this.items.CopyTo(array, arrayIndex));
        }

        public bool Remove(T item)
        {
            using (Lock())
            {
                int index;

                if ((index = this.items.IndexOf(item)) >= 0)
                {
                    RemoveItem(index, item);
                    ClearCache();
                    return true;
                }

                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return RequireLoad(() => this.items.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return RequireLoad(() => this.items.GetEnumerator());
        }
    }
}
