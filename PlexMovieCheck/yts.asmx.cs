using Microsoft.VisualBasic.FileIO;
using PlexHelpers.Common;
using PlexHelpers.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;

namespace PlexHelpers.Web
{
    /// <summary>
    /// Summary description for yts
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class yts : System.Web.Services.WebService
    {
        static readonly object Object = new object();

        private static List<PlexMovie> _plexMovies;
        private static DateTime? _cacheDateTime;

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CheckMovie(string title, int year)
        {
            PlexMovie plexMovie = null;

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
                        _plexMovies = Helpers.ReadCSV("C:\\imdb\\rarbgcheck.csv");

                        _cacheDateTime = DateTime.Now;
                    }
                }
            }

            string returnCss = "background-color:#ff6666";//Red

            var movieTitle = title.Replace(",","").Replace(":", "").Replace("'", "").Replace("-", "").Replace("  ", " ").ToLowerInvariant();

            plexMovie = _plexMovies.FirstOrDefault(p => (p.Year == year) && p.CompareTitle == movieTitle);

            if (plexMovie == null)
            {
                plexMovie = _plexMovies.FirstOrDefault(p => (p.Year == year || p.Year == year - 1 || p.Year == year + 1) && p.CompareTitle == movieTitle);
            }

            if (plexMovie == null && movieTitle.Contains(" and "))
            {
                plexMovie = _plexMovies.FirstOrDefault(p => (p.Year == year || p.Year == year - 1 || p.Year == year + 1) && p.CompareTitle == movieTitle.Replace(" and ", " & "));
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

                if (year > 1980 && plexMovie.DisplayAspectRatio < (decimal)1.5)
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
                }
                else
                {
                    returnCss = "background-color:#66ff66";//Green
                }
            }

            return returnCss;
        }
    }
}
