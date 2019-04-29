using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class TagDecorator : ProtoDecoratorBase, IProtoTypeSerializer, IProtoSerializer
	{
		private readonly bool strict;

		private readonly int fieldNumber;

		private readonly WireType wireType;

		public override Type ExpectedType
		{
			get
			{
				return this.Tail.ExpectedType;
			}
		}

		private bool NeedsHint
		{
			get
			{
				return ((int)this.wireType & -8) != (int)WireType.Variant;
			}
		}

		public override bool RequiresOldValue
		{
			get
			{
				return this.Tail.RequiresOldValue;
			}
		}

		public override bool ReturnsValue
		{
			get
			{
				return this.Tail.ReturnsValue;
			}
		}

		public TagDecorator(int fieldNumber, WireType wireType, bool strict, IProtoSerializer tail) : base(tail)
		{
			this.fieldNumber = fieldNumber;
			this.wireType = wireType;
			this.strict = strict;
		}

		public void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
			IProtoTypeSerializer tail = this.Tail as IProtoTypeSerializer;
			if (tail != null)
			{
				tail.Callback(value, callbackType, context);
			}
		}

		public bool CanCreateInstance()
		{
			IProtoTypeSerializer tail = this.Tail as IProtoTypeSerializer;
			if (tail == null)
			{
				return false;
			}
			return tail.CanCreateInstance();
		}

		public object CreateInstance(ProtoReader source)
		{
			return ((IProtoTypeSerializer)this.Tail).CreateInstance(source);
		}

		public void EmitCallback(CompilerContext ctx, Local valueFrom, TypeModel.CallbackType callbackType)
		{
			((IProtoTypeSerializer)this.Tail).EmitCallback(ctx, valueFrom, callbackType);
		}

		public void EmitCreateInstance(CompilerContext ctx)
		{
			((IProtoTypeSerializer)this.Tail).EmitCreateInstance(ctx);
		}

		protected override void EmitRead(CompilerContext ctx, Local valueFrom)
		{
			if (this.strict || this.NeedsHint)
			{
				ctx.LoadReaderWriter();
				ctx.LoadValue((int)this.wireType);
				ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod((this.strict ? "Assert" : "Hint")));
			}
			this.Tail.EmitRead(ctx, valueFrom);
		}

		protected override void EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ctx.LoadValue(this.fieldNumber);
			ctx.LoadValue((int)this.wireType);
			ctx.LoadReaderWriter();
			ctx.EmitCall(ctx.MapType(typeof(ProtoWriter)).GetMethod("WriteFieldHeader"));
			this.Tail.EmitWrite(ctx, valueFrom);
		}

		public bool HasCallbacks(TypeModel.CallbackType callbackType)
		{
			IProtoTypeSerializer tail = this.Tail as IProtoTypeSerializer;
			if (tail == null)
			{
				return false;
			}
			return tail.HasCallbacks(callbackType);
		}

		public override object Read(object value, ProtoReader source)
		{
			if (this.strict)
			{
				source.Assert(this.wireType);
			}
			else if (this.NeedsHint)
			{
				source.Hint(this.wireType);
			}
			return this.Tail.Read(value, source);
		}

		public override void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteFieldHeader(this.fieldNumber, this.wireType, dest);
			this.Tail.Write(value, dest);
		}
	}
}