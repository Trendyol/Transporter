namespace Transporter.IntegrationTests.Constants;

public static class ContainerConstants
{
    public const int BatchQuantity = 10;
    public static class Couchbase
    {
        public const string UserName = "Administrator";
        public const string Password = "password";
        public const int UiPort = 8091;
        public const string Localhost = "127.0.0.1";
        public const string Type = "Couchbase";
    }

    public static class MsSql
    {
        public const string Localhost = "localhost";
        public const string Type = "mssql";
    }
}