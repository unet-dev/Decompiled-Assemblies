using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal sealed class ArrayDecorator : ProtoDecoratorBase
	{
		private const byte OPTIONS_WritePacked = 1;

		private const byte OPTIONS_OverwriteList = 2;

		private const byte OPTIONS_SupportNull = 4;

		private readonly int fieldNumber;

		private readonly byte options;

		private readonly WireType packedWireType;

		private readonly Type arrayType;

		private readonly Type itemType;

		private bool AppendToCollection
		{
			get
			{
				return (this.options & 2) == 0;
			}
		}

		public override Type ExpectedType
		{
			get
			{
				return this.arrayType;
			}
		}

		public override bool RequiresOldValue
		{
			get
			{
				return this.AppendToCollection;
			}
		}

		public override bool ReturnsValue
		{
			get
			{
				return true;
			}
		}

		private bool SupportNull
		{
			get
			{
				return (this.options & 4) != 0;
			}
		}

		public ArrayDecorator(TypeModel model, IProtoSerializer tail, int fieldNumber, bool writePacked, WireType packedWireType, Type arrayType, bool overwriteList, bool supportNull) : base(tail)
		{
			this.itemType = arrayType.GetElementType();
			if (!supportNull)
			{
				Helpers.GetUnderlyingType(this.itemType);
			}
			if ((writePacked || packedWireType != WireType.None) && fieldNumber <= 0)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			if (!ListDecorator.CanPack(packedWireType))
			{
				if (writePacked)
				{
					throw new InvalidOperationException("Only simple data-types can use packed encoding");
				}
				packedWireType = WireType.None;
			}
			this.fieldNumber = fieldNumber;
			this.packedWireType = packedWireType;
			if (writePacked)
			{
				ArrayDecorator arrayDecorator = this;
				arrayDecorator.options = (byte)(arrayDecorator.options | 1);
			}
			if (overwriteList)
			{
				ArrayDecorator arrayDecorator1 = this;
				arrayDecorator1.options = (byte)(arrayDecorator1.options | 2);
			}
			if (supportNull)
			{
				ArrayDecorator arrayDecorator2 = this;
				arrayDecorator2.options = (byte)(arrayDecorator2.options | 4);
			}
			this.arrayType = arrayType;
		}

		protected override void EmitRead(CompilerContext ctx, Local valueFrom)
		{
			Local localWithValue;
			Local local;
			Type type = ctx.MapType(typeof(List<>));
			Type[] typeArray = new Type[] { this.itemType };
			Type type1 = type.MakeGenericType(typeArray);
			Type expectedType = this.ExpectedType;
			if (this.AppendToCollection)
			{
				localWithValue = ctx.GetLocalWithValue(expectedType, valueFrom);
			}
			else
			{
				localWithValue = null;
			}
			using (Local local1 = localWithValue)
			{
				using (Local local2 = new Local(ctx, expectedType))
				{
					using (Local local3 = new Local(ctx, type1))
					{
						ctx.EmitCtor(type1);
						ctx.StoreValue(local3);
						ListDecorator.EmitReadList(ctx, local3, this.Tail, type1.GetMethod("Add"), this.packedWireType, false);
						if (this.AppendToCollection)
						{
							local = new Local(ctx, ctx.MapType(typeof(int)));
						}
						else
						{
							local = null;
						}
						using (Local local4 = local)
						{
							Type[] typeArray1 = new Type[] { ctx.MapType(typeof(Array)), ctx.MapType(typeof(int)) };
							Type[] typeArray2 = typeArray1;
							if (!this.AppendToCollection)
							{
								ctx.LoadAddress(local3, type1);
								ctx.LoadValue(type1.GetProperty("Count"));
								ctx.CreateArray(this.itemType, null);
								ctx.StoreValue(local2);
								ctx.LoadAddress(local3, type1);
								ctx.LoadValue(local2);
								ctx.LoadValue(0);
							}
							else
							{
								ctx.LoadLength(local1, true);
								ctx.CopyValue();
								ctx.StoreValue(local4);
								ctx.LoadAddress(local3, type1);
								ctx.LoadValue(type1.GetProperty("Count"));
								ctx.Add();
								ctx.CreateArray(this.itemType, null);
								ctx.StoreValue(local2);
								ctx.LoadValue(local4);
								CodeLabel codeLabel = ctx.DefineLabel();
								ctx.BranchIfFalse(codeLabel, true);
								ctx.LoadValue(local1);
								ctx.LoadValue(local2);
								ctx.LoadValue(0);
								ctx.EmitCall(expectedType.GetMethod("CopyTo", typeArray2));
								ctx.MarkLabel(codeLabel);
								ctx.LoadValue(local3);
								ctx.LoadValue(local2);
								ctx.LoadValue(local4);
							}
							typeArray2[0] = expectedType;
							MethodInfo method = type1.GetMethod("CopyTo", typeArray2);
							if (method == null)
							{
								typeArray2[1] = ctx.MapType(typeof(Array));
								method = type1.GetMethod("CopyTo", typeArray2);
							}
							ctx.EmitCall(method);
						}
						ctx.LoadValue(local2);
					}
				}
			}
		}

		protected override void EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			Local local;
			using (Local localWithValue = ctx.GetLocalWithValue(this.arrayType, valueFrom))
			{
				using (Local local1 = new Local(ctx, ctx.MapType(typeof(int))))
				{
					bool flag = (this.options & 1) != 0;
					if (flag)
					{
						local = new Local(ctx, ctx.MapType(typeof(SubItemToken)));
					}
					else
					{
						local = null;
					}
					using (Local local2 = local)
					{
						Type type = ctx.MapType(typeof(ProtoWriter));
						if (flag)
						{
							ctx.LoadValue(this.fieldNumber);
							ctx.LoadValue(2);
							ctx.LoadReaderWriter();
							ctx.EmitCall(type.GetMethod("WriteFieldHeader"));
							ctx.LoadValue(localWithValue);
							ctx.LoadReaderWriter();
							ctx.EmitCall(type.GetMethod("StartSubItem"));
							ctx.StoreValue(local2);
							ctx.LoadValue(this.fieldNumber);
							ctx.LoadReaderWriter();
							ctx.EmitCall(type.GetMethod("SetPackedField"));
						}
						this.EmitWriteArrayLoop(ctx, local1, localWithValue);
						if (flag)
						{
							ctx.LoadValue(local2);
							ctx.LoadReaderWriter();
							ctx.EmitCall(type.GetMethod("EndSubItem"));
						}
					}
				}
			}
		}

		private void EmitWriteArrayLoop(CompilerContext ctx, Local i, Local arr)
		{
			ctx.LoadValue(0);
			ctx.StoreValue(i);
			CodeLabel codeLabel = ctx.DefineLabel();
			CodeLabel codeLabel1 = ctx.DefineLabel();
			ctx.Branch(codeLabel, false);
			ctx.MarkLabel(codeLabel1);
			ctx.LoadArrayValue(arr, i);
			if (!this.SupportNull)
			{
				ctx.WriteNullCheckedTail(this.itemType, this.Tail, null);
			}
			else
			{
				this.Tail.EmitWrite(ctx, null);
			}
			ctx.LoadValue(i);
			ctx.LoadValue(1);
			ctx.Add();
			ctx.StoreValue(i);
			ctx.MarkLabel(codeLabel);
			ctx.LoadValue(i);
			ctx.LoadLength(arr, false);
			ctx.BranchIfLess(codeLabel1, false);
		}

		public override object Read(object value, ProtoReader source)
		{
			int num;
			int fieldNumber = source.FieldNumber;
			BasicList basicLists = new BasicList();
			if (this.packedWireType == WireType.None || source.WireType != WireType.String)
			{
				do
				{
					basicLists.Add(this.Tail.Read(null, source));
				}
				while (source.TryReadFieldHeader(fieldNumber));
			}
			else
			{
				SubItemToken subItemToken = ProtoReader.StartSubItem(source);
				while (ProtoReader.HasSubValue(this.packedWireType, source))
				{
					basicLists.Add(this.Tail.Read(null, source));
				}
				ProtoReader.EndSubItem(subItemToken, source);
			}
			if (this.AppendToCollection)
			{
				num = (value == null ? 0 : ((Array)value).Length);
			}
			else
			{
				num = 0;
			}
			int num1 = num;
			Array arrays = Array.CreateInstance(this.itemType, num1 + basicLists.Count);
			if (num1 != 0)
			{
				((Array)value).CopyTo(arrays, 0);
			}
			basicLists.CopyTo(arrays, num1);
			return arrays;
		}

		public override void Write(object value, ProtoWriter dest)
		{
			SubItemToken subItemToken;
			IList lists = (IList)value;
			int count = lists.Count;
			bool flag = (this.options & 1) != 0;
			if (!flag)
			{
				subItemToken = new SubItemToken();
			}
			else
			{
				ProtoWriter.WriteFieldHeader(this.fieldNumber, WireType.String, dest);
				subItemToken = ProtoWriter.StartSubItem(value, dest);
				ProtoWriter.SetPackedField(this.fieldNumber, dest);
			}
			bool supportNull = !this.SupportNull;
			for (int i = 0; i < count; i++)
			{
				object item = lists[i];
				if (supportNull && item == null)
				{
					throw new NullReferenceException();
				}
				this.Tail.Write(item, dest);
			}
			if (flag)
			{
				ProtoWriter.EndSubItem(subItemToken, dest);
			}
		}
	}
}