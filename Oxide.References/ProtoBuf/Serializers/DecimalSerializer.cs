using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class DecimalSerializer : IProtoSerializer
	{
		private readonly static Type expectedType;

		public Type ExpectedType
		{
			get
			{
				return DecimalSerializer.expectedType;
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

		static DecimalSerializer()
		{
			DecimalSerializer.expectedType = typeof(decimal);
		}

		public DecimalSerializer(TypeModel model)
		{
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
		{
			ctx.EmitBasicRead(ctx.MapType(typeof(BclHelpers)), "ReadDecimal", this.ExpectedType);
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ctx.EmitWrite(ctx.MapType(typeof(BclHelpers)), "WriteDecimal", valueFrom);
		}

		public object Read(object value, ProtoReader source)
		{
			return BclHelpers.ReadDecimal(source);
		}

		public void Write(object value, ProtoWriter dest)
		{
			BclHelpers.WriteDecimal((decimal)value, dest);
		}
	}
}