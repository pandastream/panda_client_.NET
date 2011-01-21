using System;

namespace Panda.Domain
{
    public class Cloud
    {
        public string id { get; set; }
        public string name { get; set; }
        public string s3_videos_bucket { get; set; }
        public bool s3_private_access { get; set; }
        public string url { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
