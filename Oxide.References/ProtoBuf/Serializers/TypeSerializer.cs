using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace ProtoBuf.Serializers
{
	internal sealed class TypeSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		private readonly Type forType;

		private readonly Type constructType;

		private readonly IProtoSerializer[] serializers;

		private readonly int[] fieldNumbers;

		private readonly bool isRootType;

		private readonly bool useConstructor;

		private readonly bool isExtensible;

		private readonly bool hasConstructor;

		private readonly CallbackSet callbacks;

		private readonly MethodInfo[] baseCtorCallbacks;

		private readonly MethodInfo factory;

		private readonly static Type iextensible;

		private bool CanHaveInheritance
		{
			get
			{
				if (!this.forType.IsClass && !this.forType.IsInterface)
				{
					return false;
				}
				return !this.forType.IsSealed;
			}
		}

		public Type ExpectedType
		{
			get
			{
				return this.forType;
			}
		}

		bool ProtoBuf.Serializers.IProtoSerializer.RequiresOldValue
		{
			get
			{
				return true;
			}
		}

		bool ProtoBuf.Serializers.IProtoSerializer.ReturnsValue
		{
			get
			{
				return false;
			}
		}

		static TypeSerializer()
		{
			TypeSerializer.iextensible = typeof(IExtensible);
		}

		public TypeSerializer(TypeModel model, Type forType, int[] fieldNumbers, IProtoSerializer[] serializers, MethodInfo[] baseCtorCallbacks, bool isRootType, bool useConstructor, CallbackSet callbacks, Type constructType, MethodInfo factory)
		{
			Helpers.Sort(fieldNumbers, serializers);
			bool flag = false;
			for (int i = 1; i < (int)fieldNumbers.Length; i++)
			{
				if (fieldNumbers[i] == fieldNumbers[i - 1])
				{
					throw new InvalidOperationException(string.Concat("Duplicate field-number detected; ", fieldNumbers[i].ToString(), " on: ", forType.FullName));
				}
				if (!flag && serializers[i].ExpectedType != forType)
				{
					flag = true;
				}
			}
			this.forType = forType;
			this.factory = factory;
			if (constructType == null)
			{
				constructType = forType;
			}
			else if (!forType.IsAssignableFrom(constructType))
			{
				throw new InvalidOperationException(string.Concat(forType.FullName, " cannot be assigned from ", constructType.FullName));
			}
			this.constructType = constructType;
			this.serializers = serializers;
			this.fieldNumbers = fieldNumbers;
			this.callbacks = callbacks;
			this.isRootType = isRootType;
			this.useConstructor = useConstructor;
			if (baseCtorCallbacks != null && (int)baseCtorCallbacks.Length == 0)
			{
				baseCtorCallbacks = null;
			}
			this.baseCtorCallbacks = baseCtorCallbacks;
			if (Helpers.GetUnderlyingType(forType) != null)
			{
				throw new ArgumentException("Cannot create a TypeSerializer for nullable types", "forType");
			}
			if (model.MapType(TypeSerializer.iextensible).IsAssignableFrom(forType))
			{
				if (forType.IsValueType || !isRootType || flag)
				{
					throw new NotSupportedException("IExtensible is not supported in structs or classes with inheritance");
				}
				this.isExtensible = true;
			}
			this.hasConstructor = (constructType.IsAbstract ? false : Helpers.GetConstructor(constructType, Helpers.EmptyTypes, true) != null);
			if (constructType != forType && useConstructor && !this.hasConstructor)
			{
				throw new ArgumentException(string.Concat("The supplied default implementation cannot be created: ", constructType.FullName), "constructType");
			}
		}

		public void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
			if (this.callbacks != null)
			{
				this.InvokeCallback(this.callbacks[callbackType], value, context);
			}
			IProtoTypeSerializer moreSpecificSerializer = (IProtoTypeSerializer)this.GetMoreSpecificSerializer(value);
			if (moreSpecificSerializer != null)
			{
				moreSpecificSerializer.Callback(value, callbackType, context);
			}
		}

		private object CreateInstance(ProtoReader source, bool includeLocalCallback)
		{
			object uninitializedObject;
			if (this.factory != null)
			{
				uninitializedObject = this.InvokeCallback(this.factory, null, source.Context);
			}
			else if (!this.useConstructor)
			{
				uninitializedObject = BclHelpers.GetUninitializedObject(this.constructType);
			}
			else
			{
				if (!this.hasConstructor)
				{
					TypeModel.ThrowCannotCreateInstance(this.constructType);
				}
				uninitializedObject = Activator.CreateInstance(this.constructType, true);
			}
			ProtoReader.NoteObject(uninitializedObject, source);
			if (this.baseCtorCallbacks != null)
			{
				for (int i = 0; i < (int)this.baseCtorCallbacks.Length; i++)
				{
					this.InvokeCallback(this.baseCtorCallbacks[i], uninitializedObject, source.Context);
				}
			}
			if (includeLocalCallback && this.callbacks != null)
			{
				this.InvokeCallback(this.callbacks.BeforeDeserialize, uninitializedObject, source.Context);
			}
			return uninitializedObject;
		}

		private void EmitCallbackIfNeeded(CompilerContext ctx, Local valueFrom, TypeModel.CallbackType callbackType)
		{
			if (this.isRootType && ((IProtoTypeSerializer)this).HasCallbacks(callbackType))
			{
				((IProtoTypeSerializer)this).EmitCallback(ctx, valueFrom, callbackType);
			}
		}

		private void EmitCreateIfNull(CompilerContext ctx, Local storage)
		{
			if (!this.ExpectedType.IsValueType)
			{
				CodeLabel codeLabel = ctx.DefineLabel();
				ctx.LoadValue(storage);
				ctx.BranchIfTrue(codeLabel, false);
				((IProtoTypeSerializer)this).EmitCreateInstance(ctx);
				if (this.callbacks != null)
				{
					TypeSerializer.EmitInvokeCallback(ctx, this.callbacks.BeforeDeserialize, true, null, this.forType);
				}
				ctx.StoreValue(storage);
				ctx.MarkLabel(codeLabel);
			}
		}

		private static void EmitInvokeCallback(CompilerContext ctx, MethodInfo method, bool copyValue, Type constructType, Type type)
		{
			if (method != null)
			{
				if (copyValue)
				{
					ctx.CopyValue();
				}
				ParameterInfo[] parameters = method.GetParameters();
				bool flag = true;
				for (int i = 0; i < (int)parameters.Length; i++)
				{
					Type parameterType = parameters[0].ParameterType;
					if (parameterType == ctx.MapType(typeof(SerializationContext)))
					{
						ctx.LoadSerializationContext();
					}
					else if (parameterType == ctx.MapType(typeof(Type)))
					{
						ctx.LoadValue(constructType ?? type);
					}
					else if (parameterType != ctx.MapType(typeof(StreamingContext)))
					{
						flag = false;
					}
					else
					{
						ctx.LoadSerializationContext();
						Type type1 = ctx.MapType(typeof(SerializationContext));
						Type[] typeArray = new Type[] { ctx.MapType(typeof(SerializationContext)) };
						MethodInfo methodInfo = type1.GetMethod("op_Implicit", typeArray);
						if (methodInfo != null)
						{
							ctx.EmitCall(methodInfo);
							flag = true;
						}
					}
				}
				if (!flag)
				{
					throw CallbackSet.CreateInvalidCallbackSignature(method);
				}
				ctx.EmitCall(method);
				if (constructType != null && method.ReturnType == ctx.MapType(typeof(object)))
				{
					ctx.CastFromObject(type);
					return;
				}
			}
		}

		private IProtoSerializer GetMoreSpecificSerializer(object value)
		{
			if (!this.CanHaveInheritance)
			{
				return null;
			}
			Type type = value.GetType();
			if (type == this.forType)
			{
				return null;
			}
			for (int i = 0; i < (int)this.serializers.Length; i++)
			{
				IProtoSerializer protoSerializer = this.serializers[i];
				if (protoSerializer.ExpectedType != this.forType && Helpers.IsAssignableFrom(protoSerializer.ExpectedType, type))
				{
					return protoSerializer;
				}
			}
			if (type == this.constructType)
			{
				return null;
			}
			TypeModel.ThrowUnexpectedSubtype(this.forType, type);
			return null;
		}

		public bool HasCallbacks(TypeModel.CallbackType callbackType)
		{
			if (this.callbacks != null && this.callbacks[callbackType] != null)
			{
				return true;
			}
			for (int i = 0; i < (int)this.serializers.Length; i++)
			{
				if (this.serializers[i].ExpectedType != this.forType && ((IProtoTypeSerializer)this.serializers[i]).HasCallbacks(callbackType))
				{
					return true;
				}
			}
			return false;
		}

		private object InvokeCallback(MethodInfo method, object obj, SerializationContext context)
		{
			object[] objArray;
			bool flag;
			object obj1;
			object obj2 = null;
			if (method != null)
			{
				ParameterInfo[] parameters = method.GetParameters();
				if ((int)parameters.Length != 0)
				{
					objArray = new object[(int)parameters.Length];
					flag = true;
					for (int i = 0; i < (int)objArray.Length; i++)
					{
						Type parameterType = parameters[i].ParameterType;
						if (parameterType == typeof(SerializationContext))
						{
							obj1 = context;
						}
						else if (parameterType == typeof(Type))
						{
							obj1 = this.constructType;
						}
						else if (parameterType != typeof(StreamingContext))
						{
							obj1 = null;
							flag = false;
						}
						else
						{
							obj1 = context;
						}
						objArray[i] = obj1;
					}
				}
				else
				{
					objArray = null;
					flag = true;
				}
				if (!flag)
				{
					throw CallbackSet.CreateInvalidCallbackSignature(method);
				}
				obj2 = method.Invoke(obj, objArray);
			}
			return obj2;
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
		{
			Type expectedType = this.ExpectedType;
			using (Local localWithValue = ctx.GetLocalWithValue(expectedType, valueFrom))
			{
				using (Local local = new Local(ctx, ctx.MapType(typeof(int))))
				{
					if (this.HasCallbacks(TypeModel.CallbackType.BeforeDeserialize))
					{
						if (!this.ExpectedType.IsValueType)
						{
							CodeLabel codeLabel = ctx.DefineLabel();
							ctx.LoadValue(localWithValue);
							ctx.BranchIfFalse(codeLabel, false);
							this.EmitCallbackIfNeeded(ctx, localWithValue, TypeModel.CallbackType.BeforeDeserialize);
							ctx.MarkLabel(codeLabel);
						}
						else
						{
							this.EmitCallbackIfNeeded(ctx, localWithValue, TypeModel.CallbackType.BeforeDeserialize);
						}
					}
					CodeLabel codeLabel1 = ctx.DefineLabel();
					CodeLabel codeLabel2 = ctx.DefineLabel();
					ctx.Branch(codeLabel1, false);
					ctx.MarkLabel(codeLabel2);
					BasicList.NodeEnumerator enumerator = BasicList.GetContiguousGroups(this.fieldNumbers, this.serializers).GetEnumerator();
					while (enumerator.MoveNext())
					{
						BasicList.Group current = (BasicList.Group)enumerator.Current;
						CodeLabel codeLabel3 = ctx.DefineLabel();
						int count = current.Items.Count;
						if (count != 1)
						{
							ctx.LoadValue(local);
							ctx.LoadValue(current.First);
							ctx.Subtract();
							CodeLabel[] codeLabelArray = new CodeLabel[count];
							for (int i = 0; i < count; i++)
							{
								codeLabelArray[i] = ctx.DefineLabel();
							}
							ctx.Switch(codeLabelArray);
							ctx.Branch(codeLabel3, false);
							for (int j = 0; j < count; j++)
							{
								this.WriteFieldHandler(ctx, expectedType, localWithValue, codeLabelArray[j], codeLabel1, (IProtoSerializer)current.Items[j]);
							}
						}
						else
						{
							ctx.LoadValue(local);
							ctx.LoadValue(current.First);
							CodeLabel codeLabel4 = ctx.DefineLabel();
							ctx.BranchIfEqual(codeLabel4, true);
							ctx.Branch(codeLabel3, false);
							this.WriteFieldHandler(ctx, expectedType, localWithValue, codeLabel4, codeLabel1, (IProtoSerializer)current.Items[0]);
						}
						ctx.MarkLabel(codeLabel3);
					}
					this.EmitCreateIfNull(ctx, localWithValue);
					ctx.LoadReaderWriter();
					if (!this.isExtensible)
					{
						ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("SkipField"));
					}
					else
					{
						ctx.LoadValue(localWithValue);
						ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("AppendExtensionData"));
					}
					ctx.MarkLabel(codeLabel1);
					ctx.EmitBasicRead("ReadFieldHeader", ctx.MapType(typeof(int)));
					ctx.CopyValue();
					ctx.StoreValue(local);
					ctx.LoadValue(0);
					ctx.BranchIfGreater(codeLabel2, false);
					this.EmitCreateIfNull(ctx, localWithValue);
					this.EmitCallbackIfNeeded(ctx, localWithValue, TypeModel.CallbackType.AfterDeserialize);
					if (valueFrom != null && !localWithValue.IsSame(valueFrom))
					{
						ctx.LoadValue(localWithValue);
						ctx.Cast(valueFrom.Type);
						ctx.StoreValue(valueFrom);
					}
				}
			}
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			using (Local localWithValue = ctx.GetLocalWithValue(this.ExpectedType, valueFrom))
			{
				this.EmitCallbackIfNeeded(ctx, localWithValue, TypeModel.CallbackType.BeforeSerialize);
				CodeLabel codeLabel = ctx.DefineLabel();
				if (this.CanHaveInheritance)
				{
					for (int i = 0; i < (int)this.serializers.Length; i++)
					{
						IProtoSerializer protoSerializer = this.serializers[i];
						Type expectedType = protoSerializer.ExpectedType;
						if (expectedType != this.forType)
						{
							CodeLabel codeLabel1 = ctx.DefineLabel();
							CodeLabel codeLabel2 = ctx.DefineLabel();
							ctx.LoadValue(localWithValue);
							ctx.TryCast(expectedType);
							ctx.CopyValue();
							ctx.BranchIfTrue(codeLabel1, true);
							ctx.DiscardValue();
							ctx.Branch(codeLabel2, true);
							ctx.MarkLabel(codeLabel1);
							protoSerializer.EmitWrite(ctx, null);
							ctx.Branch(codeLabel, false);
							ctx.MarkLabel(codeLabel2);
						}
					}
					if (this.constructType == null || this.constructType == this.forType)
					{
						ctx.LoadValue(localWithValue);
						ctx.EmitCall(ctx.MapType(typeof(object)).GetMethod("GetType"));
						ctx.LoadValue(this.forType);
						ctx.BranchIfEqual(codeLabel, true);
					}
					else
					{
						using (Local local = new Local(ctx, ctx.MapType(typeof(Type))))
						{
							ctx.LoadValue(localWithValue);
							ctx.EmitCall(ctx.MapType(typeof(object)).GetMethod("GetType"));
							ctx.CopyValue();
							ctx.StoreValue(local);
							ctx.LoadValue(this.forType);
							ctx.BranchIfEqual(codeLabel, true);
							ctx.LoadValue(local);
							ctx.LoadValue(this.constructType);
							ctx.BranchIfEqual(codeLabel, true);
						}
					}
					ctx.LoadValue(this.forType);
					ctx.LoadValue(localWithValue);
					ctx.EmitCall(ctx.MapType(typeof(object)).GetMethod("GetType"));
					ctx.EmitCall(ctx.MapType(typeof(TypeModel)).GetMethod("ThrowUnexpectedSubtype", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
				}
				ctx.MarkLabel(codeLabel);
				for (int j = 0; j < (int)this.serializers.Length; j++)
				{
					IProtoSerializer protoSerializer1 = this.serializers[j];
					if (protoSerializer1.ExpectedType == this.forType)
					{
						protoSerializer1.EmitWrite(ctx, localWithValue);
					}
				}
				if (this.isExtensible)
				{
					ctx.LoadValue(localWithValue);
					ctx.LoadReaderWriter();
					ctx.EmitCall(ctx.MapType(typeof(ProtoWriter)).GetMethod("AppendExtensionData"));
				}
				this.EmitCallbackIfNeeded(ctx, localWithValue, TypeModel.CallbackType.AfterSerialize);
			}
		}

		bool ProtoBuf.Serializers.IProtoTypeSerializer.CanCreateInstance()
		{
			return true;
		}

		object ProtoBuf.Serializers.IProtoTypeSerializer.CreateInstance(ProtoReader source)
		{
			return this.CreateInstance(source, false);
		}

		void ProtoBuf.Serializers.IProtoTypeSerializer.EmitCallback(CompilerContext ctx, Local valueFrom, TypeModel.CallbackType callbackType)
		{
			MethodInfo item;
			bool flag = false;
			if (this.CanHaveInheritance)
			{
				for (int i = 0; i < (int)this.serializers.Length; i++)
				{
					IProtoSerializer protoSerializer = this.serializers[i];
					if (protoSerializer.ExpectedType != this.forType && ((IProtoTypeSerializer)protoSerializer).HasCallbacks(callbackType))
					{
						flag = true;
					}
				}
			}
			if (this.callbacks == null)
			{
				item = null;
			}
			else
			{
				item = this.callbacks[callbackType];
			}
			MethodInfo methodInfo = item;
			if (methodInfo == null && !flag)
			{
				return;
			}
			ctx.LoadAddress(valueFrom, this.ExpectedType);
			TypeSerializer.EmitInvokeCallback(ctx, methodInfo, flag, null, this.forType);
			if (flag)
			{
				CodeLabel codeLabel = ctx.DefineLabel();
				for (int j = 0; j < (int)this.serializers.Length; j++)
				{
					IProtoSerializer protoSerializer1 = this.serializers[j];
					Type expectedType = protoSerializer1.ExpectedType;
					if (expectedType != this.forType)
					{
						IProtoTypeSerializer protoTypeSerializer = (IProtoTypeSerializer)protoSerializer1;
						IProtoTypeSerializer protoTypeSerializer1 = protoTypeSerializer;
						if (protoTypeSerializer.HasCallbacks(callbackType))
						{
							CodeLabel codeLabel1 = ctx.DefineLabel();
							CodeLabel codeLabel2 = ctx.DefineLabel();
							ctx.CopyValue();
							ctx.TryCast(expectedType);
							ctx.CopyValue();
							ctx.BranchIfTrue(codeLabel1, true);
							ctx.DiscardValue();
							ctx.Branch(codeLabel2, false);
							ctx.MarkLabel(codeLabel1);
							protoTypeSerializer1.EmitCallback(ctx, null, callbackType);
							ctx.Branch(codeLabel, false);
							ctx.MarkLabel(codeLabel2);
						}
					}
				}
				ctx.MarkLabel(codeLabel);
				ctx.DiscardValue();
			}
		}

		void ProtoBuf.Serializers.IProtoTypeSerializer.EmitCreateInstance(CompilerContext ctx)
		{
			bool flag = true;
			if (this.factory != null)
			{
				TypeSerializer.EmitInvokeCallback(ctx, this.factory, false, this.constructType, this.forType);
			}
			else if (!this.useConstructor)
			{
				ctx.LoadValue(this.constructType);
				ctx.EmitCall(ctx.MapType(typeof(BclHelpers)).GetMethod("GetUninitializedObject"));
				ctx.Cast(this.forType);
			}
			else if (!this.constructType.IsClass || !this.hasConstructor)
			{
				ctx.LoadValue(this.ExpectedType);
				ctx.EmitCall(ctx.MapType(typeof(TypeModel)).GetMethod("ThrowCannotCreateInstance", BindingFlags.Static | BindingFlags.Public));
				ctx.LoadNullRef();
				flag = false;
			}
			else
			{
				ctx.EmitCtor(this.constructType);
			}
			if (flag)
			{
				ctx.CopyValue();
				ctx.LoadReaderWriter();
				ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("NoteObject", BindingFlags.Static | BindingFlags.Public));
			}
			if (this.baseCtorCallbacks != null)
			{
				for (int i = 0; i < (int)this.baseCtorCallbacks.Length; i++)
				{
					TypeSerializer.EmitInvokeCallback(ctx, this.baseCtorCallbacks[i], true, null, this.forType);
				}
			}
		}

		public object Read(object value, ProtoReader source)
		{
			if (this.isRootType && value != null)
			{
				this.Callback(value, TypeModel.CallbackType.BeforeDeserialize, source.Context);
			}
			int num = 0;
			int num1 = 0;
			while (true)
			{
				int num2 = source.ReadFieldHeader();
				int num3 = num2;
				if (num2 <= 0)
				{
					break;
				}
				bool flag = false;
				if (num3 < num)
				{
					int num4 = 0;
					num1 = num4;
					num = num4;
				}
				int num5 = num1;
				while (num5 < (int)this.fieldNumbers.Length)
				{
					if (this.fieldNumbers[num5] != num3)
					{
						num5++;
					}
					else
					{
						IProtoSerializer protoSerializer = this.serializers[num5];
						Type expectedType = protoSerializer.ExpectedType;
						if (value == null)
						{
							if (expectedType == this.forType)
							{
								value = this.CreateInstance(source, true);
							}
						}
						else if (expectedType != this.forType && ((IProtoTypeSerializer)protoSerializer).CanCreateInstance() && expectedType.IsSubclassOf(value.GetType()))
						{
							value = ProtoReader.Merge(source, value, ((IProtoTypeSerializer)protoSerializer).CreateInstance(source));
						}
						if (!protoSerializer.ReturnsValue)
						{
							protoSerializer.Read(value, source);
						}
						else
						{
							value = protoSerializer.Read(value, source);
						}
						num1 = num5;
						num = num3;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (value == null)
					{
						value = this.CreateInstance(source, true);
					}
					if (!this.isExtensible)
					{
						source.SkipField();
					}
					else
					{
						source.AppendExtensionData((IExtensible)value);
					}
				}
			}
			if (value == null)
			{
				value = this.CreateInstance(source, true);
			}
			if (this.isRootType)
			{
				this.Callback(value, TypeModel.CallbackType.AfterDeserialize, source.Context);
			}
			return value;
		}

		public void Write(object value, ProtoWriter dest)
		{
			if (this.isRootType)
			{
				this.Callback(value, TypeModel.CallbackType.BeforeSerialize, dest.Context);
			}
			IProtoSerializer moreSpecificSerializer = this.GetMoreSpecificSerializer(value);
			if (moreSpecificSerializer != null)
			{
				moreSpecificSerializer.Write(value, dest);
			}
			for (int i = 0; i < (int)this.serializers.Length; i++)
			{
				IProtoSerializer protoSerializer = this.serializers[i];
				if (protoSerializer.ExpectedType == this.forType)
				{
					protoSerializer.Write(value, dest);
				}
			}
			if (this.isExtensible)
			{
				ProtoWriter.AppendExtensionData((IExtensible)value, dest);
			}
			if (this.isRootType)
			{
				this.Callback(value, TypeModel.CallbackType.AfterSerialize, dest.Context);
			}
		}

		private void WriteFieldHandler(CompilerContext ctx, Type expected, Local loc, CodeLabel handler, CodeLabel @continue, IProtoSerializer serializer)
		{
			ctx.MarkLabel(handler);
			Type expectedType = serializer.ExpectedType;
			if (expectedType != this.forType)
			{
				if (((IProtoTypeSerializer)serializer).CanCreateInstance())
				{
					CodeLabel codeLabel = ctx.DefineLabel();
					ctx.LoadValue(loc);
					ctx.BranchIfFalse(codeLabel, false);
					ctx.LoadValue(loc);
					ctx.TryCast(expectedType);
					ctx.BranchIfTrue(codeLabel, false);
					ctx.LoadReaderWriter();
					ctx.LoadValue(loc);
					((IProtoTypeSerializer)serializer).EmitCreateInstance(ctx);
					ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("Merge"));
					ctx.Cast(expected);
					ctx.StoreValue(loc);
					ctx.MarkLabel(codeLabel);
				}
				ctx.LoadValue(loc);
				ctx.Cast(expectedType);
				serializer.EmitRead(ctx, null);
			}
			else
			{
				this.EmitCreateIfNull(ctx, loc);
				serializer.EmitRead(ctx, loc);
			}
			if (serializer.ReturnsValue)
			{
				ctx.StoreValue(loc);
			}
			ctx.Branch(@continue, false);
		}
	}
}