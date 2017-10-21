namespace Meyer.Socrates.Data.Sections
{
    using System;

    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SectionKeyAttribute : Attribute
    {
        readonly string key;

        public SectionKeyAttribute(string key)
        {
            this.key = key;
        }

        public string Key
        {
            get { return key; }
        }
    }
}
