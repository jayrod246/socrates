namespace Meyer.Socrates.Data
{
    public struct ProgressInfo
    {
        public ProgressInfo(double progressValue) : this(progressValue, string.Empty)
        {

        }

        public ProgressInfo(double progressValue, string workDescription)
        {
            ProgressValue = progressValue;
            WorkDescription = workDescription;
        }

        public string WorkDescription { get; set; }
        public double ProgressValue { get; set; }
    }
}
