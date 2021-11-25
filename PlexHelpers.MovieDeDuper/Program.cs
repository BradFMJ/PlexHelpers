using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlexHelpers.MovieDeDuper
{
    class Program
    {
        private static bool CanDelete = true;
        private static bool IsInteractive = true;
        private static List<PlexMovie> _plexMovies;

        static void Main(string[] args)
        {
            _plexMovies = Helpers.ReadCSV("C:\\imdb\\rarbgcheck.csv");

            var sourceRootDir = "J" + @":\Media\Movies";

            //filter out movies that are not in their own folders (sitting in the root directory)
            _plexMovies = _plexMovies.Where(p => p.FileInfo.Directory.FullName != sourceRootDir).ToList();

            var duplicates = _plexMovies.GroupBy(p => p.MetadataId).Select(g => new { MetadataId = g.Key, Count = g.Count() }).Where(p => p.Count > 1).ToList();

            int count = 0;
            int total = duplicates.Count;

            foreach (var duplicate in duplicates)
            {
                try
                {
                    count++;

                    var versions = _plexMovies.Where(p => p.MetadataId == duplicate.MetadataId).OrderBy(p => p.Height).ToList();

                    if (versions.Count != 2)
                    {
                        continue;
                    }

                    var durationDifference = versions[0].Duration - versions[1].Duration;
                    if (durationDifference < 0)
                    {
                        durationDifference = durationDifference * -1;
                    }

                    //five minutes in milliseconds
                    if (durationDifference > (5 * 60 * 1000))
                    {
                        //More than 5 minutes (credits/intro cut off maybe), manually check/watch movies to make sure they are the same movie
                        Console.WriteLine("SKIPPING (TIME DIFFERENCE) {0} of {1}: {2}", count, total, versions[0].MovieFolderName);
                        Console.WriteLine("-------------------------------------------------");
                        Console.WriteLine("-------------------------------------------------");
                        continue;
                    }

                    //look for the version with a preferred release group and 1080p
                    var matches = versions.Where(e => Settings.ReleaseGroups.Any(p => e.FileInfo.Name.ToLowerInvariant().Contains(p)) && e.FileInfo.Name.Contains("1080")).ToList();
                    if (matches.Count == 1)
                    {
                        //pick the the version to be saved
                        var toBeSaved = matches.First();
                        //pick the the version that was not matched to be deleted
                        var toBeRemoved = versions.Single(p => p.FullFileName != toBeSaved.FullFileName);
                        //make sure we actually still have 2 copies on disk
                        if (Directory.Exists(toBeRemoved.FileInfo.DirectoryName) && Directory.Exists(toBeSaved.FileInfo.DirectoryName))
                        {
                            var directoryInfo = new DirectoryInfo(toBeRemoved.FileInfo.DirectoryName);

                            //Make sure there are no subfolders in the folder we are about to delete
                            if (directoryInfo.GetDirectories().Length > 0)
                            {
                                Console.WriteLine("SKIPPING (FOLDER HAS SUBFOLDERS!!!) {0} of {1}: {2}", count, total, toBeRemoved.FileInfo.DirectoryName);
                                Console.WriteLine("-------------------------------------------------");
                                Console.WriteLine("-------------------------------------------------");

                                continue;
                            }

                            //Make sure there is only 1 video file in the folder we are about to delete
                            var allMovies = directoryInfo.GetFiles("*" + "." + "mkv", SearchOption.AllDirectories).ToList();
                            allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "mp4", SearchOption.AllDirectories).ToList());
                            allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "avi", SearchOption.AllDirectories).ToList());
                            allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "m4v", SearchOption.AllDirectories).ToList());
                            allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "mpeg", SearchOption.AllDirectories).ToList());
                            allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "mpg", System.IO.SearchOption.AllDirectories).ToList());
                            allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "wmv", SearchOption.AllDirectories).ToList());
                            if (allMovies.Count > 1)
                            {
                                Console.WriteLine("SKIPPING (FOLDER HAS MULTIPLE VIDEO FILES!!!) {0} of {1}: {2}", count, total, toBeRemoved.FileInfo.DirectoryName);
                                Console.WriteLine("-------------------------------------------------");
                                Console.WriteLine("-------------------------------------------------");

                                continue;
                            }

                            string input = string.Empty;
                            if (IsInteractive)
                            {
                                Console.WriteLine("KEEP   {0}, {1}", TimeSpan.FromMilliseconds(toBeSaved.Duration).ToString(), toBeSaved.FileInfo.Name);
                                Console.WriteLine("DELETE {0}, {1}", TimeSpan.FromMilliseconds(toBeRemoved.Duration).ToString(), toBeRemoved.FileInfo.Name);
                                Console.WriteLine("ENTER 'Y' TO DELETE");
                                input = Console.ReadLine().ToLowerInvariant();
                            }

                            if ((IsInteractive && input == "y") || !IsInteractive)
                            {
                                Console.WriteLine("DELETING {0} of {1}: {2}", count, total, toBeRemoved.FileInfo.DirectoryName);
                                if (CanDelete)
                                {
                                    //https://www.c-sharpcorner.com/blogs/extension-methods-for-delete-files-and-folders-to-recycle-bin
                                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(toBeRemoved.FileInfo.DirectoryName,
                                        Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                                        Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                                }

                                if (IsInteractive)
                                {
                                    Console.WriteLine("-------------------------------------------------");
                                    Console.WriteLine("-------------------------------------------------");
                                }
                            }

                            continue;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine("Done");

            Console.ReadLine();
        }
    }
}
