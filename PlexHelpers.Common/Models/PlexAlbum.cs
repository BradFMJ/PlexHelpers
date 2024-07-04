using System.Collections.Generic;

namespace PlexHelpers.Common.Models
{
    public class PlexAlbum
    {
        public PlexAlbum() {
            Tracks = new List<PlexTrack>();
        }
        public PlexMetadDataItem MetaData { get; set; }
        public List<PlexTrack> Tracks { get; set; }
    }
}
