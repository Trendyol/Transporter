using Transporter.Core.Configs.Base.Interfaces;
using Transporter.Core.Configs.Interim.Interfaces;
using Transporter.Core.Configs.Source.Interfaces;
using Transporter.Core.Configs.Target.Interfaces;
using Transporter.CouchbaseAdapter.Configs.Interim.Implementations;
using Transporter.CouchbaseAdapter.Configs.Source.Implementations;
using Transporter.CouchbaseAdapter.Configs.Target.Implementations;

namespace Transporter.CouchbaseAdapter
{
    public class CouchbaseTransferJobSettings : ITransferJobSettings
    {
        public CouchbaseTransferJobSettings()
        {
            Source = new CouchbaseSourceSettings();
            Target = new CouchbaseTargetSettings();
            Interim = new CouchbaseInterimSettings();
        }

        public string Name { get; set; }
        public string Cron { get; set; }
        public ISourceOptions Source { get; set; }
        public ITargetOptions Target { get; set; }
        public IInterimOptions Interim { get; set; }

        public override string ToString()
        {
            return
                $"Name : {Name}\tCron : {Cron}\tSource : {Source}\tTarget : {Target}";
        }
    }
}