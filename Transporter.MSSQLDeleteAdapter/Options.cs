using Transporter.Core;

namespace Transporter.MSSQLDeleteAdapter
{
    public class MsSqlJobSettings : IJobSettings
    {
        public MsSqlJobSettings()
        {
            Source = new MsSqlSourceSettings();
        }

        public string Name { get; set; }
        public string Cron { get; set; }
        public ISourceOptions Source { get; set; }
        public ITargetOptions Target { get; set; }

        public override string ToString()
        {
            return
                $"Name : {Name}\tCron : {Cron}\tSource : {Source}";
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
    }

    public interface ISqlSourceSettings : ISourceOptions
    {
        public IMsSqlSourceOptions Options { get; set; }
    }

    public interface IMsSqlSourceOptions
    {
        public string Table { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public long BatchQuantity { get; set; }
        public string StatusIdColumnName { get; set; }
        public string IdColumn { get; set; }
        public string Condition { get; set; }
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
        public string Condition { get; set; }

        public override string ToString()
        {
            return
                $"Schema : {Schema} Table : {Table} ConnectionString : {ConnectionString} IdColumn : {IdColumn} BatchCount : {BatchQuantity} StatusIdColumnName : {StatusIdColumnName} Condition : {Condition}";
        }
    }
}