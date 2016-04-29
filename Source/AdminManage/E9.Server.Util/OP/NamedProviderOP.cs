using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NM.OP
{
    public partial class NamedProviderOP
    {
        public NamedProviderOP(DataProvider dp)
        {
            DataProvider = dp;
        }

        public DataProvider DataProvider { get; private set; }

        public void RunAsync(DoWorkEventHandler doWork, RunWorkerCompletedEventHandler onFinish)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += doWork;
            worker.RunWorkerAsync();
            worker.RunWorkerCompleted += onFinish;
        }
    }
}
