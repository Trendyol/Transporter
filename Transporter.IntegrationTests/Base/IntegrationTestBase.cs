using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Transporter.CouchbaseAdapter.Utils;
using Transporter.IntegrationTests.Helpers.Couchbase.Interfaces;

namespace Transporter.IntegrationTests.Base
{
    public abstract class IntegrationTestBase
    {
        private IServiceProvider _serviceProvider;
        private readonly StringWriter _stdOut = new StringWriter();

        public const string MssqlConnectionString =
            @"Server=localhost,1433;Database=master;User=sa;Password=_S1q2l3S4e5rver;";

        public const string MssqlSchemaName = "[dbo]";
        
        [OneTimeSetUp]
        public virtual async Task OneTimeSetUp()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseStartup<TestStartup>();
            var testServer = new TestServer(webHostBuilder);
            _serviceProvider = testServer.Host.Services;
            Console.SetOut(_stdOut);
        }
        
        protected ConnectionData GetCouchbaseConnectionData()
        {
            return new ConnectionData
            {
                Hosts = "127.0.0.1",
                Password = "password",
                Username = "Administrator",
                UiPort = 8091
            };
        }

        protected T GetRequiredService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        protected string GetLogs()
        {
            return _stdOut.ToString();
        }
    }
}