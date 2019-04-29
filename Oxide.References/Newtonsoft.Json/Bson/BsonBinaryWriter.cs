using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Newtonsoft.Json.Bson
{
	[Preserve]
	internal class BsonBinaryWriter
	{
		private readonly static System.Text.Encoding Encoding;

		private readonly BinaryWriter _writer;

		private byte[] _largeByteBuffer;

		public DateTimeKind DateTimeKindHandling
		{
			get;
			set;
		}

		static BsonBinaryWriter()
		{
			BsonBinaryWriter.Encoding = new UTF8Encoding(false);
		}

		public BsonBinaryWriter(BinaryWriter writer)
		{
			this.DateTimeKindHandling = DateTimeKind.Utc;
			this._writer = writer;
		}

		private int CalculateSize(int stringByteCount)
		{
			return stringByteCount + 1;
		}

		private int CalculateSize(BsonToken t)
		{
			switch (t.Type)
			{
				case BsonType.Number:
				{
					return 8;
				}
				case BsonType.String:
				{
					BsonString bsonString = (BsonString)t;
					string value = (string)bsonString.Value;
					bsonString.ByteCount = (value != null ? BsonBinaryWriter.Encoding.GetByteCount(value) : 0);
					bsonString.CalculatedSize = this.CalculateSizeWithLength(bsonString.ByteCount, bsonString.IncludeLength);
					return bsonString.CalculatedSize;
				}
				case BsonType.Object:
				{
					BsonObject bsonObjects = (BsonObject)t;
					int num = 4;
					foreach (BsonProperty bsonProperty in bsonObjects)
					{
						int num1 = 1 + this.CalculateSize(bsonProperty.Name);
						num1 += this.CalculateSize(bsonProperty.Value);
						num += num1;
					}
					num++;
					bsonObjects.CalculatedSize = num;
					return num;
				}
				case BsonType.Array:
				{
					BsonArray bsonArrays = (BsonArray)t;
					int num2 = 4;
					ulong num3 = (ulong)0;
					foreach (BsonToken bsonToken in bsonArrays)
					{
						num2++;
						num2 += this.CalculateSize(MathUtils.IntLength(num3));
						num2 += this.CalculateSize(bsonToken);
						num3 += (long)1;
					}
					num2++;
					bsonArrays.CalculatedSize = num2;
					return bsonArrays.CalculatedSize;
				}
				case BsonType.Binary:
				{
					BsonBinary length = (BsonBinary)t;
					byte[] numArray = (byte[])length.Value;
					length.CalculatedSize = 5 + (int)numArray.Length;
					return length.CalculatedSize;
				}
				case BsonType.Undefined:
				case BsonType.Null:
				{
					return 0;
				}
				case BsonType.Oid:
				{
					return 12;
				}
				case BsonType.Boolean:
				{
					return 1;
				}
				case BsonType.Date:
				{
					return 8;
				}
				case BsonType.Regex:
				{
					BsonRegex bsonRegex = (BsonRegex)t;
					int num4 = 0 + this.CalculateSize(bsonRegex.Pattern);
					num4 += this.CalculateSize(bsonRegex.Options);
					bsonRegex.CalculatedSize = num4;
					return bsonRegex.CalculatedSize;
				}
				case BsonType.Reference:
				case BsonType.Code:
				case BsonType.Symbol:
				case BsonType.CodeWScope:
				case BsonType.TimeStamp:
				{
					throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(CultureInfo.InvariantCulture, t.Type));
				}
				case BsonType.Integer:
				{
					return 4;
				}
				case BsonType.Long:
				{
					return 8;
				}
				default:
				{
					throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(CultureInfo.InvariantCulture, t.Type));
				}
			}
		}

		private int CalculateSizeWithLength(int stringByteCount, bool includeSize)
		{
			return (includeSize ? 5 : 1) + stringByteCount;
		}

		public void Close()
		{
			this._writer.Close();
		}

		public void Flush()
		{
			this._writer.Flush();
		}

		private void WriteString(string s, int byteCount, int? calculatedlengthPrefix)
		{
			if (calculatedlengthPrefix.HasValue)
			{
				this._writer.Write(calculatedlengthPrefix.GetValueOrDefault());
			}
			this.WriteUtf8Bytes(s, byteCount);
			this._writer.Write((byte)0);
		}

		public void WriteToken(BsonToken t)
		{
			this.CalculateSize(t);
			this.WriteTokenInternal(t);
		}

		private void WriteTokenInternal(BsonToken t)
		{
			int? nullable;
			switch (t.Type)
			{
				case BsonType.Number:
				{
					BsonValue bsonValue = (BsonValue)t;
					this._writer.Write(Convert.ToDouble(bsonValue.Value, CultureInfo.InvariantCulture));
					return;
				}
				case BsonType.String:
				{
					BsonString bsonString = (BsonString)t;
					this.WriteString((string)bsonString.Value, bsonString.ByteCount, new int?(bsonString.CalculatedSize - 4));
					return;
				}
				case BsonType.Object:
				{
					BsonObject bsonObjects = (BsonObject)t;
					this._writer.Write(bsonObjects.CalculatedSize);
					foreach (BsonProperty bsonProperty in bsonObjects)
					{
						this._writer.Write((sbyte)bsonProperty.Value.Type);
						nullable = null;
						this.WriteString((string)bsonProperty.Name.Value, bsonProperty.Name.ByteCount, nullable);
						this.WriteTokenInternal(bsonProperty.Value);
					}
					this._writer.Write((byte)0);
					return;
				}
				case BsonType.Array:
				{
					BsonArray bsonArrays = (BsonArray)t;
					this._writer.Write(bsonArrays.CalculatedSize);
					ulong num = (ulong)0;
					foreach (BsonToken bsonToken in bsonArrays)
					{
						this._writer.Write((sbyte)bsonToken.Type);
						nullable = null;
						this.WriteString(num.ToString(CultureInfo.InvariantCulture), MathUtils.IntLength(num), nullable);
						this.WriteTokenInternal(bsonToken);
						num += (long)1;
					}
					this._writer.Write((byte)0);
					return;
				}
				case BsonType.Binary:
				{
					BsonBinary bsonBinary = (BsonBinary)t;
					byte[] value = (byte[])bsonBinary.Value;
					this._writer.Write((int)value.Length);
					this._writer.Write((byte)bsonBinary.BinaryType);
					this._writer.Write(value);
					return;
				}
				case BsonType.Undefined:
				case BsonType.Null:
				{
					return;
				}
				case BsonType.Oid:
				{
					byte[] numArray = (byte[])((BsonValue)t).Value;
					this._writer.Write(numArray);
					return;
				}
				case BsonType.Boolean:
				{
					BsonValue bsonValue1 = (BsonValue)t;
					this._writer.Write((bool)bsonValue1.Value);
					return;
				}
				case BsonType.Date:
				{
					BsonValue bsonValue2 = (BsonValue)t;
					long javaScriptTicks = (long)0;
					if (!(bsonValue2.Value is DateTime))
					{
						DateTimeOffset dateTimeOffset = (DateTimeOffset)bsonValue2.Value;
						javaScriptTicks = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(dateTimeOffset.UtcDateTime, dateTimeOffset.Offset);
					}
					else
					{
						DateTime universalTime = (DateTime)bsonValue2.Value;
						if (this.DateTimeKindHandling == DateTimeKind.Utc)
						{
							universalTime = universalTime.ToUniversalTime();
						}
						else if (this.DateTimeKindHandling == DateTimeKind.Local)
						{
							universalTime = universalTime.ToLocalTime();
						}
						javaScriptTicks = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(universalTime, false);
					}
					this._writer.Write(javaScriptTicks);
					return;
				}
				case BsonType.Regex:
				{
					BsonRegex bsonRegex = (BsonRegex)t;
					nullable = null;
					this.WriteString((string)bsonRegex.Pattern.Value, bsonRegex.Pattern.ByteCount, nullable);
					nullable = null;
					this.WriteString((string)bsonRegex.Options.Value, bsonRegex.Options.ByteCount, nullable);
					return;
				}
				case BsonType.Reference:
				case BsonType.Code:
				case BsonType.Symbol:
				case BsonType.CodeWScope:
				case BsonType.TimeStamp:
				{
					throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(CultureInfo.InvariantCulture, t.Type));
				}
				case BsonType.Integer:
				{
					BsonValue bsonValue3 = (BsonValue)t;
					this._writer.Write(Convert.ToInt32(bsonValue3.Value, CultureInfo.InvariantCulture));
					return;
				}
				case BsonType.Long:
				{
					BsonValue bsonValue4 = (BsonValue)t;
					this._writer.Write(Convert.ToInt64(bsonValue4.Value, CultureInfo.InvariantCulture));
					return;
				}
				default:
				{
					throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(CultureInfo.InvariantCulture, t.Type));
				}
			}
		}

		public void WriteUtf8Bytes(string s, int byteCount)
		{
			if (s != null)
			{
				if (this._largeByteBuffer == null)
				{
					this._largeByteBuffer = new byte[256];
				}
				if (byteCount <= 256)
				{
					BsonBinaryWriter.Encoding.GetBytes(s, 0, s.Length, this._largeByteBuffer, 0);
					this._writer.Write(this._largeByteBuffer, 0, byteCount);
					return;
				}
				byte[] bytes = BsonBinaryWriter.Encoding.GetBytes(s);
				this._writer.Write(bytes);
			}
		}
	}
}