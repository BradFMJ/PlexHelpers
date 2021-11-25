using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Services;
using System.Web.Services;

namespace PlexHelpers.Web
{
    /// <summary>
    /// Summary description for imdb
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class imdb : System.Web.Services.WebService
    {
        static readonly object Object = new object();

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string LogMove(string year, string id, string name, string ratings)
        {
            var yearRegex = new Regex("\\d+");
            year = yearRegex.Match(year).Value;

            var idRegex = new Regex("tt\\d+");
            id = idRegex.Match(id).Value;

            string line = id + "," + year + "," + name.Replace(",", "") + "," + ratings.Replace(",", "");

            lock (Object)
            {
                //File.AppendText("C:\\imdb\\" + year + ".csv")
                //using (var fs = File.Open("C:\\imdb\\" + year + ".csv", FileMode.OpenOrCreate))
                using (var sw = File.AppendText("C:\\imdb\\" + year + ".csv"))
                {
                    sw.WriteLine(line);
                }
            }

            return line;
        }
    }
}