using Microsoft.VisualBasic.FileIO;
using PlexHelpers.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using PlexHelpers.Common.Medusa;
using System.Linq;
using System.Dynamic;

namespace PlexHelpers.Common
{
    public static class Helpers
    {
        public static List<string> VideoFileExtensions = new List<string> { ".mkv", ".mp4", ".avi", ".m4v", ".mpeg", ".mpg", ".wmv" };

        public static List<PlexMetadDataItem> ReadPlexMetadDataItem(string filePath)
        {
            var newTVShows = new List<PlexMetadDataItem>();

            var tvShows = File.ReadAllLines(filePath);

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

                try
                {
                    newTVShows.Add(PlexMetadDataItem.Parse(parts));
                }
                catch (Exception e)
                {
                    int u = 0;
                }
            }

            return newTVShows;
        }

        public static List<PlexMovie> ReadPlexMovieCSV(string filePath)
        {
            var newMovies = new List<PlexMovie>();

            var movies = File.ReadAllLines(filePath);

            for (var i = 0; i < movies.Length; i++)
            {
                TextFieldParser parser = new TextFieldParser(new StringReader(movies[i]));
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                string[] parts = null;

                while (!parser.EndOfData)
                {
                    parts = parser.ReadFields();
                }

                try
                {
                    var plexMovie = new PlexMovie
                    {
                        Title = parts[0],
                        Container = parts[8].ToLowerInvariant(),
                        VideoCodec = parts[9].ToLowerInvariant(),
                        AudioCodec = parts[10].ToLowerInvariant(),
                        FullFileName = parts[12],
                        Hash = parts[13].ToLowerInvariant(),
                        IMDB = parts[16].ToLowerInvariant(),
                        TMDB = parts[17].ToLowerInvariant(),
                        Plex = parts[18].ToLowerInvariant()
                    };

                    plexMovie.CompareTitle = plexMovie.Title.Replace(",", "").Replace(":", "").Replace("'", "").Replace("-", "").Replace("  ", " ").ToLowerInvariant();

                    plexMovie.FileInfo = new FileInfo(plexMovie.FullFileName);

                    int parseInt;
                    if (int.TryParse(parts[1], out parseInt))
                    {
                        plexMovie.Year = parseInt;
                    }
                    long parseLong;
                    if (long.TryParse(parts[2], out parseLong))
                    {
                        plexMovie.Size = parseLong;
                    }
                    if (int.TryParse(parts[3], out parseInt))
                    {
                        plexMovie.Width = parseInt;
                    }
                    if (int.TryParse(parts[4], out parseInt))
                    {
                        plexMovie.Height = parseInt;
                    }
                    if (int.TryParse(parts[6], out parseInt))
                    {
                        plexMovie.BitRate = parseInt;
                    }
                    if (int.TryParse(parts[7], out parseInt))
                    {
                        plexMovie.AudioChannels = parseInt;
                    }
                    if (int.TryParse(parts[14], out parseInt))
                    {
                        plexMovie.MetadataId = parseInt;
                    }
                    if (long.TryParse(parts[15], out parseLong))
                    {
                        plexMovie.Duration = parseLong;
                    }

                    plexMovie.MovieFolderName = ReplaceInvalidFilePathChars(plexMovie.Title);
                    if (plexMovie.Year.HasValue && plexMovie.Year.Value > 1900)
                    {
                        plexMovie.MovieFolderName = plexMovie.MovieFolderName + " (" + plexMovie.Year + ")";
                    }

                    decimal parseDecimal;
                    if (decimal.TryParse(parts[5], out parseDecimal))
                    {
                        plexMovie.DisplayAspectRatio = parseDecimal;
                    }

                    Uri parseUri;

                    if (!string.IsNullOrWhiteSpace(parts[11]))
                    {
                        plexMovie.Guid = parts[11];
                        if (Uri.TryCreate(parts[11], UriKind.Absolute, out parseUri))
                        {
                            plexMovie.Uri = parseUri;

                            if (parseUri.Scheme == "com.plexapp.agents.imdb")
                            {
                                plexMovie.IMDB = parseUri.Host;
                            }
                            if (parseUri.Scheme == "com.plexapp.agents.themoviedb")
                            {
                                plexMovie.TMDB = parseUri.Host;
                            }
                            if (parseUri.Scheme == "plex")
                            {
                                plexMovie.Plex = parseUri.PathAndQuery.Replace(@"/", "");
                            }
                        }
                        else if (parts[11].StartsWith("tt"))
                        {
                            plexMovie.IMDB = parts[11];
                        }

                        //var map = plexMaps.FirstOrDefault(p => p.PlexGuid == plexMovie.Guid);
                        //if (map != null)
                        //{
                        //    if (!string.IsNullOrWhiteSpace(map.IMDB))
                        //    {
                        //        plexMovie.IMDB = map.IMDB;
                        //    }
                        //    if (!string.IsNullOrWhiteSpace(map.TMDB))
                        //    {
                        //        plexMovie.TMDB = map.TMDB;
                        //    }
                        //    if (!string.IsNullOrWhiteSpace(map.Plex))
                        //    {
                        //        plexMovie.Plex = map.Plex;
                        //    }
                        //}


                        if(!string.IsNullOrWhiteSpace(plexMovie.IMDB))

                        {
                            plexMovie.MovieFolderName += " {" + plexMovie.IMDB + "}";
                        }
                    }

                    newMovies.Add(plexMovie);
                }
                catch (Exception e)
                {
                    int u = 0;
                }
            }

            return newMovies;
        }

