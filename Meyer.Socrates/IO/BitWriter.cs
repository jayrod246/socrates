namespace Meyer.Socrates.IO
{
    using Meyer.Socrates.Data;
    using System;
    using System.Runtime.CompilerServices;

    internal class BitWriter
    {
        private const byte BIT_OR = 0b1000_0000;
        private const byte BIT_AND = 0b0111_1111;
        private const int BitsPerByte = 8;
        private const int BitsPerShort = 16;
        private const int BitsPerInt = 32;
        private const int BitsPerLong = 64;
        private byte[] buffer;
        private uint capacityInBits;
        private int byteIndex = 0;
        private byte partialByte = 0;
        private int bitsInPartialByte = 0;
        private int byteLength;

        public int BitPosition
        {
            get => bitsInPartialByte % 8;
        }

        public int Position
        {
            get => byteIndex;
        }

        public BitWriter() : this(0)
        {
        }

        public BitWriter(int capacity)
        {
            this.buffer = new byte[capacity];
            capacityInBits = (uint)(capacity * 8);
        }

        public BitWriter(byte[] buffer)
        {
            this.buffer = buffer ?? throw new ArgumentNullException("buffer");
            capacityInBits = (uint)(buffer.Length * 8);
            byteLength = buffer.Length;
        }

        public void WriteInt32(int value) => WriteUInt32(unchecked((uint)value), BitsPerInt);

        public void WriteInt32(int value, int bitCount) => WriteUInt32(unchecked((uint)value), bitCount);

        public void WriteUInt32(uint value) => WriteUInt32(value, BitsPerInt);

        public void WriteUInt32(uint value, int bitCount)
        {
            if (bitCount > BitsPerInt || bitCount <= 0)
                throw new ArgumentOutOfRangeException("bitCount");

            WriteShared(value, bitCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Obsolete]
        private void WriteDynamic(dynamic value, int bitCount)
        {
            EnsureLength(bitCount);

            while (bitCount > 0)
            {
                int n = BitsPerByte < bitCount ? BitsPerByte : bitCount;
                WriteByte((byte)(value & 0xFF), n);
                value >>= n;
                bitCount -= n;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteShared(long value, int bitCount)
        {
            EnsureLength(bitCount);

            while (bitCount > 0)
            {
                int n = BitsPerByte < bitCount ? BitsPerByte : bitCount;
                WriteByte((byte)(value & 0xFF), n);
                value >>= n;
                bitCount -= n;
            }
        }

        public void WriteBit(Bit value)
        {
            WriteByte(value, 1);
        }

        public void WriteByte(byte value) => WriteByte(value, BitsPerByte);

        public void WriteByte(byte value, int bitCount)
        {
            if (bitCount > BitsPerInt || bitCount <= 0)
                throw new ArgumentOutOfRangeException("bitCount");

            EnsureLength(bitCount);
            capacityInBits -= (uint)bitCount;

            for (int i = 0;i < bitCount;i++)
            {
                if (bitsInPartialByte == 8)
                {
                    FlushPartialByte();
                    bitsInPartialByte = 0;
                    if (++byteIndex > byteLength)
                        byteLength = byteIndex;
                }

                partialByte >>= 1;
                bitsInPartialByte++;

                if ((value & 1) == 1)
                    partialByte |= BIT_OR;
                else
                    partialByte &= BIT_AND;

                value >>= 1;
            }
        }

        private void EnsureLength(int bitCount)
        {
            if (bitCount > capacityInBits)
            {
                var newBuffer = new byte[buffer.Length * 2];
                Array.Copy(buffer, newBuffer, buffer.Length);
                capacityInBits += (uint)(buffer.Length * 8);
                buffer = newBuffer;
            }
        }

        private void FlushPartialByte()
        {
            var mask = (1 << bitsInPartialByte) - 1;
            var value = partialByte >> (8 - bitsInPartialByte);
            buffer[byteIndex] = (byte)((value & mask) | (buffer[byteIndex] & ~mask));
        }

        public byte[] ToArray()
        {
            if (bitsInPartialByte > 0)
                FlushPartialByte();
            var copy = new byte[byteLength];
            Array.Copy(buffer, copy, byteLength);
            return copy;
        }
    }
}
