using System.Collections.Generic;
using System.Web;

namespace PlexHelpers.Common.Plex
{
    public class CollectionAddRequest
    {
        public CollectionAddRequest()
        {
            Language = "en";
            Type = 1;
            IncludeExternalMedia = true;

        }

        public int Type { get; set; }
        public int MetadataId { get; set; }
        public bool IncludeExternalMedia { get; set; }
        public string PlexToken { get; set; }
        public string Language { get; set; }
        public List<string> Collections { get; set; }

        public string ToUrlString()
        {
            string result = "type=" + Type
                + "&id=" + MetadataId
               + "&includeExternalMedia=" + (IncludeExternalMedia ? "1" : "0");

            for (var i = 0; i < Collections.Count; i++)
            {
                result += "&" + HttpUtility.UrlEncode("collection[" + i + "].tag.tag") + "=" + HttpUtility.UrlEncode(Collections[i]);
            }

            result += "&X-Plex-Token=" + HttpUtility.UrlEncode(PlexToken)
               + "&X-Plex-Language=" + HttpUtility.UrlEncode(Language);

            return result;
        }
    }
}
