using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace GZipTest
{
    /// <summary>
    /// Write's compressed data to file.
    /// </summary>
    internal class DataWriter
    {
        private const int ERROR_CODE = 1;
        private ConsumerSynchronizer _synchronizer;
        private Thread _writingThread;  
        private string _targetFile;

        public DataWriter(string targetFile, ConsumerSynchronizer synchronizer)
        {
            _targetFile = targetFile;
            _synchronizer = synchronizer;
            _writingThread = new Thread(Routine);
        }

        private void Routine()
        {
            try {
                WriteFile();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Environment.Exit(ERROR_CODE);
            }
        }

        private void WriteFile()
        {
            using (FileStream targetSteam = File.Create(_targetFile)) {
                int index = 0;
                var data = _synchronizer.ProcessedData;
                while (data.Count != 0 || _synchronizer.IsDone()) {
                    byte[] buffer;
                    bool result = data.TryRemove(index, out buffer);
                    if (!result)
                        continue;
                    targetSteam.Write(buffer, 0, buffer.Length);
                    index++;
                }
            }
        }

        /// <summary>
        /// Start producer thread.
        /// </summary>
        public void Start()
        {
            _writingThread.Start();
        }

        /// <summary>
        /// Join producer thread.
        /// </summary>
        public void Join()
        {
            _writingThread.Join();
        }
    }
}
