using System;
using System.IO;

namespace PlexHelpers.Common.Models
{
    public class PlexMovie
    {
        public string Title { get; set; }
        public string Guid { get; set; }
        public string CompareTitle { get; set; }
        public int? Year { get; set; }
        public long Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public decimal DisplayAspectRatio { get; set; }
        public int BitRate { get; set; }
        public int AudioChannels { get; set; }
        public string Container { get; set; }
        public string VideoCodec { get; set; }
        public string AudioCodec { get; set; }
        public string FullFileName { get; set; }
        public FileInfo FileInfo { get; set; }
        public Uri Uri { get; set; }
        public string Hash { get; set; }
        public string MovieFolderName { get; set; }
        public int MetadataId { get; set; }
        public int Duration { get; set; }
        public string IMDB { get; set; }
        public string TMDB { get; set; }
        public string TVDB { get; set; }
        public string Plex { get; set; }
    }
}
