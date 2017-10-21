namespace Meyer.Socrates.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal unsafe class BitReader
    {
        public int BitOffset => bitShift;

        public int BitPosition
        {
            get => bitPosition;
            set
            {
                if (value != bitPosition)
                {
                    bitPosition += value;
                    bytePosition = bitPosition / 8;
                }
            }
        }

        public int BytePosition => bytePosition;

        public int BitLength => bitLength;

        public int ByteLength => byteLength;

        public bool EndOfStream => bitPosition >= bitLength;

        static BitReader()
        {
            masks = Enumerable.Range(0, 32).ToDictionary(i => i, i => (1 << i) - 1);
        }

        public BitReader(void* buffer, int bufferLength)
        {
            this.buffer = buffer;
            byteLength = bufferLength;
            bitLength = bufferLength * 8;
            GenerateValue(0);
        }

        public int ReadBit()
        {
            return ReadRaw(1);
        }

        public byte ReadByte(int bitCount = 8)
        {
            return unchecked((byte)ReadRaw(bitCount));
        }

        public sbyte ReadSByte(int bitCount = 8)
        {
            return unchecked((sbyte)ReadRaw(bitCount));
        }

        public short ReadInt16(int bitCount = 16)
        {
            return unchecked((short)ReadRaw(bitCount));
        }

        public ushort ReadUInt16(int bitCount = 16)
        {
            return unchecked((ushort)ReadRaw(bitCount));
        }

        public int ReadInt32(int bitCount = 32)
        {
            return ReadRaw(bitCount);
        }

        public uint ReadUInt32(int bitCount = 32)
        {
            return unchecked((uint)ReadRaw(bitCount));
        }

        public long ReadInt64(int bitCount = 64)
        {
            if (bitCount > 32)
                return ReadRaw(32) | (ReadRaw(bitCount - 32) << 32);
            else
                return ReadRaw(bitCount);
        }

        public ulong ReadUInt64(int bitCount = 64)
        {
            return unchecked((ulong)ReadInt64());
        }

        private int ReadRaw(int bitCount)
        {
            EnsureLength(bitCount);
            if (bitCount >= 32 - bitShift)
            {
                var half1 = bitCount - bitShift;
                var half2 = bitCount - half1;
                return ReadRawInternal(half1) | (ReadRawInternal(half2) << half1);
            }
            else
            {
                return ReadRawInternal(bitCount);
            }

            int ReadRawInternal(int n)
            {
                var result = value >> bitShift;
                bitPosition += n;
                bitShift += n;
                bool needsUpdate = false;
                while (bitShift >= 8)
                {
                    bitShift -= 8;
                    ++bytePosition;
                    if (!needsUpdate) needsUpdate = true;
                }
                if (needsUpdate) GenerateValue(bytePosition);
                return result & masks[n];
            }
        }

        private void EnsureLength(int bitCount)
        {
            if (bitCount > 32) throw new ArgumentOutOfRangeException("bitCount");
            if (bitLength - bitCount < bitPosition) throw new EndOfStreamException();
        }

        private unsafe void GenerateValue(int pos)
        {
            value = *(int*)((byte*)buffer + bytePosition);
        }

        private void* buffer;
        private int byteLength;
        private int bitLength;
        private int bitShift = 0;
        private int value;
        private static Dictionary<int, int> masks;
        private int bytePosition;
        private int bitPosition;
    }
}
