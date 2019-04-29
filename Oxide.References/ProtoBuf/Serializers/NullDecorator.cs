using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class NullDecorator : ProtoDecoratorBase
	{
		public const int Tag = 1;

		private readonly Type expectedType;

		public override Type ExpectedType
		{
			get
			{
				return this.expectedType;
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
				return true;
			}
		}

		public NullDecorator(TypeModel model, IProtoSerializer tail) : base(tail)
		{
			if (!tail.ReturnsValue)
			{
				throw new NotSupportedException("NullDecorator only supports implementations that return values");
			}
			Type expectedType = tail.ExpectedType;
			if (!Helpers.IsValueType(expectedType))
			{
				this.expectedType = expectedType;
				return;
			}
			Type type = model.MapType(typeof(Nullable<>));
			Type[] typeArray = new Type[] { expectedType };
			this.expectedType = type.MakeGenericType(typeArray);
		}

		protected override void EmitRead(CompilerContext ctx, Local valueFrom)
		{
			using (Local localWithValue = ctx.GetLocalWithValue(this.expectedType, valueFrom))
			{
				using (Local local = new Local(ctx, ctx.MapType(typeof(SubItemToken))))
				{
					using (Local local1 = new Local(ctx, ctx.MapType(typeof(int))))
					{
						ctx.LoadReaderWriter();
						ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("StartSubItem"));
						ctx.StoreValue(local);
						CodeLabel codeLabel = ctx.DefineLabel();
						CodeLabel codeLabel1 = ctx.DefineLabel();
						CodeLabel codeLabel2 = ctx.DefineLabel();
						ctx.MarkLabel(codeLabel);
						ctx.EmitBasicRead("ReadFieldHeader", ctx.MapType(typeof(int)));
						ctx.CopyValue();
						ctx.StoreValue(local1);
						ctx.LoadValue(1);
						ctx.BranchIfEqual(codeLabel1, true);
						ctx.LoadValue(local1);
						ctx.LoadValue(1);
						ctx.BranchIfLess(codeLabel2, false);
						ctx.LoadReaderWriter();
						ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("SkipField"));
						ctx.Branch(codeLabel, true);
						ctx.MarkLabel(codeLabel1);
						if (this.Tail.RequiresOldValue)
						{
							if (!this.expectedType.IsValueType)
							{
								ctx.LoadValue(localWithValue);
							}
							else
							{
								ctx.LoadAddress(localWithValue, this.expectedType);
								ctx.EmitCall(this.expectedType.GetMethod("GetValueOrDefault", Helpers.EmptyTypes));
							}
						}
						this.Tail.EmitRead(ctx, null);
						if (this.expectedType.IsValueType)
						{
							Type type = this.expectedType;
							Type[] expectedType = new Type[] { this.Tail.ExpectedType };
							ctx.EmitCtor(type, expectedType);
						}
						ctx.StoreValue(localWithValue);
						ctx.Branch(codeLabel, false);
						ctx.MarkLabel(codeLabel2);
						ctx.LoadValue(local);
						ctx.LoadReaderWriter();
						ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("EndSubItem"));
						ctx.LoadValue(localWithValue);
					}
				}
			}
		}

		protected override void EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			using (Local localWithValue = ctx.GetLocalWithValue(this.expectedType, valueFrom))
			{
				using (Local local = new Local(ctx, ctx.MapType(typeof(SubItemToken))))
				{
					ctx.LoadNullRef();
					ctx.LoadReaderWriter();
					ctx.EmitCall(ctx.MapType(typeof(ProtoWriter)).GetMethod("StartSubItem"));
					ctx.StoreValue(local);
					if (!this.expectedType.IsValueType)
					{
						ctx.LoadValue(localWithValue);
					}
					else
					{
						ctx.LoadAddress(localWithValue, this.expectedType);
						ctx.LoadValue(this.expectedType.GetProperty("HasValue"));
					}
					CodeLabel codeLabel = ctx.DefineLabel();
					ctx.BranchIfFalse(codeLabel, false);
					if (!this.expectedType.IsValueType)
					{
						ctx.LoadValue(localWithValue);
					}
					else
					{
						ctx.LoadAddress(localWithValue, this.expectedType);
						ctx.EmitCall(this.expectedType.GetMethod("GetValueOrDefault", Helpers.EmptyTypes));
					}
					this.Tail.EmitWrite(ctx, null);
					ctx.MarkLabel(codeLabel);
					ctx.LoadValue(local);
					ctx.LoadReaderWriter();
					ctx.EmitCall(ctx.MapType(typeof(ProtoWriter)).GetMethod("EndSubItem"));
				}
			}
		}

		public override object Read(object value, ProtoReader source)
		{
			SubItemToken subItemToken = ProtoReader.StartSubItem(source);
			while (true)
			{
				int num = source.ReadFieldHeader();
				int num1 = num;
				if (num <= 0)
				{
					break;
				}
				if (num1 != 1)
				{
					source.SkipField();
				}
				else
				{
					value = this.Tail.Read(value, source);
				}
			}
			ProtoReader.EndSubItem(subItemToken, source);
			return value;
		}

		public override void Write(object value, ProtoWriter dest)
		{
			SubItemToken subItemToken = ProtoWriter.StartSubItem(null, dest);
			if (value != null)
			{
				this.Tail.Write(value, dest);
			}
			ProtoWriter.EndSubItem(subItemToken, dest);
		}
	}
}