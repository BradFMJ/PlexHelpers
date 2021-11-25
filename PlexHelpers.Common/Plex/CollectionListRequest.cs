using System.Web;

namespace PlexHelpers.Common.Plex
{
    public class CollectionListRequest
    {
        public CollectionListRequest()
        {
            Language = "en";
        }

        public bool IncludeCollections { get; set; }
        public bool IncludeExternalMedia { get; set; }
        public string PlexToken { get; set; }
        public string Language { get; set; }

        public string ToUrlString()
        {
            return "includeCollections=" + (IncludeCollections ? "1" : "0")
               + "&includeExternalMedia=" + (IncludeExternalMedia ? "true" : "false")
               + "&X-Plex-Token=" + HttpUtility.UrlEncode(PlexToken)
               + "&X-Plex-Language=" + HttpUtility.UrlEncode(Language);
        }
    }
}
