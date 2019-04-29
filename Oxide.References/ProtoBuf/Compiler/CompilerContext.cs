using ProtoBuf;
using ProtoBuf.Meta;
using ProtoBuf.Serializers;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ProtoBuf.Compiler
{
	internal sealed class CompilerContext
	{
		private readonly DynamicMethod method;

		private static int next;

		private readonly bool isStatic;

		private readonly RuntimeTypeModel.SerializerPair[] methodPairs;

		private readonly bool isWriter;

		private readonly bool nonPublic;

		private readonly Local inputValue;

		private readonly string assemblyName;

		private readonly ILGenerator il;

		private MutableList locals = new MutableList();

		private int nextLabel;

		private BasicList knownTrustedAssemblies;

		private BasicList knownUntrustedAssemblies;

		private readonly TypeModel model;

		private readonly CompilerContext.ILVersion metadataVersion;

		public Local InputValue
		{
			get
			{
				return this.inputValue;
			}
		}

		public CompilerContext.ILVersion MetadataVersion
		{
			get
			{
				return this.metadataVersion;
			}
		}

		public TypeModel Model
		{
			get
			{
				return this.model;
			}
		}

		internal bool NonPublic
		{
			get
			{
				return this.nonPublic;
			}
		}

		internal CompilerContext(ILGenerator il, bool isStatic, bool isWriter, RuntimeTypeModel.SerializerPair[] methodPairs, TypeModel model, CompilerContext.ILVersion metadataVersion, string assemblyName, Type inputType)
		{
			if (il == null)
			{
				throw new ArgumentNullException("il");
			}
			if (methodPairs == null)
			{
				throw new ArgumentNullException("methodPairs");
			}
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			if (Helpers.IsNullOrEmpty(assemblyName))
			{
				throw new ArgumentNullException("assemblyName");
			}
			this.assemblyName = assemblyName;
			this.isStatic = isStatic;
			this.methodPairs = methodPairs;
			this.il = il;
			this.isWriter = isWriter;
			this.model = model;
			this.metadataVersion = metadataVersion;
			if (inputType != null)
			{
				this.inputValue = new Local((CompilerContext)null, inputType);
			}
		}

		private CompilerContext(Type associatedType, bool isWriter, bool isStatic, TypeModel model, Type inputType)
		{
			Type[] typeArray;
			Type type;
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			this.metadataVersion = CompilerContext.ILVersion.Net2;
			this.isStatic = isStatic;
			this.isWriter = isWriter;
			this.model = model;
			this.nonPublic = true;
			if (!isWriter)
			{
				type = typeof(object);
				typeArray = new Type[] { typeof(object), typeof(ProtoReader) };
			}
			else
			{
				type = typeof(void);
				typeArray = new Type[] { typeof(object), typeof(ProtoWriter) };
			}
			int num = CompilerContext.next + 1;
			CompilerContext.next = num;
			int num1 = num;
			this.method = new DynamicMethod(string.Concat("proto_", num1.ToString()), type, typeArray, (associatedType.IsInterface ? typeof(object) : associatedType), true);
			this.il = this.method.GetILGenerator();
			if (inputType != null)
			{
				this.inputValue = new Local((CompilerContext)null, inputType);
			}
		}

		internal void Add()
		{
			this.Emit(OpCodes.Add);
		}

		internal bool AllowInternal(PropertyInfo property)
		{
			if (this.NonPublic)
			{
				return true;
			}
			return this.InternalsVisible(property.DeclaringType.Assembly);
		}

		internal void BeginFinally()
		{
			this.il.BeginFinallyBlock();
		}

		internal CodeLabel BeginTry()
		{
			Label label = this.il.BeginExceptionBlock();
			CompilerContext compilerContext = this;
			int num = compilerContext.nextLabel;
			int num1 = num;
			compilerContext.nextLabel = num + 1;
			return new CodeLabel(label, num1);
		}

		internal void Branch(CodeLabel label, bool @short)
		{
			OpCode opCode;
			opCode = (@short ? OpCodes.Br_S : OpCodes.Br);
			this.il.Emit(opCode, label.Value);
		}

		internal void BranchIfEqual(CodeLabel label, bool @short)
		{
			OpCode opCode;
			opCode = (@short ? OpCodes.Beq_S : OpCodes.Beq);
			this.il.Emit(opCode, label.Value);
		}

		internal void BranchIfFalse(CodeLabel label, bool @short)
		{
			OpCode opCode;
			opCode = (@short ? OpCodes.Brfalse_S : OpCodes.Brfalse);
			this.il.Emit(opCode, label.Value);
		}

		internal void BranchIfGreater(CodeLabel label, bool @short)
		{
			OpCode opCode;
			opCode = (@short ? OpCodes.Bgt_S : OpCodes.Bgt);
			this.il.Emit(opCode, label.Value);
		}

		internal void BranchIfLess(CodeLabel label, bool @short)
		{
			OpCode opCode;
			opCode = (@short ? OpCodes.Blt_S : OpCodes.Blt);
			this.il.Emit(opCode, label.Value);
		}

		internal void BranchIfTrue(CodeLabel label, bool @short)
		{
			OpCode opCode;
			opCode = (@short ? OpCodes.Brtrue_S : OpCodes.Brtrue);
			this.il.Emit(opCode, label.Value);
		}

		public static ProtoDeserializer BuildDeserializer(IProtoSerializer head, TypeModel model)
		{
			Type expectedType = head.ExpectedType;
			CompilerContext compilerContext = new CompilerContext(expectedType, false, true, model, typeof(object));
			using (Local local = new Local(compilerContext, expectedType))
			{
				if (expectedType.IsValueType)
				{
					compilerContext.LoadValue(compilerContext.InputValue);
					CodeLabel codeLabel = compilerContext.DefineLabel();
					CodeLabel codeLabel1 = compilerContext.DefineLabel();
					compilerContext.BranchIfTrue(codeLabel, true);
					compilerContext.LoadAddress(local, expectedType);
					compilerContext.EmitCtor(expectedType);
					compilerContext.Branch(codeLabel1, true);
					compilerContext.MarkLabel(codeLabel);
					compilerContext.LoadValue(compilerContext.InputValue);
					compilerContext.CastFromObject(expectedType);
					compilerContext.StoreValue(local);
					compilerContext.MarkLabel(codeLabel1);
				}
				else
				{
					compilerContext.LoadValue(compilerContext.InputValue);
					compilerContext.CastFromObject(expectedType);
					compilerContext.StoreValue(local);
				}
				head.EmitRead(compilerContext, local);
				if (head.ReturnsValue)
				{
					compilerContext.StoreValue(local);
				}
				compilerContext.LoadValue(local);
				compilerContext.CastToObject(expectedType);
			}
			compilerContext.Emit(OpCodes.Ret);
			return (ProtoDeserializer)compilerContext.method.CreateDelegate(typeof(ProtoDeserializer));
		}

		public static ProtoSerializer BuildSerializer(IProtoSerializer head, TypeModel model)
		{
			ProtoSerializer protoSerializer;
			Type expectedType = head.ExpectedType;
			try
			{
				CompilerContext compilerContext = new CompilerContext(expectedType, true, true, model, typeof(object));
				compilerContext.LoadValue(compilerContext.InputValue);
				compilerContext.CastFromObject(expectedType);
				compilerContext.WriteNullCheckedTail(expectedType, head, null);
				compilerContext.Emit(OpCodes.Ret);
				protoSerializer = (ProtoSerializer)compilerContext.method.CreateDelegate(typeof(ProtoSerializer));
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string fullName = expectedType.FullName;
				if (string.IsNullOrEmpty(fullName))
				{
					fullName = expectedType.Name;
				}
				throw new InvalidOperationException(string.Concat("It was not possible to prepare a serializer for: ", fullName), exception);
			}
			return protoSerializer;
		}

		internal void Cast(Type type)
		{
			this.il.Emit(OpCodes.Castclass, type);
		}

		internal void CastFromObject(Type type)
		{
			if (CompilerContext.IsObject(type))
			{
				return;
			}
			if (!type.IsValueType)
			{
				this.il.Emit(OpCodes.Castclass, type);
				return;
			}
			if (this.MetadataVersion != CompilerContext.ILVersion.Net1)
			{
				this.il.Emit(OpCodes.Unbox_Any, type);
				return;
			}
			this.il.Emit(OpCodes.Unbox, type);
			this.il.Emit(OpCodes.Ldobj, type);
		}

		internal void CastToObject(Type type)
		{
			if (CompilerContext.IsObject(type))
			{
				return;
			}
			if (type.IsValueType)
			{
				this.il.Emit(OpCodes.Box, type);
				return;
			}
			this.il.Emit(OpCodes.Castclass, this.MapType(typeof(object)));
		}

		internal void CheckAccessibility(MemberInfo member)
		{
			Type type;
			bool flag;
			bool flag1;
			Type declaringType;
			bool flag2;
			bool flag3;
			bool flag4;
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			MemberTypes memberType = member.MemberType;
			if (!this.NonPublic)
			{
				MemberTypes memberType1 = memberType;
				if (memberType1 <= MemberTypes.Method)
				{
					if (memberType1 == MemberTypes.Constructor)
					{
						ConstructorInfo constructorInfo = (ConstructorInfo)member;
						if (constructorInfo.IsPublic)
						{
							flag2 = true;
						}
						else
						{
							flag2 = (constructorInfo.IsAssembly || constructorInfo.IsFamilyOrAssembly ? this.InternalsVisible(constructorInfo.DeclaringType.Assembly) : false);
						}
						flag = flag2;
					}
					else if (memberType1 == MemberTypes.Field)
					{
						FieldInfo fieldInfo = (FieldInfo)member;
						if (fieldInfo.IsPublic)
						{
							flag3 = true;
						}
						else
						{
							flag3 = (fieldInfo.IsAssembly || fieldInfo.IsFamilyOrAssembly ? this.InternalsVisible(fieldInfo.DeclaringType.Assembly) : false);
						}
						flag = flag3;
					}
					else
					{
						if (memberType1 != MemberTypes.Method)
						{
							throw new NotSupportedException(memberType.ToString());
						}
						MethodInfo methodInfo = (MethodInfo)member;
						if (methodInfo.IsPublic)
						{
							flag4 = true;
						}
						else
						{
							flag4 = (methodInfo.IsAssembly || methodInfo.IsFamilyOrAssembly ? this.InternalsVisible(methodInfo.DeclaringType.Assembly) : false);
						}
						flag = flag4;
						if (!flag && (member is MethodBuilder || member.DeclaringType == this.MapType(typeof(TypeModel))))
						{
							flag = true;
						}
					}
				}
				else if (memberType1 == MemberTypes.Property)
				{
					flag = true;
				}
				else if (memberType1 == MemberTypes.TypeInfo)
				{
					type = (Type)member;
					flag = (type.IsPublic ? true : this.InternalsVisible(type.Assembly));
				}
				else
				{
					if (memberType1 != MemberTypes.NestedType)
					{
						throw new NotSupportedException(memberType.ToString());
					}
					type = (Type)member;
					do
					{
						if (type.IsNestedPublic || type.IsPublic)
						{
							flag1 = true;
						}
						else
						{
							flag1 = (type.DeclaringType == null || type.IsNestedAssembly || type.IsNestedFamORAssem ? this.InternalsVisible(type.Assembly) : false);
						}
						flag = flag1;
						if (!flag)
						{
							goto Label2;
						}
						declaringType = type.DeclaringType;
						type = declaringType;
					}
					while (declaringType != null);
				}
			Label2:
				if (!flag)
				{
					MemberTypes memberType2 = memberType;
					if (memberType2 == MemberTypes.TypeInfo || memberType2 == MemberTypes.NestedType)
					{
						throw new InvalidOperationException(string.Concat("Non-public type cannot be used with full dll compilation: ", ((Type)member).FullName));
					}
					throw new InvalidOperationException(string.Concat("Non-public member cannot be used with full dll compilation: ", member.DeclaringType.FullName, ".", member.Name));
				}
			}
		}

		internal void Constrain(Type type)
		{
			this.il.Emit(OpCodes.Constrained, type);
		}

		internal void ConvertFromInt32(ProtoTypeCode typeCode, bool uint32Overflow)
		{
			switch (typeCode)
			{
				case ProtoTypeCode.SByte:
				{
					this.Emit(OpCodes.Conv_Ovf_I1);
					return;
				}
				case ProtoTypeCode.Byte:
				{
					this.Emit(OpCodes.Conv_Ovf_U1);
					return;
				}
				case ProtoTypeCode.Int16:
				{
					this.Emit(OpCodes.Conv_Ovf_I2);
					return;
				}
				case ProtoTypeCode.UInt16:
				{
					this.Emit(OpCodes.Conv_Ovf_U2);
					return;
				}
				case ProtoTypeCode.Int32:
				{
					return;
				}
				case ProtoTypeCode.UInt32:
				{
					this.Emit((uint32Overflow ? OpCodes.Conv_Ovf_U4 : OpCodes.Conv_U4));
					return;
				}
				case ProtoTypeCode.Int64:
				{
					this.Emit(OpCodes.Conv_I8);
					return;
				}
				case ProtoTypeCode.UInt64:
				{
					this.Emit(OpCodes.Conv_U8);
					return;
				}
			}
			throw new InvalidOperationException();
		}

		internal void ConvertToInt32(ProtoTypeCode typeCode, bool uint32Overflow)
		{
			switch (typeCode)
			{
				case ProtoTypeCode.SByte:
				case ProtoTypeCode.Byte:
				case ProtoTypeCode.Int16:
				case ProtoTypeCode.UInt16:
				{
					this.Emit(OpCodes.Conv_I4);
					return;
				}
				case ProtoTypeCode.Int32:
				{
					return;
				}
				case ProtoTypeCode.UInt32:
				{
					this.Emit((uint32Overflow ? OpCodes.Conv_Ovf_I4_Un : OpCodes.Conv_Ovf_I4));
					return;
				}
				case ProtoTypeCode.Int64:
				{
					this.Emit(OpCodes.Conv_Ovf_I4);
					return;
				}
				case ProtoTypeCode.UInt64:
				{
					this.Emit(OpCodes.Conv_Ovf_I4_Un);
					return;
				}
			}
			throw new InvalidOperationException(string.Concat("ConvertToInt32 not implemented for: ", typeCode.ToString()));
		}

		internal void CopyValue()
		{
			this.Emit(OpCodes.Dup);
		}

		internal void CreateArray(Type elementType, Local length)
		{
			this.LoadValue(length);
			this.il.Emit(OpCodes.Newarr, elementType);
		}

		internal CodeLabel DefineLabel()
		{
			Label label = this.il.DefineLabel();
			CompilerContext compilerContext = this;
			int num = compilerContext.nextLabel;
			int num1 = num;
			compilerContext.nextLabel = num + 1;
			return new CodeLabel(label, num1);
		}

		internal void DiscardValue()
		{
			this.Emit(OpCodes.Pop);
		}

		private void Emit(OpCode opcode)
		{
			this.il.Emit(opcode);
		}

		internal void EmitBasicRead(string methodName, Type expectedType)
		{
			MethodInfo method = this.MapType(typeof(ProtoReader)).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null || method.ReturnType != expectedType || (int)method.GetParameters().Length != 0)
			{
				throw new ArgumentException("methodName");
			}
			this.LoadReaderWriter();
			this.EmitCall(method);
		}

		internal void EmitBasicRead(Type helperType, string methodName, Type expectedType)
		{
			MethodInfo method = helperType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null || method.ReturnType != expectedType || (int)method.GetParameters().Length != 1)
			{
				throw new ArgumentException("methodName");
			}
			this.LoadReaderWriter();
			this.EmitCall(method);
		}

		internal void EmitBasicWrite(string methodName, Local fromValue)
		{
			if (Helpers.IsNullOrEmpty(methodName))
			{
				throw new ArgumentNullException("methodName");
			}
			this.LoadValue(fromValue);
			this.LoadReaderWriter();
			this.EmitCall(this.GetWriterMethod(methodName));
		}

		public void EmitCall(MethodInfo method)
		{
			OpCode opCode;
			this.CheckAccessibility(method);
			opCode = (method.IsStatic || method.DeclaringType.IsValueType ? OpCodes.Call : OpCodes.Callvirt);
			this.il.EmitCall(opCode, method, null);
		}

		public void EmitCtor(Type type)
		{
			this.EmitCtor(type, Helpers.EmptyTypes);
		}

		public void EmitCtor(ConstructorInfo ctor)
		{
			if (ctor == null)
			{
				throw new ArgumentNullException("ctor");
			}
			this.CheckAccessibility(ctor);
			this.il.Emit(OpCodes.Newobj, ctor);
		}

		public void EmitCtor(Type type, params Type[] parameterTypes)
		{
			if (type.IsValueType && (int)parameterTypes.Length == 0)
			{
				this.il.Emit(OpCodes.Initobj, type);
				return;
			}
			ConstructorInfo constructor = Helpers.GetConstructor(type, parameterTypes, true);
			if (constructor == null)
			{
				throw new InvalidOperationException(string.Concat("No suitable constructor found for ", type.FullName));
			}
			this.EmitCtor(constructor);
		}

		internal void EmitWrite(Type helperType, string methodName, Local valueFrom)
		{
			if (Helpers.IsNullOrEmpty(methodName))
			{
				throw new ArgumentNullException("methodName");
			}
			MethodInfo method = helperType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null || method.ReturnType != this.MapType(typeof(void)))
			{
				throw new ArgumentException("methodName");
			}
			this.LoadValue(valueFrom);
			this.LoadReaderWriter();
			this.EmitCall(method);
		}

		internal void EndFinally()
		{
			this.il.EndExceptionBlock();
		}

		internal void EndTry(CodeLabel label, bool @short)
		{
			OpCode opCode;
			opCode = (@short ? OpCodes.Leave_S : OpCodes.Leave);
			this.il.Emit(opCode, label.Value);
		}

		internal MethodBuilder GetDedicatedMethod(int metaKey, bool read)
		{
			if (this.methodPairs == null)
			{
				return null;
			}
			for (int i = 0; i < (int)this.methodPairs.Length; i++)
			{
				if (this.methodPairs[i].MetaKey == metaKey)
				{
					if (!read)
					{
						return this.methodPairs[i].Serialize;
					}
					return this.methodPairs[i].Deserialize;
				}
			}
			throw new ArgumentException("Meta-key not found", "metaKey");
		}

		internal LocalBuilder GetFromPool(Type type)
		{
			int count = this.locals.Count;
			for (int i = 0; i < count; i++)
			{
				LocalBuilder item = (LocalBuilder)this.locals[i];
				if (item != null && item.LocalType == type)
				{
					this.locals[i] = null;
					return item;
				}
			}
			return this.il.DeclareLocal(type);
		}

		public Local GetLocalWithValue(Type type, Local fromValue)
		{
			if (fromValue != null)
			{
				if (fromValue.Type == type)
				{
					return fromValue.AsCopy();
				}
				this.LoadValue(fromValue);
				if (!type.IsValueType && (fromValue.Type == null || !type.IsAssignableFrom(fromValue.Type)))
				{
					this.Cast(type);
				}
			}
			Local local = new Local(this, type);
			this.StoreValue(local);
			return local;
		}

		private MethodInfo GetWriterMethod(string methodName)
		{
			Type type = this.MapType(typeof(ProtoWriter));
			MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < (int)methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				if (methodInfo.Name == methodName)
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if ((int)parameters.Length == 2 && parameters[1].ParameterType == type)
					{
						return methodInfo;
					}
				}
			}
			throw new ArgumentException(string.Concat("No suitable method found for: ", methodName), "methodName");
		}

		private bool InternalsVisible(Assembly assembly)
		{
			if (Helpers.IsNullOrEmpty(this.assemblyName))
			{
				return false;
			}
			if (this.knownTrustedAssemblies != null && this.knownTrustedAssemblies.IndexOfReference(assembly) >= 0)
			{
				return true;
			}
			if (this.knownUntrustedAssemblies != null && this.knownUntrustedAssemblies.IndexOfReference(assembly) >= 0)
			{
				return false;
			}
			bool flag = false;
			Type type = this.MapType(typeof(InternalsVisibleToAttribute));
			if (type == null)
			{
				return false;
			}
			object[] customAttributes = assembly.GetCustomAttributes(type, false);
			int num = 0;
			while (num < (int)customAttributes.Length)
			{
				InternalsVisibleToAttribute internalsVisibleToAttribute = (InternalsVisibleToAttribute)customAttributes[num];
				if (internalsVisibleToAttribute.AssemblyName == this.assemblyName || internalsVisibleToAttribute.AssemblyName.StartsWith(string.Concat(this.assemblyName, ",")))
				{
					flag = true;
					break;
				}
				else
				{
					num++;
				}
			}
			if (!flag)
			{
				if (this.knownUntrustedAssemblies == null)
				{
					this.knownUntrustedAssemblies = new BasicList();
				}
				this.knownUntrustedAssemblies.Add(assembly);
			}
			else
			{
				if (this.knownTrustedAssemblies == null)
				{
					this.knownTrustedAssemblies = new BasicList();
				}
				this.knownTrustedAssemblies.Add(assembly);
			}
			return flag;
		}

		private static bool IsObject(Type type)
		{
			return type == typeof(object);
		}

		internal void LoadAddress(Local local, Type type)
		{
			OpCode opCode;
			if (!type.IsValueType)
			{
				this.LoadValue(local);
				return;
			}
			if (local == null)
			{
				throw new InvalidOperationException("Cannot load the address of a struct at the head of the stack");
			}
			if (local != this.InputValue)
			{
				opCode = (this.UseShortForm(local) ? OpCodes.Ldloca_S : OpCodes.Ldloca);
				this.il.Emit(opCode, local.Value);
				return;
			}
			this.il.Emit(OpCodes.Ldarga_S, (byte)((this.isStatic ? 0 : 1)));
		}

		internal void LoadArrayValue(Local arr, Local i)
		{
			Type type = arr.Type;
			type = type.GetElementType();
			this.LoadValue(arr);
			this.LoadValue(i);
			switch (Helpers.GetTypeCode(type))
			{
				case ProtoTypeCode.SByte:
				{
					this.Emit(OpCodes.Ldelem_I1);
					return;
				}
				case ProtoTypeCode.Byte:
				{
					this.Emit(OpCodes.Ldelem_U1);
					return;
				}
				case ProtoTypeCode.Int16:
				{
					this.Emit(OpCodes.Ldelem_I2);
					return;
				}
				case ProtoTypeCode.UInt16:
				{
					this.Emit(OpCodes.Ldelem_U2);
					return;
				}
				case ProtoTypeCode.Int32:
				{
					this.Emit(OpCodes.Ldelem_I4);
					return;
				}
				case ProtoTypeCode.UInt32:
				{
					this.Emit(OpCodes.Ldelem_U4);
					return;
				}
				case ProtoTypeCode.Int64:
				{
					this.Emit(OpCodes.Ldelem_I8);
					return;
				}
				case ProtoTypeCode.UInt64:
				{
					this.Emit(OpCodes.Ldelem_I8);
					return;
				}
				case ProtoTypeCode.Single:
				{
					this.Emit(OpCodes.Ldelem_R4);
					return;
				}
				case ProtoTypeCode.Double:
				{
					this.Emit(OpCodes.Ldelem_R8);
					return;
				}
			}
			if (!type.IsValueType)
			{
				this.Emit(OpCodes.Ldelem_Ref);
				return;
			}
			this.il.Emit(OpCodes.Ldelema, type);
			this.il.Emit(OpCodes.Ldobj, type);
		}

		internal void LoadLength(Local arr, bool zeroIfNull)
		{
			if (!zeroIfNull)
			{
				this.LoadValue(arr);
				this.Emit(OpCodes.Ldlen);
				this.Emit(OpCodes.Conv_I4);
				return;
			}
			CodeLabel codeLabel = this.DefineLabel();
			CodeLabel codeLabel1 = this.DefineLabel();
			this.LoadValue(arr);
			this.CopyValue();
			this.BranchIfTrue(codeLabel, true);
			this.DiscardValue();
			this.LoadValue(0);
			this.Branch(codeLabel1, true);
			this.MarkLabel(codeLabel);
			this.Emit(OpCodes.Ldlen);
			this.Emit(OpCodes.Conv_I4);
			this.MarkLabel(codeLabel1);
		}

		public void LoadNullRef()
		{
			this.Emit(OpCodes.Ldnull);
		}

		public void LoadReaderWriter()
		{
			this.Emit((this.isStatic ? OpCodes.Ldarg_1 : OpCodes.Ldarg_2));
		}

		internal void LoadSerializationContext()
		{
			this.LoadReaderWriter();
			this.LoadValue(((this.isWriter ? typeof(ProtoWriter) : typeof(ProtoReader))).GetProperty("Context"));
		}

		public void LoadValue(string value)
		{
			if (value == null)
			{
				this.LoadNullRef();
				return;
			}
			this.il.Emit(OpCodes.Ldstr, value);
		}

		public void LoadValue(float value)
		{
			this.il.Emit(OpCodes.Ldc_R4, value);
		}

		public void LoadValue(double value)
		{
			this.il.Emit(OpCodes.Ldc_R8, value);
		}

		public void LoadValue(long value)
		{
			this.il.Emit(OpCodes.Ldc_I8, value);
		}

		public void LoadValue(int value)
		{
			switch (value)
			{
				case -1:
				{
					this.Emit(OpCodes.Ldc_I4_M1);
					return;
				}
				case 0:
				{
					this.Emit(OpCodes.Ldc_I4_0);
					return;
				}
				case 1:
				{
					this.Emit(OpCodes.Ldc_I4_1);
					return;
				}
				case 2:
				{
					this.Emit(OpCodes.Ldc_I4_2);
					return;
				}
				case 3:
				{
					this.Emit(OpCodes.Ldc_I4_3);
					return;
				}
				case 4:
				{
					this.Emit(OpCodes.Ldc_I4_4);
					return;
				}
				case 5:
				{
					this.Emit(OpCodes.Ldc_I4_5);
					return;
				}
				case 6:
				{
					this.Emit(OpCodes.Ldc_I4_6);
					return;
				}
				case 7:
				{
					this.Emit(OpCodes.Ldc_I4_7);
					return;
				}
				case 8:
				{
					this.Emit(OpCodes.Ldc_I4_8);
					return;
				}
			}
			if (value < -128 || value > 127)
			{
				this.il.Emit(OpCodes.Ldc_I4, value);
				return;
			}
			this.il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
		}

		public void LoadValue(Local local)
		{
			OpCode opCode;
			if (local == null)
			{
				return;
			}
			if (local == this.InputValue)
			{
				this.Emit((this.isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
				return;
			}
			switch (local.Value.LocalIndex)
			{
				case 0:
				{
					this.Emit(OpCodes.Ldloc_0);
					return;
				}
				case 1:
				{
					this.Emit(OpCodes.Ldloc_1);
					return;
				}
				case 2:
				{
					this.Emit(OpCodes.Ldloc_2);
					return;
				}
				case 3:
				{
					this.Emit(OpCodes.Ldloc_3);
					return;
				}
			}
			opCode = (this.UseShortForm(local) ? OpCodes.Ldloc_S : OpCodes.Ldloc);
			this.il.Emit(opCode, local.Value);
		}

		public void LoadValue(FieldInfo field)
		{
			OpCode opCode;
			this.CheckAccessibility(field);
			opCode = (field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld);
			this.il.Emit(opCode, field);
		}

		public void LoadValue(PropertyInfo property)
		{
			this.CheckAccessibility(property);
			this.EmitCall(Helpers.GetGetMethod(property, true, true));
		}

		internal static void LoadValue(ILGenerator il, int value)
		{
			switch (value)
			{
				case -1:
				{
					il.Emit(OpCodes.Ldc_I4_M1);
					return;
				}
				case 0:
				{
					il.Emit(OpCodes.Ldc_I4_0);
					return;
				}
				case 1:
				{
					il.Emit(OpCodes.Ldc_I4_1);
					return;
				}
				case 2:
				{
					il.Emit(OpCodes.Ldc_I4_2);
					return;
				}
				case 3:
				{
					il.Emit(OpCodes.Ldc_I4_3);
					return;
				}
				case 4:
				{
					il.Emit(OpCodes.Ldc_I4_4);
					return;
				}
				case 5:
				{
					il.Emit(OpCodes.Ldc_I4_5);
					return;
				}
				case 6:
				{
					il.Emit(OpCodes.Ldc_I4_6);
					return;
				}
				case 7:
				{
					il.Emit(OpCodes.Ldc_I4_7);
					return;
				}
				case 8:
				{
					il.Emit(OpCodes.Ldc_I4_8);
					return;
				}
			}
			il.Emit(OpCodes.Ldc_I4, value);
		}

		internal void LoadValue(Type type)
		{
			this.il.Emit(OpCodes.Ldtoken, type);
			this.EmitCall(this.MapType(typeof(Type)).GetMethod("GetTypeFromHandle"));
		}

		internal void LoadValue(decimal value)
		{
			if (value == new decimal(0))
			{
				this.LoadValue(typeof(decimal).GetField("Zero"));
				return;
			}
			int[] bits = decimal.GetBits(value);
			this.LoadValue(bits[0]);
			this.LoadValue(bits[1]);
			this.LoadValue(bits[2]);
			this.LoadValue(bits[3] >> 31);
			this.LoadValue(bits[3] >> 16 & 255);
			Type type = this.MapType(typeof(decimal));
			Type[] typeArray = new Type[] { this.MapType(typeof(int)), this.MapType(typeof(int)), this.MapType(typeof(int)), this.MapType(typeof(bool)), this.MapType(typeof(byte)) };
			this.EmitCtor(type, typeArray);
		}

		internal void LoadValue(Guid value)
		{
			if (value == Guid.Empty)
			{
				this.LoadValue(typeof(Guid).GetField("Empty"));
				return;
			}
			byte[] byteArray = value.ToByteArray();
			int i = byteArray[0] | byteArray[1] << 8 | byteArray[2] << 16 | byteArray[3] << 24;
			this.LoadValue(i);
			short num = (short)(byteArray[4] | byteArray[5] << 8);
			this.LoadValue(num);
			num = (short)(byteArray[6] | byteArray[7] << 8);
			this.LoadValue(num);
			for (i = 8; i <= 15; i++)
			{
				this.LoadValue((int)byteArray[i]);
			}
			Type type = this.MapType(typeof(Guid));
			Type[] typeArray = new Type[] { this.MapType(typeof(int)), this.MapType(typeof(short)), this.MapType(typeof(short)), this.MapType(typeof(byte)), this.MapType(typeof(byte)), this.MapType(typeof(byte)), this.MapType(typeof(byte)), this.MapType(typeof(byte)), this.MapType(typeof(byte)), this.MapType(typeof(byte)), this.MapType(typeof(byte)) };
			this.EmitCtor(type, typeArray);
		}

		internal int MapMetaKeyToCompiledKey(int metaKey)
		{
			if (metaKey < 0 || this.methodPairs == null)
			{
				return metaKey;
			}
			for (int i = 0; i < (int)this.methodPairs.Length; i++)
			{
				if (this.methodPairs[i].MetaKey == metaKey)
				{
					return i;
				}
			}
			throw new ArgumentException(string.Concat("Key could not be mapped: ", metaKey.ToString()), "metaKey");
		}

		internal Type MapType(Type type)
		{
			return this.model.MapType(type);
		}

		internal void MarkLabel(CodeLabel label)
		{
			this.il.MarkLabel(label.Value);
		}

		internal void ReadNullCheckedTail(Type type, IProtoSerializer tail, Local valueFrom)
		{
			if (type.IsValueType)
			{
				Type underlyingType = Helpers.GetUnderlyingType(type);
				Type type1 = underlyingType;
				if (underlyingType != null)
				{
					if (tail.RequiresOldValue)
					{
						using (Local localWithValue = this.GetLocalWithValue(type, valueFrom))
						{
							this.LoadAddress(localWithValue, type);
							this.EmitCall(type.GetMethod("GetValueOrDefault", Helpers.EmptyTypes));
						}
					}
					tail.EmitRead(this, null);
					if (tail.ReturnsValue)
					{
						this.EmitCtor(type, new Type[] { type1 });
					}
					return;
				}
			}
			tail.EmitRead(this, valueFrom);
		}

		internal void ReleaseToPool(LocalBuilder value)
		{
			int count = this.locals.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.locals[i] == null)
				{
					this.locals[i] = value;
					return;
				}
			}
			this.locals.Add(value);
		}

		internal void Return()
		{
			this.Emit(OpCodes.Ret);
		}

		public void StoreValue(Local local)
		{
			OpCode opCode;
			if (local == this.InputValue)
			{
				this.il.Emit(OpCodes.Starg_S, (byte)((this.isStatic ? 0 : 1)));
				return;
			}
			switch (local.Value.LocalIndex)
			{
				case 0:
				{
					this.Emit(OpCodes.Stloc_0);
					return;
				}
				case 1:
				{
					this.Emit(OpCodes.Stloc_1);
					return;
				}
				case 2:
				{
					this.Emit(OpCodes.Stloc_2);
					return;
				}
				case 3:
				{
					this.Emit(OpCodes.Stloc_3);
					return;
				}
			}
			opCode = (this.UseShortForm(local) ? OpCodes.Stloc_S : OpCodes.Stloc);
			this.il.Emit(opCode, local.Value);
		}

		public void StoreValue(FieldInfo field)
		{
			OpCode opCode;
			this.CheckAccessibility(field);
			opCode = (field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld);
			this.il.Emit(opCode, field);
		}

		public void StoreValue(PropertyInfo property)
		{
			this.CheckAccessibility(property);
			this.EmitCall(Helpers.GetSetMethod(property, true, true));
		}

		public void Subtract()
		{
			this.Emit(OpCodes.Sub);
		}

		public void Switch(CodeLabel[] jumpTable)
		{
			if ((int)jumpTable.Length <= 128)
			{
				Label[] value = new Label[(int)jumpTable.Length];
				for (int i = 0; i < (int)value.Length; i++)
				{
					value[i] = jumpTable[i].Value;
				}
				this.il.Emit(OpCodes.Switch, value);
				return;
			}
			using (Local localWithValue = this.GetLocalWithValue(this.MapType(typeof(int)), null))
			{
				int length = (int)jumpTable.Length;
				int num = 0;
				int num1 = length / 128;
				if (length % 128 != 0)
				{
					num1++;
				}
				Label[] labelArray = new Label[num1];
				for (int j = 0; j < num1; j++)
				{
					labelArray[j] = this.il.DefineLabel();
				}
				CodeLabel codeLabel = this.DefineLabel();
				this.LoadValue(localWithValue);
				this.LoadValue(128);
				this.Emit(OpCodes.Div);
				this.il.Emit(OpCodes.Switch, labelArray);
				this.Branch(codeLabel, false);
				Label[] value1 = new Label[128];
				for (int k = 0; k < num1; k++)
				{
					this.il.MarkLabel(labelArray[k]);
					int num2 = Math.Min(128, length);
					length -= num2;
					if ((int)value1.Length != num2)
					{
						value1 = new Label[num2];
					}
					int num3 = num;
					for (int l = 0; l < num2; l++)
					{
						int num4 = num;
						num = num4 + 1;
						value1[l] = jumpTable[num4].Value;
					}
					this.LoadValue(localWithValue);
					if (num3 != 0)
					{
						this.LoadValue(num3);
						this.Emit(OpCodes.Sub);
					}
					this.il.Emit(OpCodes.Switch, value1);
					if (length != 0)
					{
						this.Branch(codeLabel, false);
					}
				}
				this.MarkLabel(codeLabel);
			}
		}

		internal void TryCast(Type type)
		{
			this.il.Emit(OpCodes.Isinst, type);
		}

		private bool UseShortForm(Local local)
		{
			return local.Value.LocalIndex < 256;
		}

		public IDisposable Using(Local local)
		{
			return new CompilerContext.UsingBlock(this, local);
		}

		internal void WriteNullCheckedTail(Type type, IProtoSerializer tail, Local valueFrom)
		{
			if (!type.IsValueType)
			{
				this.LoadValue(valueFrom);
				this.CopyValue();
				CodeLabel codeLabel = this.DefineLabel();
				CodeLabel codeLabel1 = this.DefineLabel();
				this.BranchIfTrue(codeLabel, true);
				this.DiscardValue();
				this.Branch(codeLabel1, false);
				this.MarkLabel(codeLabel);
				tail.EmitWrite(this, null);
				this.MarkLabel(codeLabel1);
			}
			else
			{
				if (Helpers.GetUnderlyingType(type) == null)
				{
					tail.EmitWrite(this, valueFrom);
					return;
				}
				using (Local localWithValue = this.GetLocalWithValue(type, valueFrom))
				{
					this.LoadAddress(localWithValue, type);
					this.LoadValue(type.GetProperty("HasValue"));
					CodeLabel codeLabel2 = this.DefineLabel();
					this.BranchIfFalse(codeLabel2, false);
					this.LoadAddress(localWithValue, type);
					this.EmitCall(type.GetMethod("GetValueOrDefault", Helpers.EmptyTypes));
					tail.EmitWrite(this, null);
					this.MarkLabel(codeLabel2);
				}
			}
		}

		public enum ILVersion
		{
			Net1,
			Net2
		}

		private sealed class UsingBlock : IDisposable
		{
			private Local local;

			private CompilerContext ctx;

			private CodeLabel label;

			public UsingBlock(CompilerContext ctx, Local local)
			{
				if (ctx == null)
				{
					throw new ArgumentNullException("ctx");
				}
				if (local == null)
				{
					throw new ArgumentNullException("local");
				}
				Type type = local.Type;
				if ((type.IsValueType || type.IsSealed) && !ctx.MapType(typeof(IDisposable)).IsAssignableFrom(type))
				{
					return;
				}
				this.local = local;
				this.ctx = ctx;
				this.label = ctx.BeginTry();
			}

			public void Dispose()
			{
				if (this.local == null || this.ctx == null)
				{
					return;
				}
				this.ctx.EndTry(this.label, false);
				this.ctx.BeginFinally();
				Type type = this.ctx.MapType(typeof(IDisposable));
				MethodInfo method = type.GetMethod("Dispose");
				Type type1 = this.local.Type;
				if (!type1.IsValueType)
				{
					CodeLabel codeLabel = this.ctx.DefineLabel();
					if (!type.IsAssignableFrom(type1))
					{
						using (Local local = new Local(this.ctx, type))
						{
							this.ctx.LoadValue(this.local);
							this.ctx.TryCast(type);
							this.ctx.CopyValue();
							this.ctx.StoreValue(local);
							this.ctx.BranchIfFalse(codeLabel, true);
							this.ctx.LoadAddress(local, type);
						}
					}
					else
					{
						this.ctx.LoadValue(this.local);
						this.ctx.BranchIfFalse(codeLabel, true);
						this.ctx.LoadAddress(this.local, type1);
					}
					this.ctx.EmitCall(method);
					this.ctx.MarkLabel(codeLabel);
				}
				else
				{
					this.ctx.LoadAddress(this.local, type1);
					if (this.ctx.MetadataVersion != CompilerContext.ILVersion.Net1)
					{
						this.ctx.Constrain(type1);
					}
					else
					{
						this.ctx.LoadValue(this.local);
						this.ctx.CastToObject(type1);
					}
					this.ctx.EmitCall(method);
				}
				this.ctx.EndFinally();
				this.local = null;
				this.ctx = null;
				this.label = new CodeLabel();
			}
		}
	}
}