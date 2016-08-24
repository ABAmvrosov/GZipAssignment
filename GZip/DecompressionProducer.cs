using System;
using System.Collections.Concurrent;
using System.IO;

namespace GZipTest
{
    internal class DecompressionProducer : AProducer
    {
        public DecompressionProducer(string sourceFile, BlockingCollection<byte[]> targetCollection) : base(sourceFile, targetCollection) { }

        protected override void Routine()
        {
            try {
                ReadFile();
            }
            catch (FileNotFoundException e) {
                Console.WriteLine(e.Message);
                Environment.Exit(ERROR_CODE);
            }            
            catch (FileFormatException e) {
                Console.WriteLine(e.Message);
                Environment.Exit(ERROR_CODE);
            }
            catch (WrongMemberFormatException e) {
                Console.WriteLine(e.Message);
                Environment.Exit(ERROR_CODE);
            }
        }

        private void ReadFile()
        {
            using (FileStream sourceStream = new FileStream(_sourceFile, FileMode.Open)) {
                while (sourceStream.Position != sourceStream.Length) {
                    byte[] extendedHeader = new byte[20];
                    sourceStream.Read(extendedHeader, 0, extendedHeader.Length);
                    if (!GZipFileMember.IsGzipFormat(extendedHeader))
                        throw new FileFormatException("Not Gzip member file!");
                    int memberSize = GZipFileMember.GetMemberSize(extendedHeader);
                    if (memberSize == 0) {
                        _readData.Add(new byte[0]);
                    }
                    Array.Resize<byte>(ref extendedHeader, memberSize);
                    sourceStream.Read(extendedHeader, 20, memberSize - 20);
                    _readData.Add(extendedHeader);
                }
            }
            _readData.CompleteAdding();
        }
    }
}
