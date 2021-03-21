using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using BusinessLogic.TargetData;

namespace DynamoDbAccess
{
    public class DynamoDbRepository : ITargetDataRepository
    {
        private readonly string accessKey;
        private readonly string secretKey;
        private readonly string regionName;
        private readonly string tableName;

        public DynamoDbRepository(string accessKey, string secretKey, string regionName, string tableName)
        {
            this.accessKey = accessKey;
            this.secretKey = secretKey;
            this.regionName = regionName;
            this.tableName = tableName;
        }

        public async Task Write(TargetEntity entity)
        {
            using var dynamoDbClient = new AmazonDynamoDBClient(new BasicAWSCredentials(accessKey, secretKey), RegionEndpoint.GetBySystemName(regionName));
            await dynamoDbClient.PutItemAsync(tableName, new Dictionary<string, AttributeValue>
            {
                {"Folder", new AttributeValue(entity.Folder)},
                {"Id", new AttributeValue(entity.Id)},
                {"Name", new AttributeValue(entity.Name)},
                {"PasswordHash", new AttributeValue(entity.PasswordHash)},
                {"Created", new AttributeValue(entity.Created)}
            });
        }
    }
}
