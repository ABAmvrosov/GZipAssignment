using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    /// <summary>
    /// Represent consumer thread, which compress data using Gzip algorithm.
    /// </summary>
    internal class CompressionConsumer : AConsumer
    {
        public CompressionConsumer(ConsumerSynchronizer synchronizer) : base(synchronizer) { }

        protected override void Routine()
        {
            while (!_synchronizer.HasDataToRead()) {
                byte[] buffer = _synchronizer.GetData(out _blockIndex);
                if (buffer == null)
                    continue;
                buffer = Compress(buffer);
                if (buffer.Length == 0)
                    buffer = GZipFileMember.GetDefaultHeader();
                GZipFileMember.WriteMemberSizeToExtraField(ref buffer);
                _synchronizer.WriteData(_blockIndex, buffer);
            }
        }

        private byte[] Compress(byte[] data)
        {
            using (MemoryStream targetStream = new MemoryStream()) {
                using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress)) {
                    compressionStream.Write(data, 0, data.Length);
                }
                byte[] result = targetStream.ToArray();
                return result;
            }
        }
    }
}
