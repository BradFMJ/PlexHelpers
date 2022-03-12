using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlexHelpers.DriveCopy
{
    class Program
    {
        private static bool CanMove = true;
        private static List<PlexMovie> _plexMovies;
        private static string _sourceDrive = "Q";
        private static string _targetDrive = "N";

        static void Main(string[] args)
        {
            _plexMovies = Helpers.ReadPlexMovieCSV("C:\\imdb\\plex-movies.csv", Helpers.ReadPlexMapCSV("C:\\imdb\\plex-map.csv"));

            FileInfo fileInfo;

            var sourceRootDir = _sourceDrive + ":\\Media\\Movies";

            int counter = 1;

            List<string> alreadyCopiedMovies = File.ReadAllLines("C:\\imdb\\filecopylog.txt").OfType<string>().ToList();

            foreach (var plexMovie in _plexMovies)
            {
                try
                {
                    fileInfo = new FileInfo(plexMovie.FullFileName);

                    //filter out other source drives
                    if (!fileInfo.Directory.FullName.StartsWith(_sourceDrive))
                    {
                        continue;
                    }

                    //stop root drive copy
                    if (fileInfo.Directory.FullName == sourceRootDir)
                    {
                        continue;
                    }

                    var movieFolderName = Helpers.ReplaceInvalidFilePathChars(plexMovie.Title);
                    if (plexMovie.Year.HasValue && plexMovie.Year.Value > 1900)
                    {
                        movieFolderName = movieFolderName + " (" + plexMovie.Year + ")";
                    }

                    if (alreadyCopiedMovies.Contains(movieFolderName))
                    {
                        continue;
                    }

                    var targetDrive = _targetDrive;

                    if (string.IsNullOrWhiteSpace(plexMovie.IMDB) && !string.IsNullOrWhiteSpace(plexMovie.TMDB))
                    {
                        targetDrive = "J";
                    }

                    DriveInfo driveInfo = new DriveInfo(targetDrive);

                    var freeSpace = driveInfo.TotalFreeSpace - 10737418240;

                    if (freeSpace < plexMovie.Size)
                    {
                        break;
                    }

                    var targetDirectory = targetDrive + ":\\Media\\Movies\\" + movieFolderName;

                    Console.WriteLine(counter + ": Creating " + targetDirectory);

                    if (!Directory.Exists(targetDirectory))
                    {
                        if(CanMove)
                        {
                            Helpers.DirectoryCopy(fileInfo.Directory.FullName, targetDirectory, true);

                            File.AppendAllLines("C:\\imdb\\filecopylog.txt", new List<string> { movieFolderName });
                        }
                    }
                    else
                    {
                        Console.WriteLine(counter + ": ERROR " + targetDirectory);
                    }

                    counter++;

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
