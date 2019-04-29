using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class StringSerializer : IProtoSerializer
	{
		private readonly static Type expectedType;

		public Type ExpectedType
		{
			get
			{
				return StringSerializer.expectedType;
			}
		}

		bool ProtoBuf.Serializers.IProtoSerializer.RequiresOldValue
		{
			get
			{
				return false;
			}
		}

		bool ProtoBuf.Serializers.IProtoSerializer.ReturnsValue
		{
			get
			{
				return true;
			}
		}

		static StringSerializer()
		{
			StringSerializer.expectedType = typeof(string);
		}

		public StringSerializer(TypeModel model)
		{
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
		{
			ctx.EmitBasicRead("ReadString", this.ExpectedType);
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ctx.EmitBasicWrite("WriteString", valueFrom);
		}

		public object Read(object value, ProtoReader source)
		{
			return source.ReadString();
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteString((string)value, dest);
		}
	}
}