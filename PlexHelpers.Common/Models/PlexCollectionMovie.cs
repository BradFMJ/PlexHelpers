using System.Collections.Generic;

namespace PlexHelpers.Common.Models
{
    public class PlexCollectionMovie
    {
        public string CollectionName { get; set; }
        public string MovieTitle { get; set; }
        public int? MovieYear { get; set; }
        public string IMDB { get; set; }
        public string TMDB { get; set; }
        public string CollectionKey { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as PlexCollectionMovie);
        }

        protected bool Equals(PlexCollectionMovie other)
        {
            return string.Equals(CollectionName, other.CollectionName) && string.Equals(MovieTitle, other.MovieTitle) && MovieYear == other.MovieYear && string.Equals(IMDB, other.IMDB) && string.Equals(TMDB, other.TMDB) && string.Equals(CollectionKey, other.CollectionKey);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (CollectionName != null ? CollectionName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MovieTitle != null ? MovieTitle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MovieYear.GetHashCode();
                hashCode = (hashCode * 397) ^ (IMDB != null ? IMDB.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TMDB != null ? TMDB.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CollectionKey != null ? CollectionKey.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return Helpers.EscapeCsvField(CollectionName) + "," +
                   Helpers.EscapeCsvField(MovieTitle) + "," + MovieYear + "," + IMDB + "," + TMDB + "," +
                   CollectionKey;
        }
    }

    public class PlexCollectionMovieComparer : IEqualityComparer<PlexCollectionMovie>
    {
        public bool Equals(PlexCollectionMovie x, PlexCollectionMovie y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(PlexCollectionMovie obj)
        {
            return obj.GetHashCode();
        }
    }
}
