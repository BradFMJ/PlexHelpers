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

            _plexMaps = Helpers.ReadPlexMapCSV("C:\\imdb\\plex-map.csv");
            _plexMovies = Helpers.ReadPlexMovieCSV("C:\\imdb\\plex-movies.csv", _plexMaps);

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
                                plexIMDBMap.Plex = parseUri.PathAndQuery.Replace(@"/","");
                            }
                            if(metadata.Guid == null)
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
                                    }
                                    if (parseUri.Scheme == "tmdb")
                                    {
                                        plexIMDBMap.TMDB = parseUri.Host;
                                    }
                                    if (parseUri.Scheme == "tvdb")
                                    {
                                        plexIMDBMap.TVDB = parseUri.Host;
                                    }
                                }
                            }

                            _plexMaps.Add(plexIMDBMap);
                           // Helpers.WritePlexMapCSV("C:\\imdb\\plex-map.csv", _plexMaps);
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

            Helpers.WritePlexMapCSV("C:\\imdb\\plex-map.csv", _plexMaps);
        }
    }
}
