using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meyer.Socrates.Data
{
    internal static class BufferEx
    {
        public static bool GetBit(Array array, int bitOffset)
        {
            return (Buffer.GetByte(array, bitOffset / 8) & (1 << (bitOffset % 8))) != 0;
        }

        public static void SetBit(Array array, int bitOffset, bool value)
        {
            int bIndex = bitOffset / 8;
            int b = Buffer.GetByte(array, bIndex);

            if (value)
            {
                b |= (1 << (bitOffset % 8));
            }
            else
            {
                b &= ~(1 << (bitOffset % 8));
            }

            Buffer.SetByte(array, bIndex, (byte)b);
        }

        public static void ClearBits(Array arr, int bitOffset, int bitlength)
        {
            while (--bitlength >= 0)
                SetBit(arr, bitOffset + bitlength, false);
        }

        public static void CopyBits(Array src, int srcBitOffset, Array dst, int dstBitOffset, int bitCount)
        {
            Contract.Assert(src != null);
            Contract.Assert(dst != null);
            Contract.Assert(srcBitOffset + bitCount <= BitLength(src));
            Contract.Assert(dstBitOffset + bitCount <= BitLength(dst));

            while (--bitCount >= 0)
                SetBit(dst, dstBitOffset + bitCount, GetBit(src, srcBitOffset + bitCount));
        }

        public static int BitLength(Array src)
        {
            return Buffer.ByteLength(src) * 8;
        }
    }
}
