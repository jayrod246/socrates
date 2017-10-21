namespace Meyer.Socrates.IO
{
    using SlowMember;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class DataStream: Stream, IDataReadContext, IDataWriteContext
    {
        internal Stream stream;
        internal BinaryReader reader;
        internal BinaryWriter writer;
        internal bool leaveOpen;

        public DataStream() : this(new MemoryStream(), false)
        {
        }

        public DataStream(byte[] buffer) : this(new MemoryStream(buffer), false)
        {
        }

        public DataStream(Stream stream, bool leaveOpen = false)
        {
            this.stream = stream;
            if (stream.CanRead) reader = new BinaryReader(stream, Encoding.Default, true);
            if (stream.CanWrite) writer = new BinaryWriter(stream, Encoding.Default, true);
            this.leaveOpen = leaveOpen;
        }

        public virtual T Read<T>() where T : struct
        {
            if (!CanRead) throw new NotSupportedException("Underlying stream cannot be read.");
            return (T)ReadStatic(typeof(T));
        }

        private object ReadStatic(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return reader.ReadBoolean();
                case TypeCode.Char:
                    return reader.ReadChar();
                case TypeCode.SByte:
                    return reader.ReadSByte();
                case TypeCode.Byte:
                    return reader.ReadByte();
                case TypeCode.Int16:
                    return reader.ReadInt16();
                case TypeCode.UInt16:
                    return reader.ReadUInt16();
                case TypeCode.Int32:
                    return reader.ReadInt32();
                case TypeCode.UInt32:
                    return reader.ReadUInt32();
                case TypeCode.Int64:
                    return reader.ReadInt64();
                case TypeCode.UInt64:
                    return reader.ReadUInt64();
                case TypeCode.Single:
                    return reader.ReadSingle();
                case TypeCode.Double:
                    return reader.ReadDouble();
                case TypeCode.Decimal:
                    return reader.ReadDecimal();
                default:
                    var value = Activator.CreateInstance(type);
                    if (type.IsLayoutSequential)
                    {
                        ReadSequential(value);
                        return value;
                    }
                    if (type.IsExplicitLayout)
                    {
                        ReadExplicit(value);
                        return value;
                    }
                    throw new NotImplementedException();
            }
        }

        private void ReadSequential(object value)
        {
            var type = value.GetType();
            using (var reflection = new ReflectionService())
            {
                foreach (var f in reflection.GetFields(type, true).Where(f => !f.IsStatic).OrderBy(f => f.FieldHandle.Value.ToInt64()))
                {
                    f.SetValue(value, ReadStatic(f.FieldType));
                }
            }
        }

        private void ReadExplicit(object value)
        {
            var type = value.GetType();
            using (var reflection = new ReflectionService())
            {
                foreach (var f in reflection.GetFields(type, true).Where(f => !f.IsStatic).OrderBy(f => System.Runtime.InteropServices.Marshal.OffsetOf(type, f.Name).ToInt32()))
                {
                    f.SetValue(value, ReadStatic(f.FieldType));
                }
            }
        }

        private void WriteStatic(object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    writer.Write((Boolean)value);
                    break;
                case TypeCode.Char:
                    writer.Write((Char)value);
                    break;
                case TypeCode.SByte:
                    writer.Write((SByte)value);
                    break;
                case TypeCode.Byte:
                    writer.Write((Byte)value);
                    break;
                case TypeCode.Int16:
                    writer.Write((Int16)value);
                    break;
                case TypeCode.UInt16:
                    writer.Write((UInt16)value);
                    break;
                case TypeCode.Int32:
                    writer.Write((Int32)value);
                    break;
                case TypeCode.UInt32:
                    writer.Write((UInt32)value);
                    break;
                case TypeCode.Int64:
                    writer.Write((Int64)value);
                    break;
                case TypeCode.UInt64:
                    writer.Write((UInt64)value);
                    break;
                case TypeCode.Single:
                    writer.Write((Single)value);
                    break;
                case TypeCode.Double:
                    writer.Write((Double)value);
                    break;
                case TypeCode.Decimal:
                    writer.Write((Decimal)value);
                    break;
                default:
                    var type = value.GetType();
                    if (type.IsLayoutSequential)
                    {
                        WriteSequential(value);
                    }
                    else if (type.IsExplicitLayout)
                    {
                        WriteExplicit(value);
                    }
                    else throw new NotImplementedException();
                    break;
            }
        }

        private void WriteSequential(object value)
        {
            using (var reflection = new ReflectionService())
            {
                foreach (var f in reflection.GetFields(value.GetType(), true).Where(f => !f.IsStatic).OrderBy(f => f.FieldHandle.Value.ToInt64()))
                {
                    WriteStatic(f.GetValue(value));
                }
            }
        }

        private void WriteExplicit(object value)
        {
            var type = value.GetType();

            using (var reflection = new ReflectionService())
            {
                foreach (var f in reflection.GetFields(value.GetType(), true).Where(f => !f.IsStatic).OrderBy(f => System.Runtime.InteropServices.Marshal.OffsetOf(type, f.Name).ToInt32()))
                {
                    WriteStatic(f.GetValue(value));
                }
            }
        }

        public virtual T[] ReadArray<T>(int count) where T : struct
        {
            if (!CanRead) throw new NotSupportedException("Underlying stream cannot be read.");
            var arr = new T[count];
            if (typeof(T) == typeof(byte))
            {
                var bytes = arr as byte[];
                stream.Read(bytes, 0, count);
            }
            else for (int i = 0;i < count;i++) arr[i] = (T)ReadStatic(typeof(T));
            return arr;
        }

        public override long Position { get => stream.Position; set => stream.Position = value; }

        public override long Length => stream.Length;

        public virtual void Write<T>(T value) where T : struct
        {
            if (!CanWrite) throw new NotSupportedException("Underlying stream cannot be written to.");
            WriteStatic(value);
        }

        public virtual void WriteArray<T>(T[] arr) where T : struct
        {
            if (arr == null) throw new ArgumentNullException("arr");
            if (!CanWrite) throw new NotSupportedException("Underlying stream cannot be written to.");
            WriteArrayShared(arr, 0, arr.Length);
        }

        public virtual void WriteArray<T>(T[] arr, int count) where T : struct
        {
            if (arr == null) throw new ArgumentNullException("arr");
            if (count > arr.Length) throw new ArgumentOutOfRangeException("count greater than array length.", "count");
            if (!CanWrite) throw new NotSupportedException("Underlying stream cannot be written to.");
            WriteArrayShared(arr, 0, count);
        }

        public virtual void WriteArray<T>(T[] arr, int offset, int count) where T : struct
        {
            if (arr == null) throw new ArgumentNullException("arr");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset less than zero.", "offset");
            if (count > arr.Length - offset) throw new ArgumentException("Invalid offset and count.");
            if (!CanWrite) throw new NotSupportedException("Underlying stream cannot be written to.");
            WriteArrayShared(arr, offset, count);
        }

        private void WriteArrayShared<T>(T[] arr, int offset, int count) where T : struct
        {
            if (typeof(T) == typeof(byte))
            {
                var bytes = arr as byte[];
                stream.Write(bytes, offset, count);
            }
            else for (int i = 0;i < count;i++) WriteStatic(arr[offset + i]);
        }

        public byte[] ToArray()
        {
            if (stream is MemoryStream memstream) return memstream.ToArray();
            throw new NotSupportedException($"Not a {typeof(MemoryStream).FullName} type.");
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        public override bool CanRead => stream.CanRead;

        public override bool CanSeek => stream.CanSeek;

        public override bool CanWrite => stream.CanWrite;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (stream.CanRead) reader.Dispose();
                if (stream.CanWrite) writer.Dispose();
                if (!leaveOpen) stream.Dispose();
            }

            base.Dispose(disposing);
        }

        public virtual void Assert<T>(T value) where T : struct
        {
            var got = (T)ReadStatic(typeof(T));
            if (!EqualityComparer<T>.Default.Equals(value, got)) throw new InvalidDataException($"Assertion failed. Expected: {value} Got: {got}") { Data = { { "expectedValue", value }, { "got", got } } };
        }

        public virtual void AssertArray<T>(T[] arr) where T : struct
        {
            if (arr == null) throw new ArgumentNullException("arr");
            var got = ReadArray<T>(arr.Length);
            if (!arr.Zip(got, (a, b) => EqualityComparer<T>.Default.Equals(a, b)).All(b => b)) throw new InvalidDataException($"Assertion failed. Expected: {arr} Got: {got}") { Data = { { "expectedValue", arr }, { "got", got } } };
        }

        public virtual T AssertAny<T>(params T[] values) where T : struct
        {
            var got = (T)ReadStatic(typeof(T));
            if (!values.Any(x => EqualityComparer<T>.Default.Equals(x, got))) throw new InvalidDataException($"Assertion failed. Expected: {values} Got: {got}") { Data = { { "expectedValues", values }, { "got", got } } };
            return got;
        }

        public virtual void ReadArray<T>(T[] arr, int count) where T : struct
        {
            if (!CanRead) throw new NotSupportedException("Underlying stream cannot be read.");
            if (typeof(T) == typeof(byte))
            {
                var bytes = arr as byte[];
                stream.Read(bytes, 0, count);
            }
            else for (int i = 0;i < count;i++) arr[i] = (T)ReadStatic(typeof(T));
        }

        public virtual void ReadArray<T>(T[] arr, int offset, int count) where T : struct
        {
            if (!CanRead) throw new NotSupportedException("Underlying stream cannot be read.");
            if (typeof(T) == typeof(byte))
            {
                var bytes = arr as byte[];
                stream.Read(bytes, offset, count);
            }
            else for (int i = 0;i < count;i++) arr[offset + i] = (T)ReadStatic(typeof(T));
        }
    }
}
