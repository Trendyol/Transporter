namespace Transporter.Core
{
    public class TemporaryTableOptions
    {
        public interface ITemporaryTableJobSettings
        {
            public string Name { get; set; }
            public string Cron { get; set; }
            public ITemporaryTableSourceOptions Source { get; set; }
            public ITemporaryTableTargetOptions Target { get; set; }
        }

        public class TemporaryTableJobSettings : ITemporaryTableJobSettings
        {
            public TemporaryTableJobSettings()
            {
                Source = new TemporaryTableSourceOptions();
                Target = new TemporaryTableTargetOptions();
            }

            public string Name { get; set; }
            public string Cron { get; set; }
            public ITemporaryTableSourceOptions Source { get; set; }
            public ITemporaryTableTargetOptions Target { get; set; }

            public override string ToString()
            {
                return
                    $"Name : {Name}\tCron : {Cron}\tSource : {Source}\tTarget : {Target}";
            }
        }

        public interface ITemporaryTableOptions
        {
            public string Type { get; set; }
        }

        public interface ITemporaryTableSourceOptions : ITemporaryTableOptions
        {
            public string Host { get; set; }
        }

        public class TemporaryTableSourceOptions : ITemporaryTableSourceOptions
        {
            public string Type { get; set; }
            public string Host { get; set; }
        }

        public interface ITemporaryTableTargetOptions : ITemporaryTableOptions
        {
            public string Host { get; set; }
        }

        public class TemporaryTableTargetOptions : ITemporaryTableTargetOptions
        {
            public string Type { get; set; }
            public string Host { get; set; }
        }
    }
}