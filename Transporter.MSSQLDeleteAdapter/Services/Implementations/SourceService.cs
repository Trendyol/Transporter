using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Transporter.MSSQLDeleteAdapter.Configs.Source.Interfaces;
using Transporter.MSSQLDeleteAdapter.Data.Interfaces;
using Transporter.MSSQLDeleteAdapter.Services.Interfaces;

namespace Transporter.MSSQLDeleteAdapter.Services.Implementations
{
    public class SourceService : ISourceService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SourceService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<dynamic>> GetSourceDataAsync(IMsSqlSourceSettings settings)
        {
            using var connection =
                _dbConnectionFactory.GetConnection(settings.Options.ConnectionString);
            var query = await GetSourceQueryAsync(settings);
            var result = await connection.ExecuteAsync(query);
            Console.WriteLine($"Delete affected rows count:{result}");
            return new List<dynamic>();
        }

        private async Task<string> GetSourceQueryAsync(IMsSqlSourceSettings settings)
        {
            var sqlOptions = settings.Options;
            var query = new StringBuilder();
            query.AppendLine($"DELETE FROM {sqlOptions.Schema}.{sqlOptions.Table}");
            query.AppendLine($"WHERE ({(string.IsNullOrEmpty(sqlOptions.Condition) ? "1=1" : sqlOptions.Condition)})");

            return await Task.FromResult(query.ToString());
        }
    }
}