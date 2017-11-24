namespace Meyer.Socrates.Data
{
    public interface IResolvable<out T>
    {
        T Resolve();
    }
}
