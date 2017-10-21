using System;
using System.Runtime.InteropServices;

namespace Meyer.Socrates.Data
{
    public static class BinaryOperations
    {
        private static IntPtr handle = IntPtr.Zero;
        private static int lastHandleSize;
        private static object Synchronized = new object();

        static BinaryOperations()
        {
            EnsureAllocated(1024);
        }

        private static void EnsureAllocated(int size)
        {
            lock (Synchronized)
            {
                if (lastHandleSize < size)
                {
                    if (handle != IntPtr.Zero) Marshal.FreeHGlobal(handle);
                    handle = Marshal.AllocHGlobal(lastHandleSize = size);
                }
            }
        }

        public static byte[] StructToBytes<T>(T value) where T : struct
        {
            int size = GetByteLength(value);
            EnsureAllocated(size);
            lock (Synchronized)
            {
                Marshal.StructureToPtr(value, handle, false);
                var copy = new byte[size];
                Marshal.Copy(handle, copy, 0, size);
                return copy;
            }
        }

        public static byte[] StructToBytes(object value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (!value.GetType().IsValueType) throw new ArgumentException("value is not a structure", "value");
            int size = GetByteLength(value);
            EnsureAllocated(size);
            lock (Synchronized)
            {
                Marshal.StructureToPtr(value, handle, false);
                var copy = new byte[size];
                Marshal.Copy(handle, copy, 0, size);
                return copy;
            }
        }

        public static T BytesToStruct<T>(byte[] buffer) where T : struct
        {
            return (T)BytesToStruct(buffer, typeof(T));
        }

        public static object BytesToStruct(byte[] buffer, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return BitConverter.ToBoolean(buffer, 0);
                case TypeCode.Char:
                    return BitConverter.ToChar(buffer, 0);
                case TypeCode.SByte:
                    return (SByte)buffer[0];
                case TypeCode.Byte:
                    return buffer[0];
                case TypeCode.Int16:
                    return BitConverter.ToInt16(buffer, 0);
                case TypeCode.UInt16:
                    return BitConverter.ToUInt16(buffer, 0);
                case TypeCode.Int32:
                    return BitConverter.ToInt32(buffer, 0);
                case TypeCode.UInt32:
                    return BitConverter.ToUInt32(buffer, 0);
                case TypeCode.Int64:
                    return BitConverter.ToInt64(buffer, 0);
                case TypeCode.UInt64:
                    return BitConverter.ToUInt64(buffer, 0);
                case TypeCode.Single:
                    return BitConverter.ToSingle(buffer, 0);
                case TypeCode.Double:
                    return BitConverter.ToDouble(buffer, 0);
                case TypeCode.Decimal:
                    return (Decimal)BitConverter.ToDouble(buffer, 0);
            }
            int size = GetByteLength(type);
            if (buffer.Length < size) throw new ArgumentException("buffer size is too small.", "buffer");
            EnsureAllocated(size);
            lock (Synchronized)
            {
                Marshal.Copy(buffer, 0, handle, size);
                return Marshal.PtrToStructure(handle, type);
            }
        }

        public static int GetByteLength<T>() where T : struct
        {
            return Marshal.SizeOf<T>();
        }

        public static int GetByteLength<T>(T value) where T : struct
        {
            return Marshal.SizeOf(value);
        }

        public static int GetByteLength(object value)
        {
            return Marshal.SizeOf(value);
        }

        public static int GetByteLength(Type type)
        {
            return Marshal.SizeOf(type);
        }
    }
}
