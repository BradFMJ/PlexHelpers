using System;
using System.IO;
using System.Text.RegularExpressions;

namespace PlexHelpers.Common.Renaming
{
    public static class ChangeSeason
    {
        public static void Move(DirectoryInfo directoryInfo, string targetSeason, bool canMove)
        {
            var tvShows = directoryInfo.GetFilesByExtensions(Settings.VideoFileExtensions.ToArray());
            foreach (var tvShow in tvShows)
            {
                var match = Regex.Match(tvShow.Name, @"[Ss](\d+)[Ee](\d+)");
                if (!match.Success)
                {
                    continue;
                }

                string season = match.Groups[1].Value.PadLeft(2, '0');
                string episode = match.Groups[2].Value.PadLeft(2, '0');


                string destination = string.Format("S{0}E{1}", targetSeason, episode);
                string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                newFileName = newFileName.Replace(" ", "").Replace("-", "");

                Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName), canMove);
            }
        }

        private static bool Move(string sourceFileName, string destFileName, bool canMove)
        {
            if (string.Equals(sourceFileName, destFileName, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (File.Exists(destFileName))
            {
                Console.WriteLine("ERROR: Cannot move file {0}", destFileName);
                return false;
            }

            Console.WriteLine("RENAMING {0} to {1}", sourceFileName, destFileName);

            if (canMove)
            {
                File.Move(sourceFileName, destFileName);
            }

            return true;
        }
    }
}
