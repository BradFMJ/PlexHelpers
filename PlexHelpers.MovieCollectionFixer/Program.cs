using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlexHelpers.MovieCollectionFixer
{
    class Program
    {

        private static List<PlexCollectionMovie> _plexImportListMovies = new List<PlexCollectionMovie>();

        static void Main(string[] args)
        {
            string path = @"C:\imdb\Lists\backup";
            DirectoryInfo dInfo = new DirectoryInfo(path);
            foreach (var fileInfo in dInfo.EnumerateFiles())
            {
                _plexImportListMovies.AddRange(Helpers.ReadCollectionCSV(fileInfo.FullName));
            }

            _plexImportListMovies = _plexImportListMovies.Distinct(new PlexCollectionMovieComparer()).ToList();
            _plexImportListMovies = _plexImportListMovies.Where(p => p.CollectionName != "In Theaters").ToList();

            File.WriteAllLines(@"C:\imdb\Lists\import.csv", _plexImportListMovies.Select(p => p.ToString()));
        }
    }
}
