using Microsoft.VisualBasic.FileIO;
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

namespace PlexHelpers.TVSeriesCollectionImporter
{
    class Program
    {
        private static int port = 32400;

        private static string host;
        private static string plexToken;

        private static List<PlexMetadDataItem> _plexTVShows;
        private static List<PlexCollectionTVShow> _plexImportTVShows;
        private static List<CollectionResponse> _plexCollections = new List<CollectionResponse>();

        //--List TV Shows for Import
        //SELECT[tag],[title],[year]   FROM metadata_items
        //left join taggings on taggings.metadata_item_id = metadata_items.id
        //left join tags on tags.id = taggings.tag_id
        //where metadata_items.metadata_type= 2 and tag_type = 319
        //order by [tag]
        private static string _plexCollectionTVShowImportListPath = @"C:\Share\H\import-tv.csv";

        static async Task Main(string[] args)
        {

            var plexKey = File.ReadAllText("plex-key.txt");
            host = plexKey.Split(':')[0]; 
            plexToken = plexKey.Split(':')[1];

            HttpClient httClient = new HttpClient();

            //SELECT * FROM metadata_items where metadata_items.metadata_type = 2
            _plexTVShows = Helpers.ReadPlexMetadDataItem(@"C:\Share\H\plex-tv-shows.csv");

            _plexImportTVShows = Helpers.ReadTVShowCollectionCSV(_plexCollectionTVShowImportListPath);

            var tvShowsNotFound = new List<PlexCollectionTVShow>();

            _plexCollections = JsonConvert.DeserializeObject<List<CollectionResponse>>(File.ReadAllText(@"C:\imdb\collections-tv.json"));


            Dictionary<string,string> studioMappings = new Dictionary<string, string>();

            var studioMappingLines = File.ReadAllLines("C:\\imdb\\tv-show-mapping.csv");

            for (var i = 0; i < studioMappingLines.Length; i++)
            {
                TextFieldParser parser = new TextFieldParser(new StringReader(studioMappingLines[i]))
                {
                    HasFieldsEnclosedInQuotes = true
                };
                parser.SetDelimiters(",");

                string[] parts = null;

                while (!parser.EndOfData)
                {
                    parts = parser.ReadFields();
                }

                try
                {
                    if (parts.Length >1)
                    {
                        studioMappings[parts[0]] = parts[1];
                    }
                }
                catch (Exception e)
                {
                    int u = 0;
                }
            }

            foreach (var plexTVShow in _plexTVShows)
            {
                if(plexTVShow.Title == "The Fairly OddParents")
                {
                    int i = 0;
                }
                string studioCollection;
                if (studioMappings.TryGetValue(plexTVShow.Studio, out studioCollection))
                {
                    if (string.IsNullOrWhiteSpace(studioCollection))
                    {
                        continue;
                    }

                    var collection = _plexCollections.FirstOrDefault(p => string.Equals(p.MediaContainer.Title2, studioCollection, StringComparison.InvariantCulture));

                    if (collection == null)
                    {
                        Console.WriteLine("No Collection for Studio {0}", studioCollection);
                        continue;
                    }
                    var plexCollectionTVShow = new PlexCollectionTVShow();
                    plexCollectionTVShow.CollectionKey = collection.MediaContainer.Key;
                    plexCollectionTVShow.Title = plexTVShow.Title;
                    plexCollectionTVShow.CollectionName = studioCollection;
                    plexCollectionTVShow.Year = plexTVShow.Year;

                    _plexImportTVShows.Add(plexCollectionTVShow);
                }
                else
                {
                    Console.WriteLine("No Match for Studio {0}", plexTVShow.Studio);
                }
            }

            foreach (var tvShowToImport in _plexImportTVShows)
            {
                try
                {
                    PlexMetadDataItem plexTVShow = null;

                    #region Match Collection Import TV Show to existing Plex TV Show

                    var found = _plexTVShows.Where(p => p.Title == tvShowToImport.Title && p.Year == tvShowToImport.Year).ToList();
                    if (found.Count == 1)
                    {
                        plexTVShow = found.First();
                    }
                    else if (found.Count > 1)
                    {
                        Console.WriteLine("Multiple TV Show Matches Found In Lookup For {0}", tvShowToImport.Title);
                    }
                    else
                    {
                        Console.WriteLine("No TV Show Matches Found In Lookup For {0}", tvShowToImport.Title);
                    }
                    #endregion

                    CollectionResponse targetPlexCollection;

                    #region Match Collection Import TV Show to existing Plex Collection

                    if (tvShowToImport.CollectionName.Contains("Apple")) 
                    {
                        int h = 0;
                    }
                    if (!string.IsNullOrWhiteSpace(tvShowToImport.CollectionKey))
                    {
                        targetPlexCollection = _plexCollections.SingleOrDefault(p => p.MediaContainer.Key == tvShowToImport.CollectionKey);
                    }
                    else
                    {
                        string studioCollection2 = null;
                        //bool isFound = studioMappings.TryGetValue(tvShowToImport.CollectionName, out studioCollection);
                        if(studioMappings.ContainsKey(tvShowToImport.CollectionName))
                        {
                            studioCollection2 = studioMappings[tvShowToImport.CollectionName];
                            if(string.IsNullOrWhiteSpace(studioCollection2))
                            {
                                studioCollection2 = tvShowToImport.CollectionName;
                            }
                        }
                        else
                        {
                            studioCollection2 = tvShowToImport.CollectionName;
                        }
                        //if (!isFound)
                        //{
                        //    studioCollection = tvShowToImport.CollectionName;
                        //}
                        //if (isFound && studioCollection == null)
                        //{
                        //    studioCollection = tvShowToImport.CollectionName;
                        //}
                        targetPlexCollection = _plexCollections.SingleOrDefault(p => p.MediaContainer.Title2 == studioCollection2);
                        if (targetPlexCollection != null)
                        {
                            tvShowToImport.CollectionKey = targetPlexCollection.MediaContainer.Key;
                        }
                    }

                    #endregion

                    if (plexTVShow == null)
                    {
                        tvShowsNotFound.Add(tvShowToImport);
                        continue;
                    }

                    if (targetPlexCollection == null)
                    {
                        tvShowsNotFound.Add(tvShowToImport);
                        Console.WriteLine("No Collection Matches Found In Lookup For {0}", tvShowToImport.CollectionName);
                        continue;
                    }

                    if (targetPlexCollection.MediaContainer.Metadata.Any(p => p.Guid == plexTVShow.Guid))
                    {
                        //TV Show already exists in the collection
                        //Console.WriteLine("Removing TV Show already in Collection {0}, {1}", tvShowToImport.CollectionName, tvShowToImport.Title);
                        continue;
                    }

                    var movieRequest = new MovieRequest
                    {
                        PlexToken = plexToken
                    };

                    var url = new UriBuilder
                    {
                        Scheme = "https",
                        Host = host,
                        Port = port,
                        Path = "/library/metadata/" + plexTVShow.Id,
                        Query = movieRequest.ToUrlString()
                    };


                    Console.WriteLine(plexTVShow.Id);

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

                            var movieResponse = JsonConvert.DeserializeObject<TVShowResponse>(responseContent);

                            existingCollections = movieResponse.MediaContainer.Metadata.SelectMany(p => p.Collections.Select(c => c.Tag)).ToList();
                        }
                    }

                    //get all collections that this TV Show is in
                    existingCollections.Add(targetPlexCollection.MediaContainer.Title2);
                    existingCollections = existingCollections.Distinct().ToList();

                    var collectionAddRequest = new CollectionAddRequest
                    {
                        PlexToken = plexToken,
                        MetadataId = plexTVShow.Id,
                        Collections = existingCollections,
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
                    Console.WriteLine("Added {0} to {1}", tvShowToImport.Title, targetPlexCollection.MediaContainer.Title2);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }


            }

            Helpers.WriteTVShowCollectionCSV(_plexCollectionTVShowImportListPath, tvShowsNotFound);

            Console.WriteLine("Done");

            Console.ReadLine();
        }
    }
}
