using Transporter.CouchbaseAdapter.Configs.Source.Interfaces;

namespace Transporter.CouchbaseAdapter.Configs.Source.Implementations
{
    public class CouchbaseSourceSettings : ICouchbaseSourceSettings
    {
        public CouchbaseSourceSettings()
        {
            Options = new CouchbaseSourceOptions();
        }

        public string Type { get; set; }
        public ICouchbaseSourceOptions Options { get; set; }

        public override string ToString()
        {
            return
                $"Type : {Type} Options : {Options}";
        }

        public string Host { get; set; }
    }
}