using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Transporter.Core.Utils;
using Transporter.PostgreSQLAdapter.Configs.Target.Interfaces;
using Transporter.PostgreSQLAdapter.Data.Interfaces;
using Transporter.PostgreSQLAdapter.Services.Target.Interfaces;

namespace Transporter.PostgreSQLAdapter.Services.Target.Implementations
{
    public class TargetService : ITargetService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public TargetService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task SetTargetDataAsync(IPostgreSqlTargetSettings setting, string data)
        {
            var insertData = data.ToObject<List<Dictionary<string, dynamic>>>();

            if (insertData is null || !insertData.Any()) return;
            
            var parameters = await GetParameters(insertData.ToList());

            using var connection = _dbConnectionFactory.GetConnection(setting.Options.ConnectionString);
            var query = await GetTargetInsertQueryAsync(setting, insertData.FirstOrDefault());
            
            await connection.ExecuteAsync(query, parameters);
        }

        public async Task SetTargetTemporaryDataAsync(IPostgreSqlTargetSettings setting, string data, string dataSourceName)
        {
            var insertData = data.ToObject<List<Dictionary<string, dynamic>>>();
            if (insertData is null || !insertData.Any()) return;

            var upperCasedInsertData = insertData
                .Select(ConvertKeysToId("id"))
                .ToList();

            foreach (var dictionary in upperCasedInsertData)
            {
                dictionary.Add("last_modified_date", DateTime.UtcNow);
                dictionary.Add("data_source_name", dataSourceName);
            }

            var parameters = await GetParameters(upperCasedInsertData);

            using var connection = _dbConnectionFactory.GetConnection(setting.Options.ConnectionString);
            var query = await GetTargetInsertIdDataQueryAsync(setting, upperCasedInsertData.FirstOrDefault());
            await ExecuteQueryWithParameters(connection, query, parameters);
        }

        private static async Task ExecuteQueryWithParameters(IDbConnection connection, string query,
            List<DynamicParameters> parameters)
        {
            try
            {
                await connection.ExecuteAsync(query, parameters);
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static Func<Dictionary<string, dynamic>, Dictionary<string, dynamic>> ConvertKeysToId(string id)
        {
            return dictionary => dictionary.ToDictionary(x => id, x => x.Value.ToString());
        }
        
        private async Task<List<DynamicParameters>> GetParameters(List<Dictionary<string, dynamic>> insertData)
        {
            var parameters = new List<DynamicParameters>();
            insertData.ForEach(sData =>
            {
                var values = sData.Select(x => new KeyValuePair<string, object>($"@{x.Key}", x.Value));
                parameters.Add(new DynamicParameters(values));
            });
            return await Task.Run(() => parameters);
        }

        private async Task<string> GetTargetInsertQueryAsync(IPostgreSqlTargetSettings settings,
            Dictionary<string, dynamic> insertData)
        {
            var excludedColumns = GetExcludedColumnsFromSettings(settings);
            var queryData = insertData.Where(x => !excludedColumns.Contains(x.Key.ToUpper())).ToList();
            var idColumn = settings.Options.IdColumn;
            var query = new StringBuilder();
            
            query.Append($"INSERT INTO {settings.Options.Schema}.{settings.Options.Table}");
            query.AppendLine(" (" + string.Join(',', queryData.Select(x => x.Key)));
            query.AppendLine(" ) SELECT ");
            query.AppendLine(string.Join(',', queryData.Select(x => $"@{x.Key}")));
            query.AppendLine($"WHERE NOT EXISTS (SELECT {idColumn} FROM {settings.Options.Schema}.{settings.Options.Table} WHERE {idColumn} = @{idColumn})");

            return await Task.FromResult(query.ToString());
        }

        private static IEnumerable<string> GetExcludedColumnsFromSettings(IPostgreSqlTargetSettings settings)
        {
            return settings.Options.ExcludedColumns?.Split(",").Select(x => x.ToUpper()) ??
                   Array.Empty<string>();
        }

        private async Task<string> GetTargetInsertIdDataQueryAsync(IPostgreSqlTargetSettings setting,
            Dictionary<string, dynamic> insertData)
        {
            var queryData = insertData.ToList();
            var query = new StringBuilder();
            query.AppendLine($"INSERT INTO {setting.Options.Schema}.{setting.Options.Table}");
            query.AppendLine(" (" + string.Join(',', queryData.Select(x => x.Key)));
            query.AppendLine(" ) SELECT ");
            query.AppendLine(string.Join(',', queryData.Select(x => $"@{x.Key}")));
            query.AppendLine($"WHERE NOT EXISTS (SELECT id FROM {setting.Options.Schema}.{setting.Options.Table} WHERE id = @id " +
                             "AND data_source_name = @data_source_name)");

            return await Task.FromResult(query.ToString());
        }
    }
}
