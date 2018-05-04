using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UrlCrawl.Core;

namespace UrlCrawl.model
{
    public class Setting
    {
        /// <summary>
        /// 是否开启线程
        /// </summary>
        public bool IsStartThread { set; get; }

        /// <summary>
        /// 线程数
        /// </summary>
        public int ThreadCount { set; get; }

        /// <summary>
        /// 线程休息时间
        /// </summary>
        public int SleepSecond { set; get; }

        public static Setting Current { set; get; }

        static Setting()
        {

            var d = Common.GetXMl("/Setting", Environment.CurrentDirectory + @"\setting.xml");

            int threadCount=3;
            int SleepSecond=2;

            int.TryParse(d["ThreadCount"],out threadCount);
            int.TryParse(d["SleepSecond"], out SleepSecond);

            Current = new Setting()
            {
                IsStartThread = d["IsStartThread"]=="1"?true:false,
                ThreadCount=threadCount,
                SleepSecond = SleepSecond
            };
        }
    }
}
