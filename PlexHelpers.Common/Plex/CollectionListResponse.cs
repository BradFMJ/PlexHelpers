using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PlexHelpers.Common.Plex
{
    [DataContract]
    public class CollectionListResponse
    {
        public CollectionListResponse()
        {
            MediaContainer = new CollectionListResponseMediaContainer();
        }

        [DataMember(Name = "MediaContainer")]
        public CollectionListResponseMediaContainer MediaContainer { get; set; }
    }

    [DataContract]
    public class CollectionListResponseMediaContainer
    {
        public CollectionListResponseMediaContainer()
        {
            Metadata = new List<CollectionListResponseItem>();
        }

        [DataMember(Name = "size")]
        public long Size { get; set; }

        [DataMember(Name = "allowSync")]
        public bool AllowSync { get; set; }

        [DataMember(Name = "identifier")]
        public string Identifier { get; set; }

        [DataMember(Name = "art")]
        public string Art { get; set; }

        [DataMember(Name = "librarySectionID")]
        public long LibrarySectionID { get; set; }

        [DataMember(Name = "librarySectionTitle")]
        public string LibrarySectionTitle { get; set; }

        [DataMember(Name = "librarySectionUUID")]
        public string LibrarySectionUUID { get; set; }

        [DataMember(Name = "mediaTagPrefix")]
        public string MediaTagPrefix { get; set; }

        [DataMember(Name = "mediaTagVersion")]
        public long MediaTagVersion { get; set; }

        [DataMember(Name = "thumb")]
        public string Thumb { get; set; }

        [DataMember(Name = "title1")]
        public string Title1 { get; set; }

        [DataMember(Name = "title2")]
        public string Title2 { get; set; }

        [DataMember(Name = "viewGroup")]
        public string ViewGroup { get; set; }

        [DataMember(Name = "viewMode")]
        public long ViewMode { get; set; }

        [DataMember(Name = "Metadata")]
        public List<CollectionListResponseItem> Metadata { get; set; }
    }

    [DataContract]
    public class CollectionListResponseItem
    {
        [DataMember(Name = "ratingKey")]
        public string RatingKey { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "guid")]
        public string Guid { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "contentRating")]
        public string ContentRating { get; set; }

        [DataMember(Name = "subtype")]
        public string Subtype { get; set; }

        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        [DataMember(Name = "index")]
        public long index { get; set; }

        [DataMember(Name = "thumb")]
        public string Thumb { get; set; }

        [DataMember(Name = "addedAt")]
        public long addedAt { get; set; }

        [DataMember(Name = "updatedAt")]
        public long UpdatedAt { get; set; }

        [DataMember(Name = "childCount")]
        public string ChildCount { get; set; }

        [DataMember(Name = "maxYear")]
        public string MaxYear { get; set; }

        [DataMember(Name = "minYear")]
        public string MinYear { get; set; }
    }
}
