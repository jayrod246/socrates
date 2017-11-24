namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.Data;
    using Meyer.Socrates.Data.ActorEvents;
    using Meyer.Socrates.IO;
    using System;
    using System.IO;
    using System.Linq;

    [SectionKey("GGAE")]
    public class GGAE: IndexableSection<ActorEvent>
    {
        protected override void SetItem(int index, ActorEvent item)
        {
            Attach(item);
            Detach(this[index]);
            base.SetItem(index, item);
        }

        protected override void RemoveItem(int index, ActorEvent item)
        {
            base.RemoveItem(index, item);
            Detach(item);
        }

        protected override void InsertItem(int index, ActorEvent item)
        {
            base.InsertItem(index, item);
            Attach(item);
        }

        protected override void Read(IDataReadContext c)
        {
            MagicNumber = c.Read<uint>();
            var count = c.Read<UInt32>();
            var topLen = c.Read<UInt32>();
            var directoryOffset = topLen + 20;
            if (c.Read<UInt32>() != 0xFFFFFFFF || c.Read<UInt32>() != 20)
                throw new InvalidDataException();

            var offsets = new uint[count];
            var lengths = new uint[count];
            c.Position = directoryOffset;

            for (int i = 0;i < count;i++)
            {
                offsets[i] = c.Read<UInt32>();
                lengths[i] = c.Read<UInt32>();
            }

            for (int i = 0;i < count;i++)
            {
                c.Position = offsets[i] + 20;
                var ae = ActorEvent.Create(c.Read<ActorEventType>());
                ae.Begin = c.Read<Int32>();
                ae.PathIndex = c.Read<UInt32>();
                ae.PathUnits = c.Read<BrScalar>();
                ae.Wait = c.Read<Int32>();
                ae.Data = c.ReadArray<byte>((int)(lengths[i] - 20));
                Add(ae);
            }
        }

        protected override void Write(IDataWriteContext c)
        {
            var sorted = this.OrderBy(m => m.PathIndex).ThenBy(m => m.Begin).ToList();
            var offsets = new uint[sorted.Count];
            var lengths = new uint[sorted.Count];
            c.Position = 20;

            // Write data sections
            for (int i = 0;i < Count;i++)
            {
                offsets[i] = (uint)c.Position;
                c.Write(sorted[i].Code);
                c.Write(sorted[i].Begin);
                c.Write(sorted[i].PathIndex);
                c.Write(sorted[i].PathUnits);
                c.Write(sorted[i].Wait);
                c.WriteArray(sorted[i].Data);
                lengths[i] = (uint)c.Position - offsets[i];
            }

            uint toplen = (uint)c.Length - 20;

            // Write offsets and lengths
            for (int i = 0;i < Count;i++)
            {
                c.Write(offsets[i] - 20);
                c.Write(lengths[i]);
            }

            // Write header
            c.Position = 0;
            c.Write(MagicNumber);
            c.Write((uint)Count);
            c.Write(toplen);
            c.Write(0xFFFFFFFF);
            c.Write(20);
        }

        private void Attach(ActorEvent item)
        {
            if (item.owner != null && item.owner != this) throw new InvalidOperationException("Actor Event is already attached.");
            item.owner = this;
        }

        private void Detach(ActorEvent item)
        {
            item.owner = null;
        }
    }
}
