using System.Collections.Generic;

namespace PlexHelpers.Common.Models
{
    public class PlexArtist
    {
        public PlexArtist()
        {
            Albums = new List<PlexAlbum>();
        }
        public PlexMetadDataItem MetaData { get; set; }
        public List<PlexAlbum> Albums { get; set; }
    } 
}