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

    public class Ms3dmmFile: ChunkyCollection
    {
        private string fileFullPath;
        internal Ms3dmmCollection container;
        internal static readonly Quad defaultIdentifier = new Quad("CHN2");
        internal static readonly byte[] identifierBytes = new byte[] { 0x43, 0x48, 0x4E, 0x32 };

        #region Public properties

        public string FileName => Path.GetFileName(fileFullPath ?? "");
        public Quad Identifier => defaultIdentifier;
        public Quad Signature { get; set; }
        public ushort Unk1 { get; set; }
        public ushort Unk2 { get; set; }
        public uint MagicNumber { get; set; }

        public IEnumerable<Section> Sections => this.Select(c => c.Section);

        public Ms3dmmCollection Container => container;

        #endregion

        public Ms3dmmFile()
        {
            Signature = new Quad("CHMP");
            MagicNumber = Ms3dmm.MAGIC_NUM_US;
        }

        public Ms3dmmFile(string filename, IProgress<ProgressInfo> progress = null) : this()
        {
            fileFullPath = Path.GetFullPath(filename);
            Read(filename, new ThrottleProgressInfo(progress));
        }

        internal virtual void Read(string filename, IProgress<ProgressInfo> progress)
        {
            using (var c = new DataStream(File.OpenRead(filename)))
                Read(c, progress);
        }

        internal void Read(IDataReadContext c, IProgress<ProgressInfo> progress)
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

            for (int i = 0;i < count;i++)
            {
                if (directoryPadding != 20)
                {

                }

                c.Position = entryOffset + entryLength + (8 * i);
                directoryOffsets[i] = entryOffset + c.Read<UInt32>();
                var directoryLength = c.Read<UInt32>();
                c.Position = directoryOffsets[i];

                var chunk = new Chunk();
                uint sectionOffset;
                uint sectionLength;
                uint referenceCount;
                uint timesReferenced;

                switch (directoryPadding)
                {
                    case 20:
                        chunk.Quad = c.Read<Quad>();
                        chunk.ID = c.Read<UInt32>();
                        sectionOffset = chunk.sectionOffset = (uint)fileOffset + c.Read<UInt32>();
                        chunk.Mode = (ModeFlags)c.Read<Byte>();
                        sectionLength = c.Read<UInt32>() & 0xFFFFFF;
                        c.Position--;
                        referenceCount = c.Read<UInt16>();
                        timesReferenced = c.Read<UInt16>();
                        break;
                    case 32:
                        chunk.Quad = c.Read<Quad>();
                        chunk.ID = c.Read<UInt32>();
                        sectionOffset = chunk.sectionOffset = (uint)fileOffset + c.Read<UInt32>();
                        sectionLength = c.Read<UInt32>(); // & 0xFFFFFF;
                        //c.Position--;
                        //chunk.Mode = (ModeFlags)c.Read<Byte>();
                        referenceCount = c.Read<UInt32>();
                        timesReferenced = c.Read<UInt32>();

                        var bigMode = c.ReadArray<Byte>(8);
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
                        Quad = c.Read<Quad>(),
                        ID = c.Read<UInt32>(),
                        ReferenceID = c.Read<UInt32>()
                    });
                }

                if (directoryLength - directoryPadding - (referenceCount * 12) > 0)
                {
                    var strType = c.Read<UInt16>();

                    switch (strType)
                    {
                        case Ms3dmmEncodings.Default: // Windows-1252 string.
                            chunk.String = Encoding.GetEncoding(1252).GetString(c.ReadArray<byte>(c.Read<byte>()));
                            break;

                        case Ms3dmmEncodings.Unicode: // Unicode string.
                            chunk.String = Encoding.Unicode.GetString(c.ReadArray<byte>(c.Read<ushort>() * 2));
                            break;

                        default:
                            throw new InvalidDataException($"Encoding of a string could not be determined. Got: 0x{strType:X4}");
                    }
                }

                c.Position = sectionOffset;
                chunk.Section = Section.Create(chunk.Quad);
                chunk.Section.Data = c.ReadArray<Byte>((int)sectionLength);
                chunks[i] = chunk;
                progress.Report(new ProgressInfo(((double)i / count) * 99, $"Loaded {chunk.Quad} {chunk.ID}"));
            }

            foreach (var chunk in OrderByUsingIndex(chunks, (o, i) => directoryOffsets[i]))
            {
                this.Add(chunk);
            }

            progress.Report(new ProgressInfo(100));
        }

        public static Ms3dmmFile Open(string filename, IProgress<ProgressInfo> progress = null)
        {
            return new Ms3dmmFile(filename, progress);
        }

        public static bool TryOpen(string filename, out Ms3dmmFile ms3dmmFile, IProgress<ProgressInfo> progress = null)
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

        public static void SaveAs(Ms3dmmFile file, string filename)
        {
            if (file == null) throw new ArgumentNullException("file");
            file.Save(filename);
        }

        public void Save(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename) || Path.GetInvalidPathChars().Any(c => filename.Contains(c))) throw new ArgumentException("Invalid filename", "filename");
            using (var c = new DataStream(File.Open(filename, FileMode.Create, FileAccess.Write)))
                Write(c);
        }

        internal void Write(IDataWriteContext c)
        {
            long fileOffset = c.Position;
            c.Position = fileOffset + 128;
            var chunks = this.AsEnumerable().ToArray();
            var directorySorted = chunks.OrderBy(x => x.Quad).ThenBy(x => x.ID).ToArray();
            var tmpMetadata = new MetadataDictionary<Chunk>();

            foreach (var x in chunks)
            {
                tmpMetadata.GetMetadata(x)["SectionOffset"] = x.sectionOffset = (uint)c.Position;
                var mode = x.Mode;
                if (mode == ModeFlags.Automatic)
                {
                    mode = 0;
                    if (x.Section != null && x.Section.CompressionType != CompressionType.Uncompressed) mode |= ModeFlags.Compressed;
                    if (x.ReferencedBy.Count == 0) mode |= ModeFlags.Main;
                }
                tmpMetadata.GetMetadata(x)["Mode"] = mode;

                byte[] data = null;
                if (x.Section == null) data = Array.Empty<byte>();
                else
                {
                    data = x.Section.Data;
                    if (!mode.HasFlag(ModeFlags.Compressed)) data = Compression.Decompress(data); // Ensure data is uncompressed, otherwise 3DMM will throw a fit.
                    else
                    {
                        // Ensure data is compressed, otherwise 3DMM will throw a fit.
                        var compressionType = x.Section.CompressionType == CompressionType.Uncompressed ? CompressionType.KCDC : x.Section.CompressionType;

                        if (compressionType != Compression.GetCompressionType(data))
                        {
                            var uncompressed = Compression.Decompress(data);
                            data = Compression.Compress(uncompressed, compressionType);
                        }
                    }
                }
                tmpMetadata.GetMetadata(x)["SectionLength"] = (uint)data.Length;
                c.WriteArray(data);
            }

            uint indexOffset = (uint)c.Position;
            c.Position += 20;

            foreach (var chk in chunks)
            {
                var meta = tmpMetadata.GetMetadata(chk);
                meta["DirectoryOffset"] = (uint)(c.Position - indexOffset - 20);
                c.Write(chk.Quad);
                c.Write(chk.ID);
                c.Write(chk.SectionOffset);
                c.Write((byte)meta["Mode"]);
                c.Write((uint)meta["SectionLength"]);
                c.Position--;
                c.Write((ushort)chk.References.Count);
                c.Write((ushort)chk.ReferencedBy.Count);
                uint directoryLength = 20;

                foreach (var r in chk.References)
                {
                    c.Write(r.Quad);
                    c.Write(r.ID);
                    c.Write(r.ReferenceID);
                    directoryLength += 12;
                }

                // Strings
                if (!string.IsNullOrEmpty(chk.String))
                {
                    string str = chk.String;

                    switch (Ms3dmmEncodings.GetSuitableEncoding(chk.String))
                    {
                        case Ms3dmmEncodings.Default:
                            if (chk.ForcesUnicode) goto case Ms3dmmEncodings.Unicode;
                            if (str.Length > byte.MaxValue) str = str.Substring(0, byte.MaxValue);

                            c.Write(Ms3dmmEncodings.Default);
                            c.Write((byte)str.Length);
                            c.WriteArray(Encoding.GetEncoding(1252).GetBytes(chk.String));

                            if (str.Length % 4 != 0)
                                c.WriteArray(new byte[4 - (str.Length % 4)]);

                            c.Write((char)0);
                            directoryLength += (uint)(3 + str.Length + 1);
                            break;
                        case Ms3dmmEncodings.Unicode:
                            var bufferUnicode = Encoding.Unicode.GetBytes(chk.String);
                            var result = Encoding.GetEncoding(1252).GetString(bufferUnicode);

                            if (result != chk.String)
                            {
                                // TODO: Show a warning message for incompatible strings.
                            }

                            c.Write(Ms3dmmEncodings.Unicode);
                            c.Write((ushort)result.Length);
                            c.WriteArray(bufferUnicode);

                            if (str.Length % 2 == 0)
                                c.WriteArray(new byte[4]);
                            else
                                c.WriteArray(new byte[2]);

                            directoryLength += (uint)(4 + (str.Length * 2) + 2);
                            break;
                    }
                }

                meta["DirectoryLength"] = directoryLength;
            }

            uint entryLength = (uint)(c.Position - indexOffset - 20);

            foreach (var chk in directorySorted)
            {
                var meta = tmpMetadata.GetMetadata(chk);
                c.Write((uint)meta["DirectoryOffset"]);
                c.Write((uint)meta["DirectoryLength"]);
            }

            uint indexLength = (uint)(c.Position - indexOffset);
            uint fileLength = (uint)(c.Position - fileOffset);
            c.Position = indexOffset;

            // Index Header
            c.Write(MagicNumber);
            c.Write((uint)chunks.Length);
            c.Write(entryLength);
            c.Write(0xFFFFFFFF);
            c.Write(20);

            c.Position = fileOffset;

            // Header
            c.WriteArray(identifierBytes);
            c.Write(Signature);
            c.Write(Unk1);
            c.Write(Unk2);
            c.Write(MagicNumber);
            c.Write(fileLength);
            c.Write(indexOffset);
            c.Write(indexLength);
            c.Write(fileLength);
            c.WriteArray(new byte[24]);
        }

        public IEnumerable<Chunk> ByQuad(string quad) => this.AsEnumerable().Where(x => string.Equals(x.Quad, quad, StringComparison.InvariantCultureIgnoreCase));

        internal static IEnumerable<T> OrderByUsingIndex<T, TKey>(IEnumerable<T> source, Func<T, int, TKey> keySelector)
        {
            int i = 0;
            return source.OrderBy(o => keySelector.Invoke(o, i++));
        }

        public void PreloadSections()
        {
            System.Threading.Tasks.Parallel.ForEach(Sections.OfType<VirtualSection>().ToArray(), s => s.EnsureLoaded());
        }
    }
}
