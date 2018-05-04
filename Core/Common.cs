using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace UrlCrawl.Core
{
    public class Common
    {

        /// <summary>
        /// 正则获得匹配的字符串
        /// </summary>
        /// <param name="expression">正则表达式</param>
        /// <param name="strContent">匹配的内容</param>
        /// <returns></returns>
        public static string MatchRegex(string expression, string strContent)
        {
            var str = "";
            if (!string.IsNullOrEmpty(strContent) && !string.IsNullOrEmpty(expression))
            {
                Regex regex = new Regex(expression);
                Match match = regex.Match(strContent);
                if (match.Success)
                {
                    str = match.Groups[1].Value;
                }
            }
            return str;
        }

        /// <summary>
        /// 正则获得匹配的对象
        /// </summary>
        /// <param name="expression">正则表达式</param>
        /// <param name="strContent">内容</param>
        /// <returns></returns>
        public static MatchCollection MatchCollectionRegex(string expression, string strContent)
        {

            if (!string.IsNullOrEmpty(expression) && !string.IsNullOrEmpty(strContent))
            {
                Regex regex = new Regex(expression);
                MatchCollection matchCollection = regex.Matches(strContent);
                return matchCollection;
            }
            return null;
        }

        public static string BuildPostData(NameValueCollection parameter, Encoding encoding)
        {

            StringBuilder sb = new StringBuilder();
            if (parameter != null)
            {
                foreach (string key in parameter)
                {
                    sb.Append(string.Format("{0}={1}&", System.Web.HttpUtility.UrlEncode(key, encoding), System.Web.HttpUtility.UrlEncode(parameter[key], encoding)));
                }
            }
            if (sb.Length > 0)
            {
                sb = sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        public static string ReBuildUrl(string url, NameValueCollection nameValueCollection, Encoding encode)
        {
            UriBuilder urlBuilder = new UriBuilder(url);
            if (nameValueCollection != null && nameValueCollection.Count > 0)
            {
                if (string.IsNullOrEmpty(urlBuilder.Query))
                {
                    urlBuilder.Query = BuildGetNoQuery(nameValueCollection, encode);
                }
                else
                {
                    urlBuilder.Query.Insert(urlBuilder.Query.Length, BuildGetExistQuery(nameValueCollection, encode));
                }
            }
            return urlBuilder.Uri.AbsoluteUri;
        }

        public static string BuildGetNoQuery(NameValueCollection nameValueCollection, Encoding encode)
        {
            StringBuilder sbQuery = new StringBuilder();
            foreach (string key in nameValueCollection.Keys)
            {
                sbQuery.Append(HttpUtility.UrlEncode(key, encode));
                sbQuery.Append('=');
                sbQuery.Append(HttpUtility.UrlEncode(nameValueCollection[key], encode));
                sbQuery.Append('&');

            }
            return sbQuery.ToString().TrimEnd('&');
        }

        public static string BuildGetExistQuery(NameValueCollection nameValueCollection, Encoding encode)
        {
            StringBuilder sbQuery = new StringBuilder();
            foreach (string key in nameValueCollection.Keys)
            {
                sbQuery.Append('&');
                sbQuery.Append(HttpUtility.UrlEncode(key, encode));
                sbQuery.Append('=');
                sbQuery.Append(HttpUtility.UrlEncode(nameValueCollection[key], encode));
            }
            return sbQuery.ToString();
        }

        public static string SaveFile(byte[] bytes, string fileType, string fileName, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += fileName + "." + fileType;
            var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
            return path;
        }


        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="filePath"></param>
        public static Dictionary<string, string> GetXMl(string xpath, string filePath)
        {
            Dictionary<string, string> maps = new Dictionary<string, string>();
            XmlDocument xmldoc = new XmlDocument();

            xmldoc.Load(filePath);

            XmlNode node = xmldoc.DocumentElement.SelectSingleNode(xpath);

            if (node != null)
            {
                var nodeList = node.ChildNodes;
                string key = string.Empty;
                string value = string.Empty;

                foreach (XmlNode n in nodeList)
                {
                    key = n.Attributes["key"].Value;
                    value = n.Attributes["value"].Value;

                    maps.Add(key, value);
                }
            }
            return maps;
        }

        public static string UrlCheck(string strUrl,string httpScheme, string host = "", string relativeUrl = "")
        {
            if (!strUrl.Contains("http://") && !strUrl.Contains("https://"))
            {
                if (host != "" && relativeUrl != "")
                {
                    strUrl =httpScheme +"://"+ host + relativeUrl;
                }
                else
                {
                    strUrl = httpScheme+"://" + strUrl;
                }
            }
            return strUrl;
        }
    }
}
