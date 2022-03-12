using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Services;
using System.Web.Services;

namespace PlexHelpers.Web
{
    /// <summary>
    /// Summary description for rarbg
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class rarbg : System.Web.Services.WebService
    {
        static readonly object Object = new object();

        private static List<PlexMovie> _plexMovies;
        private static DateTime? _cacheDateTime;

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CheckMovie(string imdb, string title, int seeders, string size, bool isDocumentary)
        {
            if (!string.IsNullOrWhiteSpace(imdb))
            {
                imdb = imdb.Split('=')[1];
            }
            if (_cacheDateTime != null && (DateTime.Now - _cacheDateTime.Value).TotalHours > 1)
            {
                _plexMovies = null;
            }

            if (_plexMovies == null)
            {
                lock (Object)
                {
                    if (_plexMovies == null)
                    {
                        _plexMovies = Helpers.ReadPlexMovieCSV("C:\\imdb\\plex-movies.csv", Helpers.ReadPlexMapCSV("C:\\imdb\\plex-map.csv"));

                        _cacheDateTime = DateTime.Now;
                    }
                }
            }

            var yearRegex = new Regex("\\.(19[123456789]\\d{1}|20[012]\\d{1})\\.");

            var match = yearRegex.Match(title);

            string returnCss = "background-color:#ff6666";//Red

            if (match.Success)
            {
                var year = int.Parse(match.Groups[1].Value);

                var movieTitle = title.Substring(0, title.IndexOf(match.Groups[0].Value, StringComparison.Ordinal));
                movieTitle = movieTitle.Replace(".", " ");
                movieTitle = movieTitle.ToLowerInvariant();

                PlexMovie plexMovie = null;

                if (!string.IsNullOrWhiteSpace(imdb))
                {
                    plexMovie = _plexMovies.FirstOrDefault(p => p.IMDB == imdb);
                }

                if (plexMovie == null)
                {
                    plexMovie = _plexMovies.FirstOrDefault(p => (p.Year == year) && p.Title == movieTitle);
                }

                if (plexMovie == null)
                {
                    plexMovie = _plexMovies.FirstOrDefault(p => (p.Year == year || p.Year == year - 1 || p.Year == year + 1) && p.Title == movieTitle);
                }

                if (plexMovie == null && movieTitle.Contains(" and "))
                {
                    plexMovie = _plexMovies.FirstOrDefault(p => (p.Year == year || p.Year == year - 1 || p.Year == year + 1) && p.Title == movieTitle.Replace(" and ", " & "));
                }

                if (plexMovie != null)
                {
                    returnCss = "background-color:#66ff66";//Green

                    if (plexMovie.Container != "mkv" && plexMovie.Container != "mp4")
                    {
                        returnCss = "background-color:#ffff66";//Yellow
                    }

                    if (plexMovie.AudioChannels < 2)
                    {
                        returnCss = "background-color:#ffff66";//Yellow
                    }

                    if (plexMovie.Size < 694960128)
                    {
                        returnCss = "background-color:#ffff66";//Yellow
                    }

                    if (plexMovie.DisplayAspectRatio < (decimal)1.5)
                    {
                        returnCss = "background-color:#ffff66";//Yellow
                    }

                    if (plexMovie.Width < 720)
                    {
                        returnCss = "background-color:#ffff66";//Yellow
                    }

                    if (plexMovie.FullFileName.Contains("dvd"))
                    {
                        returnCss = "background-color:#ffff66";//Yellow
                    }

                    if (Settings.ReleaseGroups.All(p => !plexMovie.FullFileName.ToLowerInvariant().Contains(p)))
                    {
                        returnCss = "background-color:#ffff66";//Yellow

                        if(movieTitle.Contains("rarbg"))
                        {
                            returnCss = "background-color:#ff6666";//Red
                        }
                    }
                    else
                    {
                        returnCss = "background-color:#66ff66";//Green
                    }

                    if (size.Contains("GB"))
                    {
                        decimal gb = decimal.Parse(size.Replace("GB", ""));
                        if (gb > 5)
                        {
                            returnCss = "background-color:#cccccc";//Grey
                        }
                    }
                }
                else
                {
                    if (size.Contains("GB"))
                    {
                        decimal gb = decimal.Parse(size.Replace("GB", ""));
                        if (gb > 5)
                        {
                            returnCss = "background-color:#cccccc";//Grey
                        }
                        else if (seeders < 10)
                        {
                            returnCss = "background-color:#ffb422";//Orange
                        }
                    }

                    if (size.Contains("MB"))
                    {
                        //returnCss = "background-color:#ffff66";//Yellow
                        if (seeders < 10)
                        {
                            returnCss = "background-color:#ffb422";//Orange
                        }
                    }

                    var afterMovieTitle = title.Substring(title.IndexOf(match.Groups[0].Value, StringComparison.Ordinal)).ToLowerInvariant();
                    if (afterMovieTitle.Contains("chinese")
                        || afterMovieTitle.Contains("italian")
                        || afterMovieTitle.Contains("japanese")
                        || afterMovieTitle.Contains("thai")
                        || afterMovieTitle.Contains("turkish")
                        || afterMovieTitle.Contains("spanish")
                        || afterMovieTitle.Contains("korean")
                        || afterMovieTitle.Contains("german")
                        || afterMovieTitle.Contains("norwegian")
                        || afterMovieTitle.Contains("cantonese")
                        || afterMovieTitle.Contains("french")
                        || afterMovieTitle.Contains("swedish")
                        || afterMovieTitle.Contains("finnish")
                        || afterMovieTitle.Contains("russian")
                        || afterMovieTitle.Contains("portuguese")
                        || afterMovieTitle.Contains("vietnamese")
                        || afterMovieTitle.Contains("polish")
                        //|| isDocumentary
                        )
                    {
                        returnCss = "background-color:#cccccc";//Grey
                    }
                }
            }
            else
            {
                returnCss = "background-color:#000000";//Black
            }

            return returnCss;
        }
    }
}
