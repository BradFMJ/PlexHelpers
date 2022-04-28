using System;

namespace PlexHelpers.Common.Models
{
    public class PlexMetadDataItem
    {
        public int Id; //0
        public int LibrarySectionId; //1
        public int ParentId; //2
        public int MetadataType; //3
        public string Guid; //4
        public int MediaItemCount; //5
        public string Title; //6
        public string TitleSort; //7
        public string OriginalTitle; //8
        public string Studio; //9
        public decimal Rating; //10
        public int RatingCount; //11
        public string Tagline; //12
        public string Summary; //13
        public string Trivia; //14
        public string Quotes; //15
        public string ContentRating; //16
        public int ContentRatingAge; //17
        public int Index; //18
        public int AbsoluteIndex; //19
        public int Duration; //20
        public string UserThumbUrl; //21
        public string UserArtUrl; //22
        public string UserBannerUrl; //23
        public string UserMusicUrl; //24
        public string UserFields; //25
        public string TagsGenre; //26
        public string TagsCollection; //27
        public string TagsDirector; //28
        public string TagsWriter; //29
        public string TagsStar; //30
        public DateTime OriginallyAvailableAt; //31
        public DateTime AvailableAt; //32
        public DateTime ExpiresAt; //33
        public DateTime RefreshedAt; //34
        public int Year; //35
        public DateTime AddedAt; //36
        public DateTime CreatedAt; //37
        public DateTime UpdatedAt; //38
        public DateTime DeletedAt; //39
        public string TagsCountry; //40
        public string ExtraData; //41
        public string Hash; //42
        public decimal AudienceRating; //43
        public int ChangedAt; //44
        public int ResourcesChangedAt; //45
        public int Remote; //46

        public string IMDB; //Not mapped to sql query

        public static PlexMetadDataItem Parse(string[] parts)
        {
            var plexCollectionTVShow = new PlexMetadDataItem
            {
                Guid = parts.Length > 4 ? parts[4] : null,
                Title = parts.Length > 6 ? parts[6] : null,
                TitleSort = parts.Length > 7 ? parts[7] : null,
                OriginalTitle = parts.Length > 8 ? parts[8] : null,
                Studio = parts.Length > 9 ? parts[9] : null,
                Tagline = parts.Length > 12 ? parts[12] : null,
                Summary = parts.Length > 13 ? parts[13] : null,
                Trivia = parts.Length > 14 ? parts[14] : null,
                Quotes = parts.Length > 15 ? parts[15] : null,
                ContentRating = parts.Length > 16 ? parts[16] : null,
                UserThumbUrl = parts.Length > 21 ? parts[21] : null,
                UserArtUrl = parts.Length > 22 ? parts[22] : null,
                UserBannerUrl = parts.Length > 23 ? parts[23] : null,
                UserMusicUrl = parts.Length > 24 ? parts[24] : null,
                UserFields = parts.Length > 25 ? parts[25] : null,
                TagsGenre = parts.Length > 26 ? parts[26] : null,
                TagsCollection = parts.Length > 27 ? parts[27] : null,
                TagsDirector = parts.Length > 28 ? parts[28] : null,
                TagsWriter = parts.Length > 29 ? parts[29] : null,
                TagsStar = parts.Length > 30 ? parts[30] : null,
                TagsCountry = parts.Length > 40 ? parts[40] : null,
                ExtraData = parts.Length > 41 ? parts[41] : null,
                Hash = parts.Length > 42 ? parts[42] : null
            };

            int parseInt;
            if (parts.Length > 0 && int.TryParse(parts[0], out parseInt))
            {
                plexCollectionTVShow.Id = parseInt;
            }
            if (parts.Length > 1 && int.TryParse(parts[1], out parseInt))
            {
                plexCollectionTVShow.LibrarySectionId = parseInt;
            }
            if (parts.Length > 2 && int.TryParse(parts[2], out parseInt))
            {
                plexCollectionTVShow.ParentId = parseInt;
            }
            if (parts.Length > 3 && int.TryParse(parts[3], out parseInt))
            {
                plexCollectionTVShow.MetadataType = parseInt;
            }
            if (parts.Length > 5 && int.TryParse(parts[5], out parseInt))
            {
                plexCollectionTVShow.MediaItemCount = parseInt;
            }
            if (parts.Length > 11 && int.TryParse(parts[11], out parseInt))
            {
                plexCollectionTVShow.RatingCount = parseInt;
            }
            if (parts.Length > 17 && int.TryParse(parts[17], out parseInt))
            {
                plexCollectionTVShow.ContentRatingAge = parseInt;
            }
            if (parts.Length > 18 && int.TryParse(parts[18], out parseInt))
            {
                plexCollectionTVShow.Index = parseInt;
            }
            if (parts.Length > 19 && int.TryParse(parts[19], out parseInt))
            {
                plexCollectionTVShow.AbsoluteIndex = parseInt;
            }
            if (parts.Length > 20 && int.TryParse(parts[20], out parseInt))
            {
                plexCollectionTVShow.Duration = parseInt;
            }
            if (parts.Length > 35 && int.TryParse(parts[35], out parseInt))
            {
                plexCollectionTVShow.Year = parseInt;
            }
            if (parts.Length > 44 && int.TryParse(parts[44], out parseInt))
            {
                plexCollectionTVShow.ChangedAt = parseInt;
            }
            if (parts.Length > 45 && int.TryParse(parts[45], out parseInt))
            {
                plexCollectionTVShow.ResourcesChangedAt = parseInt;
            }
            if (parts.Length > 46 && int.TryParse(parts[46], out parseInt))
            {
                plexCollectionTVShow.Remote = parseInt;
            }

            DateTime parseDateTime;

            if (parts.Length > 31 && DateTime.TryParse(parts[31], out parseDateTime))
            {
                plexCollectionTVShow.OriginallyAvailableAt = parseDateTime;
            }
            if (parts.Length > 32 && DateTime.TryParse(parts[32], out parseDateTime))
            {
                plexCollectionTVShow.AvailableAt = parseDateTime;
            }
            if (parts.Length > 33 && DateTime.TryParse(parts[33], out parseDateTime))
            {
                plexCollectionTVShow.ExpiresAt = parseDateTime;
            }
            if (parts.Length > 34 && DateTime.TryParse(parts[34], out parseDateTime))
            {
                plexCollectionTVShow.RefreshedAt = parseDateTime;
            }
            if (parts.Length > 36 && DateTime.TryParse(parts[36], out parseDateTime))
            {
                plexCollectionTVShow.AddedAt = parseDateTime;
            }
            if (parts.Length > 37 && DateTime.TryParse(parts[37], out parseDateTime))
            {
                plexCollectionTVShow.CreatedAt = parseDateTime;
            }
            if (parts.Length > 38 && DateTime.TryParse(parts[38], out parseDateTime))
            {
                plexCollectionTVShow.UpdatedAt = parseDateTime;
            }
            if (parts.Length > 39 && DateTime.TryParse(parts[39], out parseDateTime))
            {
                plexCollectionTVShow.DeletedAt = parseDateTime;
            }

            Decimal parseDecimal;

            if (parts.Length > 10 && Decimal.TryParse(parts[10], out parseDecimal))
            {
                plexCollectionTVShow.Rating = parseDecimal;
            }
            if (parts.Length > 43 && Decimal.TryParse(parts[43], out parseDecimal))
            {
                plexCollectionTVShow.AudienceRating = parseDecimal;
            }
        
            return plexCollectionTVShow;
        }
    }
}
