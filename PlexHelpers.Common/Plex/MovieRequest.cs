using System.Web;

namespace PlexHelpers.Common.Plex
{
    public class MovieRequest
    {
        public MovieRequest()
        {
            Language = "en";
            IncludeExtras = true;
            IncludePreferences = true;
            IncludeExternalMedia = true;
        }

        public bool IncludeExternalMedia { get; set; }
        public bool IncludeExtras { get; set; }
        public bool IncludePreferences { get; set; }
        public string PlexToken { get; set; }
        public string Language { get; set; }

        public string ToUrlString()
        {
            return "includeExternalMedia=" + (IncludeExternalMedia ? "1" : "0")
               + "&includeExtras=" + (IncludeExtras ? "1" : "0")
               + "&includePreferences=" + (IncludePreferences ? "1" : "0")
               + "&X-Plex-Token=" + HttpUtility.UrlEncode(PlexToken)
               + "&X-Plex-Language=" + HttpUtility.UrlEncode(Language);
        }
    }
}
