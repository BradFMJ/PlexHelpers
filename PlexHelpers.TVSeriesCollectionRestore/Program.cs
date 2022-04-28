using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using PlexHelpers.Common.Plex;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlexHelpers.TVSeriesCollectionRestore
{
    internal class Program
    {
        private static int port = 32400;

        private static string host;
        private static string plexToken;

        private static List<PlexMetadDataItem> _plexTVShows;
        private static List<PlexMetadDataItem> _oldPlexTVShows;

        static async Task Main(string[] args)
        {
            var plexKey = File.ReadAllText("plex-key.txt");
            host = plexKey.Split(':')[0];
            plexToken = plexKey.Split(':')[1];

            UriBuilder url;
            HttpClient httClient = new HttpClient();

            _plexTVShows = Helpers.ReadPlexMetadDataItem("C:\\imdb\\plex-tv-shows.csv");
            _oldPlexTVShows = Helpers.ReadPlexMetadDataItem("C:\\imdb\\plex-tv-shows.old.csv");

            foreach(var oldPlexTVShows in _oldPlexTVShows)
            {
                PlexMetadDataItem plexTVShow = null;

                var found = _plexTVShows.Where(p => p.Title == oldPlexTVShows.Title && p.Year == oldPlexTVShows.Year).ToList();
                if (found.Count == 1)
                {
                    plexTVShow = found.First();

                    if(string.IsNullOrWhiteSpace(oldPlexTVShows.TagsCollection))
                    {
                        continue;
                    }

                    var collectionAddRequest = new CollectionAddRequest
                    {
                        PlexToken = plexToken,
                        MetadataId = plexTVShow.Id,
                        Collections = oldPlexTVShows.TagsCollection.Split('|').ToList(),
                        Type = 2
                    };

                    url = new UriBuilder
                    {
                        Scheme = "https",
                        Host = host,
                        Port = port,
                        Path = "/library/sections/2/all",
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
                    Console.WriteLine("Added {0} to {1}", plexTVShow.Title, collectionAddRequest.Collections);
                }
            }
        }
    }
}