        public static void WritePlexMovieCSV(string filePath, List<PlexMovie> movies)
        {
            var lines = new List<string>();

            foreach (var movie in movies)
            {
                lines.Add(Helpers.EscapeCsvField(movie.Title)
                    + "," + (movie.Year.HasValue ? movie.Year.Value.ToString() : "")
                    + "," + movie.Size
                    + "," + movie.Width
                    + "," + movie.Height
                    + "," + movie.DisplayAspectRatio
                    + "," + movie.BitRate
                    + "," + movie.AudioChannels
                    + "," + Helpers.EscapeCsvField(movie.Container)
                    + "," + Helpers.EscapeCsvField(movie.VideoCodec)
                    + "," + Helpers.EscapeCsvField(movie.AudioCodec)
                    + "," + Helpers.EscapeCsvField(movie.Guid)
                    + "," + Helpers.EscapeCsvField(movie.FullFileName)
                    + "," + Helpers.EscapeCsvField(movie.Hash)
                    + "," + movie.MetadataId
                    + "," + movie.Duration
                    + "," + Helpers.EscapeCsvField(movie.IMDB)
                    + "," + Helpers.EscapeCsvField(movie.TMDB)
                    + "," + Helpers.EscapeCsvField(movie.Plex)

                    );
            }

            File.WriteAllLines(filePath, lines);
        }

        public static List<PlexIMDBMap> ReadPlexMapCSV(string filePath)
        {
            var maps = new List<PlexIMDBMap>();

            var lines = File.ReadAllLines(filePath);

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

                try
                {
                    var map = new PlexIMDBMap
                    {
                        PlexGuid = parts[0],
                        Plex = parts[1],
                        IMDB = parts[2],
                        TMDB = parts[3],
                        TVDB = parts[4]
                    };


                    maps.Add(map);
                }
                catch (Exception e)
                {
                    int u = 0;
                }
            }

