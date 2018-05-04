using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace UrlCrawl.Model
{
    /// <summary>  
    /// Http请求/响应model
    /// </summary>  
    public class HttpItem
    {
        public string URL { set; get; }

        private string method = "GET";
        public string Method
        {
            get { return method; }
            set { method = value; }
        }

        private int timeout = 100000;
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        private int readWriteTimeout = 30000;
        /// <summary>  
        /// 默认写入Post数据超时间  
        /// </summary>  
        public int ReadWriteTimeout
        {
            get { return readWriteTimeout; }
            set { readWriteTimeout = value; }
        }

        private Boolean keepAlive = true;
        public Boolean KeepAlive
        {
            get { return keepAlive; }
            set { keepAlive = value; }
        }
        private string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        public string Accept
        {
            get { return accept; }
            set { accept = value; }
        }
        
        private string contentType = "text/html;charset=UTF-8";
        public string ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }

        private string userAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.130 Safari/537.36";
        public string UserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
        }

        public Encoding Encoding { set; get; }
      
         /// <summary>  
        /// Post的数据类型  
        /// </summary>  
        public PostDataType PostDataType{set;get;}

        /// <summary>
        /// post请求发送的数据
        /// </summary>
        public NameValueCollection Postdata { set; get; }
        
        /// <summary>
        /// get请求带的参数
        /// </summary>
        public NameValueCollection Getdata{ set; get; }

        public byte[] PostdataByte { set; get; }

        public WebProxy WebProxy{set;get;}

        
	    private ResultCookieType resultCookieType = ResultCookieType.String;  
	    /// <summary>  
	    /// Cookie返回类型,默认的是只返回字符串类型  
	    /// </summary>  
        public ResultCookieType ResultCookieType
        {
            get { return resultCookieType; }
            set { resultCookieType = value; }
        }

        public CookieCollection CookieCollection { set; get; }
	     
	    private string cookie = string.Empty;  
	    /// <summary>  
	    /// 请求时的Cookie  
	    /// </summary>  
        public string Cookie
        {
            get { return cookie; }
            set { cookie = value; }
        }

        public string CookieKey { set; get; }

        public string CookiePath { set; get; }

        public string CookieDomain { set; get; }

        public string Host { set; get; }
        /// <summary>
        /// http或者https
        /// </summary>
        public string HttpScheme { set; get; }

        public string Referer { set; get; }
      
        /// <summary>  
        /// 证书绝对路径  
        /// </summary>  
        public string CerPath { set; get; }
     
        private Boolean isToLower = false;
        /// <summary>  
        /// 是否设置为全文小写，默认为不转化  
        /// </summary>  
        public Boolean IsToLower
        {
            get { return isToLower; }
            set { isToLower = value; }
        }

        private Boolean allowautoredirect = false;
        /// <summary>  
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转  
        /// </summary>  
        public Boolean Allowautoredirect
        {
            get { return allowautoredirect; }
            set { allowautoredirect = value; }
        }

        private int connectionlimit = 1024;
        /// <summary>  
        /// 最大连接数  
        /// </summary>  
        public int Connectionlimit
        {
            get { return connectionlimit; }
            set { connectionlimit = value; }
        }

        private string proxyusername = string.Empty;
        /// <summary>  
        /// 代理Proxy 服务器用户名  
        /// </summary>  
        public string ProxyUserName
        {
            get { return proxyusername; }
            set { proxyusername = value; }
        }
        private string proxypwd = string.Empty;
        /// <summary>  
        /// 代理 服务器密码  
        /// </summary>  
        public string ProxyPwd
        {
            get { return proxypwd; }
            set { proxypwd = value; }
        }
        private string proxyip = string.Empty;
        /// <summary>  
        /// 代理 服务IP ,如果要使用IE代理就设置为ieproxy  
        /// </summary>  
        public string ProxyIp
        {
            get { return proxyip; }
            set { proxyip = value; }
        }
      
        public ResultType ResultType { set; get; }
       
        private WebHeaderCollection header = new WebHeaderCollection();
        /// <summary>  
        /// header对象  
        /// </summary>  
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }

        private Version protocolVersion= HttpVersion.Version11;
        public Version ProtocolVersion
        {
            get { return protocolVersion; }
            set { protocolVersion = value; }
        }
        private Boolean expect100continue = false;
        public Boolean Expect100Continue
        {
            get { return expect100continue; }
            set { expect100continue = value; }
        }
        /// <summary>  
        /// 设置509证书集合  
        /// </summary>  
        public X509CertificateCollection ClentCertificates { set; get; }

        private ICredentials iCredentials = CredentialCache.DefaultCredentials;
        /// <summary>  
        /// 获取或设置请求的身份验证信息。  
        /// </summary>  
        public ICredentials ICredentials
        {
            get { return iCredentials; }
            set { iCredentials = value; }
        }
        /// <summary>  
        /// 设置请求将跟随的重定向的最大数目  
        /// </summary>  
        public int MaximumAutomaticRedirections { set; get; }
       
        /// <summary>  
        /// 获取和设置IfModifiedSince，默认为当前日期和时间  
        /// </summary>  
        public DateTime? IfModifiedSince { set; get; }

        private bool isDownLoadFile = false;

        public bool IsDownLoadFile
        {
            set
            {
                isDownLoadFile = value;

            }
            get
            {
                return isDownLoadFile;
            }
        }
        public string DownLoadFileType { set; get; }
        public string DownLoadFileName { set; get; }
        public string DownLoadBaseUrl { set; get; }
        public string DownLoadFilePath { set; get; }

        /// <summary>
        /// html结果处理
        /// </summary>
        public Action<string> HandleResult { set; get; }

    }
    /// <summary>  
    /// Http返回参数类  
    /// </summary>  
    public class HttpResult
    {
        public string Cookie { set; get; }
        /// <summary>  
        /// Cookie对象集合  
        /// </summary>  
        public CookieCollection CookieCollection { set; get; }
      
        private string html = string.Empty;
        /// <summary>  
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空  
        /// </summary>  
        public string Html
        {
            get { return html; }
            set { html = value; }
        }
        private byte[] resultByte;
        /// <summary>  
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空  
        /// </summary>  
        public byte[] ResultByte
        {
            get { return resultByte; }
            set { resultByte = value; }
        }
        /// <summary>  
        /// header对象  
        /// </summary>  
        public WebHeaderCollection Header { set; get; }
      
        /// <summary>  
        /// 返回状态说明  
        /// </summary>  
        public string StatusDescription { set; get; }

        /// <summary>  
        /// 返回状态码,默认为OK  
        /// </summary>  
        public HttpStatusCode StatusCode { set; get; }

        public string RedirectUrl { set; get; }
       
    }
    /// <summary>  
    /// 返回类型  
    /// </summary>  
    public enum ResultType
    {
        /// <summary>  
        /// 表示只返回字符串 只有Html有数据  
        /// </summary>  
        String,
        /// <summary>  
        /// 表示返回字符串和字节流 ResultByte和Html都有数据返回  
        /// </summary>  
        Byte
    }
    /// <summary>  
    /// Post的数据格式默认为string  
    /// </summary>  
    public enum PostDataType
    {
        /// <summary>  
        /// 字符串类型，这时编码Encoding可不设置  
        /// </summary>  
        String,
        /// <summary>  
        /// Byte类型，需要设置PostdataByte参数的值编码Encoding可设置为空  
        /// </summary>  
        Byte,
        /// <summary>  
        /// 传文件，Postdata必须设置为文件的绝对路径，必须设置Encoding的值  
        /// </summary>  
        FilePath
    }

    
  /// <summary>  
  /// Cookie返回类型  
  /// </summary>  
  public enum ResultCookieType  
  {  
      /// <summary>  
      /// 只返回字符串类型的Cookie  
      /// </summary>  
      String,  
       /// <summary>  
       /// CookieCollection格式的Cookie集合同时也返回String类型的cookie  
       /// </summary>  
       CookieCollection  
   }  


}
