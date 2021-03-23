using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using BusinessLogic;
using DynamoDbAccess;
using Microsoft.Extensions.Configuration;
using S3Access;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AwsLambdaApi
{
    public class Function
    {
        public async Task FunctionHandler(string folderName, ILambdaContext context)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange:true)
                .Build();

            var accessKey = config["accessKey"];
            var secretKet = config["secretKey"];

            var s3RegionName = Environment.GetEnvironmentVariable("S3_region");
            var dynamoDbRegionName = Environment.GetEnvironmentVariable("DynamoDB_region");
            var bucketName = Environment.GetEnvironmentVariable("S3_bucket_name");
            var tableName = Environment.GetEnvironmentVariable("DynamoDB_table_name");

            var s3Repository = new S3Repository(accessKey, secretKet, s3RegionName, bucketName);
            var dynamoDbRepository = new DynamoDbRepository(accessKey, secretKet, dynamoDbRegionName, tableName);
            var entityProcessor = new EntityProcessor(s3Repository, dynamoDbRepository);

            await entityProcessor.ProcessFolder(folderName);
        }
    }
}
