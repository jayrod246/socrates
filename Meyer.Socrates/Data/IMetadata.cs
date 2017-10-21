namespace Meyer.Socrates.Data
{
    internal interface IMetadata<T>
    {
        object this[string propertyName] { get; set; }
        T Instance { get; }
        object GetProperty(string propertyName);
        void SetProperty(string propertyName, object value);
    }
}
