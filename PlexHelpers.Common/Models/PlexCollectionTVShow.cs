using System.Collections.Generic;

namespace PlexHelpers.Common.Models
{
    public class PlexCollectionTVShow
    {
        public PlexCollectionTVShow()
        {
            TheTVDB = string.Empty;
            TMDB = string.Empty;
            Plex = string.Empty;
            None = string.Empty;
            Local = string.Empty;
        }
        public string CollectionName { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public string CollectionKey { get; set; }

        public string TheTVDB { get; set; }
        public string TMDB { get; set; }
        public string Plex { get; set; }
        public string None { get; set; }
        public string Local { get; set; }

        public static PlexCollectionTVShow Parse(string[] parts)
        {
            var plexCollectionTVShow = new PlexCollectionTVShow
            {
                CollectionName = parts[0],
                Title = parts[1],
                CollectionKey = parts.Length > 3 ? parts[3] : null,
                TheTVDB = parts.Length > 4 ? parts[4] : null,
                TMDB = parts.Length > 5 ? parts[5] : null,
                Plex = parts.Length > 6 ? parts[6] : null,
                None = parts.Length > 7 ? parts[7] : null,
                Local = parts.Length > 8 ? parts[8] : null,
            };

            int parseInt;
            if (parts.Length > 2 && int.TryParse(parts[2], out parseInt))
            {
                plexCollectionTVShow.Year = parseInt;
            }


            return plexCollectionTVShow;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PlexCollectionTVShow);
        }

        protected bool Equals(PlexCollectionTVShow other)
        {
            return string.Equals(CollectionName, other.CollectionName) 
                   && string.Equals(Title, other.Title) 
                   && Year == other.Year 
                   && string.Equals(TheTVDB, other.TheTVDB) 
                   && string.Equals(TMDB, other.TMDB)
                   && string.Equals(Plex, other.Plex)
                   && string.Equals(None, other.None)
                   && string.Equals(Local, other.Local)
                   && string.Equals(CollectionKey, other.CollectionKey);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (CollectionName != null ? CollectionName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Year.GetHashCode();
                hashCode = (hashCode * 397) ^ (TheTVDB != null ? TheTVDB.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TMDB != null ? TMDB.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Plex != null ? Plex.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (None != null ? None.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Local != null ? Local.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CollectionKey != null ? CollectionKey.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return Helpers.EscapeCsvField(CollectionName)
                   + "," + Helpers.EscapeCsvField(Title) 
                   + "," + Year
                   + "," + CollectionKey
                   + "," + TheTVDB
                   + "," + TMDB
                   + "," + Plex
                   + "," + None
                   + "," + Local;
        }
    }

    public class PlexCollectionTVShowComparer : IEqualityComparer<PlexCollectionTVShow>
    {
        public bool Equals(PlexCollectionTVShow x, PlexCollectionTVShow y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(PlexCollectionTVShow obj)
        {
            return obj.GetHashCode();
        }
    }
}
