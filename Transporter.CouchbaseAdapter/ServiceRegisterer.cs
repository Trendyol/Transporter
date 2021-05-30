using Microsoft.Extensions.DependencyInjection;
using Transporter.Core;
using Transporter.CouchbaseAdapter.Adapters;
using Transporter.CouchbaseAdapter.Data.Implementations;
using Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.Services.Implementations;
using Transporter.CouchbaseAdapter.Services.Interfaces;

namespace Transporter.CouchbaseAdapter
{
    public static class ServiceRegisterer
    {
        public static void TransporterCouchbaseAdapterRegister(this IServiceCollection builder)
        {
            builder.AddSingleton<ICouchbaseProvider, CouchbaseProvider>();
            builder.AddTransient<IBucketProvider, BucketProvider>();
            builder.AddTransient<ISourceService, SourceService>();
            builder.AddTransient<ITargetService, TargetService>();
            builder.AddTransient<ISourceAdapter, CouchbaseSourceAdapter>();
            builder.AddTransient<ITargetAdapter, CouchbaseTargetAdapter>();
        }
    }
}