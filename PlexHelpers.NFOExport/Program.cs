using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace PlexHelpers.NFOExport
{
    class Program
    {
        private static bool CanMove = true;
        private static List<PlexMovie> _plexMovies;
        private static string _targetDrive = "V";
        private static string _drivePath = @":\Media\Movies";

        static void Main(string[] args)
        {
            _plexMovies = Helpers.ReadPlexMovieCSV("C:\\imdb\\plex-movies.csv", Helpers.ReadPlexMapCSV("C:\\imdb\\plex-map.csv"));

            XmlDataDocument xmldoc;
            XmlNodeList xmlnode;

            DirectoryInfo targetDirectoryInfo = new DirectoryInfo(_targetDrive + _drivePath);

            #region Check for incorrect folder names. Check for incorrect NFO Info.

            foreach (var plexMovie in _plexMovies.Where(p => p.FullFileName.StartsWith(_targetDrive)))
            {
                //Check for incorrect folder names
                var targetDirectory = _targetDrive + _drivePath + "\\" + plexMovie.MovieFolderName;

                if (!plexMovie.FullFileName.ToLowerInvariant().StartsWith(targetDirectory.ToLowerInvariant()))
                {
                    Console.WriteLine(plexMovie.Title + " (" + plexMovie.Year + ")" + ",Incorrect Folder Name," + plexMovie.IMDB + "," + plexMovie.TMDB + "," + plexMovie.FullFileName);
                }

                //Check for incorrect NFO Info
                if (!string.IsNullOrWhiteSpace(plexMovie.IMDB) || !string.IsNullOrWhiteSpace(plexMovie.TMDB))
                {
                    var xmlPath = plexMovie.FileInfo.FullName.Substring(0, plexMovie.FileInfo.FullName.Length - plexMovie.FileInfo.Extension.Length) + ".nfo";

                    if (File.Exists(xmlPath))
                    {
                        try
                        {
                            FileStream fs = new FileStream(xmlPath, FileMode.Open, FileAccess.Read);
                            xmldoc = new XmlDataDocument();
                            xmldoc.Load(fs);
                            xmlnode = xmldoc.GetElementsByTagName("uniqueid");


                            if (xmlnode.Count > 0)
                            {
                                bool isMatch = false;

                                var nfoIMDB = string.Empty;
                                var nfoTMDB = string.Empty;

                                for (var t = 0; t < xmlnode.Count; t++)
                                {
                                    var node = xmlnode[t];

                                    if (node.Attributes["type"].Value.ToLowerInvariant() == "tmdb")
                                    {
                                        nfoTMDB = node.InnerText.ToLowerInvariant();
                                    }
                                    if (node.Attributes["type"].Value.ToLowerInvariant() == "imdb")
                                    {
                                        nfoIMDB = node.InnerText.ToLowerInvariant();
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(plexMovie.IMDB) && string.Equals(plexMovie.IMDB, nfoIMDB, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    isMatch = true;
                                }

                                if (!string.IsNullOrWhiteSpace(plexMovie.TMDB) && string.Equals(plexMovie.TMDB, nfoTMDB, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    isMatch = true;
                                }
                                if (!isMatch)
                                {
                                    string misMatch = string.Empty;
                                    if (!string.IsNullOrWhiteSpace(nfoIMDB))
                                    {
                                        misMatch = "imdb:" + nfoIMDB;
                                    }
                                    else if (!string.IsNullOrWhiteSpace(nfoTMDB))
                                    {
                                        misMatch = "tmdb:" + nfoTMDB;
                                    }

                                    Console.WriteLine(plexMovie.Title + " (" + plexMovie.Year + ")" + ",Incorrect NFO information (" + misMatch + "),imdb:" + plexMovie.IMDB + ",tmdb:" + plexMovie.TMDB + "," + plexMovie.FullFileName);
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }
            }

            #endregion

            #region Check for movies that should be duplicates, but are not in Plex

            var duplicates = from c in _plexMovies
                             join c2 in _plexMovies on c.FileInfo.Name equals c2.FileInfo.Name
                             where c.Title != c2.Title || c.IMDB != c2.IMDB || c.TMDB != c2.TMDB
                             select c;

            foreach (var plexMovie in duplicates)
            {
                Console.WriteLine(plexMovie.Title + " (" + plexMovie.Year + ")" + ",Should Be Duplicate But Is Not," + plexMovie.IMDB + "," + plexMovie.TMDB + "," + plexMovie.FullFileName);
            }

            #endregion

            #region Check for folder name collisions

            var collisions = from c in _plexMovies
                             join c2 in _plexMovies on c.MovieFolderName equals c2.MovieFolderName
                             where c != c2
                             select c;

            foreach (var plexMovie in collisions)
            {
                Console.WriteLine(plexMovie.Title + " (" + plexMovie.Year + ")" + ",Folder Name Collision," + plexMovie.IMDB + "," + plexMovie.TMDB + "," + plexMovie.FullFileName);
            }

            #endregion

            #region Check for items on Disk that are not in the Plex Movie Library. Check for Empty Directories. Check for Directories with multiple movies.

            foreach (var directoryInfo in targetDirectoryInfo.EnumerateDirectories())
            {
                //Check for items on Disk that are not in the Plex Movie Library.
                if (!_plexMovies.Any(p => p.FullFileName.StartsWith(directoryInfo.FullName)))
                {
                    Console.WriteLine(directoryInfo.FullName + ",Missing From Plex");
                }

                //Check for Directories with Subdirectories
                if (directoryInfo.EnumerateDirectories().Any())
                {
                    Console.WriteLine(directoryInfo.FullName + ",Folder Has Subdirectories");
                }

                var allMovies = directoryInfo.GetFiles("*" + "." + "mkv", System.IO.SearchOption.AllDirectories).ToList();
                allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "mp4", System.IO.SearchOption.AllDirectories).ToList());
                allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "avi", System.IO.SearchOption.AllDirectories).ToList());
                allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "m4v", System.IO.SearchOption.AllDirectories).ToList());
                allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "mpeg", System.IO.SearchOption.AllDirectories).ToList());
                allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "mpg", System.IO.SearchOption.AllDirectories).ToList());
                allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "wmv", System.IO.SearchOption.AllDirectories).ToList());

                //Check for Empty Directories.
                if (allMovies.Count == 0)
                {
                    Console.WriteLine(directoryInfo.FullName + ",Folder Is Empty");
                }

                //Check for Directories with multiple movies.
                if (allMovies.Count > 1)
                {
                    Console.WriteLine(directoryInfo.FullName + ",Folder Has Multiple Movies");
                }
            }

            #endregion

            int i = 0;

            Console.WriteLine("Done");

            Console.ReadLine();

            return;

            foreach (var plexMovie in _plexMovies)
            {
                try
                {
                    //filter out other drives
                    if (!plexMovie.FileInfo.Directory.FullName.StartsWith(_targetDrive))
                    {
                        continue;
                    }

                    xmldoc = new XmlDataDocument();

                    var xmlPath = string.Format(@"C:\Users\Brad\AppData\Local\Plex Media Server\Metadata\Movies\{0}\{1}.bundle\Contents\_combined\Info.xml", plexMovie.Hash.Substring(0, 1), plexMovie.Hash.Substring(1));

                    if (!File.Exists(xmlPath))
                    {
                        Console.WriteLine("XML File Not Found " + plexMovie.FullFileName);
                        continue;
                    }

                    var sb = new StringBuilder();
                    sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?>");
                    sb.AppendLine("<movie>");
                    FileStream fs = new FileStream(xmlPath, FileMode.Open, FileAccess.Read);
                    xmldoc.Load(fs);

                    if (!string.IsNullOrWhiteSpace(plexMovie.Title))
                    {
                        sb.AppendLine("<title>" + WebUtility.HtmlEncode(plexMovie.Title.Replace("\"", "")) + "</title>");
                    }
                    if (!string.IsNullOrWhiteSpace(plexMovie.Title))
                    {
                        sb.AppendLine("<sorttitle>" + WebUtility.HtmlEncode(plexMovie.Title.Replace("\"", "")) + "</sorttitle>");
                    }
                    if (plexMovie.Year > 0)
                    {
                        sb.AppendLine("<year>" + plexMovie.Year + "</year>");
                    }
                    if (!string.IsNullOrWhiteSpace(plexMovie.IMDB))
                    {
                        sb.AppendLine("<uniqueid type=\"imdb\" default=\"true\">" + WebUtility.HtmlEncode(plexMovie.IMDB) + "</uniqueid>");
                    }
                    else if (!string.IsNullOrWhiteSpace(plexMovie.TMDB))
                    {
                        sb.AppendLine("<uniqueid type=\"tmdb\" default=\"true\">" + WebUtility.HtmlEncode(plexMovie.TMDB) + "</uniqueid>");
                    }


                    xmlnode = xmldoc.GetElementsByTagName("genres");
                    if (xmlnode.Count == 1)
                    {
                        var firstNode = xmlnode[0];

                        if (firstNode.ChildNodes.Count > 0)
                        {
                            for (var t = 0; t < firstNode.ChildNodes.Count; t++)
                            {
                                var node = firstNode.ChildNodes[t];
                                sb.AppendLine("<genre>" + WebUtility.HtmlEncode(node.InnerText) + "</genre>");
                            }
                        }
                    }
                    xmlnode = xmldoc.GetElementsByTagName("countries");
                    if (xmlnode.Count == 1)
                    {
                        var firstNode = xmlnode[0];

                        if (firstNode.ChildNodes.Count > 0)
                        {
                            for (var t = 0; t < firstNode.ChildNodes.Count; t++)
                            {
                                var node = firstNode.ChildNodes[t];
                                sb.AppendLine("<country>" + WebUtility.HtmlEncode(node.InnerText) + "</country>");
                            }
                        }
                    }
                    xmlnode = xmldoc.GetElementsByTagName("summary");
                    if (xmlnode.Count == 1)
                    {
                        var firstNode = xmlnode[0];

                        var node = firstNode.ChildNodes[0];
                        sb.AppendLine("<plot>" + WebUtility.HtmlEncode(firstNode.InnerText) + "</plot>");
                    }
                    xmlnode = xmldoc.GetElementsByTagName("tagline");
                    if (xmlnode.Count == 1)
                    {
                        var firstNode = xmlnode[0];

                        var node = firstNode.ChildNodes[0];
                        sb.AppendLine("<tagline>" + WebUtility.HtmlEncode(firstNode.InnerText) + "</tagline>");
                    }
                    xmlnode = xmldoc.GetElementsByTagName("studio");
                    if (xmlnode.Count == 1)
                    {
                        var firstNode = xmlnode[0];

                        var node = firstNode.ChildNodes[0];
                        sb.AppendLine("<studio>" + WebUtility.HtmlEncode(firstNode.InnerText) + "</studio>");
                    }

                    sb.Append("</movie>");
                    var destination = plexMovie.FileInfo.FullName.Substring(0, plexMovie.FileInfo.FullName.Length - plexMovie.FileInfo.Extension.Length) + ".nfo";
                    //
                    if (CanMove && !File.Exists(destination))
                    {
                        try
                        {
                            File.WriteAllText(destination, sb.ToString());

                            Console.WriteLine("Done: " + plexMovie.FullFileName);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            Console.WriteLine("Done");

            Console.ReadLine();
        }
    }
}
