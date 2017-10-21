namespace Meyer.Socrates.Data.Volatile
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class VolatileObject: SocObjectBase, INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsReadOnly => isReadOnly;

        /// <summary>
        /// Sets the value of a property using a backing field.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="field">The backing field.</param>
        /// <param name="value">The value to assign to the property.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>True if the property was assigned a different value than it had previously.</returns>
        protected override bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {

            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            var oldValue = field;
            OnPropertyChanging(propertyName, oldValue, value);
            field = value;
            OnPropertyChanged(propertyName, oldValue, value);
            return true;
        }

        /// <summary>
        /// Gets the value of a property using a backing field.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="field">The backing field.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The value of the property.</returns>
        protected override T GetValue<T>(ref T field, [CallerMemberName] string propertyName = null)
        {
            return field;
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

        private void EnsureCanWrite()
        {
            if (isReadOnly) throw new NotSupportedException("The VolatileObject is in a read-only state.");
        }

        internal void MakeReadOnly() => isReadOnly = true;

        private bool isReadOnly;
    }
}
