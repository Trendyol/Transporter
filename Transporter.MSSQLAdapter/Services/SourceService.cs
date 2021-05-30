using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Transporter.MSSQLAdapter.Data;

namespace Transporter.MSSQLAdapter.Services
{
    public class SourceService : ISourceService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SourceService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<dynamic>> GetSourceDataAsync(ISqlSourceSettings settings)
        {
            using var connection =
                _dbConnectionFactory.GetConnection(settings.Options.ConnectionString);
            var query = await GetSourceQueryAsync(settings);
            var result = await connection.QueryAsync<dynamic>(query);
            return result;
        }

        private async Task<string> GetSourceQueryAsync(ISqlSourceSettings settings)
        {
            var sqlOptions = settings.Options;
            var query = new StringBuilder();
            query.AppendLine($"DELETE TOP({sqlOptions.BatchQuantity}) FROM {sqlOptions.Schema}.{sqlOptions.Table}");
            query.AppendLine("OUTPUT DELETED.*");
            SetSourceDataConditions(query, sqlOptions);

            return await Task.FromResult(query.ToString());
        }

        private void SetSourceDataConditions(StringBuilder query, IMsSqlSourceOptions sqlOptions)
        {
            query.AppendLine($"WHERE ({(string.IsNullOrEmpty(sqlOptions.Condition) ? "1=1" : sqlOptions.Condition)})");
        }
    }
}