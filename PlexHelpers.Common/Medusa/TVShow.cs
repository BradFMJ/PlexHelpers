using System;
using System.Collections.Generic;
using System.Linq;

namespace PlexHelpers.Common.Medusa
{
    public class TVShow
    {
        public TVShow()
        {
            Episodes = new List<Episode>();
        }

        public int show_id { get; set; }
        public int indexer_id { get; set; }
        public int indexer { get; set; }
        public string show_name { get; set; }
        public string location { get; set; }
        public string network { get; set; }
        public string genre { get; set; }
        public string classification { get; set; }
        public int runtime { get; set; }
        public int quality { get; set; }
        public string airs { get; set; }
        public string status { get; set; }
        public int flatten_folders { get; set; }
        public int paused { get; set; }
        public int startyear { get; set; }
        public int air_by_date { get; set; }
        public string lang { get; set; }
        public int subtitles { get; set; }
        public string notify_list { get; set; }
        public string imdb_id { get; set; }
        public int last_update_indexer { get; set; }
        public int dvdorder { get; set; }
        public int? archive_firstmatch { get; set; }
        public string rls_require_words { get; set; }
        public string rls_ignore_words { get; set; }
        public int sports { get; set; }
        public int anime { get; set; }
        public int scene { get; set; }
        public int default_ep_status { get; set; }
        public string plot { get; set; }
        public int airdate_offset { get; set; }
        public int rls_require_exclude { get; set; }
        public int rls_ignore_exclude { get; set; }
        public string show_lists { get; set; }

        public List<Episode> Episodes { get; set; }

        public bool HasAllEpisodes {
            get
            {
                return Episodes.Count > 0 && Episodes.All(p => !string.IsNullOrWhiteSpace(p.location));

            }
        }

        public bool HasNoEpisodes
        {
            get
            {
                return Episodes.Count > 0 && Episodes.All(p => string.IsNullOrWhiteSpace(p.location));

            }
        }

        public bool HasAllSubtitles
        {
            get
            {
                return Episodes.All(p => p.subtitles.Contains("eng") || p.subtitles.Contains("und"));

            }
        }

        public bool AllSubtitlesRecentlySearched
        {
            get
            {
                DateTime now = DateTime.Now;
                return Episodes.All(p => p.SubTitlesLastSearched.HasValue && (now - p.SubTitlesLastSearched.Value).Days < 21);

            }
        }

        public long SeriesSizeMB
        {
            get
            {
                return Episodes.Sum(p=>p.file_size / 1024) / 1024;
            }
        }
    }
}
