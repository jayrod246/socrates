namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.IO;
    using System;
    using System.ComponentModel;

    [SectionKey("HTOP")]
    public class HTOP: IndexableSection<HTOP.Chunk>
    {
        public HTOP()
        {
        }

        protected override void InsertItem(int index, Chunk item)
        {
            lock (Synchronized)
            {
                item.owner = this;
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index, Chunk item)
        {
            lock (Synchronized)
            {
                item.owner = null;
                base.RemoveItem(index, item);
            }
        }

        protected override void SetItem(int index, Chunk item)
        {
            var oldItem = this[index];
            oldItem.owner = null;
            item.owner = this;
            base.SetItem(index, item);
        }

        protected override void Read(IDataReadContext c)
        {
            while (c.Position < c.Length)
            {
                var chunk = new Chunk();
                chunk.ReadInternal(c);
                Add(chunk);
            }
        }

        protected override void Write(IDataWriteContext c)
        {
            foreach (var item in this)
                item.WriteInternal(c);
        }

        public class Chunk: SocDataObject<HTOP>, INotifyPropertyChanging, INotifyPropertyChanged, IResolvable<Data.Chunk>
        {
            public UInt32 MagicNumber { get => GetValue<UInt32>(); set => SetValue(value); }
            public UInt32 Unk0 { get => GetValue<UInt32>(); set => SetValue(value); }
            public UInt32 Unk1 { get => GetValue<UInt32>(); set => SetValue(value); }
            public UInt32 Unk2 { get => GetValue<UInt32>(); set => SetValue(value); }
            public virtual Quad Quad { get => GetValue<Quad>(); set => SetValue(value); }
            public virtual Section Section
            {
                get => GetValue(ref section);
                set
                {
                    lock (Synchronized)
                    {
                        if (value != null) value.owner = this;
                        if (section != null) section.owner = null;
                        SetValue(ref section, value);
                    }
                }
            }

            internal sealed override void EnsureCachedInternal()
            {
                if (cache == null)
                {
                    using (var d = new DataStream())
                    {
                        Write(d);
                        cache = d.ToArray();
                    }
                }
            }

            internal sealed override void EnsureLoadedInternal()
            {
                if (!isLoaded)
                {
                    if (cache != null)
                    {
                        using (var d = new DataStream(Compression.Decompress(cache)))
                            Read(d);
                    }
                    isLoaded = true;
                }
            }

            internal sealed override void NeedsReload()
            {
                using (Lock())
                {
                    base.NeedsReload();
                    isLoaded = false;
                }
            }

            internal virtual void ReadInternal(IDataReadContext c)
            {
                using (Lock())
                {
                    Read(c);
                }
            }

            internal virtual void WriteInternal(IDataWriteContext c)
            {
                using (Lock())
                {
                    Write(c);
                }
            }

            private void Read(IDataReadContext c)
            {
                MagicNumber = c.Read<uint>();
                Quad = c.Read<Quad>();
                Unk0 = c.Read<UInt32>();
                var sectLength = c.Read<Int32>();
                Unk1 = c.Read<UInt32>();
                Unk2 = c.Read<UInt32>();
                if (Quad != "HTOP") Section = Create(Quad);
                else Section = Create();
                Section.Data = c.ReadArray<byte>(sectLength);
            }

            private void Write(IDataWriteContext c)
            {
                c.Write(MagicNumber);
                c.Write(Quad);
                c.Write(Unk0);
                if (Section == null) c.Write(0);
                else c.Write(Section.Data.Length);
                c.Write(Unk1);
                c.Write(Unk2);
                if (Section != null) c.WriteArray(Section.Data);
            }

            protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
            {
                base.OnPropertyChanged(propertyName, oldValue, newValue);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            protected override void OnPropertyChanging(string propertyName, object currentValue, object newValue)
            {
                base.OnPropertyChanging(propertyName, currentValue, newValue);
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public event PropertyChangingEventHandler PropertyChanging;

            private Section section;
            private bool isLoaded;

            global::Meyer.Socrates.Data.Chunk IResolvable<global::Meyer.Socrates.Data.Chunk>.Resolve()
            {
                return Owner?.Owner;
            }
        }
    }
}
