using System.Web;

namespace PlexHelpers.Common.Plex
{
    public class CollectionRequest
    {
        public CollectionRequest()
        {
            Language = "en";
        }

        public bool ExcludeAllLeaves { get; set; }
        public string PlexToken { get; set; }
        public string Language { get; set; }

        public string ToUrlString()
        {
            return "excludeAllLeaves=" + (ExcludeAllLeaves ? "1" : "0")
               + "&X-Plex-Token=" + HttpUtility.UrlEncode(PlexToken)
               + "&X-Plex-Language=" + HttpUtility.UrlEncode(Language);
        }
    }
}
