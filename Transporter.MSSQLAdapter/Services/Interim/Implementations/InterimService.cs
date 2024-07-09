using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Transporter.Core.Utils;
using Transporter.MSSQLAdapter.Configs.Interim.Interfaces;
using Transporter.MSSQLAdapter.Data.Interfaces;
using Transporter.MSSQLAdapter.Services.Interim.Interfaces;

namespace Transporter.MSSQLAdapter.Services.Interim.Implementations
{
    public class InterimService(IDbConnectionFactory dbConnectionFactory, IConfiguration configuration) : IInterimService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
        private readonly IConfiguration _configuration = configuration;

        public async Task<IEnumerable<dynamic>> GetInterimDataAsync(IMsSqlInterimSettings settings)
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

            return list.Select(dictionary => dictionary["Id"]).Cast<dynamic>().ToList();
        }

        public async Task DeleteAsync(IMsSqlInterimSettings settings, IEnumerable<dynamic> ids)
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

        private async Task<string> GetInterimQueryAsync(IMsSqlInterimSettings settings)
        {
            var timeDifferenceThreshold = GetTimeDifferenceThreshold();
            var sqlOptions = settings.Options;
            var query = new StringBuilder();
            query.AppendLine($"UPDATE TOP ({sqlOptions.BatchQuantity}) {sqlOptions.Schema}.{sqlOptions.Table}");
            query.AppendLine("SET Lmd=GETDATE() OUTPUT inserted.Id");
            query.AppendLine($"WHERE DataSourceName='{sqlOptions.DataSourceName}'");
            query.AppendLine($"AND DATEDIFF(minute, lmd, GETDATE()) > {timeDifferenceThreshold}");

            return await Task.FromResult(query.ToString());
        }

        private int GetTimeDifferenceThreshold() =>
            _configuration.GetSection(Constants.TimeDifferenceThreshold).Exists() ? _configuration.GetValue<int>(Constants.TimeDifferenceThreshold) : 5;

        private async Task<string> GetDeleteQueryAsync(IMsSqlInterimSettings settings, IEnumerable<dynamic> ids)
        {
            var sqlOptions = settings.Options;
            var query = new StringBuilder();
            query.AppendLine($"DELETE FROM {sqlOptions.Schema}.{sqlOptions.Table} ");
            query.AppendLine(
                $"WHERE Id IN ({string.Join(',', ids)}) AND DataSourceName='{settings.Options.DataSourceName}'");

            return await Task.FromResult(query.ToString());
        }
    }
}