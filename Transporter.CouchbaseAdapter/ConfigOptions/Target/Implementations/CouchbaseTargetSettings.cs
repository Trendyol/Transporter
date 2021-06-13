using Transporter.CouchbaseAdapter.ConfigOptions.Target.Interfaces;

namespace Transporter.CouchbaseAdapter.ConfigOptions.Target.Implementations
{
    public class CouchbaseTargetSettings : ICouchbaseTargetSettings
    {
        public CouchbaseTargetSettings()
        {
            Options = new CouchbaseTargetOptions();
        }

        public string Type { get; set; }
        public ICouchbaseTargetOptions Options { get; set; }

        public override string ToString()
        {
            return
                $"Type : {Type} Options : {Options}";
        }

        public string Host { get; set; }
    }
}