using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace ProtoBuf
{
	public static class Serializer
	{
		private const string ProtoBinaryField = "proto";

		public const int ListItemTag = 1;

		public static TTo ChangeType<TFrom, TTo>(TFrom instance)
		{
			TTo tTo;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Serializer.Serialize<TFrom>(memoryStream, instance);
				memoryStream.Position = (long)0;
				tTo = Serializer.Deserialize<TTo>(memoryStream);
			}
			return tTo;
		}

		public static IFormatter CreateFormatter<T>()
		{
			return RuntimeTypeModel.Default.CreateFormatter(typeof(T));
		}

		public static T DeepClone<T>(T instance)
		{
			if (instance == null)
			{
				return instance;
			}
			return (T)RuntimeTypeModel.Default.DeepClone(instance);
		}

		public static T Deserialize<T>(Stream source)
		{
			return (T)RuntimeTypeModel.Default.Deserialize(source, null, typeof(T));
		}

		public static IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int fieldNumber)
		{
			return RuntimeTypeModel.Default.DeserializeItems<T>(source, style, fieldNumber);
		}

		public static T DeserializeWithLengthPrefix<T>(Stream source, PrefixStyle style)
		{
			return Serializer.DeserializeWithLengthPrefix<T>(source, style, 0);
		}

		public static T DeserializeWithLengthPrefix<T>(Stream source, PrefixStyle style, int fieldNumber)
		{
			RuntimeTypeModel @default = RuntimeTypeModel.Default;
			return (T)@default.DeserializeWithLengthPrefix(source, null, @default.MapType(typeof(T)), style, fieldNumber);
		}

		public static void FlushPool()
		{
			BufferPool.Flush();
		}

		public static string GetProto<T>()
		{
			return RuntimeTypeModel.Default.GetSchema(RuntimeTypeModel.Default.MapType(typeof(T)));
		}

		public static T Merge<T>(Stream source, T instance)
		{
			return (T)RuntimeTypeModel.Default.Deserialize(source, instance, typeof(T));
		}

		public static void Merge<T>(XmlReader reader, T instance)
		where T : IXmlSerializable
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			byte[] numArray = new byte[4096];
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int depth = reader.Depth;
				do
				{
				Label1:
					if (!reader.Read() || reader.Depth <= depth)
					{
						break;
					}
					if (reader.NodeType == XmlNodeType.Text)
					{
						while (true)
						{
							int num = reader.ReadContentAsBase64(numArray, 0, 4096);
							int num1 = num;
							if (num <= 0)
							{
								goto Label0;
							}
							memoryStream.Write(numArray, 0, num1);
						}
					}
					else
					{
						goto Label1;
					}
				Label0:
				}
				while (reader.Depth > depth);
				memoryStream.Position = (long)0;
				Serializer.Merge<T>(memoryStream, instance);
			}
		}

		public static void Merge<T>(SerializationInfo info, T instance)
		where T : class, ISerializable
		{
			Serializer.Merge<T>(info, new StreamingContext(StreamingContextStates.Persistence), instance);
		}

		public static void Merge<T>(SerializationInfo info, StreamingContext context, T instance)
		where T : class, ISerializable
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (instance.GetType() != typeof(T))
			{
				throw new ArgumentException("Incorrect type", "instance");
			}
			byte[] value = (byte[])info.GetValue("proto", typeof(byte[]));
			using (MemoryStream memoryStream = new MemoryStream(value))
			{
				if (!object.ReferenceEquals((T)RuntimeTypeModel.Default.Deserialize(memoryStream, instance, typeof(T), context), instance))
				{
					throw new ProtoException("Deserialization changed the instance; cannot succeed.");
				}
			}
		}

		public static T MergeWithLengthPrefix<T>(Stream source, T instance, PrefixStyle style)
		{
			RuntimeTypeModel @default = RuntimeTypeModel.Default;
			return (T)@default.DeserializeWithLengthPrefix(source, instance, @default.MapType(typeof(T)), style, 0);
		}

		public static void PrepareSerializer<T>()
		{
			RuntimeTypeModel @default = RuntimeTypeModel.Default;
			@default[@default.MapType(typeof(T))].CompileInPlace();
		}

		public static void Serialize<T>(Stream destination, T instance)
		{
			if (instance != null)
			{
				RuntimeTypeModel.Default.Serialize(destination, instance);
			}
		}

		public static void Serialize<T>(SerializationInfo info, T instance)
		where T : class, ISerializable
		{
			Serializer.Serialize<T>(info, new StreamingContext(StreamingContextStates.Persistence), instance);
		}

		public static void Serialize<T>(SerializationInfo info, StreamingContext context, T instance)
		where T : class, ISerializable
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (instance.GetType() != typeof(T))
			{
				throw new ArgumentException("Incorrect type", "instance");
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				RuntimeTypeModel.Default.Serialize(memoryStream, instance, context);
				info.AddValue("proto", memoryStream.ToArray());
			}
		}

		public static void Serialize<T>(XmlWriter writer, T instance)
		where T : IXmlSerializable
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Serializer.Serialize<T>(memoryStream, instance);
				writer.WriteBase64(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			}
		}

		public static void SerializeWithLengthPrefix<T>(Stream destination, T instance, PrefixStyle style)
		{
			Serializer.SerializeWithLengthPrefix<T>(destination, instance, style, 0);
		}

		public static void SerializeWithLengthPrefix<T>(Stream destination, T instance, PrefixStyle style, int fieldNumber)
		{
			RuntimeTypeModel @default = RuntimeTypeModel.Default;
			@default.SerializeWithLengthPrefix(destination, instance, @default.MapType(typeof(T)), style, fieldNumber);
		}

		public static bool TryReadLengthPrefix(Stream source, PrefixStyle style, out int length)
		{
			int num;
			int num1;
			length = ProtoReader.ReadLengthPrefix(source, false, style, out num, out num1);
			return num1 > 0;
		}

		public static bool TryReadLengthPrefix(byte[] buffer, int index, int count, PrefixStyle style, out int length)
		{
			bool flag;
			using (Stream memoryStream = new MemoryStream(buffer, index, count))
			{
				flag = Serializer.TryReadLengthPrefix(memoryStream, style, out length);
			}
			return flag;
		}

		public static class GlobalOptions
		{
			[Obsolete("Please use RuntimeTypeModel.Default.InferTagFromNameDefault instead (or on a per-model basis)", false)]
			public static bool InferTagFromName
			{
				get
				{
					return RuntimeTypeModel.Default.InferTagFromNameDefault;
				}
				set
				{
					RuntimeTypeModel.Default.InferTagFromNameDefault = value;
				}
			}
		}

		public static class NonGeneric
		{
			public static bool CanSerialize(Type type)
			{
				return RuntimeTypeModel.Default.IsDefined(type);
			}

			public static object DeepClone(object instance)
			{
				if (instance == null)
				{
					return null;
				}
				return RuntimeTypeModel.Default.DeepClone(instance);
			}

			public static object Deserialize(Type type, Stream source)
			{
				return RuntimeTypeModel.Default.Deserialize(source, null, type);
			}

			public static object Merge(Stream source, object instance)
			{
				if (instance == null)
				{
					throw new ArgumentNullException("instance");
				}
				return RuntimeTypeModel.Default.Deserialize(source, instance, instance.GetType(), null);
			}

			public static void Serialize(Stream dest, object instance)
			{
				if (instance != null)
				{
					RuntimeTypeModel.Default.Serialize(dest, instance);
				}
			}

			public static void SerializeWithLengthPrefix(Stream destination, object instance, PrefixStyle style, int fieldNumber)
			{
				if (instance == null)
				{
					throw new ArgumentNullException("instance");
				}
				RuntimeTypeModel @default = RuntimeTypeModel.Default;
				@default.SerializeWithLengthPrefix(destination, instance, @default.MapType(instance.GetType()), style, fieldNumber);
			}

			public static bool TryDeserializeWithLengthPrefix(Stream source, PrefixStyle style, Serializer.TypeResolver resolver, out object value)
			{
				value = RuntimeTypeModel.Default.DeserializeWithLengthPrefix(source, null, null, style, 0, resolver);
				return value != null;
			}
		}

		public delegate Type TypeResolver(int fieldNumber);
	}
}