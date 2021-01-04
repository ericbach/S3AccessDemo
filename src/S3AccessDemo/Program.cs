using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace S3AccessDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ListS3Buckets().Wait();
        }

        private static async Task ListS3Buckets()
        {
            try
            {
                // Retrieve a temporary credential from STS
                var tempCredentials = await GetTemporaryCredentialsAsync();

                // Assume role
                var assumedRole = await AssumeRole(tempCredentials);

                // List bucket contents
                var objects = await ListBucketAsync(assumedRole.Credentials);

                // Output bucket contents
                foreach (var o in objects)
                {
                    Console.WriteLine(o.Key);
                }
                Console.WriteLine("Object count = {0}", objects.Count);
            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message, s3Exception.InnerException);
            }
            catch (AmazonSecurityTokenServiceException stsException)
            {
                Console.WriteLine(stsException.Message, stsException.InnerException);
            }
        }

        private static async Task<List<S3Object>> ListBucketAsync(AWSCredentials credentials)
        {
            using (var s3Client = new AmazonS3Client(credentials, RegionEndpoint.USWest2))
            {
                var listObjectRequest = new ListObjectsRequest
                {
                    BucketName = "eric-bach-stuff"
                };

                var response = await s3Client.ListObjectsAsync(listObjectRequest);
                var objects = response.S3Objects;

                return objects;
            }
        }

        private static async Task<AssumeRoleResponse> AssumeRole(AWSCredentials tempCredentials)
        {
            using (var client = new AmazonSecurityTokenServiceClient(tempCredentials, RegionEndpoint.USWest2))
            {
                return await client.AssumeRoleAsync(new AssumeRoleRequest
                {
                    RoleArn = "arn:aws:iam::{ACCOUNT_ID}:role/eric-bach-demo-assume-role",
                    RoleSessionName = "eric-bach-demo-assume-role"
                });
            }
        }

        private static async Task<SessionAWSCredentials> GetTemporaryCredentialsAsync()
        {
            using (var stsClient = new AmazonSecurityTokenServiceClient())
            {
                var getSessionTokenRequest = new GetSessionTokenRequest
                {
                    DurationSeconds = 900
                };

                var sessionTokenResponse = await stsClient.GetSessionTokenAsync(getSessionTokenRequest);

                var credentials = sessionTokenResponse.Credentials;

                return new SessionAWSCredentials(credentials.AccessKeyId, credentials.SecretAccessKey,
                    credentials.SessionToken);
            }
        }
    }
}
