using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlexHelpers.MovieCollectionMerge
{
    class Program
    {
        private static bool CanCopy = true;
        private static List<PlexMovie> _sourcePlexMovies;
        private static List<PlexMovie> _targetPlexMovies;
        private static string _targetDrive = "B";
        private static string _drivePath = @":\Media\Movies";

        static void Main(string[] args)
        {
            //_sourcePlexMovies = Helpers.ReadPlexMovieCSV("C:\\imdb\\plex-movies.csv", Helpers.ReadPlexMapCSV("C:\\imdb\\plex-map.csv"));
            //_targetPlexMovies = Helpers.ReadPlexMovieCSV("C:\\imdb\\NKplex2.csv", Helpers.ReadPlexMapCSV("C:\\imdb\\plex-map.csv"));

            _sourcePlexMovies = Helpers.ReadPlexMovieCSV("C:\\imdb\\plex-movies.csv");
            _targetPlexMovies = Helpers.ReadPlexMovieCSV("C:\\Share\\H\\plex-movies.csv");

            var sourceRootDir = "J" + _drivePath;

            int count = 0;
            int total = _sourcePlexMovies.Count();
            int totalCopied = 0;
            long usedSpace = 0;

            foreach (var sourcePlexMovie in _sourcePlexMovies)
            {
                try
                {
                    count++;

                    //stop root drive copy
                    if (sourcePlexMovie.FileInfo.Directory.FullName == sourceRootDir)
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(sourcePlexMovie.IMDB))
                    {
                        //Check for incorrect folder names
                        //var sourceDirectory = _sourceDrive + _drivePath + "\\" + plexMovie.MovieFolderName;
                        var targetDirectory = _targetDrive + _drivePath + "\\" + sourcePlexMovie.MovieFolderName;

                        if (string.Equals(targetDirectory, sourcePlexMovie.FileInfo.DirectoryName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        var targetMovie = _targetPlexMovies.Where(p => p.FileInfo.Name == sourcePlexMovie.FileInfo.Name)
                            .ToList();

                        if (targetMovie.Count != 0)
                        {
                            Console.WriteLine("{0} of {1} CANNOT COPY IMDB (FILE ALREADY EXISTS) {2} to {3}", count, total, sourcePlexMovie.FileInfo.DirectoryName, targetDirectory);
                            continue;
                        }

                        //targetMovie = _targetPlexMovies.Where(p => p.MovieFolderName == sourcePlexMovie.MovieFolderName)
                        //    .ToList();

                        //if (targetMovie.Count != 0)
                        //{
                        //    Console.WriteLine("{0} of {1} CANNOT COPY IMDB (ANOTHER VERSION ALREADY EXISTS) {2} to {3}", count, total, sourcePlexMovie.FileInfo.DirectoryName, targetDirectory);
                        //    continue;
                        //}

                        if (!string.Equals(sourcePlexMovie.FileInfo.Directory.FullName, targetDirectory, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!Directory.Exists(targetDirectory))
                            {
                                totalCopied++;
                                usedSpace += sourcePlexMovie.Size;
                                Console.WriteLine("{0} of {1} Copying IMDB {2} to {3}", count, total, sourcePlexMovie.FileInfo.DirectoryName, targetDirectory);
                                if (CanCopy)
                                {
                                    DriveInfo driveInfo = new DriveInfo(_targetDrive);

                                    var freeSpace = driveInfo.TotalFreeSpace - 1073741824;
                                    if (freeSpace < sourcePlexMovie.Size)
                                    {
                                        Console.WriteLine("CANNOT COPY IMDB {0} to {1}", sourcePlexMovie.FileInfo.DirectoryName, targetDirectory);
                                        continue;
                                    }

                                    Helpers.DirectoryCopy(sourcePlexMovie.FileInfo.DirectoryName, targetDirectory, true);
                                }
                            }
                            else
                            {
                                Console.WriteLine("{0} of {1} CANNOT COPY IMDB (DIRECTORY ALREADY EXISTS) {2} to {3}", count, total, sourcePlexMovie.FileInfo.DirectoryName, targetDirectory);
                            }
                        }
                        else
                        {
                            Console.WriteLine("{0} of {1} SKIPPING IMDB {2}", count, total, sourcePlexMovie.FileInfo.DirectoryName);
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
