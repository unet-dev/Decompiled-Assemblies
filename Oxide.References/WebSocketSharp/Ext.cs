using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace WebSocketSharp
{
	public static class Ext
	{
		private readonly static byte[] _last;

		private readonly static int _retry;

		private const string _tspecials = "()<>@,;:\\\"/[]?={} \t";

		static Ext()
		{
			Ext._last = new byte[1];
			Ext._retry = 5;
		}

		internal static byte[] Append(this ushort code, string reason)
		{
			byte[] byteArray = code.InternalToByteArray(ByteOrder.Big);
			if ((reason == null ? false : reason.Length > 0))
			{
				List<byte> nums = new List<byte>(byteArray);
				nums.AddRange(Encoding.UTF8.GetBytes(reason));
				byteArray = nums.ToArray();
			}
			return byteArray;
		}

		internal static string CheckIfAvailable(this ServerState state, bool ready, bool start, bool shutting)
		{
			string str;
			if ((ready || state != ServerState.Ready && state != ServerState.Stop) && (start || state != ServerState.Start) && (shutting || state != ServerState.ShuttingDown))
			{
				str = null;
			}
			else
			{
				str = string.Concat("This operation isn't available in: ", state.ToString().ToLower());
			}
			return str;
		}

		internal static string CheckIfAvailable(this WebSocketState state, bool connecting, bool open, bool closing, bool closed)
		{
			string str;
			if ((connecting || state != WebSocketState.Connecting) && (open || state != WebSocketState.Open) && (closing || state != WebSocketState.Closing) && (closed || state != WebSocketState.Closed))
			{
				str = null;
			}
			else
			{
				str = string.Concat("This operation isn't available in: ", state.ToString().ToLower());
			}
			return str;
		}

		internal static string CheckIfValidProtocols(this string[] protocols)
		{
			string str;
			if (protocols.Contains<string>((string protocol) => (protocol == null || protocol.Length == 0 ? true : !protocol.IsToken())))
			{
				str = "Contains an invalid value.";
			}
			else if (protocols.ContainsTwice())
			{
				str = "Contains a value twice.";
			}
			else
			{
				str = null;
			}
			return str;
		}

		internal static string CheckIfValidServicePath(this string path)
		{
			string str;
			if (path == null || path.Length == 0)
			{
				str = "'path' is null or empty.";
			}
			else if (path[0] != '/')
			{
				str = "'path' isn't an absolute path.";
			}
			else if (path.IndexOfAny(new char[] { '?', '#' }) > -1)
			{
				str = "'path' includes either or both query and fragment components.";
			}
			else
			{
				str = null;
			}
			return str;
		}

		internal static string CheckIfValidSessionID(this string id)
		{
			string str;
			if (id == null || id.Length == 0)
			{
				str = "'id' is null or empty.";
			}
			else
			{
				str = null;
			}
			return str;
		}

		internal static string CheckIfValidWaitTime(this TimeSpan time)
		{
			string str;
			if (time <= TimeSpan.Zero)
			{
				str = "A wait time is zero or less.";
			}
			else
			{
				str = null;
			}
			return str;
		}

		internal static bool CheckWaitTime(this TimeSpan time, out string message)
		{
			bool flag;
			message = null;
			if (time > TimeSpan.Zero)
			{
				flag = true;
			}
			else
			{
				message = "A wait time is zero or less.";
				flag = false;
			}
			return flag;
		}

		internal static void Close(this WebSocketSharp.Net.HttpListenerResponse response, WebSocketSharp.Net.HttpStatusCode code)
		{
			response.StatusCode = (int)code;
			response.OutputStream.Close();
		}

		internal static void CloseWithAuthChallenge(this WebSocketSharp.Net.HttpListenerResponse response, string challenge)
		{
			response.Headers.InternalSet("WWW-Authenticate", challenge, true);
			response.Close(WebSocketSharp.Net.HttpStatusCode.Unauthorized);
		}

		private static byte[] compress(this byte[] data)
		{
			byte[] array;
			if ((long)data.Length != (long)0)
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					array = memoryStream.compressToArray();
				}
			}
			else
			{
				array = data;
			}
			return array;
		}

		private static MemoryStream compress(this Stream stream)
		{
			MemoryStream memoryStream;
			MemoryStream memoryStream1 = new MemoryStream();
			if (stream.Length != (long)0)
			{
				stream.Position = (long)0;
				using (DeflateStream deflateStream = new DeflateStream(memoryStream1, CompressionMode.Compress, true))
				{
					stream.CopyTo(deflateStream, 1024);
					deflateStream.Close();
					memoryStream1.Write(Ext._last, 0, 1);
					memoryStream1.Position = (long)0;
					memoryStream = memoryStream1;
				}
			}
			else
			{
				memoryStream = memoryStream1;
			}
			return memoryStream;
		}

		internal static byte[] Compress(this byte[] data, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate ? data.compress() : data);
		}

		internal static Stream Compress(this Stream stream, CompressionMethod method)
		{
			Stream stream1;
			if (method == CompressionMethod.Deflate)
			{
				stream1 = stream.compress();
			}
			else
			{
				stream1 = stream;
			}
			return stream1;
		}

		private static byte[] compressToArray(this Stream stream)
		{
			byte[] array;
			using (MemoryStream memoryStream = stream.compress())
			{
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		internal static byte[] CompressToArray(this Stream stream, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate ? stream.compressToArray() : stream.ToByteArray());
		}

		internal static bool Contains<T>(this IEnumerable<T> source, Func<T, bool> condition)
		{
			bool flag;
			foreach (T t in source)
			{
				if (!condition(t))
				{
					continue;
				}
				flag = true;
				return flag;
			}
			flag = false;
			return flag;
		}

		public static bool Contains(this string value, params char[] chars)
		{
			bool flag;
			if (chars == null || chars.Length == 0)
			{
				flag = true;
			}
			else
			{
				flag = (value == null || value.Length == 0 ? false : value.IndexOfAny(chars) > -1);
			}
			return flag;
		}

		public static bool Contains(this NameValueCollection collection, string name)
		{
			return (collection == null || collection.Count <= 0 ? false : collection[name] != null);
		}

		public static bool Contains(this NameValueCollection collection, string name, string value)
		{
			bool flag;
			if ((collection == null ? false : collection.Count != 0))
			{
				string item = collection[name];
				if (item != null)
				{
					string[] strArrays = item.Split(new char[] { ',' });
					int num = 0;
					while (num < (int)strArrays.Length)
					{
						if (!strArrays[num].Trim().Equals(value, StringComparison.OrdinalIgnoreCase))
						{
							num++;
						}
						else
						{
							flag = true;
							return flag;
						}
					}
					flag = false;
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		internal static bool ContainsTwice(this string[] values)
		{
			int length = (int)values.Length;
			Func<int, bool> func = null;
			func = (int idx) => {
				bool flag;
				if (idx >= this.len - 1)
				{
					flag = false;
				}
				else
				{
					int num = idx + 1;
					while (num < this.len)
					{
						if (this.values[num] != this.values[idx])
						{
							num++;
						}
						else
						{
							flag = true;
							return flag;
						}
					}
					int num1 = idx + 1;
					idx = num1;
					flag = this.contains(num1);
				}
				return flag;
			};
			return func(0);
		}

		internal static T[] Copy<T>(this T[] source, long length)
		{
			T[] tArray = new T[checked((IntPtr)length)];
			Array.Copy(source, (long)0, tArray, (long)0, length);
			return tArray;
		}

		internal static void CopyTo(this Stream source, Stream destination, int bufferLength)
		{
			byte[] numArray = new byte[bufferLength];
			int num = 0;
			while (true)
			{
				int num1 = source.Read(numArray, 0, bufferLength);
				num = num1;
				if (num1 <= 0)
				{
					break;
				}
				destination.Write(numArray, 0, num);
			}
		}

		internal static void CopyToAsync(this Stream source, Stream destination, int bufferLength, Action completed, Action<Exception> error)
		{
			byte[] numArray = new byte[bufferLength];
			AsyncCallback asyncCallback = null;
			asyncCallback = (IAsyncResult ar) => {
				try
				{
					int num = this.source.EndRead(ar);
					if (num > 0)
					{
						this.destination.Write(this.buff, 0, num);
						this.source.BeginRead(this.buff, 0, this.bufferLength, this.callback, null);
					}
					else if (this.completed != null)
					{
						this.completed();
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (this.error != null)
					{
						this.error(exception);
					}
				}
			};
			try
			{
				source.BeginRead(numArray, 0, bufferLength, asyncCallback, null);
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				if (error != null)
				{
					error(exception2);
				}
			}
		}

		private static byte[] decompress(this byte[] data)
		{
			byte[] array;
			if ((long)data.Length != (long)0)
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					array = memoryStream.decompressToArray();
				}
			}
			else
			{
				array = data;
			}
			return array;
		}

		private static MemoryStream decompress(this Stream stream)
		{
			MemoryStream memoryStream;
			MemoryStream memoryStream1 = new MemoryStream();
			if (stream.Length != (long)0)
			{
				stream.Position = (long)0;
				using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true))
				{
					deflateStream.CopyTo(memoryStream1, 1024);
					memoryStream1.Position = (long)0;
					memoryStream = memoryStream1;
				}
			}
			else
			{
				memoryStream = memoryStream1;
			}
			return memoryStream;
		}

		internal static byte[] Decompress(this byte[] data, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate ? data.decompress() : data);
		}

		internal static Stream Decompress(this Stream stream, CompressionMethod method)
		{
			Stream stream1;
			if (method == CompressionMethod.Deflate)
			{
				stream1 = stream.decompress();
			}
			else
			{
				stream1 = stream;
			}
			return stream1;
		}

		private static byte[] decompressToArray(this Stream stream)
		{
			byte[] array;
			using (MemoryStream memoryStream = stream.decompress())
			{
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		internal static byte[] DecompressToArray(this Stream stream, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate ? stream.decompressToArray() : stream.ToByteArray());
		}

		public static void Emit(this EventHandler eventHandler, object sender, EventArgs e)
		{
			if (eventHandler != null)
			{
				eventHandler(sender, e);
			}
		}

		public static void Emit<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e)
		where TEventArgs : EventArgs
		{
			if (eventHandler != null)
			{
				eventHandler(sender, e);
			}
		}

		internal static bool EqualsWith(this int value, char c, Action<int> action)
		{
			action(value);
			return value == c;
		}

		internal static string GetAbsolutePath(this Uri uri)
		{
			string absolutePath;
			if (!uri.IsAbsoluteUri)
			{
				string originalString = uri.OriginalString;
				if (originalString[0] == '/')
				{
					int num = originalString.IndexOfAny(new char[] { '?', '#' });
					absolutePath = (num > 0 ? originalString.Substring(0, num) : originalString);
				}
				else
				{
					absolutePath = null;
				}
			}
			else
			{
				absolutePath = uri.AbsolutePath;
			}
			return absolutePath;
		}

		public static WebSocketSharp.Net.CookieCollection GetCookies(this NameValueCollection headers, bool response)
		{
			string str = (response ? "Set-Cookie" : "Cookie");
			return (headers == null || !headers.Contains(str) ? new WebSocketSharp.Net.CookieCollection() : WebSocketSharp.Net.CookieCollection.Parse(headers[str], response));
		}

		public static string GetDescription(this WebSocketSharp.Net.HttpStatusCode code)
		{
			return ((int)code).GetStatusDescription();
		}

		internal static string GetMessage(this CloseStatusCode code)
		{
			string str;
			if (code == CloseStatusCode.ProtocolError)
			{
				str = "A WebSocket protocol error has occurred.";
			}
			else if (code == CloseStatusCode.UnsupportedData)
			{
				str = "Unsupported data has been received.";
			}
			else if (code == CloseStatusCode.Abnormal)
			{
				str = "An exception has occurred.";
			}
			else if (code == CloseStatusCode.InvalidData)
			{
				str = "Invalid data has been received.";
			}
			else if (code == CloseStatusCode.PolicyViolation)
			{
				str = "A policy violation has occurred.";
			}
			else if (code == CloseStatusCode.TooBig)
			{
				str = "A too big message has been received.";
			}
			else if (code == CloseStatusCode.MandatoryExtension)
			{
				str = "WebSocket client didn't receive expected extension(s).";
			}
			else if (code == CloseStatusCode.ServerError)
			{
				str = "WebSocket server got an internal error.";
			}
			else
			{
				str = (code == CloseStatusCode.TlsHandshakeFailure ? "An error has occurred during a TLS handshake." : string.Empty);
			}
			return str;
		}

		internal static string GetName(this string nameAndValue, char separator)
		{
			string str;
			int num = nameAndValue.IndexOf(separator);
			if (num > 0)
			{
				str = nameAndValue.Substring(0, num).Trim();
			}
			else
			{
				str = null;
			}
			return str;
		}

		public static string GetStatusDescription(this int code)
		{
			string empty;
			int num = code;
			if (num > 207)
			{
				switch (num)
				{
					case 300:
					{
						empty = "Multiple Choices";
						break;
					}
					case 301:
					{
						empty = "Moved Permanently";
						break;
					}
					case 302:
					{
						empty = "Found";
						break;
					}
					case 303:
					{
						empty = "See Other";
						break;
					}
					case 304:
					{
						empty = "Not Modified";
						break;
					}
					case 305:
					{
						empty = "Use Proxy";
						break;
					}
					case 306:
					{
						empty = string.Empty;
						return empty;
					}
					case 307:
					{
						empty = "Temporary Redirect";
						break;
					}
					default:
					{
						switch (num)
						{
							case 400:
							{
								empty = "Bad Request";
								break;
							}
							case 401:
							{
								empty = "Unauthorized";
								break;
							}
							case 402:
							{
								empty = "Payment Required";
								break;
							}
							case 403:
							{
								empty = "Forbidden";
								break;
							}
							case 404:
							{
								empty = "Not Found";
								break;
							}
							case 405:
							{
								empty = "Method Not Allowed";
								break;
							}
							case 406:
							{
								empty = "Not Acceptable";
								break;
							}
							case 407:
							{
								empty = "Proxy Authentication Required";
								break;
							}
							case 408:
							{
								empty = "Request Timeout";
								break;
							}
							case 409:
							{
								empty = "Conflict";
								break;
							}
							case 410:
							{
								empty = "Gone";
								break;
							}
							case 411:
							{
								empty = "Length Required";
								break;
							}
							case 412:
							{
								empty = "Precondition Failed";
								break;
							}
							case 413:
							{
								empty = "Request Entity Too Large";
								break;
							}
							case 414:
							{
								empty = "Request-Uri Too Long";
								break;
							}
							case 415:
							{
								empty = "Unsupported Media Type";
								break;
							}
							case 416:
							{
								empty = "Requested Range Not Satisfiable";
								break;
							}
							case 417:
							{
								empty = "Expectation Failed";
								break;
							}
							case 418:
							case 419:
							case 420:
							case 421:
							{
								empty = string.Empty;
								return empty;
							}
							case 422:
							{
								empty = "Unprocessable Entity";
								break;
							}
							case 423:
							{
								empty = "Locked";
								break;
							}
							case 424:
							{
								empty = "Failed Dependency";
								break;
							}
							default:
							{
								switch (num)
								{
									case 500:
									{
										empty = "Internal Server Error";
										break;
									}
									case 501:
									{
										empty = "Not Implemented";
										break;
									}
									case 502:
									{
										empty = "Bad Gateway";
										break;
									}
									case 503:
									{
										empty = "Service Unavailable";
										break;
									}
									case 504:
									{
										empty = "Gateway Timeout";
										break;
									}
									case 505:
									{
										empty = "Http Version Not Supported";
										break;
									}
									case 506:
									{
										empty = string.Empty;
										return empty;
									}
									case 507:
									{
										empty = "Insufficient Storage";
										break;
									}
									default:
									{
										empty = string.Empty;
										return empty;
									}
								}
								break;
							}
						}
						break;
					}
				}
			}
			else
			{
				switch (num)
				{
					case 100:
					{
						empty = "Continue";
						break;
					}
					case 101:
					{
						empty = "Switching Protocols";
						break;
					}
					case 102:
					{
						empty = "Processing";
						break;
					}
					default:
					{
						switch (num)
						{
							case 200:
							{
								empty = "OK";
								break;
							}
							case 201:
							{
								empty = "Created";
								break;
							}
							case 202:
							{
								empty = "Accepted";
								break;
							}
							case 203:
							{
								empty = "Non-Authoritative Information";
								break;
							}
							case 204:
							{
								empty = "No Content";
								break;
							}
							case 205:
							{
								empty = "Reset Content";
								break;
							}
							case 206:
							{
								empty = "Partial Content";
								break;
							}
							case 207:
							{
								empty = "Multi-Status";
								break;
							}
							default:
							{
								empty = string.Empty;
								return empty;
							}
						}
						break;
					}
				}
			}
			return empty;
		}

		internal static string GetValue(this string nameAndValue, char separator)
		{
			string str;
			int num = nameAndValue.IndexOf(separator);
			if (num <= -1 || num >= nameAndValue.Length - 1)
			{
				str = null;
			}
			else
			{
				str = nameAndValue.Substring(num + 1).Trim();
			}
			return str;
		}

		internal static string GetValue(this string nameAndValue, char separator, bool unquote)
		{
			string str;
			int num = nameAndValue.IndexOf(separator);
			if ((num < 0 ? false : num != nameAndValue.Length - 1))
			{
				string str1 = nameAndValue.Substring(num + 1).Trim();
				str = (unquote ? str1.Unquote() : str1);
			}
			else
			{
				str = null;
			}
			return str;
		}

		internal static TcpListenerWebSocketContext GetWebSocketContext(this TcpClient tcpClient, string protocol, bool secure, ServerSslConfiguration sslConfig, Logger logger)
		{
			return new TcpListenerWebSocketContext(tcpClient, protocol, secure, sslConfig, logger);
		}

		internal static byte[] InternalToByteArray(this ushort value, ByteOrder order)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (!order.IsHostOrder())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		internal static byte[] InternalToByteArray(this ulong value, ByteOrder order)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (!order.IsHostOrder())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		public static bool IsCloseStatusCode(this ushort value)
		{
			return (value <= 999 ? false : value < 5000);
		}

		internal static bool IsCompressionExtension(this string value, CompressionMethod method)
		{
			bool flag = value.StartsWith(method.ToExtensionString(new string[0]));
			return flag;
		}

		internal static bool IsControl(this byte opcode)
		{
			return (opcode <= 7 ? false : opcode < 16);
		}

		internal static bool IsControl(this Opcode opcode)
		{
			return opcode >= Opcode.Close;
		}

		internal static bool IsData(this byte opcode)
		{
			return (opcode == 1 ? true : opcode == 2);
		}

		internal static bool IsData(this Opcode opcode)
		{
			return (opcode == Opcode.Text ? true : opcode == Opcode.Binary);
		}

		public static bool IsEnclosedIn(this string value, char c)
		{
			return (value == null || value.Length <= 1 || value[0] != c ? false : value[value.Length - 1] == c);
		}

		public static bool IsHostOrder(this ByteOrder order)
		{
			return BitConverter.IsLittleEndian == order == ByteOrder.Little;
		}

		public static bool IsLocal(this IPAddress address)
		{
			bool flag;
			if (address == null)
			{
				flag = false;
			}
			else if (address.Equals(IPAddress.Any))
			{
				flag = true;
			}
			else if (!address.Equals(IPAddress.Loopback))
			{
				if (Socket.OSSupportsIPv6)
				{
					if (address.Equals(IPAddress.IPv6Any))
					{
						flag = true;
						return flag;
					}
					else if (address.Equals(IPAddress.IPv6Loopback))
					{
						flag = true;
						return flag;
					}
				}
				IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
				int num = 0;
				while (num < (int)hostAddresses.Length)
				{
					if (!address.Equals(hostAddresses[num]))
					{
						num++;
					}
					else
					{
						flag = true;
						return flag;
					}
				}
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public static bool IsNullOrEmpty(this string value)
		{
			return (value == null ? true : value.Length == 0);
		}

		internal static bool IsPortNumber(this int value)
		{
			return (value <= 0 ? false : value < 65536);
		}

		public static bool IsPredefinedScheme(this string value)
		{
			bool flag;
			bool flag1;
			bool flag2;
			if ((value == null ? false : value.Length >= 2))
			{
				char chr = value[0];
				if (chr == 'h')
				{
					flag = (value == "http" ? true : value == "https");
				}
				else if (chr == 'w')
				{
					flag = (value == "ws" ? true : value == "wss");
				}
				else if (chr == 'f')
				{
					flag = (value == "file" ? true : value == "ftp");
				}
				else if (chr != 'n')
				{
					if (chr != 'g' || !(value == "gopher"))
					{
						flag1 = (chr != 'm' ? false : value == "mailto");
					}
					else
					{
						flag1 = true;
					}
					flag = flag1;
				}
				else
				{
					chr = value[1];
					if (chr == 'e')
					{
						flag2 = (value == "news" || value == "net.pipe" ? true : value == "net.tcp");
					}
					else
					{
						flag2 = value == "nntp";
					}
					flag = flag2;
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		internal static bool IsReserved(this ushort code)
		{
			return (code == 1004 || code == 1005 || code == 1006 ? true : code == 1015);
		}

		internal static bool IsReserved(this CloseStatusCode code)
		{
			return (code == CloseStatusCode.Undefined || code == CloseStatusCode.NoStatus || code == CloseStatusCode.Abnormal ? true : code == CloseStatusCode.TlsHandshakeFailure);
		}

		internal static bool IsSupported(this byte opcode)
		{
			return Enum.IsDefined(typeof(Opcode), opcode);
		}

		internal static bool IsText(this string value)
		{
			bool flag;
			bool flag1;
			int length = value.Length;
			int num = 0;
			while (true)
			{
				if (num < length)
				{
					char chr = value[num];
					if ((chr >= ' ' ? false : !"\r\n\t".Contains(new char[] { chr })))
					{
						flag = false;
						break;
					}
					else if (chr != '\u007F')
					{
						if (chr != '\n')
						{
							flag1 = false;
						}
						else
						{
							int num1 = num + 1;
							num = num1;
							flag1 = num1 < length;
						}
						if (flag1)
						{
							chr = value[num];
							if (!" \t".Contains(new char[] { chr }))
							{
								flag = false;
								break;
							}
						}
						num++;
					}
					else
					{
						flag = false;
						break;
					}
				}
				else
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		internal static bool IsToken(this string value)
		{
			bool flag;
			string str = value;
			int num = 0;
			while (true)
			{
				if (num < str.Length)
				{
					char chr = str[num];
					if ((chr < ' ' || chr >= '\u007F' ? false : !"()<>@,;:\\\"/[]?={} \t".Contains(new char[] { chr })))
					{
						num++;
					}
					else
					{
						flag = false;
						break;
					}
				}
				else
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		public static bool IsUpgradeTo(this WebSocketSharp.Net.HttpListenerRequest request, string protocol)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (protocol == null)
			{
				throw new ArgumentNullException("protocol");
			}
			if (protocol.Length == 0)
			{
				throw new ArgumentException("An empty string.", "protocol");
			}
			return (!request.Headers.Contains("Upgrade", protocol) ? false : request.Headers.Contains("Connection", "Upgrade"));
		}

		public static bool MaybeUri(this string value)
		{
			bool flag;
			if ((value == null ? false : value.Length != 0))
			{
				int num = value.IndexOf(':');
				if (num != -1)
				{
					flag = (num < 10 ? value.Substring(0, num).IsPredefinedScheme() : false);
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		internal static string Quote(this string value)
		{
			string str = string.Format("\"{0}\"", value.Replace("\"", "\\\""));
			return str;
		}

		internal static byte[] ReadBytes(this Stream stream, int length)
		{
			byte[] numArray = new byte[length];
			int num = 0;
			try
			{
				int num1 = 0;
				while (length > 0)
				{
					num1 = stream.Read(numArray, num, length);
					if (num1 != 0)
					{
						num += num1;
						length -= num1;
					}
					else
					{
						break;
					}
				}
			}
			catch
			{
			}
			return numArray.SubArray<byte>(0, num);
		}

		internal static byte[] ReadBytes(this Stream stream, long length, int bufferLength)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				try
				{
					byte[] numArray = new byte[bufferLength];
					int num = 0;
					while (length > (long)0)
					{
						if (length < (long)bufferLength)
						{
							bufferLength = (int)length;
						}
						num = stream.Read(numArray, 0, bufferLength);
						if (num != 0)
						{
							memoryStream.Write(numArray, 0, num);
							length -= (long)num;
						}
						else
						{
							break;
						}
					}
				}
				catch
				{
				}
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		internal static void ReadBytesAsync(this Stream stream, int length, Action<byte[]> completed, Action<Exception> error)
		{
			byte[] numArray = new byte[length];
			int num1 = 0;
			AsyncCallback asyncCallback = null;
			asyncCallback = (IAsyncResult ar) => {
				try
				{
					int num = this.stream.EndRead(ar);
					if ((num != 0 ? false : this.retry < Ext._retry))
					{
						this.retry++;
						this.stream.BeginRead(this.buff, this.offset, this.length, this.callback, null);
					}
					else if ((num == 0 ? false : num != this.length))
					{
						this.retry = 0;
						this.offset += num;
						this.length -= num;
						this.stream.BeginRead(this.buff, this.offset, this.length, this.callback, null);
					}
					else if (this.completed != null)
					{
						this.completed(this.buff.SubArray<byte>(0, this.offset + num));
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (this.error != null)
					{
						this.error(exception);
					}
				}
			};
			try
			{
				stream.BeginRead(numArray, num1, length, asyncCallback, null);
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				if (error != null)
				{
					error(exception2);
				}
			}
		}

		internal static void ReadBytesAsync(this Stream stream, long length, int bufferLength, Action<byte[]> completed, Action<Exception> error)
		{
			MemoryStream memoryStream = new MemoryStream();
			byte[] numArray = new byte[bufferLength];
			Action<long> action = null;
			action = (long len) => {
				if (len < (long)this.bufferLength)
				{
					this.bufferLength = (int)len;
				}
				this.stream.BeginRead(this.buff, 0, this.bufferLength, (IAsyncResult ar) => {
					try
					{
						int num = this.stream.EndRead(ar);
						if (num > 0)
						{
							this.dest.Write(this.buff, 0, num);
						}
						if ((num != 0 ? false : this.retry < Ext._retry))
						{
							int cSu0024u003cu003e8_locals1 = this.retry;
							this.retry = cSu0024u003cu003e8_locals1 + 1;
							this.read(len);
						}
						else if ((num == 0 ? false : (long)num != len))
						{
							this.retry = 0;
							this.read(len - (long)num);
						}
						else
						{
							if (this.completed != null)
							{
								this.dest.Close();
								this.completed(this.dest.ToArray());
							}
							this.dest.Dispose();
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this.dest.Dispose();
						if (this.error != null)
						{
							this.error(exception);
						}
					}
				}, null);
			};
			try
			{
				action(length);
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				memoryStream.Dispose();
				if (error != null)
				{
					error(exception2);
				}
			}
		}

		internal static string RemovePrefix(this string value, params string[] prefixes)
		{
			int length = 0;
			string[] strArrays = prefixes;
			int num = 0;
			while (num < (int)strArrays.Length)
			{
				string str = strArrays[num];
				if (!value.StartsWith(str))
				{
					num++;
				}
				else
				{
					length = str.Length;
					break;
				}
			}
			return (length > 0 ? value.Substring(length) : value);
		}

		internal static T[] Reverse<T>(this T[] array)
		{
			int length = (int)array.Length;
			T[] tArray = new T[length];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				tArray[i] = array[num - i];
			}
			return tArray;
		}

		internal static IEnumerable<string> SplitHeaderValue(this string value, params char[] separators)
		{
			bool flag;
			int length = value.Length;
			string str = new string(separators);
			StringBuilder stringBuilder = new StringBuilder(32);
			bool flag1 = false;
			bool flag2 = false;
			for (int i = 0; i < length; i++)
			{
				char chr = value[i];
				if (chr == '\"')
				{
					if (!flag1)
					{
						flag2 = !flag2;
					}
					else
					{
						flag1 = !flag1;
					}
				}
				else if (chr == '\\')
				{
					flag = (i >= length - 1 ? false : value[i + 1] == '\"');
					if (flag)
					{
						flag1 = true;
					}
				}
				else if (str.Contains(new char[] { chr }))
				{
					if (!flag2)
					{
						goto Label1;
					}
				}
				stringBuilder.Append(chr);
			Label0:
			}
			if (stringBuilder.Length > 0)
			{
				yield return stringBuilder.ToString();
			}
			yield break;
		Label1:
			yield return stringBuilder.ToString();
			stringBuilder.Length = 0;
			goto Label0;
		}

		public static T[] SubArray<T>(this T[] array, int startIndex, int length)
		{
			int num = 0;
			T[] tArray;
			bool flag;
			if (array == null)
			{
				flag = true;
			}
			else
			{
				int num1 = (int)array.Length;
				num = num1;
				flag = num1 == 0;
			}
			if (flag)
			{
				tArray = new T[0];
			}
			else if ((startIndex < 0 || length <= 0 ? true : startIndex + length > num))
			{
				tArray = new T[0];
			}
			else if ((startIndex != 0 ? true : length != num))
			{
				T[] tArray1 = new T[length];
				Array.Copy(array, startIndex, tArray1, 0, length);
				tArray = tArray1;
			}
			else
			{
				tArray = array;
			}
			return tArray;
		}

		public static T[] SubArray<T>(this T[] array, long startIndex, long length)
		{
			long num = 0L;
			T[] tArray;
			bool flag;
			if (array == null)
			{
				flag = true;
			}
			else
			{
				long num1 = (long)array.Length;
				num = num1;
				flag = num1 == (long)0;
			}
			if (flag)
			{
				tArray = new T[0];
			}
			else if ((startIndex < (long)0 || length <= (long)0 ? true : startIndex + length > num))
			{
				tArray = new T[0];
			}
			else if ((startIndex != 0 ? true : length != num))
			{
				T[] tArray1 = new T[checked((IntPtr)length)];
				Array.Copy(array, startIndex, tArray1, (long)0, length);
				tArray = tArray1;
			}
			else
			{
				tArray = array;
			}
			return tArray;
		}

		private static void times(this ulong n, Action action)
		{
			for (ulong i = (ulong)0; i < n; i += (long)1)
			{
				action();
			}
		}

		public static void Times(this int n, Action action)
		{
			if ((n <= 0 ? false : action != null))
			{
				((ulong)n).times(action);
			}
		}

		public static void Times(this long n, Action action)
		{
			if ((n <= (long)0 ? false : action != null))
			{
				((ulong)n).times(action);
			}
		}

		public static void Times(this uint n, Action action)
		{
			if ((n <= 0 ? false : action != null))
			{
				((ulong)n).times(action);
			}
		}

		public static void Times(this ulong n, Action action)
		{
			if ((n <= (long)0 ? false : action != null))
			{
				n.times(action);
			}
		}

		public static void Times(this int n, Action<int> action)
		{
			if ((n <= 0 ? false : action != null))
			{
				for (int i = 0; i < n; i++)
				{
					action(i);
				}
			}
		}

		public static void Times(this long n, Action<long> action)
		{
			if ((n <= (long)0 ? false : action != null))
			{
				for (long i = (long)0; i < n; i += (long)1)
				{
					action(i);
				}
			}
		}

		public static void Times(this uint n, Action<uint> action)
		{
			if ((n <= 0 ? false : action != null))
			{
				for (uint i = 0; i < n; i++)
				{
					action(i);
				}
			}
		}

		public static void Times(this ulong n, Action<ulong> action)
		{
			if ((n <= (long)0 ? false : action != null))
			{
				for (ulong i = (ulong)0; i < n; i += (long)1)
				{
					action(i);
				}
			}
		}

		public static T To<T>(this byte[] source, ByteOrder sourceOrder)
		where T : struct
		{
			T t;
			T t1;
			T flag;
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source.Length != 0)
			{
				Type type = typeof(T);
				byte[] hostOrder = source.ToHostOrder(sourceOrder);
				if (type == typeof(bool))
				{
					flag = (T)(object)BitConverter.ToBoolean(hostOrder, 0);
				}
				else if (type == typeof(char))
				{
					flag = (T)(object)BitConverter.ToChar(hostOrder, 0);
				}
				else if (type == typeof(double))
				{
					flag = (T)(object)BitConverter.ToDouble(hostOrder, 0);
				}
				else if (type == typeof(short))
				{
					flag = (T)(object)BitConverter.ToInt16(hostOrder, 0);
				}
				else if (type == typeof(int))
				{
					flag = (T)(object)BitConverter.ToInt32(hostOrder, 0);
				}
				else if (type == typeof(long))
				{
					flag = (T)(object)BitConverter.ToInt64(hostOrder, 0);
				}
				else if (type == typeof(float))
				{
					flag = (T)(object)BitConverter.ToSingle(hostOrder, 0);
				}
				else if (type == typeof(ushort))
				{
					flag = (T)(object)BitConverter.ToUInt16(hostOrder, 0);
				}
				else if (type == typeof(uint))
				{
					flag = (T)(object)BitConverter.ToUInt32(hostOrder, 0);
				}
				else if (type == typeof(ulong))
				{
					flag = (T)(object)BitConverter.ToUInt64(hostOrder, 0);
				}
				else
				{
					t = default(T);
					flag = t;
				}
				t1 = flag;
			}
			else
			{
				t = default(T);
				t1 = t;
			}
			return t1;
		}

		internal static byte[] ToByteArray(this Stream stream)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				stream.Position = (long)0;
				stream.CopyTo(memoryStream, 1024);
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		public static byte[] ToByteArray<T>(this T value, ByteOrder order)
		where T : struct
		{
			byte[] bytes;
			Type type = typeof(T);
			if (type == typeof(bool))
			{
				bytes = BitConverter.GetBytes((bool)(object)value);
			}
			else if (type == typeof(byte))
			{
				bytes = new byte[] { (byte)(object)value };
			}
			else if (type == typeof(char))
			{
				bytes = BitConverter.GetBytes((char)(object)value);
			}
			else if (type == typeof(double))
			{
				bytes = BitConverter.GetBytes((double)(object)value);
			}
			else if (type == typeof(short))
			{
				bytes = BitConverter.GetBytes((short)(object)value);
			}
			else if (type == typeof(int))
			{
				bytes = BitConverter.GetBytes((int)(object)value);
			}
			else if (type == typeof(long))
			{
				bytes = BitConverter.GetBytes((long)(object)value);
			}
			else if (type == typeof(float))
			{
				bytes = BitConverter.GetBytes((float)(object)value);
			}
			else if (type == typeof(ushort))
			{
				bytes = BitConverter.GetBytes((ushort)(object)value);
			}
			else if (type == typeof(uint))
			{
				bytes = BitConverter.GetBytes((uint)(object)value);
			}
			else
			{
				bytes = (type == typeof(ulong) ? BitConverter.GetBytes((ulong)(object)value) : WebSocket.EmptyBytes);
			}
			byte[] numArray = bytes;
			if (((int)numArray.Length <= 1 ? false : !order.IsHostOrder()))
			{
				Array.Reverse(numArray);
			}
			return numArray;
		}

		internal static CompressionMethod ToCompressionMethod(this string value)
		{
			CompressionMethod compressionMethod;
			foreach (CompressionMethod compressionMethod1 in Enum.GetValues(typeof(CompressionMethod)))
			{
				if (compressionMethod1.ToExtensionString(new string[0]) != value)
				{
					continue;
				}
				compressionMethod = compressionMethod1;
				return compressionMethod;
			}
			compressionMethod = CompressionMethod.None;
			return compressionMethod;
		}

		internal static string ToExtensionString(this CompressionMethod method, params string[] parameters)
		{
			string empty;
			if (method != CompressionMethod.None)
			{
				string str = string.Format("permessage-{0}", method.ToString().ToLower());
				empty = ((parameters == null ? false : parameters.Length != 0) ? string.Format("{0}; {1}", str, parameters.ToString<string>("; ")) : str);
			}
			else
			{
				empty = string.Empty;
			}
			return empty;
		}

		public static byte[] ToHostOrder(this byte[] source, ByteOrder sourceOrder)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return ((int)source.Length <= 1 || sourceOrder.IsHostOrder() ? source : source.Reverse<byte>());
		}

		internal static IPAddress ToIPAddress(this string hostnameOrAddress)
		{
			IPAddress pAddress;
			IPAddress hostAddresses;
			if (!IPAddress.TryParse(hostnameOrAddress, out pAddress))
			{
				try
				{
					hostAddresses = Dns.GetHostAddresses(hostnameOrAddress)[0];
				}
				catch
				{
					hostAddresses = null;
				}
			}
			else
			{
				hostAddresses = pAddress;
			}
			return hostAddresses;
		}

		internal static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			return new List<TSource>(source);
		}

		public static string ToString<T>(this T[] array, string separator)
		{
			string str;
			string empty = separator;
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			int length = (int)array.Length;
			if (length != 0)
			{
				if (empty == null)
				{
					empty = string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder(64);
				(length - 1).Times((int i) => stringBuilder.AppendFormat("{0}{1}", array[i].ToString(), empty));
				stringBuilder.Append(array[length - 1].ToString());
				str = stringBuilder.ToString();
			}
			else
			{
				str = string.Empty;
			}
			return str;
		}

		internal static ushort ToUInt16(this byte[] source, ByteOrder sourceOrder)
		{
			ushort num = BitConverter.ToUInt16(source.ToHostOrder(sourceOrder), 0);
			return num;
		}

		internal static ulong ToUInt64(this byte[] source, ByteOrder sourceOrder)
		{
			ulong num = BitConverter.ToUInt64(source.ToHostOrder(sourceOrder), 0);
			return num;
		}

		public static Uri ToUri(this string uriString)
		{
			Uri uri;
			Uri.TryCreate(uriString, (uriString.MaybeUri() ? UriKind.Absolute : UriKind.Relative), out uri);
			return uri;
		}

		internal static string TrimEndSlash(this string value)
		{
			value = value.TrimEnd(new char[] { '/' });
			return (value.Length > 0 ? value : "/");
		}

		internal static bool TryCreateWebSocketUri(this string uriString, out Uri result, out string message)
		{
			bool flag;
			Uri uri;
			result = null;
			Uri uri1 = uriString.ToUri();
			if (uri1 == null)
			{
				message = string.Concat("An invalid URI string: ", uriString);
				flag = false;
			}
			else if (uri1.IsAbsoluteUri)
			{
				string scheme = uri1.Scheme;
				if ((scheme == "ws" ? false : scheme != "wss"))
				{
					message = string.Concat("The scheme part isn't 'ws' or 'wss': ", uriString);
					flag = false;
				}
				else if (uri1.Fragment.Length <= 0)
				{
					int port = uri1.Port;
					if (port != 0)
					{
						if (port != -1)
						{
							uri = uri1;
						}
						else
						{
							object[] host = new object[] { scheme, uri1.Host, null, null };
							host[2] = (scheme == "ws" ? 80 : 443);
							host[3] = uri1.PathAndQuery;
							uri = new Uri(string.Format("{0}://{1}:{2}{3}", host));
						}
						result = uri;
						message = string.Empty;
						flag = true;
					}
					else
					{
						message = string.Concat("The port part is zero: ", uriString);
						flag = false;
					}
				}
				else
				{
					message = string.Concat("Includes the fragment component: ", uriString);
					flag = false;
				}
			}
			else
			{
				message = string.Concat("Not an absolute URI: ", uriString);
				flag = false;
			}
			return flag;
		}

		internal static string Unquote(this string value)
		{
			string str;
			string str1;
			int num = value.IndexOf('\"');
			if (num >= 0)
			{
				int num1 = value.LastIndexOf('\"') - num - 1;
				if (num1 < 0)
				{
					str1 = value;
				}
				else
				{
					str1 = (num1 == 0 ? string.Empty : value.Substring(num + 1, num1).Replace("\\\"", "\""));
				}
				str = str1;
			}
			else
			{
				str = value;
			}
			return str;
		}

		public static string UrlDecode(this string value)
		{
			return (value == null || value.Length <= 0 ? value : HttpUtility.UrlDecode(value));
		}

		public static string UrlEncode(this string value)
		{
			return (value == null || value.Length <= 0 ? value : HttpUtility.UrlEncode(value));
		}

		internal static string UTF8Decode(this byte[] bytes)
		{
			string str;
			try
			{
				str = Encoding.UTF8.GetString(bytes);
			}
			catch
			{
				str = null;
			}
			return str;
		}

		internal static byte[] UTF8Encode(this string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		internal static void WriteBytes(this Stream stream, byte[] bytes, int bufferLength)
		{
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				memoryStream.CopyTo(stream, bufferLength);
			}
		}

		internal static void WriteBytesAsync(this Stream stream, byte[] bytes, int bufferLength, Action completed, Action<Exception> error)
		{
			MemoryStream memoryStream = new MemoryStream(bytes);
			memoryStream.CopyToAsync(stream, bufferLength, () => {
				if (completed != null)
				{
					completed();
				}
				memoryStream.Dispose();
			}, (Exception ex) => {
				memoryStream.Dispose();
				if (error != null)
				{
					error(ex);
				}
			});
		}

		public static void WriteContent(this WebSocketSharp.Net.HttpListenerResponse response, byte[] content)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			long length = (long)content.Length;
			if (length != (long)0)
			{
				response.ContentLength64 = length;
				Stream outputStream = response.OutputStream;
				if (length > (long)2147483647)
				{
					outputStream.WriteBytes(content, 1024);
				}
				else
				{
					outputStream.Write(content, 0, (int)length);
				}
				outputStream.Close();
			}
			else
			{
				response.Close();
			}
		}
	}
}