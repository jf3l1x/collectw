using System.ServiceProcess;
using CollectW.Config;

namespace CollectW.Service
{
    public partial class Daemon : ServiceBase
    {
        private Configuration _configuration;
        private Collector _collector;
        public Daemon()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _configuration=new Configuration();
            _collector = _configuration.CreateCollector();
            _collector.Start();
        }

        protected override void OnStop()
        {
            _collector.Stop();
            _collector.Dispose();
            _configuration.Dispose();
        }
    }
}