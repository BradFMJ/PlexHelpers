using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PlexSubFix
{
    class Program
    {
        private static bool CanDelete = true;
        private static bool CanMove = true;
        private static long minFileSize = 1000;
        private static long forcedMovieCutOffSize = 20480;

        private static Regex TVShowRegex = new Regex(@"[Ss](\d+)[Ee](\d+)", RegexOptions.Compiled);

        private static List<Tuple<string, string>> LanguageCodes = new List<Tuple<string, string>> {
             new Tuple<string, string>("Afrikaans","af"),
             new Tuple<string, string>("Albanian","sq"),
             new Tuple<string, string>("Arabic","ar"),
             new Tuple<string, string>("Basque","eu"),
             new Tuple<string, string>("Belarusian","be"),
             new Tuple<string, string>("Bulgarian","bg"),
             new Tuple<string, string>("Catalan","ca"),
             new Tuple<string, string>("Chinese","zh"),
             new Tuple<string, string>("Croatian","hr"),
             new Tuple<string, string>("Czech","cs"),
             new Tuple<string, string>("Danish","da"),
             new Tuple<string, string>("Dutch","nl"),
             new Tuple<string, string>("Estonian","et"),
             new Tuple<string, string>("Faeroese","fo"),
             new Tuple<string, string>("Farsi","fa"),
             new Tuple<string, string>("Finnish","fi"),
             new Tuple<string, string>("French","fr"),
             new Tuple<string, string>("Gaelic (Scotland)","gd"),
             new Tuple<string, string>("German","de"),
             new Tuple<string, string>("Greek","el"),
             new Tuple<string, string>("Hebrew","he"),
             new Tuple<string, string>("Hindi","hi"),
             new Tuple<string, string>("Hungarian","hu"),
             new Tuple<string, string>("Icelandic","is"),
             new Tuple<string, string>("Indonesian","id"),
             new Tuple<string, string>("Irish","ga"),
             new Tuple<string, string>("Italian","it"),
             new Tuple<string, string>("Japanese","ja"),
             new Tuple<string, string>("Korean","ko"),
             new Tuple<string, string>("Kurdish","ku"),
             new Tuple<string, string>("Latvian","lv"),
             new Tuple<string, string>("Lithuanian","lt"),
             new Tuple<string, string>("Macedonian","mk"),
             new Tuple<string, string>("Malayalam","ml"),
             new Tuple<string, string>("Malaysian","ms"),
             new Tuple<string, string>("Maltese","mt"),
             new Tuple<string, string>("Norwegian","no"),
             new Tuple<string, string>("Polish","pl"),
             new Tuple<string, string>("Portuguese","pt"),
             new Tuple<string, string>("Punjabi","pa"),
             new Tuple<string, string>("Rhaeto-Romanic","rm"),
             new Tuple<string, string>("Romanian","ro"),
             new Tuple<string, string>("Russian","ru"),
             new Tuple<string, string>("Serbian","sr"),
             new Tuple<string, string>("Slovak","sk"),
             new Tuple<string, string>("Slovenian","sl"),
             new Tuple<string, string>("Sorbian","sb"),
             new Tuple<string, string>("Spanish","es"),
             new Tuple<string, string>("Swedish","sv"),
             new Tuple<string, string>("Thai","th"),
             new Tuple<string, string>("Tsonga","ts"),
             new Tuple<string, string>("Tswana","tn"),
             new Tuple<string, string>("Turkish","tr"),
             new Tuple<string, string>("Ukrainian","uk"),
             new Tuple<string, string>("Urdu","ur"),
             new Tuple<string, string>("Venda","ve"),
             new Tuple<string, string>("Vietnamese","vi"),
             new Tuple<string, string>("Welsh","cy"),
             new Tuple<string, string>("Xhosa","xh"),
             new Tuple<string, string>("Yiddish","ji"),
             new Tuple<string, string>("Zulu","zu")
        };

        private static Dictionary<string, List<Regex>> LanguageCodesNew = new Dictionary<string, List<Regex>>();

        private static List<string> VideoFileExtensions = new List<string> { ".mkv", ".mp4", ".avi", ".m4v", ".idx", ".sub", ".srt", ".wmv" };

        static void Main(string[] args)
        {
            #region Languages

            LanguageCodesNew["en"] = new List<Regex> {
                new Regex("^eng\\.srt$", RegexOptions.Compiled),
                new Regex("^english.*\\.srt$", RegexOptions.Compiled),
                new Regex(".*_english\\.srt$", RegexOptions.Compiled),
                new Regex(".*_eng\\.srt$", RegexOptions.Compiled),
                new Regex(".*_und\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.eng\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["es"] = new List<Regex> {
                new Regex("^spa\\.srt$", RegexOptions.Compiled),
                new Regex("^spanish.*\\.srt$", RegexOptions.Compiled),
                new Regex("^castilian.*\\.srt$", RegexOptions.Compiled),
                new Regex(".*_spanish\\.srt$", RegexOptions.Compiled),
                new Regex(".*_spa\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.spa\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["it"] = new List<Regex> {
                new Regex(".*_italian.*\\.srt$", RegexOptions.Compiled),
                new Regex("^ita\\.srt$", RegexOptions.Compiled),
                new Regex("^italian.*\\.srt$", RegexOptions.Compiled),
                new Regex(".*_ita\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["pt"] = new List<Regex> {
                new Regex(".*_portuguese\\.srt$", RegexOptions.Compiled),
                new Regex("^por\\.srt$", RegexOptions.Compiled),
                new Regex("^portuguese.*\\.srt$", RegexOptions.Compiled),
                new Regex("^brazilian portuguese.*\\.srt$", RegexOptions.Compiled),
                new Regex(".*_por\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.por\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["de"] = new List<Regex> {
                new Regex("^ger\\.srt$", RegexOptions.Compiled),
                new Regex("^german\\.srt$", RegexOptions.Compiled),
                new Regex(".*_ger\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.ger\\.srt$", RegexOptions.Compiled),
                new Regex(".*_german\\.srt$", RegexOptions.Compiled),
                new Regex("^deu\\.srt$", RegexOptions.Compiled),
                new Regex(".*_deu\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.deu\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["el"] = new List<Regex> {
                new Regex("^greek\\.srt$", RegexOptions.Compiled),
                new Regex(".*_greek\\.srt$", RegexOptions.Compiled),
                new Regex("^gre\\.srt$", RegexOptions.Compiled),
                new Regex(".*_gre\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.gre\\.srt$", RegexOptions.Compiled)
            };
            //LanguageCodesNew["af"] = new List<Regex> { };
            LanguageCodesNew["sq"] = new List<Regex> { new Regex(".*_albanian\\.srt$", RegexOptions.Compiled), new Regex(".*_alb\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["ar"] = new List<Regex> { new Regex("^ara\\.srt$", RegexOptions.Compiled), new Regex(".*_arabic\\.srt$", RegexOptions.Compiled), new Regex(".*_ara\\.srt$", RegexOptions.Compiled), new Regex("^arabic.*\\.srt$", RegexOptions.Compiled) };
            //LanguageCodesNew["eu"] = new List<Regex> { };
            //LanguageCodesNew["be"] = new List<Regex> { };
            LanguageCodesNew["bg"] = new List<Regex> { new Regex(".*_bulgarian\\.srt$", RegexOptions.Compiled), new Regex(".*_bul\\.srt$", RegexOptions.Compiled) };
            //LanguageCodesNew["ca"] = new List<Regex> { };
            LanguageCodesNew["zh"] = new List<Regex> { new Regex("^chi\\.srt$", RegexOptions.Compiled), new Regex(".*_chinese\\.srt$", RegexOptions.Compiled), new Regex(".*_chi\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.chi\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["hr"] = new List<Regex> { new Regex(".*_croatian\\.srt$", RegexOptions.Compiled), new Regex(".*_hrv\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["ca"] = new List<Regex> {
                new Regex(".*_cat\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.cat\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["cs"] = new List<Regex>
            {
                new Regex(".*_czech\\.srt$", RegexOptions.Compiled),
                new Regex(".*_cze\\.srt$", RegexOptions.Compiled),
                new Regex("^cze\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.cze\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["da"] = new List<Regex> {
                new Regex(".*_danish\\.srt$", RegexOptions.Compiled),
                new Regex(".*_dan\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.dan\\.srt$", RegexOptions.Compiled), new Regex("^dan\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["nl"] = new List<Regex> {
                new Regex("^dut\\.srt$", RegexOptions.Compiled),
                new Regex(".*_dutch\\.srt$", RegexOptions.Compiled),
                new Regex(".*_dut\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.dut\\.srt$", RegexOptions.Compiled),
                new Regex("^nld\\.srt$", RegexOptions.Compiled),
                new Regex(".*_nld\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.nld\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["et"] = new List<Regex> { new Regex(".*_estonian\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["ee"] = new List<Regex> { new Regex(".*_ewe\\.srt$") };
            //LanguageCodesNew["fo"] = new List<Regex> { };
            LanguageCodesNew["fa"] = new List<Regex> { new Regex(".*_farsi\\.srt$", RegexOptions.Compiled), new Regex(".*_per\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["fi"] = new List<Regex> { new Regex(".*_finnish\\.srt$", RegexOptions.Compiled), new Regex(".*_fin\\.srt$", RegexOptions.Compiled), new Regex("^fin\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.fin\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["fr"] = new List<Regex> {
                new Regex("^fre\\.srt$", RegexOptions.Compiled),
                new Regex(".*_french\\.srt$", RegexOptions.Compiled),
                new Regex(".*_fre\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.fre\\.srt$", RegexOptions.Compiled),
                new Regex("^french.*\\.srt$", RegexOptions.Compiled),
                new Regex("^fra\\.srt$", RegexOptions.Compiled),
                new Regex(".*fra\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.fra\\.srt$", RegexOptions.Compiled)
            };
            //LanguageCodesNew["gd"] = new List<Regex> { };
            LanguageCodesNew["he"] = new List<Regex>
            {
                new Regex(".*_hebrew\\.srt$", RegexOptions.Compiled),
                new Regex(".*_heb\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.heb\\.srt$", RegexOptions.Compiled),
                new Regex("^heb\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["hi"] = new List<Regex> { new Regex(".*_hindi\\.srt$", RegexOptions.Compiled), new Regex(".*_hin\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.hin\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["hu"] = new List<Regex>
            {
                new Regex(".*_hungarian\\.srt$", RegexOptions.Compiled),
                new Regex(".*_hun\\.srt$"),
                new Regex("^hun\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.hun\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["is"] = new List<Regex> { new Regex(".*_icelandic\\.srt$", RegexOptions.Compiled), new Regex(".*_ice\\.srt$", RegexOptions.Compiled), new Regex("^ice\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["id"] = new List<Regex> {
                new Regex(".*_indonesian\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.ind\\.srt$", RegexOptions.Compiled),
                new Regex("^ind\\.srt$", RegexOptions.Compiled)
            };
            //LanguageCodesNew["ga"] = new List<Regex> { };
            LanguageCodesNew["ja"] = new List<Regex> {
                new Regex("^japanese\\.srt$", RegexOptions.Compiled),
                new Regex("^jpn\\.srt$", RegexOptions.Compiled),
                new Regex(".*_japanese\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.jpn\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["ko"] = new List<Regex> {
                    new Regex("^korean\\.srt$", RegexOptions.Compiled),
                new Regex("^kor\\.srt$", RegexOptions.Compiled),
                new Regex(".*_korean\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.kor\\.srt$", RegexOptions.Compiled) }
                ;
            LanguageCodesNew["kk"] = new List<Regex> { new Regex(".*_kazakh\\.srt$", RegexOptions.Compiled), new Regex(".*_kaz\\.srt$", RegexOptions.Compiled) };
            //LanguageCodesNew["ku"] = new List<Regex> { };
            LanguageCodesNew["lv"] = new List<Regex> { new Regex(".*_latvian\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["lt"] = new List<Regex> { new Regex(".*_lithuanian\\.srt$", RegexOptions.Compiled) };
            //LanguageCodesNew["mk"] = new List<Regex> { };
            //LanguageCodesNew["ml"] = new List<Regex> { };
            LanguageCodesNew["ms"] = new List<Regex> { new Regex(".*_may\\.srt$", RegexOptions.Compiled), new Regex(".*_malaysian\\.srt$", RegexOptions.Compiled) };
            //LanguageCodesNew["mt"] = new List<Regex> { };
            LanguageCodesNew["no"] = new List<Regex>
            {
                new Regex(".*_nor\\.srt$", RegexOptions.Compiled),
                new Regex(".*_norwegian\\.srt$", RegexOptions.Compiled),
                new Regex("^nor\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.nor\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["nb"] = new List<Regex> {
                new Regex(".*_nob\\.srt$", RegexOptions.Compiled),
                new Regex(".*_bokmal\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.nob\\.srt$", RegexOptions.Compiled),
                new Regex("^nob\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["pl"] = new List<Regex> { new Regex("^pol\\.srt$", RegexOptions.Compiled), new Regex(".*_polish\\.srt$", RegexOptions.Compiled), new Regex(".*_pol\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.pol\\.srt$", RegexOptions.Compiled) };
            //LanguageCodesNew["pa"] = new List<Regex> { };
            //LanguageCodesNew["rm"] = new List<Regex> { };
            LanguageCodesNew["ro"] = new List<Regex>
            {
                new Regex("^romanian\\.srt$", RegexOptions.Compiled),
                new Regex(".*_romanian\\.srt$", RegexOptions.Compiled),
                new Regex(".*_rum\\.srt$", RegexOptions.Compiled),
                new Regex("^rum\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.rum\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["ru"] = new List<Regex> { new Regex(".*_russian\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.rus\\.srt$", RegexOptions.Compiled), new Regex("^rus\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["sr"] = new List<Regex> {  new Regex(".*_srp\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["sk"] = new List<Regex> { new Regex(".*_slo\\.srt$", RegexOptions.Compiled), new Regex(".*_slk\\.srt$", RegexOptions.Compiled), new Regex("^slo\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["sl"] = new List<Regex> { new Regex(".*_slovenian\\.srt$", RegexOptions.Compiled), new Regex(".*_slv\\.srt$", RegexOptions.Compiled) };
            //LanguageCodesNew["sb"] = new List<Regex> { };
            LanguageCodesNew["sv"] = new List<Regex> { new Regex(".*_swedish\\.srt$", RegexOptions.Compiled), new Regex(".*_swe\\.srt$", RegexOptions.Compiled), new Regex("^swe\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.swe\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["th"] = new List<Regex> {
                new Regex("^thai\\.srt$", RegexOptions.Compiled),
                new Regex(".*_thai\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.tha\\.srt$", RegexOptions.Compiled),
                new Regex("^tha\\.srt$", RegexOptions.Compiled)
            };
            LanguageCodesNew["ta"] = new List<Regex> { new Regex(".*_tamil\\.srt$", RegexOptions.Compiled), new Regex(".*_tam\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["te"] = new List<Regex> { new Regex(".*_telugo\\.srt$", RegexOptions.Compiled), new Regex(".*_tel\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["tl"] = new List<Regex> { new Regex(".*_fil\\.srt$", RegexOptions.Compiled) };
            //LanguageCodesNew["ts"] = new List<Regex> { };
            //LanguageCodesNew["tn"] = new List<Regex> { };
            LanguageCodesNew["tr"] = new List<Regex> { new Regex("^tur\\.srt$", RegexOptions.Compiled), new Regex(".*_turkish\\.srt$", RegexOptions.Compiled), new Regex(".*_tur\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.tur\\.srt$", RegexOptions.Compiled) };
            LanguageCodesNew["uk"] = new List<Regex>
            {
                new Regex(".*_ukrainian\\.srt$", RegexOptions.Compiled),
                new Regex(".*_ukr\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.ukr\\.srt$", RegexOptions.Compiled)
            };
            //LanguageCodesNew["ur"] = new List<Regex> { };
            //LanguageCodesNew["ve"] = new List<Regex> { };
            LanguageCodesNew["vi"] = new List<Regex>
            {
                new Regex(".*_vietnamese\\.srt$", RegexOptions.Compiled),
                new Regex(".*_vie\\.srt$", RegexOptions.Compiled),
                new Regex("^vie\\.srt$", RegexOptions.Compiled),
                new Regex(".*\\.vie\\.srt$", RegexOptions.Compiled)
            };
            //LanguageCodesNew["cy"] = new List<Regex> { };
            //LanguageCodesNew["xh"] = new List<Regex> { };
            //LanguageCodesNew["ji"] = new List<Regex> { };
            //LanguageCodesNew["zu"] = new List<Regex> { };

            #endregion

            //CheckTVFolder(@"I:\Media\TV Shows");
            //CheckTVFolder(@"K:\Media\TV Shows");
            //CheckTVFolder(@"H:\Media\TV Shows");
            //CheckTVFolder(@"L:\Media\TV Shows");
            //CheckTVFolder(@"O:\Media\TV Shows");

            //FixTvShowNames(@"P:\Media\TV Shows\The Fall (2013)");
            //FixTvShowSubTitles(@"P:\Media\TV Shows\The Fall (2013)");

            //CleanCompletedFolder(@"J:\Media\Movies");
            //CheckFolder(@"N:\Media\Movies");
            //CheckFolder(@"Q:\Media\Movies");
            //CheckFolder(@"U:\Media\Movies");
            //CheckFolder(@"T:\Media\Movies");

            CleanCompletedFolder(@"C:\Users\Brad\Downloads\Newshosting");
            CleanCompletedFolder(@"H:\Media\Completed");
            //CleanCompletedFolder(@"H:\Media\Backlog");

            //FixMovieSubTitles(@"U:\Media\Movies");
            //FixMovieSubTitles(@"J:\Media\Movies");
            FixMovieSubTitles(@"H:\Media\Movies");
            FixMovieSubTitles(@"H:\Media\Completed");
            FixTvShowSubTitles(@"H:\Media\Completed");


            //FixTvShowNames(@"C:\Users\Brad\Downloads\Newshosting");
            //FixTvShowSubTitles(@"H:\Media\Backlog");


            Console.WriteLine("Done");

            Console.ReadLine();
        }

        public static void CheckFolder(string path)
        {
            DirectoryInfo dInfo = new DirectoryInfo(path);
            foreach (var directoryInfo in dInfo.EnumerateDirectories())
            {
                _CheckFolder(directoryInfo);
            }
        }

        private static void _CheckFolder(DirectoryInfo directoryInfo)
        {
            if (directoryInfo.EnumerateDirectories().Any())
            {
                Console.WriteLine("Folder has subdirectories: {0}", directoryInfo.FullName);
            }

            if (directoryInfo.EnumerateFiles().Count() == 0)
            {
                Console.WriteLine("Folder is empty: {0}", directoryInfo.FullName);
            }
        }

        public static void CheckTVFolder(string path)
        {
            DirectoryInfo dInfo = new DirectoryInfo(path);
            foreach (var directoryInfo in dInfo.EnumerateDirectories())
            {
                _CheckTVFolder(directoryInfo);
            }
        }

        private static void _CheckTVFolder(DirectoryInfo dInfo)
        {
            foreach (var directoryInfo in dInfo.EnumerateDirectories())
            {
                var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                if (!folderMatch.Success)
                {
                    Console.WriteLine("Folder Not Valid {0}", directoryInfo.FullName);
                }
            }
        }

        public static void CleanCompletedFolder(string path)
        {
            DirectoryInfo dInfo = new DirectoryInfo(path);

            List<FileInfo> files = dInfo.GetFiles("*.txt", SearchOption.AllDirectories).Where(p => p.Extension == ".txt").ToList();
            files.AddRange(dInfo.GetFiles("*.nfo", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".nfo").ToList());
            files.AddRange(dInfo.GetFiles("*.exe", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".exe").ToList());
            files.AddRange(dInfo.GetFiles("*.jpg", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".jpg").ToList());
            files.AddRange(dInfo.GetFiles("*.jpeg", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".jpeg").ToList());
            files.AddRange(dInfo.GetFiles("*.png", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".png").ToList());
            files.AddRange(dInfo.GetFiles("*.htm", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".htm").ToList());
            files.AddRange(dInfo.GetFiles("*.html", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".html").ToList());
            files.AddRange(dInfo.GetFiles("*.sfv", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".sfv").ToList());
            files.AddRange(dInfo.GetFiles("*.srr", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".srr").ToList());
            files.AddRange(dInfo.GetFiles("*.nzb", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".nzb").ToList());
            files.AddRange(dInfo.GetFiles("*.m3u", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".m3u").ToList());
            files.AddRange(dInfo.GetFiles("RARBG.COM.mp4", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".mp4").Where(p => p.Length < 104857600).ToList());
            files.AddRange(dInfo.GetFiles("RARBG.mp4", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".mp4").Where(p => p.Length < 104857600).ToList());
            files.AddRange(dInfo.GetFiles("RARBG.avi", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".avi").Where(p => p.Length < 104857600).ToList());
            files.AddRange(dInfo.GetFiles("RARBG.mkv", SearchOption.AllDirectories).Where(p => p.Extension.ToLower() == ".mkv").Where(p => p.Length < 104857600).ToList());

            files.AddRange(dInfo.GetFiles("*sample*", SearchOption.AllDirectories).Where(p => p.Length < 104857600).ToList());

            foreach (FileInfo file in files)
            {
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    Delete(file.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void FixTvShowSubTitles(string path)
        {
            DirectoryInfo dInfo = new DirectoryInfo(path);
            foreach (var directoryInfo in dInfo.EnumerateDirectories())
            {
                FixTVShowSubTitles(directoryInfo, "srt");
                FixTVShowSubTitles(directoryInfo, "sub");
                FixTVShowSubTitles(directoryInfo, "idx");
            }
        }

        public static void FixTvShowNames(string path)
        {
            DirectoryInfo dInfo = new DirectoryInfo(path);
            foreach (var directoryInfo in dInfo.EnumerateDirectories())
            {
                _FixTvShowNames(directoryInfo);
            }
        }

        private static void _FixTvShowNames(DirectoryInfo directoryInfo)
        {
            var subDirs = directoryInfo.GetDirectories();
            foreach (var subDir in subDirs)
            {
                _FixTvShowNames(subDir);
            }

            var tvShows = directoryInfo.GetFilesByExtensions(VideoFileExtensions.ToArray());

            Match match;

            foreach (var tvShow in tvShows)
            {

                //match = Regex.Match(tvShow.Name, @"^[Ss](\d+)[Ee](\d+)");
                //if (match.Success)
                //{
                //    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                //    if (!folderMatch.Success)
                //    {
                //        continue;
                //    }
                //    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');

                //    string season = match.Groups[1].Value.PadLeft(2, '0');
                //    string episode = match.Groups[2].Value.PadLeft(2, '0');

                //    if (folderSeason != season)
                //    {
                //        continue;
                //    }

                //    var badChar = tvShow.Name.Substring(match.Groups[0].Value.Length, 1);

                //    if (badChar == " " || badChar == ".")
                //    {
                //        continue;
                //    }
                //    if (badChar == "E")
                //    {
                //        badChar = tvShow.Name.Substring(match.Groups[0].Value.Length + 1, 1);
                //        var arr = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                //        if (arr.Contains(badChar))
                //        {
                //            continue;
                //        }
                //    }
                //    string destination = string.Format("S{0}E{1} ", season, episode);
                //    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                //    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                //    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                //    continue;
                //}

                //match = Regex.Match(tvShow.Name, @"sprinter-sgctc[Ss](\d+)[Ee](\d+)");
                //if (match.Success)
                //{
                //    string season = match.Groups[1].Value.PadLeft(2, '0');
                //    string episode = match.Groups[2].Value.PadLeft(2, '0');

                //    string destination = string.Format("aaf-sgctc - S{0}E{1}", season, episode);
                //    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                //    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                //    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                //    continue;
                //}



                match = Regex.Match(tvShow.Name, @"[Ss](\d+)[Ee](\d+)");
                if (match.Success)
                {
                    continue;
                }
                //match = Regex.Match(tvShow.Name, @"- (\d{3}) -");
                //if (match.Success)
                //{
                //    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                //    if (!folderMatch.Success)
                //    {
                //        continue;
                //    }
                //    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');

                //    string episode = match.Groups[1].Value.PadLeft(2, '0');

                //    var intEpisode = int.Parse(episode);

                //    if (folderSeason == "02")
                //    {
                //        intEpisode -= 5;
                //    }
                //    if (folderSeason == "03")
                //    {
                //        intEpisode -= 25;
                //    }
                //    if (folderSeason == "04")
                //    {
                //        intEpisode -= 45;
                //    }

                //    string destination = string.Format("- S{0}E{1} -", folderSeason, intEpisode.ToString().PadLeft(2, '0'));
                //    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                //    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                //    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                //    continue;
                //}
                //match = Regex.Match(tvShow.Name, @"SE(\d+) EP(\d+)");
                //if (match.Success)
                //{
                //    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                //    if (!folderMatch.Success)
                //    {
                //        continue;
                //    }
                //    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');
                //    string season = match.Groups[1].Value.PadLeft(2, '0');

                //    int intEpisode = int.Parse(match.Groups[2].Value);
                //    string episode = intEpisode.ToString().PadLeft(2, '0');

                //    if (season == "02")
                //    {
                //        episode = (intEpisode - 21).ToString().PadLeft(2, '0');
                //    }

                //    if (season == "03")
                //    {
                //        episode = (intEpisode - 51).ToString().PadLeft(2, '0');
                //    }

                //    if (season == "04")
                //    {
                //        episode = (intEpisode - 65).ToString().PadLeft(2, '0');
                //    }

                //    if (season != folderSeason)
                //    {
                //        continue;
                //    }


                //    string destination = string.Format("S{0}E{1} - ", season, episode);
                //    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                //    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                //    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                //    continue;
                //}
                //match = Regex.Match(tvShow.Name, @" \- (\d{2}) \- ");
                //if (match.Success)
                //{
                //    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                //    if (!folderMatch.Success)
                //    {
                //        continue;
                //    }
                //    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');


                //    string episode = match.Groups[1].Value.PadLeft(2, '0');
                //    int episodeInt = int.Parse(episode);
                //    if (folderSeason == "02")
                //    {
                //        episodeInt = episodeInt - 15;
                //        episode = episodeInt.ToString().PadLeft(2, '0');
                //    }

                //    string destination = string.Format(" - S{0}E{1} - ", folderSeason, episode);
                //    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                //    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                //    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                //    continue;
                //}
                match = Regex.Match(tvShow.Name, @"(\d+)e(\d+)-");
                if (match.Success)
                {
                    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                    if (!folderMatch.Success)
                    {
                        continue;
                    }
                    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');
                    string season = match.Groups[1].Value.PadLeft(4, '0');
                    string episode = match.Groups[2].Value.PadLeft(2, '0');

                    string destination = string.Format("S{0}E{1} - ", season, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                match = Regex.Match(tvShow.Name, @"S(\d+)-E(\d+)_");
                if (match.Success)
                {
                    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                    if (!folderMatch.Success)
                    {
                        continue;
                    }
                    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');
                    string season = match.Groups[1].Value.PadLeft(2, '0');
                    string episode = match.Groups[2].Value.PadLeft(2, '0');

                    if (season == "02")
                    {
                        var s2 = int.Parse(episode);
                        episode = (s2 - 13).ToString().PadLeft(2, '0');
                    }

                    if (season == "03")
                    {
                        var s3 = int.Parse(episode);
                        episode = (s3 - 21).ToString().PadLeft(2, '0');
                    }

                    if (season != folderSeason)
                    {
                        continue;
                    }


                    string destination = string.Format("S{0}E{1} - ", season, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }

                match = Regex.Match(tvShow.Name, @"Season (\d+) Episode (\d+)");
                if (match.Success)
                {
                    string season = match.Groups[1].Value.PadLeft(2, '0');
                    string episode = match.Groups[2].Value.PadLeft(2, '0');

                    string destination = string.Format("S{0}E{1}", season, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                match = Regex.Match(tvShow.Name, @"Season (\d+) - (\d+)");
                if (match.Success)
                {
                    string season = match.Groups[1].Value.PadLeft(2, '0');
                    string episode = match.Groups[2].Value.PadLeft(2, '0');

                    string destination = string.Format("S{0}E{1}", season, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                match = Regex.Match(tvShow.Name, @"\[(\d+)x(\d+)\]");
                if (match.Success)
                {
                    string season = match.Groups[1].Value.PadLeft(2, '0');
                    string episode = match.Groups[2].Value.PadLeft(2, '0');

                    string destination = string.Format("S{0}E{1}", season, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                match = Regex.Match(tvShow.Name, @"(\d+)x(\d+)");
                if (match.Success)
                {
                    string season = match.Groups[1].Value.PadLeft(2, '0');
                    string episode = match.Groups[2].Value.PadLeft(2, '0');

                    string destination = string.Format("S{0}E{1}", season, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                match = Regex.Match(tvShow.Name, @"[Ss](\d+)\s+[Ee](\d+)");
                if (match.Success)
                {

                    string season = match.Groups[1].Value.PadLeft(2, '0');
                    string episode = match.Groups[2].Value.PadLeft(2, '0');

                    string destination = string.Format("S{0}E{1}", season, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                match = Regex.Match(tvShow.Name, @"[Ss](\d+)\.[Ee](\d+)");
                if (match.Success)
                {

                    string season = match.Groups[1].Value.PadLeft(2, '0');
                    string episode = match.Groups[2].Value.PadLeft(2, '0');

                    string destination = string.Format("S{0}E{1}", season, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                match = Regex.Match(tvShow.Name, @"^Episode (\d{3})");
                if (match.Success)
                {
                    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                    if (!folderMatch.Success)
                    {
                        continue;
                    }
                    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');


                    string episode = match.Groups[1].Value.PadLeft(2, '0');

                    string destination = string.Format("S{0}E{1}", folderSeason, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                match = Regex.Match(tvShow.Name, @"^Episode (\d{2})");
                if (match.Success)
                {
                    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                    if (!folderMatch.Success)
                    {
                        continue;
                    }
                    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');


                    string episode = match.Groups[1].Value.PadLeft(2, '0');

                    string destination = string.Format("S{0}E{1}", folderSeason, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                match = Regex.Match(tvShow.Name, @"- Episode (\d{2}) -");
                if (match.Success)
                {
                    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                    if (!folderMatch.Success)
                    {
                        continue;
                    }
                    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');


                    string episode = match.Groups[1].Value.PadLeft(2, '0');

                    string destination = string.Format("- S{0}E{1} -", folderSeason, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                match = Regex.Match(tvShow.Name, @"(\d{1})\.(\d{2})");
                if (match.Success)
                {
                    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                    if (!folderMatch.Success)
                    {
                        continue;
                    }
                    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');
                    string season = match.Groups[1].Value.PadLeft(2, '0');
                    string episode = match.Groups[2].Value.PadLeft(2, '0');


                    if (season != folderSeason)
                    {
                        continue;
                    }


                    string destination = string.Format("S{0}E{1}", season, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                if (MatchFourAndThreeDigitYearAndSeason(directoryInfo, tvShow, Regex.Match(tvShow.Name, @"\.(\d{4})-")))
                {
                    continue;
                }
                if (MatchFourAndThreeDigitYearAndSeason(directoryInfo, tvShow, Regex.Match(tvShow.Name, @"(\d{4})")))
                {
                    continue;
                }
                if (MatchFourAndThreeDigitYearAndSeason(directoryInfo, tvShow, Regex.Match(tvShow.Name, @"^(\d{3}) ")))
                {
                    continue;
                }
                if (MatchFourAndThreeDigitYearAndSeason(directoryInfo, tvShow, Regex.Match(tvShow.Name, @"\.(\d{3})\.")))
                {
                    continue;
                }
                if (MatchFourAndThreeDigitYearAndSeason(directoryInfo, tvShow, Regex.Match(tvShow.Name, @" \- (\d{3}) \- ")))
                {
                    continue;
                }
                if (MatchFourAndThreeDigitYearAndSeason(directoryInfo, tvShow, Regex.Match(tvShow.Name, @" (\d{3})")))
                {
                    continue;
                }
                //if (match.Success)
                //{
                //    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                //    if (!folderMatch.Success)
                //    {
                //        continue;
                //    }
                //    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');


                //    string total = match.Groups[1].Value.PadLeft(4, '0');
                //    string season = total.Substring(0, 2);
                //    string episode = total.Substring(2, 2);

                //    if (folderSeason != season)
                //    {
                //        continue;
                //    }

                //    string destination = string.Format(" - S{0}E{1} - ", season, episode);
                //    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                //    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                //    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                //    continue;
                //}

                //match = Regex.Match(tvShow.Name, @" \- (\d{2}) \- ");
                //if (match.Success)
                //{
                //    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                //    if (!folderMatch.Success)
                //    {
                //        continue;
                //    }
                //    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');


                //    string episode = match.Groups[1].Value.PadLeft(2, '0');
                //    int episodeInt = int.Parse(episode);
                //    if(folderSeason=="02")
                //    {
                //        episodeInt = episodeInt - 40;
                //        episode= episodeInt.ToString().PadLeft(2, '0');
                //    }

                //    string destination = string.Format(" - S{0}E{1} - ", folderSeason, episode);
                //    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                //    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                //    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                //    continue;
                //}

                match = Regex.Match(tvShow.Name, @"^Episode (\d{1})");
                if (match.Success)
                {
                    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                    if (!folderMatch.Success)
                    {
                        continue;
                    }
                    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');


                    string episode = match.Groups[1].Value.PadLeft(2, '0');

                    string destination = string.Format("S{0}E{1}", folderSeason, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                match = Regex.Match(tvShow.Name, @"_eps(\d+)_");
                if (match.Success)
                {
                    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                    if (!folderMatch.Success)
                    {
                        continue;
                    }
                    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');



                    string episode = match.Groups[1].Value.PadLeft(2, '0');

                    string destination = string.Format(" - S{0}E{1} - ", folderSeason, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }
                match = Regex.Match(tvShow.Name, @"- Ep. (\d+) -");
                if (match.Success)
                {
                    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                    if (!folderMatch.Success)
                    {
                        continue;
                    }
                    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');



                    string episode = match.Groups[1].Value.PadLeft(2, '0');

                    string destination = string.Format(" - S{0}E{1} - ", folderSeason, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }

                match = Regex.Match(tvShow.Name, @"(\d{2})\. ");
                if (match.Success)
                {
                    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                    if (!folderMatch.Success)
                    {
                        continue;
                    }
                    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');



                    string episode = match.Groups[1].Value.PadLeft(2, '0');

                    string destination = string.Format("S{0}E{1} - ", folderSeason, episode);
                    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                    continue;
                }

                //match = Regex.Match(tvShow.Name, @" (\d{2}) ");
                //if (match.Success)
                //{
                //    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                //    if (!folderMatch.Success)
                //    {
                //        continue;
                //    }
                //    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');



                //    string episode = match.Groups[1].Value.PadLeft(2, '0');

                //    string destination = string.Format(" S{0}E{1} ", folderSeason, episode);
                //    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                //    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                //    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                //    continue;
                //}
                //match = Regex.Match(tvShow.Name, @"(\d{3})");
                //if (match.Success)
                //{
                //    var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
                //    if (!folderMatch.Success)
                //    {
                //        continue;
                //    }
                //    string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');


                //    string seasonAndEpisode = match.Groups[1].Value.PadLeft(3, '0');
                //    string season = seasonAndEpisode.Substring(0, 1).PadLeft(2, '0');
                //    string episode = seasonAndEpisode.Substring(1, 2);
                //    if (folderSeason != season)
                //    {
                //        continue;
                //    }

                //    string destination = string.Format(" S{0}E{1} ", season, episode);
                //    string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

                //    Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

                //    Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
                //    continue;
                //}
            }
        }

        public static void FixMovieSubTitles(string path)
        {
            DirectoryInfo dInfo = new DirectoryInfo(path);
            foreach (var directoryInfo in dInfo.EnumerateDirectories())
            {
                _FixMovieSubTitles(directoryInfo);
            }
        }

        private static void MoveFiles(DirectoryInfo searchPath, string targetPath)
        {
            foreach (var directoryInfo in searchPath.EnumerateDirectories())
            {
                MoveFiles(directoryInfo, targetPath);
            }

            FileInfo[] sourcefiles = searchPath.GetFiles();

            foreach (FileInfo sourcefile in sourcefiles)
            {
                string destFile = Path.Combine(targetPath, sourcefile.Name);

                if (File.Exists(destFile))
                {
                    Console.WriteLine("Cannot move file" + destFile);
                    continue;
                }
                Move(sourcefile.FullName, destFile);
            }
        }

        private static void _FixMovieSubTitles(DirectoryInfo directoryInfo)
        {

            var allMovies = directoryInfo.GetFiles("*" + "." + "mkv", SearchOption.AllDirectories).ToList();
            allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "mp4", SearchOption.AllDirectories).ToList());
            allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "avi", SearchOption.AllDirectories).ToList());
            allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "m4v", SearchOption.AllDirectories).ToList());
            allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "mpeg", SearchOption.AllDirectories).ToList());
            allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "mpg", System.IO.SearchOption.AllDirectories).ToList());
            allMovies.AddRange(directoryInfo.GetFiles("*" + "." + "wmv", SearchOption.AllDirectories).ToList());

            if (IsTVDirectory(allMovies))
            {
                return;
            }

            if (allMovies.Count != 1)
            {
                Console.WriteLine("ERROR: Folder has multiple files {0}", directoryInfo.FullName);
                return;
            }

            #region Flatten folder structure

            var subDirs = directoryInfo.GetDirectories();
            foreach (var subDir in subDirs)
            {
                MoveFiles(subDir, directoryInfo.FullName);
            }

            subDirs = directoryInfo.GetDirectories();
            foreach (var subDir in subDirs)
            {
                if (subDir.GetFiles().Count() == 0)
                {
                    subDir.Delete();
                }
            }

            #endregion

            var movies = directoryInfo.GetFilesByExtensions(".mkv", ".mp4", ".avi", ".m4v", ".mpeg", ".mpg", ".wmv");
            var movieFileInfo = movies.First();
            string subTitleDestination;

            #region Move idx files

            var idxFiles = directoryInfo.GetFiles("*.idx", SearchOption.TopDirectoryOnly).ToList();
            if (idxFiles.Count() == 1)
            {
                subTitleDestination = movieFileInfo.FullName.Substring(0, movieFileInfo.FullName.Length - movieFileInfo.Extension.Length) + ".idx";

                Move(idxFiles.First().FullName, subTitleDestination);
            }
            else if (idxFiles.Count() > 1)
            {
                Console.WriteLine("ERROR: Folder has multiple idx files {0}", directoryInfo.FullName);
            }

            #endregion

            #region Move sub files

            var subFiles = directoryInfo.GetFiles("*.sub", SearchOption.TopDirectoryOnly).ToList();
            if (subFiles.Count() == 1)
            {
                subTitleDestination = movieFileInfo.FullName.Substring(0, movieFileInfo.FullName.Length - movieFileInfo.Extension.Length) + ".sub";

                Move(subFiles.First().FullName, subTitleDestination);
            }
            else if (subFiles.Count() > 1)
            {
                Console.WriteLine("ERROR: Folder has multiple idx files {0}", directoryInfo.FullName);
            }

            #endregion

            var movieName = movieFileInfo.Name.ToLowerInvariant();

            foreach (var keyValue in LanguageCodesNew)
            {
                var srtFiles = directoryInfo.GetFiles("*.srt", SearchOption.TopDirectoryOnly).ToList();

                var languageBasedSubtitles = srtFiles.Where(p => !p.Name.ToLowerInvariant().Contains(movieName) && keyValue.Value.Any(s => s.IsMatch(p.Name.ToLowerInvariant()))).ToList();

                var exactMatches = srtFiles.Where(p => p.Name.ToLowerInvariant().StartsWith(movieName.Substring(0, movieName.Length - movieFileInfo.Extension.Length) + "." + keyValue.Key.ToLowerInvariant() + ".")).ToList();


                languageBasedSubtitles.AddRange(exactMatches);
                languageBasedSubtitles = languageBasedSubtitles.Distinct(new FileInfoComparer()).ToList();
                RenameMovieSubTitlesBasedOnLanguage(movieFileInfo, keyValue.Key, languageBasedSubtitles);
            }

            if (directoryInfo.EnumerateDirectories().Any())
            {
                Console.WriteLine("Folder has subdirectories {0}", directoryInfo.FullName);
            }
        }

        private static void RenameMovieSubTitlesBasedOnLanguage(FileInfo movie, string languageCode, List<FileInfo> subTitleFiles)
        {
            subTitleFiles = subTitleFiles.OrderBy(p => p.Length).ThenBy(p => p.Name.Length).ToList();

            //TODO: hash contents and compare that

            List<int> filesToDelete = new List<int>();
            for (var i = 0; i < subTitleFiles.Count; i++)
            {
                var current = subTitleFiles[i];

                if (current.Length == 0)
                {
                    filesToDelete.Add(i);
                }

                if (i + 1 == subTitleFiles.Count)
                {
                    break;
                }
                var next = subTitleFiles[i + 1];



                if (current.Length == next.Length)
                {
                    var currentText = File.ReadAllText(current.FullName);
                    var nextText = File.ReadAllText(next.FullName);
                    if (string.Equals(currentText, nextText))
                    {
                        filesToDelete.Add(i);
                    }
                }
            }
            foreach (var index in filesToDelete.Distinct())
            {
                Console.WriteLine("DELETING {0} AS A DUPLICATE", subTitleFiles[index].FullName);
                Delete(subTitleFiles[index].FullName);
                subTitleFiles.RemoveAt(index);
            }

            string subTitleDestination = movie.FullName.Substring(0, movie.FullName.Length - movie.Extension.Length) + "." + languageCode;

            if (subTitleFiles.Count == 1)
            {
                var first = subTitleFiles.First();
                if (first.Length < forcedMovieCutOffSize)
                {
                    if (!Move(first.FullName, subTitleDestination + ".forced.srt"))
                    {
                        Move(first.FullName, subTitleDestination + "." + Guid.NewGuid().ToString("N") + ".forced.srt");
                    }
                }
                else
                {
                    if (!Move(first.FullName, subTitleDestination + ".srt"))
                    {
                        Move(first.FullName, subTitleDestination + "." + Guid.NewGuid().ToString("N") + ".srt");
                    }
                }
            }
            else if (subTitleFiles.Count == 2)
            {
                var first = subTitleFiles[0];
                var second = subTitleFiles[1];
                if (first.Length < forcedMovieCutOffSize)
                {
                    if (!Move(first.FullName, subTitleDestination + ".forced.srt"))
                    {
                        Move(first.FullName, subTitleDestination + "." + Guid.NewGuid().ToString("N") + ".forced.srt");
                    }
                    if (!Move(second.FullName, subTitleDestination + ".srt"))
                    {
                        Move(first.FullName, subTitleDestination + "." + Guid.NewGuid().ToString("N") + ".srt");
                    }
                }
                else
                {
                    if (!Move(second.FullName, subTitleDestination + ".sdh.srt"))
                    {
                        Move(first.FullName, subTitleDestination + "." + Guid.NewGuid().ToString("N") + ".sdh.srt");
                    }
                    if (!Move(first.FullName, subTitleDestination + ".srt"))
                    {
                        Move(first.FullName, subTitleDestination + "." + Guid.NewGuid().ToString("N") + ".srt");
                    }
                }
            }
            else if (subTitleFiles.Count == 3)
            {
                var first = subTitleFiles[0];
                var second = subTitleFiles[1];
                var third = subTitleFiles[2];
                if (first.Length < forcedMovieCutOffSize)
                {
                    if (!Move(first.FullName, subTitleDestination + ".forced.srt"))
                    {
                        Move(first.FullName, subTitleDestination + "." + Guid.NewGuid().ToString("N") + ".forced.srt");
                    }
                    if (!Move(third.FullName, subTitleDestination + ".sdh.srt"))
                    {
                        Move(first.FullName, subTitleDestination + "." + Guid.NewGuid().ToString("N") + ".sdh.srt");
                    }
                    if (!Move(second.FullName, subTitleDestination + ".srt"))
                    {
                        Move(first.FullName, subTitleDestination + "." + Guid.NewGuid().ToString("N") + ".srt");
                    }
                }
                else
                {
                    Console.WriteLine("ERROR: Folder has unknown subtitle {0}", first.FullName);
                    if (!Move(third.FullName, subTitleDestination + ".sdh.srt"))
                    {
                        Move(first.FullName, subTitleDestination + "." + Guid.NewGuid().ToString("N") + ".sdh.srt");
                    }
                    if (!Move(second.FullName, subTitleDestination + ".srt"))
                    {
                        Move(first.FullName, subTitleDestination + "." + Guid.NewGuid().ToString("N") + ".srt");
                    }
                }
            }
            else if (subTitleFiles.Count > 3)
            {
                Console.WriteLine("ERROR: Folder has too many subtitles {0}", movie.FullName);
            }
        }

        private static void FixTVShowSubTitles(DirectoryInfo directoryInfo, string extension)
        {
            var tvShows = directoryInfo.GetFilesByExtensions(".mkv", ".mp4", ".avi", ".m4v");
            if (tvShows.Count() < 1)
            {
                var subDirs = directoryInfo.GetDirectories();
                if (subDirs.Length == 0)
                {
                    Console.WriteLine(directoryInfo.FullName + " has zero files");
                    return;
                }
                foreach (var subDir in subDirs)
                {
                    FixTVShowSubTitles(subDir, extension);
                }
                return;
            }

            foreach (var tvShow in tvShows)
            {
                SearchLanguage(directoryInfo, tvShows.Count(), tvShow, "English", "Eng", "en", "srt");
                SearchLanguage(directoryInfo, tvShows.Count(), tvShow, "English", "Eng", "en", "idx");
                SearchLanguage(directoryInfo, tvShows.Count(), tvShow, "English", "Eng", "en", "sub");
                foreach (var languageCode in LanguageCodes)
                {
                    SearchLanguage(directoryInfo, tvShows.Count(), tvShow, languageCode.Item1, null, languageCode.Item2, "srt");
                }
            }

        }

        private static void SearchLanguage(DirectoryInfo directoryInfo, int tvShowsCount, FileInfo tvShow, string search, string altSearch, string code, string extension)
        {
            var subTitleDestination = tvShow.FullName.Substring(0, tvShow.FullName.Length - tvShow.Extension.Length) + "." + code + "." + extension;

            if (File.Exists(subTitleDestination))
            {
                return;
            }

            var match = Regex.Match(tvShow.Name, @"[Ss](\d+)[Ee](\d+)");
            var unkonwnFileName = Regex.Match(tvShow.Name, @"[Ss](\d+)[Ee](\d+)");
            if (!match.Success)
            {
                return;
            }

            //Assume 1 match with no lang code is english
            var subTitlesFiles = directoryInfo.GetFiles("*" + match.Groups[0].Value + "*" + "." + extension, SearchOption.AllDirectories);
            subTitlesFiles = subTitlesFiles.Where(p => LanguageCodes.Select(l => l.Item2 + "." + extension).All(l => !p.Name.EndsWith(l)))
                .Where(p => !p.Name.ToLower().EndsWith(".en." + extension)).ToArray();
            if (subTitlesFiles.Count() == 1)
            {
                var singleMatchName = subTitlesFiles.First().Name;
                var matchPattern = tvShow.Name.Substring(0, tvShow.Name.Length - tvShow.Extension.Length) + "\\.\\w{2}\\." + extension;
                match = Regex.Match(singleMatchName, matchPattern);
                if (!match.Success)
                {
                    Move(subTitlesFiles.First().FullName, subTitleDestination);
                    return;
                }
            }

            if (tvShowsCount == 1)
            {
                subTitlesFiles = directoryInfo.GetFiles("*" + search + "*" + "." + extension, SearchOption.AllDirectories).Where(p => p.Length > minFileSize).OrderBy(p => p.Length).ToArray();

                if (subTitlesFiles.Any())
                {
                    Move(subTitlesFiles.First().FullName, subTitleDestination);
                    return;
                }
                if (subTitlesFiles.Count() == 0 && !string.IsNullOrWhiteSpace(altSearch))
                {
                    subTitlesFiles = directoryInfo.GetFiles("*" + altSearch + "*" + "." + extension, SearchOption.AllDirectories).Where(p => p.Length > minFileSize).OrderBy(p => p.Length).ToArray();

                    if (subTitlesFiles.Any())
                    {
                        Move(subTitlesFiles.First().FullName, subTitleDestination);
                        return;
                    }
                }
            }

            if (tvShowsCount > 1)
            {
                var subdirs = directoryInfo.GetDirectories(tvShow.Name.Replace(tvShow.Extension, ""), SearchOption.AllDirectories);
                if (subdirs.Length == 1)
                {
                    var subdir = subdirs.First();

                    subTitlesFiles = subdir.GetFiles("*" + search + "*" + "." + extension, SearchOption.AllDirectories).Where(p => p.Length > minFileSize).OrderBy(p => p.Length).ToArray();
                    if (subTitlesFiles.Any())
                    {
                        subTitleDestination = tvShow.FullName.Substring(0, tvShow.FullName.Length - tvShow.Extension.Length) + "." + code + "." + extension;
                        Console.WriteLine("Creating " + subTitleDestination);
                        Move(subTitlesFiles.First().FullName, subTitleDestination);
                        return;
                    }
                    else if (search == "English" && subdir.FullName.Contains(match.Value))
                    {
                        //if searching for english assume any unknown srt file is english
                        subTitlesFiles = subdir.GetFiles("*" + "." + extension, SearchOption.AllDirectories).Where(p => p.Length > minFileSize).Where(p => Regex.IsMatch(p.Name.Replace(p.Extension, ""), @"[^a-zA-Z]+")).OrderBy(p => p.Length).ToArray();
                        if (subTitlesFiles.Any())
                        {
                            subTitleDestination = tvShow.FullName.Substring(0, tvShow.FullName.Length - tvShow.Extension.Length) + "." + code + "." + extension;
                            Console.WriteLine("Creating " + subTitleDestination);
                            Move(subTitlesFiles.First().FullName, subTitleDestination);
                            return;
                        }
                    }

                }
            }
        }

        private static bool Move(string sourceFileName, string destFileName)
        {
            if (string.Equals(sourceFileName, destFileName, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (File.Exists(destFileName))
            {
                Console.WriteLine("ERROR: Cannot move file {0}", destFileName);
                return false;
            }

            Console.WriteLine("RENAMING {0} to {1}", sourceFileName, destFileName);

            if (CanMove)
            {
                File.Move(sourceFileName, destFileName);
            }

            return true;
        }

        private static void Delete(string fullName)
        {
            Console.WriteLine("DELETING {0}", fullName);

            if (CanDelete)
            {
                File.Delete(fullName);
            }
        }

        private static bool MatchFourAndThreeDigitYearAndSeason(DirectoryInfo directoryInfo, FileInfo tvShow, Match match)
        {
            if (!match.Success)
            {
                return false;
            }
            var folderMatch = Regex.Match(directoryInfo.Name, @"^Season (\d+)$");
            if (!folderMatch.Success)
            {
                return false;
            }
            string folderSeason = folderMatch.Groups[1].Value.PadLeft(2, '0');


            string total = match.Groups[1].Value.PadLeft(4, '0');
            string season = total.Substring(0, 2);
            string episode = total.Substring(2, 2);

            if (folderSeason != season)
            {
                return false;
            }

            string destination = string.Format(" - S{0}E{1} - ", season, episode);
            string newFileName = tvShow.Name.Replace(match.Groups[0].Value, destination);

            if (VideoFileExtensions.All(p => !newFileName.EndsWith(p)))
            {
                throw new Exception("Invalid File Name");
            }

            Console.WriteLine("Renaming {0} to {1}", tvShow.Name, newFileName);

            Move(tvShow.FullName, tvShow.FullName.Replace(tvShow.Name, newFileName));
            return true;
        }

        private static bool IsTVDirectory(List<FileInfo> files)
        {
            return files.Any(p => TVShowRegex.Match(p.Name).Success);
        }

        private class FileInfoComparer : EqualityComparer<FileInfo>
        {
            public override bool Equals(FileInfo x, FileInfo y)
            {
                return x.FullName == y.FullName;
            }

            public override int GetHashCode(FileInfo obj)
            {
                return obj.GetHashCode();
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
