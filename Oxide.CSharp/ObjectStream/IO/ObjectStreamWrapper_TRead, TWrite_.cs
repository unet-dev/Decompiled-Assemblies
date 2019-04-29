using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ObjectStream.IO
{
	public class ObjectStreamWrapper<TRead, TWrite>
	where TRead : class
	where TWrite : class
	{
		private readonly BinaryFormatter _binaryFormatter;

		private readonly Stream _inStream;

		private readonly Stream _outStream;

		private bool _run;

		public bool CanRead
		{
			get
			{
				if (!this._run)
				{
					return false;
				}
				return this._inStream.CanRead;
			}
		}

		public bool CanWrite
		{
			get
			{
				if (!this._run)
				{
					return false;
				}
				return this._outStream.CanWrite;
			}
		}

		public ObjectStreamWrapper(Stream inStream, Stream outStream)
		{
			this._inStream = inStream;
			this._outStream = outStream;
			this._run = true;
		}

		public void Close()
		{
			if (!this._run)
			{
				return;
			}
			this._run = false;
			try
			{
				this._outStream.Close();
			}
			catch (Exception exception)
			{
			}
			try
			{
				this._inStream.Close();
			}
			catch (Exception exception1)
			{
			}
		}

		private void Flush()
		{
			this._outStream.Flush();
		}

		private int ReadLength()
		{
			byte[] numArray = new byte[4];
			int num = this._inStream.Read(numArray, 0, 4);
			if (num == 0)
			{
				return 0;
			}
			if (num == 4)
			{
				return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(numArray, 0));
			}
			Array.Resize<byte>(ref numArray, (int)Encoding.UTF8.GetPreamble().Length);
			if (!Encoding.UTF8.GetPreamble().SequenceEqual<byte>(numArray))
			{
				throw new IOException(string.Format("Expected {0} bytes but read {1}", 4, num));
			}
			return this.ReadLength();
		}

		public TRead ReadObject()
		{
			int num = this.ReadLength();
			if (num != 0)
			{
				return this.ReadObject(num);
			}
			return default(TRead);
		}

		private TRead ReadObject(int len)
		{
			int num = 0;
			TRead tRead;
			byte[] numArray = new byte[len];
			for (int i = 0; len - i > 0; i += num)
			{
				int num1 = this._inStream.Read(numArray, i, len - i);
				num = num1;
				if (num1 <= 0)
				{
					break;
				}
			}
			using (MemoryStream memoryStream = new MemoryStream(numArray))
			{
				tRead = (TRead)this._binaryFormatter.Deserialize(memoryStream);
			}
			return tRead;
		}

		private byte[] Serialize(TWrite obj)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this._binaryFormatter.Serialize(memoryStream, obj);
				array = memoryStream.ToArray();
			}
			return array;
		}

		private void WriteLength(int len)
		{
			byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(len));
			this._outStream.Write(bytes, 0, (int)bytes.Length);
		}

		public void WriteObject(TWrite obj)
		{
			byte[] numArray = this.Serialize(obj);
			this.WriteLength((int)numArray.Length);
			this.WriteObject(numArray);
			this.Flush();
		}

		private void WriteObject(byte[] data)
		{
			this._outStream.Write(data, 0, (int)data.Length);
		}
	}
}