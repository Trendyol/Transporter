using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using Newtonsoft.Json.Linq;
using Transporter.Core.Models;
using Transporter.Core.Utils;
using Transporter.CouchbaseAdapter.Configs.Target.Interfaces;
using Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.Services.Target.Interfaces;
using Transporter.CouchbaseAdapter.Utils;

namespace Transporter.CouchbaseAdapter.Services.Target.Implementations
{
    public class TargetService : ITargetService
    {
        private readonly IBucketProvider _bucketProvider;
        private readonly IDateTimeProvider _dateTimeProvider;
        public TargetService(IBucketProvider bucketProvider, IDateTimeProvider dateTimeProvider)
        {
            _bucketProvider = bucketProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task SetTargetDataAsync(ICouchbaseTargetSettings settings, string data)
        {
            var insertDataItems = data.ToObject<List<dynamic>>();

            if (insertDataItems is null || !insertDataItems.Any())
            {
                return;
            }

            try
            {
                var tasks = await InsertItems(settings, insertDataItems);
                await Task.WhenAll(tasks);
            }
            catch (DocumentExistsException)
            {
            }
        }

        public async Task SetInterimDataAsync(ICouchbaseTargetSettings settings, string data, string dataSourceName)
        {
            var dataItemIds = data.ToObject<List<IdObject>>().ToList();
            var insertDataItems = dataItemIds.Select(SelectTemporaryTableDataFromId(dataSourceName)).ToList();

            if (!insertDataItems.Any())
            {
                return;
            }

            try
            {
                var tasks = await InsertItems(settings, insertDataItems, dataSourceName);
                await Task.WhenAll(tasks);
            }
            catch (DocumentExistsException)
            {
            }
        }

        private  Func<IdObject, InterimTable> SelectTemporaryTableDataFromId(string dataSourceName)
        {
            return dataItemId => CreateTemporaryTableData(dataSourceName, dataItemId);
        }

        private  InterimTable CreateTemporaryTableData(string dataSourceName, IdObject dataItemId)
        {
            return new InterimTable
            {
                Id = dataItemId.Id,
                Lmd = _dateTimeProvider.Now,
                DataSourceName = dataSourceName
            };
        }

        private async Task<List<Task<IMutationResult>>> InsertItems(ICouchbaseTargetSettings settings,
            List<InterimTable> insertDataItems, string dataSourceName)
        {
            var collection = await GetCollectionAsync(settings.Options.ConnectionData, settings.Options.Bucket);
            var tasks = new List<Task<IMutationResult>>();

            foreach (var interimData in insertDataItems)
            {
                var task = collection.InsertAsync($"{interimData.Id}_{dataSourceName}", interimData);
                tasks.Add(task);
            }

            return tasks;
        }

        private async Task<List<Task<IMutationResult>>> InsertItems(ICouchbaseTargetSettings settings,
            List<dynamic> insertDataItems)
        {
            var collection = await GetCollectionAsync(settings.Options.ConnectionData, settings.Options.Bucket);
            var tasks = new List<Task<IMutationResult>>();
            var excludedProperties = GetExcludedPropertiesFromSettings(settings).ToList();
            var keyProperty = settings.Options.KeyProperty;

            for (var i = 0; i < insertDataItems.Count; i++)
            {
                var data = insertDataItems[i];
                var id = ((JObject)data)[keyProperty].ToString();
                ((JObject)data).Remove(keyProperty);

                foreach (var excludedProperty in excludedProperties)
                {
                    if (((JObject)data).ContainsKey(excludedProperty))
                    {
                        ((JObject)data).Remove(excludedProperty);
                    }
                }

                var task = collection.InsertAsync(id, data);
                tasks.Add(task);
            }

            return tasks;
        }

        private static IEnumerable<string> GetExcludedPropertiesFromSettings(ICouchbaseTargetSettings settings)
        {
            return settings.Options.ExcludedProperties?.Split(",") ?? Array.Empty<string>();
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