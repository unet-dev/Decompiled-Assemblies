using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class DateTimeSerializer : IProtoSerializer
	{
		private readonly static Type expectedType;

		public Type ExpectedType
		{
			get
			{
				return DateTimeSerializer.expectedType;
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

		static DateTimeSerializer()
		{
			DateTimeSerializer.expectedType = typeof(DateTime);
		}

		public DateTimeSerializer(TypeModel model)
		{
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
		{
			ctx.EmitBasicRead(ctx.MapType(typeof(BclHelpers)), "ReadDateTime", this.ExpectedType);
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ctx.EmitWrite(ctx.MapType(typeof(BclHelpers)), "WriteDateTime", valueFrom);
		}

		public object Read(object value, ProtoReader source)
		{
			return BclHelpers.ReadDateTime(source);
		}

		public void Write(object value, ProtoWriter dest)
		{
			BclHelpers.WriteDateTime((DateTime)value, dest);
		}
	}
}