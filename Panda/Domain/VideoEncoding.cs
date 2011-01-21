using System;

namespace Panda.Domain
{
    public class VideoEncoding
    {
        public string id { get; set; }
        public string video_id { get; set; }
        public string profile_id { get; set; }
        public string status { get; set; }
        public string extname { get; set; }
        public string path { get; set; }
        public int encoding_progress { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public long file_size { get; set; }
        public DateTime started_encoding_at { get; set; }
        public int encoding_time { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
