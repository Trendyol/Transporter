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
            //Arrange & Act
            await using var connection = new SqlConnection(MssqlConnectionString);
            connection.Open();

            var id = CreateFixture<long>();
            var age = CreateFixture<int>();
            await connection.ExecuteAsync("Create Table dbo.TestTable_MsSql_MsSql (Id int, Age int)");
            var insertScript = $"INSERT INTO dbo.TestTable_MsSql_MsSql values({id},{age})";
            await connection.ExecuteAsync(insertScript);
            await connection.ExecuteAsync("Create Table dbo.TestTable_Interim_Mssql (dataSourceName nvarchar(max), lmd DateTime, Id int)");
            await connection.ExecuteAsync("Create Table dbo.TestTable_Target_Mssql_Mssql (Id nvarchar(max), Age int)");
            
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(JobConstants.MsSqlToMsSql.AppSettingsFileName)
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

        private T CreateFixture<T>()
        {
            return _fixture.Create<T>();
        }
    }
}