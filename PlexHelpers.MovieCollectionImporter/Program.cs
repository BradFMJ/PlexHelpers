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

namespace PlexHelpers.MovieCollectionImporter
{
    class Program
    {
        private static int port = 32400;

        private static string host;
        private static string plexToken;

        private static List<PlexMovie> _plexMovies;
        private static List<PlexCollectionMovie> _plexImportListMovies;
        private static List<CollectionResponse> _plexCollections = new List<CollectionResponse>();

        private static string _plexCollectionMovieImportListPath = @"C:\imdb\Lists\import.csv";

        static async Task Main(string[] args)
        {

            var plexKey = File.ReadAllText("plex-key.txt");
            host = plexKey.Split(':')[0];
            plexToken = plexKey.Split(':')[1];

            HttpClient httClient = new HttpClient();

            //_plexMovies = Helpers.ReadPlexMovieCSV("C:\\imdb\\plex-movies.csv", Helpers.ReadPlexMapCSV("C:\\imdb\\plex-map.csv"));
            _plexMovies = Helpers.ReadPlexMovieCSV("C:\\Share\\H\\plex-movies.csv");

            //_plexImportListMovies = Helpers.ReadCollectionCSV("C:\\imdb\\collections.csv");
            _plexImportListMovies = Helpers.ReadCollectionCSV(_plexCollectionMovieImportListPath);

            var moviesNotFound = new List<PlexCollectionMovie>();
            var moviesAdded = new List<PlexCollectionMovie>();

            _plexCollections = JsonConvert.DeserializeObject<List<CollectionResponse>>(File.ReadAllText(@"C:\imdb\collections.json"));

            foreach (var movieToImport in _plexImportListMovies)
            {
                try
                {
                    PlexMovie plexMovie = null;

                    #region Match Collection Import Movie to existing Plex Movie

                    if (!string.IsNullOrWhiteSpace(movieToImport.IMDB))
                    {
                        var found = _plexMovies.Where(p => p.IMDB == movieToImport.IMDB).ToList();
                        if (found.Count == 1)
                        {
                            plexMovie = found.First();
                        }
                        else if (found.Count > 1)
                        {
                            Console.WriteLine("Multiple Movie Matches Found In Lookup For {0}", movieToImport.MovieTitle);
                        }
                        else
                        {
                            Console.WriteLine("No Movie Matches Found In Lookup For {0}", movieToImport.MovieTitle);
                        }
                    }
                    if(plexMovie == null && !string.IsNullOrWhiteSpace(movieToImport.TMDB))
                    {
                        var found = _plexMovies.Where(p => p.TMDB == movieToImport.TMDB).ToList();
                        if (found.Count == 1)
                        {
                            plexMovie = found.First();
                        }
                        else if (found.Count > 1)
                        {
                            Console.WriteLine("Multiple Movie Matches Found In Lookup For {0}", movieToImport.MovieTitle);
                        }
                        else
                        {
                            Console.WriteLine("No Movie Matches Found In Lookup For {0}", movieToImport.MovieTitle);
                        }
                    }
                    if (plexMovie == null)
                    {
                        var found = _plexMovies.Where(p => string.Equals(p.Title, movieToImport.MovieTitle.Replace(",", ""), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        if (found.Count == 1)
                        {
                            if (movieToImport.MovieYear.HasValue)
                            {
                                if (movieToImport.MovieYear == found.First().Year || movieToImport.MovieYear == (found.First().Year - 1) || movieToImport.MovieYear == (found.First().Year + 1))
                                {
                                    plexMovie = found.First();
                                }
                                else
                                {
                                    Console.WriteLine("No Movie Matches Found In Lookup For {0}", movieToImport.MovieTitle);
                                }
                            }
                            else
                            {
                                plexMovie = found.First();
                            }
                        }
                        else if (found.Count > 1)
                        {
                            found = found.Where(p => p.Year == movieToImport.MovieYear || p.Year == (movieToImport.MovieYear - 1) || p.Year == (movieToImport.MovieYear + 1)).ToList();
                            if (found.Count == 1)
                            {
                                plexMovie = found.First();
                            }
                            else if (found.Count > 1)
                            {
                                Console.WriteLine("Multiple Movie Matches Found In Lookup For {0}", movieToImport.MovieTitle);
                            }
                            else
                            {
                                Console.WriteLine("Multiple Movie Matches Found In Lookup For {0}", movieToImport.MovieTitle);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No Movie Matches Found In Lookup For {0}", movieToImport.MovieTitle);
                        }
                    }
                    #endregion

                    CollectionResponse targetPlexCollection = null;

                    #region Match Collection Import Movie to existing Plex Collection

                    //if (!string.IsNullOrWhiteSpace(movieToImport.CollectionKey))
                    //{
                    //    targetPlexCollection = _plexCollections.SingleOrDefault(p => p.MediaContainer.Key == movieToImport.CollectionKey);
                    //}
                    //else
                    //{
                    targetPlexCollection = _plexCollections.SingleOrDefault(p => p.MediaContainer.Title2 == movieToImport.CollectionName);
                    if (targetPlexCollection != null)
                    {
                        movieToImport.CollectionKey = targetPlexCollection.MediaContainer.Key;
                    }
                    // }

                    #endregion

                    if (plexMovie == null)
                    {
                        moviesNotFound.Add(movieToImport);
                        continue;
                    }

                    if (targetPlexCollection == null)
                    {
                        moviesNotFound.Add(movieToImport);
                        Console.WriteLine("No Collection Matches Found In Lookup For {0}", movieToImport.CollectionName);
                        continue;
                    }

                    if (targetPlexCollection.MediaContainer.Metadata.Any(p => p.Guid == plexMovie.Guid))
                    {
                        //Movie already exists in the collection
                        Console.WriteLine("Removing Movie already in Collection {0}, {1}", movieToImport.CollectionName, movieToImport.MovieTitle);
                        continue;
                    }

                    moviesAdded.Add(movieToImport);

                    var movieRequest = new MovieRequest();
                    movieRequest.PlexToken = plexToken;

                    var url = new UriBuilder();
                    url.Scheme = "https";
                    url.Host = host;
                    url.Port = port;
                    url.Path = "/library/metadata/" + plexMovie.MetadataId;
                    url.Query = movieRequest.ToUrlString();


                    Console.WriteLine("Adding {0} to {1}", movieToImport.MovieTitle, targetPlexCollection.MediaContainer.Title2);

                    List<string> existingCollections = new List<string>();

                    using (var request = new HttpRequestMessage())
                    {
                        request.RequestUri = url.Uri;
                        request.Method = HttpMethod.Get;
                        request.Headers.Add("Accept", "application/json");

                        using (var response = await httClient.SendAsync(request))
                        {
                            var responseContent = response.Content.ReadAsStringAsync().Result;
                            response.EnsureSuccessStatusCode();

                            //var movieResponse = JsonConvert.DeserializeObject<MovieResponse>(responseContent);
                            var movieResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                            try
                            {
                                if (Helpers.DoesPropertyExist(movieResponse.MediaContainer.Metadata[0], "Collection"))
                                {
                                    existingCollections = ((IEnumerable<dynamic>)movieResponse.MediaContainer.Metadata[0].Collection).Select(p => (string)p.tag).ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                moviesNotFound.Add(movieToImport);
                                Console.WriteLine(ex);
                            }
                        }
                    }

                    //get all collections that this movie is in
                    //existingCollections = _plexCollections.Where(p => p.MediaContainer.Metadata.Any(c => c.Guid == plexMovie.Guid)).Select(p => p.MediaContainer.Title2).ToList();
                    existingCollections.Add(targetPlexCollection.MediaContainer.Title2);
                    existingCollections = existingCollections.Distinct().ToList();

                    var collectionAddRequest = new CollectionAddRequest();
                    collectionAddRequest.PlexToken = plexToken;
                    collectionAddRequest.MetadataId = plexMovie.MetadataId;
                    collectionAddRequest.Collections = existingCollections;
                    url = new UriBuilder();
                    url.Scheme = "https";
                    url.Host = host;
                    url.Port = port;
                    url.Path = "/library/sections/1/all";
                    url.Query = collectionAddRequest.ToUrlString();

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
                }
                catch (Exception ex)
                {
                    moviesNotFound.Add(movieToImport);
                    Console.WriteLine(ex);
                }
            }

            Helpers.WriteCollectionCSV(_plexCollectionMovieImportListPath, moviesNotFound);

            Console.WriteLine("Done");

            Console.ReadLine();
        }
    }
}
