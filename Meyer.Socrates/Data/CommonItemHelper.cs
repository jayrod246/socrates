namespace Meyer.Socrates.Data
{
    using Meyer.Socrates.Collections.Generic;
    using System.Collections.Generic;

    internal static class CommonItemHelper
    {
        internal static void ValidateValue<TItem, TCollection, TValue>(TItem item, ref TCollection container, ref TValue field, TValue value) where TCollection : CollectionBase<TItem>
        {
            if (container != null && !EqualityComparer<TValue>.Default.Equals(field, value))
            {
                var oldValue = field;
                var oldContainer = container;
                try
                {
                    container = null;
                    field = value;
                    oldContainer.DoValidateItem(item);
                }
                finally
                {
                    field = oldValue;
                    container = oldContainer;
                }
            }
        }
    }
}
