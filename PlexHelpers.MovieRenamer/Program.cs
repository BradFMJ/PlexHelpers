using Newtonsoft.Json.Linq;
using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlexHelpers.PlexIMDBScraper
{
    internal class Program
    {

        private static List<PlexMovie> _plexMovies;
        private static List<PlexIMDBMap> _plexMaps;
        private static string _targetDrive = "C";
        private static string _targetDrivePath = @":\Share\Movies\1080P";

        static async Task Main(string[] args)
        {
            HttpClient httClient = new HttpClient();

            _plexMaps = Helpers.ReadPlexMapCSV(@"K:\Media\H\Media\plex-map.csv");
            _plexMovies = Helpers.ReadPlexMovieCSV(@"K:\Media\H\Media\plex-movies.csv");
            int count = 1;
            int total = _plexMovies.Count();
            foreach (var plexMovie in _plexMovies)
            {
                try
                {
                    var map = _plexMaps.FirstOrDefault(p => p.PlexGuid == plexMovie.Guid);
                    if (map != null)
                    {
                        if (!string.IsNullOrWhiteSpace(map.IMDB))
                        {
                            plexMovie.IMDB = map.IMDB;
                        }
                        if (!string.IsNullOrWhiteSpace(map.TMDB))
                        {
                            plexMovie.TMDB = map.TMDB;
                        }
                        if (!string.IsNullOrWhiteSpace(map.Plex))
                        {
                            plexMovie.Plex = map.Plex;
                        }
                        var targetDirectory = string.Format("{0}{1}\\{2} ({3}) {{4}}", _targetDrive, _targetDrivePath, plexMovie.Title, plexMovie.Year, plexMovie.IMDB);
                        if (!Directory.Exists(targetDirectory))
                        {
                            //Directory.Move(plexMovie.FileInfo.DirectoryName, targetDirectory);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing movie {plexMovie.Title}: {ex.Message}");
                }
            }
        }
    }
}