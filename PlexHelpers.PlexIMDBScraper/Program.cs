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
        private static int port = 32400;

        private static string host;
        private static string plexToken;

        private static List<PlexMovie> _plexMovies;
        private static List<PlexIMDBMap> _plexMaps;

        static async Task Main(string[] args)
        {
            HttpClient httClient = new HttpClient();

            _plexMaps = Helpers.ReadPlexMapCSV("C:\\Share\\H\\plex-map.csv");
            //_plexMovies = Helpers.ReadPlexMovieCSV("C:\\Share\\H\\plex-movies.csv", _plexMaps);
            _plexMovies = Helpers.ReadPlexMovieCSV("C:\\Share\\H\\plex-movies.csv");

            var plexKey = File.ReadAllText("plex-key.txt");
            host = plexKey.Split(':')[0];
            plexToken = plexKey.Split(':')[1];

            var url = new UriBuilder();
            url.Scheme = "https";
            url.Host = host;
            url.Port = port;
            url.Query = "X-Plex-Token=" + plexToken;
            int count = 1;
            int total = _plexMovies.Count();
            foreach (var movie in _plexMovies)
            {
                try
                {
                    Console.WriteLine("Processing {0} of {1}: {2}", count, total, movie.Title);

                    if (!string.IsNullOrWhiteSpace(movie.Guid))
                    {
                        var map = _plexMaps.FirstOrDefault(p => p.PlexGuid == movie.Guid);

                        if (map == null)
                        {
                            //fetch and parse xml

                            url.Path = @"/library/metadata/" + movie.MetadataId;
                            string responseContent;
                            using (var request = new HttpRequestMessage())
                            {
                                request.RequestUri = url.Uri;
                                request.Method = HttpMethod.Get;
                                request.Headers.Add("Accept", "application/json");

                                using (var response = await httClient.SendAsync(request))
                                {
                                    responseContent = response.Content.ReadAsStringAsync().Result;
                                    response.EnsureSuccessStatusCode();
                                }
                            }

                            Uri parseUri;

                            dynamic metaData = JObject.Parse(responseContent);

                            var mediaContainer = metaData.MediaContainer;

                            var metadata = mediaContainer.Metadata[0];

                            var plexIMDBMap = new PlexIMDBMap
                            {
                                PlexGuid = metadata.guid
                            };
                            if (Uri.TryCreate(plexIMDBMap.PlexGuid, UriKind.Absolute, out parseUri))
                            {
                                plexIMDBMap.Plex = parseUri.PathAndQuery.Replace(@"/", "");
                                movie.Plex = plexIMDBMap.Plex;
                            }
                            if (metadata.Guid == null)
                            {
                                continue;
                            }
                            foreach (dynamic guid in metadata.Guid)
                            {
                                string guidurl = guid.id;

                                if (Uri.TryCreate(guidurl, UriKind.Absolute, out parseUri))
                                {
                                    if (parseUri.Scheme == "imdb")
                                    {
                                        plexIMDBMap.IMDB = parseUri.Host;
                                        movie.IMDB = plexIMDBMap.IMDB;
                                    }
                                    if (parseUri.Scheme == "tmdb")
                                    {
                                        plexIMDBMap.TMDB = parseUri.Host;
                                        movie.TMDB = plexIMDBMap.TMDB;
                                    }
                                    if (parseUri.Scheme == "tvdb")
                                    {
                                        plexIMDBMap.TVDB = parseUri.Host;
                                        movie.TVDB = plexIMDBMap.TVDB;
                                    }
                                }
                            }

                            _plexMaps.Add(plexIMDBMap);
                            File.AppendAllLines("C:\\Share\\H\\plex-map.csv", new List<string> { Helpers.EscapeCsvField(plexIMDBMap.PlexGuid) + "," + Helpers.EscapeCsvField(plexIMDBMap.Plex) + "," + Helpers.EscapeCsvField(plexIMDBMap.IMDB) + "," + Helpers.EscapeCsvField(plexIMDBMap.TMDB) + "," + Helpers.EscapeCsvField(plexIMDBMap.TVDB) });
                            Helpers.WritePlexMapCSV("C:\\imdb\\plex-map.csv", _plexMaps);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    count++;
                }
            }
            Helpers.WritePlexMapCSV("C:\\Share\\H\\plex-map.csv", _plexMaps);

            foreach (var plexMovie in _plexMovies)
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
                }
            }


            Helpers.WritePlexMovieCSV("C:\\Share\\H\\plex-movies.csv", _plexMovies);
        }
    }
}
