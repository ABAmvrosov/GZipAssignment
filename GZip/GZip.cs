﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    public sealed class GZip
    {
        private const int ERROR_CODE = 1;
        private BlockingCollection<byte[]> _readData = new BlockingCollection<byte[]>();
        private ConcurrentDictionary<int, byte[]> _processedData = new ConcurrentDictionary<int, byte[]>();
        private ConsumerSynchronizer synchronizer;

        public GZip()
        {
            synchronizer = new ConsumerSynchronizer(_readData, _processedData);
        }

        public void Compress(string fileToCompress, string archiveName)
        {
            AProducer producer = new CompressionProducer(fileToCompress, _readData);
            AConsumer[] consumers = new CompressionConsumer[Environment.ProcessorCount];
            for (int i = 0; i < consumers.Length; i++) {
                consumers[i] = new CompressionConsumer(synchronizer);
                consumers[i].Start();
            }
            DataWriter writer = new DataWriter(archiveName, synchronizer);
            producer.Start();
            writer.Start();
            writer.Join();
        }

        public void Decompress(string archiveToDecompress, string resultFileName)
        {
            MultiThreadDecompress(archiveToDecompress, resultFileName);
        }

        public void MultiThreadDecompress(string archiveToDecompress, string resultFileName)
        {
            AProducer producer = new DecompressionProducer(archiveToDecompress, _readData);
            AConsumer[] consumers = new DecompressionConsumer[Environment.ProcessorCount];
            for (int i = 0; i < consumers.Length; i++) {
                consumers[i] = new DecompressionConsumer(synchronizer);
                consumers[i].Start();
            }
            DataWriter writer = new DataWriter(resultFileName, synchronizer);
            producer.Start();
            writer.Start();
            writer.Join();
        }
    }
}
