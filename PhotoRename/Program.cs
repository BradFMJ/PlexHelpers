using NExifTool;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhotoRename
{
    internal class Program
    {
        private static bool CanUpdateInfo = true;
        private static bool CanMove = true;
        private static string CurrentPile = "orange4_";

        static async Task Main(string[] args)
        {
            CreateMonths(new DirectoryInfo(@"C:\Users\bradf\OneDrive\Pictures\FastFoto"));
            await RenameFiles(new DirectoryInfo(@"C:\Users\bradf\OneDrive\Pictures\FastFoto"));
            //await UpdateInfo(new DirectoryInfo(@"C:\Users\bradf\OneDrive\Pictures\FastFoto"));

            Console.WriteLine("Done");

            Console.ReadLine();
        }

        public static void CreateMonths(DirectoryInfo directoryInfo)
        {
            foreach (var directory in directoryInfo.GetDirectories())
            {
                if (directory.Name.Length == 4)
                {
                    var monthDirectories = directory.GetDirectories();
                    if (!monthDirectories.Any(p => p.Name == "January"))
                    {
                        directory.CreateSubdirectory("January");
                    }
                    if (!monthDirectories.Any(p => p.Name == "February"))
                    {
                        directory.CreateSubdirectory("February");
                    }
                    if (!monthDirectories.Any(p => p.Name == "March"))
                    {
                        directory.CreateSubdirectory("March");
                    }
                    if (!monthDirectories.Any(p => p.Name == "April"))
                    {
                        directory.CreateSubdirectory("April");
                    }
                    if (!monthDirectories.Any(p => p.Name == "May"))
                    {
                        directory.CreateSubdirectory("May");
                    }
                    if (!monthDirectories.Any(p => p.Name == "June"))
                    {
                        directory.CreateSubdirectory("June");
                    }
                    if (!monthDirectories.Any(p => p.Name == "July"))
                    {
                        directory.CreateSubdirectory("July");
                    }
                    if (!monthDirectories.Any(p => p.Name == "August"))
                    {
                        directory.CreateSubdirectory("August");
                    }
                    if (!monthDirectories.Any(p => p.Name == "September"))
                    {
                        directory.CreateSubdirectory("September");
                    }
                    if (!monthDirectories.Any(p => p.Name == "October"))
                    {
                        directory.CreateSubdirectory("October");
                    }
                    if (!monthDirectories.Any(p => p.Name == "November"))
                    {
                        directory.CreateSubdirectory("November");
                    }
                    if (!monthDirectories.Any(p => p.Name == "December"))
                    {
                        directory.CreateSubdirectory("December");
                    }
                }
            }
        }

        public static async Task RenameFiles(DirectoryInfo directoryInfo)
        {
            foreach (var directory in directoryInfo.GetDirectories())
            {
                await RenameFiles(directory);
            }

            var photos = directoryInfo.GetFiles();
            foreach (var photo in photos)
            {

                //var destFileName2 = photo.FullName.Replace(photo.Extension, "_a" + photo.Extension);
                //Console.WriteLine("RENAMING {0} to {1}", photo.FullName, destFileName2);
                //if (CanMove)
                //{
                //    File.Move(photo.FullName, destFileName2);
                //}


                if (photo.Name.Contains("photo_"))
                {
                    var destFileName = photo.FullName.Replace("photo_", CurrentPile);
                    Console.WriteLine("RENAMING {0} to {1}", photo.FullName, destFileName);
                    if (CanMove)
                    {
                        File.Move(photo.FullName, destFileName);
                    }
                }
            }
        }

        public static async Task UpdateInfo(DirectoryInfo directoryInfo)
        {
            foreach (var directory in directoryInfo.GetDirectories())
            {
                await UpdateInfo(directory);
            }

            var photos = directoryInfo.GetFiles();
            foreach (var photo in photos)
            {
                //string pattern0 = @"^(19|20)\d{2}";
                //if (photo.Name.Contains(CurrentPile) || Regex.IsMatch(photo.Name, pattern0))
                //if (photo.Name.StartsWith("_NAX"))
                if (photo.Name.Contains(CurrentPile))
                {
                    Console.WriteLine("Checking DateTime for {0}", photo.FullName);

                    DateTime? newDate = null;
                    DateTime? existingDate = null;

                    int year;
                    int day = 1;
                    string pattern = @"^(?:[0][1-9]|[12][0-9]|3[01])$";
                    if (Regex.IsMatch(directoryInfo.Name, pattern))
                    {
                        int.TryParse(directoryInfo.Name, out day);
                        int.TryParse(directoryInfo.Parent.Parent.Name, out year);
                        newDate = new DateTime(year, DateTime.ParseExact(directoryInfo.Parent.Name, "MMMM", CultureInfo.CurrentCulture).Month, day);
                    }
                    else
                    {
                        if (int.TryParse(directoryInfo.Parent.Name, out year))
                        {
                            newDate = new DateTime(year, DateTime.ParseExact(directoryInfo.Name, "MMMM", CultureInfo.CurrentCulture).Month, day);
                        }
                        else if (int.TryParse(directoryInfo.Name, out year))
                        {
                            newDate = new DateTime(year, 1, day);
                        }
                    }


                    if (CanUpdateInfo && newDate.HasValue)
                    {
                        existingDate = null;
                        var et = new ExifTool(new ExifToolOptions());
                        var list = await et.GetTagsAsync(photo.FullName);
                        //var existing = list.FirstOrDefault(p => p.Id == "36868");
                        var existing = list.FirstOrDefault(p => p.Id == "36867");
                        DateTime parsedDate;
                        string format = "yyyy:MM:dd HH:mm:ss";
                        if (existing != null && DateTime.TryParseExact(existing.Value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                        {
                            existingDate = parsedDate;
                        }

                        if (existing == null || (existingDate.HasValue && (existingDate.Value.Year != newDate.Value.Year || existingDate.Value.Month != newDate.Value.Month)))
                        {
                            Console.WriteLine("Writing DateTime to {0}", photo.FullName);

                            ProcessStartInfo ExifTool = new ProcessStartInfo();
                            Process process = new Process();

                            ExifTool.FileName = AppDomain.CurrentDomain.BaseDirectory + @"\exiftool.exe";
                            ExifTool.Arguments = "-ModifyDate=\"" + newDate.Value.ToString("yyyy:MM:dd 00:00:00", CultureInfo.CurrentCulture) + "\"" + " " + "-DateTimeOriginal=\"" + newDate.Value.ToString("yyyy:MM:dd 00:00:00", CultureInfo.CurrentCulture) + "\"" + " " + "-CreateDate=\"" + newDate.Value.ToString("yyyy:MM:dd 00:00:00", CultureInfo.CurrentCulture) + "\"" + " " + photo.FullName;
                            ExifTool.UseShellExecute = false;
                            ExifTool.RedirectStandardOutput = true;
                            ExifTool.CreateNoWindow = true;
                            ExifTool.RedirectStandardError = true;
                            ExifTool.LoadUserProfile = true;

                            process.StartInfo = ExifTool;
                            process.Start();
                        }
                    }
                }
            }
        }

        public static async Task UpdateInfo2(DirectoryInfo directoryInfo)
        {
            foreach (var directory in directoryInfo.GetDirectories())
            {
                await UpdateInfo2(directory);
            }

            var photos = directoryInfo.GetFiles();
            foreach (var photo in photos)
            {
                if(photo.Extension != ".heic" && photo.Extension != ".jpeg" && photo.Extension != ".jpg")
                {
                    continue;
                }
                Console.WriteLine("Checking DateTime for {0}", photo.FullName);

                DateTime? newDate = null;
                DateTime? existingDate = null;
                int year;
                int month = 1;
                int day = 1;
                string pattern = @"(\d{4})(\d{2})(\d{2})";
                var match = Regex.Match(photo.Name, pattern);
                if (match.Success)
                {
                    int.TryParse(match.Groups[1].Value, out year);
                    int.TryParse(match.Groups[2].Value, out month);
                    int.TryParse(match.Groups[3].Value, out day);
                    newDate = new DateTime(year, month, day);
                }

                if (CanUpdateInfo && newDate.HasValue)
                {
                    existingDate = null;
                    var et = new ExifTool(new ExifToolOptions());
                    var list = await et.GetTagsAsync(photo.FullName);
                    //var existing = list.FirstOrDefault(p => p.Id == "36868");
                    var existing = list.FirstOrDefault(p => p.Id == "36867");
                    DateTime parsedDate;
                    string format = "yyyy:MM:dd HH:mm:ss";
                    if (existing != null && DateTime.TryParseExact(existing.Value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                    {
                        existingDate = parsedDate;
                    }

                    if (existing == null || (existingDate.HasValue && (existingDate.Value.Year != newDate.Value.Year || existingDate.Value.Month != newDate.Value.Month)))
                    {
                        Console.WriteLine("Writing DateTime to {0}", photo.FullName);

                        ProcessStartInfo ExifTool = new ProcessStartInfo();
                        Process process = new Process();

                        ExifTool.FileName = AppDomain.CurrentDomain.BaseDirectory + @"\exiftool.exe";
                        ExifTool.Arguments = "-ModifyDate=\"" + newDate.Value.ToString("yyyy:MM:dd 00:00:00", CultureInfo.CurrentCulture) + "\"" + " " + "-DateTimeOriginal=\"" + newDate.Value.ToString("yyyy:MM:dd 00:00:00", CultureInfo.CurrentCulture) + "\"" + " " + "-CreateDate=\"" + newDate.Value.ToString("yyyy:MM:dd 00:00:00", CultureInfo.CurrentCulture) + "\"" + " \"" + photo.FullName + "\"";
                        ExifTool.UseShellExecute = false;
                        ExifTool.RedirectStandardOutput = true;
                        ExifTool.CreateNoWindow = true;
                        ExifTool.RedirectStandardError = true;
                        ExifTool.LoadUserProfile = true;

                        process.StartInfo = ExifTool;
                        process.Start();
                    }
                }
            }
        }
    }
}
