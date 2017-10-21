namespace Meyer.Socrates.Data
{
    using System.Collections;
    using System.Collections.Generic;

    public class CostumeDefinition: IList<uint>
    {
        private readonly List<uint> costumes = new List<uint>();

        public CostumeDefinition(params uint[] costumes)
        {
            this.costumes.AddRange(costumes);
        }

        public int IndexOf(uint item)
        {
            return this.costumes.IndexOf(item);
        }

        public void Insert(int index, uint item)
        {
            this.costumes.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.costumes.RemoveAt(index);
        }

        public uint this[int index] { get => this.costumes[index]; set => this.costumes[index] = value; }

        public void Add(uint item)
        {
            this.costumes.Add(item);
        }

        public void Clear()
        {
            this.costumes.Clear();
        }

        public bool Contains(uint item)
        {
            return this.costumes.Contains(item);
        }

        public void CopyTo(uint[] array, int arrayIndex)
        {
            this.costumes.CopyTo(array, arrayIndex);
        }

        public bool Remove(uint item)
        {
            return this.costumes.Remove(item);
        }

        public int Count => this.costumes.Count;

        public bool IsReadOnly => false;

        public IEnumerator<uint> GetEnumerator()
        {
            return this.costumes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.costumes.GetEnumerator();
        }

        public void AddRange(IEnumerable<uint> collection)
        {
            this.costumes.AddRange(collection);
        }

        public void InsertRange(int index, IEnumerable<uint> collection)
        {
            this.costumes.InsertRange(index, collection);
        }
    }
}
