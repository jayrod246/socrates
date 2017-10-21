namespace Meyer.Socrates.Data
{
    public interface IResolvable
    {
        T Resolve<T>();
    }

    public interface IResolvable<out T>
    {
        T Resolve();
    }
}
