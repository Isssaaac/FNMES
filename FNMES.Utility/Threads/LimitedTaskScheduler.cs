using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FNMES.Utility.Threads
{
    public class LimitedTaskScheduler : TaskScheduler, IDisposable
    {
        #region 外部方法
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        #endregion

        #region 变量属性事件
        private BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        List<Thread> _threadList = new List<Thread>();
        private int _threadCount = 0;
        private int _timeOut = Timeout.Infinite;
        private Task _tempTask;
        #endregion

        #region 构造函数
        public LimitedTaskScheduler(int threadCount = 10)
        {
            CreateThreads(threadCount);
        }
        #endregion

        #region override GetScheduledTasks
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasks;
        }
        #endregion

        #region override TryExecuteTaskInline
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }
        #endregion

        #region override QueueTask
        protected override void QueueTask(Task task)
        {
            _tasks.Add(task);
        }
        #endregion

        #region 资源释放
        /// <summary>
        /// 资源释放
        /// 如果尚有任务在执行，则会在调用此方法的线程上引发System.Threading.ThreadAbortException，请使用Task.WaitAll等待任务执行完毕后，再调用该方法
        /// </summary>
        public void Dispose()
        {
            _timeOut = 100;

            foreach (Thread item in _threadList)
            {
                item.Abort();
            }
            _threadList.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion

        #region 创建线程池
        /// <summary>
        /// 创建线程池
        /// </summary>
        private void CreateThreads(int? threadCount = null)
        {
            if (threadCount != null) _threadCount = threadCount.Value;
            _timeOut = Timeout.Infinite;

            for (int i = 0; i < _threadCount; i++)
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    Task task;
                    while (_tasks.TryTake(out task, _timeOut))
                    {
                        TryExecuteTask(task);
                    }
                }));
                thread.IsBackground = true;
                thread.Start();
                _threadList.Add(thread);
            }
        }
        #endregion

        #region 全部取消
        /// <summary>
        /// 全部取消
        /// 当前正在执行的任务无法取消，取消的只是后续任务，相当于AbortAll
        /// </summary>
        public void CancelAll()
        {
            while (_tasks.TryTake(out _tempTask)) { }
        }
        #endregion

    }
}
