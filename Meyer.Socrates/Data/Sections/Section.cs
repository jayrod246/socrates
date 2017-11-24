namespace Meyer.Socrates.Data.Sections
{
    using Meyer.Socrates.Services;
    using System;
    using System.ComponentModel;
    using System.IO;

    public class Section: SocDataObject<Chunk>, INotifyPropertyChanging, INotifyPropertyChanged, IResolvable<Chunk>, ICloneable
    {
        [SocProperty(SocPropertyFlags.KeepCache)]
        public CompressionType CompressionType { get => GetValueWithoutLock(ref this.compressionType); }

        public VersionInfo VersionInfo
        {
            get => GetValue(ref versionInfo) ?? (VersionInfo = VersionInfo.English);
            set => SetValue(ref versionInfo, value);
        }

        protected uint MagicNumber
        {
            get => VersionInfo.MagicNumber;
            set
            {
                if (!VersionInfo.TryGetVersionInfo(value, out var versionInfo))
                    throw new ArgumentException("Bad magic number", "value");
                VersionInfo = versionInfo;
            }
        }

        internal void SetCompressionTypeInternal(CompressionType compressionType)
        {
            SetValueWithoutLock(ref this.compressionType, compressionType, nameof(CompressionType));
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
                if (buffer == null || buffer.Length == 0) SetCompressionTypeInternal(CompressionType.Uncompressed);
                else SetCompressionTypeInternal(Compression.GetCompressionType(buffer));
            }
        }

        public static T FromFile<T>(string fileName) where T : VirtualSection
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException("File does not exist.", fileName);
            var section = Activator.CreateInstance<T>();
            section.Data = File.ReadAllBytes(fileName);
            return section;
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
        private VersionInfo versionInfo;
        private CompressionType compressionType;
    }
}