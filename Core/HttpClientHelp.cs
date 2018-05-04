using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ConsoleApplication2
{
    public class HTTPClientHelper
    {
        private static readonly HttpClient HttpClient;
        static HTTPClientHelper()
        {
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };
            HttpClient = new HttpClient(handler);
        }


        /// <summary>
        /// get请求，可以对请求头进行多项设置
        /// </summary>
        /// <param name="paramArray"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetResponseByGet(List<KeyValuePair<string,string>> paramArray, string url)
        {
            string result = "";

            var httpclient = HTTPClientHelper.HttpClient;

            url = url + "?" + BuildParam(paramArray);
            var response = httpclient.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                Stream myResponseStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                result = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
            }

            return result;
        }
        public static string GetResponseBySimpleGet(List<KeyValuePair<string,string>> paramArray, string url)
        {

            var httpclient = HTTPClientHelper.HttpClient;

            url = url + "?" + BuildParam(paramArray);
            var result = httpclient.GetStringAsync(url).Result;
            return result;
        }

        public static string HttpPostRequestAsync(string Url, List<KeyValuePair<string, string>> paramArray, string ContentType = "application/x-www-form-urlencoded")
        {
            string result = "";

            var postData = BuildParam(paramArray);

            var data = Encoding.ASCII.GetBytes(postData);

            try
            {
                using (HttpClient http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (compatible; Baiduspider/2.0; +http://www.baidu.com/search/spider.html)");
                    http.DefaultRequestHeaders.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

                    HttpResponseMessage message = null;
                    using (Stream dataStream = new MemoryStream(data ?? new byte[0]))
                    {
                        using (HttpContent content = new StreamContent(dataStream))
                        {
                            content.Headers.Add("Content-Type", ContentType);
                            var task = http.PostAsync(Url, content);
                            message = task.Result;
                        }
                    }
                    if (message != null && message.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        using (message)
                        {
                            result = message.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        private static string Encode(string content, Encoding encode = null)
        {
            if (encode == null) return content;

            return System.Web.HttpUtility.UrlEncode(content, Encoding.UTF8);

        }

        private static string BuildParam(List<KeyValuePair<string, string>> paramArray, Encoding encode = null)
        {
            string url = "";

            if (encode == null) encode = Encoding.UTF8;

            if (paramArray != null && paramArray.Count > 0)
            {
                var parms = "";
                foreach (var item in paramArray)
                {
                    parms += string.Format("{0}={1}&", Encode(item.Key, encode), Encode(item.Value, encode));
                }
                if (parms != "")
                {
                    parms = parms.TrimEnd('&');
                }
                url += parms;

            }
            return url;
        }
    }

}
