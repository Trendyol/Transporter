using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using Couchbase.Query;
using Transporter.CouchbaseAdapter.ConfigOptions.Interim.Interfaces;
using Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.Services.Interim.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Services.Interim.Implementations
{
    public class InterimService : IInterimService
    {
        private readonly ICouchbaseProvider _couchbaseProvider;
        private readonly IBucketProvider _bucketProvider;

        public InterimService(ICouchbaseProvider couchbaseProvider, IBucketProvider bucketProvider)
        {
            _couchbaseProvider = couchbaseProvider;
            _bucketProvider = bucketProvider;
        }

        public async Task<IEnumerable<dynamic>> GetInterimDataAsync(ICouchbaseInterimSettings settings)
        {
            var cluster = await _couchbaseProvider.GetCluster(settings.Options.ConnectionData);
            var query = await GetInterimQueryAsync(settings);

            var result = await cluster.QueryAsync<dynamic>(query);
            var list = await TransformQueryResultToList(result);

            return list;
        }

        public async Task DeleteAsync(ICouchbaseInterimSettings settings, IEnumerable<dynamic> ids)
        {
            var dataItemIds = ids.ToList();
            var dataSourceName = settings.Options.DataSourceName;
            if (!dataItemIds.Any())
            {
                return;
            }

            try
            {
                var collection = await GetCollectionAsync(settings.Options.ConnectionData, settings.Options.Bucket);
                var tasks = new List<Task>();

                for (var i = 0; i < dataItemIds.Count; i++)
                {
                    var task = collection.RemoveAsync($"{dataItemIds[i]}_{dataSourceName}");
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
            }
            catch (DocumentNotFoundException)
            {
            }
        }

        private static async Task<List<dynamic>> TransformQueryResultToList(IQueryResult<dynamic> result)
        {
            var list = new List<dynamic>();

            await result.Rows.ForEachAsync(x => { list.Add(x); });
            return list;
        }

        private async Task<string> GetInterimQueryAsync(ICouchbaseInterimSettings settings)
        {
            var options = settings.Options;
            var query = BuildUpdateQuery(settings, options);

            return await Task.FromResult(query.ToString());
        }

        private static StringBuilder BuildUpdateQuery(ICouchbaseInterimSettings settings,
            ICouchbaseInterimOptions options)
        {
            var query = new StringBuilder();
            query.AppendLine($"UPDATE {options.Bucket} SET lmd=CLOCK_LOCAL()");
            query.AppendLine($"WHERE dataSourceName='{options.DataSourceName}'");
            query.AppendLine("AND DATE_DIFF_STR(CLOCK_LOCAL(),lmd, 'second') > 5"); // TODO second minute olacak ileride.
            query.AppendLine($"limit {settings.Options.BatchQuantity} RETURNING RAW id");

            return query;
        }

        private async Task<ICouchbaseCollection> GetCollectionAsync(ConnectionData connectionData, string bucket)
        {
            return await (await GetBucketAsync(connectionData, bucket)).DefaultCollectionAsync();
        }

        private Task<IBucket> GetBucketAsync(ConnectionData connectionData, string bucket)
        {
            return _bucketProvider.GetBucket(connectionData, bucket);
        }
    }
}