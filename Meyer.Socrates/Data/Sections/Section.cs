namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.Services;
    using System;
    using System.ComponentModel;

    public class Section: SocDataObject<Chunk>, INotifyPropertyChanging, INotifyPropertyChanged, IResolvable<Chunk>, ICloneable
    {
        [SocProperty(SocPropertyFlags.KeepCache)]
        public CompressionType CompressionType { get => GetValue<CompressionType>(); }

        internal void SetCompressionTypeInternal(CompressionType compressionType)
        {
            SetValue(compressionType, nameof(CompressionType));
        }

        internal Section()
        {

        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnPropertyChanged(propertyName, oldValue, newValue);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(Data))
            {
                var buffer = (byte[])newValue;
                if (buffer == null) SetCompressionTypeInternal(CompressionType.Uncompressed);
                else SetCompressionTypeInternal(Compression.GetCompressionType(buffer));
            }
        }

        protected override void OnPropertyChanging(string propertyName, object currentValue, object newValue)
        {
            base.OnPropertyChanging(propertyName, currentValue, newValue);
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        object ICloneable.Clone() => Clone();

        public Section Clone()
        {
            var clone = (Section)Activator.CreateInstance(GetType());
            clone.Data = Data;
            return clone;
        }

        public static Section Create(string quad)
        {
            if (string.IsNullOrEmpty(quad)) return Create();
            return SectionFactory.CreateSection(quad);
        }

        public static Section Create()
        {
            return new Section();
        }

        Chunk IResolvable<Chunk>.Resolve()
        {
            return Owner;
        }

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}