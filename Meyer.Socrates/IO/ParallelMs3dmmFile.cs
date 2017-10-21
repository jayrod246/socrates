namespace Meyer.Socrates.IO
{
    using Meyer.Socrates.Collections;
    using Meyer.Socrates.Data;
    using Meyer.Socrates.Data.Sections;
    using Meyer.Socrates.Services;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ParallelMs3dmmFile: Ms3dmmFile
    {
        public ParallelMs3dmmFile()
        {
        }

        public ParallelMs3dmmFile(string filename, IProgress<ProgressInfo> progress = null) : base(filename, progress)
        {
        }

        internal override void Read(string filename, IProgress<ProgressInfo> progress)
        {
            var fileData = File.ReadAllBytes(filename);

            using (var c = new DataStream(fileData))
            {
                long fileOffset = c.Position;
                c.AssertArray(identifierBytes);
                Signature = c.Read<Quad>();
                Unk1 = c.Read<UInt16>();
                Unk2 = c.Read<UInt16>();
                MagicNumber = c.Read<UInt32>();
                var fileLength = c.Read<UInt32>();
                var indexOffset = c.Read<UInt32>();
                var indexLength = c.Read<UInt32>();

                if (fileLength != c.Read<UInt32>())
                {
                    throw new InvalidDataException("Invalid 3DMM header.");
                }

                c.Position = indexOffset;

                if (MagicNumber != c.Read<UInt32>())
                {
                    throw new InvalidDataException("MagicNumber mismatch in file stream.");
                }

                var count = c.Read<UInt32>();
                var entryLength = c.Read<UInt32>();
                int directoryPadding;

                if (c.Read<UInt32>() != 0xFFFFFFFF || (directoryPadding = c.Read<Int32>()) < 20)
                {
                    throw new InvalidDataException("Invalid index header.");
                }

                var entryOffset = (uint)c.Position;
                c.Position = entryOffset + entryLength;

                var directoryOffsets = new uint[count];
                var chunks = new Chunk[count]; // Enumerable.Range(0, (int)count).Select(x => new { Chunk = (Chunk)null, DirectoryOffset = (uint)0 }).ToArray();
                double num = 0;

                Parallel.For(0, count, new ParallelOptions() { MaxDegreeOfParallelism = 3 }, i =>
                {
                    using (var parallelC = new DataStream(fileData))
                    {
                        parallelC.Position = entryOffset + entryLength + (8 * i);
                        directoryOffsets[i] = entryOffset + parallelC.Read<UInt32>();
                        var directoryLength = parallelC.Read<UInt32>();
                        parallelC.Position = directoryOffsets[i];

                        var chunk = new Chunk();
                        uint sectionOffset;
                        uint sectionLength;
                        uint referenceCount;
                        uint timesReferenced;

                        switch (directoryPadding)
                        {
                            case 20:
                                chunk.Quad = parallelC.Read<Quad>();
                                chunk.ID = parallelC.Read<UInt32>();
                                sectionOffset = chunk.sectionOffset = (uint)fileOffset + parallelC.Read<UInt32>();
                                chunk.Mode = (ModeFlags)parallelC.Read<Byte>();
                                sectionLength = parallelC.Read<UInt32>() & 0xFFFFFF;
                                parallelC.Position--;
                                referenceCount = parallelC.Read<UInt16>();
                                timesReferenced = parallelC.Read<UInt16>();
                                break;
                            case 32:
                                chunk.Quad = parallelC.Read<Quad>();
                                chunk.ID = parallelC.Read<UInt32>();
                                sectionOffset = chunk.sectionOffset = (uint)fileOffset + parallelC.Read<UInt32>();
                                sectionLength = parallelC.Read<UInt32>(); // & 0xFFFFFF;
                                                                          //c.Position--;
                                                                          //chunk.Mode = (ModeFlags)c.Read<Byte>();
                                referenceCount = parallelC.Read<UInt32>();
                                timesReferenced = parallelC.Read<UInt32>();

                                var bigMode = parallelC.ReadArray<Byte>(8);
                                bigMode.All(b => b == 0 || b == 1);
                                if (bigMode[0] != 0)
                                {

                                }
                                chunk.Mode = (ModeFlags)(bigMode[6] << 2 | bigMode[5] << 1);
                                //chunk.Mode = (ModeFlags)(bigMode[7] << 1 | bigMode[6] << 2 | bigMode[5] << 3 | bigMode[4] << 4 | bigMode[3] << 5 | bigMode[2] << 6 | bigMode[1] << 7);
                                if ((int)chunk.Mode == 8)
                                {

                                }
                                if (chunk.Mode == ModeFlags.Compressed)
                                {

                                }
                                break;
                            default: throw new InvalidDataException("Failed to handle index padding in file.");
                        }

                        for (int j = 0;j < referenceCount;j++)
                        {
                            chunk.References.Add(new Reference()
                            {
                                Quad = parallelC.Read<Quad>(),
                                ID = parallelC.Read<UInt32>(),
                                ReferenceID = parallelC.Read<UInt32>()
                            });
                        }

                        if (directoryLength - directoryPadding - (referenceCount * 12) > 0)
                        {
                            var strType = parallelC.Read<UInt16>();

                            switch (strType)
                            {
                                case Ms3dmmEncodings.Default: // Windows-1252 string.
                                    chunk.String = Encoding.GetEncoding(1252).GetString(parallelC.ReadArray<byte>(parallelC.Read<byte>()));
                                    break;

                                case Ms3dmmEncodings.Unicode: // Unicode string.
                                    chunk.String = Encoding.Unicode.GetString(parallelC.ReadArray<byte>(parallelC.Read<ushort>() * 2));
                                    break;

                                default:
                                    throw new InvalidDataException($"Encoding of a string could not be determined. Got: 0x{strType:X4}");
                            }
                        }

                        parallelC.Position = sectionOffset;
                        chunk.Section = Section.Create(chunk.Quad);
                        chunk.Section.Data = parallelC.ReadArray<Byte>((int)sectionLength);
                        chunks[i] = chunk;
                        lock(Synchronized)
                        {
                            progress.Report(new ProgressInfo((num++ / count) * 99, $"Loaded {chunk.Quad} {chunk.ID}"));
                        }
                    }
                });

                foreach (var chunk in OrderByUsingIndex(chunks, (o, i) => directoryOffsets[i]))
                {
                    this.Add(chunk);
                }

                progress.Report(new ProgressInfo(100));
            }
        }

        public new static Ms3dmmFile Open(string filename, IProgress<ProgressInfo> progress = null)
        {
            return new ParallelMs3dmmFile(filename, progress);
        }

        public new static bool TryOpen(string filename, out Ms3dmmFile ms3dmmFile, IProgress<ProgressInfo> progress = null)
        {
            try
            {
                ms3dmmFile = Open(filename, progress);
                return true;
            }
            catch
            {
                ms3dmmFile = null;
                return false;
            }
        }
    }
}
