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
using Transporter.IntegrationTests.Helpers.Couchbase.Interfaces;
using Transporter.IntegrationTests.Objects;
using TransporterService;

namespace Transporter.IntegrationTests.Tests.Couchbase_MSSQL
{
    public class MssqlToCouchbaseIntegrationTests : IntegrationTestBase
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

            var id = CreateFixture<int>();
            var age = CreateFixture<int>();
            await connection.ExecuteAsync("Create Table dbo.TestTable_MsSql (Id int, Age int)");
            var insertScript = $"INSERT INTO dbo.TestTable_MsSql values({id},{age})";
            await connection.ExecuteAsync(insertScript);
            await GetRequiredService<ICouchbaseProviderHelper>()
                .CreateBucketAsync(GetCouchbaseConnectionData(), JobConstants.MsSqlToCouchbase.InterimBucketName);
            await GetRequiredService<ICouchbaseProviderHelper>()
                .CreateBucketAsync(GetCouchbaseConnectionData(), JobConstants.MsSqlToCouchbase.TargetBucketName);
            await GetRequiredService<ICouchbaseProviderHelper>()
                .CreateIndexAsync(GetCouchbaseConnectionData(), $"`{JobConstants.MsSqlToCouchbase.InterimBucketName}`", JobConstants.MsSqlToCouchbase.InterimBucketIndexName, new List<string>
                {
                    "dataSourceName",
                    "lmd"
                });
            
            var configSettings = GetConfigSettings();
            
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configSettings)
                .Build();
           
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
            var byIdAsync = await GetRequiredService<ICouchbaseProviderHelper>()
                .GetByIdAsync<UserClass>(GetCouchbaseConnectionData(), JobConstants.MsSqlToCouchbase.TargetBucketName, id.ToString());
            byIdAsync.Age.Should().Be(age);
        }

        private Dictionary<string, string> GetConfigSettings()
        {
            var configSettings = new Dictionary<string, string>
            {
                { "PollingJobSettings:0:Name",CreateFixture<string>()},
                { "PollingJobSettings:0:Cron",JobConstants.MsSqlToCouchbase.PollingJobSettingsCron},
                { "PollingJobSettings:0:Source:Type",ContainerConstants.MsSql.Type},
                { "PollingJobSettings:0:Source:Host",ContainerConstants.MsSql.Localhost},
                { "PollingJobSettings:0:Source:Options:ConnectionString",MssqlConnectionString},
                { "PollingJobSettings:0:Source:Options:Schema",MssqlSchemaName},
                { "PollingJobSettings:0:Source:Options:Table",JobConstants.MsSqlToCouchbase.SourceTableName},
                { "PollingJobSettings:0:Source:Options:IdColumn",JobConstants.MsSqlToCouchbase.IdColumn},
                { "PollingJobSettings:0:Source:Options:Condition",string.Empty},
                { "PollingJobSettings:0:Source:Options:BatchQuantity",ContainerConstants.BatchQuantity.ToString()},
                { "PollingJobSettings:0:Target:Host",ContainerConstants.Couchbase.Localhost},
                { "PollingJobSettings:0:Target:Type",ContainerConstants.Couchbase.Type},
                { "PollingJobSettings:0:Target:Options:Bucket",JobConstants.MsSqlToCouchbase.InterimBucketName},
                { "PollingJobSettings:0:Target:Options:ConnectionData:Hosts",ContainerConstants.Couchbase.Localhost},
                { "PollingJobSettings:0:Target:Options:ConnectionData:Username",ContainerConstants.Couchbase.UserName},
                { "PollingJobSettings:0:Target:Options:ConnectionData:Password",ContainerConstants.Couchbase.Password},
                { "PollingJobSettings:0:Target:Options:ConnectionData:UiPort",ContainerConstants.Couchbase.UiPort.ToString()},
                { "TransferJobSettings:0:Name", CreateFixture<string>() },
                { "TransferJobSettings:0:Cron", JobConstants.MsSqlToCouchbase.TransferJobSettingsCron},
                { "TransferJobSettings:0:Source:Host", ContainerConstants.MsSql.Localhost},
                { "TransferJobSettings:0:Source:Type", ContainerConstants.MsSql.Type},
                { "TransferJobSettings:0:Source:Options:ConnectionString", MssqlConnectionString},
                { "TransferJobSettings:0:Source:Options:Schema", MssqlSchemaName},
                { "TransferJobSettings:0:Source:Options:Table", JobConstants.MsSqlToCouchbase.SourceTableName},
                { "TransferJobSettings:0:Source:Options:IdColumn", JobConstants.MsSqlToCouchbase.IdColumn},
                { "TransferJobSettings:0:Interim:Host", ContainerConstants.Couchbase.Localhost},
                { "TransferJobSettings:0:Interim:Type", ContainerConstants.Couchbase.Type},
                { "TransferJobSettings:0:Interim:Options:Bucket", JobConstants.MsSqlToCouchbase.InterimBucketName},
                { "TransferJobSettings:0:Interim:Options:ConnectionData:Hosts", ContainerConstants.Couchbase.Localhost},
                { "TransferJobSettings:0:Interim:Options:ConnectionData:Username", ContainerConstants.Couchbase.UserName},
                { "TransferJobSettings:0:Interim:Options:ConnectionData:Password", ContainerConstants.Couchbase.Password},
                { "TransferJobSettings:0:Interim:Options:ConnectionData:UiPort", ContainerConstants.Couchbase.UiPort.ToString()},
                { "TransferJobSettings:0:Interim:Options:BatchQuantity", ContainerConstants.BatchQuantity.ToString()},
                { "TransferJobSettings:0:Interim:Options:DataSourceName", $"{MssqlSchemaName}.{JobConstants.MsSqlToCouchbase.SourceTableName}"},
                { "TransferJobSettings:0:Target:Host", ContainerConstants.Couchbase.Localhost},
                { "TransferJobSettings:0:Target:Type", ContainerConstants.Couchbase.Type},
                { "TransferJobSettings:0:Target:Options:Bucket", JobConstants.MsSqlToCouchbase.TargetBucketName},
                { "TransferJobSettings:0:Target:Options:KeyProperty", JobConstants.MsSqlToCouchbase.IdColumn},
                { "TransferJobSettings:0:Target:Options:ConnectionData:Hosts", ContainerConstants.Couchbase.Localhost},
                { "TransferJobSettings:0:Target:Options:ConnectionData:Username", ContainerConstants.Couchbase.UserName},
                { "TransferJobSettings:0:Target:Options:ConnectionData:Password", ContainerConstants.Couchbase.Password},
                { "TransferJobSettings:0:Target:Options:ConnectionData:UiPort", ContainerConstants.Couchbase.UiPort.ToString()},
            };
            return configSettings;
        }

        private T CreateFixture<T>()
        {
            return _fixture.Create<T>();
        }
    }
}