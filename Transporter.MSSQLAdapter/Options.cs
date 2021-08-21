using Transporter.Core;

namespace Transporter.MSSQLAdapter
{
    public class MsSqlJobSettings : IJobSettings
    {
        public MsSqlJobSettings()
        {
            Source = new MsSqlSourceSettings();
            Target = new MsSqlTargetSettings();
        }

        public string Name { get; set; }
        public string Cron { get; set; }
        public ISourceOptions Source { get; set; }
        public ITargetOptions Target { get; set; }

        public override string ToString()
        {
            return
                $"Name : {Name}\tCron : {Cron}\tSource : {Source}\tTarget : {Target}";
        }
    }

    public class MsSqlSourceSettings : ISqlSourceSettings
    {
        public MsSqlSourceSettings()
        {
            Options = new MsSqlSourceOptions();
        }

        public string Type { get; set; }
        public IMsSqlSourceOptions Options { get; set; }

        public override string ToString()
        {
            return
                $"Type : {Type} Options : {Options}";
        }

        public string Host { get; set; }
        public bool IsInsertableOnFailure { get; set; }
    }

    public class MsSqlTargetSettings : ISqlTargetSettings
    {
        public MsSqlTargetSettings()
        {
            Options = new MsSqlTargetOptions();
        }

        public string Type { get; set; }
        public IMsSqlTargetOptions Options { get; set; }

        public override string ToString()
        {
            return
                $"Type : {Type} Options : {Options}";
        }

        public string Host { get; set; }
    }

    public interface ISqlSourceSettings : ISourceOptions
    {
        public IMsSqlSourceOptions Options { get; set; }
    }

    public interface ISqlTargetSettings : ITargetOptions
    {
        public IMsSqlTargetOptions Options { get; set; }
    }

    public interface IMsSqlSourceOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public long BatchQuantity { get; set; }
        public string IdColumn { get; set; }
        public bool IsIdAutoIncrementOn { get; set; }
        public string Condition { get; set; }
    }

    public interface IMsSqlTargetOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public string ExcludedColumns { get; set; }
    }

    public class MsSqlTargetOptions : IMsSqlTargetOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public string ExcludedColumns { get; set; }
    }

    public class MsSqlSourceOptions : IMsSqlSourceOptions
    {
        private long _batchQuantity;
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }

        public long BatchQuantity
        {
            get => _batchQuantity;
            set => _batchQuantity = value > 1000 ? 1000 : value;
        }

        public string StatusIdColumnName { get; set; }
        public string IdColumn { get; set; }
        public bool IsIdAutoIncrementOn { get; set; }
        public string Condition { get; set; }
        public override string ToString()
        {
            return
                $"Schema : {Schema} Table : {Table} ConnectionString : {ConnectionString} IdColumn : {IdColumn} BatchCount : {BatchQuantity} StatusIdColumnName : {StatusIdColumnName} Condition : {Condition}";
        }
    }
}