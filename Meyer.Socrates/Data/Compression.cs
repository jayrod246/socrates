namespace Meyer.Socrates.Data
{
    using Meyer.Socrates.IO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Static class which supports compressing and decompressing section data.
    /// </summary>
    public static class Compression
    {
        private const uint KCDC = (uint)CompressionType.KCDC;
        private const uint KCD2 = (uint)CompressionType.KCD2;
        private const int DEFAULT_MaxLookBack = 2048;

        /// <summary>
        /// The maximum number of bytes to look back when compressing sections.
        /// </summary>
        public static int MaxLookBack { get; set; } = DEFAULT_MaxLookBack;

        /// <summary>
        /// Decompresses the data in the buffer specified.
        /// </summary>
        /// <param name="buffer">The data to be decompressed.</param>
        /// <returns>The decompressed data, or the original buffer if it was not compressed.</returns>
        /// <exception cref="ArgumentNullException">buffer is null.</exception>
        /// <exception cref="EndOfStreamException">The end of the data stream was unexpectedly reached.</exception>
        public static byte[] Decompress(byte[] buffer)
        {
            return Decompress(buffer, out _);
        }

        /// <summary>
        /// Decompresses the data in the buffer specified.
        /// </summary>
        /// <param name="buffer">The data to be decompressed.</param>
        /// <param name="compressionType">The <seealso cref="CompressionType"/> the original data was in.</param>
        /// <returns>The decompressed data, or the original buffer if it was not compressed.</returns>
        /// <exception cref="ArgumentNullException">buffer is null</exception>
        /// <exception cref="EndOfStreamException">The end of the data stream was unexpectedly reached.</exception>
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

        /// <summary>
        /// Gets the compression type of a buffer.
        /// </summary>
        /// <param name="buffer">The bytes to get the compression type of.</param>
        /// <returns>CompressionType</returns>
        public unsafe static CompressionType GetCompressionType(byte[] buffer)
        {
            fixed (byte* ptr = buffer)
            {
                return GetCompressionType(ptr);
            }
        }

        /// <summary>
        /// Gets the compression type from a pointer.
        /// </summary>
        /// <param name="buffer">The pointer to the first byte from which to get the compression type.</param>
        /// <returns>CompressionType</returns>
        public unsafe static CompressionType GetCompressionType(void* buffer)
        {
            const uint KCDC = (uint)CompressionType.KCDC;
            const uint KCD2 = (uint)CompressionType.KCD2;

            if (buffer == default(void*))
                return CompressionType.Uncompressed;

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
            int outIndex = 0; // The current position in the output.
            int copyLength = 0; // The amount of bytes to copy.
            int lookBack = 0; // The offset to begin copying from, relative to outIndex.
            int n = 0; // A variable used for various loops.

            while (outIndex < output.Length)
            {
                /* 
                    *** How to read CopyLength ***

                    Read n bits (n = number of bits it takes until we get a 0), and interpret that as a n-bit integer.
                    Then, using the same n as before, read the next n bits after the zero, interpret as a n-bit integer and add this to the first value.

                    EXAMPLE:

                        Let's say the following bits are next to be read:

                        01011

                        Starting from the right side, or LSB (least significant bit):

                        - 2 bits are set before the first 0, therefore:
                          n = 2

                        - Interpret those bits as a 2 bit integer:
                          v1 = 3    010|11
                                        ^
                        - Interpret the next 2 bits after the 0 as a 2 bit integer:
                          v2 = 1    01|011
                                    ^
                        - Add v1 and v2 together:
                          copyLength = 4
                */

                n = 0;
                while (true)
                {
                    if (bitReader.EndOfStream)
                        throw new EndOfStreamException("The end of the data stream was unexpectedly reached.");
                    if (bitReader.ReadBit() == 0)
                        break;
                    ++n;
                }
                copyLength = ((1 << n) - 1) + bitReader.ReadInt32(n);

                /*
                   We determine if we should:
                        1) Look back from the current position and copy.
                        2) Insert new bytes at the current position.

                   Possible values:
                   0    -   1 bit
                   01   -   2 bits
                   011  -   3 bits
                   0111 -   4 bits
                   1111 -   4 bits
                */

                n = 0;
                while (n < 4 && bitReader.ReadBit() == 1) n++; // Count the number of bits set to 1, without exceeding a maximum of 4 bits.
                switch (n)
                {
                    case 0:
                        lookBack = 0;
                        break;
                    case 1:
                        // CopyLength gets increased by 2.
                        copyLength += 2;
                        // LookBack is a 6-bit integer + 1.
                        lookBack = bitReader.ReadInt32(6) + 1;
                        // Range: (1:64)
                        break;
                    case 2:
                        // CopyLength gets increased by 2.
                        copyLength += 2;
                        // LookBack is a 9-bit integer + 65.
                        lookBack = bitReader.ReadInt32(9) + 65;
                        // Range: (65:576)
                        break;
                    case 3:
                        // CopyLength gets increased by 2.
                        copyLength += 2;
                        // LookBack is a 12-bit integer + 577.
                        lookBack = bitReader.ReadInt32(12) + 577;
                        // Range: (577:4672)
                        break;
                    case 4:
                        // CopyLength gets increased by 3.
                        copyLength += 3;
                        // LookBack is a 20-bit integer + 4673.
                        lookBack = bitReader.ReadInt32(20) + 4673;
                        // Range: (4673:1053248)
                        break;
                }
                if (lookBack > 0)
                {
                    // Case 1) Look back from the current position and copy.
                    for (int i = 0;i < copyLength;i++)
                    {
                        output[outIndex] = output[outIndex - lookBack];
                        outIndex++;
                    }
                }
                else
                {
                    // Case 2) Insert new bytes at the current position.
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
            int outIndex = 0; // The current position in the output.
            int copyLength = 0; // The amount of bytes to copy.
            int lookBack = 0; // The offset to begin copying from, relative to outIndex.
            int n = 0; // A variable used for various loops.

            while (outIndex < output.Length)
            {
                /*
                   We determine if we should:
                        1) Look back from the current position and copy.
                        2) Insert new bytes at the current position.

                   Possible values:
                   0    -   1 bit
                   01   -   2 bits
                   011  -   3 bits
                   0111 -   4 bits
                   1111 -   4 bits
                */
                n = 0;
                while (true)
                {
                    if (bitReader.EndOfStream)
                        throw new EndOfStreamException("The end of the data stream was unexpectedly reached.");
                    if (bitReader.ReadBit() == 0 || ++n >= 4)
                        break;
                }
                switch (n)
                {
                    case 0:
                        lookBack = 0;
                        break;
                    case 1:
                        // CopyLength gets increased by 2.
                        copyLength += 2;
                        // LookBack is a 6-bit integer + 1.
                        lookBack = bitReader.ReadInt32(6) + 1;
                        // Range: (1:64)
                        break;
                    case 2:
                        // CopyLength gets increased by 2.
                        copyLength += 2;
                        // LookBack is a 9-bit integer + 65.
                        lookBack = bitReader.ReadInt32(9) + 65;
                        // Range: (65:576)
                        break;
                    case 3:
                        // CopyLength gets increased by 2.
                        copyLength += 2;
                        // LookBack is a 12-bit integer + 577.
                        lookBack = bitReader.ReadInt32(12) + 577;
                        // Range: (577:4672)
                        break;
                    case 4:
                        // CopyLength gets increased by 3.
                        copyLength += 3;
                        // LookBack is a 20-bit integer + 4673.
                        lookBack = bitReader.ReadInt32(20) + 4673;
                        // Range: (4673:1053248)
                        break;
                }
                if (lookBack > 0)
                {
                    // Case 1) Look back from the current position and copy.
                    /* 
                        *** How to read CopyLength ***

                        Read n bits (n = number of bits it takes until we get a 0), and interpret that as a n-bit integer.
                        Then, using the same n as before, read the next n bits after the zero, interpret as a n-bit integer and add this to the first value.

                        EXAMPLE:

                            Let's say the following bits are next to be read:

                            01011

                            Starting from the right side, or LSB (least significant bit):
                    
                            - 2 bits are set before the first 0, therefore:
                              n = 2

                            - Interpret those bits as a 2 bit integer:
                              v1 = 3    010|11
                                            ^
                            - Interpret the next 2 bits after the 0 as a 2 bit integer:
                              v2 = 1    01|011
                                        ^
                            - Add v1 and v2 together:
                              copyLength = 4
                    */
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

        // TODO: Cleanup Compression.Compress method.
        /// <summary>
        /// Compresses the buffer using the specified <seealso cref="CompressionType"/>.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="compressionType"></param>
        /// <returns></returns>
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

            var bitWriter = new BitWriter(buffer.Length);

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
                                bitWriter.WriteBit(Bit.Zero);
                                bitWriter.WriteByte(lit);
                            }
                            l.Clear();
                        }
                    };

                    copy = (lookBack, copyLength) =>
                    {
                        if (lookBack >= 4673)
                        {
                            copyLength -= 3;
                            bitWriter.WriteByte(0b1111, 4);
                            bitWriter.WriteInt32(lookBack - 4673, 20);
                        }
                        else if (lookBack >= 577)
                        {
                            copyLength -= 2;
                            bitWriter.WriteByte(0b0111, 4);
                            bitWriter.WriteInt32(lookBack - 577, 12);
                        }
                        else if (lookBack >= 65)
                        {
                            copyLength -= 2;
                            bitWriter.WriteByte(0b0011, 3);
                            bitWriter.WriteInt32(lookBack - 65, 9);
                        }
                        else if (lookBack >= 1)
                        {
                            copyLength -= 2;
                            bitWriter.WriteByte(0b0001, 2);
                            bitWriter.WriteInt32(lookBack - 1, 6);
                        }

                        int n = 1;
                        while (copyLength >= (1 << n) - 1)
                        {
                            bitWriter.WriteBit(Bit.One);
                            n++;
                        }
                        bitWriter.WriteBit(Bit.Zero);
                        if (n > 1) bitWriter.WriteInt32(copyLength - ((1 << n) - 1), n - 1);
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
                                bitWriter.WriteBit(Bit.One);
                                n++;
                            }
                            bitWriter.WriteBit(Bit.Zero);
                            if (n > 1) bitWriter.WriteInt32(copyLength - ((1 << n) - 1), n - 1);
                            bitWriter.WriteBit(Bit.Zero);
                            bool hasPartial = bitWriter.BitPosition != 0;
                            if (!hasPartial)
                            {
                                for (int i = 0;i < l.Count;i++)
                                    bitWriter.WriteByte(l[i]);
                            }
                            else
                            {
                                var partialByte = l[l.Count - 1];
                                var half1 = bitWriter.BitPosition;
                                var half2 = 8 - half1;
                                bitWriter.WriteByte(partialByte, half2);
                                for (int i = 0;i < l.Count - 1;i++)
                                    bitWriter.WriteByte(l[i]);
                                bitWriter.WriteByte((byte)(partialByte >> half2), half1);
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
                            bitWriter.WriteBit(Bit.One);
                            n++;
                        }
                        bitWriter.WriteBit(Bit.Zero);
                        if (n > 1) bitWriter.WriteInt32(copyLength - ((1 << n) - 1), n - 1);
                        bitWriter.WriteInt32(lookBackValue, lookBackBits);
                    };
                    break;
                default:
                    throw new ArgumentException("compressionType");
            }

            bitWriter.WriteUInt32((uint)compressionType);
            var len = buffer.Length;
            bitWriter.WriteInt32((len >> 24) & 0xFF, 8);
            bitWriter.WriteInt32((len >> 16) & 0xFF, 8);
            bitWriter.WriteInt32((len >> 8) & 0xFF, 8);
            bitWriter.WriteInt32(len & 0xFF, 8);
            bitWriter.WriteByte(0);
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
            bitWriter.WriteUInt32(0xFFFFFFFF);
            bitWriter.WriteUInt32(0xFFFFFFFF, 32 - bitWriter.BitPosition);

            return bitWriter.ToArray();
        }
    }
}
