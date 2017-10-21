using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Meyer.Socrates.Data
{
    internal unsafe static class DecompProxy
    {
        [DllImport("DecompProxy.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr DP_Init([MarshalAs(UnmanagedType.LPStr)] string filename);

        [DllImport("DecompProxy.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int DP_Shutdown(IntPtr extractContext);

        [DllImport("DecompProxy.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int DP_GetSize([MarshalAs(UnmanagedType.LPArray)] byte[] section, int sectionSize);

        [DllImport("DecompProxy.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int DP_DecompressSmart(IntPtr extractContext, [MarshalAs(UnmanagedType.LPArray)] byte[] section, int sectionSize, [Out, MarshalAs(UnmanagedType.LPArray)] byte[] output);

        private static IntPtr extractContext;

        private static object Synchronized = new object();

        public static byte[] Decompress(byte[] buffer)
        {
            if (extractContext == IntPtr.Zero)
            {
                lock (Synchronized)
                {
                    if (extractContext == IntPtr.Zero)
                    {
                        var filename = Ms3dmm.GetAnyEXE();
                        if (!File.Exists(filename)) throw new InvalidOperationException("3D Movie Maker exe could not be found.");
                        extractContext = DP_Init(filename);
                    }
                }
            }

            int size = DP_GetSize(buffer, buffer.Length);
            if (size < 0) return buffer;
            var result = new byte[size];
            if (DP_DecompressSmart(extractContext, buffer, buffer.Length, result) != size)
                return buffer;
            return result;
        }
    }
}
