# AWS S3 Assume Demo

This demo application shows how to access an S3 bucket using the AWS .NET SDK with temporary credentials by assuming a role with STS.

## Documentation

![Access](https://github.com/ericbach/S3AssumeDemo/blob/master/docs/Access.jpg)

https://github.com/ericbach/S3AssumeDemo/blob/master/docs/VPC%20endpoints.mp4

## Create the AWS resources

Set the AWS Account ID
https://github.com/ericbach/S3AssumeDemo/blob/master/src/S3AccessDemo/Program.cs#L72

`aws cloudformation create-stack --stack-name eric-bach-s3-assume-demo --template-body src/template.yml`

## Run the application

`dotnet run`
