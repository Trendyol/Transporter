using Microsoft.Extensions.DependencyInjection;
using Transporter.Core.Adapters.Source.Interfaces;
using Transporter.MSSQLDeleteAdapter.Adapters;
using Transporter.MSSQLDeleteAdapter.Data.Implementations;
using Transporter.MSSQLDeleteAdapter.Data.Interfaces;
using Transporter.MSSQLDeleteAdapter.Services.Implementations;
using Transporter.MSSQLDeleteAdapter.Services.Interfaces;

namespace Transporter.MSSQLDeleteAdapter
{
    public static class ServiceRegisterer
    {
        public static void TransporterMsSqlDeleteAdapterRegister(this IServiceCollection builder)
        {
            builder.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            builder.AddTransient<ISourceService, SourceService>();
            builder.AddTransient<ISourceAdapter, SourceAdapter>();
        }
    }
}