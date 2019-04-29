using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal sealed class ImmutableCollectionDecorator : ListDecorator
	{
		private readonly MethodInfo builderFactory;

		private readonly MethodInfo @add;

		private readonly MethodInfo addRange;

		private readonly MethodInfo finish;

		protected override bool RequireAdd
		{
			get
			{
				return false;
			}
		}

		internal ImmutableCollectionDecorator(TypeModel model, Type declaredType, Type concreteType, IProtoSerializer tail, int fieldNumber, bool writePacked, WireType packedWireType, bool returnList, bool overwriteList, bool supportNull, MethodInfo builderFactory, MethodInfo add, MethodInfo addRange, MethodInfo finish) : base(model, declaredType, concreteType, tail, fieldNumber, writePacked, packedWireType, returnList, overwriteList, supportNull)
		{
			this.builderFactory = builderFactory;
			this.@add = add;
			this.addRange = addRange;
			this.finish = finish;
		}

		protected override void EmitRead(CompilerContext ctx, Local valueFrom)
		{
			MethodInfo methodInfo;
			MethodInfo methodInfo1;
			Local localWithValue;
			if (base.AppendToCollection)
			{
				localWithValue = ctx.GetLocalWithValue(this.ExpectedType, valueFrom);
			}
			else
			{
				localWithValue = null;
			}
			using (Local local = localWithValue)
			{
				using (Local local1 = new Local(ctx, this.builderFactory.ReturnType))
				{
					ctx.EmitCall(this.builderFactory);
					ctx.StoreValue(local1);
					if (base.AppendToCollection)
					{
						CodeLabel codeLabel = ctx.DefineLabel();
						if (!this.ExpectedType.IsValueType)
						{
							ctx.LoadValue(local);
							ctx.BranchIfFalse(codeLabel, false);
						}
						PropertyInfo property = Helpers.GetProperty(this.ExpectedType, "Length", false) ?? Helpers.GetProperty(this.ExpectedType, "Count", false) ?? Helpers.GetProperty(ImmutableCollectionDecorator.ResolveIReadOnlyCollection(this.ExpectedType, this.Tail.ExpectedType), "Count", false);
						ctx.LoadAddress(local, local.Type);
						ctx.EmitCall(Helpers.GetGetMethod(property, false, false));
						ctx.BranchIfFalse(codeLabel, false);
						Type type = ctx.MapType(typeof(void));
						if (this.addRange == null)
						{
							MethodInfo enumeratorInfo = base.GetEnumeratorInfo(ctx.Model, out methodInfo, out methodInfo1);
							Type returnType = enumeratorInfo.ReturnType;
							using (Local local2 = new Local(ctx, returnType))
							{
								ctx.LoadAddress(local, this.ExpectedType);
								ctx.EmitCall(enumeratorInfo);
								ctx.StoreValue(local2);
								using (IDisposable disposable = ctx.Using(local2))
								{
									CodeLabel codeLabel1 = ctx.DefineLabel();
									CodeLabel codeLabel2 = ctx.DefineLabel();
									ctx.Branch(codeLabel2, false);
									ctx.MarkLabel(codeLabel1);
									ctx.LoadAddress(local1, local1.Type);
									ctx.LoadAddress(local2, returnType);
									ctx.EmitCall(methodInfo1);
									ctx.EmitCall(this.@add);
									if (this.@add.ReturnType != null && this.@add.ReturnType != type)
									{
										ctx.DiscardValue();
									}
									ctx.MarkLabel(codeLabel2);
									ctx.LoadAddress(local2, returnType);
									ctx.EmitCall(methodInfo);
									ctx.BranchIfTrue(codeLabel1, false);
								}
							}
						}
						else
						{
							ctx.LoadValue(local1);
							ctx.LoadValue(local);
							ctx.EmitCall(this.addRange);
							if (this.addRange.ReturnType != null && this.@add.ReturnType != type)
							{
								ctx.DiscardValue();
							}
						}
						ctx.MarkLabel(codeLabel);
					}
					ListDecorator.EmitReadList(ctx, local1, this.Tail, this.@add, this.packedWireType, false);
					ctx.LoadAddress(local1, local1.Type);
					ctx.EmitCall(this.finish);
					if (this.ExpectedType != this.finish.ReturnType)
					{
						ctx.Cast(this.ExpectedType);
					}
				}
			}
		}

		internal static bool IdentifyImmutable(TypeModel model, Type declaredType, out MethodInfo builderFactory, out MethodInfo add, out MethodInfo addRange, out MethodInfo finish)
		{
			Type[] typeArray;
			object obj = null;
			MethodInfo methodInfo = (MethodInfo)obj;
			finish = (MethodInfo)obj;
			MethodInfo methodInfo1 = methodInfo;
			MethodInfo methodInfo2 = methodInfo1;
			addRange = methodInfo1;
			MethodInfo methodInfo3 = methodInfo2;
			MethodInfo methodInfo4 = methodInfo3;
			add = methodInfo3;
			builderFactory = methodInfo4;
			if (model == null || declaredType == null)
			{
				return false;
			}
			Type type = declaredType;
			if (!type.IsGenericType)
			{
				return false;
			}
			Type[] genericArguments = type.GetGenericArguments();
			switch ((int)genericArguments.Length)
			{
				case 1:
				{
					typeArray = genericArguments;
					break;
				}
				case 2:
				{
					Type type1 = model.MapType(typeof(KeyValuePair<,>));
					if (type1 == null)
					{
						return false;
					}
					type1 = type1.MakeGenericType(genericArguments);
					typeArray = new Type[] { type1 };
					break;
				}
				default:
				{
					return false;
				}
			}
			if (ImmutableCollectionDecorator.ResolveIReadOnlyCollection(declaredType, null) == null)
			{
				return false;
			}
			string name = declaredType.Name;
			int num = name.IndexOf('\u0060');
			if (num <= 0)
			{
				return false;
			}
			name = (type.IsInterface ? name.Substring(1, num - 1) : name.Substring(0, num));
			Type type2 = model.GetType(string.Concat(declaredType.Namespace, ".", name), type.Assembly);
			if (type2 == null && name == "ImmutableSet")
			{
				type2 = model.GetType(string.Concat(declaredType.Namespace, ".ImmutableHashSet"), type.Assembly);
			}
			if (type2 == null)
			{
				return false;
			}
			MethodInfo[] methods = type2.GetMethods();
			int num1 = 0;
			while (num1 < (int)methods.Length)
			{
				MethodInfo methodInfo5 = methods[num1];
				if (!methodInfo5.IsStatic || methodInfo5.Name != "CreateBuilder" || !methodInfo5.IsGenericMethodDefinition || (int)methodInfo5.GetParameters().Length != 0 || (int)methodInfo5.GetGenericArguments().Length != (int)genericArguments.Length)
				{
					num1++;
				}
				else
				{
					builderFactory = methodInfo5.MakeGenericMethod(genericArguments);
					break;
				}
			}
			Type type3 = model.MapType(typeof(void));
			if (builderFactory == null || builderFactory.ReturnType == null || builderFactory.ReturnType == type3)
			{
				return false;
			}
			add = Helpers.GetInstanceMethod(builderFactory.ReturnType, "Add", typeArray);
			if (add == null)
			{
				return false;
			}
			finish = Helpers.GetInstanceMethod(builderFactory.ReturnType, "ToImmutable", Helpers.EmptyTypes);
			if (finish == null || finish.ReturnType == null || finish.ReturnType == type3)
			{
				return false;
			}
			if (finish.ReturnType != declaredType && !Helpers.IsAssignableFrom(declaredType, finish.ReturnType))
			{
				return false;
			}
			Type returnType = builderFactory.ReturnType;
			Type[] typeArray1 = new Type[] { declaredType };
			addRange = Helpers.GetInstanceMethod(returnType, "AddRange", typeArray1);
			if (addRange == null)
			{
				Type type4 = model.MapType(typeof(IEnumerable<>), false);
				if (type4 != null)
				{
					Type returnType1 = builderFactory.ReturnType;
					Type[] typeArray2 = new Type[] { type4.MakeGenericType(typeArray) };
					addRange = Helpers.GetInstanceMethod(returnType1, "AddRange", typeArray2);
				}
			}
			return true;
		}

		public override object Read(object value, ProtoReader source)
		{
			object obj = this.builderFactory.Invoke(null, null);
			int fieldNumber = source.FieldNumber;
			object[] objArray = new object[1];
			if (base.AppendToCollection && value != null && ((IList)value).Count != 0)
			{
				if (this.addRange == null)
				{
					foreach (object obj1 in (IList)value)
					{
						objArray[0] = obj1;
						this.@add.Invoke(obj, objArray);
					}
				}
				else
				{
					objArray[0] = value;
					this.addRange.Invoke(obj, objArray);
				}
			}
			if (this.packedWireType == WireType.None || source.WireType != WireType.String)
			{
				do
				{
					objArray[0] = this.Tail.Read(null, source);
					this.@add.Invoke(obj, objArray);
				}
				while (source.TryReadFieldHeader(fieldNumber));
			}
			else
			{
				SubItemToken subItemToken = ProtoReader.StartSubItem(source);
				while (ProtoReader.HasSubValue(this.packedWireType, source))
				{
					objArray[0] = this.Tail.Read(null, source);
					this.@add.Invoke(obj, objArray);
				}
				ProtoReader.EndSubItem(subItemToken, source);
			}
			return this.finish.Invoke(obj, null);
		}

		private static Type ResolveIReadOnlyCollection(Type declaredType, Type t)
		{
			Type type;
			Type[] interfaces = declaredType.GetInterfaces();
			int num = 0;
			while (true)
			{
				if (num >= (int)interfaces.Length)
				{
					return null;
				}
				type = interfaces[num];
				if (type.IsGenericType && type.Name.StartsWith("IReadOnlyCollection`"))
				{
					if (t == null)
					{
						break;
					}
					Type[] genericArguments = type.GetGenericArguments();
					if ((int)genericArguments.Length == 1 || genericArguments[0] == t)
					{
						break;
					}
				}
				num++;
			}
			return type;
		}
	}
}