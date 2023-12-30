using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PlexHelpers.Common.Plex
{
    [DataContract]
    public class TVShowResponse
    {
        public TVShowResponse()
        {
            MediaContainer = new TVShowResponseMediaContainer();
        }

        [DataMember(Name = "MediaContainer")]
        public TVShowResponseMediaContainer MediaContainer { get; set; }
    }

    [DataContract]
    public class TVShowResponseMediaContainer
    {
        public TVShowResponseMediaContainer()
        {
            Metadata = new List<TVShowResponseItem>();
        }

        [DataMember(Name = "size")]
        public long Size { get; set; }

        [DataMember(Name = "allowSync")]
        public bool AllowSync { get; set; }

        [DataMember(Name = "identifier")]
        public string Identifier { get; set; }

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

        [DataMember(Name = "Metadata")]
        public List<TVShowResponseItem> Metadata { get; set; }
    }

    [DataContract]
    public class TVShowResponseItem
    {
        public TVShowResponseItem()
        {
            Collections = new List<TVShowResponseItemCollection>();
        }
 
        [DataMember(Name = "ratingKey")]
        public string RatingKey { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "studio")]
        public string Studio { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "librarySectionTitle")]
        public string LibrarySectionTitle { get; set; }

        [DataMember(Name = "librarySectionID")]
        public long LibrarySectionID { get; set; }

        [DataMember(Name = "librarySectionKey")]
        public string LibrarySectionKey { get; set; }

        [DataMember(Name = "contentRating")]
        public string ContentRating { get; set; }

        //[DataMember(Name = "titleSort")]
        //public string TitleSort { get; set; }

        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        //[DataMember(Name = "rating")]
        //public decimal Rating { get; set; }

        [DataMember(Name = "audienceRating")]
        public decimal AudienceRating { get; set; }

        [DataMember(Name = "year")]
        public int? Year { get; set; }

        [DataMember(Name = "tagline")]
        public string Tagline { get; set; }

        [DataMember(Name = "thumb")]
        public string Thumb { get; set; }

        [DataMember(Name = "art")]
        public string Art { get; set; }

        [DataMember(Name = "duration")]
        public long Duration { get; set; }

        [DataMember(Name = "originallyAvailableAt")]
        public string OriginallyAvailableAt { get; set; }

        [DataMember(Name = "addedAt")]
        public long AddedAt { get; set; }

        [DataMember(Name = "updatedAt")]
        public long UpdatedAt { get; set; }

        [DataMember(Name = "primaryExtraKey")]
        public string PrimaryExtraKey { get; set; }

        [DataMember(Name = "Collection")]
        public List<TVShowResponseItemCollection> Collections { get; set; }
    }

    [DataContract]
    public class TVShowResponseItemCollection
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "filter")]
        public string Filter { get; set; }

        [DataMember(Name = "tag")]
        public string Tag { get; set; }
    }
}
