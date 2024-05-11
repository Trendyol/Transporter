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
using Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.IntegrationTests.Base;
using Transporter.IntegrationTests.Constants;
using Transporter.IntegrationTests.Helpers.Couchbase.Interfaces;
using Transporter.IntegrationTests.Objects;
using TransporterService;

namespace Transporter.IntegrationTests.Tests.Couchbase_Couchbase
{
    public class CouchbaseToCouchbaseIntegrationTests : IntegrationTestBase
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
            var id = CreateFixture<Guid>().ToString();
            var age = CreateFixture<int>();
            var collection =  await GetRequiredService<IBucketProvider>()
                .GetBucket(GetCouchbaseConnectionData(), JobConstants.CouchbaseToCouchbaseJob.SourceBucketName);
            var couchbaseCollection =    await collection.DefaultCollectionAsync();
            await GetRequiredService<ICouchbaseProviderHelper>()
                .CreatePrimaryIndexAsync(GetCouchbaseConnectionData(), $"`{JobConstants.CouchbaseToCouchbaseJob.SourceBucketName}`");
            await GetRequiredService<ICouchbaseProviderHelper>()
                .CreateBucketAsync(GetCouchbaseConnectionData(), JobConstants.CouchbaseToCouchbaseJob.InterimBucketName);
            await GetRequiredService<ICouchbaseProviderHelper>()
                .CreateBucketAsync(GetCouchbaseConnectionData(), JobConstants.CouchbaseToCouchbaseJob.TargetBucketName);
            await GetRequiredService<ICouchbaseProviderHelper>()
                .CreateIndexAsync(GetCouchbaseConnectionData(), $"`{JobConstants.CouchbaseToCouchbaseJob.InterimBucketName}`", JobConstants.CouchbaseToCouchbaseJob.InterimBucketIndexName, new List<string>
                {
                    "dataSourceName",
                    "lmd"
                });
            
            var user = _fixture.Build<UserClass>().With(x=> x.Age, age).Create();
            await couchbaseCollection.InsertAsync(id, user);
            
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(JobConstants.CouchbaseToCouchbaseJob.AppSettingsFileName)
                .Build();
            
            var build = Program.CreateHostBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddConfiguration(configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient(_ =>_mockDateTimeProvider.Object);
                }) 
                .Build();
            
            
            await build.StartAsync();
            Thread.Sleep(65000);
            await build.StopAsync();

            //Verify
            var byIdAsync = await GetRequiredService<ICouchbaseProviderHelper>()
                .GetByIdAsync<UserClass>(GetCouchbaseConnectionData(), JobConstants.CouchbaseToCouchbaseJob.TargetBucketName, id.ToString());
            byIdAsync.Age.Should().Be(user.Age);
        }

        private T CreateFixture<T>()
        {
            return _fixture.Create<T>();
        }
    }
}