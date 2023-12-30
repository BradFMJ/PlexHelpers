using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;


namespace PlexHelpers.TTGrabber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(@"C:\Share\H\plex-rarbg-season-packs-filtered.csv");

            var total = lines.Length;

            for (var i = 0; i < lines.Length; i++)
            {
                TextFieldParser parser = new TextFieldParser(new StringReader(lines[i]));
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                string[] parts = null;

                while (!parser.EndOfData)
                {
                    parts = parser.ReadFields();
                }

                if(File.Exists(parts[1] + "\\tvshow.nfo"))
                {
                    try
                    {
                        var tvshowinfo = File.ReadAllText(parts[1] + "\\tvshow.nfo");

                        Regex re = new Regex(@"tt\d+");
                        if (re.IsMatch(tvshowinfo))
                        {
                            var match = re.Match(tvshowinfo);
                            lines[i] = match.Value + "," + lines[i];
                            Console.WriteLine(i + " of " + total + " match " + match.Value);
                        }
                        else
                        {
                            lines[i] = "," + lines[i];
                            Console.WriteLine(i + " of " + total);
                        }
                    }
                    catch(Exception ex)
                    {
                        lines[i] = "," + lines[i];
                        Console.WriteLine(i + " of " + total);
                    }

                }
                else
                {
                    lines[i] = "," + lines[i];
                    Console.WriteLine(i + " of " + total);
                }
            }

            File.WriteAllLines(@"C:\Share\H\plex-rarbg-season-packs-filtered-output.csv", lines);
        }
    }
}
