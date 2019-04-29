using ProtoBuf;
using ProtoBuf.Compiler;
using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal sealed class FieldDecorator : ProtoDecoratorBase
	{
		private readonly FieldInfo field;

		private readonly Type forType;

		public override Type ExpectedType
		{
			get
			{
				return this.forType;
			}
		}

		public override bool RequiresOldValue
		{
			get
			{
				return true;
			}
		}

		public override bool ReturnsValue
		{
			get
			{
				return false;
			}
		}

		public FieldDecorator(Type forType, FieldInfo field, IProtoSerializer tail) : base(tail)
		{
			this.forType = forType;
			this.field = field;
		}

		protected override void EmitRead(CompilerContext ctx, Local valueFrom)
		{
			using (Local localWithValue = ctx.GetLocalWithValue(this.ExpectedType, valueFrom))
			{
				if (this.Tail.RequiresOldValue)
				{
					ctx.LoadAddress(localWithValue, this.ExpectedType);
					ctx.LoadValue(this.field);
				}
				ctx.ReadNullCheckedTail(this.field.FieldType, this.Tail, null);
				if (this.Tail.ReturnsValue)
				{
					using (Local local = new Local(ctx, this.field.FieldType))
					{
						ctx.StoreValue(local);
						if (!this.field.FieldType.IsValueType)
						{
							CodeLabel codeLabel = ctx.DefineLabel();
							ctx.LoadValue(local);
							ctx.BranchIfFalse(codeLabel, true);
							ctx.LoadAddress(localWithValue, this.ExpectedType);
							ctx.LoadValue(local);
							ctx.StoreValue(this.field);
							ctx.MarkLabel(codeLabel);
						}
						else
						{
							ctx.LoadAddress(localWithValue, this.ExpectedType);
							ctx.LoadValue(local);
							ctx.StoreValue(this.field);
						}
					}
				}
			}
		}

		protected override void EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ctx.LoadAddress(valueFrom, this.ExpectedType);
			ctx.LoadValue(this.field);
			ctx.WriteNullCheckedTail(this.field.FieldType, this.Tail, null);
		}

		public override object Read(object value, ProtoReader source)
		{
			object obj = this.Tail.Read((this.Tail.RequiresOldValue ? this.field.GetValue(value) : null), source);
			if (obj != null)
			{
				this.field.SetValue(value, obj);
			}
			return null;
		}

		public override void Write(object value, ProtoWriter dest)
		{
			value = this.field.GetValue(value);
			if (value != null)
			{
				this.Tail.Write(value, dest);
			}
		}
	}
}