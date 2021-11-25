using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PlexHelpers.Common.Plex
{
    [DataContract]
    public class CollectionResponse
    {
        public CollectionResponse()
        {
            MediaContainer = new CollectionResponseMediaContainer();
        }

        [DataMember(Name = "MediaContainer")]
        public CollectionResponseMediaContainer MediaContainer { get; set; }
    }

    [DataContract]
    public class CollectionResponseMediaContainer
    {
        public CollectionResponseMediaContainer()
        {
            Metadata = new List<CollectionResponseItem>();
        }

        [DataMember(Name = "size")]
        public long Size { get; set; }

        [DataMember(Name = "art")]
        public string Art { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "nocache")]
        public bool Nocache { get; set; }

        [DataMember(Name = "parentIndex")]
        public long ParentIndex { get; set; }

        [DataMember(Name = "parentTitle")]
        public string ParentTitle { get; set; }

        [DataMember(Name = "thumb")]
        public string Thumb { get; set; }

        [DataMember(Name = "title1")]
        public string Title1 { get; set; }

        [DataMember(Name = "Title2")]
        public string Title2 { get; set; }

        [DataMember(Name = "viewGroup")]
        public string ViewGroup { get; set; }

        [DataMember(Name = "viewMode")]
        public long ViewMode { get; set; }

        [DataMember(Name = "Metadata")]
        public List<CollectionResponseItem> Metadata { get; set; }
    }

    [DataContract]
    public class CollectionResponseItem
    {
        private string _guid;
        private Uri _url;

        public Uri Url
        {
            get
            {
                return _url;
            }
        }

        [DataMember(Name = "ratingKey")]
        public string RatingKey { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "guid")]
        public string Guid
        {
            get
            {
                return _guid;
            }
            set
            {
                _guid = value;
                Uri.TryCreate(_guid, UriKind.RelativeOrAbsolute, out _url);
            }
        }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "titleSort")]
        public string TitleSort { get; set; }

        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        [DataMember(Name = "year")]
        public int Year { get; set; }

        [DataMember(Name = "thumb")]
        public string Thumb { get; set; }

        [DataMember(Name = "art")]
        public string Art { get; set; }

        [DataMember(Name = "duration")]
        public long Duration { get; set; }

        [DataMember(Name = "addedAt")]
        public long AddedAt { get; set; }

        [DataMember(Name = "updatedAt")]
        public long UpdatedAt { get; set; }
    }
}
