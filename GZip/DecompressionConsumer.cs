using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    internal class DecompressionConsumer : AConsumer
    {

        public DecompressionConsumer(ConsumerSynchronizer synchronizer) : base(synchronizer) { }

        protected override void Routine()
        {
            while (!_synchronizer.HasDataToRead()) {
                byte[] buffer = _synchronizer.GetData(out _blockIndex);
                if (buffer == null)
                    continue;
                using (MemoryStream targetStream = new MemoryStream()) {
                    using (MemoryStream sourceStream = new MemoryStream(buffer))
                    using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress)) {
                        decompressionStream.CopyTo(targetStream);
                    }
                    byte[] decompressedData = targetStream.ToArray();
                    _synchronizer.WriteData(_blockIndex, decompressedData);
                }
            }
        }
    }
}
