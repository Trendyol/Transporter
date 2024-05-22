using Microsoft.Extensions.DependencyInjection;
using Transporter.Core.Adapters.Interim.Interfaces;
using Transporter.Core.Adapters.Source.Interfaces;
using Transporter.Core.Adapters.Target.Interfaces;
using Transporter.PostgreSqlAdapter.Adapters;
using Transporter.PostgreSQLAdapter.Adapters;
using Transporter.PostgreSqlAdapter.Data.Implementations;
using Transporter.PostgreSQLAdapter.Data.Interfaces;
using Transporter.PostgreSQLAdapter.Services.Interim.Implementations;
using Transporter.PostgreSQLAdapter.Services.Interim.Interfaces;
using Transporter.PostgreSQLAdapter.Services.Source.Implementations;
using Transporter.PostgreSQLAdapter.Services.Source.Interfaces;
using Transporter.PostgreSQLAdapter.Services.Target.Implementations;
using Transporter.PostgreSQLAdapter.Services.Target.Interfaces;

namespace Transporter.PostgreSqlAdapter
{
    public static class ServiceRegisterer
    {
        public static void TransporterPostgreSqlAdapterRegister(this IServiceCollection builder)
        {
            builder.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            builder.AddTransient<ISourceService, SourceService>();
            builder.AddTransient<ITargetService, TargetService>();
            builder.AddTransient<IInterimService, InterimService>();
            builder.AddTransient<ISourceAdapter, PostgreSqlSourceAdapter>();
            builder.AddTransient<ITargetAdapter, PostgreSqlTargetAdapter>();
            builder.AddTransient<IInterimAdapter, PostgreSqlInterimAdapter>();
        }
    }
}