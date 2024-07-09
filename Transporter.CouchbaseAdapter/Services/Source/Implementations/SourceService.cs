using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Query;
using Transporter.CouchbaseAdapter.Configs.Source.Interfaces;
using Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.Services.Source.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Services.Source.Implementations
{
    public class SourceService(ICouchbaseProvider couchbaseProvider) : ISourceService
    {
        private readonly ICouchbaseProvider _couchbaseProvider = couchbaseProvider;

        public async Task<IEnumerable<dynamic>> GetSourceDataAsync(ICouchbaseSourceSettings settings,
            IEnumerable<dynamic> ids)
        {
            var cluster = await _couchbaseProvider.GetCluster(settings.Options.ConnectionData);
            var stringIdList = ids.Select(id => ((string)id.ToString()).SurroundWith("\"")).ToList();
            var query = await GetSourceQueryAsync(settings, stringIdList);

            var result = await cluster.QueryAsync<dynamic>(query);
            var list = await TransformQueryResultToList(result);

            return list;
        }

        public async Task DeleteDataByListOfIdsAsync(ICouchbaseSourceSettings settings,
            IEnumerable<dynamic> ids)
        {
            var cluster = await _couchbaseProvider.GetCluster(settings.Options.ConnectionData);
            var stringIdList = ids.Select(id => ((string)id.ToString()).SurroundWith("\"")).ToList();
            var query = await GetDeleteQueryAsync(settings, stringIdList);

            await cluster.QueryAsync<dynamic>(query);
        }

        public async Task<IEnumerable<dynamic>> GetIdDataAsync(ICouchbaseSourceSettings settings)
        {
            var cluster = await _couchbaseProvider.GetCluster(settings.Options.ConnectionData);
            var query = await GetSourceIdDataQueryAsync(settings);

            var result = await cluster.QueryAsync<dynamic>(query);
            var list = await TransformIdQueryResultToList(result);

            return list;
        }
        
        private static async Task<List<dynamic>> TransformIdQueryResultToList(IQueryResult<dynamic> result)
        {
            var list = new List<dynamic>();

            await result.Rows.ForEachAsync(row =>
            {
                list.Add(row);
            });

            return list;
        }

        private static async Task<List<dynamic>> TransformQueryResultToList(IQueryResult<dynamic> result)
        {
            var list = new List<dynamic>();

            await result.Rows.ForEachAsync(row =>
            {
                var data = row["SourceBucket"];
                data["id"] = row["id"];
                list.Add(data);
            });

            return list;
        }

        private async Task<string> GetSourceQueryAsync(ICouchbaseSourceSettings settings, IEnumerable<dynamic> ids)
        {
            var query = new StringBuilder();
            query.AppendLine($"SELECT *,meta(SourceBucket).id FROM `{settings.Options.Bucket}` SourceBucket");
            query.AppendLine($"USE KEYS [{string.Join(',', ids)}]");

            return await Task.FromResult(query.ToString());
        }

        private async Task<string> GetDeleteQueryAsync(ICouchbaseSourceSettings settings, IEnumerable<dynamic> ids)
        {
            var query = new StringBuilder();
            query.AppendLine($"DELETE FROM `{settings.Options.Bucket}`");
            query.AppendLine($"USE KEYS [{string.Join(',', ids)}]");

            return await Task.FromResult(query.ToString());
        }

        private async Task<string> GetSourceIdDataQueryAsync(ICouchbaseSourceSettings settings)
        {
            var options = settings.Options;
            var query = new StringBuilder();
            query.AppendLine($"SELECT meta().id FROM `{options.Bucket}` b");
            query.AppendLine($"WHERE ({(string.IsNullOrEmpty(options.Condition) ? "1=1" : options.Condition)})");
            query.AppendLine($"LIMIT {settings.Options.BatchQuantity}");

            return await Task.FromResult(query.ToString());
        }
    }
}