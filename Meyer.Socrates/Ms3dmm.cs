namespace Meyer.Socrates
{
    using Meyer.Socrates.Helper;
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Contains a few helpful methods, most of which were taken from Foone's code.
    /// https://github.com/foone/3dmmInternals/blob/master/generate/ms3dmm.py
    /// </summary>
    public static class Ms3dmm
    {
        internal const double PROGRESS_THRESHOLD = 1d;
        internal const string EXE_NAME = "3dmovie.exe";
        internal const string KEY_PATH = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft Kids\3D Movie Maker";
        internal const string PRODUCTS_KEY_PATH = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft Kids\3D Movie Maker\Products";
        internal const uint MAGIC_NUM_US = 0x03030001;
        internal const uint MAGIC_NUM_JP = 0x05050001;
        internal static readonly string[] FILE_EXTENSIONS = ".3cn;.3th;.chk".Split(';');

        public static IEnumerable<Ms3dmmCollection> EnumerateCollections()
        {
            foreach (var value in RegistryHelper.GetValueNames(PRODUCTS_KEY_PATH))
            {
                if (uint.TryParse(value, out uint collectionID) && Ms3dmmCollection.TryOpen(collectionID, out var coll))
                    yield return coll;
            }
        }

        public static string GetInstalledEXEPath()
        {
            return Path.Combine(Get3DMovieMakerPath(), EXE_NAME);
        }

        public static string Get3DMovieMakerPath()
        {
            //using (var hKey = RegistryHelper.LocalMachine.OpenSubKey(KEY_PATH))
            //    return Path.Combine((string)hKey.GetValue("InstallDirectory"), (string)hKey.GetValue("InstallSubDir"));
            var installDirectory = RegistryHelper.GetValue(KEY_PATH, "InstallDirectory", "");
            var installSubDir = RegistryHelper.GetValue(KEY_PATH, "InstallSubDir", "");
            return Path.Combine(installDirectory, installSubDir);
        }

        public static string GetInstallDirectory()
        {
            //using (var hKey = RegistryHelper.LocalMachine.OpenSubKey(KEY_PATH))
            //    return (string)hKey.GetValue("InstallDirectory");
            return RegistryHelper.GetValue(KEY_PATH, "InstallDirectory", "");
        }

        public static string GetAnyEXE()
        {
            return File.Exists(EXE_NAME) ? EXE_NAME : GetInstalledEXEPath();
        }

        public static string[] GetCollectionDirectories(uint collectionID)
        {
            var drives = DriveInfo.GetDrives();
            var buffer = new string[drives.Length + 1];
            int num = 0;

            string relative_path = RegistryHelper.GetValue<string>(PRODUCTS_KEY_PATH, collectionID.ToString(), null);

            if (relative_path == null) throw new ArgumentException("collectionID does not exist in registry.", "collectionID");


            foreach (var root in drives.Where(d => d.DriveType.HasFlag(DriveType.CDRom)).Select(d => d.RootDirectory.FullName).Prepend(GetInstallDirectory()))
            {
                if (relative_path.Split('/').Select(s => Path.Combine(root, s)).FirstOrDefault(s => Directory.Exists(s)) is string path)
                    buffer[num++] = path;
            }

            if (num == 0) throw new DirectoryNotFoundException("No directory could be found.");
            var paths = new string[num];
            Array.Copy(buffer, paths, num);
            return paths;
        }
    }
}
