using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Common.Dtos;
using Core.Services.UploadServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

public class UploadService : IUploadService
{
    private readonly AwsSettings _awsSettings;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<UploadService> _logger;

    public UploadService(
        IOptions<AwsSettings> awsOptions,
        ILogger<UploadService> logger)
    {
        _awsSettings = awsOptions.Value;
        _logger = logger;

        _s3Client = new AmazonS3Client(
           _awsSettings.AccessKey,
            _awsSettings.SecretKey,
            RegionEndpoint.GetBySystemName(_awsSettings.Region)
        );
    }

    public async Task<string> UploadBase64ImageAsync(string base64Image)
    {
        if (string.IsNullOrWhiteSpace(base64Image))
            throw new ValidationException("Image is required");

        try
        {
            // Remove base64 prefix if present
            var commaIndex = base64Image.IndexOf(",");
            if (commaIndex >= 0)
                base64Image = base64Image[(commaIndex + 1)..];

            var bytes = Convert.FromBase64String(base64Image);

            var fileName = $"products/{Guid.NewGuid()}.jpg";

            using var stream = new MemoryStream(bytes);

            var request = new PutObjectRequest
            {
                BucketName = _awsSettings.BucketName,
                Key = fileName,
                InputStream = stream,
                ContentType = "image/jpeg"
            };

            await _s3Client.PutObjectAsync(request);

            var url = $"https://{_awsSettings.BucketName}.s3.amazonaws.com/{fileName}";

            _logger.LogInformation("Image uploaded to S3: {Url}", url);

            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image to S3");
            throw new Exception("Image upload failed");
        }
    }
}