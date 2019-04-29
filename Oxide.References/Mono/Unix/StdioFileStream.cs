using Mono.Unix.Native;
using System;
using System.IO;

namespace Mono.Unix
{
	public class StdioFileStream : Stream
	{
		public readonly static IntPtr InvalidFileStream;

		public readonly static IntPtr StandardInput;

		public readonly static IntPtr StandardOutput;

		public readonly static IntPtr StandardError;

		private bool canSeek;

		private bool canRead;

		private bool canWrite;

		private bool owner = true;

		private IntPtr file = StdioFileStream.InvalidFileStream;

		public override bool CanRead
		{
			get
			{
				return this.canRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.canSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.canWrite;
			}
		}

		public IntPtr Handle
		{
			get
			{
				this.AssertNotDisposed();
				GC.KeepAlive(this);
				return this.file;
			}
		}

		public override long Length
		{
			get
			{
				this.AssertNotDisposed();
				if (!this.CanSeek)
				{
					throw new NotSupportedException("File Stream doesn't support seeking");
				}
				long num = Stdlib.ftell(this.file);
				if (num == (long)-1)
				{
					throw new NotSupportedException("Unable to obtain current file position");
				}
				int num1 = Stdlib.fseek(this.file, (long)0, SeekFlags.SEEK_END);
				UnixMarshal.ThrowExceptionForLastErrorIf(num1);
				long num2 = Stdlib.ftell(this.file);
				if (num2 == (long)-1)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				UnixMarshal.ThrowExceptionForLastErrorIf(Stdlib.fseek(this.file, num, SeekFlags.SEEK_SET));
				GC.KeepAlive(this);
				return num2;
			}
		}

