using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlexHelpers.MusicImporter
{
    internal class Program
    {
        private static List<PlexMetadDataItem> _plexMusicItems;
        private static List<PlexArtist> _plexArtists = new List<PlexArtist>();

        private static List<PlexCollectionTrack> _plexImportMusic;
        private static string _plexCollectionMusicImportListPath = @"C:\imdb\Lists\import-music.csv";


        static void Main(string[] args)
        {
            _plexMusicItems = Helpers.ReadPlexMetadDataItem("C:\\imdb\\plex-music.csv");

            foreach (var artist in _plexMusicItems.Where(p => p.MetadataType == 8))
            {
                var plexArtist = new PlexArtist { MetaData = artist };
                foreach (var album in _plexMusicItems.Where(p => p.ParentId == artist.Id))
                {
                    var plexAlbum = new PlexAlbum() { MetaData = album };
                    foreach (var track in _plexMusicItems.Where(p => p.ParentId == album.Id))
                    {
                        plexAlbum.Tracks.Add(new PlexTrack() { MetaData = track });
                    }
                    plexArtist.Albums.Add(plexAlbum);
                }
                _plexArtists.Add(plexArtist);
            }


            _plexImportMusic = Helpers.ReadMusicCollectionCSV(_plexCollectionMusicImportListPath);

            List<PlexCollectionTrack> _missedtracks = new List<PlexCollectionTrack>();

            foreach (var plexImportMusic in _plexImportMusic)
            {
                var plexArtist = _plexArtists.FirstOrDefault(p => p.MetaData.Title.ToLowerInvariant().Trim() == plexImportMusic.Artist.ToLowerInvariant().Trim());
                if (plexArtist != null)
                {
                    var plexAlbum = plexArtist.Albums.FirstOrDefault(p => p.MetaData.Title.ToLowerInvariant().Trim() == plexImportMusic.Album.ToLowerInvariant().Trim());
                    if (plexAlbum != null)
                    {
                        var plexTrack = plexAlbum.Tracks.FirstOrDefault(p => p.MetaData.Title.ToLowerInvariant().Trim() == plexImportMusic.Track.ToLowerInvariant().Trim());
                        if (plexTrack != null)
                        {
                            //todo: add to playlist
                            continue;
                        }
                    }
                }
                Console.WriteLine(plexImportMusic.ToString());
                _missedtracks.Add(plexImportMusic);
            }

            var output = _missedtracks.Select(p=> p.Artist + ", " + p.Album).Distinct().ToList();
            foreach(var foo in output)
            {
               // Console.WriteLine(foo);
            }

            Console.ReadLine();
        }
    }
}
