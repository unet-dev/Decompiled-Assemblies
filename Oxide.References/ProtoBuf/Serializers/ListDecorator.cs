using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal class ListDecorator : ProtoDecoratorBase
	{
		private const byte OPTIONS_IsList = 1;

		private const byte OPTIONS_SuppressIList = 2;

		private const byte OPTIONS_WritePacked = 4;

		private const byte OPTIONS_ReturnList = 8;

		private const byte OPTIONS_OverwriteList = 16;

		private const byte OPTIONS_SupportNull = 32;

		private readonly byte options;

		private readonly Type declaredType;

		private readonly Type concreteType;

		private readonly MethodInfo @add;

		private readonly int fieldNumber;

		protected readonly WireType packedWireType;

		private readonly static Type ienumeratorType;

		private readonly static Type ienumerableType;

		protected bool AppendToCollection
		{
			get
			{
				return (this.options & 16) == 0;
			}
		}

		public override Type ExpectedType
		{
			get
			{
				return this.declaredType;
			}
		}

		private bool IsList
		{
			get
			{
				return (this.options & 1) != 0;
			}
		}

		protected virtual bool RequireAdd
		{
			get
			{
				return true;
			}
		}

		public override bool RequiresOldValue
		{
			get
			{
				return this.AppendToCollection;
			}
		}

		private bool ReturnList
		{
			get
			{
				return (this.options & 8) != 0;
			}
		}

		public override bool ReturnsValue
		{
			get
			{
				return this.ReturnList;
			}
		}

		private bool SupportNull
		{
			get
			{
				return (this.options & 32) != 0;
			}
		}

		private bool SuppressIList
		{
			get
			{
				return (this.options & 2) != 0;
			}
		}

		private bool WritePacked
		{
			get
			{
				return (this.options & 4) != 0;
			}
		}

		static ListDecorator()
		{
			ListDecorator.ienumeratorType = typeof(IEnumerator);
			ListDecorator.ienumerableType = typeof(IEnumerable);
		}

		protected ListDecorator(TypeModel model, Type declaredType, Type concreteType, IProtoSerializer tail, int fieldNumber, bool writePacked, WireType packedWireType, bool returnList, bool overwriteList, bool supportNull) : base(tail)
		{
			bool flag;
			if (returnList)
			{
				ListDecorator listDecorator = this;
				listDecorator.options = (byte)(listDecorator.options | 8);
			}
			if (overwriteList)
			{
				ListDecorator listDecorator1 = this;
				listDecorator1.options = (byte)(listDecorator1.options | 16);
			}
			if (supportNull)
			{
				ListDecorator listDecorator2 = this;
				listDecorator2.options = (byte)(listDecorator2.options | 32);
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
			if (writePacked)
			{
				ListDecorator listDecorator3 = this;
				listDecorator3.options = (byte)(listDecorator3.options | 4);
			}
			this.packedWireType = packedWireType;
			if (declaredType == null)
			{
				throw new ArgumentNullException("declaredType");
			}
			if (declaredType.IsArray)
			{
				throw new ArgumentException("Cannot treat arrays as lists", "declaredType");
			}
			this.declaredType = declaredType;
			this.concreteType = concreteType;
			if (this.RequireAdd)
			{
				this.@add = TypeModel.ResolveListAdd(model, declaredType, tail.ExpectedType, out flag);
				if (flag)
				{
					ListDecorator listDecorator4 = this;
					listDecorator4.options = (byte)(listDecorator4.options | 1);
					string fullName = declaredType.FullName;
					if (fullName != null && fullName.StartsWith("System.Data.Linq.EntitySet`1[["))
					{
						ListDecorator listDecorator5 = this;
						listDecorator5.options = (byte)(listDecorator5.options | 2);
					}
				}
				if (this.@add == null)
				{
					throw new InvalidOperationException(string.Concat("Unable to resolve a suitable Add method for ", declaredType.FullName));
				}
			}
		}

		internal static bool CanPack(WireType wireType)
		{
			WireType wireType1 = wireType;
			switch (wireType1)
			{
				case WireType.Variant:
				case WireType.Fixed64:
				{
					return true;
				}
				default:
				{
					if (wireType1 == WireType.Fixed32 || wireType1 == WireType.SignedVariant)
					{
						return true;
					}
					return false;
				}
			}
		}

		internal static ListDecorator Create(TypeModel model, Type declaredType, Type concreteType, IProtoSerializer tail, int fieldNumber, bool writePacked, WireType packedWireType, bool returnList, bool overwriteList, bool supportNull)
		{
			MethodInfo methodInfo;
			MethodInfo methodInfo1;
			MethodInfo methodInfo2;
			MethodInfo methodInfo3;
			if (!returnList || !ImmutableCollectionDecorator.IdentifyImmutable(model, declaredType, out methodInfo, out methodInfo1, out methodInfo2, out methodInfo3))
			{
				return new ListDecorator(model, declaredType, concreteType, tail, fieldNumber, writePacked, packedWireType, returnList, overwriteList, supportNull);
			}
			return new ImmutableCollectionDecorator(model, declaredType, concreteType, tail, fieldNumber, writePacked, packedWireType, returnList, overwriteList, supportNull, methodInfo, methodInfo1, methodInfo2, methodInfo3);
		}

		protected override void EmitRead(CompilerContext ctx, Local valueFrom)
		{
			Local local;
			bool returnList = this.ReturnList;
			using (Local local1 = (this.AppendToCollection ? ctx.GetLocalWithValue(this.ExpectedType, valueFrom) : new Local(ctx, this.declaredType)))
			{
				if (!returnList || !this.AppendToCollection)
				{
					local = null;
				}
				else
				{
					local = new Local(ctx, this.ExpectedType);
				}
				using (Local local2 = local)
				{
					if (!this.AppendToCollection)
					{
						ctx.LoadNullRef();
						ctx.StoreValue(local1);
					}
					else if (returnList)
					{
						ctx.LoadValue(local1);
						ctx.StoreValue(local2);
					}
					if (this.concreteType != null)
					{
						ctx.LoadValue(local1);
						CodeLabel codeLabel = ctx.DefineLabel();
						ctx.BranchIfTrue(codeLabel, true);
						ctx.EmitCtor(this.concreteType);
						ctx.StoreValue(local1);
						ctx.MarkLabel(codeLabel);
					}
					bool flag = !this.@add.DeclaringType.IsAssignableFrom(this.declaredType);
					ListDecorator.EmitReadList(ctx, local1, this.Tail, this.@add, this.packedWireType, flag);
					if (returnList)
					{
						if (!this.AppendToCollection)
						{
							ctx.LoadValue(local1);
						}
						else
						{
							ctx.LoadValue(local2);
							ctx.LoadValue(local1);
							CodeLabel codeLabel1 = ctx.DefineLabel();
							CodeLabel codeLabel2 = ctx.DefineLabel();
							ctx.BranchIfEqual(codeLabel1, true);
							ctx.LoadValue(local1);
							ctx.Branch(codeLabel2, true);
							ctx.MarkLabel(codeLabel1);
							ctx.LoadNullRef();
							ctx.MarkLabel(codeLabel2);
						}
					}
				}
			}
		}

		private static void EmitReadAndAddItem(CompilerContext ctx, Local list, IProtoSerializer tail, MethodInfo add, bool castListForAdd)
		{
			ctx.LoadAddress(list, list.Type);
			if (castListForAdd)
			{
				ctx.Cast(add.DeclaringType);
			}
			Type expectedType = tail.ExpectedType;
			bool returnsValue = tail.ReturnsValue;
			if (!tail.RequiresOldValue)
			{
				if (!returnsValue)
				{
					throw new InvalidOperationException();
				}
				tail.EmitRead(ctx, null);
			}
			else if (expectedType.IsValueType || !returnsValue)
			{
				using (Local local = new Local(ctx, expectedType))
				{
					if (!expectedType.IsValueType)
					{
						ctx.LoadNullRef();
						ctx.StoreValue(local);
					}
					else
					{
						ctx.LoadAddress(local, expectedType);
						ctx.EmitCtor(expectedType);
					}
					tail.EmitRead(ctx, local);
					if (!returnsValue)
					{
						ctx.LoadValue(local);
					}
				}
			}
			else
			{
				ctx.LoadNullRef();
				tail.EmitRead(ctx, null);
			}
			Type parameterType = add.GetParameters()[0].ParameterType;
			if (parameterType != expectedType)
			{
				if (parameterType != ctx.MapType(typeof(object)))
				{
					if (Helpers.GetUnderlyingType(parameterType) != expectedType)
					{
						throw new InvalidOperationException("Conflicting item/add type");
					}
					Type[] typeArray = new Type[] { expectedType };
					ctx.EmitCtor(Helpers.GetConstructor(parameterType, typeArray, false));
				}
				else
				{
					ctx.CastToObject(expectedType);
				}
			}
			ctx.EmitCall(add);
			if (add.ReturnType != ctx.MapType(typeof(void)))
			{
				ctx.DiscardValue();
			}
		}

		internal static void EmitReadList(CompilerContext ctx, Local list, IProtoSerializer tail, MethodInfo add, WireType packedWireType, bool castListForAdd)
		{
			using (Local local = new Local(ctx, ctx.MapType(typeof(int))))
			{
				CodeLabel codeLabel = (packedWireType == WireType.None ? new CodeLabel() : ctx.DefineLabel());
				if (packedWireType != WireType.None)
				{
					ctx.LoadReaderWriter();
					ctx.LoadValue(typeof(ProtoReader).GetProperty("WireType"));
					ctx.LoadValue(2);
					ctx.BranchIfEqual(codeLabel, false);
				}
				ctx.LoadReaderWriter();
				ctx.LoadValue(typeof(ProtoReader).GetProperty("FieldNumber"));
				ctx.StoreValue(local);
				CodeLabel codeLabel1 = ctx.DefineLabel();
				ctx.MarkLabel(codeLabel1);
				ListDecorator.EmitReadAndAddItem(ctx, list, tail, add, castListForAdd);
				ctx.LoadReaderWriter();
				ctx.LoadValue(local);
				ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("TryReadFieldHeader"));
				ctx.BranchIfTrue(codeLabel1, false);
				if (packedWireType != WireType.None)
				{
					CodeLabel codeLabel2 = ctx.DefineLabel();
					ctx.Branch(codeLabel2, false);
					ctx.MarkLabel(codeLabel);
					ctx.LoadReaderWriter();
					ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("StartSubItem"));
					CodeLabel codeLabel3 = ctx.DefineLabel();
					CodeLabel codeLabel4 = ctx.DefineLabel();
					ctx.MarkLabel(codeLabel3);
					ctx.LoadValue((int)packedWireType);
					ctx.LoadReaderWriter();
					ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("HasSubValue"));
					ctx.BranchIfFalse(codeLabel4, false);
					ListDecorator.EmitReadAndAddItem(ctx, list, tail, add, castListForAdd);
					ctx.Branch(codeLabel3, false);
					ctx.MarkLabel(codeLabel4);
					ctx.LoadReaderWriter();
					ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("EndSubItem"));
					ctx.MarkLabel(codeLabel2);
				}
			}
		}

		protected override void EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			MethodInfo methodInfo;
			MethodInfo methodInfo1;
			Local local;
			using (Local localWithValue = ctx.GetLocalWithValue(this.ExpectedType, valueFrom))
			{
				MethodInfo enumeratorInfo = this.GetEnumeratorInfo(ctx.Model, out methodInfo, out methodInfo1);
				Type returnType = enumeratorInfo.ReturnType;
				bool writePacked = this.WritePacked;
				using (Local local1 = new Local(ctx, returnType))
				{
					if (writePacked)
					{
						local = new Local(ctx, ctx.MapType(typeof(SubItemToken)));
					}
					else
					{
						local = null;
					}
					using (Local local2 = local)
					{
						if (writePacked)
						{
							ctx.LoadValue(this.fieldNumber);
							ctx.LoadValue(2);
							ctx.LoadReaderWriter();
							ctx.EmitCall(ctx.MapType(typeof(ProtoWriter)).GetMethod("WriteFieldHeader"));
							ctx.LoadValue(localWithValue);
							ctx.LoadReaderWriter();
							ctx.EmitCall(ctx.MapType(typeof(ProtoWriter)).GetMethod("StartSubItem"));
							ctx.StoreValue(local2);
							ctx.LoadValue(this.fieldNumber);
							ctx.LoadReaderWriter();
							ctx.EmitCall(ctx.MapType(typeof(ProtoWriter)).GetMethod("SetPackedField"));
						}
						ctx.LoadAddress(localWithValue, this.ExpectedType);
						ctx.EmitCall(enumeratorInfo);
						ctx.StoreValue(local1);
						using (IDisposable disposable = ctx.Using(local1))
						{
							CodeLabel codeLabel = ctx.DefineLabel();
							CodeLabel codeLabel1 = ctx.DefineLabel();
							ctx.Branch(codeLabel1, false);
							ctx.MarkLabel(codeLabel);
							ctx.LoadAddress(local1, returnType);
							ctx.EmitCall(methodInfo1);
							Type expectedType = this.Tail.ExpectedType;
							if (expectedType != ctx.MapType(typeof(object)) && methodInfo1.ReturnType == ctx.MapType(typeof(object)))
							{
								ctx.CastFromObject(expectedType);
							}
							this.Tail.EmitWrite(ctx, null);
							ctx.MarkLabel(codeLabel1);
							ctx.LoadAddress(local1, returnType);
							ctx.EmitCall(methodInfo);
							ctx.BranchIfTrue(codeLabel, false);
						}
						if (writePacked)
						{
							ctx.LoadValue(local2);
							ctx.LoadReaderWriter();
							ctx.EmitCall(ctx.MapType(typeof(ProtoWriter)).GetMethod("EndSubItem"));
						}
					}
				}
			}
		}

		protected MethodInfo GetEnumeratorInfo(TypeModel model, out MethodInfo moveNext, out MethodInfo current)
		{
			Type returnType;
			MethodInfo getMethod;
			Type type = null;
			Type expectedType = this.ExpectedType;
			MethodInfo instanceMethod = Helpers.GetInstanceMethod(expectedType, "GetEnumerator", null);
			Type expectedType1 = this.Tail.ExpectedType;
			if (instanceMethod != null)
			{
				returnType = instanceMethod.ReturnType;
				moveNext = Helpers.GetInstanceMethod(returnType, "MoveNext", null);
				PropertyInfo property = Helpers.GetProperty(returnType, "Current", false);
				if (property == null)
				{
					getMethod = null;
				}
				else
				{
					getMethod = Helpers.GetGetMethod(property, false, false);
				}
				current = getMethod;
				if (moveNext == null && model.MapType(ListDecorator.ienumeratorType).IsAssignableFrom(returnType))
				{
					moveNext = Helpers.GetInstanceMethod(model.MapType(ListDecorator.ienumeratorType), "MoveNext", null);
				}
				if (moveNext != null && moveNext.ReturnType == model.MapType(typeof(bool)) && current != null && current.ReturnType == expectedType1)
				{
					return instanceMethod;
				}
				object obj = null;
				instanceMethod = (MethodInfo)obj;
				MethodInfo methodInfo = (MethodInfo)obj;
				current = (MethodInfo)obj;
				moveNext = methodInfo;
			}
			Type type1 = model.MapType(typeof(IEnumerable<>), false);
			if (type1 != null)
			{
				type1 = type1.MakeGenericType(new Type[] { expectedType1 });
				type = type1;
			}
			if (type != null && type.IsAssignableFrom(expectedType))
			{
				instanceMethod = Helpers.GetInstanceMethod(type, "GetEnumerator");
				returnType = instanceMethod.ReturnType;
				moveNext = Helpers.GetInstanceMethod(model.MapType(ListDecorator.ienumeratorType), "MoveNext");
				current = Helpers.GetGetMethod(Helpers.GetProperty(returnType, "Current", false), false, false);
				return instanceMethod;
			}
			type = model.MapType(ListDecorator.ienumerableType);
			instanceMethod = Helpers.GetInstanceMethod(type, "GetEnumerator");
			returnType = instanceMethod.ReturnType;
			moveNext = Helpers.GetInstanceMethod(returnType, "MoveNext");
			current = Helpers.GetGetMethod(Helpers.GetProperty(returnType, "Current", false), false, false);
			return instanceMethod;
		}

		public override object Read(object value, ProtoReader source)
		{
			int fieldNumber = source.FieldNumber;
			object obj = value;
			if (value == null)
			{
				value = Activator.CreateInstance(this.concreteType);
			}
			bool flag = (!this.IsList ? false : !this.SuppressIList);
			if (this.packedWireType != WireType.None && source.WireType == WireType.String)
			{
				SubItemToken subItemToken = ProtoReader.StartSubItem(source);
				if (!flag)
				{
					object[] objArray = new object[1];
					while (ProtoReader.HasSubValue(this.packedWireType, source))
					{
						objArray[0] = this.Tail.Read(null, source);
						this.@add.Invoke(value, objArray);
					}
				}
				else
				{
					IList lists = (IList)value;
					while (ProtoReader.HasSubValue(this.packedWireType, source))
					{
						lists.Add(this.Tail.Read(null, source));
					}
				}
				ProtoReader.EndSubItem(subItemToken, source);
			}
			else if (!flag)
			{
				object[] objArray1 = new object[1];
				do
				{
					objArray1[0] = this.Tail.Read(null, source);
					this.@add.Invoke(value, objArray1);
				}
				while (source.TryReadFieldHeader(fieldNumber));
			}
			else
			{
				IList lists1 = (IList)value;
				do
				{
					lists1.Add(this.Tail.Read(null, source));
				}
				while (source.TryReadFieldHeader(fieldNumber));
			}
			if (obj != value)
			{
				return value;
			}
			return null;
		}

		public override void Write(object value, ProtoWriter dest)
		{
			SubItemToken subItemToken;
			bool writePacked = this.WritePacked;
			if (!writePacked)
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
			foreach (object obj in (IEnumerable)value)
			{
				if (supportNull && obj == null)
				{
					throw new NullReferenceException();
				}
				this.Tail.Write(obj, dest);
			}
			if (writePacked)
			{
				ProtoWriter.EndSubItem(subItemToken, dest);
			}
		}
	}
}