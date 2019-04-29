using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class Int64Serializer : IProtoSerializer
	{
		private readonly static Type expectedType;

		public Type ExpectedType
		{
			get
			{
				return Int64Serializer.expectedType;
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

		static Int64Serializer()
		{
			Int64Serializer.expectedType = typeof(long);
		}

		public Int64Serializer(TypeModel model)
		{
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
		{
			ctx.EmitBasicRead("ReadInt64", this.ExpectedType);
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ctx.EmitBasicWrite("WriteInt64", valueFrom);
		}

		public object Read(object value, ProtoReader source)
		{
			return source.ReadInt64();
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteInt64((long)value, dest);
		}
	}
}