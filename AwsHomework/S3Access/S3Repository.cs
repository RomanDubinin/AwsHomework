using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using BusinessLogic.SourceData;
using CsvHelper;
using CsvHelper.Configuration;

namespace S3Access
{
    public class S3Repository : ISourceDataRepository
    {
        private readonly string accessKey;
        private readonly string secretKey;
        private readonly string regionName;
        private readonly string bucketName;

        public S3Repository(string accessKey, string secretKey, string regionName, string bucketName)
        {
            this.accessKey = accessKey;
            this.secretKey = secretKey;
            this.regionName = regionName;
            this.bucketName = bucketName;
        }

        public async Task<IEnumerable<SourceEntity>> ReadAllRecursively(string folderName)
        {
            using var client = new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), RegionEndpoint.GetBySystemName(regionName));
            var fileNames = await GetAllFileNamesRecursive(client, bucketName, folderName, ".csv");

            var result = new List<SourceEntity>();
            foreach (var fileName in fileNames)
            {
                var content = await GetFileContent(client, bucketName, fileName);
                var entities = ReadEntitiesFromCsv(content);
                result.AddRange(entities.Select(x => Convert(x, fileName)));
            }

            return result;
        }

        private async Task<string[]> GetAllFileNamesRecursive(IAmazonS3 client, string bucketName, string directory, string fileExtension)
        {
            var files = new List<string>();
            var listObjectsRequest = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = directory
            };

            ListObjectsV2Response listResponse;
            do
            {
                listResponse = await client.ListObjectsV2Async(listObjectsRequest);

                files.AddRange(listResponse.S3Objects.Where(x => x.Key.EndsWith(fileExtension)).Select(x => x.Key));

                listObjectsRequest.ContinuationToken = listResponse.NextContinuationToken;
            } while (listResponse.IsTruncated);

            return files.ToArray();
        }

        private async Task<string> GetFileContent(IAmazonS3 client, string bucketName, string fileName)
        {
            var objectRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileName
            };
            using var response = await client.GetObjectAsync(objectRequest);
            using var responseStream = response.ResponseStream;
            using var reader = new StreamReader(responseStream);
            var responseBody = await reader.ReadToEndAsync();

            return responseBody;
        }

        private CsvEntity[] ReadEntitiesFromCsv(string fileContent)
        {
            using var csv = new CsvReader(new StringReader(fileContent), new CsvConfiguration { CultureInfo = CultureInfo.InvariantCulture, HasHeaderRecord = false });
            return csv.GetRecords<CsvEntity>().ToArray();
        }

        private SourceEntity Convert(CsvEntity entity, string fileName)
        {
            return new SourceEntity
            {
                Id = entity.Id,
                Name = entity.Name,
                Password = entity.Password,
                Created = entity.Created,
                Folder = Path.GetDirectoryName(fileName)
            };
        }

    }
}