		public override long Position
		{
			get
			{
				this.AssertNotDisposed();
				if (!this.CanSeek)
				{
					throw new NotSupportedException("The stream does not support seeking");
				}
				long num = Stdlib.ftell(this.file);
				if (num == (long)-1)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				GC.KeepAlive(this);
				return num;
			}
			set
			{
				this.AssertNotDisposed();
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		static StdioFileStream()
		{
			StdioFileStream.InvalidFileStream = IntPtr.Zero;
			StdioFileStream.StandardInput = Stdlib.stdin;
			StdioFileStream.StandardOutput = Stdlib.stdout;
			StdioFileStream.StandardError = Stdlib.stderr;
		}

		public StdioFileStream(IntPtr fileStream) : this(fileStream, true)
		{
		}

		public StdioFileStream(IntPtr fileStream, bool ownsHandle)
		{
			this.InitStream(fileStream, ownsHandle);
		}

		public StdioFileStream(IntPtr fileStream, FileAccess access) : this(fileStream, access, true)
		{
		}

		public StdioFileStream(IntPtr fileStream, FileAccess access, bool ownsHandle)
		{
			this.InitStream(fileStream, ownsHandle);
			this.InitCanReadWrite(access);
		}

		public StdioFileStream(string path)
		{
			this.InitStream(StdioFileStream.Fopen(path, "rb"), true);
		}

		public StdioFileStream(string path, string mode)
		{
			this.InitStream(StdioFileStream.Fopen(path, mode), true);
		}

		public StdioFileStream(string path, FileMode mode)
		{
			this.InitStream(StdioFileStream.Fopen(path, StdioFileStream.ToFopenMode(path, mode)), true);
		}

		public StdioFileStream(string path, FileAccess access)
		{
			this.InitStream(StdioFileStream.Fopen(path, StdioFileStream.ToFopenMode(path, access)), true);
			this.InitCanReadWrite(access);
		}

		public StdioFileStream(string path, FileMode mode, FileAccess access)
		{
			this.InitStream(StdioFileStream.Fopen(path, StdioFileStream.ToFopenMode(path, mode, access)), true);
			this.InitCanReadWrite(access);
		}

		private static bool AssertFileMode(string file, FileMode mode)
		{
			bool flag = StdioFileStream.FileExists(file);
			if (mode == FileMode.CreateNew && flag)
			{
				throw new IOException("File exists and FileMode.CreateNew specified");
			}
			if ((mode == FileMode.Open || mode == FileMode.Truncate) && !flag)
			{
				throw new FileNotFoundException("File doesn't exist and FileMode.Open specified", file);
			}
			return flag;
		}

		private void AssertNotDisposed()
		{
			if (this.file == StdioFileStream.InvalidFileStream)
			{
				throw new ObjectDisposedException("Invalid File Stream");
			}
			GC.KeepAlive(this);
		}

		private void AssertValidBuffer(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (offset > (int)buffer.Length)
			{
				throw new ArgumentException("destination offset is beyond array size");
			}
			if (offset > (int)buffer.Length - count)
			{
				throw new ArgumentException("would overrun buffer");
			}
		}

		public override void Close()
		{
			if (this.file == StdioFileStream.InvalidFileStream)
			{
				return;
			}
			if (!this.owner)
			{
				this.Flush();
			}
			else if (Stdlib.fclose(this.file) != 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			this.file = StdioFileStream.InvalidFileStream;
			this.canRead = false;
			this.canSeek = false;
			this.canWrite = false;
			GC.SuppressFinalize(this);
			GC.KeepAlive(this);
		}

		private static bool FileExists(string file)
		{
			bool zero = false;
			IntPtr intPtr = Stdlib.fopen(file, "r");
			zero = intPtr != IntPtr.Zero;
			if (intPtr != IntPtr.Zero)
			{
				Stdlib.fclose(intPtr);
			}
			return zero;
		}

		protected override void Finalize()
		{
			try
			{
				this.Close();
			}
			finally
			{
				base.Finalize();
			}
		}

		public override void Flush()
		{
			this.AssertNotDisposed();
			if (Stdlib.fflush(this.file) != 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			GC.KeepAlive(this);
		}

		private static IntPtr Fopen(string path, string mode)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("path");
			}
			if (mode == null)
			{
				throw new ArgumentNullException("mode");
			}
			IntPtr intPtr = Stdlib.fopen(path, mode);
			if (intPtr == IntPtr.Zero)
			{
				throw new DirectoryNotFoundException("path", UnixMarshal.CreateExceptionForLastError());
			}
			return intPtr;
		}

		private void InitCanReadWrite(FileAccess access)
		{
			bool flag;
			bool flag1;
			if (!this.canRead)
			{
				flag = false;
			}
			else
			{
				flag = (access == FileAccess.Read ? true : access == FileAccess.ReadWrite);
			}
			this.canRead = flag;
			if (!this.canWrite)
			{
				flag1 = false;
			}
			else
			{
				flag1 = (access == FileAccess.Write ? true : access == FileAccess.ReadWrite);
			}
			this.canWrite = flag1;
		}

		private void InitStream(IntPtr fileStream, bool ownsHandle)
		{
			if (StdioFileStream.InvalidFileStream == fileStream)
			{
				throw new ArgumentException(Locale.GetText("Invalid file stream"), "fileStream");
			}
			this.file = fileStream;
			this.owner = ownsHandle;
			try
			{
				if ((long)Stdlib.fseek(this.file, (long)0, SeekFlags.SEEK_CUR) != (long)-1)
				{
					this.canSeek = true;
				}
				Stdlib.fread(IntPtr.Zero, (ulong)0, (ulong)0, this.file);
				if (Stdlib.ferror(this.file) == 0)
				{
					this.canRead = true;
				}
				Stdlib.fwrite(IntPtr.Zero, (ulong)0, (ulong)0, this.file);
				if (Stdlib.ferror(this.file) == 0)
				{
					this.canWrite = true;
				}
				Stdlib.clearerr(this.file);
			}
			catch (Exception exception)
			{
				throw new ArgumentException(Locale.GetText("Invalid file stream"), "fileStream");
			}
			GC.KeepAlive(this);
		}

		public override int Read([In][Out] byte[] buffer, int offset, int count)
		{
			unsafe
			{
				this.AssertNotDisposed();
				this.AssertValidBuffer(buffer, offset, count);
				if (!this.CanRead)
				{
					throw new NotSupportedException("Stream does not support reading");
				}
				ulong num = (ulong)0;
				fixed (byte* numPointer = &buffer[offset])
				{
					num = Stdlib.fread(numPointer, (ulong)1, (ulong)count, this.file);
				}
				if (num != (long)count && Stdlib.ferror(this.file) != 0)
				{
					throw new IOException();
				}
				GC.KeepAlive(this);
				return (int)num;
			}
		}

		public void RestoreFilePosition(FilePosition pos)
		{
			this.AssertNotDisposed();
			if (pos == null)
			{
				throw new ArgumentNullException("value");
			}
			UnixMarshal.ThrowExceptionForLastErrorIf(Stdlib.fsetpos(this.file, pos));
			GC.KeepAlive(this);
		}

		public void Rewind()
		{
			this.AssertNotDisposed();
			Stdlib.rewind(this.file);
			GC.KeepAlive(this);
		}

		public void SaveFilePosition(FilePosition pos)
		{
			this.AssertNotDisposed();
			UnixMarshal.ThrowExceptionForLastErrorIf(Stdlib.fgetpos(this.file, pos));
			GC.KeepAlive(this);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.AssertNotDisposed();
			if (!this.CanSeek)
			{
				throw new NotSupportedException("The File Stream does not support seeking");
			}
			SeekFlags seekFlag = SeekFlags.SEEK_CUR;
			switch (origin)
			{
				case SeekOrigin.Begin:
				{
					seekFlag = SeekFlags.SEEK_SET;
					break;
				}
				case SeekOrigin.Current:
				{
					seekFlag = SeekFlags.SEEK_CUR;
					break;
				}
				case SeekOrigin.End:
				{
					seekFlag = SeekFlags.SEEK_END;
					break;
				}
				default:
				{
					throw new ArgumentException("origin");
				}
			}
			if (Stdlib.fseek(this.file, offset, seekFlag) != 0)
			{
				throw new IOException("Unable to seek", UnixMarshal.CreateExceptionForLastError());
			}
			long num = Stdlib.ftell(this.file);
			if (num == (long)-1)
			{
				throw new IOException("Unable to get current file position", UnixMarshal.CreateExceptionForLastError());
			}
			GC.KeepAlive(this);
			return num;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("ANSI C doesn't provide a way to truncate a file");
		}

		private static string ToFopenMode(string file, FileMode mode)
		{
			string fopenMode = NativeConvert.ToFopenMode(mode);
			StdioFileStream.AssertFileMode(file, mode);
			return fopenMode;
		}

		private static string ToFopenMode(string file, FileAccess access)
		{
			return NativeConvert.ToFopenMode(access);
		}

		private static string ToFopenMode(string file, FileMode mode, FileAccess access)
		{
			string fopenMode = NativeConvert.ToFopenMode(mode, access);
			bool flag = StdioFileStream.AssertFileMode(file, mode);
			if (mode == FileMode.OpenOrCreate && access == FileAccess.Read && !flag)
			{
				fopenMode = "w+b";
			}
			return fopenMode;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			unsafe
			{
				this.AssertNotDisposed();
				this.AssertValidBuffer(buffer, offset, count);
				if (!this.CanWrite)
				{
					throw new NotSupportedException("File Stream does not support writing");
				}
				ulong num = (ulong)0;
				fixed (byte* numPointer = &buffer[offset])
				{
					num = Stdlib.fwrite(numPointer, (ulong)1, (ulong)count, this.file);
				}
				if (num != (long)count)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				GC.KeepAlive(this);
			}
		}
	}
}