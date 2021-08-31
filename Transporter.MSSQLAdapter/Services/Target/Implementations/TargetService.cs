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

        public TargetService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task SetTargetDataAsync(IMsSqlTargetSettings setting, string data)
        {
            var insertData = ConvertDataToDictionary(data);

            if (insertData is null || !insertData.Any()) return;
            
            var upperCasedInsertData = insertData
                .Select(ConvertDictionaryKeysToUpperCase())
                .ToList();

            var parameters = await GetParameters(upperCasedInsertData);

            using var connection = _dbConnectionFactory.GetConnection(setting.Options.ConnectionString);
            var query = await GetTargetInsertQueryAsync(setting, upperCasedInsertData.FirstOrDefault());
            await connection.ExecuteAsync(query, parameters);
        }

        private static List<Dictionary<string, string>> ConvertDataToDictionary(string stringData)
        {
            var dynamicListData = stringData.ToObject<List<dynamic>>();
            var newInsertData = new List<dynamic>();

            foreach (var dynamicData in dynamicListData)
            {
                var insertData = dynamicData["SourceBucket"];
                insertData["id"] = dynamicData["id"];
                newInsertData.Add(insertData);
            }

            return newInsertData.ToJson().ToObject<List<Dictionary<string, string>>>();
        }

        public async Task SetTargetTemporaryDataAsync(IMsSqlTargetSettings setting, string data, string dataSourceName)
        {
            var insertData = data.ToObject<List<Dictionary<string, string>>>();
            if (insertData is null || !insertData.Any()) return;

            var upperCasedInsertData = insertData
                .Select(ConvertDictionaryKeysToUpperCase())
                .ToList();

            foreach (var dictionary in upperCasedInsertData)
            {
                dictionary.Add("Lmd", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                dictionary.Add("DataSourceName", dataSourceName);
            }

            var parameters = await GetParameters(upperCasedInsertData);

            using var connection = _dbConnectionFactory.GetConnection(setting.Options.ConnectionString);
            var query = await GetTargetInsertIdDataQueryAsync(setting, upperCasedInsertData.FirstOrDefault());
            await ExecuteQueryWithParameters(connection, query, parameters);
        }

        private static async Task ExecuteQueryWithParameters(IDbConnection connection, string query, List<DynamicParameters> parameters)
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

        private static Func<Dictionary<string, string>, Dictionary<string, string>> ConvertDictionaryKeysToUpperCase()
        {
            return dictionary => dictionary.ToDictionary(x => x.Key.ToUpper(), x => x.Value);
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

        private async Task<string> GetTargetInsertQueryAsync(IMsSqlTargetSettings setting,
            Dictionary<string, string> insertData)
        {
            var excludedColumns = setting.Options.ExcludedColumns?.Split(",") ?? Array.Empty<string>();
            var queryData = insertData.Where(x => !excludedColumns.Contains(x.Key)).ToList();
            var query = new StringBuilder();
            query.Append($"INSERT INTO {setting.Options.Schema}.{setting.Options.Table}");
            query.AppendLine(" (" + string.Join(',', queryData.Select(x => x.Key)));
            query.AppendLine(" ) VALUES (");
            query.AppendLine(string.Join(',', queryData.Select(x => $"@{x.Key}")));
            query.AppendLine(" )");

            return await Task.FromResult(query.ToString());
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