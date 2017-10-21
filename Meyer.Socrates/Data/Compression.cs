namespace Meyer.Socrates.Data
{
    using Meyer.Socrates.IO;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Static class which supports compressing and decompressing section data.
    /// </summary>
    public static class Compression
    {
        private const int DEFAULT_MaxLookBack = 2048; // 5697;
        public static int MaxLookBack { get; set; } = DEFAULT_MaxLookBack;
        private const uint KCDC = (uint)CompressionType.KCDC;
        private const uint KCD2 = (uint)CompressionType.KCD2;

        public static byte[] Decompress(byte[] buffer)
        {
            return Decompress(buffer, out _);
        }

        public static byte[] Decompress(byte[] buffer, out CompressionType compressionType)
        {
            const int minLength = 9 + 6;
            compressionType = CompressionType.Uncompressed;
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (buffer.Length < minLength) return buffer;
            for (int i = 1;i < 7;i++)
            {
                if (buffer[buffer.Length - i] != 0xFF)
                    return buffer;
            }
            var output = new byte[buffer[4] << 24 | buffer[5] << 16 | buffer[6] << 8 | buffer[7]];
            unsafe
            {
                fixed (byte* b = buffer)
                {
                    DecompressSectionWrapper(b + 9, buffer.Length - minLength, output, compressionType = GetCompressionType(b));
                }
            }
            return output;
        }

        // TODO: Cleanup Compression.Compress method.
        public static byte[] Compress(byte[] buffer, CompressionType compressionType)
        {
            if (compressionType == CompressionType.Uncompressed) return buffer;
            var lookbacks = new int[buffer.Length][];
            var loop = Parallel.For(1, buffer.Length, i =>
            {
                var list = new List<int>();
                var current = buffer[i];
                int index = i - MaxLookBack;
                if (index < 0) index = 0;
                do
                {
                    if (buffer[index] == current) list.Add(i - index);
                    if (i - index - 1 < 0) break;
                    index = Array.FindIndex(buffer, index + 1, i - index - 1, x => x == current);
                }
                while (index > 0);
                lookbacks[i] = list.ToArray();
            });

            var bitstream = new BitWriter(buffer.Length);

            Action<IList<byte>> dump;
            Action<int, int> copy;

            switch (compressionType)
            {
                case CompressionType.KCDC:
                    dump = l =>
                    {
                        if (l.Count > 0)
                        {
                            foreach (var lit in l)
                            {
                                bitstream.WriteBit(Bit.Zero);
                                bitstream.WriteByte(lit);
                            }
                            l.Clear();
                        }
                    };

                    copy = (lookBack, copyLength) =>
                    {
                        if (lookBack >= 4673)
                        {
                            copyLength -= 3;
                            bitstream.WriteByte(0b1111, 4);
                            bitstream.WriteInt32(lookBack - 4673, 20);
                        }
                        else if (lookBack >= 577)
                        {
                            copyLength -= 2;
                            bitstream.WriteByte(0b0111, 4);
                            bitstream.WriteInt32(lookBack - 577, 12);
                        }
                        else if (lookBack >= 65)
                        {
                            copyLength -= 2;
                            bitstream.WriteByte(0b0011, 3);
                            bitstream.WriteInt32(lookBack - 65, 9);
                        }
                        else if (lookBack >= 1)
                        {
                            copyLength -= 2;
                            bitstream.WriteByte(0b0001, 2);
                            bitstream.WriteInt32(lookBack - 1, 6);
                        }

                        int n = 1;
                        while (copyLength >= (1 << n) - 1)
                        {
                            bitstream.WriteBit(Bit.One);
                            n++;
                        }
                        bitstream.WriteBit(Bit.Zero);
                        if (n > 1) bitstream.WriteInt32(copyLength - ((1 << n) - 1), n - 1);
                    };
                    break;
                case CompressionType.KCD2:
                    dump = l =>
                    {
                        if (l.Count > 0)
                        {
                            var copyLength = l.Count - 1;
                            int n = 1;
                            while (copyLength >= (1 << n) - 1)
                            {
                                bitstream.WriteBit(Bit.One);
                                n++;
                            }
                            bitstream.WriteBit(Bit.Zero);
                            if (n > 1) bitstream.WriteInt32(copyLength - ((1 << n) - 1), n - 1);
                            bitstream.WriteBit(Bit.Zero);
                            bool hasPartial = bitstream.BitPosition != 0;
                            if (!hasPartial)
                            {
                                for (int i = 0;i < l.Count;i++)
                                    bitstream.WriteByte(l[i]);
                            }
                            else
                            {
                                var partialByte = l[l.Count - 1];
                                var half1 = bitstream.BitPosition;
                                var half2 = 8 - half1;
                                bitstream.WriteByte(partialByte, half2);
                                for (int i = 0;i < l.Count - 1;i++)
                                    bitstream.WriteByte(l[i]);
                                bitstream.WriteByte((byte)(partialByte >> half2), half1);
                            }
                            l.Clear();
                        }
                    };

                    copy = (lookBack, copyLength) =>
                    {
                        int lookBackValue;
                        int lookBackBits;

                        if (lookBack >= 4673)
                        {
                            copyLength -= 3;
                            lookBackValue = ((lookBack - 4673) << 4) | 0b1111;
                            lookBackBits = 24;
                        }
                        else if (lookBack >= 577)
                        {
                            copyLength -= 2;
                            lookBackValue = ((lookBack - 577) << 4) | 0b0111;
                            lookBackBits = 16;
                        }
                        else if (lookBack >= 65)
                        {
                            copyLength -= 2;
                            lookBackValue = ((lookBack - 65) << 3) | 0b0011;
                            lookBackBits = 12;
                        }
                        else if (lookBack >= 1)
                        {
                            copyLength -= 2;
                            lookBackValue = ((lookBack - 1) << 2) | 0b0001;
                            lookBackBits = 8;
                        }
                        else throw new InvalidOperationException();

                        int n = 1;
                        while (copyLength >= (1 << n) - 1)
                        {
                            bitstream.WriteBit(Bit.One);
                            n++;
                        }
                        bitstream.WriteBit(Bit.Zero);
                        if (n > 1) bitstream.WriteInt32(copyLength - ((1 << n) - 1), n - 1);
                        bitstream.WriteInt32(lookBackValue, lookBackBits);
                    };
                    break;
                default:
                    throw new ArgumentException("compressioNType");
            }

            bitstream.WriteUInt32((uint)compressionType);
            var len = buffer.Length;
            bitstream.WriteInt32((len >> 24) & 0xFF, 8);
            bitstream.WriteInt32((len >> 16) & 0xFF, 8);
            bitstream.WriteInt32((len >> 8) & 0xFF, 8);
            bitstream.WriteInt32(len & 0xFF, 8);
            bitstream.WriteByte(0);
            var literals = new List<byte>(new[] { buffer[0] });

            for (int i = 1;i < buffer.Length;)
            {
                int lb = 0;
                int copyLength = 0;

                if (lookbacks[i].Length > 0)
                {
                    for (int j = lookbacks[i].Length - 1;j >= 0;j--)
                    {
                        var currentLb = lookbacks[i][j];
                        var index = 1;
                        while (i + index < buffer.Length && buffer[index + i - currentLb] == buffer[i + index])
                        {
                            if (++index > copyLength)
                            {
                                copyLength = index;
                                lb = currentLb;
                            }
                        }
                    }
                }

                if (copyLength < 2 || (copyLength < 3 && lb >= 4673))
                {
                    literals.Add(buffer[i++]);
                }
                else
                {
                    dump.Invoke(literals);
                    copy.Invoke(lb, copyLength);
                    i += copyLength;
                }
            }
            dump.Invoke(literals);
            bitstream.WriteUInt32(0xFFFFFFFF);
            bitstream.WriteUInt32(0xFFFFFFFF, 32 - bitstream.BitPosition);

            return bitstream.ToArray();
        }

        public static CompressionType GetCompressionType(byte[] buffer)
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                unsafe
                {
                    return GetCompressionType(handle.AddrOfPinnedObject().ToPointer());
                }
            }
            finally
            {
                handle.Free();
            }
        }

        public unsafe static CompressionType GetCompressionType(void* buffer)
        {
            const uint KCDC = (uint)CompressionType.KCDC;
            const uint KCD2 = (uint)CompressionType.KCD2;

            switch (*(uint*)buffer)
            {
                case KCDC:
                    return CompressionType.KCDC;
                case KCD2:
                    return CompressionType.KCD2;
                default:
                    return CompressionType.Uncompressed;
            }
        }

        private unsafe static void DecompressSectionWrapper(void* section, int sectionLength, byte[] output, CompressionType compressionType)
        {
            switch (compressionType)
            {
                case CompressionType.KCDC:
                    DecompressSectionKCDC(new BitReader(section, sectionLength), output);
                    break;
                case CompressionType.KCD2:
                    DecompressSectionKCD2(new BitReader(section, sectionLength), output);
                    break;
                default:
                    throw new NotSupportedException($"Specified CompressionType is not supported: {compressionType}", new ArgumentException("", "compressionType"));
            }
        }

        private static void DecompressSectionKCD2(BitReader bitReader, byte[] output)
        {
            int outIndex = 0;
            while (outIndex < output.Length)
            {
                int copyLength = 0;
                int n = 0;
                while (true)
                {
                    if (bitReader.EndOfStream)
                        return;
                    if (bitReader.ReadBit() == 0)
                        break;
                    ++n;
                }
                copyLength = ((1 << n) - 1) + bitReader.ReadInt32(n);
                n = 0;

                while (n < 4 && bitReader.ReadBit() == 1) n++;
                int lookBack = 0;
                switch (n)
                {
                    case 0:
                        break;
                    case 1:
                        copyLength += 2;
                        lookBack = bitReader.ReadInt32(6) + 1;
                        break;
                    case 2:
                        copyLength += 2;
                        lookBack = bitReader.ReadInt32(9) + 65;
                        break;
                    case 3:
                        copyLength += 2;
                        lookBack = bitReader.ReadInt32(12) + 577;
                        break;
                    case 4:
                        copyLength += 3;
                        lookBack = bitReader.ReadInt32(20) + 4673;
                        break;
                }
                if (lookBack > 0)
                {
                    for (int i = 0;i < copyLength;i++)
                    {
                        output[outIndex] = output[outIndex - lookBack];
                        outIndex++;
                    }
                }
                else
                {
                    if (bitReader.BitOffset == 0)
                    {
                        for (int i = 0;i < copyLength + 1;i++) output[outIndex++] = bitReader.ReadByte(8);
                    }
                    else
                    {
                        var half1 = bitReader.BitOffset;
                        var half2 = 8 - half1;
                        var partialByte = bitReader.ReadInt32(half2);
                        for (int i = 0;i < copyLength;i++) output[outIndex++] = bitReader.ReadByte(8);
                        partialByte |= bitReader.ReadInt32(half1) << half2;
                        output[outIndex++] = (byte)partialByte;
                    }
                }
            }
        }

        private static void DecompressSectionKCDC(BitReader bitReader, byte[] output)
        {
            int outIndex = 0;
            while (outIndex < output.Length)
            {
                int n = 0;
                while (true)
                {
                    if (bitReader.EndOfStream)
                        return;
                    if (bitReader.ReadBit() == 0 || ++n >= 4)
                        break;
                }
                int copyLength = 0;
                int lookBack = 0;
                switch (n)
                {
                    case 0:
                        break;
                    case 1:
                        copyLength += 2;
                        lookBack = bitReader.ReadInt32(6) + 1;
                        break;
                    case 2:
                        copyLength += 2;
                        lookBack = bitReader.ReadInt32(9) + 65;
                        break;
                    case 3:
                        copyLength += 2;
                        lookBack = bitReader.ReadInt32(12) + 577;
                        break;
                    case 4:
                        copyLength += 3;
                        lookBack = bitReader.ReadInt32(20) + 4673;
                        break;
                }
                if (lookBack > 0)
                {
                    n = 0;
                    while (bitReader.ReadBit() == 1) ++n;
                    if (n > 0)
                        copyLength += ((1 << n) - 1) + bitReader.ReadInt32(n);

                    for (n = 0;n < copyLength;n++)
                    {
                        output[outIndex] = output[outIndex - lookBack];
                        outIndex++;
                    }
                }
                else
                {
                    output[outIndex++] = bitReader.ReadByte(8);
                }
            }
        }
    }
}
