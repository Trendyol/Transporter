using Microsoft.Extensions.DependencyInjection;
using Transporter.Core;
using Transporter.MSSQLDeleteAdapter.Data;
using Transporter.MSSQLDeleteAdapter.Services;

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