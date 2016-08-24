using System.Collections.Concurrent;
using System.Threading;

namespace GZipTest
{
    /// <summary>
    /// Abstraction of producer thread.
    /// </summary>
    internal abstract class AProducer
    {
        protected const int ERROR_CODE = 1;
        protected const int WAIT_TIME = 25;
        protected const int MEGABYTE = 1024 * 1024;
        protected string _sourceFile;
        protected Thread _readingThread;
        protected BlockingCollection<byte[]> _readData;

        /// <summary>
        /// Abstraction of producer thread.
        /// </summary>
        /// <param name="sourceFile">File to read from.</param>
        /// <param name="targetCollection">Collection to store readed file portions.</param>
        protected AProducer(string sourceFile, BlockingCollection<byte[]> targetCollection)
        {
            _sourceFile = sourceFile;
            _readData = targetCollection;
            _readingThread = new Thread(Routine);
        }

        protected abstract void Routine();

        /// <summary>
        /// Start producer thread.
        /// </summary>
        public void Start()
        {
            _readingThread.Start();
        }

        /// <summary>
        /// Join producer thread.
        /// </summary>
        public void Join()
        {
            _readingThread.Join();
        }
    }
}