            return maps;
        }

        public static void WritePlexMapCSV(string filePath, List<PlexIMDBMap> maps)
        {
            var lines = new List<string>();

            foreach (var map in maps)
            {
                lines.Add(Helpers.EscapeCsvField(map.PlexGuid) + "," + Helpers.EscapeCsvField(map.Plex) + "," + Helpers.EscapeCsvField(map.IMDB) + "," + Helpers.EscapeCsvField(map.TMDB) + "," + Helpers.EscapeCsvField(map.TVDB));
            }

            File.WriteAllLines(filePath, lines);
        }

        public static List<PlexCollectionMovie> ReadCollectionCSV(string filePath)
        {
            var newMovies = new List<PlexCollectionMovie>();

            var movies = File.ReadAllLines(filePath);

            for (var i = 0; i < movies.Length; i++)
            {
                TextFieldParser parser = new TextFieldParser(new StringReader(movies[i]));
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                string[] parts = null;

                while (!parser.EndOfData)
                {
                    parts = parser.ReadFields();
                }

                try
                {
                    var plexCollectionMovie = new PlexCollectionMovie
                    {
                        CollectionName = parts[0],
                        MovieTitle = parts[1],
                        IMDB = parts.Length > 3 ? parts[3] : null,
                        TMDB = parts.Length > 4 ? parts[4] : null,
                        CollectionKey = parts.Length > 5 ? parts[5] : null,
                    };

                    int parseInt;
                    if (parts.Length > 2 && int.TryParse(parts[2], out parseInt))
                    {
                        plexCollectionMovie.MovieYear = parseInt;
                    }


                    newMovies.Add(plexCollectionMovie);
                }
                catch (Exception e)
                {
                    int u = 0;
                }
            }

            return newMovies;
        }

        public static List<PlexCollectionTVShow> ReadTVShowCollectionCSV(string filePath)
        {
            var newTVShows = new List<PlexCollectionTVShow>();

            var tvShows = File.ReadAllLines(filePath);

            for (var i = 0; i < tvShows.Length; i++)
            {
                TextFieldParser parser = new TextFieldParser(new StringReader(tvShows[i]))
                {
                    HasFieldsEnclosedInQuotes = true
                };
                parser.SetDelimiters(",");

                string[] parts = null;

                while (!parser.EndOfData)
                {
                    parts = parser.ReadFields();
                }

                try
                {
                    newTVShows.Add(PlexCollectionTVShow.Parse(parts));
                }
                catch (Exception e)
                {
                    int u = 0;
                }
            }

            return newTVShows;
        }


        public static List<PlexCollectionTVShow> ReadTVShowBackupCollectionCSV(string filePath)
        {
            var newTVShows = new List<PlexCollectionTVShow>();

            var tvShows = File.ReadAllLines(filePath);

            for (var i = 0; i < tvShows.Length; i++)
            {
                TextFieldParser parser = new TextFieldParser(new StringReader(tvShows[i]))
                {
                    HasFieldsEnclosedInQuotes = true
                };
                parser.SetDelimiters(",");

                string[] parts = null;

                while (!parser.EndOfData)
                {
                    parts = parser.ReadFields();
                }

                try
                {
                    newTVShows.Add(PlexCollectionTVShow.ParseBackup(parts));
                }
                catch (Exception e)
                {
                    int u = 0;
                }
            }

            return newTVShows;
        }


        public static List<PlexCollectionTrack> ReadMusicCollectionCSV(string filePath)
        {
            var newTracks = new List<PlexCollectionTrack>();

            var tracks = File.ReadAllLines(filePath);

            for (var i = 0; i < tracks.Length; i++)
            {
                TextFieldParser parser = new TextFieldParser(new StringReader(tracks[i]))
                {
                    HasFieldsEnclosedInQuotes = true
                };
                parser.SetDelimiters(",");

                string[] parts = null;

                while (!parser.EndOfData)
                {
                    parts = parser.ReadFields();
                }

                try
                {
                    newTracks.Add(PlexCollectionTrack.Parse(parts));
                }
                catch (Exception e)
                {
                    int u = 0;
                }
            }

            return newTracks;
        }

        public static void WriteCollectionCSV(string filePath, List<PlexCollectionMovie> movies)
        {
            var lines = new List<string>();

            foreach (var movie in movies)
            {
                lines.Add(Helpers.EscapeCsvField(movie.CollectionName) + "," + Helpers.EscapeCsvField(movie.MovieTitle) + "," + (movie.MovieYear.HasValue ? movie.MovieYear.Value.ToString() : "") + "," + (!string.IsNullOrWhiteSpace(movie.IMDB) ? movie.IMDB : "") + "," + (!string.IsNullOrWhiteSpace(movie.TMDB) ? movie.TMDB : "") + "," + (!string.IsNullOrWhiteSpace(movie.CollectionKey) ? movie.CollectionKey : ""));
            }

            File.WriteAllLines(filePath, lines);
        }

        public static void WriteTVShowCollectionCSV(string filePath, List<PlexCollectionTVShow> tvShows)
        {
            var lines = new List<string>();

            foreach (var tvShow in tvShows)
            {
                lines.Add(tvShow.ToString());
            }

            File.WriteAllLines(filePath, lines);
        }

        public static string ReplaceInvalidFilePathChars(string filename)
        {
            return string.Join("", filename.Split(Path.GetInvalidFileNameChars()));
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        public static string EscapeCsvField(string data)
        {
            if (data == null)
            {
                data = String.Empty;
            }
            // CSV rules: http://en.wikipedia.org/wiki/Comma-separated_values#Basic_rules
            // From the rules:
            // 1. if the data has quote, escape the quote in the data
            // 2. if the data contains the delimiter (in our case ','), double-quote it
            // 3. if the data contains the new-line, double-quote it.

            if (data.Contains("\""))
            {
                data = data.Replace("\"", "\"\"");
            }

            if (data.Contains(","))
            {
                data = string.Format("\"{0}\"", data);
            }

            if (data.Contains(System.Environment.NewLine))
            {
                data = string.Format("\"{0}\"", data);
            }

            return data;
        }

        public static List<TVShow> ReadMedusaTVShowCSV(string filePath)
        {
            var newTVShows = new List<TVShow>();

            var tvShows = File.ReadAllLines(filePath);

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

                try
                {
                    var tvshow = new TVShow
                    {
                        show_name = parts[3],
                        location = parts[4],
                        network = parts[5],
                        genre = parts[6],
                        classification = parts[7],
                        airs = parts[10],
                        status = parts[11],
                        lang = parts[16],
                        notify_list = parts[18],
                        imdb_id = parts[19],
                        rls_require_words = parts[23],
                        rls_ignore_words = parts[24],
                        plot = parts[29],
                        show_lists = parts[33]
                    };

                    int parseInt;
                    if (parts.Length > 0 && int.TryParse(parts[0], out parseInt))
                    {
                        tvshow.show_id = parseInt;
                    }
                    if (parts.Length > 1 && int.TryParse(parts[1], out parseInt))
                    {
                        tvshow.indexer_id = parseInt;
                    }
                    if (parts.Length > 2 && int.TryParse(parts[2], out parseInt))
                    {
                        tvshow.indexer = parseInt;
                    }
                    if (parts.Length > 8 && int.TryParse(parts[8], out parseInt))
                    {
                        tvshow.runtime = parseInt;
                    }
                    if (parts.Length > 9 && int.TryParse(parts[9], out parseInt))
                    {
                        tvshow.quality = parseInt;
                    }
                    if (parts.Length > 12 && int.TryParse(parts[12], out parseInt))
                    {
                        tvshow.flatten_folders = parseInt;
                    }
                    if (parts.Length > 13 && int.TryParse(parts[13], out parseInt))
                    {
                        tvshow.paused = parseInt;
                    }
                    if (parts.Length > 14 && int.TryParse(parts[14], out parseInt))
                    {
                        tvshow.startyear = parseInt;
                    }
                    if (parts.Length > 15 && int.TryParse(parts[15], out parseInt))
                    {
                        tvshow.air_by_date = parseInt;
                    }
                    if (parts.Length > 17 && int.TryParse(parts[17], out parseInt))
                    {
                        tvshow.subtitles = parseInt;
                    }
                    if (parts.Length > 20 && int.TryParse(parts[20], out parseInt))
                    {
                        tvshow.last_update_indexer = parseInt;
                    }
                    if (parts.Length > 21 && int.TryParse(parts[21], out parseInt))
                    {
                        tvshow.dvdorder = parseInt;
                    }
                    if (parts.Length > 22 && int.TryParse(parts[22], out parseInt))
                    {
                        tvshow.archive_firstmatch = parseInt;
                    }
                    if (parts.Length > 25 && int.TryParse(parts[25], out parseInt))
                    {
                        tvshow.sports = parseInt;
                    }
                    if (parts.Length > 26 && int.TryParse(parts[26], out parseInt))
                    {
                        tvshow.anime = parseInt;
                    }
                    if (parts.Length > 27 && int.TryParse(parts[27], out parseInt))
                    {
                        tvshow.scene = parseInt;
                    }
                    if (parts.Length > 28 && int.TryParse(parts[28], out parseInt))
                    {
                        tvshow.default_ep_status = parseInt;
                    }
                    if (parts.Length > 30 && int.TryParse(parts[30], out parseInt))
                    {
                        tvshow.airdate_offset = parseInt;
                    }
                    if (parts.Length > 31 && int.TryParse(parts[31], out parseInt))
                    {
                        tvshow.rls_require_exclude = parseInt;
                    }
                    if (parts.Length > 32 && int.TryParse(parts[32], out parseInt))
                    {
                        tvshow.rls_ignore_exclude = parseInt;
                    }


                    newTVShows.Add(tvshow);
                }
                catch (Exception e)
                {
                    int u = 0;
                }
            }

            return newTVShows;
        }

        public static List<Episode> ReadMedusaEpisodeCSV(string filePath)
        {
            var newEpisodes = new List<Episode>();

            var tvShows = File.ReadAllLines(filePath);

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

                try
                {
                    var episode = new Episode
                    {
                        name = parts[4],
                        description = parts[7],
                        location = parts[13],
                        release_name = parts[15],
                        subtitles = parts[16],
                        subtitles_lastsearch = parts[18],
                        release_group = parts[25]
                    };

                    int parseInt;
                    long parseLong;
                    if (parts.Length > 0 && int.TryParse(parts[0], out parseInt))
                    {
                        episode.episode_id = parseInt;
                    }
                    if (parts.Length > 1 && int.TryParse(parts[1], out parseInt))
                    {
                        episode.showid = parseInt;
                    }
                    if (parts.Length > 2 && int.TryParse(parts[2], out parseInt))
                    {
                        episode.indexerid = parseInt;
                    }
                    if (parts.Length > 3 && int.TryParse(parts[3], out parseInt))
                    {
                        episode.indexer = parseInt;
                    }
                    if (parts.Length > 5 && int.TryParse(parts[5], out parseInt))
                    {
                        episode.season = parseInt;
                    }
                    if (parts.Length > 6 && int.TryParse(parts[6], out parseInt))
                    {
                        episode.episode = parseInt;
                    }
                    if (parts.Length > 8 && int.TryParse(parts[8], out parseInt))
                    {
                        episode.airdate = parseInt;
                    }
                    if (parts.Length > 9 && int.TryParse(parts[9], out parseInt))
                    {
                        episode.hasnfo = parseInt;
                    }
                    if (parts.Length > 10 && int.TryParse(parts[10], out parseInt))
                    {
                        episode.hastbn = parseInt;
                    }
                    if (parts.Length > 11 && int.TryParse(parts[11], out parseInt))
                    {
                        episode.status = parseInt;
                    }
                    if (parts.Length > 12 && int.TryParse(parts[12], out parseInt))
                    {
                        episode.quality = parseInt;
                    }
                    if (parts.Length > 14 && long.TryParse(parts[14], out parseLong))
                    {
                        episode.file_size = parseLong;
                    }
                    if (parts.Length > 17 && int.TryParse(parts[17], out parseInt))
                    {
                        episode.subtitles_searchcount = parseInt;
                    }
                    if (parts.Length > 19 && int.TryParse(parts[19], out parseInt))
                    {
                        episode.is_proper = parseInt;
                    }
                    if (parts.Length > 20 && int.TryParse(parts[20], out parseInt))
                    {
                        episode.scene_season = parseInt;
                    }
                    if (parts.Length > 21 && int.TryParse(parts[21], out parseInt))
                    {
                        episode.scene_episode = parseInt;
                    }
                    if (parts.Length > 22 && int.TryParse(parts[22], out parseInt))
                    {
                        episode.absolute_number = parseInt;
                    }
                    if (parts.Length > 23 && int.TryParse(parts[23], out parseInt))
                    {
                        episode.scene_absolute_number = parseInt;
                    }
                    if (parts.Length > 24 && int.TryParse(parts[24], out parseInt))
                    {
                        episode.version = parseInt;
                    }
                    if (parts.Length > 26 && int.TryParse(parts[26], out parseInt))
                    {
                        episode.manually_searched = parseInt;
                    }
                    if (parts.Length > 27 && int.TryParse(parts[27], out parseInt))
                    {
                        episode.watched = parseInt;
                    }

                    DateTime parseDateTime;

                    if (parts.Length > 18 && DateTime.TryParse(parts[18], out parseDateTime))
                    {
                        episode.SubTitlesLastSearched = parseDateTime;
                    }

                    newEpisodes.Add(episode);
                }
                catch (Exception e)
                {
                    int u = 0;
                }
            }

            return newEpisodes;
        }

        public static string GetIndexerFriendlyName(int indexerId)
        {
            switch (indexerId)
            {
                case 1:
                    return "tvdb";
                case 3:
                    return "tvmaze";
                case 4:
                    return "tmdb";
                default:
                    return "";

            }

        }

        public static DirectoryInfo SeriesFolderNameFixer(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            if (path.StartsWith("C:\\Share\\PlexNewTV3"))
            {
                di = new DirectoryInfo("P:\\Media\\TV Shows\\" + di.Name);
            }
            if (path.StartsWith("C:\\Share\\PlexNewTV4"))
            {
                di = new DirectoryInfo("A:\\Media\\TV Shows\\" + di.Name);
            }
            if (path.StartsWith("C:\\Share\\PlexNewTV2"))
            {
                di = new DirectoryInfo("I:\\Media\\TV Shows\\" + di.Name);
            }
            if (path.StartsWith("C:\\Share\\PlexNewTV1"))
            {
                di = new DirectoryInfo("K:\\Media\\TV Shows\\" + di.Name);
            }
            if (path.StartsWith("C:\\Media\\TV Shows"))
            {
                di = new DirectoryInfo("H:\\Media\\TV Shows\\" + di.Name);
            }

            return di;
        }

        public static bool DoesPropertyExist(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);

            return settings.GetType().GetProperty(name) != null;
        }
    }
}
