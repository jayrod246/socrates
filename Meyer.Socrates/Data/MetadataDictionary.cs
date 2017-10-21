using System.Collections.Generic;

namespace Meyer.Socrates.Data
{
    internal class MetadataDictionary<T>
    {
        Dictionary<T, IMetadata<T>> dict;

        public MetadataDictionary()
        {
            dict = new Dictionary<T, IMetadata<T>>();
        }

        public MetadataDictionary(IEqualityComparer<T> comparer)
        {
            dict = new Dictionary<T, IMetadata<T>>(comparer);
        }

        internal IMetadata<T> GetMetadata(T obj)
        {
            if (!dict.ContainsKey(obj))
                dict[obj] = new Metadata(obj);
            return dict[obj];
        }

        private class Metadata : IMetadata<T>
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            private T obj;

            public T Instance => obj;

            public object this[string propertyName]
            {
                get => GetProperty(propertyName);
                set => SetProperty(propertyName, value);
            }

            public Metadata(T obj)
            {
                this.obj = obj;
            }

            public object GetProperty(string propertyName)
            {
                if (!dict.ContainsKey(propertyName))
                    return null;

                return dict[propertyName];
            }

            public void SetProperty(string propertyName, object value)
            {
                dict[propertyName] = value;
            }
        }
    }
}
