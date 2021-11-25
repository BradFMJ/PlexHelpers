using System;
using PlexHelpers.Common;
using PlexHelpers.Common.Medusa;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace PlexHelpers.TVSeriesMover
{
    class Program
    {
        private static bool CanMove = true;
        private static bool CanDelete = true;
        private static string _targetDrive = "S";
        private static string _drivePath = @":\Media\TV Shows";

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

            _episodes = _episodes.Where(p => p.season != 0).ToList();

            _tvShows = _tvShows.Where(p => p.status == "Ended").ToList();

            foreach (var tvShow in _tvShows)
            {
                tvShow.Episodes = _episodes.Where(p => p.showid == tvShow.indexer_id && p.indexer == tvShow.indexer).ToList();
            }
            _tvShows = _tvShows.Where(p => p.HasAllEpisodes).OrderBy(p => p.SeriesSizeMB).ToList();

            _tvShows = _tvShows.Where(p => p.HasAllSubtitles || p.AllSubtitlesRecentlySearched).OrderBy(p => p.SeriesSizeMB).ToList();

            int totalCount = _tvShows.Count;
            int count = 0;
            ulong totalMBMoved = 0;
            foreach (var tvShow in _tvShows)
            {
                if (tvShow.HasAllEpisodes && (tvShow.HasAllSubtitles || tvShow.AllSubtitlesRecentlySearched))
                {
                    count++;

                    DirectoryInfo tvshowLocation = Helpers.SeriesFolderNameFixer(tvShow.location);

                    var folderName = Helpers.ReplaceInvalidFilePathChars(tvShow.show_name);

                    if (tvShow.startyear > 0 && !folderName.EndsWith(")"))
                    {
                        folderName = folderName + " (" + tvShow.startyear + ")";
                    }

                    var targetDirectory = _targetDrive + _drivePath + "\\" + folderName;


                    if (!tvshowLocation.Exists)
                    {
                        Console.WriteLine("{0}/{1} CANNOT MOVE {2} to {3}. Source Directory Does Not Exist.", count, totalCount, tvShow.show_name, targetDirectory);
                        continue;
                    }

                    var indexString = Helpers.GetIndexerFriendlyName(tvShow.indexer);

                    if (string.IsNullOrWhiteSpace(indexString))
                    {
                        Console.WriteLine("{0}/{1} CANNOT MOVE {2} to {3}. Unknown Indexer: {4}.", count, totalCount, tvShow.show_name, targetDirectory, tvShow.indexer);
                        continue;
                    }

                    DriveInfo driveInfo = new DriveInfo(_targetDrive);

                    int safety = 10240; // ~10GB

                    var freeSpaceMB = (driveInfo.TotalFreeSpace / 1024 / 1024) - safety;
                    if (freeSpaceMB < tvShow.SeriesSizeMB)
                    {
                        Console.WriteLine("{0}/{1} CANNOT MOVE {2} to {3}. Target Drive Out Of Space.", count, totalCount, tvShow.show_name, targetDirectory);
                        continue;
                    }

                    if (CanMove)
                    {
                        DirectoryInfo di2 = new DirectoryInfo(targetDirectory);
                        if (di2.Exists)
                        {
                            Console.WriteLine("{0}/{1} CANNOT MOVE {2} to {3}. Target Directory Exists.", count, totalCount, tvShow.show_name, targetDirectory);
                            continue;
                        }

                        totalMBMoved += (ulong)tvShow.SeriesSizeMB;
                        Console.WriteLine("{0}/{1}: {2} {3} ({4})", count, totalCount, tvShow.show_name, tvshowLocation.FullName, tvShow.SeriesSizeMB);
                        Helpers.DirectoryCopy(tvshowLocation.FullName, targetDirectory, true);

                        //Remove From Medusa
                        string requestUri = "deleteShow?showslug=" + indexString + tvShow.indexer_id;
                        HttpResponseMessage response = _client.GetAsync(requestUri).Result;

                        string result = response.Content.ReadAsStringAsync().Result;

                        if (response.StatusCode != HttpStatusCode.Found)
                        {

                        }
                        if (CanDelete)
                        {
                            try
                            {
                                Directory.Delete(tvshowLocation.FullName, true);

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("{0}/{1} MEDUSA ISSUE REMOVING {2} in {3}.", count, totalCount, tvShow.show_name, targetDirectory);
                                continue;
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Done: " + totalMBMoved.ToString("N0"));
            Console.ReadLine();
        }
    }
}
