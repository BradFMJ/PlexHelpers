using System;

namespace PlexHelpers.Common.Medusa
{
    public class Episode
    {
        public int episode_id { get; set; }
        public int showid { get; set; }
        public int indexerid { get; set; }
        public int indexer { get; set; }
        public string name { get; set; }
        public int season { get; set; }
        public int episode { get; set; }
        public string description { get; set; }
        public int airdate { get; set; }
        public int hasnfo { get; set; }
        public int hastbn { get; set; }
        public int status { get; set; }
        public int quality { get; set; }
        public string location { get; set; }
        public long file_size { get; set; }
        public string release_name { get; set; }
        public string subtitles { get; set; }
        public int subtitles_searchcount { get; set; }
        public string subtitles_lastsearch { get; set; }
        public int is_proper { get; set; }
        public int? scene_season { get; set; }
        public int? scene_episode { get; set; }
        public int absolute_number { get; set; }
        public int? scene_absolute_number { get; set; }
        public int version { get; set; }
        public string release_group { get; set; }
        public int manually_searched { get; set; }
        public int watched { get; set; }

        public DateTime? SubTitlesLastSearched { get; set; }
    }
}
