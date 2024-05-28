using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Transporter.PostgreSQLAdapter.Configs.Source.Interfaces;
using Transporter.PostgreSQLAdapter.Data.Interfaces;
using Transporter.PostgreSQLAdapter.Services.Source.Interfaces;

namespace Transporter.PostgreSQLAdapter.Services.Source.Implementations
{
    public class SourceService : ISourceService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SourceService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<dynamic>> GetSourceDataAsync(IPostgreSqlSourceSettings settings, IEnumerable<dynamic> ids)
        {
            using var connection =
                _dbConnectionFactory.GetConnection(settings.Options.ConnectionString);
            var query = await GetSourceQueryAsync(settings, ids);
            var result = await connection.QueryAsync<dynamic>(query);
            
            return result;
        }

        public async Task DeleteDataByListOfIdsAsync(IPostgreSqlSourceSettings settings, IEnumerable<dynamic> ids)
        {
            using var connection =
                _dbConnectionFactory.GetConnection(settings.Options.ConnectionString);
            var query = await GetDeleteQueryAsync(settings, ids);
            await connection.QueryAsync<dynamic>(query);
        }

        public async Task<IEnumerable<dynamic>> GetIdDataAsync(IPostgreSqlSourceSettings settings)
        {
            using var connection =
                _dbConnectionFactory.GetConnection(settings.Options.ConnectionString);
            var query = await GetSourceIdDataQueryAsync(settings);
            var result = await connection.QueryAsync<dynamic>(query);
            return result;
        }

        private async Task<string> GetSourceQueryAsync(IPostgreSqlSourceSettings settings, IEnumerable<dynamic> ids)
        {
            var sqlOptions = settings.Options;
            var query = new StringBuilder();
            var formattedIds = ids.Select(id => $"'{id}'");

            query.AppendLine($"SELECT * FROM {sqlOptions.Schema}.{sqlOptions.Table} ");
            query.AppendLine($"WHERE {settings.Options.IdColumn} IN ({string.Join(',', formattedIds)})");
            
            return await Task.FromResult(query.ToString());
        }
        
        private async Task<string> GetDeleteQueryAsync(IPostgreSqlSourceSettings settings, IEnumerable<dynamic> ids)
        {
            var sqlOptions = settings.Options;
            var query = new StringBuilder();
            var formattedIds = ids.Select(id => $"'{id}'");

            query.AppendLine($"DELETE FROM {sqlOptions.Schema}.{sqlOptions.Table}");
            query.AppendLine($"WHERE {settings.Options.IdColumn} IN ({string.Join(',', formattedIds)})");

            return await Task.FromResult(query.ToString());
        }
        
        private async Task<string> GetSourceIdDataQueryAsync(IPostgreSqlSourceSettings settings)
        {
            var sqlOptions = settings.Options;
            var query = new StringBuilder();
            query.AppendLine($"SELECT {sqlOptions.IdColumn} FROM {sqlOptions.Schema}.{sqlOptions.Table}");
            query.AppendLine($"WHERE {(string.IsNullOrEmpty(sqlOptions.Condition) ? "1=1" : sqlOptions.Condition)}");
            query.AppendLine($"LIMIT ({sqlOptions.BatchQuantity})");

            return await Task.FromResult(query.ToString());
        }
    }
}
