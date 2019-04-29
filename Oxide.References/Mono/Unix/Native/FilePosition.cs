using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Unix.Native
{
	public sealed class FilePosition : MarshalByRefObject, IEquatable<FilePosition>, IDisposable
	{
		private readonly static int FilePositionDumpSize;

		private HandleRef pos;

		internal HandleRef Handle
		{
			get
			{
				return this.pos;
			}
		}

		static FilePosition()
		{
			FilePosition.FilePositionDumpSize = Stdlib.DumpFilePosition(null, new HandleRef(null, IntPtr.Zero), 0);
		}

		public FilePosition()
		{
			IntPtr intPtr = Stdlib.CreateFilePosition();
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException("Unable to malloc fpos_t!");
			}
			this.pos = new HandleRef(this, intPtr);
		}

		private void Cleanup()
		{
			if (this.pos.Handle != IntPtr.Zero)
			{
				Stdlib.free(this.pos.Handle);
				this.pos = new HandleRef(this, IntPtr.Zero);
			}
		}

		public void Dispose()
		{
			this.Cleanup();
			GC.SuppressFinalize(this);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj as FilePosition == null)
			{
				return false;
			}
			return this.ToString().Equals(obj.ToString());
		}

		public bool Equals(FilePosition value)
		{
			if (object.ReferenceEquals(this, value))
			{
				return true;
			}
			return this.ToString().Equals(value.ToString());
		}

		protected override void Finalize()
		{
			try
			{
				this.Cleanup();
			}
			finally
			{
				base.Finalize();
			}
		}

		private string GetDump()
		{
			if (FilePosition.FilePositionDumpSize <= 0)
			{
				return "internal error";
			}
			StringBuilder stringBuilder = new StringBuilder(FilePosition.FilePositionDumpSize + 1);
			if (Stdlib.DumpFilePosition(stringBuilder, this.Handle, FilePosition.FilePositionDumpSize + 1) <= 0)
			{
				return "internal error dumping fpos_t";
			}
			return stringBuilder.ToString();
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public static bool operator ==(FilePosition lhs, FilePosition rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(FilePosition lhs, FilePosition rhs)
		{
			return !object.Equals(lhs, rhs);
		}

		public override string ToString()
		{
			return string.Concat(new string[] { "(", base.ToString(), " ", this.GetDump(), ")" });
		}
	}
}