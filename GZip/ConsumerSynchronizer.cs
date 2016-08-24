using System.Collections.Concurrent;

namespace GZipTest
{
    /// <summary>
    /// Synchronize work of consumer threads.
    /// </summary>
    internal class ConsumerSynchronizer
    {
        public ConcurrentDictionary<int, byte[]> ProcessedData { get; private set; }
        public BlockingCollection<byte[]> ReadData { get; private set; }
        private ConcurrentBag<AConsumer> consumers = new ConcurrentBag<AConsumer>();
        private const int WAIT_TIME = 25;
        private object thisLock = new object();
        private int _readIndex = 0;

        /// <summary>
        /// Synchronize work of consumer threads.
        /// </summary>
        /// <param name="readData">Collection from which consumer threads taking data.</param>
        /// <param name="processedData">Dictionary where consumers write processed data in format index:data.</param>
        public ConsumerSynchronizer(BlockingCollection<byte[]> readData, ConcurrentDictionary<int, byte[]> processedData)
        {
            ReadData = readData;
            this.ProcessedData = processedData;
        }

        /// <summary>
        /// Get source data to compress. 
        /// </summary>
        /// <param name="blockIndex">Current readData index.</param>
        /// <returns>Data to compress.</returns>
        public byte[] GetData(out int blockIndex)
        {
            lock (thisLock) {
                byte[] result;
                ReadData.TryTake(out result, WAIT_TIME);
                blockIndex = (result == null) ? -1 : _readIndex++;
                return result;
            }
        }

        /// <summary>
        /// Write's compressed data.
        /// </summary>
        /// <param name="index">ReadData index.</param>
        /// <param name="data">Data to write.</param>
        public void WriteData(int index, byte[] data)
        {
            ProcessedData.TryAdd(index, data);
        }

        /// <summary>
        /// True if no more data to compress.
        /// </summary>
        /// <returns></returns>
        public bool HasDataToRead()
        {
            return ReadData.IsCompleted;
        }

        /// <summary>
        /// Register consumer thread.
        /// </summary>
        /// <param name="consumer">Thread to register.</param>
        public void Register(AConsumer consumer)
        {
            consumers.Add(consumer);
        }

        /// <summary>
        /// True if any registered consumer thread still alive.
        /// </summary>
        public bool IsDone()
        {
            foreach (AConsumer consumer in consumers) {
                if (consumer.IsAlive()) {
                    return true;
                }
            }
            return false;
        }
    }
}
