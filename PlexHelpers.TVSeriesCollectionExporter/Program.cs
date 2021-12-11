using Newtonsoft.Json;
using PlexHelpers.Common;
using PlexHelpers.Common.Plex;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using PlexHelpers.Common.Models;

namespace PlexHelpers.TVSeriesCollectionExporter
{
    class Program
    {
        private static int port = 32400;

        private static string _host;
        private static string _plexToken;

        static async Task Main(string[] args)
        {
            var plexKey = File.ReadAllText("plex-key.txt");
            _host = plexKey.Split(':')[0];
            _plexToken = plexKey.Split(':')[1];

            HttpClient httClient = new HttpClient();


            var collectionListRequest = new CollectionListRequest
            {
                IncludeCollections = true,
                IncludeExternalMedia = true,
                PlexToken = _plexToken
            };

            var url = new UriBuilder
            {
                Scheme = "https",
                Host = _host,
                Port = port,
                Path = "/library/sections/4/collections",
                Query = collectionListRequest.ToUrlString()
            };

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
            int total = collectionListResponse.MediaContainer.Metadata.Count;

            foreach (var collection in collectionListResponse.MediaContainer.Metadata)
            {
                if (collection.Title == "In Theaters")
                {
                    continue;
                }
                Console.WriteLine("Processing {0} of {1}: {2}", count, total, collection.Title);
                count++;
                var collectionRequest = new CollectionRequest
                {
                    ExcludeAllLeaves = true,
                    PlexToken = _plexToken
                };

                url = new UriBuilder
                {
                    Scheme = "https",
                    Host = _host,
                    Port = port,
                    Path = collection.Key,
                    Query = collectionRequest.ToUrlString()
                };

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
                foreach (var tvShow in collection.MediaContainer.Metadata)
                {
                    try
                    {
                        var plexCollectionTvShow = new PlexCollectionTVShow
                        {
                            CollectionName = collection.MediaContainer.Title2,
                            Year = tvShow.Year,
                            Title = tvShow.Title,
                            CollectionKey = collection.MediaContainer.Key
                        };


                        if (tvShow.Url.IsAbsoluteUri)
                        {
                            if (tvShow.Url.Scheme == "com.plexapp.agents.thetvdb")
                            {
                                plexCollectionTvShow.TheTVDB = tvShow.Url.Host;
                            }
                            else if (tvShow.Url.Scheme == "com.plexapp.agents.themoviedb")
                            {
                                plexCollectionTvShow.TMDB = tvShow.Url.Host;
                            }
                            else if (tvShow.Url.Scheme == "plex")
                            {
                                plexCollectionTvShow.Plex = tvShow.Url.PathAndQuery.Replace("/","");
                            }
                            else if (tvShow.Url.Scheme == "com.plexapp.agents.none")
                            {
                                plexCollectionTvShow.None = tvShow.Url.Host;
                            }
                            else if (tvShow.Url.Scheme == "local")
                            {
                                plexCollectionTvShow.Local = tvShow.Url.Host;
                            }
                            else
                            {
                                int i = 0;
                            }
                        }

                        lines.Add(plexCollectionTvShow.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }

            File.WriteAllLines(@"C:\imdb\lists\backup-tv\collections." + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv", lines);
            File.WriteAllText(@"C:\imdb\collections-tv.json", JsonConvert.SerializeObject(collections));
        }
    }
}
