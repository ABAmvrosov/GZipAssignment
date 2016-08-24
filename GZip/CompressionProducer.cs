using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace GZipTest
{
    /// <summary>
    /// Represent producer thread, which read data by portions from file to given collection.
    /// </summary>
    internal class CompressionProducer : AProducer
    {
        private int _bufferSize = MEGABYTE;       

        /// <summary>
        /// Represent producer thread, which read data by portions from file to given collection.
        /// </summary>
        /// <param name="sourceFile">File to read from.</param>
        /// <param name="targetCollection">Collection to store sliced portions of file.</param>
        public CompressionProducer(string sourceFile, BlockingCollection<byte[]> targetCollection) : base(sourceFile, targetCollection) { }

        protected override void Routine()
        {
            try {
                ReadFile();
            }
            catch (FileNotFoundException e) {
                Environment.Exit(ERROR_CODE);
            }
        }

        private void ReadFile()
        {
            using (FileStream sourceStream = new FileStream(_sourceFile, FileMode.Open)) {
                if (sourceStream.Length == 0) {
                    _readData.Add(new byte[0]);
                    _readData.CompleteAdding();
                    return;
                }
                PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available Bytes");
                byte[] buffer = new byte[_bufferSize];
                while (!IsEndOfStream(sourceStream)) {
                    if (!IsEnoughMemory(ramCounter)) {
                        Thread.Sleep(WAIT_TIME);
                        continue;
                    }
                    sourceStream.Read(buffer, 0, buffer.Length);
                    _readData.Add(buffer);
                }
                _readData.CompleteAdding();
            }
        }

        private bool IsEndOfStream(Stream stream) {
            return stream.Position >= stream.Length;
        }

        private bool IsEnoughMemory(PerformanceCounter counter)
        {
            return _bufferSize < counter.NextValue();
        }
    }
}
