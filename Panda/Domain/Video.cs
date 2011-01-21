using System;

namespace Panda.Domain
{
    public class Video
    {
        public string id { get; set; }
        public string source_url { get; set; }
        public string status { get; set; }
        public string original_filename { get; set; }
        public string extname { get; set; }
        public string video_codec { get; set; }
        public string audio_codec { get; set; }
        public string path { get; set; }
        public int? thumbnail_position { get; set; }
        public int? height { get; set; }
        public int? width { get; set; }
        public int? fps { get; set; }
        public int? duration { get; set; }
        public long? file_size { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
