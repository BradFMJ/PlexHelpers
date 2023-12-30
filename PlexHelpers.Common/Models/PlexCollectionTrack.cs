using System.Collections.Generic;

namespace PlexHelpers.Common.Models
{
    public class PlexCollectionTrack
    {
        public string CollectionName { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Track { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as PlexCollectionTrack);
        }

        protected bool Equals(PlexCollectionTrack other)
        {
            return string.Equals(CollectionName, other.CollectionName) && string.Equals(Artist, other.Artist) && Album == other.Album && string.Equals(Track, other.Track);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (CollectionName != null ? CollectionName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Artist != null ? Artist.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Album != null ? Album.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Track != null ? Track.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return Helpers.EscapeCsvField(CollectionName) + "," +
                   Helpers.EscapeCsvField(Artist) + "," + 
                   Helpers.EscapeCsvField(Album) + "," + 
                   Helpers.EscapeCsvField(Track);
        }

        public static PlexCollectionTrack Parse(string[] parts)
        {
            var plexCollectionTrack = new PlexCollectionTrack
            {
                CollectionName = parts[0],
                Artist = parts[1],
                Album = parts[2],
                Track = parts[3]
            };
            return plexCollectionTrack;
        }
    }

    public class PlexCollectionTrackComparer : IEqualityComparer<PlexCollectionTrack>
    {
        public bool Equals(PlexCollectionTrack x, PlexCollectionTrack y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(PlexCollectionTrack obj)
        {
            return obj.GetHashCode();
        }
    }
}
