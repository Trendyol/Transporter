using Transporter.CouchbaseAdapter.Configs.Target.Interfaces;

namespace Transporter.CouchbaseAdapter.Configs.Target.Implementations
{
    public class CouchbaseTargetSettings : ICouchbaseTargetSettings
    {
        public CouchbaseTargetSettings() => Options = new CouchbaseTargetOptions();

        public string Type { get; set; }
        public ICouchbaseTargetOptions Options { get; set; }

        public override string ToString() => $"Type : {Type} Options : {Options}";

        public string Host { get; set; }
    }
}