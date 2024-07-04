using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexHelpers.DumbMover
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var tvShows = File.ReadAllLines(@"C:\imdb\tv_show_move2.csv");

            for (var i = 0; i < tvShows.Length; i++)
            {
                TextFieldParser parser = new TextFieldParser(new StringReader(tvShows[i]));
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                string[] parts = null;

                while (!parser.EndOfData)
                {
                    parts = parser.ReadFields();
                }

                DirectoryInfo di1 = new DirectoryInfo(parts[0]);
                if (!di1.Exists)
                {
                    Console.WriteLine("CANNOT MOVE. Source Directory Does Not Exist. {0}", parts[0]);
                    continue;
                }

                DirectoryInfo di2 = new DirectoryInfo(parts[1]);
                if (di2.Exists)
                {
                    Console.WriteLine("CANNOT MOVE. Target Directory Exists. {0}", parts[1]);
                    continue;
                }
                DirectoryInfo di3 = new DirectoryInfo(parts[1]);
                if (di3.Exists)
                {
                    Console.WriteLine("CANNOT MOVE. Target Directory Exists. {0}", parts[1]);
                    continue;
                }

                Directory.Move(parts[0], parts[1]);
            }




            Console.ReadLine();
        }
    }
}
