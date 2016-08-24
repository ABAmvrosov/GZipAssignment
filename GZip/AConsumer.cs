using System.Threading;

namespace GZipTest
{
    /// <summary>
    /// Abstraction of consumer thread.
    /// </summary>
    internal abstract class AConsumer
    {
        protected ConsumerSynchronizer _synchronizer;
        protected Thread _consumingThread;
        protected int _blockIndex;

        protected AConsumer(ConsumerSynchronizer synchronizer)
        {
            _synchronizer = synchronizer;
            _consumingThread = new Thread(Routine);
            _synchronizer.Register(this);
        }

        protected abstract void Routine();

        /// <summary>
        /// Start producer thread.
        /// </summary>
        public void Start()
        {
            _consumingThread.Start();
        }

        /// <summary>
        /// Join producer thread.
        /// </summary>
        public void Join()
        {
            _consumingThread.Join();
        }

        /// <summary>
        /// True if consumer thread still working.
        /// </summary>
        public bool IsAlive()
        {
            return _consumingThread.IsAlive;
        }
    }
}
