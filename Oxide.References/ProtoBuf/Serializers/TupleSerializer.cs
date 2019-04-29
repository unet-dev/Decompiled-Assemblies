using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal sealed class TupleSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		private readonly MemberInfo[] members;

		private readonly ConstructorInfo ctor;

		private IProtoSerializer[] tails;

		public Type ExpectedType
		{
			get
			{
				return this.ctor.DeclaringType;
			}
		}

		public bool RequiresOldValue
		{
			get
			{
				return true;
			}
		}

		public bool ReturnsValue
		{
			get
			{
				return false;
			}
		}

		public TupleSerializer(RuntimeTypeModel model, ConstructorInfo ctor, MemberInfo[] members)
		{
			WireType wireType;
			IProtoSerializer arrayDecorator;
			if (ctor == null)
			{
				throw new ArgumentNullException("ctor");
			}
			if (members == null)
			{
				throw new ArgumentNullException("members");
			}
			this.ctor = ctor;
			this.members = members;
			this.tails = new IProtoSerializer[(int)members.Length];
			ParameterInfo[] parameters = ctor.GetParameters();
			for (int i = 0; i < (int)members.Length; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				Type type = null;
				Type type1 = null;
				MetaType.ResolveListTypes(model, parameterType, ref type, ref type1);
				Type type2 = (type == null ? parameterType : type);
				bool asReferenceDefault = false;
				if (model.FindOrAddAuto(type2, false, true, false) >= 0)
				{
					asReferenceDefault = model[type2].AsReferenceDefault;
				}
				IProtoSerializer tagDecorator = ValueMember.TryGetCoreSerializer(model, DataFormat.Default, type2, out wireType, asReferenceDefault, false, false, true);
				if (tagDecorator == null)
				{
					throw new InvalidOperationException(string.Concat("No serializer defined for type: ", type2.FullName));
				}
				tagDecorator = new TagDecorator(i + 1, wireType, false, tagDecorator);
				if (type == null)
				{
					arrayDecorator = tagDecorator;
				}
				else if (!parameterType.IsArray)
				{
					arrayDecorator = ListDecorator.Create(model, parameterType, type1, tagDecorator, i + 1, false, wireType, true, false, false);
				}
				else
				{
					arrayDecorator = new ArrayDecorator(model, tagDecorator, i + 1, false, wireType, parameterType, false, false);
				}
				this.tails[i] = arrayDecorator;
			}
		}

		public void EmitCallback(CompilerContext ctx, Local valueFrom, TypeModel.CallbackType callbackType)
		{
		}

		public void EmitRead(CompilerContext ctx, Local incoming)
		{
			Local local;
			using (Local localWithValue = ctx.GetLocalWithValue(this.ExpectedType, incoming))
			{
				Local[] localArray = new Local[(int)this.members.Length];
				try
				{
					for (int i = 0; i < (int)localArray.Length; i++)
					{
						Type memberType = this.GetMemberType(i);
						bool flag = true;
						localArray[i] = new Local(ctx, memberType);
						if (!this.ExpectedType.IsValueType)
						{
							if (!memberType.IsValueType)
							{
								ctx.LoadNullRef();
							}
							else
							{
								ProtoTypeCode typeCode = Helpers.GetTypeCode(memberType);
								switch (typeCode)
								{
									case ProtoTypeCode.Boolean:
									case ProtoTypeCode.SByte:
									case ProtoTypeCode.Byte:
									case ProtoTypeCode.Int16:
									case ProtoTypeCode.UInt16:
									case ProtoTypeCode.Int32:
									case ProtoTypeCode.UInt32:
									{
										ctx.LoadValue(0);
										break;
									}
									case ProtoTypeCode.Char:
									{
										ctx.LoadAddress(localArray[i], memberType);
										ctx.EmitCtor(memberType);
										flag = false;
										break;
									}
									case ProtoTypeCode.Int64:
									case ProtoTypeCode.UInt64:
									{
										ctx.LoadValue((long)0);
										break;
									}
									case ProtoTypeCode.Single:
									{
										ctx.LoadValue(0f);
										break;
									}
									case ProtoTypeCode.Double:
									{
										ctx.LoadValue(0);
										break;
									}
									case ProtoTypeCode.Decimal:
									{
										ctx.LoadValue(new decimal(0));
										break;
									}
									default:
									{
										if (typeCode == ProtoTypeCode.Guid)
										{
											ctx.LoadValue(Guid.Empty);
											break;
										}
										else
										{
											goto case ProtoTypeCode.Char;
										}
									}
								}
							}
							if (flag)
							{
								ctx.StoreValue(localArray[i]);
							}
						}
					}
					CodeLabel codeLabel = (this.ExpectedType.IsValueType ? new CodeLabel() : ctx.DefineLabel());
					if (!this.ExpectedType.IsValueType)
					{
						ctx.LoadAddress(localWithValue, this.ExpectedType);
						ctx.BranchIfFalse(codeLabel, false);
					}
					for (int j = 0; j < (int)this.members.Length; j++)
					{
						ctx.LoadAddress(localWithValue, this.ExpectedType);
						MemberTypes memberType1 = this.members[j].MemberType;
						if (memberType1 == MemberTypes.Field)
						{
							ctx.LoadValue((FieldInfo)this.members[j]);
						}
						else if (memberType1 == MemberTypes.Property)
						{
							ctx.LoadValue((PropertyInfo)this.members[j]);
						}
						ctx.StoreValue(localArray[j]);
					}
					if (!this.ExpectedType.IsValueType)
					{
						ctx.MarkLabel(codeLabel);
					}
					using (Local local1 = new Local(ctx, ctx.MapType(typeof(int))))
					{
						CodeLabel codeLabel1 = ctx.DefineLabel();
						CodeLabel codeLabel2 = ctx.DefineLabel();
						CodeLabel codeLabel3 = ctx.DefineLabel();
						ctx.Branch(codeLabel1, false);
						CodeLabel[] codeLabelArray = new CodeLabel[(int)this.members.Length];
						for (int k = 0; k < (int)this.members.Length; k++)
						{
							codeLabelArray[k] = ctx.DefineLabel();
						}
						ctx.MarkLabel(codeLabel2);
						ctx.LoadValue(local1);
						ctx.LoadValue(1);
						ctx.Subtract();
						ctx.Switch(codeLabelArray);
						ctx.Branch(codeLabel3, false);
						for (int l = 0; l < (int)codeLabelArray.Length; l++)
						{
							ctx.MarkLabel(codeLabelArray[l]);
							IProtoSerializer protoSerializer = this.tails[l];
							if (protoSerializer.RequiresOldValue)
							{
								local = localArray[l];
							}
							else
							{
								local = null;
							}
							ctx.ReadNullCheckedTail(localArray[l].Type, protoSerializer, local);
							if (protoSerializer.ReturnsValue)
							{
								if (!localArray[l].Type.IsValueType)
								{
									CodeLabel codeLabel4 = ctx.DefineLabel();
									CodeLabel codeLabel5 = ctx.DefineLabel();
									ctx.CopyValue();
									ctx.BranchIfTrue(codeLabel4, true);
									ctx.DiscardValue();
									ctx.Branch(codeLabel5, true);
									ctx.MarkLabel(codeLabel4);
									ctx.StoreValue(localArray[l]);
									ctx.MarkLabel(codeLabel5);
								}
								else
								{
									ctx.StoreValue(localArray[l]);
								}
							}
							ctx.Branch(codeLabel1, false);
						}
						ctx.MarkLabel(codeLabel3);
						ctx.LoadReaderWriter();
						ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("SkipField"));
						ctx.MarkLabel(codeLabel1);
						ctx.EmitBasicRead("ReadFieldHeader", ctx.MapType(typeof(int)));
						ctx.CopyValue();
						ctx.StoreValue(local1);
						ctx.LoadValue(0);
						ctx.BranchIfGreater(codeLabel2, false);
					}
					for (int m = 0; m < (int)localArray.Length; m++)
					{
						ctx.LoadValue(localArray[m]);
					}
					ctx.EmitCtor(this.ctor);
					ctx.StoreValue(localWithValue);
				}
				finally
				{
					for (int n = 0; n < (int)localArray.Length; n++)
					{
						if (localArray[n] != null)
						{
							localArray[n].Dispose();
						}
					}
				}
			}
		}

		public void EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			using (Local localWithValue = ctx.GetLocalWithValue(this.ctor.DeclaringType, valueFrom))
			{
				for (int i = 0; i < (int)this.tails.Length; i++)
				{
					Type memberType = this.GetMemberType(i);
					ctx.LoadAddress(localWithValue, this.ExpectedType);
					MemberTypes memberType1 = this.members[i].MemberType;
					if (memberType1 == MemberTypes.Field)
					{
						ctx.LoadValue((FieldInfo)this.members[i]);
					}
					else if (memberType1 == MemberTypes.Property)
					{
						ctx.LoadValue((PropertyInfo)this.members[i]);
					}
					ctx.WriteNullCheckedTail(memberType, this.tails[i], null);
				}
			}
		}

		private Type GetMemberType(int index)
		{
			Type memberType = Helpers.GetMemberType(this.members[index]);
			if (memberType == null)
			{
				throw new InvalidOperationException();
			}
			return memberType;
		}

		private object GetValue(object obj, int index)
		{
			PropertyInfo propertyInfo = this.members[index] as PropertyInfo;
			PropertyInfo propertyInfo1 = propertyInfo;
			if (propertyInfo != null)
			{
				if (obj != null)
				{
					return propertyInfo1.GetValue(obj, null);
				}
				if (!Helpers.IsValueType(propertyInfo1.PropertyType))
				{
					return null;
				}
				return Activator.CreateInstance(propertyInfo1.PropertyType);
			}
			FieldInfo fieldInfo = this.members[index] as FieldInfo;
			FieldInfo fieldInfo1 = fieldInfo;
			if (fieldInfo == null)
			{
				throw new InvalidOperationException();
			}
			if (obj != null)
			{
				return fieldInfo1.GetValue(obj);
			}
			if (!Helpers.IsValueType(fieldInfo1.FieldType))
			{
				return null;
			}
			return Activator.CreateInstance(fieldInfo1.FieldType);
		}

		public bool HasCallbacks(TypeModel.CallbackType callbackType)
		{
			return false;
		}

		void ProtoBuf.Serializers.IProtoTypeSerializer.Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
		}

		bool ProtoBuf.Serializers.IProtoTypeSerializer.CanCreateInstance()
		{
			return false;
		}

		object ProtoBuf.Serializers.IProtoTypeSerializer.CreateInstance(ProtoReader source)
		{
			throw new NotSupportedException();
		}

		void ProtoBuf.Serializers.IProtoTypeSerializer.EmitCreateInstance(CompilerContext ctx)
		{
			throw new NotSupportedException();
		}

		public object Read(object value, ProtoReader source)
		{
			object[] objArray = new object[(int)this.members.Length];
			bool flag = false;
			if (value == null)
			{
				flag = true;
			}
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				objArray[i] = this.GetValue(value, i);
			}
			while (true)
			{
				int num = source.ReadFieldHeader();
				int num1 = num;
				if (num <= 0)
				{
					break;
				}
				flag = true;
				if (num1 > (int)this.tails.Length)
				{
					source.SkipField();
				}
				else
				{
					IProtoSerializer protoSerializer = this.tails[num1 - 1];
					objArray[num1 - 1] = this.tails[num1 - 1].Read((protoSerializer.RequiresOldValue ? objArray[num1 - 1] : null), source);
				}
			}
			if (!flag)
			{
				return value;
			}
			return this.ctor.Invoke(objArray);
		}

		public void Write(object value, ProtoWriter dest)
		{
			for (int i = 0; i < (int)this.tails.Length; i++)
			{
				object obj = this.GetValue(value, i);
				if (obj != null)
				{
					this.tails[i].Write(obj, dest);
				}
			}
		}
	}
}