namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;

    [SectionKey("GLPI")]
    public sealed class GLPI: IndexableSection<int>
    {
        private void ValidateItem(int item)
        {
            if (item < Int16.MinValue) throw new ArgumentOutOfRangeException($"item value less than {Int16.MinValue}", "item");
            if (item > Int16.MaxValue) throw new ArgumentOutOfRangeException($"item value greater than {Int16.MaxValue}", "item");
        }

        protected override void SetItem(int index, int item)
        {
            ValidateItem(item);
            base.SetItem(index, item);
        }

        protected override void InsertItem(int index, int item)
        {
            ValidateItem(item);
            base.InsertItem(index, item);
        }

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.Read<uint>();
            c.Assert<UInt32>(2);
            var count = c.Read<UInt32>();
            for (int i = 0;i < count;i++)
                Add(c.Read<Int16>());
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(MagicNumber);
            c.Write<UInt32>(2);
            c.Write((UInt32)Count);
            for (int i = 0;i < Count;i++)
                c.Write((Int16)this[i]);
        }

        public int GetParentIndex(int bodyPart)
        {
            if (bodyPart < 0 || bodyPart >= Count)
                throw new ArgumentOutOfRangeException("bodyPart");

            return this[bodyPart];
        }
    }
}
