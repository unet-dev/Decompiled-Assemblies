using Facepunch.Extend;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Network
{
	public class NetRead : Stream
	{
		private MemoryStream _stream;

		public byte[] Data = new byte[3145728];

		public long _length;

		public long _position;

		private static byte[] staticbuffer;

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return this._length;
			}
		}

		public override long Position
		{
			get
			{
				return this._position;
			}
			set
			{
				this._position = value;
			}
		}

		public int Unread
		{
			get
			{
				return (int)(this.Length - this.Position);
			}
		}

		static NetRead()
		{
			NetRead.staticbuffer = new byte[1048576];
		}

		public NetRead()
		{
		}

		public bool Bit()
		{
			return this.UInt8() != 0;
		}

		public int BytesWithSize(byte[] buffer)
		{
			uint num = this.UInt32();
			if (num == 0)
			{
				return 0;
			}
			if ((ulong)num > (long)((int)buffer.Length))
			{
				return -1;
			}
			if ((long)this.Read(buffer, 0, (int)num) != (ulong)num)
			{
				return -1;
			}
			return (int)num;
		}

		public byte[] BytesWithSize()
		{
			uint num = this.UInt32();
			if (num == 0)
			{
				return null;
			}
			if (num > 10485760)
			{
				return null;
			}
			byte[] numArray = new byte[num];
			if ((long)this.Read(numArray, 0, (int)num) != (ulong)num)
			{
				return null;
			}
			return numArray;
		}

		public double Double()
		{
			return this.Read<double>();
		}

		public uint EntityID()
		{
			return this.UInt32();
		}

		public float Float()
		{
			return this.Read<float>();
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		public MemoryStream GetStreamForDecryption()
		{
			if (this._stream == null)
			{
				this._stream = new MemoryStream(this.Data, 0, (int)this.Data.Length, true, true);
			}
			this._stream.SetLength(this.Length);
			return this._stream;
		}

		public uint GroupID()
		{
			return this.UInt32();
		}

		public short Int16()
		{
			return this.Read<short>();
		}

		public int Int32()
		{
			return this.Read<int>();
		}

		public long Int64()
		{
			return this.Read<long>();
		}

		public sbyte Int8()
		{
			return this.Read<sbyte>();
		}

		public byte PacketID()
		{
			return this.UInt8();
		}

		public Quaternion Quaternion()
		{
			return this.Read<Quaternion>();
		}

		public Ray Ray()
		{
			return this.Read<Ray>();
		}

		public override unsafe int Read(byte[] buffer, int offset, int count)
		{
			// 
			// Current member / type: System.Int32 Network.NetRead::Read(System.Byte[],System.Int32,System.Int32)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Network.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Int32 Read(System.Byte[],System.Int32,System.Int32)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public T Read<T>()
		where T : struct, /* modreq(System.Runtime.InteropServices.UnmanagedType) */ ValueType
		{
			if (this.Unread < sizeof(T))
			{
				return default(T);
			}
			T t = this.Data.ReadUnsafe<T>((int)this._position);
			this._position += (long)sizeof(T);
			return t;
		}

		public override int ReadByte()
		{
			if (this._position == this.Length)
			{
				return -1;
			}
			byte data = this.Data[checked((IntPtr)this._position)];
			this._position += (long)1;
			return data;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (origin != SeekOrigin.Current)
			{
				throw new NotImplementedException();
			}
			this._position += offset;
			return this._position;
		}

		public override void SetLength(long value)
		{
			this._length = value;
		}

		public unsafe bool Start(IntPtr data, int length)
		{
			// 
			// Current member / type: System.Boolean Network.NetRead::Start(System.IntPtr,System.Int32)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Network.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean Start(System.IntPtr,System.Int32)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public string String()
		{
			int num = this.BytesWithSize(NetRead.staticbuffer);
			if (num <= 0)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(NetRead.staticbuffer, 0, num);
		}

		public bool TemporaryBytesWithSize(out byte[] buffer, out int size)
		{
			buffer = NetRead.staticbuffer;
			size = 0;
			uint num = this.UInt32();
			if (num == 0)
			{
				return false;
			}
			if ((ulong)num > (long)((int)NetRead.staticbuffer.Length))
			{
				return false;
			}
			size = this.Read(NetRead.staticbuffer, 0, (int)num);
			if ((long)size != (ulong)num)
			{
				return false;
			}
			return true;
		}

		public ushort UInt16()
		{
			return this.Read<ushort>();
		}

		public uint UInt32()
		{
			return this.Read<uint>();
		}

		public ulong UInt64()
		{
			return this.Read<ulong>();
		}

		public byte UInt8()
		{
			return this.Read<byte>();
		}

		public Vector3 Vector3()
		{
			return this.Read<Vector3>();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override void WriteByte(byte value)
		{
			throw new NotImplementedException();
		}
	}
}