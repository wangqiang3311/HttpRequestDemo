using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UrlCrawl.Model;

namespace UrlCrawl.Core
{
    public class UriQueue
    {
        private Queue<HttpItem> queue;
        /// <summary>
        /// 是否多线程
        /// </summary>
        public bool IsMultiThread { set; get; }

        public UriQueue(bool isMultiThread)
        {
            this.IsMultiThread = isMultiThread;
            this.queue = new Queue<HttpItem>();
        }

        public HttpItem DeQueue()
        {
            HttpItem d = null;

            if (IsMultiThread)
            {
                lock (this)
                {
                    if (queue.Count > 0)
                    {
                        d = queue.Dequeue();
                    }
                }
            }
            else
            {
                d = queue.Dequeue();
            }

            return d;

        }

        public void EnterQueue(HttpItem item)
        {
            this.queue.Enqueue(item);
        }

        public void EnterQueuebatch(IEnumerable<HttpItem> items)
        {
            foreach (var item in items)
            {
                EnterQueue(item);
            }
        }

        public void Clear()
        {
            this.queue.Clear();
        }

        public int Count()
        {
            return this.queue.Count;
        }
    }

}
