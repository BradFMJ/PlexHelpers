using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexHelpers.DumbMover
{
    internal class Program2
    {
        private static string _sourceDrive = "I";
        private static string _sourceDrivePath = @":\Media\TV Shows Migrate";
        private static string _targetDrive = "I";
        private static string _targetDrivePath = @":\Media\TV Shows Migrate2";
        static void Main2(string[] args)
        {
            string[] tvShows = Directory.GetDirectories(_sourceDrive + _sourceDrivePath)
                            .Select(Path.GetFileName)
                            .ToArray();
            foreach (var tvShow in tvShows)
            {
                var targetDirectory = _targetDrive + _targetDrivePath + "\\" + tvShow;
                var testDirectory = "C:\\Share\\tvshows\\1080P\\" + tvShow;

                DirectoryInfo di2 = new DirectoryInfo(targetDirectory);
                if (di2.Exists)
                {
                    Console.WriteLine("CANNOT MOVE. Target Directory Exists. {0}", targetDirectory);
                    continue;
                }
                DirectoryInfo di3 = new DirectoryInfo(testDirectory);
                if (di3.Exists)
                {
                    Console.WriteLine("CANNOT MOVE. Target Directory Exists. {0}", testDirectory);
                    continue;
                }

                Directory.Move(_sourceDrive + _sourceDrivePath + "\\" + tvShow, targetDirectory);
            }

            Console.ReadLine();
        }
    }
}
