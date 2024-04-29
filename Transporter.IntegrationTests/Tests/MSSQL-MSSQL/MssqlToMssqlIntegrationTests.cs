using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Transporter.Core.Utils;
using Transporter.IntegrationTests.Base;
using Transporter.IntegrationTests.Constants;
using Transporter.IntegrationTests.Objects;
using TransporterService;

namespace Transporter.IntegrationTests.Tests.Couchbase_MSSQL
{
    public class MssqlToMssqlIntegrationTests : IntegrationTestBase
    {
        private Fixture _fixture;
        private Mock<IDateTimeProvider> _mockDateTimeProvider;
        
        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _mockDateTimeProvider.Setup(m => m.Now).Returns(DateTime.UtcNow.AddMinutes(-5));
        }

        [Test]
        public async Task Transporter_ShouldTransportData()
        {
            //Arrange
            await using var connection = new SqlConnection(MssqlConnectionString);
            connection.Open();

            var id = CreateFixture<long>();
            var age = CreateFixture<int>();
            await connection.ExecuteAsync("Create Table dbo.TestTable_MsSql_MsSql (Id int, Age int)");
            var insertScript = $"INSERT INTO dbo.TestTable_MsSql_MsSql values({id},{age})";
            await connection.ExecuteAsync(insertScript);
            await connection.ExecuteAsync("Create Table dbo.TestTable_Interim_Mssql (dataSourceName nvarchar(max), lmd DateTime, Id int)");
            await connection.ExecuteAsync("Create Table dbo.TestTable_Target_Mssql_Mssql (Id nvarchar(max), Age int)");
            
            var configSettings = GetConfigSettings();
            
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configSettings)
                .Build();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            mockDateTimeProvider.Setup(m => m.Now).Returns(DateTime.UtcNow.AddMinutes(-5));
            var build = Program.CreateHostBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddConfiguration(configuration);
                })
                .ConfigureServices((_, services) =>
                {
                    services.AddTransient(_ =>_mockDateTimeProvider.Object);
                }) 
                .Build();
            
            
            await build.StartAsync();
            Thread.Sleep(65000);
            await build.StopAsync();

            //Verify
            var userFromTargetTable = await connection.QueryFirstAsync<UserClass>("SELECT Id FROM [dbo].[TestTable_Target_Mssql_Mssql]");
            id.Should().Be(userFromTargetTable.Id);
        }

        private Dictionary<string, string> GetConfigSettings()
        {
            var configSettings = new Dictionary<string, string>
            {
                { "PollingJobSettings:0:Name",CreateFixture<string>()},
                { "PollingJobSettings:0:Cron",JobConstants.MsSqlToMsSql.PollingJobSettingsCron},
                { "PollingJobSettings:0:Source:Type",ContainerConstants.MsSql.Type},
                { "PollingJobSettings:0:Source:Host",ContainerConstants.MsSql.Localhost},
                { "PollingJobSettings:0:Source:Options:ConnectionString",MssqlConnectionString},
                { "PollingJobSettings:0:Source:Options:Schema",MssqlSchemaName},
                { "PollingJobSettings:0:Source:Options:Table",JobConstants.MsSqlToMsSql.SourceTableName},
                { "PollingJobSettings:0:Source:Options:IdColumn",JobConstants.MsSqlToMsSql.IdColumn},
                { "PollingJobSettings:0:Source:Options:Condition",string.Empty},
                { "PollingJobSettings:0:Source:Options:BatchQuantity",ContainerConstants.BatchQuantity.ToString()},
                { "PollingJobSettings:0:Target:Host",ContainerConstants.MsSql.Localhost},
                { "PollingJobSettings:0:Target:Type",ContainerConstants.MsSql.Type},
                { "PollingJobSettings:0:Target:Options:ConnectionString",MssqlConnectionString},
                { "PollingJobSettings:0:Target:Options:Schema",MssqlSchemaName},
                { "PollingJobSettings:0:Target:Options:Table",JobConstants.MsSqlToMsSql.InterimTableName},
                { "TransferJobSettings:0:Name", CreateFixture<string>() },
                { "TransferJobSettings:0:Cron", JobConstants.MsSqlToMsSql.TransferJobSettingsCron},
                { "TransferJobSettings:0:Source:Host", ContainerConstants.MsSql.Localhost},
                { "TransferJobSettings:0:Source:Type", ContainerConstants.MsSql.Type},
                { "TransferJobSettings:0:Source:Options:ConnectionString", MssqlConnectionString},
                { "TransferJobSettings:0:Source:Options:Schema", MssqlSchemaName},
                { "TransferJobSettings:0:Source:Options:Table", JobConstants.MsSqlToMsSql.SourceTableName},
                { "TransferJobSettings:0:Source:Options:IdColumn", JobConstants.MsSqlToMsSql.IdColumn},
                { "TransferJobSettings:0:Interim:Host", ContainerConstants.MsSql.Localhost},
                { "TransferJobSettings:0:Interim:Type",ContainerConstants.MsSql.Type},
                { "TransferJobSettings:0:Interim:Options:ConnectionString", MssqlConnectionString},
                { "TransferJobSettings:0:Interim:Options:Schema", MssqlSchemaName},
                { "TransferJobSettings:0:Interim:Options:Table", JobConstants.MsSqlToMsSql.InterimTableName},
                { "TransferJobSettings:0:Interim:Options:DataSourceName", $"{MssqlSchemaName}.{JobConstants.MsSqlToMsSql.SourceTableName}"},
                { "TransferJobSettings:0:Interim:Options:BatchQuantity", ContainerConstants.BatchQuantity.ToString()},
                { "TransferJobSettings:0:Target:Host", ContainerConstants.MsSql.Localhost},
                { "TransferJobSettings:0:Target:Type", ContainerConstants.MsSql.Type},
                { "TransferJobSettings:0:Target:Options:ConnectionString", MssqlConnectionString},
                { "TransferJobSettings:0:Target:Options::Schema", MssqlSchemaName},
                { "TransferJobSettings:0:Target:Options:Table",JobConstants.MsSqlToMsSql.TargetTableName},
                { "TransferJobSettings:0:Target:Options:IdColumn", JobConstants.MsSqlToMsSql.IdColumn.ToLowerInvariant()},
            };
            return configSettings;
        }

        private T CreateFixture<T>()
        {
            return _fixture.Create<T>();
        }
    }
}