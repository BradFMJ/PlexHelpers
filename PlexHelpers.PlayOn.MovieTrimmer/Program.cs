using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexHelpers.PlayOn.MovieTrimmer
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo dInfo = new DirectoryInfo(@"C:\Users\Brad\Videos\PlayOn\Xfinity");
            foreach (var directoryInfo in dInfo.EnumerateDirectories())
            {
                SplitIntoChapters(directoryInfo);
            }
        }

        static void TrimMovies2(DirectoryInfo dInfo)
        {
            foreach (var directoryInfo in dInfo.EnumerateDirectories())
            {
                TrimMovies2(directoryInfo);
            }

            var recordings = dInfo.GetFilesByExtensions(".mp4");

            foreach (var recording in recordings)
            {
                var outputFileFullName = $"H:\\Media\\Completed\\{recording.Name}";

                if (!File.Exists(outputFileFullName))
                {
                    var proc = new Process();
                    proc.StartInfo.FileName = @"C:\Users\Brad\Videos\PlayOn\ffmpeg\bin\ffmpeg.exe ";
                    //proc.StartInfo.Arguments = $" -i \"{recording.FullName}\" -ss 4.5 -vcodec copy -acodec copy \"{outputFileFullName}\"";
                    proc.StartInfo.Arguments = $" -i \"{recording.FullName}\" -t 00:44:02 -vcodec copy -acodec copy \"{outputFileFullName}\"";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.Start();
                    string outPut = proc.StandardOutput.ReadToEnd();

                    proc.WaitForExit();
                    var exitCode = proc.ExitCode;
                    if (exitCode == 0)
                    {
                        //File.Delete(recording.FullName);
                    }
                    else
                    {
                        int i = 0;
                    }
                    proc.Close();
                }
                else
                {
                    int i = 0;
                }
            }
        }

        static void SplitIntoChapters(DirectoryInfo dInfo)
        {
            foreach (var directoryInfo in dInfo.EnumerateDirectories())
            {
                SplitIntoChapters(directoryInfo);
            }

            var recordings = dInfo.GetFilesByExtensions(".mp4");

            foreach (var recording in recordings)
            {
                var outputFileFullName = @"H:\Media\Completed\" + recording.Name;

                if(!File.Exists(outputFileFullName))
                {
                    var proc = new Process();
                    proc.StartInfo.FileName = @"C:\Program Files\MKVToolNix\mkvmerge.exe ";
                    proc.StartInfo.Arguments = $" -o \"{outputFileFullName}\" --split chapters:all \"{recording.FullName}\"";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.Start();
                    string outPut = proc.StandardOutput.ReadToEnd();

                    proc.WaitForExit();
                    var exitCode = proc.ExitCode;
                    if (exitCode == 0)
                    {
                        //File.Delete(recording.FullName);
                    }
                    else
                    {
                        int i = 0;
                    }
                    proc.Close();
                }
                else
                {
                    int i = 0;
                }
            }
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
