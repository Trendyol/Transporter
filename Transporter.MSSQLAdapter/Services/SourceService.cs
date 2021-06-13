using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Transporter.Core;
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

        public async Task SetSourceDataAsync(ISqlSourceSettings setting, string data)
        {
            var insertData = data.ToObject<List<Dictionary<string, string>>>();
            if (insertData is null || !insertData.Any()) return;

            var parameters = await GetParameters(insertData, setting);

            using var connection = _dbConnectionFactory.GetConnection(setting.Options.ConnectionString);
            var query = await GetSourceInsertQueryAsync(setting, insertData.FirstOrDefault());
            await connection.ExecuteAsync(query, parameters);
        }

        private async Task<List<DynamicParameters>> GetParameters(List<Dictionary<string, string>> insertData,
            ISqlSourceSettings sqlSourceSettings)
        {
            var parameters = new List<DynamicParameters>();

            if (sqlSourceSettings.Options.IsIdAutoIncrementOn)
            {
                insertData.First()?.Remove(sqlSourceSettings.Options.IdColumn);
            }

            insertData.ForEach(sData =>
            {
                var values = sData.Select(x => new KeyValuePair<string, object>($"@{x.Key}", x.Value));
                parameters.Add(new DynamicParameters(values));
            });
            return await Task.Run(() => parameters);
        }

        private async Task<string> GetSourceInsertQueryAsync(ISqlSourceSettings setting,
            Dictionary<string, string> insertData)
        {
            var queryData = insertData.ToList();
            var query = new StringBuilder();
            query.Append($"INSERT INTO {setting.Options.Schema}.{setting.Options.Table}");
            query.AppendLine(" (" + string.Join(',', queryData.Select(x => x.Key)));
            query.AppendLine(" ) VALUES (");
            query.AppendLine(string.Join(',', queryData.Select(x => $"@{x.Key}")));
            query.AppendLine(" )");

            return await Task.FromResult(query.ToString());
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