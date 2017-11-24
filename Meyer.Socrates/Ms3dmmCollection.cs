namespace Meyer.Socrates
{
    using Meyer.Socrates.Data;
    using Meyer.Socrates.Data.Sections;
    using Meyer.Socrates.IO;
    using Meyer.Socrates.Services;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class Ms3dmmCollection: IEnumerable<Chunk>, IResolver<IChunkIdentifier, Chunk>
    {
        private uint collectionID;
        private string[] directoryPaths;
        private List<Ms3dmmFile> files = new List<Ms3dmmFile>();
        private Dictionary<string, Ms3dmmFile> dictionary;
        private List<Stream> fileLocks = new List<Stream>();

        public Ms3dmmFile GetFile(string filename)
        {
            if (!dictionary.TryGetValue(filename, out var file))
                file = dictionary.FirstOrDefault(kvp => kvp.Key.StartsWith(filename, StringComparison.InvariantCultureIgnoreCase)).Value;
            return file;
        }

        private object Synchronized = new object();

        internal Ms3dmmCollection(uint collectionID, string[] directoryPaths, IProgress<ProgressInfo> progress)
        {
            UpdateCollection(collectionID, directoryPaths, progress);
        }

        private void FileSystem_Created(object sender, FileSystemEventArgs e)
        {
            lock (Synchronized)
            {
                if (File.Exists(e.FullPath) && directoryPaths.Any(path => path.ToLowerInvariant() == Directory.GetParent(e.FullPath).FullName.ToLowerInvariant()) && Ms3dmmFile.TryOpen(e.FullPath, out var file))
                {
                    AddFile(e.FullPath, file);
                }
            }
        }

        private void FileSystem_Deleted(object sender, FileSystemEventArgs e)
        {
            lock (Synchronized)
            {
                if (directoryPaths.Any(path => path.ToLowerInvariant() == Directory.GetParent(e.FullPath).FullName.ToLowerInvariant()))
                {
                    RemoveFile(e.FullPath);
                }
            }
        }

        private void FileSystem_Changed(object sender, FileSystemEventArgs e)
        {

        }

        internal void UpdateCollection(uint collectionID, string[] directoryPaths, IProgress<ProgressInfo> progress)
        {
            lock (Synchronized)
            {
                this.collectionID = collectionID;
                this.directoryPaths = directoryPaths;
                files.Clear();
                dictionary = new Dictionary<string, Ms3dmmFile>();

                //if (watchers != null)
                //    foreach (var w in watchers)
                //        w.Dispose();
                //watchers = new FileSystemWatcher[directoryPaths.Length];

                //for(int i=0;i<directoryPaths.Length;i++)
                //{
                //    var watcher = new FileSystemWatcher
                //    {
                //        Path = directoryPaths[i],
                //        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.DirectoryName,
                //        Filter = "*.3th OR *.3cn OR *.chk)"
                //    };
                //    watcher.Changed += FileSystem_Changed;
                //    watcher.Created += FileSystem_Created;
                //    watcher.Deleted += FileSystem_Deleted;
                //    watcher.EnableRaisingEvents = true;
                //}
            }

            var i = 0d;
            var ms3dmmFiles = EnumerateMs3dmmFiles().ToArray();

            var progressInfo = new ProgressInfo();
            Parallel.ForEach(ms3dmmFiles, fname =>
            {
                var oldValue = 0d;
                var fileProgress = new Progress<ProgressInfo>(v =>
                {
                    lock (Synchronized)
                    {
                        i += v.ProgressValue - oldValue;
                        oldValue = v.ProgressValue;
                        progressInfo.ProgressValue = i / ms3dmmFiles.Length;
                        progress.Report(progressInfo);
                    }
                });
                if (ParallelMs3dmmFile.TryOpen(fname, out var file, fileProgress))
                {
                    AddFile(fname, file);
                    lock (Synchronized)
                    {
                        progressInfo.WorkDescription = $"Loaded {fname}";
                        progressInfo.ProgressValue = i / ms3dmmFiles.Length;
                        progress.Report(progressInfo);
                    }
                }
                else
                {
                    lock (Synchronized)
                    {
                        i += 100d - oldValue;
                        progressInfo.WorkDescription = $"Couldn't load {fname}";
                        progressInfo.ProgressValue = i / ms3dmmFiles.Length;
                        progress.Report(progressInfo);
                    }
                }
            });
        }

        private void AddFile(string fname, Ms3dmmFile f)
        {
            lock (Synchronized)
            {
                f.container = this;
                var key = System.IO.Path.GetFileName(fname);
                if (!dictionary.ContainsKey(key))
                {
                    foreach (var c in f)
                    {
                        c.MakeReadOnly();
                        c.Section?.MakeReadOnly();
                    }
                    files.Add(f);
                    dictionary.Add(key, f);
                    fileLocks.Add(File.Open(fname, FileMode.Open, FileAccess.Read, FileShare.Read));
                }
                else
                {
                    // TODO: Find out what happens when two files with same name exist (on CD and on harddrive, for example)
                }
            }
        }

        private void RemoveFile(string fname)
        {
            lock (Synchronized)
            {
                var key = System.IO.Path.GetFileName(fname);
                if (dictionary.TryGetValue(key, out var f))
                {
                    files.Remove(f);
                    dictionary.Remove(key);
                }
            }
        }

        private IEnumerable<string> EnumerateMs3dmmFiles()
        {
            foreach (var fname in directoryPaths.SelectMany(p => Directory.EnumerateFiles(p)))
            {
                var lower = fname.ToLowerInvariant();
                if (Ms3dmm.FILE_EXTENSIONS.Any(e => lower.EndsWith(e)))
                    yield return fname;
            }
        }

        public static Ms3dmmCollection Open(uint collectionID, IProgress<ProgressInfo> progress = null)
        {
            return Ms3dmmCollectionResolver.Default.Resolve(collectionID, progress);
        }

        public static bool TryOpen(uint collectionID, out Ms3dmmCollection coll, IProgress<ProgressInfo> progress = null)
        {
            try
            {
                coll = Open(collectionID, progress);
                return true;
            }
            catch
            {
                coll = null;
                return false;
            }
        }

        public uint CollectionID => collectionID;
        public string[] Directories => directoryPaths;

        public IEnumerable<Section> Sections => files.SelectMany(f => f.Sections);

        public IEnumerable<Ms3dmmFile> Files => files;

        public IEnumerator<Chunk> GetEnumerator()
        {
            return files.SelectMany(f => f).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Chunk Resolve(IChunkIdentifier input)
        {
            return files.AsParallel().Select(f => Resolve(f, input)).FirstOrDefault(c => c != null);
        }

        private Chunk Resolve(IResolver<IChunkIdentifier, Chunk> resolver, IChunkIdentifier input)
        {
            return resolver.Resolve(input);
        }
    }
}
