namespace Meyer.Socrates.Services
{
    using Meyer.Socrates.Data;
    using System;
    using System.Threading.Tasks;

    public abstract class ThresholdProgress<T>: IProgress<T>
    {
        IProgress<T> progressBase;
        private T lastReport;
        private readonly object Synchronized = new object();

        public ThresholdProgress(IProgress<T> progressBase) : this(progressBase, default(T))
        {
        }

        public ThresholdProgress(IProgress<T> progressBase, T initialValue)
        {
            this.progressBase = progressBase ?? ProgressDummy<T>.Instance;
            this.lastReport = initialValue;
        }

        public void Report(T value)
        {
            Task.Run(() => ReportAsync(value));
            //ReportAsync(value);
        }

        private void ReportAsync(T value)
        {
            if (Compare(lastReport, value))
            {
                lock (Synchronized)
                {
                    if (Compare(lastReport, value))
                    {
                        progressBase.Report(value);
                        lastReport = value;
                    }
                }
            }

        }

        protected abstract bool Compare(T lastReport, T currentReport);
    }

    internal class ProgressInfoThresholdProgress: ThresholdProgress<ProgressInfo>
    {
        public ProgressInfoThresholdProgress(IProgress<ProgressInfo> progressBase) : base(progressBase)
        {
        }

        public ProgressInfoThresholdProgress(IProgress<ProgressInfo> progressBase, ProgressInfo initialValue) : base(progressBase, initialValue)
        {
        }

        protected override bool Compare(ProgressInfo lastReport, ProgressInfo currentReport)
        {
            var tmp = currentReport.ProgressValue - lastReport.ProgressValue;
            return tmp < Ms3dmm.PROGRESS_THRESHOLD && tmp > 0;
        }
    }

    public class ThrottleProgress<T>: IProgress<T>
    {
        private IProgress<T> progressBase;
        private TimeSpan cutOff;
        private DateTime lastCall;
        private readonly object Synchronized = new object();

        public ThrottleProgress(IProgress<T> progressBase, TimeSpan cutOff)
        {
            this.progressBase = progressBase;
            this.cutOff = cutOff;
        }

        public void Report(T value)
        {
            if (progressBase == null) return;

            if ((DateTime.Now - lastCall) > cutOff)
            {
                lock (Synchronized)
                {
                    if ((DateTime.Now - lastCall) > cutOff)
                    {
                        lastCall = DateTime.Now;
                        progressBase.Report(value);
                    }
                }
            }
        }
    }

    internal class ThrottleProgressInfo: ThrottleProgress<ProgressInfo>
    {
        private static readonly TimeSpan CUT_OFF = TimeSpan.FromMilliseconds(5);

        public ThrottleProgressInfo(IProgress<ProgressInfo> progressBase) : base(progressBase, CUT_OFF)
        {
        }
    }
}
