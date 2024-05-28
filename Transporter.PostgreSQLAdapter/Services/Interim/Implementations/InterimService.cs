using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Transporter.Core.Utils;
using Transporter.PostgreSQLAdapter.Configs.Interim.Interfaces;
using Transporter.PostgreSQLAdapter.Data.Interfaces;
using Transporter.PostgreSQLAdapter.Services.Interim.Interfaces;

namespace Transporter.PostgreSQLAdapter.Services.Interim.Implementations
{
    public class InterimService : IInterimService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public InterimService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<dynamic>> GetInterimDataAsync(IPostgreSqlInterimSettings settings)
        {
            using var connection =
                _dbConnectionFactory.GetConnection(settings.Options.ConnectionString);
            var query = await GetInterimQueryAsync(settings);
            var result = await connection.QueryAsync<dynamic>(query);

            var response = GetIdsFromResultData(result);

            return response;
        }

        private static IEnumerable<dynamic> GetIdsFromResultData(IEnumerable<dynamic> result)
        {
            var list = result.ToJson().ToObject<List<Dictionary<string, string>>>();

            return list.Select(dictionary => dictionary["id"]).Cast<dynamic>().ToList();
        }

        public async Task DeleteAsync(IPostgreSqlInterimSettings settings, IEnumerable<dynamic> ids)
        {
            using var connection =
                _dbConnectionFactory.GetConnection(settings.Options.ConnectionString);

            var dataItemIds = ids.ToList();
            if (!dataItemIds.Any())
            {
                return;
            }

            var query = await GetDeleteQueryAsync(settings, dataItemIds);
            await connection.QueryAsync<dynamic>(query);
        }

        private async Task<string> GetInterimQueryAsync(IPostgreSqlInterimSettings settings)
        {
            var sqlOptions = settings.Options;
            var query = new StringBuilder();
            query.AppendLine($"WITH limited_rows AS (");
            query.AppendLine($"    SELECT ctid");
            query.AppendLine($"    FROM {sqlOptions.Schema}.{sqlOptions.Table}");
            query.AppendLine($"    WHERE data_source_name = '{sqlOptions.DataSourceName}'");
            query.AppendLine($"    AND EXTRACT(EPOCH FROM (NOW() - last_modified_date)) > 300");
            query.AppendLine($"    ORDER BY ctid");
            query.AppendLine($"    LIMIT {sqlOptions.BatchQuantity}");
            query.AppendLine($")");
            query.AppendLine($"UPDATE {sqlOptions.Schema}.{sqlOptions.Table}");
            query.AppendLine($"SET last_modified_date = NOW()");
            query.AppendLine($"FROM limited_rows");
            query.AppendLine($"WHERE {sqlOptions.Schema}.{sqlOptions.Table}.ctid = limited_rows.ctid");
            query.AppendLine($"RETURNING {sqlOptions.Schema}.{sqlOptions.Table}.id;");
            
            return await Task.FromResult(query.ToString());
        }

        private async Task<string> GetDeleteQueryAsync(IPostgreSqlInterimSettings settings, IEnumerable<dynamic> ids)
        {
            var sqlOptions = settings.Options;
            var query = new StringBuilder();
            var formattedIds = ids.Select(id => $"'{id}'");

            query.AppendLine($"DELETE FROM {sqlOptions.Schema}.{sqlOptions.Table} ");
            query.AppendLine(
                $"WHERE id IN ({string.Join(',', formattedIds)}) AND data_source_name='{settings.Options.DataSourceName}'");
            
            return await Task.FromResult(query.ToString());
        }
    }
}
