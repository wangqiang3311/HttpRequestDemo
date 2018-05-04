using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UrlCrawl.Core
{
    public class CookieHelper
    {
        static Dictionary<string, CookieContainer> cookieDic = new Dictionary<string, CookieContainer>();

        private static object lock4 = new object();

        public static CookieContainer GetCookie(string key)
        {
            lock (lock4)
            {
                if (!cookieDic.Keys.Contains(key))
                {
                    cookieDic.Add(key, new CookieContainer());
                }
                return cookieDic[key];
            }
        }

        public static void SetCookie(string key,string result, string domain, string path="/")
        {
            if (!string.IsNullOrEmpty(result))
            {
                var first = result.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in first)
                {
                    if (s.Contains("GMT"))
                    {
                        continue;
                    }
                    var tow = s.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    var keyValue = tow[0].Split('=');

                    if (cookieDic.Keys.Contains(key))
                    {
                        cookieDic[key].Add(new Cookie(keyValue[0], keyValue[1], path, domain));
                    }
                }
            }
        }
    }
}
