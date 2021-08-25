using Microsoft.Extensions.DependencyInjection;
using Transporter.Core.Adapters.Interim.Interfaces;
using Transporter.Core.Adapters.Source.Interfaces;
using Transporter.Core.Adapters.Target.Interfaces;
using Transporter.CouchbaseAdapter.Adapters;
using Transporter.CouchbaseAdapter.Data.Implementations;
using Transporter.CouchbaseAdapter.Data.Interfaces;
using Transporter.CouchbaseAdapter.Services.Interim.Implementations;
using Transporter.CouchbaseAdapter.Services.Interim.Interfaces;
using Transporter.CouchbaseAdapter.Services.Source.Implementations;
using Transporter.CouchbaseAdapter.Services.Source.Interfaces;
using Transporter.CouchbaseAdapter.Services.Target.Implementations;
using Transporter.CouchbaseAdapter.Services.Target.Interfaces;

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
            builder.AddTransient<IInterimService, InterimService>();
            builder.AddTransient<ISourceAdapter, CouchbaseSourceAdapter>();
            builder.AddTransient<ITargetAdapter, CouchbaseTargetAdapter>();
            builder.AddTransient<IInterimAdapter, CouchbaseInterimAdapter>();
        }
    }
}