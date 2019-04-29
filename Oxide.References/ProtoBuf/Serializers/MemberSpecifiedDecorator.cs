using ProtoBuf;
using ProtoBuf.Compiler;
using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal sealed class MemberSpecifiedDecorator : ProtoDecoratorBase
	{
		private readonly MethodInfo getSpecified;

		private readonly MethodInfo setSpecified;

		public override Type ExpectedType
		{
			get
			{
				return this.Tail.ExpectedType;
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

		public MemberSpecifiedDecorator(MethodInfo getSpecified, MethodInfo setSpecified, IProtoSerializer tail) : base(tail)
		{
			if (getSpecified == null && setSpecified == null)
			{
				throw new InvalidOperationException();
			}
			this.getSpecified = getSpecified;
			this.setSpecified = setSpecified;
		}

		protected override void EmitRead(CompilerContext ctx, Local valueFrom)
		{
			if (this.setSpecified == null)
			{
				this.Tail.EmitRead(ctx, valueFrom);
				return;
			}
			using (Local localWithValue = ctx.GetLocalWithValue(this.ExpectedType, valueFrom))
			{
				this.Tail.EmitRead(ctx, localWithValue);
				ctx.LoadAddress(localWithValue, this.ExpectedType);
				ctx.LoadValue(1);
				ctx.EmitCall(this.setSpecified);
			}
		}

		protected override void EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			if (this.getSpecified == null)
			{
				this.Tail.EmitWrite(ctx, valueFrom);
				return;
			}
			using (Local localWithValue = ctx.GetLocalWithValue(this.ExpectedType, valueFrom))
			{
				ctx.LoadAddress(localWithValue, this.ExpectedType);
				ctx.EmitCall(this.getSpecified);
				CodeLabel codeLabel = ctx.DefineLabel();
				ctx.BranchIfFalse(codeLabel, false);
				this.Tail.EmitWrite(ctx, localWithValue);
				ctx.MarkLabel(codeLabel);
			}
		}

		public override object Read(object value, ProtoReader source)
		{
			object obj = this.Tail.Read(value, source);
			if (this.setSpecified != null)
			{
				MethodInfo methodInfo = this.setSpecified;
				object[] objArray = new object[] { true };
				methodInfo.Invoke(value, objArray);
			}
			return obj;
		}

		public override void Write(object value, ProtoWriter dest)
		{
			if (this.getSpecified == null || (bool)this.getSpecified.Invoke(value, null))
			{
				this.Tail.Write(value, dest);
			}
		}
	}
}