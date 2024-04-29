using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Transporter.Core.Utils;
using Transporter.MSSQLAdapter.Configs.Target.Interfaces;
using Transporter.MSSQLAdapter.Data.Interfaces;
using Transporter.MSSQLAdapter.Services.Target.Interfaces;

namespace Transporter.MSSQLAdapter.Services.Target.Implementations
{
    public class TargetService : ITargetService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        
        public TargetService(IDbConnectionFactory dbConnectionFactory, IDateTimeProvider dateTimeProvider)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task SetTargetDataAsync(IMsSqlTargetSettings setting, string data)
        {
            var insertData = data.ToObject<List<Dictionary<string, string>>>();

            if (insertData is null || !insertData.Any()) return;

            var upperCasedInsertData = insertData
                .Select(ConvertDictionaryKeysToUpperCase())
                .ToList();

            var parameters = await GetParameters(upperCasedInsertData);

            using var connection = _dbConnectionFactory.GetConnection(setting.Options.ConnectionString);
            var query = await GetTargetInsertQueryAsync(setting, upperCasedInsertData.FirstOrDefault());
            await connection.ExecuteAsync(query, parameters);
        }

        public async Task SetTargetTemporaryDataAsync(IMsSqlTargetSettings setting, string data, string dataSourceName)
        {
            var insertData = data.ToObject<List<Dictionary<string, string>>>();
            if (insertData is null || !insertData.Any()) return;

            var upperCasedInsertData = insertData
                .Select(ConvertKeysToId("Id"))
                .ToList();

            foreach (var dictionary in upperCasedInsertData)
            {
                dictionary.Add("Lmd", _dateTimeProvider.Now.ToString(CultureInfo.InvariantCulture));
                dictionary.Add("DataSourceName", dataSourceName);
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
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static Func<Dictionary<string, string>, Dictionary<string, string>> ConvertKeysToId(string id)
        {
            return dictionary => dictionary.ToDictionary(x => id, x => x.Value);
        }

        private static Func<Dictionary<string, string>, Dictionary<string, string>> ConvertDictionaryKeysToUpperCase()
        {
            return dictionary => dictionary.ToDictionary(CapitalizeKey, x => x.Value);
        }

        private static string CapitalizeKey(KeyValuePair<string, string> x)
        {
            return char.ToUpper(x.Key[0]) + x.Key[1..];
        }

        private async Task<List<DynamicParameters>> GetParameters(List<Dictionary<string, string>> insertData)
        {
            var parameters = new List<DynamicParameters>();
            insertData.ForEach(sData =>
            {
                var values = sData.Select(x => new KeyValuePair<string, object>($"@{x.Key}", x.Value));
                parameters.Add(new DynamicParameters(values));
            });
            return await Task.Run(() => parameters);
        }

        private async Task<string> GetTargetInsertQueryAsync(IMsSqlTargetSettings settings,
            Dictionary<string, string> insertData)
        {
            var excludedColumns = GetExcludedColumnsFromSettings(settings);
            var queryData = insertData.Where(x => !excludedColumns.Contains(x.Key.ToUpper())).ToList();
            var idColumn = settings.Options.IdColumn;
            var query = new StringBuilder();

            query.Append(
                $"IF NOT EXISTS (SELECT {idColumn} FROM {settings.Options.Schema}.{settings.Options.Table} WHERE {idColumn} = @{idColumn}) ");
            query.Append($"INSERT INTO {settings.Options.Schema}.{settings.Options.Table}");
            query.AppendLine(" (" + string.Join(',', queryData.Select(x => x.Key)));
            query.AppendLine(" ) VALUES (");
            query.AppendLine(string.Join(',', queryData.Select(x => $"@{x.Key}")));
            query.AppendLine(" )");

            return await Task.FromResult(query.ToString());
        }

        private static IEnumerable<string> GetExcludedColumnsFromSettings(IMsSqlTargetSettings settings)
        {
            return settings.Options.ExcludedColumns?.Split(",").Select(x => x.ToUpper()) ??
                   Array.Empty<string>();
        }

        private async Task<string> GetTargetInsertIdDataQueryAsync(IMsSqlTargetSettings setting,
            Dictionary<string, string> insertData)
        {
            var queryData = insertData.ToList();
            var query = new StringBuilder();
            query.Append(
                $"IF NOT EXISTS (SELECT Id FROM {setting.Options.Schema}.{setting.Options.Table} WHERE Id = @Id " +
                "AND DataSourceName = @DataSourceName) ");
            query.AppendLine($"INSERT INTO {setting.Options.Schema}.{setting.Options.Table}");
            query.AppendLine(" (" + string.Join(',', queryData.Select(x => x.Key)));
            query.AppendLine(" ) VALUES (");
            query.AppendLine(string.Join(',', queryData.Select(x => $"@{x.Key}")));
            query.AppendLine(" )");

            return await Task.FromResult(query.ToString());
        }
    }
}