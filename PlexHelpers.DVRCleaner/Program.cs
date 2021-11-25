using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PlexHelpers.DVRCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo dInfo = new DirectoryInfo(@"M:\Media\DVR TV Shows");
            foreach (var directoryInfo in dInfo.EnumerateDirectories())
            {
                if(directoryInfo.Name !=".grab")
                {
                    FixDVRShows(directoryInfo);
                }
            }
            dInfo = new DirectoryInfo(@"M:\Media\Recorded TV Shows");
            foreach (var directoryInfo in dInfo.EnumerateDirectories())
            {
                if (directoryInfo.Name != ".grab")
                {
                    FixDVRShows(directoryInfo);
                }
            }
        }

        static void FixDVRShows(DirectoryInfo dInfo)
        {
            foreach (var directoryInfo in dInfo.EnumerateDirectories())
            {
                FixDVRShows(directoryInfo);
            }

            if(dInfo.FullName.Contains("Football") || dInfo.FullName.Contains("Futbol") || dInfo.FullName.Contains("Fútbol") || dInfo.FullName.Contains("NHL") || dInfo.FullName.Contains("MLB") || dInfo.FullName.Contains("IndyCar"))
            {
                return;
            }

            var recordings = dInfo.GetFilesByExtensions(".ts");

            foreach (var recording in recordings)
            {
                try
                {
                    //if ((DateTime.UtcNow - recording.CreationTimeUtc).TotalHours <= 23)
                    //{
                    //    continue;
                    //}
                    if ((DateTime.UtcNow - recording.CreationTimeUtc).TotalMinutes <= 47)
                    {
                        continue;
                    }


                    var outputFileFullName = recording.FullName.Substring(0, recording.FullName.Length - 2) + "mp4";

                    var fileInfo = new FileInfo(outputFileFullName);

                    if (!fileInfo.Exists)
                    {
                        var proc = new Process();
                        proc.StartInfo.FileName = @"C:\Users\Brad\Videos\PlayOn\ffmpeg\bin\ffmpeg.exe ";
                        proc.StartInfo.Arguments = $" -i \"{recording.FullName}\" -c:v libx264 -c:a aac \"{outputFileFullName}\"";
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.Start();
                        string outPut = proc.StandardOutput.ReadToEnd();

                        proc.WaitForExit();
                        var exitCode = proc.ExitCode;
                        if (exitCode != 0)
                        {
                            int i = 0;
                        }
                        proc.Close();
                    }
                    else
                    {
                        if ((DateTime.UtcNow - fileInfo.CreationTimeUtc).TotalHours >= 23)
                        {
                            File.Delete(recording.FullName);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }


            //TODO: Kick of library scan
        }
    }

    public static class ExtensionMethods
    {
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException("extensions");
            }
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension.ToLower()));
        }
    }
}
