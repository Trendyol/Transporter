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
            //Arrange
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
            
            var configSettings = GetConfigSettings();
            
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configSettings)
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
            byIdAsync.Age.Should().Be(age);
        }

        private Dictionary<string, string> GetConfigSettings()
        {
            var configSettings = new Dictionary<string, string>
            {
                { "PollingJobSettings:0:Name",CreateFixture<string>()},
                { "PollingJobSettings:0:Cron",JobConstants.CouchbaseToCouchbaseJob.PollingJobSettingsCron},
                { "PollingJobSettings:0:Source:Type",ContainerConstants.Couchbase.Type},
                { "PollingJobSettings:0:Source:Host",ContainerConstants.Couchbase.Localhost},
                { "PollingJobSettings:0:Source:Options:Bucket",JobConstants.CouchbaseToCouchbaseJob.SourceBucketName},
                { "PollingJobSettings:0:Source:Options:ConnectionData:Hosts",ContainerConstants.Couchbase.Localhost},
                { "PollingJobSettings:0:Source:Options:ConnectionData:Username",ContainerConstants.Couchbase.UserName},
                { "PollingJobSettings:0:Source:Options:ConnectionData:Password",ContainerConstants.Couchbase.Password},
                { "PollingJobSettings:0:Source:Options:ConnectionData:UiPort",ContainerConstants.Couchbase.UiPort.ToString()},
                { "PollingJobSettings:0:Source:Options:Condition",string.Empty},
                { "PollingJobSettings:0:Source:Options:BatchQuantity",ContainerConstants.BatchQuantity.ToString()},
                { "PollingJobSettings:0:Target:Host",ContainerConstants.Couchbase.Localhost},
                { "PollingJobSettings:0:Target:Type",ContainerConstants.Couchbase.Type},
                { "PollingJobSettings:0:Target:Options:Bucket",JobConstants.CouchbaseToCouchbaseJob.InterimBucketName},
                { "PollingJobSettings:0:Target:Options:ConnectionData:Hosts",ContainerConstants.Couchbase.Localhost},
                { "PollingJobSettings:0:Target:Options:ConnectionData:Username",ContainerConstants.Couchbase.UserName},
                { "PollingJobSettings:0:Target:Options:ConnectionData:Password",ContainerConstants.Couchbase.Password},
                { "PollingJobSettings:0:Target:Options:ConnectionData:UiPort",ContainerConstants.Couchbase.UiPort.ToString()},
                { "TransferJobSettings:0:Name", CreateFixture<string>() },
                { "TransferJobSettings:0:Cron", JobConstants.CouchbaseToCouchbaseJob.TransferJobSettingsCron},
                { "TransferJobSettings:0:Source:Host", ContainerConstants.Couchbase.Localhost},
                { "TransferJobSettings:0:Source:Type", ContainerConstants.Couchbase.Type},
                { "TransferJobSettings:0:Source:Options:IdColumn", JobConstants.CouchbaseToCouchbaseJob.IdColumn},
                { "TransferJobSettings:0:Source:Options:ConnectionData:Hosts", ContainerConstants.Couchbase.Localhost},
                { "TransferJobSettings:0:Source:Options:ConnectionData:Username", ContainerConstants.Couchbase.UserName},
                { "TransferJobSettings:0:Source:Options:ConnectionData:Password", ContainerConstants.Couchbase.Password},
                { "TransferJobSettings:0:Source:Options:ConnectionData:UiPort", ContainerConstants.Couchbase.UiPort.ToString()},
                { "TransferJobSettings:0:Source:Options:Bucket",JobConstants.CouchbaseToCouchbaseJob.SourceBucketName},
                { "TransferJobSettings:0:Interim:Host", ContainerConstants.Couchbase.Localhost},
                { "TransferJobSettings:0:Interim:Type", ContainerConstants.Couchbase.Type},
                { "TransferJobSettings:0:Interim:Options:Bucket", JobConstants.CouchbaseToCouchbaseJob.InterimBucketName},
                { "TransferJobSettings:0:Interim:Options:ConnectionData:Hosts", ContainerConstants.Couchbase.Localhost},
                { "TransferJobSettings:0:Interim:Options:ConnectionData:Username", ContainerConstants.Couchbase.UserName},
                { "TransferJobSettings:0:Interim:Options:ConnectionData:Password", ContainerConstants.Couchbase.Password},
                { "TransferJobSettings:0:Interim:Options:ConnectionData:UiPort", ContainerConstants.Couchbase.UiPort.ToString()},
                { "TransferJobSettings:0:Interim:Options:BatchQuantity",ContainerConstants.BatchQuantity.ToString()},
                { "TransferJobSettings:0:Interim:Options:DataSourceName", JobConstants.CouchbaseToCouchbaseJob.SourceBucketName},
                { "TransferJobSettings:0:Target:Host", ContainerConstants.Couchbase.Localhost},
                { "TransferJobSettings:0:Target:Type", ContainerConstants.Couchbase.Type},
                { "TransferJobSettings:0:Target:Options:Bucket", JobConstants.CouchbaseToCouchbaseJob.TargetBucketName},
                { "TransferJobSettings:0:Target:Options:KeyProperty", JobConstants.CouchbaseToCouchbaseJob.IdColumn.ToLowerInvariant()},
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