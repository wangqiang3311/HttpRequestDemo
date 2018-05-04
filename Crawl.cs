using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UrlCrawl.Core;
using UrlCrawl.model;
using UrlCrawl.Model;

namespace UrlCrawl.main
{
    public class Crawl
    {
        Setting config;
        UriQueue queue;

        Task[] tasks;
       

        public Crawl()
        {
            config = Setting.Current;
            queue = new UriQueue(config.IsStartThread);
        }

        public void Start()
        {
            if (config.IsStartThread)
            {
                tasks = new Task[config.ThreadCount];

                for (int i = 0; i < config.ThreadCount; i++)
                {
                   var t= Task.Factory.StartNew(DoWork);
                   tasks[i] = t;
                   Console.WriteLine("线程:" + i + "启动");
                }
            }
            else
            {
                DoWork();
            }
        }

        public void DoWork()
        {
            Console.WriteLine("当前线程:" + Thread.CurrentThread.ManagedThreadId + "工作");

            HttpHelper http = new HttpHelper();

            while (true)
            {
                if (queue.Count() > 0)
                {
                    HttpItem item = queue.DeQueue();

                    if (item == null || item.URL.Trim() == "")
                    {
                        continue;
                    }
                    if (item.IsDownLoadFile)
                    {
                        item.Header = new WebHeaderCollection();
                        item.Header.Add(HttpRequestHeader.AcceptLanguage, "zh-CN,en-US;q=0.5");
                        item.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; MS-RTC LM 8; CIBA; .NET4.0C; .NET4.0E)";
                    }

                    Uri uri = new Uri(item.URL);
                    item.HttpScheme = uri.Scheme;

                    item.URL = Common.UrlCheck(item.URL,item.HttpScheme);

                    HttpResult result = http.GetResult(item);

                    while (!string.IsNullOrEmpty(result.RedirectUrl))
                    {
                        item.URL = Common.UrlCheck(result.RedirectUrl,item.HttpScheme,item.Host,result.RedirectUrl);
                        result = http.GetResult(item);
                    }
                   
                    Thread.Sleep(config.SleepSecond * 1000);


                    if (item.HandleResult != null)
                    {
                        if (item.IsDownLoadFile)
                        {
                            item.HandleResult(item.DownLoadFilePath);
                        }
                        else
                        {
                            item.HandleResult(result.Html);
                        }
                    }
                }
            }
        }

        public void EnterQueue(HttpItem item)
        {
            queue.EnterQueue(item);
        }
       
    }
}
