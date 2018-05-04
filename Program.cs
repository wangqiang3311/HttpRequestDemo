using ConsoleApplication2;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UrlCrawl.main;
using UrlCrawl.Model;

namespace HttpRequestTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var uri = "https://www.emeraldinsight.com/action/doSearch?AllField=computer&content=articlesChapters";

            TestMethod1(uri);

            string html = TestMethod2(uri, "");

            Console.WriteLine(html);
            TestMethod3();
            Console.Read();

        }

        public static void TestMethod1(string url)
        {

            Crawl c = new Crawl();


            c.EnterQueue(new HttpItem()
            {
                URL = url,

                HandleResult = (result) =>
                {
                    Console.WriteLine(result);
                },
                Method = "Get",
            });

            c.Start();
        }

        public static string TestMethod2(string Url, string cookieStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.Timeout = 1000 * 900;

            request.Headers.Add(HttpRequestHeader.Cookie, cookieStr);

            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2;

            string back = "";
            try
            {
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                back = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                response.Close();
            }
            catch (Exception ex)
            {

                back = ex.Message;
            }



            return back;
        }


        static void TestMethod3()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            string url = "https://www.emeraldinsight.com/action/doSearch?AllField=computer&content=articlesChapters&cookie=I2KBRCK=1";
          
            //post 请求

            List<KeyValuePair<string, string>> pars = new List<KeyValuePair<string, string>>();
            pars.Add(new KeyValuePair<string, string>("doi", "10.1108/al.1999.8.2.54.1"));
            pars.Add(new KeyValuePair<string, string>("doi", "10.1108/IJPCC-02-2017-0012"));
            
            pars.Add(new KeyValuePair<string, string>("format", "bibtex"));
            pars.Add(new KeyValuePair<string, string>("cookie", "I2KBRCK=1"));


            url = "https://www.emeraldinsight.com/action/downloadCitation";
            var r = HTTPClientHelper.HttpPostRequestAsync(url, pars);

            File.WriteAllText(@"d:/mydata.html", r);

            if (r.Length > 50)

                Console.WriteLine("ok");
            else
                Console.WriteLine("empty");

            Console.Read();

        }
    }
}
