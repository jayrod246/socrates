namespace Meyer.Socrates.Data.Sections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    public abstract class IndexableSection<T>: VirtualSection, IList<T>
    {
        public int Count
        {
            get
            {
                using (Lock())
                {
                    return this.items.Count;
                }
            }
        }

        public T this[int index]
        {
            get
            {
                using (Lock())
                {
                    return this.items[index];
                }
            }
            set
            {
                using (Lock(true))
                {
                    SetItemWrapper(index, value);
                }
            }
        }

        private void SetItemWrapper(int index, T item)
        {
            if (isReading)
                stagedActions.Add(i => SetItem(index - i, item));
            else
                SetItem(index, item);
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

        private void InsertItemWrapper(int index, T item)
        {
            if (isReading)
                stagedActions.Add(i => InsertItem(index - i, item));
            else
                InsertItem(index, item);
        }

        protected virtual void InsertItem(int index, T item)
        {
            this.items.Insert(index, item);
            if (item is INotifyPropertyChanged) ((INotifyPropertyChanged)item).PropertyChanged += OnItemPropertyChanged;
        }

        private void RemoveItemWrapper(int index, T item)
        {
            if (isReading)
                stagedActions.Add(i => RemoveItem(index - i, item));
            else
                RemoveItem(index, item);
        }

        protected virtual void RemoveItem(int index, T item)
        {
            this.items.RemoveAt(index);
            if (item is INotifyPropertyChanged) ((INotifyPropertyChanged)item).PropertyChanged -= OnItemPropertyChanged;
        }

        internal override void EnsureLoadedInternal()
        {
            isReading = !isLoaded && cache != null;
            try
            {
                if (isReading)
                    stagedActions.Clear();

                base.EnsureLoadedInternal();

                if (isReading)
                {
                    isReading = false;
                    var oldLength = items.Count;
                    Clear();
                    foreach (var action in stagedActions)
                        action.Invoke(oldLength);
                }
            }
            finally
            {
                isReading = false;
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            using (Lock(true))
            {
                foreach (var item in collection)
                    InsertItemWrapper(this.items.Count, item);
            }
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            using (Lock(true))
            {
                int cnt = this.items.Count;

                foreach (var item in collection)
                {
                    InsertItemWrapper(index, item);

                    if (cnt < this.items.Count)
                        index++;
                }
            }
        }

        public int IndexOf(T item)
        {
            using (Lock())
            {
                return this.items.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            using (Lock(true))
            {
                InsertItemWrapper(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            using (Lock())
            {
                if (index < 0 || index >= this.items.Count)
                    throw new ArgumentOutOfRangeException("index");

                RemoveItemWrapper(index, this.items[index]);
                ClearCache();
            }
        }

        public void Add(T item)
        {
            using (Lock(true))
            {
                InsertItemWrapper(this.items.Count, item);
            }
        }

        public void Clear()
        {
            using (Lock())
            {
                bool clearCache = items.Count > 0;
                for (int i = this.items.Count - 1;i >= 0;i--)
                    RemoveItemWrapper(i, this.items[i]);
                if (clearCache) ClearCache();
            }
        }

        public bool Contains(T item)
        {
            using (Lock())
            {
                return this.items.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            using (Lock())
            {
                this.items.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            using (Lock())
            {
                int index;

                if ((index = this.items.IndexOf(item)) >= 0)
                {
                    RemoveItemWrapper(index, item);
                    ClearCache();
                    return true;
                }

                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            using (Lock())
            {
                return this.items.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (Lock())
            {
                return this.items.GetEnumerator();
            }
        }

        private IList<T> items = new List<T>();
        private IList<Action<int>> stagedActions = new List<Action<int>>();
        private bool isReading;
    }
}
