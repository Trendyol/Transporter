using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoFixture;
using Dapper;
using FluentAssertions;
using NUnit.Framework;
using Transporter.IntegrationTests.Base;
using Transporter.IntegrationTests.Helpers.Couchbase.Interfaces;
using Transporter.IntegrationTests.Objects;
using TransporterService;

namespace Transporter.IntegrationTests.Tests.Couchbase_MSSQL
{
    public class CouchbaseMssqlIntegrationTests : IntegrationTestBase
    {
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public async Task Transporter_ShouldTransportData()
        {
            //Arrange
            await using var connection = new SqlConnection(MssqlConnectionString);
            connection.Open();

            var id = CreateFixture<int>();
            var age = CreateFixture<int>();
            var insertScript = $"INSERT INTO dbo.TestTable values({id}, {age})";

            await connection.ExecuteAsync("Create Table dbo.TestTable (Id int, Age int)");
            await connection.ExecuteAsync(insertScript);

            //Act
            var build = Program.CreateHostBuilder(Array.Empty<string>()).Build();
            await build.StartAsync();
            await build.StopAsync();

            //Verify
            var byIdAsync = await GetRequiredService<ICouchbaseProviderHelper>()
                .GetByIdAsync<UserClass>(GetCouchbaseConnectionData(), "test-source-bucket", id.ToString());
            byIdAsync.Id.Should().Be(id);
        }

        private T CreateFixture<T>()
        {
            return _fixture.Create<T>();
        }
    }
}