using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class SystemTypeSerializer : IProtoSerializer
	{
		private readonly static Type expectedType;

		public Type ExpectedType
		{
			get
			{
				return SystemTypeSerializer.expectedType;
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

		static SystemTypeSerializer()
		{
			SystemTypeSerializer.expectedType = typeof(Type);
		}

		public SystemTypeSerializer(TypeModel model)
		{
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
		{
			ctx.EmitBasicRead("ReadType", this.ExpectedType);
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ctx.EmitBasicWrite("WriteType", valueFrom);
		}

		object ProtoBuf.Serializers.IProtoSerializer.Read(object value, ProtoReader source)
		{
			return source.ReadType();
		}

		void ProtoBuf.Serializers.IProtoSerializer.Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteType((Type)value, dest);
		}
	}
}