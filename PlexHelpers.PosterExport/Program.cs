using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml;

namespace PlexHelpers.PosterExport
{
    class Program
    {
        private static bool CanMove = true;
        private static List<PlexMovie> _plexMovies;
        private static string _targetDrive = "V";

        static void Main(string[] args)
        {
            _plexMovies = Helpers.ReadCSV("C:\\imdb\\rarbgcheck.csv");

            XmlDataDocument xmldoc;
            XmlNodeList xmlnode;
            FileInfo fileInfo;

            foreach (var plexMovie in _plexMovies)
            {
                try
                {
                    fileInfo = new FileInfo(plexMovie.FullFileName);

                    //filter out other drives
                    if (!string.IsNullOrWhiteSpace(_targetDrive) && !fileInfo.Directory.FullName.StartsWith(_targetDrive))
                    {
                        continue;
                    }

                    xmldoc = new XmlDataDocument();

                    var xmlPath = string.Format(@"C:\Users\Brad\AppData\Local\Plex Media Server\Metadata\Movies\{0}\{1}.bundle\Contents\_combined\Info.xml", plexMovie.Hash.Substring(0, 1), plexMovie.Hash.Substring(1));

                    if (!File.Exists(xmlPath))
                    {
                        Console.WriteLine("XML File Not Found");
                        continue;
                    }

                    FileStream fs = new FileStream(xmlPath, FileMode.Open, FileAccess.Read);
                    xmldoc.Load(fs);
                    xmlnode = xmldoc.GetElementsByTagName("posters");
                    if (xmlnode.Count > 1)
                    {
                        Console.WriteLine("too many poster folders");
                    }
                    if (xmlnode.Count < 1)
                    {
                        Console.WriteLine("No Posters Found");
                        continue;
                    }

                    var posterNode = xmlnode[0];

                    if (posterNode.ChildNodes.Count > 0)
                    {
                        for (var t = 0; t < posterNode.ChildNodes.Count; t++)
                        {
                            var poster = posterNode.ChildNodes[t];

                            if ((poster.Attributes["sort_order"] != null && poster.Attributes["sort_order"].Value == "1") || (poster.Attributes["external"] != null && poster.Attributes["external"].Value == "True"))
                            {
                                if (poster.Attributes["media"] != null)
                                {
                                    var source = string.Format(@"C:\Users\Brad\AppData\Local\Plex Media Server\Metadata\Movies\{0}\{1}.bundle\Contents\_combined\posters\{2}", plexMovie.Hash.Substring(0, 1), plexMovie.Hash.Substring(1), poster.Attributes["media"].Value);
                                    var extension = DetectExtension(source);
                                    var destination = fileInfo.FullName.Substring(0, fileInfo.FullName.Length - fileInfo.Extension.Length) + "." + extension;
                                    var destination2 = fileInfo.Directory.FullName + @"\poster." + extension;

                                    Copy(source, destination);
                                    Copy(source, destination2);
                                    break;
                                }
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine("No Posters Found");
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

        private static bool Copy(string sourceFileName, string destFileName)
        {
            if (string.Equals(sourceFileName, destFileName))
            {
                return true;
            }

            if (!File.Exists(sourceFileName))
            {
                Console.WriteLine("ERROR: Cannot move file {0}", destFileName);
                return false;
            }

            //if (File.Exists(destFileName))
            //{
            //    Console.WriteLine("ERROR: Cannot move file {0}", destFileName);
            //    return false;
            //}

            Console.WriteLine("COPYING {0} to {1}", sourceFileName, destFileName);

            if (CanMove)
            {
                try
                {
                    File.Copy(sourceFileName, destFileName, true);
                }
                catch (Exception e)
                {

                }
            }

            return true;
        }

        private static string DetectExtension(string sourceFileName)
        {
            string result = "jpg";

            var fileStream = File.ReadAllBytes(sourceFileName);

            Image image = null;
            using (MemoryStream stream = new MemoryStream(fileStream))
            {
                image = Image.FromStream(stream);

                if (ImageFormat.Jpeg.Equals(image.RawFormat))
                {
                    result = "jpg";
                }
                else if (ImageFormat.Png.Equals(image.RawFormat))
                {
                    result = "png";
                }
                else if (ImageFormat.Gif.Equals(image.RawFormat))
                {
                    result = "gif";
                }
            }

            return result;

        }
    }
}
