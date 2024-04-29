namespace Transporter.IntegrationTests.Constants;

public static class JobConstants
{
    public static class CouchbaseToCouchbaseJob
    {
        public const string PollingJobSettingsCron = "*/10 * * * * ?";
        public const string TransferJobSettingsCron = "*/20 * * * * ?";
        public const string IdColumn = "Id";
        public const string SourceBucketName = "test-source-bucket";
        public const string InterimBucketName = "test-interim-bucket-couchbase-to-couchbase";
        public const string TargetBucketName = "test-target-bucket-couchbase-to-couchbase";
        public const string InterimBucketIndexName = "test_interim_index_couchbase_to_mssql";
    }
    
    public static class CouchbaseToMsSqlJob
    {
        public const string TransferJobSettingsCron = "*/20 * * * * ?";
        public const string PollingJobSettingsCron = "*/10 * * * * ?";
        public const string SourceBucketName = "test-source-bucket-couchbase-to-msql";
        public const string IdColumn = "Id";
        public const string InterimTableName = "[TestTable_Interim]";
        public const string TargetTableName = "[TestTable_Target]";
    }
    public static class MsSqlToCouchbase
    {
        public const string TransferJobSettingsCron = "*/20 * * * * ?";
        public const string PollingJobSettingsCron = "*/30 * * * * ?";
        public const string InterimBucketName = "test-interim-bucket-couchbase-to-mssql";
        public const string InterimBucketIndexName = "test_interim_bucket_couchbase_to_mssql_index";
        public const string TargetBucketName = "test-target-bucket-couchbase-to-mssql";
        public const string IdColumn = "Id";
        public const string SourceTableName = "[TestTable_MsSql]";
    }

    public static class MsSqlToMsSql
    {
        public const string TransferJobSettingsCron = "*/20 * * * * ?";
        public const string PollingJobSettingsCron = "*/10 * * * * ?";
        public const string IdColumn = "Id";
        public const string SourceTableName = "[TestTable_MsSql_MsSql]";
        public const string InterimTableName = "[TestTable_Interim_Mssql]";
        public const string TargetTableName = "[TestTable_Target_Mssql_Mssql]";
    }
}