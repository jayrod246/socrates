namespace Meyer.Socrates.Data
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SocPropertyAttribute: Attribute
    {
        readonly SocPropertyFlags propertyFlags;

        public SocPropertyAttribute(SocPropertyFlags propertyFlags)
        {
            this.propertyFlags = propertyFlags;
        }

        public SocPropertyFlags PropertyFlags
        {
            get { return propertyFlags; }
        }
    }
}
