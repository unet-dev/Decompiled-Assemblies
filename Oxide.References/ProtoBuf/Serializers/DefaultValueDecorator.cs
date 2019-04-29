using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal sealed class DefaultValueDecorator : ProtoDecoratorBase
	{
		private readonly object defaultValue;

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

		public DefaultValueDecorator(TypeModel model, object defaultValue, IProtoSerializer tail) : base(tail)
		{
			if (defaultValue == null)
			{
				throw new ArgumentNullException("defaultValue");
			}
			if (model.MapType(defaultValue.GetType()) != tail.ExpectedType)
			{
				throw new ArgumentException("Default value is of incorrect type", "defaultValue");
			}
			this.defaultValue = defaultValue;
		}

		private void EmitBeq(CompilerContext ctx, CodeLabel label, Type type)
		{
			switch (Helpers.GetTypeCode(type))
			{
				case ProtoTypeCode.Boolean:
				case ProtoTypeCode.Char:
				case ProtoTypeCode.SByte:
				case ProtoTypeCode.Byte:
				case ProtoTypeCode.Int16:
				case ProtoTypeCode.UInt16:
				case ProtoTypeCode.Int32:
				case ProtoTypeCode.UInt32:
				case ProtoTypeCode.Int64:
				case ProtoTypeCode.UInt64:
				case ProtoTypeCode.Single:
				case ProtoTypeCode.Double:
				{
					ctx.BranchIfEqual(label, false);
					return;
				}
			}
			Type[] typeArray = new Type[] { type, type };
			MethodInfo method = type.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public, null, typeArray, null);
			if (method == null || method.ReturnType != ctx.MapType(typeof(bool)))
			{
				throw new InvalidOperationException(string.Concat("No suitable equality operator found for default-values of type: ", type.FullName));
			}
			ctx.EmitCall(method);
			ctx.BranchIfTrue(label, false);
		}

		private void EmitBranchIfDefaultValue(CompilerContext ctx, CodeLabel label)
		{
			Type expectedType = this.ExpectedType;
			ProtoTypeCode typeCode = Helpers.GetTypeCode(expectedType);
			switch (typeCode)
			{
				case ProtoTypeCode.Boolean:
				{
					if ((bool)this.defaultValue)
					{
						ctx.BranchIfTrue(label, false);
						return;
					}
					ctx.BranchIfFalse(label, false);
					return;
				}
				case ProtoTypeCode.Char:
				{
					if ((char)this.defaultValue == 0)
					{
						ctx.BranchIfFalse(label, false);
						return;
					}
					ctx.LoadValue((char)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.SByte:
				{
					if ((sbyte)this.defaultValue == 0)
					{
						ctx.BranchIfFalse(label, false);
						return;
					}
					ctx.LoadValue((sbyte)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.Byte:
				{
					if ((byte)this.defaultValue == 0)
					{
						ctx.BranchIfFalse(label, false);
						return;
					}
					ctx.LoadValue((int)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.Int16:
				{
					if ((short)this.defaultValue == 0)
					{
						ctx.BranchIfFalse(label, false);
						return;
					}
					ctx.LoadValue((short)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.UInt16:
				{
					if ((ushort)this.defaultValue == 0)
					{
						ctx.BranchIfFalse(label, false);
						return;
					}
					ctx.LoadValue((int)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.Int32:
				{
					if ((int)this.defaultValue == 0)
					{
						ctx.BranchIfFalse(label, false);
						return;
					}
					ctx.LoadValue((int)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.UInt32:
				{
					if ((uint)this.defaultValue == 0)
					{
						ctx.BranchIfFalse(label, false);
						return;
					}
					ctx.LoadValue((int)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.Int64:
				{
					ctx.LoadValue((long)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.UInt64:
				{
					ctx.LoadValue((long)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.Single:
				{
					ctx.LoadValue((float)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.Double:
				{
					ctx.LoadValue((double)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.Decimal:
				{
					ctx.LoadValue((decimal)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.DateTime:
				{
					ctx.LoadValue(((DateTime)this.defaultValue).ToBinary());
					ctx.EmitCall(ctx.MapType(typeof(DateTime)).GetMethod("FromBinary"));
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				case ProtoTypeCode.Unknown | ProtoTypeCode.DateTime:
				{
					throw new NotSupportedException(string.Concat("Type cannot be represented as a default value: ", expectedType.FullName));
				}
				case ProtoTypeCode.String:
				{
					ctx.LoadValue((string)this.defaultValue);
					this.EmitBeq(ctx, label, expectedType);
					return;
				}
				default:
				{
					switch (typeCode)
					{
						case ProtoTypeCode.TimeSpan:
						{
							TimeSpan timeSpan = (TimeSpan)this.defaultValue;
							if (timeSpan != TimeSpan.Zero)
							{
								ctx.LoadValue(timeSpan.Ticks);
								ctx.EmitCall(ctx.MapType(typeof(TimeSpan)).GetMethod("FromTicks"));
							}
							else
							{
								ctx.LoadValue(typeof(TimeSpan).GetField("Zero"));
							}
							this.EmitBeq(ctx, label, expectedType);
							return;
						}
						case ProtoTypeCode.ByteArray:
						{
							throw new NotSupportedException(string.Concat("Type cannot be represented as a default value: ", expectedType.FullName));
						}
						case ProtoTypeCode.Guid:
						{
							ctx.LoadValue((Guid)this.defaultValue);
							this.EmitBeq(ctx, label, expectedType);
							return;
						}
						default:
						{
							throw new NotSupportedException(string.Concat("Type cannot be represented as a default value: ", expectedType.FullName));
						}
					}
					break;
				}
			}
		}

		protected override void EmitRead(CompilerContext ctx, Local valueFrom)
		{
			this.Tail.EmitRead(ctx, valueFrom);
		}

		protected override void EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			CodeLabel codeLabel = ctx.DefineLabel();
			if (valueFrom != null)
			{
				ctx.LoadValue(valueFrom);
				this.EmitBranchIfDefaultValue(ctx, codeLabel);
				this.Tail.EmitWrite(ctx, valueFrom);
			}
			else
			{
				ctx.CopyValue();
				CodeLabel codeLabel1 = ctx.DefineLabel();
				this.EmitBranchIfDefaultValue(ctx, codeLabel1);
				this.Tail.EmitWrite(ctx, null);
				ctx.Branch(codeLabel, true);
				ctx.MarkLabel(codeLabel1);
				ctx.DiscardValue();
			}
			ctx.MarkLabel(codeLabel);
		}

		public override object Read(object value, ProtoReader source)
		{
			return this.Tail.Read(value, source);
		}

		public override void Write(object value, ProtoWriter dest)
		{
			if (!object.Equals(value, this.defaultValue))
			{
				this.Tail.Write(value, dest);
			}
		}
	}
}