using Microsoft.Extensions.DependencyInjection;
using Transporter.Core;
using Transporter.MSSQLAdapter.Data;
using Transporter.MSSQLAdapter.Services;

namespace Transporter.MSSQLAdapter
{
    public static class ServiceRegisterer
    {
        public static void OctopusTransporterMsSqlAdapterRegister(this IServiceCollection builder)
        {
            builder.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            builder.AddTransient<ISourceService, SourceService>();
            builder.AddTransient<ITargetService, TargetService>();
            builder.AddTransient<ISourceAdapter, MsSqlSourceAdapter>();
            builder.AddTransient<ITargetAdapter, MsSqlTargetAdapter>();
        }
    }
}