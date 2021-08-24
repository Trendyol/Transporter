using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.KeyValue;
using Couchbase.Query;
using Transporter.Core;
using Transporter.CouchbaseAdapter.ConfigOptions.Source.Interfaces;
using Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.Services.Source.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Services.Source.Implementations
{
    public class SourceService : ISourceService
    {
        private readonly ICouchbaseProvider _couchbaseProvider;
        private readonly IBucketProvider _bucketProvider;

        public SourceService(ICouchbaseProvider couchbaseProvider, IBucketProvider bucketProvider)
        {
            _couchbaseProvider = couchbaseProvider;
            _bucketProvider = bucketProvider;
        }

        public async Task<IEnumerable<dynamic>> GetSourceDataAsync(ICouchbaseSourceSettings settings,
            IEnumerable<dynamic> ids)
        {
            var cluster = await _couchbaseProvider.GetCluster(settings.Options.ConnectionData);
            var query = await GetSourceQueryAsync(settings, ids);

            var result = await cluster.QueryAsync<dynamic>(query);
            var list = await TransformQueryResultToList(result);

            return list;
        }
        
        public async Task DeleteDataByListOfIdsAsync(ICouchbaseSourceSettings settings,
            IEnumerable<dynamic> ids)
        {
            var cluster = await _couchbaseProvider.GetCluster(settings.Options.ConnectionData);
            var query = await GetDeleteQueryAsync(settings, ids);

            await cluster.QueryAsync<dynamic>(query);
        }

        public async Task<IEnumerable<dynamic>> GetIdDataAsync(ICouchbaseSourceSettings settings)
        {
            var cluster = await _couchbaseProvider.GetCluster(settings.Options.ConnectionData);
            var query = await GetSourceIdDataQueryAsync(settings);

            var result = await cluster.QueryAsync<dynamic>(query);
            var list = await TransformQueryResultToList(result);

            return list;
        }

        public async Task SetTargetDataAsync(ICouchbaseSourceSettings settings, string data)
        {
            var insertDataItems = data.ToObject<List<dynamic>>();
            var dataItemIds = data.ToObject<List<IdObject>>();

            if (insertDataItems is null || !insertDataItems.Any())
            {
                return;
            }

            var tasks = await InsertItems(settings, insertDataItems, dataItemIds);

            await Task.WhenAll(tasks);
        }

        private async Task<List<Task<IMutationResult>>> InsertItems(ICouchbaseSourceSettings settings,
            List<dynamic> insertDataItems, List<IdObject> dataItemIds)
        {
            var collection = await GetCollectionAsync(settings.Options.ConnectionData, settings.Options.Bucket);
            var tasks = new List<Task<IMutationResult>>();

            for (var i = 0; i < insertDataItems.Count; i++)
            {
                var task = collection.InsertAsync(dataItemIds[i].Id, insertDataItems[i]);
                tasks.Add(task);
            }

            return tasks;
        }

        private async Task<ICouchbaseCollection> GetCollectionAsync(ConnectionData connectionData, string bucket)
        {
            return await (await GetBucketAsync(connectionData, bucket)).DefaultCollectionAsync();
        }

        private Task<IBucket> GetBucketAsync(ConnectionData connectionData, string bucket)
        {
            return _bucketProvider.GetBucket(connectionData, bucket);
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

        private async Task<string> GetSourceQueryAsync(ICouchbaseSourceSettings settings, IEnumerable<dynamic> ids)
        {
            var query = new StringBuilder();
            query.AppendLine($"SELECT * FROM {settings.Options.Bucket}");
            query.AppendLine($"USE KEYS [{string.Join(',', ids)}]");

            return await Task.FromResult(query.ToString());
        }
        
        private async Task<string> GetDeleteQueryAsync(ICouchbaseSourceSettings settings, IEnumerable<dynamic> ids)
        {
            var query = new StringBuilder();
            query.AppendLine($"DELETE FROM {settings.Options.Bucket}");
            query.AppendLine($"USE KEYS [{string.Join(',', ids)}]");

            return await Task.FromResult(query.ToString());
        }

        private async Task<string> GetSourceIdDataQueryAsync(ICouchbaseSourceSettings settings)
        {
            var options = settings.Options;
            var query = new StringBuilder();
            query.AppendLine($"SELECT meta().id FROM {options.Bucket} b");
            query.AppendLine($"WHERE ({(string.IsNullOrEmpty(options.Condition) ? "1=1" : options.Condition)})");
            query.AppendLine($"LIMIT {settings.Options.BatchQuantity}");

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