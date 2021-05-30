using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Query;
using Octopus.Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.ConfigOptions.Source.Interfaces;
using Transporter.CouchbaseAdapter.Services.Interfaces;

namespace Transporter.CouchbaseAdapter.Services.Implementations
{
    public class SourceService : ISourceService
    {
        private readonly ICouchbaseProvider _couchbaseProvider;

        public SourceService(ICouchbaseProvider couchbaseProvider)
        {
            _couchbaseProvider = couchbaseProvider;
        }

        public async Task<IEnumerable<dynamic>> GetSourceDataAsync(ICouchbaseSourceSettings settings)
        {
            var cluster = await _couchbaseProvider.GetCluster(settings.Options.ConnectionData);
            var query = await GetSourceQueryAsync(settings);
            
            var result = await cluster.QueryAsync<dynamic>(query);
            var list = await TransformQueryResultToList(result);

            return list;
        }

        private static async Task<List<dynamic>> TransformQueryResultToList(IQueryResult<dynamic> result)
        {
            var list = new List<dynamic>();

            await result.Rows.ForEachAsync(x => { list.Add(x); });
            return list;
        }

        private async Task<string> GetSourceQueryAsync(ICouchbaseSourceSettings settings)
        {
            var options = settings.Options;
            var query = BuildQuery(settings, options);

            return await Task.FromResult(query.ToString());
        }

        private static StringBuilder BuildQuery(ICouchbaseSourceSettings settings, ICouchbaseSourceOptions options)
        {
            var query = new StringBuilder();
            query.AppendLine($"DELETE FROM {options.Bucket} b");
            query.AppendLine($"WHERE ({(string.IsNullOrEmpty(options.Condition) ? "1=1" : options.Condition)})");
            query.AppendLine($"limit {settings.Options.BatchQuantity} Returning b.*, meta().id");

            return query;
        }
    }
}