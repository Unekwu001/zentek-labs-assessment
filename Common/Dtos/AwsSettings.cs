using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dtos
{
    public class AwsSettings
    {
        public string AccessKey { get; set; } 
        public string SecretKey { get; set; } 
        public string BucketName { get; set; }
        public string Region { get; set; }
    }
}
