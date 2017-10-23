namespace Meyer.Socrates.Collections
{
    using Meyer.Socrates.Data;
    using Meyer.Socrates.Data.Volatile;
    using System;
    using System.Collections.Generic;

    public class ReferenceCollection: VolatileCollection<Reference>
    {
        internal ReferenceCollection(Chunk owner)
        {
            this.owner = owner;
        }

        protected override void InsertItem(int index, Reference item)
        {
            lock (Synchronized)
            {
                item.container = this;
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index, Reference item)
        {
            lock (Synchronized)
            {
                item.container = null;
                base.RemoveItem(index, item);
            }
        }

        protected override void SetItem(int index, Reference oldItem, Reference newItem)
        {
            lock (Synchronized)
            {
                oldItem.container = null;
                newItem.container = this;
                base.SetItem(index, oldItem, newItem);
            }
        }

        protected override void ClearItems()
        {
            foreach (var r in this)
                r.container = null;
            base.ClearItems();
        }

        protected override void ValidateItem(Reference item)
        {
            if (item == null) throw new ValidationException(new ArgumentNullException("item"));
            if (item.container != null) throw new ValidationException("Reference is already attached.");
            if (EqualityComparer<ChunkIdentity>.Default.Equals(new ChunkIdentity(item.Quad, item.ID), new ChunkIdentity(owner.Quad, owner.ID))) throw new ValidationException("Chunk cannot reference self.");
            base.ValidateItem(item);
        }

        internal readonly Chunk owner;
    }
}
