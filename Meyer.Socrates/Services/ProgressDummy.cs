namespace Meyer.Socrates.Services
{
    internal class ProgressDummy<T>: System.IProgress<T>
    {
        private static volatile ProgressDummy<T> dummy;

        private ProgressDummy()
        {

        }

        public static ProgressDummy<T> Instance
        {
            get
            {
                if (dummy == null) dummy = new ProgressDummy<T>();
                return dummy;
            }
        }

        public void Report(T value)
        {
        }
    }
}
