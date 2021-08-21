using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using Transporter.Core;
using Transporter.CouchbaseAdapter.ConfigOptions.Target.Interfaces;
using Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.Services.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Services.Implementations
{
    public class TargetService : ITargetService
    {
        private readonly IBucketProvider _bucketProvider;

        public TargetService(IBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task SetTargetDataAsync(ICouchbaseTargetSettings settings, string data)
        {
            var insertDataItems = data.ToObject<List<dynamic>>();
            var dataItemIds = data.ToObject<List<IdObject>>();

            if (insertDataItems is null || !insertDataItems.Any())
            {
                return;
            }

            try
            {
                var tasks = await InsertItems(settings, insertDataItems, dataItemIds);
                await Task.WhenAll(tasks);
            }
            catch (DocumentExistsException)
            {
            }
        }

        public async Task SetTargetTemporaryDataAsync(ICouchbaseTargetSettings settings, string data,
            string dataSourceName)
        {
            var dataItemIds = data.ToObject<List<IdObject>>();
            var insertDataItems = dataItemIds.Select(SelectTemporaryTableDataFromId(dataSourceName)).ToList();

            if (!insertDataItems.Any())
            {
                return;
            }

            try
            {
                var tasks = await InsertItems(settings, insertDataItems, dataItemIds, dataSourceName);
                await Task.WhenAll(tasks);
            }
            catch (DocumentExistsException)
            {
            }
        }

        private static Func<IdObject, InterimTable> SelectTemporaryTableDataFromId(string dataSourceName)
        {
            return dataItemId => CreateTemporaryTableData(dataSourceName, dataItemId);
        }

        private static InterimTable CreateTemporaryTableData(string dataSourceName, IdObject dataItemId)
        {
            return new InterimTable
            {
                Id = dataItemId.Id,
                Lmd = DateTime.Now,
                DataSourceName = dataSourceName
            };
        }

        private async Task<List<Task<IMutationResult>>> InsertItems(ICouchbaseTargetSettings settings,
            List<InterimTable> insertDataItems, List<IdObject> dataItemIds, string dataSourceName)
        {
            var collection = await GetCollectionAsync(settings.Options.ConnectionData, settings.Options.Bucket);
            var tasks = new List<Task<IMutationResult>>();

            for (var i = 0; i < insertDataItems.Count; i++)
            {
                var task = collection.InsertAsync($"{dataItemIds[i].Id}_{dataSourceName}", insertDataItems[i]);
                tasks.Add(task);
            }

            return tasks;
        }

        private async Task<List<Task<IMutationResult>>> InsertItems(ICouchbaseTargetSettings settings,
            List<dynamic> insertDataItems, List<IdObject> dataItemIds)
        {
            var collection = await GetCollectionAsync(settings.Options.ConnectionData, settings.Options.Bucket);
            var tasks = new List<Task<IMutationResult>>();

            for (var i = 0; i < insertDataItems.Count; i++)
            {
                var task = collection.InsertAsync(dataItemIds[i].Id, insertDataItems[i]);
                tasks.Add(task);
            }

            return tasks;
        }

        private async Task<ICouchbaseCollection> GetCollectionAsync(ConnectionData connectionData, string bucket)
        {
            return await (await GetBucketAsync(connectionData, bucket)).DefaultCollectionAsync();
        }

        private Task<IBucket> GetBucketAsync(ConnectionData connectionData, string bucket)
        {
            return _bucketProvider.GetBucket(connectionData, bucket);
        }
    }
}