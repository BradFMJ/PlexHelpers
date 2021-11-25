using PlexHelpers.Common;
using PlexHelpers.Common.Medusa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace PlexHelpers.TVSeriesRefresher
{
    class Program
    {
        private static List<TVShow> _tvShows;
        private static List<Episode> _episodes;

        private static HttpClient _client;

        static void Main(string[] args)
        {
            _client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = false
            });
            _client.BaseAddress = new Uri("http://192.168.1.207:8081/home/");

            _tvShows = Helpers.ReadTVShowCSV("C:\\imdb\\medusa_tvshows.csv");
            _episodes = Helpers.ReadEpisodeCSV("C:\\imdb\\medusa_episodes.csv");

            foreach (var tvShow in _tvShows)
            {
                tvShow.Episodes = _episodes.Where(p => p.showid == tvShow.indexer_id && p.indexer == tvShow.indexer).ToList();
            }

            _tvShows = _tvShows.Where(p => p.HasNoEpisodes).ToList();

            int totalCount = _tvShows.Count;

            int count = 0;

            foreach (var tvShow in _tvShows)
            {
                if (tvShow.HasNoEpisodes)
                {
                    var indexString = Helpers.GetIndexerFriendlyName(tvShow.indexer);

                    if (string.IsNullOrWhiteSpace(indexString))
                    {
                        continue;
                    }

                    count++;

                    //Update Medusa
                    string requestUri = "refreshShow?showslug=" + indexString + tvShow.indexer_id;
                    Console.WriteLine("{0}/{1} REFRESHING {2}  | {3}.", count, totalCount, tvShow.show_name, requestUri);
                    HttpResponseMessage response = _client.GetAsync(requestUri).Result;

                    string result = response.Content.ReadAsStringAsync().Result;

                    if (response.StatusCode != HttpStatusCode.Found)
                    {
                        Console.WriteLine("{0}/{1} ERROR REFRESHING {2}  | {3}.", count, totalCount, tvShow.show_name, requestUri);
                    }
                }
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
