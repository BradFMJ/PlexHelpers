using PlexHelpers.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlexHelpers.NewshostingMover
{
    class Program
    {
        private static bool CanDelete = true;
        private static bool CanMove = true;

        static void Main(string[] args)
        {
            CleanCompletedFolder(@"C:\Users\bradf\Downloads\Newshosting\ToProcess");

            MoveFiles(new DirectoryInfo(@"C:\Users\bradf\Downloads\Newshosting\ToProcess"));

            CleanCompletedFolder(@"C:\Users\bradf\Downloads\Newshosting\ToProcess");
        }

        public static void MoveFiles(DirectoryInfo directoryInfo)
        {
            var tvShows = directoryInfo.GetFilesByExtensions(Settings.VideoFileExtensions.ToArray());
            foreach (var tvShow in tvShows)
            {
                string destFile = Path.Combine(@"C:\Users\bradf\Downloads\Newshosting", tvShow.Name);

                if (File.Exists(destFile))
                {
                    Console.WriteLine("Cannot move file" + destFile);
                    continue;
                }
                Move(tvShow.FullName, destFile);
            }
            var subDirs = directoryInfo.GetDirectories();

            foreach (var subDir in subDirs)
            {
                MoveFiles(subDir);
            }
        }

        private static bool Move(string sourceFileName, string destFileName)
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

            if (CanMove)
            {
                File.Move(sourceFileName, destFileName);
            }

            return true;
        }

        public static void CleanCompletedFolder(string path)
        {
            DirectoryInfo dInfo = new DirectoryInfo(path);

            List<FileInfo> files = dInfo.GetFiles("*.txt", SearchOption.AllDirectories).Where(p => p.Extension == ".txt").ToList();
            files.AddRange(dInfo.GetFiles("*.nfo", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".nfo").ToList());
            files.AddRange(dInfo.GetFiles("*.exe", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".exe").ToList());
            files.AddRange(dInfo.GetFiles("*.jpg", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".jpg").ToList());
            files.AddRange(dInfo.GetFiles("*.jpeg", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".jpeg").ToList());
            files.AddRange(dInfo.GetFiles("*.png", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".png").ToList());
            files.AddRange(dInfo.GetFiles("*.htm", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".htm").ToList());
            files.AddRange(dInfo.GetFiles("*.html", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".html").ToList());
            files.AddRange(dInfo.GetFiles("*.par2", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".par2").ToList());
            files.AddRange(dInfo.GetFiles("*.sfv", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".sfv").ToList());
            files.AddRange(dInfo.GetFiles("*.srr", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".srr").ToList());
            files.AddRange(dInfo.GetFiles("*.nzb", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".nzb").ToList());
            files.AddRange(dInfo.GetFiles("*.m3u", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".m3u").ToList());
            files.AddRange(dInfo.GetFiles("RARBG.COM.mp4", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".mp4").Where(p => p.Length < 104857600).ToList());
            files.AddRange(dInfo.GetFiles("RARBG.mp4", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".mp4").Where(p => p.Length < 104857600).ToList());
            files.AddRange(dInfo.GetFiles("RARBG.avi", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".avi").Where(p => p.Length < 104857600).ToList());
            files.AddRange(dInfo.GetFiles("RARBG.mkv", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".mkv").Where(p => p.Length < 104857600).ToList());

            files.AddRange(dInfo.GetFiles("*sample*", SearchOption.AllDirectories).Where(p => p.Length < 104857600).ToList());

            foreach (FileInfo file in files)
            {
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    Delete(file.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            var subDirs = dInfo.GetDirectories();
            foreach (var subDir in subDirs)
            {
                DeleteEmptyFolders(subDir);
            }
        }

        private static void Delete(string fullName)
        {
            Console.WriteLine("DELETING {0}", fullName);

            if (CanDelete)
            {
                File.Delete(fullName);
            }
        }

        private static void DeleteEmptyFolders(DirectoryInfo dInfo)
        {
            var subDirs = dInfo.GetDirectories();
            foreach (var subDir in subDirs)
            {
                DeleteEmptyFolders(subDir);
            }

            if (!dInfo.GetFiles().Any() && !dInfo.GetDirectories().Any())
            {
                dInfo.Delete();
            }
        }
    }
}
