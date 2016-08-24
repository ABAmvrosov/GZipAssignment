using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    /// <summary>
    /// More info about member format can be found here http://www.zlib.org/rfc-gzip.html.
    /// </summary>
    public class GZipFileMember
    {
        private const short EXTRA_FIELD_SIZE = 10;
        private const int MEMBER_HEADER_SIZE = 10;
        private const int XLEN_BYTES_SIZE = 2;
        private const int XLEN_INDEX = MEMBER_HEADER_SIZE;
        private const int SI1_INDEX = MEMBER_HEADER_SIZE + 2;
        private const int SI2_INDEX = MEMBER_HEADER_SIZE + 3;
        private const int LEN_INDEX = MEMBER_HEADER_SIZE + 4;
        private const int INFO_INDEX = MEMBER_HEADER_SIZE + 6;
        private const byte ID1 = 0x1f;
        private const byte ID2 = 0x8b;
        private const byte CM = 8;
        private const byte XFL = 4;
        public static byte SI1 = 0x01; // any other non-registred combination of SI1 & SI2 to determine this multithread compression-decompression algorithm.
        public static byte SI2 = 0x01;

        private enum HeaderIndexes
        {
            IDENTIFICATION1 = 0,
            IDENTIFICATION2 = 1,
            COMPRESSIONG_METHOD = 2,
            FLAGS = 3,
            MTIME = 4,
            XFL = 8,
            OS = 9
        }

        public enum Flags
        {
            FTEXT = 0,
            FHCRC = 2,
            FEXTRA = 4,
            FNAME = 8,
            FCOMMENT = 16
        }

        public static bool IsGzipFormat(byte[] memberHeader)
        {
            byte ID1 = memberHeader[(int)HeaderIndexes.IDENTIFICATION1];
            byte ID2 = memberHeader[(int)HeaderIndexes.IDENTIFICATION2];
            return (ID1 == 0x1f && ID2 == 0x8b) ? true : false;
        }
        
        public static int GetMemberSize(byte[] extendedHeader)
        {
            if (!HasFlag(extendedHeader, Flags.FEXTRA) | !IsSubfieldIDsCorrect(extendedHeader))
                throw new WrongMemberFormatException();
            int result = 0;
            result = BitConverter.ToInt32(extendedHeader, INFO_INDEX);
            return result;
        }

        private static bool HasFlag(byte[] memberHeader, Flags flag)
        {
            byte flags = memberHeader[(int)HeaderIndexes.FLAGS];
            return (flags & (byte)flag) != 0;
        }

        private static bool IsSubfieldIDsCorrect(byte[] extendedHeader)
        {
            byte SI1 = extendedHeader[SI1_INDEX];
            byte SI2 = extendedHeader[SI2_INDEX];
            return GZipFileMember.SI1 == SI1 && GZipFileMember.SI2 == SI2;
        }

        public static byte[] GetDefaultHeader()
        {
            byte[] result = new byte[MEMBER_HEADER_SIZE];
            result[(int)HeaderIndexes.IDENTIFICATION1] = ID1;
            result[(int)HeaderIndexes.IDENTIFICATION2] = ID2;
            result[(int)HeaderIndexes.COMPRESSIONG_METHOD] = CM;
            result[(int)HeaderIndexes.XFL] = XFL;
            return result;
        }

        public static void WriteMemberSizeToExtraField(ref byte[] dataBlock)
        {
            dataBlock[(int)HeaderIndexes.FLAGS] |= (byte)Flags.FEXTRA;
            AddExtraField(ref dataBlock);
            WriteExtraFieldInformation(ref dataBlock);
        }

        private static void AddExtraField(ref byte[] dataBlock)
        {
            Array.Resize<byte>(ref dataBlock, dataBlock.Length + EXTRA_FIELD_SIZE);
            int compressedDataSize = dataBlock.Length - MEMBER_HEADER_SIZE - EXTRA_FIELD_SIZE;
            Array.Copy(dataBlock, MEMBER_HEADER_SIZE, dataBlock, MEMBER_HEADER_SIZE + EXTRA_FIELD_SIZE, compressedDataSize);
        }

        private static void WriteExtraFieldInformation(ref byte[] dataBlock)
        {
            byte[] xlen = BitConverter.GetBytes(EXTRA_FIELD_SIZE - XLEN_BYTES_SIZE);
            xlen.CopyTo(dataBlock, XLEN_INDEX);
            dataBlock[SI1_INDEX] = SI1;
            dataBlock[SI2_INDEX] = SI2;
            byte[] memberSizeInfo = BitConverter.GetBytes(dataBlock.Length);
            byte[] len = BitConverter.GetBytes((short)memberSizeInfo.Length);
            len.CopyTo(dataBlock, LEN_INDEX);
            memberSizeInfo.CopyTo(dataBlock, INFO_INDEX);
        }
    }
}
