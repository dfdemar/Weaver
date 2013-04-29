using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Weaver
{
    public class ThreadManager
    {
        private int threadID { get; set; }
        public Dictionary<int, Thread> ThreadList { get; set; }

        public ThreadManager()
        {
            this.ThreadList = new Dictionary<int, Thread>();
        }

        public void LaunchThread(Action<Url> FetchNewPage, Url url)
        {
            Thread thread = new Thread(() => FetchNewPage(url));
            int id = threadID++;
            thread.Name = id.ToString();
            ThreadList.Add(id, thread);
            thread.Start();
        }

        public void KillThread()
        {
            ThreadList.Remove(Int32.Parse(Thread.CurrentThread.Name));
        }
    }
}
