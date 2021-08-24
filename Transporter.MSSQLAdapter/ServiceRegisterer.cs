using Microsoft.Extensions.DependencyInjection;
using Transporter.Core;
using Transporter.MSSQLAdapter.Adapters;
using Transporter.MSSQLAdapter.Data;
using Transporter.MSSQLAdapter.Services;
using Transporter.MSSQLAdapter.Services.Interim.Implementations;
using Transporter.MSSQLAdapter.Services.Interim.Interfaces;
using Transporter.MSSQLAdapter.Services.Source.Implementations;
using Transporter.MSSQLAdapter.Services.Source.Interfaces;
using Transporter.MSSQLAdapter.Services.Target.Implementations;
using Transporter.MSSQLAdapter.Services.Target.Interfaces;

namespace Transporter.MSSQLAdapter
{
    public static class ServiceRegisterer
    {
        public static void TransporterMsSqlAdapterRegister(this IServiceCollection builder)
        {
            builder.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            builder.AddTransient<ISourceService, SourceService>();
            builder.AddTransient<ITargetService, TargetService>();
            builder.AddTransient<IInterimService, InterimService>();
            builder.AddTransient<ISourceAdapter, MsSqlSourceAdapter>();
            builder.AddTransient<ITargetAdapter, MsSqlTargetAdapter>();
            builder.AddTransient<IInterimAdapter, MsSqlInterimAdapter>();
        }
    }
}