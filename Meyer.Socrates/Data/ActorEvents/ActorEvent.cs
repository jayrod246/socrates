namespace Meyer.Socrates.Data.ActorEvents
{
    using Meyer.Socrates.Data.Sections;
    using Meyer.Socrates.Data;
    using Meyer.Socrates.Services;
    using System;
    using System.ComponentModel;

    /// <summary>
    /// The base class for all Actor Events. This class is abstract.
    /// </summary>
    public class ActorEvent: SocDataObject<GGAE>, ICloneable, INotifyPropertyChanging, INotifyPropertyChanged
    {
        /// <summary>
        /// The type of actor event.
        /// </summary>
        public ActorEventType Code => code;

        /// <summary>
        /// Number of frames to wait after a PATH is frozen.
        /// </summary>
        public int Wait { get => GetValue<int>(); set => SetValue(value); }

        /// <summary>
        /// The frame that this event starts on.
        /// </summary>
        public int Begin { get => GetValue<int>(); set => SetValue(value); }

        /// <summary>
        /// The index into the PATH.
        /// </summary>
        public uint PathIndex { get => GetValue<uint>(); set => SetValue(value); }

        /// <summary>
        /// How many units between the current and next path indices.
        /// </summary>
        public BrScalar PathUnits { get => GetValue<BrScalar>(); set => SetValue(value); }

        public virtual bool IsVirtual => false;

        internal ActorEvent()
        {
            // Prevent external types from inheriting.
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

        public static ActorEvent Create(ActorEventType type)
        {
            var result = VirtualActorEventRegistry.TryResolveTypeFromKey(type, out Type sectionType) ? (ActorEvent)Activator.CreateInstance(sectionType) : new ActorEvent();
            result.code = type;
            return result;
        }

        public float CalculatePathPosition(PATH path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (PathIndex >= path.Count)
                throw new IndexOutOfRangeException("PATH does not contain enough elements.");

            float pathUnits = path[(int)PathIndex].PathUnits;

            if (pathUnits == 0 && PathUnits == 0)
                return PathIndex;

            return PathIndex + (PathUnits / pathUnits);
        }

        public ActorEvent Clone()
        {
            var clone = (ActorEvent)Activator.CreateInstance(GetType());
            clone.Data = Data;
            return clone;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
        private ActorEventType code;

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
