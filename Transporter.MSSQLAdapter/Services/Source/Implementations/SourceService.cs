using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Transporter.Core.Utils;
using Transporter.MSSQLAdapter.Configs.Source.Interfaces;
using Transporter.MSSQLAdapter.Data.Interfaces;
using Transporter.MSSQLAdapter.Services.Source.Interfaces;

namespace Transporter.MSSQLAdapter.Services.Source.Implementations
{
    public class SourceService(IDbConnectionFactory dbConnectionFactory) : ISourceService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

        public async Task<IEnumerable<dynamic>> GetSourceDataAsync(IMsSqlSourceSettings settings, IEnumerable<dynamic> ids)
        {
            using var connection =
                _dbConnectionFactory.GetConnection(settings.Options.ConnectionString);
            var query = await GetSourceQueryAsync(settings, ids);
            var result = await connection.QueryAsync<dynamic>(query);
            
            return result;
        }

        public async Task DeleteDataByListOfIdsAsync(IMsSqlSourceSettings settings, IEnumerable<dynamic> ids)
        {
            using var connection =
                _dbConnectionFactory.GetConnection(settings.Options.ConnectionString);
            var query = await GetDeleteQueryAsync(settings, ids);
            await connection.QueryAsync<dynamic>(query);
        }

        public async Task<IEnumerable<dynamic>> GetIdDataAsync(IMsSqlSourceSettings settings)
        {
            using var connection =
                _dbConnectionFactory.GetConnection(settings.Options.ConnectionString);
            var query = await GetSourceIdDataQueryAsync(settings);
            var result = await connection.QueryAsync<dynamic>(query);
            return result;
        }

        private async Task<string> GetSourceQueryAsync(IMsSqlSourceSettings settings, IEnumerable<dynamic> ids)
        {
            var sqlOptions = settings.Options;
            var query = new StringBuilder();
            query.AppendLine($"SELECT * FROM {sqlOptions.Schema}.{sqlOptions.Table} ");
            query.AppendLine($"WHERE {settings.Options.IdColumn} IN ({string.Join(',', ids)})");

            return await Task.FromResult(query.ToString());
        }
        
        private async Task<string> GetDeleteQueryAsync(IMsSqlSourceSettings settings, IEnumerable<dynamic> ids)
        {
            var sqlOptions = settings.Options;
            var query = new StringBuilder();
            query.AppendLine($"DELETE FROM {sqlOptions.Schema}.{sqlOptions.Table}");
            query.AppendLine($"WHERE {settings.Options.IdColumn} IN ({string.Join(',', ids)})");

            return await Task.FromResult(query.ToString());
        }
        
        private async Task<string> GetSourceIdDataQueryAsync(IMsSqlSourceSettings settings)
        {
            var sqlOptions = settings.Options;
            var query = new StringBuilder();
            query.AppendLine($"SELECT TOP({sqlOptions.BatchQuantity}) {sqlOptions.IdColumn} FROM {sqlOptions.Schema}.{sqlOptions.Table}");
            query.AppendLine($"WHERE {(string.IsNullOrEmpty(sqlOptions.Condition) ? "1=1" : sqlOptions.Condition)}");

            return await Task.FromResult(query.ToString());
        }
    }
}