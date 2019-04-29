using ProtoBuf;
using ProtoBuf.Compiler;
using System;

namespace ProtoBuf.Serializers
{
	internal abstract class ProtoDecoratorBase : IProtoSerializer
	{
		protected readonly IProtoSerializer Tail;

		public abstract Type ExpectedType
		{
			get;
		}

		public abstract bool RequiresOldValue
		{
			get;
		}

		public abstract bool ReturnsValue
		{
			get;
		}

		protected ProtoDecoratorBase(IProtoSerializer tail)
		{
			this.Tail = tail;
		}

		protected abstract void EmitRead(CompilerContext ctx, Local valueFrom);

		protected abstract void EmitWrite(CompilerContext ctx, Local valueFrom);

		void ProtoBuf.Serializers.IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
		{
			this.EmitRead(ctx, valueFrom);
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			this.EmitWrite(ctx, valueFrom);
		}

		public abstract object Read(object value, ProtoReader source);

		public abstract void Write(object value, ProtoWriter dest);
	}
}