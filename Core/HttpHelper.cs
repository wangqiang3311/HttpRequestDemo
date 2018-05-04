using System;  
using System.Collections.Generic;  
using System.Text;  
using System.Net;  
using System.IO;  
using System.Text.RegularExpressions;  
using System.IO.Compression;  
using System.Security.Cryptography.X509Certificates;  
using System.Net.Security;
using UrlCrawl.Model;

namespace UrlCrawl.Core
{
    public class HttpHelper
    {
        private Encoding encoding = Encoding.Default;
        private HttpWebRequest request = null;
        private HttpWebResponse response = null;

        public HttpResult GetResult(HttpItem item)
        {
            HttpResult result = new HttpResult();
            try
            {
                SetRequest(item);
            }
            catch (Exception ex)
            {
                result.Cookie = string.Empty;
                result.Header = null;
                result.Html = ex.Message;
                result.StatusDescription = "配置参数时出错：" + ex.Message;
                return result;
            }
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    GetData(item, result);
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (response = (HttpWebResponse)ex.Response)
                    {
                        GetData(item, result,true);
                    }
                }
                else
                {
                    result.Html = ex.Message;
                }
            }
            catch (Exception ex)
            {
                result.Html = ex.Message;
            }
            if (item.IsToLower) result.Html = result.Html.ToLower();
            return result;
        }

        #region GetData

        /// <summary>  
        /// 获取数据
        /// </summary>  
        /// <param name="item"></param>  
        /// <param name="result"></param>  
        private void GetData(HttpItem item, HttpResult result,bool iserror=false)
        {
            string location = response.GetResponseHeader("Location");

            if (!string.IsNullOrWhiteSpace(location))
            {
                result.RedirectUrl = location;
            }
            result.StatusCode = response.StatusCode;
            result.StatusDescription = response.StatusDescription;
            result.Header = response.Headers;
            if (response.Cookies != null) result.CookieCollection = response.Cookies;
            if (response.Headers["set-cookie"] != null) result.Cookie = response.Headers["set-cookie"];

            if (!string.IsNullOrEmpty(item.CookieKey))
            {
                if (item.IsDownLoadFile)
                {
                    CookieHelper.GetCookie(item.CookieKey).SetCookies(response.ResponseUri, response.Headers[HttpResponseHeader.Server]);
                }
                else
                {
                    CookieHelper.SetCookie(item.CookieKey, result.Cookie, item.CookieDomain ?? "domain", item.CookiePath ?? "/");
                }
            }

            if (iserror == false)
            {
                if (item.IsDownLoadFile)
                {
                    byte[] responseByte = GetByte();
                    string path = Common.SaveFile(responseByte, item.DownLoadFileType, item.DownLoadFileName, item.DownLoadBaseUrl);
                    item.DownLoadFilePath = path;
                }
                else
                {
                    result.Html = GetStringFromResponse();
                }
            }
        }
        /// <summary>  
        /// 设置编码  
        /// </summary>  
        /// <param name="item">HttpItem</param>  
        /// <param name="result">HttpResult</param>  
        /// <param name="ResponseByte">byte[]</param>  
        private void SetEncoding(HttpItem item, HttpResult result, byte[] ResponseByte)
        {
            if (item.ResultType == ResultType.Byte) result.ResultByte = ResponseByte;
            if (encoding == null)
            {
                Match meta = Regex.Match(Encoding.Default.GetString(ResponseByte), "<meta[^<]*charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                string c = string.Empty;
                if (meta != null && meta.Groups.Count > 0)
                {
                    c = meta.Groups[1].Value.ToLower().Trim();
                }
                if (c.Length > 2)
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(c.Replace("\"", string.Empty).Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk").Trim());
                    }
                    catch
                    {
                        if (string.IsNullOrEmpty(response.CharacterSet))
                        {
                            encoding = Encoding.UTF8;
                        }
                        else
                        {
                            encoding = Encoding.GetEncoding(response.CharacterSet);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(response.CharacterSet))
                    {
                        encoding = Encoding.UTF8;
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(response.CharacterSet);
                    }
                }
            }
        }
        /// <summary>  
        /// 提取网页Byte  
        /// </summary>  
        /// <returns></returns>  
        private byte[] GetByte()
        {
            byte[] ResponseByte = null;
            MemoryStream _stream = new MemoryStream();

           var res= GetResponseStream();
            _stream = GetMemoryStream(res);
            //获取Byte  
            ResponseByte = _stream.ToArray();
            _stream.Close();
            return ResponseByte;
        }

        private Stream GetResponseStream()
        {
            Stream s=null;

            if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
            {
                s= new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
            }
            else
            {
                s = response.GetResponseStream();
            }
            return s;
        }

        /// <summary>  
        /// 4.0以下.net版本取数据使用  
        /// </summary>  
        /// <param name="streamResponse">流</param>  
        private MemoryStream GetMemoryStream(Stream streamResponse)
        {
            MemoryStream _stream = new MemoryStream();

            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = streamResponse.Read(buffer, 0, Length);
            while (bytesRead > 0)
            {
                _stream.Write(buffer, 0, bytesRead);
                bytesRead = streamResponse.Read(buffer, 0, Length);
            }
            return _stream;
        }

        private string GetStringFromResponse()
        {
            var res = GetResponseStream();

            using (StreamReader sr = new StreamReader(res))
            {
              return sr.ReadToEnd();
            }
        }


        #endregion

        #region SetRequest

        /// <summary>  
        /// 为请求准备参数  
        /// </summary>  
        ///<param name="item">参数列表</param>  
        private void SetRequest(HttpItem item)
        {
            // 验证证书  
            SetCer(item);
            //设置Header参数  
            if (item.Header != null && item.Header.Count > 0)
            {
                foreach (string key in item.Header.AllKeys)
                {
                    request.Headers.Add(key, item.Header[key]);
                }
            }
            // 设置代理  
            SetProxy(item);
            if (item.ProtocolVersion != null) request.ProtocolVersion = item.ProtocolVersion;
            request.ServicePoint.Expect100Continue = item.Expect100Continue;
            //请求方式Get或者Post  
            request.Method = item.Method;
            request.Timeout = item.Timeout;
            request.KeepAlive = item.KeepAlive;
            request.ReadWriteTimeout = item.ReadWriteTimeout;
            if (item.IfModifiedSince != null) request.IfModifiedSince = Convert.ToDateTime(item.IfModifiedSince);

            request.Accept = item.Accept;

            if (request.Method.ToUpper() == "POST")
            {
                item.ContentType= "application/x-www-form-urlencoded";
            }

            request.ContentType = item.ContentType;

            request.UserAgent = item.UserAgent;

            encoding = item.Encoding;

            request.Credentials = item.ICredentials;
            SetCookie(item);

            item.Host = request.Host;

            request.Referer = item.Referer;
            request.AllowAutoRedirect = item.Allowautoredirect;

            request.MaximumAutomaticRedirections = item.MaximumAutomaticRedirections == 0 ? 10000 : item.MaximumAutomaticRedirections;

            //设置请求数据  
            SetData(item);
            //设置最大连接  
            if (item.Connectionlimit > 0) request.ServicePoint.ConnectionLimit = item.Connectionlimit;
        }
        /// <summary>  
        /// 设置证书  
        /// </summary>  
        /// <param name="item"></param>  
        private void SetCer(HttpItem item)
        {

            if (item.URL.StartsWith("https"))
            {
    
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                request = (HttpWebRequest)WebRequest.Create(item.URL);

                if (!string.IsNullOrEmpty(item.CerPath))
                {
                    SetCerList(item);
                    request.ClientCertificates.Add(new X509Certificate(item.CerPath));
                }
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(item.URL);
                SetCerList(item);
            }
        }
        /// <summary>  
        /// 设置多个证书  
        /// </summary>  
        /// <param name="item"></param>  
        private void SetCerList(HttpItem item)
        {
            if (item.ClentCertificates != null && item.ClentCertificates.Count > 0)
            {
                foreach (X509Certificate c in item.ClentCertificates)
                {
                    request.ClientCertificates.Add(c);
                }
            }
        }
        /// <summary>  
        /// 设置Cookie  
        /// </summary>  
        /// <param name="item">Http参数</param>  
        private void SetCookie(HttpItem item)
        {
            if (!string.IsNullOrEmpty(item.Cookie)) request.Headers[HttpRequestHeader.Cookie] = item.Cookie;
            //设置CookieCollection  
            if (item.ResultCookieType == ResultCookieType.CookieCollection)
            {
                request.CookieContainer = new CookieContainer();
                if (item.CookieCollection != null && item.CookieCollection.Count > 0)
                    request.CookieContainer.Add(item.CookieCollection);
            }

            if (!string.IsNullOrEmpty(item.CookieKey))
            {
                request.CookieContainer = CookieHelper.GetCookie(item.CookieKey);
            }

        }
        /// <summary>  
        /// 设置请求数据  
        /// </summary>  
        /// <param name="item">Http参数</param>  
        private void SetData(HttpItem item)
        {
            //post请求数据
            if (!request.Method.Trim().ToLower().Contains("get"))
            {
                if (item.Encoding != null)
                {
                    encoding = item.Encoding;
                }
                else
                {
                    encoding = Encoding.UTF8;
                }

                string postData = Common.BuildPostData(item.Postdata, encoding);

               
                byte[] buffer = null;
                if (item.PostDataType == PostDataType.Byte && item.PostdataByte != null && item.PostdataByte.Length > 0)
                {
                    buffer = item.PostdataByte;
                }
                else if (item.PostDataType == PostDataType.FilePath && !string.IsNullOrEmpty(postData))
                {
                    StreamReader r = new StreamReader(postData, encoding);
                    buffer = encoding.GetBytes(r.ReadToEnd());
                    r.Close();
                }
                else if (!string.IsNullOrEmpty(postData))
                {
                    buffer = encoding.GetBytes(postData);
                }
                if (buffer != null)
                {
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }
            }
            else
            {
                //get请求数据
                item.URL = Common.ReBuildUrl(item.URL, item.Getdata, encoding);
            }
        }
        /// <summary>  
        /// 设置代理  
        /// </summary>  
        /// <param name="item">参数对象</param>  
        private void SetProxy(HttpItem item)
        {
            bool isIeProxy = false;
            if (!string.IsNullOrEmpty(item.ProxyIp))
            {
                isIeProxy = item.ProxyIp.ToLower().Contains("ieproxy");
            }
            if (!string.IsNullOrEmpty(item.ProxyIp) && !isIeProxy)
            {
                if (item.ProxyIp.Contains(":"))
                {
                    string[] plist = item.ProxyIp.Split(':');
                    WebProxy myProxy = new WebProxy(plist[0].Trim(), Convert.ToInt32(plist[1].Trim()));
                    myProxy.Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd);
                    request.Proxy = myProxy;
                }
                else
                {
                    WebProxy myProxy = new WebProxy(item.ProxyIp, false);
                    myProxy.Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd);
                    request.Proxy = myProxy;
                }
            }
            else if (isIeProxy)
            {
             
            }
            else
            {
                request.Proxy = item.WebProxy;
            }
        }
        #endregion

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }

    }
}

