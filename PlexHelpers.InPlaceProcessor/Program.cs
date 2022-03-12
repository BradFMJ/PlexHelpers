using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlexHelpers.InPlaceProcessor
{
    class Program
    {
        private static bool CanMove = true;
        private static bool CanDelete = true;
        private static List<PlexMovie> _plexMovies;
        private static string _sourceDrive = "J";
        private static string _targetDrive = "B";
        private static string _tmdbDrive = "J";
        private static string _unmatchedDrive = "J";
        private static string _drivePath = @":\Media\Movies";

        static void Main(string[] args)
        {
            _plexMovies = Helpers.ReadPlexMovieCSV("C:\\imdb\\plex-movies.csv", Helpers.ReadPlexMapCSV("C:\\imdb\\plex-map.csv"));

            FileInfo fileInfo;

            var sourceRootDir = _sourceDrive + _drivePath;

            //filter out only movies that are in sourceRootDir. Also, filter out movies that are not in their own folders (sitting in the root directory)
            _plexMovies = _plexMovies.Where(p => p.FullFileName.StartsWith(sourceRootDir) && p.FileInfo.Directory.FullName != sourceRootDir).ToList();

            int count = 0;
            int total = _plexMovies.Count();

            foreach (var plexMovie in _plexMovies)
            {
                try
                {
                    count++;

                    //stop root drive copy
                    if (plexMovie.FileInfo.Directory.FullName == sourceRootDir)
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(plexMovie.IMDB))
                    {
                        //Check for incorrect folder names
                        //var sourceDirectory = _sourceDrive + _drivePath + "\\" + plexMovie.MovieFolderName;
                        var targetDirectory = _targetDrive + _drivePath + "\\" + plexMovie.MovieFolderName;

                        if (string.Equals(targetDirectory, plexMovie.FileInfo.DirectoryName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        //if (!plexMovie.FileInfo.Directory.FullName.ToLowerInvariant().StartsWith(targetDirectory.ToLowerInvariant()))
                        if (!string.Equals(plexMovie.FileInfo.Directory.FullName, targetDirectory, StringComparison.InvariantCultureIgnoreCase))
                        // if (!string.Equals(sourceDirectory, targetDirectory, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!Directory.Exists(targetDirectory))
                            {
                                Console.WriteLine("{0} of {1} Moving IMDB {2} to {3}", count, total, plexMovie.FileInfo.DirectoryName, targetDirectory);
                                if (CanMove)
                                {
                                    if (plexMovie.FileInfo.DirectoryName.StartsWith(_targetDrive))
                                    {
                                        Directory.Move(plexMovie.FileInfo.DirectoryName, targetDirectory);
                                    }
                                    else
                                    {
                                        DriveInfo driveInfo = new DriveInfo(_targetDrive);

                                        var freeSpace = driveInfo.TotalFreeSpace - 10737418240;
                                        if (freeSpace < plexMovie.Size)
                                        {
                                            Console.WriteLine("CANNOT MOVE IMDB {0} to {1}", plexMovie.FileInfo.DirectoryName, targetDirectory);
                                            continue;
                                        }

                                        Helpers.DirectoryCopy(plexMovie.FileInfo.DirectoryName, targetDirectory, true);
                                        if (CanDelete)
                                        {
                                            Directory.Delete(plexMovie.FileInfo.DirectoryName, true);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("CANNOT MOVE IMDB {0} to {1}", plexMovie.FileInfo.DirectoryName, targetDirectory);
                            }
                        }
                        else
                        {
                            Console.WriteLine("SKIPPING IMDB {0}", plexMovie.FileInfo.DirectoryName);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(plexMovie.TMDB))
                    {
                        DriveInfo driveInfo = new DriveInfo(_tmdbDrive);

                        var targetDirectory = _tmdbDrive + _drivePath + "\\" + plexMovie.MovieFolderName;

                        if (string.Equals(targetDirectory, plexMovie.FileInfo.DirectoryName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        var freeSpace = driveInfo.TotalFreeSpace - 10737418240;

                        if (freeSpace < plexMovie.Size)
                        {
                            Console.WriteLine("CANNOT MOVE TMDB {0} to {1}", plexMovie.FileInfo.DirectoryName, targetDirectory);
                            continue;
                        }

                        if (!Directory.Exists(targetDirectory))
                        {
                            Console.WriteLine("Moving TMDB {0} to {1}", plexMovie.FileInfo.DirectoryName, targetDirectory);
                            if (CanMove)
                            {
                                if (plexMovie.FileInfo.DirectoryName.StartsWith(_tmdbDrive))
                                {
                                    Directory.Move(plexMovie.FileInfo.DirectoryName, targetDirectory);
                                }
                                else
                                {
                                    Helpers.DirectoryCopy(plexMovie.FileInfo.DirectoryName, targetDirectory, true);
                                    if (CanDelete)
                                    {
                                        Directory.Delete(plexMovie.FileInfo.DirectoryName, true);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("CANNOT MOVE TMDB {0} to {1}", plexMovie.FileInfo.DirectoryName, targetDirectory);
                        }
                    }
                    else if (plexMovie.Uri != null && (plexMovie.Uri.Scheme == "local" || plexMovie.Uri.Scheme == "plex"))
                    {
                        DriveInfo driveInfo = new DriveInfo(_unmatchedDrive);

                        var targetDirectory = _unmatchedDrive + _drivePath + "\\" + plexMovie.MovieFolderName;

                        if(string.Equals(targetDirectory, plexMovie.FileInfo.DirectoryName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        var freeSpace = driveInfo.TotalFreeSpace - 10737418240;

                        if (freeSpace < plexMovie.Size)
                        {
                            Console.WriteLine("CANNOT MOVE local {0} to {1}", plexMovie.FileInfo.DirectoryName, targetDirectory);
                            continue;
                        }

                        if (!Directory.Exists(targetDirectory))
                        {
                            Console.WriteLine("Moving local {0} to {1}", plexMovie.FileInfo.DirectoryName, targetDirectory);
                            if (CanMove)
                            {
                                if (plexMovie.FileInfo.DirectoryName.StartsWith(_unmatchedDrive))
                                {
                                    Directory.Move(plexMovie.FileInfo.DirectoryName, targetDirectory);
                                }
                                else
                                {
                                    Helpers.DirectoryCopy(plexMovie.FileInfo.DirectoryName, targetDirectory, true);
                                    if (CanDelete)
                                    {
                                        Directory.Delete(plexMovie.FileInfo.DirectoryName, true);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("CANNOT MOVE local {0} to {1}", plexMovie.FileInfo.DirectoryName, targetDirectory);
                        }
                    }
                    else
                    {
                        Console.WriteLine("SKIPPING {0}", plexMovie.FileInfo.DirectoryName);
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
