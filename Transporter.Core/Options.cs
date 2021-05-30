namespace Octopus.Transporter.Core
{
    public interface IJobSettings
    {
        public string Name { get; set; }
        public string Cron { get; set; }
        public ISourceOptions Source { get; set; }
        public ITargetOptions Target { get; set; }
    }

    public class JobSettings : IJobSettings
    {
        public JobSettings()
        {
            Source = new SourceOptions();
            Target = new TargetOptions();
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

    public interface IOptions
    {
        public string Type { get; set; }
    }

    public interface ISourceOptions : IOptions
    {
    }

    public class SourceOptions : ISourceOptions
    {
        public string Type { get; set; }
    }

    public interface ITargetOptions : IOptions
    {
    }

    public class TargetOptions : ITargetOptions
    {
        public string Type { get; set; }
    }
}