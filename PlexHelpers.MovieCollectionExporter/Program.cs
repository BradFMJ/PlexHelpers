using Newtonsoft.Json;
using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using PlexHelpers.Common.Plex;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlexHelpers.MovieCollectionExporter
{
    class Program
    {
        private static int port = 32400;

        private static string host;
        private static string plexToken;

        static async Task Main(string[] args)
        {
            var plexKey = File.ReadAllText("plex-key.txt");
            host = plexKey.Split(':')[0];
            plexToken = plexKey.Split(':')[1];

            HttpClient httClient = new HttpClient();


            var collectionListRequest = new CollectionListRequest();
            collectionListRequest.IncludeCollections = true;
            collectionListRequest.IncludeExternalMedia = true;
            collectionListRequest.PlexToken = plexToken;

            var url = new UriBuilder();
            url.Scheme = "https";
            url.Host = host;
            url.Port = port;
            url.Path = "/library/sections/1/collections";
            url.Query = collectionListRequest.ToUrlString();

            CollectionListResponse collectionListResponse;

            using (var request = new HttpRequestMessage())
            {
                request.RequestUri = url.Uri;
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");

                using (var response = await httClient.SendAsync(request))
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    response.EnsureSuccessStatusCode();

                    collectionListResponse = JsonConvert.DeserializeObject<CollectionListResponse>(responseContent);
                }
            }

            List<CollectionResponse> collections = new List<CollectionResponse>();

            int count = 1;
            int total = collectionListResponse.MediaContainer.Metadata.Count();

            foreach (var collection in collectionListResponse.MediaContainer.Metadata)
            {
                if (collection.Title == "In Theaters")
                {
                    continue;
                }
                Console.WriteLine("Processing {0} of {1}: {2}", count, total, collection.Title);
                count++;
                var collectionRequest = new CollectionRequest();
                collectionRequest.ExcludeAllLeaves = true;
                collectionRequest.PlexToken = plexToken;

                url = new UriBuilder();
                url.Scheme = "https";
                url.Host = host;
                url.Port = port;
                url.Path = collection.Key;
                url.Query = collectionRequest.ToUrlString();

                using (var request = new HttpRequestMessage())
                {
                    request.RequestUri = url.Uri;
                    request.Method = HttpMethod.Get;
                    request.Headers.Add("Accept", "application/json");

                    using (var response = await httClient.SendAsync(request))
                    {
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        response.EnsureSuccessStatusCode();

                        var collectionResponse = JsonConvert.DeserializeObject<CollectionResponse>(responseContent);
                        collectionResponse.MediaContainer.Key = collection.RatingKey;
                        collectionResponse.MediaContainer.Title2 = collection.Title;
                        collections.Add(collectionResponse);

                    }
                }
            }

            var lines = new List<string>();
            foreach (var collection in collections)
            {
                foreach (var movie in collection.MediaContainer.Metadata)
                {
                    try
                    {
                        var plexCollectionMovie = new PlexCollectionMovie()
                        {
                            CollectionName = Helpers.EscapeCsvField(collection.MediaContainer.Title2),
                            MovieTitle = Helpers.EscapeCsvField(movie.Title),
                            MovieYear = movie.Year,
                            CollectionKey = collection.MediaContainer.Key
                        };
                        string imdb = string.Empty;
                        string tmdb = string.Empty;
                        string plex = string.Empty;
                        if (movie.Url.IsAbsoluteUri)
                        {
                            if (movie.Url.Scheme == "com.plexapp.agents.imdb")
                            {
                                plexCollectionMovie.IMDB = movie.Url.Host;
                            }
                            if (movie.Url.Scheme == "com.plexapp.agents.themoviedb")
                            {
                                plexCollectionMovie.TMDB = movie.Url.Host;
                            }
                            if (movie.Url.Scheme == "plex")
                            {
                                plexCollectionMovie.Plex = movie.Url.AbsoluteUri;
                            }
                        }

                        lines.Add(plexCollectionMovie.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }

            File.WriteAllLines(@"C:\imdb\lists\backup\collections." + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv", lines);
            File.WriteAllText(@"C:\imdb\collections.json", JsonConvert.SerializeObject(collections));
        }
    }
}
