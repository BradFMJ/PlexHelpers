using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace PlexHelpers.Web
{
    /// <summary>
    /// Summary description for mitm
    /// </summary>
    public class mitm : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            List<string> log = new List<string>();
            int responseCode = 200;
            try
            {
                //Tell .net to ignore SSL Errors
                System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate (
                    object sender,
                    X509Certificate cert,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

                var interceptedRequest = HttpContext.Current.Request;
                var newResponse = context.Response;

                var newUrl = new UriBuilder();
                newUrl.Scheme = interceptedRequest.Url.Scheme;
                //todo: remove this
                newUrl.Scheme = "https";
                newUrl.Host = "my.photoprism.app";
                var rawUrlParts = interceptedRequest.RawUrl.Split('?');
                newUrl.Path = rawUrlParts[0];
                newUrl.Query = rawUrlParts.Length > 1 ? rawUrlParts[1] : "";

                string url = newUrl.ToString();
                url = url.Replace("/mitm.ashx", "");
                HttpWebRequest newRequest = (HttpWebRequest)WebRequest.Create(url);
                newRequest.Method = interceptedRequest.HttpMethod;
                newRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                log.Add($"{interceptedRequest.HttpMethod} {interceptedRequest.Url.Scheme}://{interceptedRequest.Url.Host}{interceptedRequest.Path}{interceptedRequest.Url.Query}");

                foreach (var key in interceptedRequest.Headers.AllKeys)
                {
                    log.Add($"{key}: {interceptedRequest.Headers[key]}");

                    if (key.ToLowerInvariant() == "host")
                    {
                        //WebRequest will handle this based on the URL its asking for
                        continue;
                    }
                    if (key.ToLowerInvariant() == "referer")
                    {
                        //TODO: Fill this back in
                        continue;
                    }
                    if (key.ToLowerInvariant() == "connection")
                    {
                        //Cannot set this via headers, must use property of WebRequest
                        newRequest.KeepAlive = interceptedRequest.Headers[key] == "keep-alive";
                        continue;
                    }
                    if (key.ToLowerInvariant() == "content-type")
                    {
                        //Cannot set this via headers, must use property of WebRequest
                        newRequest.ContentType = interceptedRequest.Headers[key];
                        continue;
                    }
                    if (key.ToLowerInvariant() == "content-length")
                    {
                        //WebRequest will handle this based on the request
                        continue;
                    }
                    if (key.ToLowerInvariant() == "accept")
                    {
                        //Cannot set this via headers, must use property of WebRequest
                        newRequest.Accept = interceptedRequest.Headers[key];
                        continue;
                    }
                    if (key.ToLowerInvariant() == "user-agent")
                    {
                        //Cannot set this via headers, must use property of WebRequest
                        newRequest.UserAgent = interceptedRequest.Headers[key];
                        continue;
                    }
                    if (key.ToLowerInvariant() == "if-modified-since")
                    {
                        //Cannot set this via headers, must use property of WebRequest
                        newRequest.IfModifiedSince = DateTime.Parse(interceptedRequest.Headers[key]);
                        continue;
                    }
                    newRequest.Headers.Add(key, interceptedRequest.Headers[key]);
                }

                if (interceptedRequest.HttpMethod == WebRequestMethods.Http.Post
                    || interceptedRequest.HttpMethod == WebRequestMethods.Http.Put)
                {
                    if (interceptedRequest.InputStream.CanSeek)
                    {
                        interceptedRequest.InputStream.Position = 0;
                        var rawRequestBody = new StreamReader(interceptedRequest.InputStream).ReadToEnd();
                        log.Add(rawRequestBody);
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        byte[] byte1 = encoding.GetBytes(rawRequestBody);
                        newRequest.ContentLength = byte1.Length;
                        Stream newStream = newRequest.GetRequestStream();
                        newStream.Write(byte1, 0, byte1.Length);
                    }

                }

                HttpWebResponse interceptedResponse = (HttpWebResponse)newRequest.GetResponse();
                responseCode = (int)interceptedResponse.StatusCode;
                newResponse.StatusCode = (int)interceptedResponse.StatusCode;
                log.Add("");
                log.Add($"{newRequest.Method} {newRequest.RequestUri.Scheme}://{newRequest.RequestUri.Host}{newRequest.RequestUri.PathAndQuery}");

                foreach (var key in interceptedResponse.Headers.AllKeys)
                {
                    log.Add($"{key}: {interceptedResponse.Headers[key]}");

                    if (key.ToLowerInvariant() == "content-encoding")
                    {
                        //Content-Encoding has been removed automatically by WebRequest.
                        //We need the response decoded anyways to manipulate it.
                        //Leaving encoding off, no reason to re-encode at this point.
                        continue;
                    }
                    if (key.ToLowerInvariant() == "transfer-encoding")
                    {
                        //same as content-encoding for our purposes
                        continue;
                    }
                    if (key.ToLowerInvariant() == "content-length")
                    {
                        //HttpContext will handle this on newResponse
                        continue;
                    }
                    if (key.ToLowerInvariant() == "content-type")
                    {
                        //Cannot set this via headers, must use property of HttpContext
                        newResponse.ContentType = interceptedResponse.Headers[key];
                        continue;
                    }

                    newResponse.Headers.Add(key, interceptedResponse.Headers[key]);
                }

                if (newResponse.ContentType.StartsWith("Foo"))
                {
                    using (Stream stream = interceptedResponse.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        String responseString = reader.ReadToEnd();
                        log.Add(responseString);
                        //TODO: Manipulate Response;

                        newResponse.Write(responseString);
                    }
                }
                else
                {
                    using (Stream stream = interceptedResponse.GetResponseStream())
                    {
                        stream.CopyTo(newResponse.OutputStream);
                    }
                }


                interceptedResponse.Close();
            }
            catch (Exception ex)
            {
                log.Add(ex != null ? ex.ToString() : "");
                log.Add(ex.InnerException != null ? ex.InnerException.ToString() : "");
                if (responseCode == 200)
                {
                    responseCode = 400;
                }
            }
            finally
            {
                var fileName = System.AppDomain.CurrentDomain.BaseDirectory + responseCode + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
                File.WriteAllLines(fileName, log);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}