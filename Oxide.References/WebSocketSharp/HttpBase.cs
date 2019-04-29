using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using WebSocketSharp.Net;

namespace WebSocketSharp
{
	internal abstract class HttpBase
	{
		private NameValueCollection _headers;

		private const int _headersMaxLength = 8192;

		private Version _version;

		internal byte[] EntityBodyData;

		protected const string CrLf = "\r\n";

		public string EntityBody
		{
			get
			{
				string str;
				if ((this.EntityBodyData == null ? false : (long)this.EntityBodyData.Length != (long)0))
				{
					Encoding encoding = null;
					string item = this._headers["Content-Type"];
					if ((item == null ? false : item.Length > 0))
					{
						encoding = HttpUtility.GetEncoding(item);
					}
					str = (encoding ?? Encoding.UTF8).GetString(this.EntityBodyData);
				}
				else
				{
					str = string.Empty;
				}
				return str;
			}
		}

		public NameValueCollection Headers
		{
			get
			{
				return this._headers;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return this._version;
			}
		}

		protected HttpBase(Version version, NameValueCollection headers)
		{
			this._version = version;
			this._headers = headers;
		}

		protected static T Read<T>(Stream stream, Func<string[], T> parser, int millisecondsTimeout)
		where T : HttpBase
		{
			string str;
			bool flag = false;
			Timer timer = new Timer((object state) => {
				flag = true;
				stream.Close();
			}, null, millisecondsTimeout, -1);
			T t = default(T);
			Exception exception = null;
			try
			{
				try
				{
					t = parser(HttpBase.readHeaders(stream, 8192));
					string item = t.Headers["Content-Length"];
					if ((item == null ? false : item.Length > 0))
					{
						t.EntityBodyData = HttpBase.readEntityBody(stream, item);
					}
				}
				catch (Exception exception1)
				{
					exception = exception1;
				}
			}
			finally
			{
				timer.Change(-1, -1);
				timer.Dispose();
			}
			if (flag)
			{
				str = "A timeout has occurred while reading an HTTP request/response.";
			}
			else if (exception != null)
			{
				str = "An exception has occurred while reading an HTTP request/response.";
			}
			else
			{
				str = null;
			}
			string str1 = str;
			if (str1 != null)
			{
				throw new WebSocketException(str1, exception);
			}
			return t;
		}

		private static byte[] readEntityBody(Stream stream, string length)
		{
			long num;
			byte[] numArray;
			if (!long.TryParse(length, out num))
			{
				throw new ArgumentException("Cannot be parsed.", "length");
			}
			if (num < (long)0)
			{
				throw new ArgumentOutOfRangeException("length", "Less than zero.");
			}
			if (num > (long)1024)
			{
				numArray = stream.ReadBytes(num, 1024);
			}
			else if (num > (long)0)
			{
				numArray = stream.ReadBytes((int)num);
			}
			else
			{
				numArray = null;
			}
			return numArray;
		}

		private static string[] readHeaders(Stream stream, int maxLength)
		{
			List<byte> nums = new List<byte>();
			int num = 0;
			Action<int> action = (int i) => {
				if (i == -1)
				{
					throw new EndOfStreamException("The header cannot be read from the data source.");
				}
				nums.Add((byte)i);
				num++;
			};
			bool flag = false;
			while (num < maxLength)
			{
				if ((!stream.ReadByte().EqualsWith('\r', action) || !stream.ReadByte().EqualsWith('\n', action) || !stream.ReadByte().EqualsWith('\r', action) ? false : stream.ReadByte().EqualsWith('\n', action)))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				throw new WebSocketException("The length of header part is greater than the max length.");
			}
			return Encoding.UTF8.GetString(nums.ToArray()).Replace("\r\n ", " ").Replace("\r\n\t", " ").Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		}

		public byte[] ToByteArray()
		{
			return Encoding.UTF8.GetBytes(this.ToString());
		}
	}
}