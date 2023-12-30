using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using PlexHelpers.Common.Plex;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlexHelpers.MovieCollectionRestore
{
    internal class Program
    {

        private static int port = 32400;

        private static string host;
        private static string plexToken;

        private static List<PlexMetadDataItem> _plexMovies;
        private static List<PlexMetadDataItem> _oldPlexMovies;

        static async Task Main(string[] args)
        {
            var plexKey = File.ReadAllText("plex-key.txt");
            host = plexKey.Split(':')[0];
            plexToken = plexKey.Split(':')[1];

            UriBuilder url;
            HttpClient httClient = new HttpClient();

            _plexMovies = Helpers.ReadPlexMetadDataItem("C:\\imdb\\plex-movies-list.csv");
            _oldPlexMovies = Helpers.ReadPlexMetadDataItem("C:\\imdb\\plex-movies-list.old.csv");

            int counter = 0;

            //get IMDB for new movies
            var plexMaps = Helpers.ReadPlexMapCSV("C:\\imdb\\plex-map.csv");
            foreach (var plexMovie in _plexMovies)
            {
                var map = plexMaps.FirstOrDefault(p => p.PlexGuid == plexMovie.Guid);
                if (map != null)
                {
                    if (!string.IsNullOrWhiteSpace(map.IMDB))
                    {
                        plexMovie.IMDB = map.IMDB;
                    }
                }
                if (string.IsNullOrWhiteSpace(plexMovie.IMDB))
                {
                    counter++;
                }
            }

            counter = 0;

            //get IMDB for old movies
            Uri parseUri;
            foreach (var plexMovie in _oldPlexMovies)
            {
                if (Uri.TryCreate(plexMovie.Guid, UriKind.Absolute, out parseUri))
                {

                    if (parseUri.Scheme == "com.plexapp.agents.imdb")
                    {
                        plexMovie.IMDB = parseUri.Host;
                    }
                }
                else if (plexMovie.Guid.StartsWith("tt"))
                {
                    plexMovie.IMDB = plexMovie.Guid;
                }
                if (string.IsNullOrWhiteSpace(plexMovie.IMDB))
                {
                    counter++;
                }
            }
            counter = 0;

            foreach (var oldPlexMovie in _oldPlexMovies)
            {
                PlexMetadDataItem plexMovie = null;

                var found = _plexMovies.Where(p => p.IMDB == oldPlexMovie.IMDB).ToList();
                if (found.Count == 0)
                {
                    found = _plexMovies.Where(p => p.Title == oldPlexMovie.Title && p.Year == oldPlexMovie.Year).ToList();
                }
                if (found.Count == 1)
                {
                    counter++;
                    //continue;
                    plexMovie = found.First();

                    if (string.IsNullOrWhiteSpace(oldPlexMovie.TagsCollection))
                    {
                        continue;
                    }

                    var oldCollections = oldPlexMovie.TagsCollection.Split('|').ToList();
                    var collections = plexMovie.TagsCollection.Split('|').ToList();
                    var missingCollections = oldCollections.Except(collections).Distinct().ToList();
                    missingCollections.Remove("In Theaters");
                    if (!missingCollections.Any())
                    {
                        continue;
                    }
                    oldCollections.AddRange(collections);
                    var collectionAddRequest = new CollectionAddRequest
                    {
                        PlexToken = plexToken,
                        MetadataId = plexMovie.Id,
                        Collections = oldCollections.Distinct().ToList(),
                        Type = 1
                    };

                    url = new UriBuilder
                    {
                        Scheme = "https",
                        Host = host,
                        Port = port,
                        Path = "/library/sections/1/all",
                        //Path = "/library/sections/4/all",
                        Query = collectionAddRequest.ToUrlString()
                    };

                    using (var request = new HttpRequestMessage())
                    {
                        request.RequestUri = url.Uri;
                        request.Method = HttpMethod.Put;
                        request.Headers.Add("Accept", "application/json");

                        using (var response = await httClient.SendAsync(request))
                        {
                            var responseContent = response.Content.ReadAsStringAsync().Result;
                            response.EnsureSuccessStatusCode();
                        }
                    }
                    Console.WriteLine("Added {0} to {1}", plexMovie.Title, string.Join(",", collectionAddRequest.Collections));
                }
            }

            counter = 0;
        }
    }
}
