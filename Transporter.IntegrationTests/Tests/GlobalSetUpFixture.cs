using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Transporter.IntegrationTests.Containers;

namespace Transporter.IntegrationTests.Tests
{
    [SetUpFixture]
    public class GlobalSetUpFixture
    {
        private SqlServerContainer _sqlServerContainer;
        private CouchbaseContainer _couchbaseContainer;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _sqlServerContainer = new SqlServerContainer();
            var sqlServerTask = _sqlServerContainer.StartAsync();

            _couchbaseContainer = new CouchbaseContainer();
            var couchbaseTask = _couchbaseContainer.StartAsync();

            await Task.WhenAll(sqlServerTask, couchbaseTask);
            Thread.Sleep(10000);
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _sqlServerContainer.StopAndDisposeAsync();
            await _couchbaseContainer.DisposeAsync();
        }
    }
}