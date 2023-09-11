using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FNMES.Utility.Threads
{
    public class MyTask
    {
        private bool close;
        Thread thread;
        public void Run(Action action)
        {
            thread = new Thread(() => action());
            thread.Start();
        }

        public void RunWhileTrue(Action actionLoop)
        {
            RunWhileTrue(actionLoop, 200);
        }

        public void RunWhileTrue(Action actionLoop, int sleepTime)
        {
            thread = new Thread(() =>
            {
                while (!close)
                {
                    try
                    {
                        actionLoop();
                    }
                    catch
                    {

                    }
                    Thread.Sleep(sleepTime);
                }
            });
            thread.Start();
        }

        public void Close()
        {
            close = true;
            try { thread.Abort(); } catch { }
        }

    }
}
