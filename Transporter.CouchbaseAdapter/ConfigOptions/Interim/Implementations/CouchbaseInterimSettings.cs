using Transporter.CouchbaseAdapter.ConfigOptions.Interim.Interfaces;

namespace Transporter.CouchbaseAdapter.ConfigOptions.Interim.Implementations
{
    public class CouchbaseInterimSettings : ICouchbaseInterimSettings
    {
        public CouchbaseInterimSettings()
        {
            Options = new CouchbaseInterimOptions();
        }

        public string Type { get; set; }
        public ICouchbaseInterimOptions Options { get; set; }

        public override string ToString()
        {
            return
                $"Type : {Type} Options : {Options}";
        }

        public string Host { get; set; }
    }
}