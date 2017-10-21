namespace Meyer.Socrates.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public abstract class SocObjectBase
    {
        /// <summary>
        /// Gets the value of a property using an auto backing field.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The value of the property.</returns>
        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            lock (Synchronized)
            {
                return (T)GetValue(ref GetBackingField(propertyName).Field, propertyName);
            }
        }

        /// <summary>
        /// Sets the value of a property using an auto backing field.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="value">The value to assign to the property.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>True if the property was assigned a different value than it had previously.</returns>
        protected bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            lock (Synchronized)
            {
                return SetValue(ref GetBackingField(propertyName).Field, value, propertyName);
            }
        }

        /// <summary>
        /// Sets the value of a property using a backing field.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="field">The backing field.</param>
        /// <param name="value">The value to assign to the property.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>True if the property was assigned a different value than it had previously.</returns>
        protected abstract bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null);

        /// <summary>
        /// Gets the value of a property using a backing field.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="field">The backing field.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The value of the property.</returns>
        protected abstract T GetValue<T>(ref T field, [CallerMemberName] string propertyName = null);

        /// <summary>
        /// Called after a property is changed.
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {

        }

        /// <summary>
        /// Called when a property is about to change.
        /// </summary>
        protected virtual void OnPropertyChanging(string propertyName, object currentValue, object newValue)
        {

        }

        internal BoxedField GetBackingField(string propertyName)
        {
            Contract.Assert(Monitor.IsEntered(Synchronized));

            if (!backingFields.TryGetValue(propertyName, out var field))
            {
                var propInfo = GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (propInfo == null) throw new MemberAccessException("No backing field for property.");
                backingFields[propertyName] = field = new BoxedField() { Field = propInfo.PropertyType.IsValueType ? Activator.CreateInstance(propInfo.PropertyType) : null };
            }
            return field;
        }

        internal class BoxedField
        {
            public object Field;
        }

        private readonly Dictionary<string, BoxedField> backingFields = new Dictionary<string, BoxedField>();
        internal readonly object Synchronized = new object();
    }
}
