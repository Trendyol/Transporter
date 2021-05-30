using Transporter.Core;
using Transporter.CouchbaseAdapter.ConfigOptions.Source.Implementations;
using Transporter.CouchbaseAdapter.ConfigOptions.Target.Implementations;

namespace Transporter.CouchbaseAdapter
{
    public class CouchbaseJobSettings : IJobSettings
    {
        public CouchbaseJobSettings()
        {
            Source = new CouchbaseSourceSettings();
            Target = new CouchbaseTargetSettings();
        }

        public string Name { get; set; }
        public string Cron { get; set; }
        public ISourceOptions Source { get; set; }
        public ITargetOptions Target { get; set; }

        public override string ToString()
        {
            return
                $"Name : {Name}\tCron : {Cron}\tSource : {Source}\tTarget : {Target}";
        }
    }
}