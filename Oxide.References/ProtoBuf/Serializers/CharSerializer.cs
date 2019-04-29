using ProtoBuf;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class CharSerializer : UInt16Serializer
	{
		private readonly static Type expectedType;

		public override Type ExpectedType
		{
			get
			{
				return CharSerializer.expectedType;
			}
		}

		static CharSerializer()
		{
			CharSerializer.expectedType = typeof(char);
		}

		public CharSerializer(TypeModel model) : base(model)
		{
		}

		public override object Read(object value, ProtoReader source)
		{
			return (char)source.ReadUInt16();
		}

		public override void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteUInt16((char)value, dest);
		}
	}
}