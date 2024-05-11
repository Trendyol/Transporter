namespace Transporter.IntegrationTests.Constants;

public static class JobConstants
{
    public static class CouchbaseToCouchbaseJob
    {
        public const string SourceBucketName = "test-source-bucket";
        public const string InterimBucketName = "test-interim-bucket-couchbase-to-couchbase";
        public const string TargetBucketName = "test-target-bucket-couchbase-to-couchbase";
        public const string InterimBucketIndexName = "test_interim_index_couchbase_to_couchbase";
        public const string AppSettingsFileName = "appsettings_couchbaseToCouchbase.json";
    }
    
    public static class CouchbaseToMsSqlJob
    {
        public const string SourceBucketName = "test-source-bucket-couchbase-to-msql";
        public const string AppSettingsFileName = "appsettings_couchbaseToMsSql.json";
    }
    public static class MsSqlToCouchbase
    {
        public const string InterimBucketName = "test-interim-bucket-couchbase-to-mssql";
        public const string InterimBucketIndexName = "test_interim_bucket_couchbase_to_mssql_index";
        public const string TargetBucketName = "test-target-bucket-couchbase-to-mssql";
        public const string AppSettingsFileName = "appsettings_MsSqlToCouchbase.json";
    }

    public static class MsSqlToMsSql
    {
        public const string AppSettingsFileName = "appsettings_MsSqlToMsSql.json";
    }
}