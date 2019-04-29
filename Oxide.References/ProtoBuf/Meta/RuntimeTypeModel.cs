using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace ProtoBuf.Meta
{
	public sealed class RuntimeTypeModel : TypeModel
	{
		private const byte OPTIONS_InferTagFromNameDefault = 1;

		private const byte OPTIONS_IsDefaultModel = 2;

		private const byte OPTIONS_Frozen = 4;

		private const byte OPTIONS_AutoAddMissingTypes = 8;

		private const byte OPTIONS_AutoCompile = 16;

		private const byte OPTIONS_UseImplicitZeroDefaults = 32;

		private const byte OPTIONS_AllowParseableTypes = 64;

		private const byte OPTIONS_AutoAddProtoContractTypesOnly = 128;

		private const int KnownTypes_Array = 1;

		private const int KnownTypes_Dictionary = 2;

		private const int KnownTypes_Hashtable = 3;

		private const int KnownTypes_ArrayCutoff = 20;

		private byte options;

		private readonly static BasicList.MatchPredicate MetaTypeFinder;

		private readonly static BasicList.MatchPredicate BasicTypeFinder;

		private BasicList basicTypes = new BasicList();

		private readonly BasicList types = new BasicList();

		private int metadataTimeoutMilliseconds = 5000;

		private int contentionCounter = 1;

		private readonly object contentionLock = new object();

		private MethodInfo defaultFactory;

		public bool AllowParseableTypes
		{
			get
			{
				return this.GetOption(64);
			}
			set
			{
				this.SetOption(64, value);
			}
		}

		public bool AutoAddMissingTypes
		{
			get
			{
				return this.GetOption(8);
			}
			set
			{
				if (!value && this.GetOption(2))
				{
					throw new InvalidOperationException("The default model must allow missing types");
				}
				this.ThrowIfFrozen();
				this.SetOption(8, value);
			}
		}

		public bool AutoAddProtoContractTypesOnly
		{
			get
			{
				return this.GetOption(128);
			}
			set
			{
				this.SetOption(128, value);
			}
		}

		public bool AutoCompile
		{
			get
			{
				return this.GetOption(16);
			}
			set
			{
				this.SetOption(16, value);
			}
		}

		public static RuntimeTypeModel Default
		{
			get
			{
				return RuntimeTypeModel.Singleton.Value;
			}
		}

		public bool InferTagFromNameDefault
		{
			get
			{
				return this.GetOption(1);
			}
			set
			{
				this.SetOption(1, value);
			}
		}

		public MetaType this[Type type]
		{
			get
			{
				return (MetaType)this.types[this.FindOrAddAuto(type, true, false, false)];
			}
		}

		public int MetadataTimeoutMilliseconds
		{
			get
			{
				return this.metadataTimeoutMilliseconds;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("MetadataTimeoutMilliseconds");
				}
				this.metadataTimeoutMilliseconds = value;
			}
		}

		public bool UseImplicitZeroDefaults
		{
			get
			{
				return this.GetOption(32);
			}
			set
			{
				if (!value && this.GetOption(2))
				{
					throw new InvalidOperationException("UseImplicitZeroDefaults cannot be disabled on the default model");
				}
				this.SetOption(32, value);
			}
		}

		static RuntimeTypeModel()
		{
			RuntimeTypeModel.MetaTypeFinder = new BasicList.MatchPredicate(RuntimeTypeModel.MetaTypeFinderImpl);
			RuntimeTypeModel.BasicTypeFinder = new BasicList.MatchPredicate(RuntimeTypeModel.BasicTypeFinderImpl);
		}

		internal RuntimeTypeModel(bool isDefault)
		{
			this.AutoAddMissingTypes = true;
			this.UseImplicitZeroDefaults = true;
			this.SetOption(2, isDefault);
			this.AutoCompile = true;
		}

		public MetaType Add(Type type, bool applyDefaultBehaviour)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			MetaType metaType = this.FindWithoutAdd(type);
			if (metaType != null)
			{
				return metaType;
			}
			int num = 0;
			if (type.IsInterface && base.MapType(MetaType.ienumerable).IsAssignableFrom(type) && TypeModel.GetListItemType(this, type) == null)
			{
				throw new ArgumentException("IEnumerable[<T>] data cannot be used as a meta-type unless an Add method can be resolved");
			}
			try
			{
				metaType = this.RecogniseCommonTypes(type);
				if (metaType != null)
				{
					if (!applyDefaultBehaviour)
					{
						throw new ArgumentException(string.Concat("Default behaviour must be observed for certain types with special handling; ", type.FullName), "applyDefaultBehaviour");
					}
					applyDefaultBehaviour = false;
				}
				if (metaType == null)
				{
					metaType = this.Create(type);
				}
				metaType.Pending = true;
				this.TakeLock(ref num);
				if (this.FindWithoutAdd(type) != null)
				{
					throw new ArgumentException("Duplicate type", "type");
				}
				this.ThrowIfFrozen();
				this.types.Add(metaType);
				if (applyDefaultBehaviour)
				{
					metaType.ApplyDefaultBehaviour();
				}
				metaType.Pending = false;
			}
			finally
			{
				this.ReleaseLock(num);
			}
			return metaType;
		}

		private void AddContention()
		{
			lock (this.contentionLock)
			{
				this.contentionCounter++;
			}
		}

		private static bool BasicTypeFinderImpl(object value, object ctx)
		{
			return ((RuntimeTypeModel.BasicType)value).Type == (Type)ctx;
		}

		private void BuildAllSerializers()
		{
			for (int i = 0; i < this.types.Count; i++)
			{
				MetaType item = (MetaType)this.types[i];
				if (item.Serializer == null)
				{
					throw new InvalidOperationException(string.Concat("No serializer available for ", item.Type.Name));
				}
			}
		}

		private void CascadeDependents(BasicList list, MetaType metaType)
		{
			MetaType surrogateOrBaseOrSelf;
			WireType wireType;
			MemberInfo[] memberInfoArray;
			WireType wireType1;
			WireType wireType2;
			if (!metaType.IsList)
			{
				if (!metaType.IsAutoTuple)
				{
					foreach (ValueMember field in metaType.Fields)
					{
						Type itemType = field.ItemType ?? field.MemberType;
						if (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, itemType, out wireType2, false, false, false, false) != null)
						{
							continue;
						}
						int num = this.FindOrAddAuto(itemType, false, false, false);
						if (num < 0)
						{
							continue;
						}
						surrogateOrBaseOrSelf = ((MetaType)this.types[num]).GetSurrogateOrBaseOrSelf(false);
						if (list.Contains(surrogateOrBaseOrSelf))
						{
							continue;
						}
						list.Add(surrogateOrBaseOrSelf);
						this.CascadeDependents(list, surrogateOrBaseOrSelf);
					}
				}
				else if (MetaType.ResolveTupleConstructor(metaType.Type, out memberInfoArray) != null)
				{
					for (int i = 0; i < (int)memberInfoArray.Length; i++)
					{
						Type propertyType = null;
						if (memberInfoArray[i] is PropertyInfo)
						{
							propertyType = ((PropertyInfo)memberInfoArray[i]).PropertyType;
						}
						else if (memberInfoArray[i] is FieldInfo)
						{
							propertyType = ((FieldInfo)memberInfoArray[i]).FieldType;
						}
						if (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, propertyType, out wireType1, false, false, false, false) == null)
						{
							int num1 = this.FindOrAddAuto(propertyType, false, false, false);
							if (num1 >= 0)
							{
								surrogateOrBaseOrSelf = ((MetaType)this.types[num1]).GetSurrogateOrBaseOrSelf(false);
								if (!list.Contains(surrogateOrBaseOrSelf))
								{
									list.Add(surrogateOrBaseOrSelf);
									this.CascadeDependents(list, surrogateOrBaseOrSelf);
								}
							}
						}
					}
				}
				if (metaType.HasSubtypes)
				{
					SubType[] subtypes = metaType.GetSubtypes();
					for (int j = 0; j < (int)subtypes.Length; j++)
					{
						surrogateOrBaseOrSelf = subtypes[j].DerivedType.GetSurrogateOrSelf();
						if (!list.Contains(surrogateOrBaseOrSelf))
						{
							list.Add(surrogateOrBaseOrSelf);
							this.CascadeDependents(list, surrogateOrBaseOrSelf);
						}
					}
				}
				surrogateOrBaseOrSelf = metaType.BaseType;
				if (surrogateOrBaseOrSelf != null)
				{
					surrogateOrBaseOrSelf = surrogateOrBaseOrSelf.GetSurrogateOrSelf();
				}
				if (surrogateOrBaseOrSelf != null && !list.Contains(surrogateOrBaseOrSelf))
				{
					list.Add(surrogateOrBaseOrSelf);
					this.CascadeDependents(list, surrogateOrBaseOrSelf);
				}
			}
			else
			{
				Type listItemType = TypeModel.GetListItemType(this, metaType.Type);
				if (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, listItemType, out wireType, false, false, false, false) == null)
				{
					int num2 = this.FindOrAddAuto(listItemType, false, false, false);
					if (num2 >= 0)
					{
						surrogateOrBaseOrSelf = ((MetaType)this.types[num2]).GetSurrogateOrBaseOrSelf(false);
						if (!list.Contains(surrogateOrBaseOrSelf))
						{
							list.Add(surrogateOrBaseOrSelf);
							this.CascadeDependents(list, surrogateOrBaseOrSelf);
							return;
						}
					}
				}
			}
		}

		public TypeModel Compile()
		{
			return this.Compile(null, null);
		}

		public TypeModel Compile(string name, string path)
		{
			RuntimeTypeModel.CompilerOptions compilerOption = new RuntimeTypeModel.CompilerOptions()
			{
				TypeName = name,
				OutputPath = path
			};
			return this.Compile(compilerOption);
		}

		public TypeModel Compile(RuntimeTypeModel.CompilerOptions options)
		{
			string name;
			string str;
			int num;
			bool flag;
			RuntimeTypeModel.SerializerPair[] serializerPairArray;
			CompilerContext.ILVersion lVersion;
			ILGenerator lGenerator;
			int num1;
			FieldBuilder fieldBuilder;
			Type type;
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			string typeName = options.TypeName;
			string outputPath = options.OutputPath;
			this.BuildAllSerializers();
			this.Freeze();
			bool flag1 = !Helpers.IsNullOrEmpty(outputPath);
			if (Helpers.IsNullOrEmpty(typeName))
			{
				if (flag1)
				{
					throw new ArgumentNullException("typeName");
				}
				typeName = Guid.NewGuid().ToString();
			}
			if (outputPath != null)
			{
				name = (new FileInfo(Path.GetFileNameWithoutExtension(outputPath))).Name;
				str = string.Concat(name, Path.GetExtension(outputPath));
			}
			else
			{
				name = typeName;
				str = string.Concat(name, ".dll");
			}
			AssemblyName assemblyName = new AssemblyName()
			{
				Name = name
			};
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, (flag1 ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run));
			ModuleBuilder moduleBuilder = (flag1 ? assemblyBuilder.DefineDynamicModule(str, outputPath) : assemblyBuilder.DefineDynamicModule(str));
			this.WriteAssemblyAttributes(options, name, assemblyBuilder);
			TypeBuilder typeBuilder = this.WriteBasicTypeModel(options, typeName, moduleBuilder);
			this.WriteSerializers(options, name, typeBuilder, out num, out flag, out serializerPairArray, out lVersion);
			this.WriteGetKeyImpl(typeBuilder, flag, serializerPairArray, lVersion, name, out lGenerator, out num1, out fieldBuilder, out type);
			CompilerContext compilerContext = this.WriteSerializeDeserialize(name, typeBuilder, serializerPairArray, lVersion, ref lGenerator);
			this.WriteConstructors(typeBuilder, ref num, serializerPairArray, ref lGenerator, num1, fieldBuilder, type, compilerContext);
			Type type1 = typeBuilder.CreateType();
			if (!Helpers.IsNullOrEmpty(outputPath))
			{
				assemblyBuilder.Save(outputPath);
			}
			return (TypeModel)Activator.CreateInstance(type1);
		}

		public void CompileInPlace()
		{
			BasicList.NodeEnumerator enumerator = this.types.GetEnumerator();
			while (enumerator.MoveNext())
			{
				((MetaType)enumerator.Current).CompileInPlace();
			}
		}

		private MetaType Create(Type type)
		{
			this.ThrowIfFrozen();
			return new MetaType(this, type, this.defaultFactory);
		}

		protected internal override object Deserialize(int key, object value, ProtoReader source)
		{
			IProtoSerializer serializer = ((MetaType)this.types[key]).Serializer;
			if (value != null || !Helpers.IsValueType(serializer.ExpectedType))
			{
				return serializer.Read(value, source);
			}
			if (serializer.RequiresOldValue)
			{
				value = Activator.CreateInstance(serializer.ExpectedType);
			}
			return serializer.Read(value, source);
		}

		private static MethodBuilder EmitBoxedSerializer(TypeBuilder type, int i, Type valueType, RuntimeTypeModel.SerializerPair[] methodPairs, TypeModel model, CompilerContext.ILVersion ilVersion, string assemblyName)
		{
			MethodInfo deserialize = methodPairs[i].Deserialize;
			string str = string.Concat("_", i.ToString());
			Type type1 = model.MapType(typeof(object));
			Type[] typeArray = new Type[] { model.MapType(typeof(object)), model.MapType(typeof(ProtoReader)) };
			MethodBuilder methodBuilder = type.DefineMethod(str, MethodAttributes.Static, CallingConventions.Standard, type1, typeArray);
			CompilerContext compilerContext = new CompilerContext(methodBuilder.GetILGenerator(), true, false, methodPairs, model, ilVersion, assemblyName, model.MapType(typeof(object)));
			compilerContext.LoadValue(compilerContext.InputValue);
			CodeLabel codeLabel = compilerContext.DefineLabel();
			compilerContext.BranchIfFalse(codeLabel, true);
			Type type2 = valueType;
			compilerContext.LoadValue(compilerContext.InputValue);
			compilerContext.CastFromObject(type2);
			compilerContext.LoadReaderWriter();
			compilerContext.EmitCall(deserialize);
			compilerContext.CastToObject(type2);
			compilerContext.Return();
			compilerContext.MarkLabel(codeLabel);
			using (Local local = new Local(compilerContext, type2))
			{
				compilerContext.LoadAddress(local, type2);
				compilerContext.EmitCtor(type2);
				compilerContext.LoadValue(local);
				compilerContext.LoadReaderWriter();
				compilerContext.EmitCall(deserialize);
				compilerContext.CastToObject(type2);
				compilerContext.Return();
			}
			return methodBuilder;
		}

		internal int FindOrAddAuto(Type type, bool demand, bool addWithContractOnly, bool addEvenIfAutoDisabled)
		{
			MetaType item;
			int num;
			int num1 = this.types.IndexOf(RuntimeTypeModel.MetaTypeFinder, type);
			if (num1 >= 0)
			{
				item = (MetaType)this.types[num1];
				if (item.Pending)
				{
					this.WaitOnLock(item);
				}
				return num1;
			}
			bool flag = (this.AutoAddMissingTypes ? true : addEvenIfAutoDisabled);
			if (!Helpers.IsEnum(type) && this.TryGetBasicTypeSerializer(type) != null)
			{
				if (flag && !addWithContractOnly)
				{
					throw MetaType.InbuiltType(type);
				}
				return -1;
			}
			Type type1 = TypeModel.ResolveProxies(type);
			if (type1 != null)
			{
				num1 = this.types.IndexOf(RuntimeTypeModel.MetaTypeFinder, type1);
				type = type1;
			}
			if (num1 < 0)
			{
				int num2 = 0;
				try
				{
					this.TakeLock(ref num2);
					MetaType metaType = this.RecogniseCommonTypes(type);
					item = metaType;
					if (metaType == null)
					{
						MetaType.AttributeFamily contractFamily = MetaType.GetContractFamily(this, type, null);
						if (contractFamily == MetaType.AttributeFamily.AutoTuple)
						{
							int num3 = 1;
							addEvenIfAutoDisabled = (bool)num3;
							flag = (bool)num3;
						}
						if (!flag || !Helpers.IsEnum(type) && addWithContractOnly && contractFamily == MetaType.AttributeFamily.None)
						{
							if (demand)
							{
								TypeModel.ThrowUnexpectedType(type);
							}
							num = num1;
							return num;
						}
						else
						{
							item = this.Create(type);
						}
					}
					item.Pending = true;
					bool flag1 = false;
					int num4 = this.types.IndexOf(RuntimeTypeModel.MetaTypeFinder, type);
					if (num4 >= 0)
					{
						num1 = num4;
					}
					else
					{
						this.ThrowIfFrozen();
						num1 = this.types.Add(item);
						flag1 = true;
					}
					if (flag1)
					{
						item.ApplyDefaultBehaviour();
						item.Pending = false;
					}
					return num1;
				}
				finally
				{
					this.ReleaseLock(num2);
				}
				return num;
			}
			return num1;
		}

		internal MetaType FindWithoutAdd(Type type)
		{
			BasicList.NodeEnumerator enumerator = this.types.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MetaType current = (MetaType)enumerator.Current;
				if (current.Type != type)
				{
					continue;
				}
				if (current.Pending)
				{
					this.WaitOnLock(current);
				}
				return current;
			}
			Type type1 = TypeModel.ResolveProxies(type);
			if (type1 == null)
			{
				return null;
			}
			return this.FindWithoutAdd(type1);
		}

		public void Freeze()
		{
			if (this.GetOption(2))
			{
				throw new InvalidOperationException("The default model cannot be frozen");
			}
			this.SetOption(4, true);
		}

		private int GetContention()
		{
			int num;
			lock (this.contentionLock)
			{
				num = this.contentionCounter;
			}
			return num;
		}

		internal EnumSerializer.EnumPair[] GetEnumMap(Type type)
		{
			int num = this.FindOrAddAuto(type, false, false, false);
			if (num < 0)
			{
				return null;
			}
			return ((MetaType)this.types[num]).GetEnumMap();
		}

		internal int GetKey(Type type, bool demand, bool getBaseKey)
		{
			int num;
			try
			{
				int num1 = this.FindOrAddAuto(type, demand, true, false);
				if (num1 >= 0)
				{
					MetaType item = (MetaType)this.types[num1];
					if (getBaseKey)
					{
						item = MetaType.GetRootType(item);
						num1 = this.FindOrAddAuto(item.Type, true, true, false);
					}
				}
				num = num1;
			}
			catch (NotSupportedException notSupportedException)
			{
				throw;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (exception.Message.IndexOf(type.FullName) < 0)
				{
					throw new ProtoException(string.Concat(exception.Message, " (", type.FullName, ")"), exception);
				}
				throw;
			}
			return num;
		}

		protected override int GetKeyImpl(Type type)
		{
			return this.GetKey(type, false, true);
		}

		private bool GetOption(byte option)
		{
			return (this.options & option) == option;
		}

		public override string GetSchema(Type type)
		{
			WireType wireType;
			BasicList basicLists = new BasicList();
			MetaType surrogateOrBaseOrSelf = null;
			bool flag = false;
			if (type != null)
			{
				Type underlyingType = Helpers.GetUnderlyingType(type);
				if (underlyingType != null)
				{
					type = underlyingType;
				}
				flag = ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out wireType, false, false, false, false) != null;
				if (!flag)
				{
					int num = this.FindOrAddAuto(type, false, false, false);
					if (num < 0)
					{
						throw new ArgumentException("The type specified is not a contract-type", "type");
					}
					surrogateOrBaseOrSelf = ((MetaType)this.types[num]).GetSurrogateOrBaseOrSelf(false);
					basicLists.Add(surrogateOrBaseOrSelf);
					this.CascadeDependents(basicLists, surrogateOrBaseOrSelf);
				}
			}
			else
			{
				BasicList.NodeEnumerator enumerator = this.types.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MetaType metaType = ((MetaType)enumerator.Current).GetSurrogateOrBaseOrSelf(false);
					if (basicLists.Contains(metaType))
					{
						continue;
					}
					basicLists.Add(metaType);
					this.CascadeDependents(basicLists, metaType);
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			string str = null;
			if (!flag)
			{
				foreach (MetaType metaType1 in (surrogateOrBaseOrSelf == null ? this.types : basicLists))
				{
					if (metaType1.IsList)
					{
						continue;
					}
					string @namespace = metaType1.Type.Namespace;
					if (Helpers.IsNullOrEmpty(@namespace) || @namespace.StartsWith("System."))
					{
						continue;
					}
					if (str != null)
					{
						if (str == @namespace)
						{
							continue;
						}
						str = null;
						break;
					}
					else
					{
						str = @namespace;
					}
				}
			}
			if (!Helpers.IsNullOrEmpty(str))
			{
				stringBuilder.Append("package ").Append(str).Append(';');
				Helpers.AppendLine(stringBuilder);
			}
			bool flag1 = false;
			StringBuilder stringBuilder1 = new StringBuilder();
			MetaType[] metaTypeArray = new MetaType[basicLists.Count];
			basicLists.CopyTo(metaTypeArray, 0);
			Array.Sort<MetaType>(metaTypeArray, MetaType.Comparer.Default);
			if (!flag)
			{
				for (int i = 0; i < (int)metaTypeArray.Length; i++)
				{
					MetaType metaType2 = metaTypeArray[i];
					if (!metaType2.IsList || metaType2 == surrogateOrBaseOrSelf)
					{
						metaType2.WriteSchema(stringBuilder1, 0, ref flag1);
					}
				}
			}
			else
			{
				Helpers.AppendLine(stringBuilder1).Append("message ").Append(type.Name).Append(" {");
				MetaType.NewLine(stringBuilder1, 1).Append("optional ").Append(this.GetSchemaTypeName(type, DataFormat.Default, false, false, ref flag1)).Append(" value = 1;");
				Helpers.AppendLine(stringBuilder1).Append('}');
			}
			if (flag1)
			{
				stringBuilder.Append("import \"bcl.proto\"; // schema for protobuf-net's handling of core .NET types");
				Helpers.AppendLine(stringBuilder);
			}
			return Helpers.AppendLine(stringBuilder.Append(stringBuilder1)).ToString();
		}

		internal string GetSchemaTypeName(Type effectiveType, DataFormat dataFormat, bool asReference, bool dynamicType, ref bool requiresBclImport)
		{
			WireType wireType;
			Type underlyingType = Helpers.GetUnderlyingType(effectiveType);
			if (underlyingType != null)
			{
				effectiveType = underlyingType;
			}
			if (effectiveType == base.MapType(typeof(byte[])))
			{
				return "bytes";
			}
			IProtoSerializer protoSerializer = ValueMember.TryGetCoreSerializer(this, dataFormat, effectiveType, out wireType, false, false, false, false);
			if (protoSerializer == null)
			{
				if (asReference || dynamicType)
				{
					requiresBclImport = true;
					return "bcl.NetObjectProxy";
				}
				return this[effectiveType].GetSurrogateOrBaseOrSelf(true).GetSchemaTypeName();
			}
			if (protoSerializer is ParseableSerializer)
			{
				if (asReference)
				{
					requiresBclImport = true;
				}
				if (!asReference)
				{
					return "string";
				}
				return "bcl.NetObjectProxy";
			}
			ProtoTypeCode typeCode = Helpers.GetTypeCode(effectiveType);
			switch (typeCode)
			{
				case ProtoTypeCode.Boolean:
				{
					return "bool";
				}
				case ProtoTypeCode.Char:
				case ProtoTypeCode.Byte:
				case ProtoTypeCode.UInt16:
				case ProtoTypeCode.UInt32:
				{
					if (dataFormat == DataFormat.FixedSize)
					{
						return "fixed32";
					}
					return "uint32";
				}
				case ProtoTypeCode.SByte:
				case ProtoTypeCode.Int16:
				case ProtoTypeCode.Int32:
				{
					switch (dataFormat)
					{
						case DataFormat.ZigZag:
						{
							return "sint32";
						}
						case DataFormat.TwosComplement:
						{
							return "int32";
						}
						case DataFormat.FixedSize:
						{
							return "sfixed32";
						}
						default:
						{
							return "int32";
						}
					}
					break;
				}
				case ProtoTypeCode.Int64:
				{
					switch (dataFormat)
					{
						case DataFormat.ZigZag:
						{
							return "sint64";
						}
						case DataFormat.TwosComplement:
						{
							return "int64";
						}
						case DataFormat.FixedSize:
						{
							return "sfixed64";
						}
						default:
						{
							return "int64";
						}
					}
					break;
				}
				case ProtoTypeCode.UInt64:
				{
					if (dataFormat == DataFormat.FixedSize)
					{
						return "fixed64";
					}
					return "uint64";
				}
				case ProtoTypeCode.Single:
				{
					return "float";
				}
				case ProtoTypeCode.Double:
				{
					return "double";
				}
				case ProtoTypeCode.Decimal:
				{
					requiresBclImport = true;
					return "bcl.Decimal";
				}
				case ProtoTypeCode.DateTime:
				{
					requiresBclImport = true;
					return "bcl.DateTime";
				}
				case ProtoTypeCode.Unknown | ProtoTypeCode.DateTime:
				{
					throw new NotSupportedException(string.Concat("No .proto map found for: ", effectiveType.FullName));
				}
				case ProtoTypeCode.String:
				{
					if (asReference)
					{
						requiresBclImport = true;
					}
					if (!asReference)
					{
						return "string";
					}
					return "bcl.NetObjectProxy";
				}
				default:
				{
					switch (typeCode)
					{
						case ProtoTypeCode.TimeSpan:
						{
							requiresBclImport = true;
							return "bcl.TimeSpan";
						}
						case ProtoTypeCode.ByteArray:
						{
							throw new NotSupportedException(string.Concat("No .proto map found for: ", effectiveType.FullName));
						}
						case ProtoTypeCode.Guid:
						{
							requiresBclImport = true;
							return "bcl.Guid";
						}
						default:
						{
							throw new NotSupportedException(string.Concat("No .proto map found for: ", effectiveType.FullName));
						}
					}
					break;
				}
			}
		}

		internal ProtoSerializer GetSerializer(IProtoSerializer serializer, bool compiled)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			if (compiled)
			{
				return CompilerContext.BuildSerializer(serializer, this);
			}
			IProtoSerializer protoSerializer = serializer;
			return new ProtoSerializer(protoSerializer.Write);
		}

		public IEnumerable GetTypes()
		{
			return this.types;
		}

		internal bool IsPrepared(Type type)
		{
			MetaType metaType = this.FindWithoutAdd(type);
			if (metaType == null)
			{
				return false;
			}
			return metaType.IsPrepared();
		}

		private static bool MetaTypeFinderImpl(object value, object ctx)
		{
			return ((MetaType)value).Type == (Type)ctx;
		}

		private static ILGenerator Override(TypeBuilder type, string name)
		{
			MethodInfo method = type.BaseType.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
			ParameterInfo[] parameters = method.GetParameters();
			Type[] parameterType = new Type[(int)parameters.Length];
			for (int i = 0; i < (int)parameterType.Length; i++)
			{
				parameterType[i] = parameters[i].ParameterType;
			}
			MethodBuilder methodBuilder = type.DefineMethod(method.Name, method.Attributes & (MethodAttributes.MemberAccessMask | MethodAttributes.Private | MethodAttributes.FamANDAssem | MethodAttributes.Assembly | MethodAttributes.Family | MethodAttributes.FamORAssem | MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.CheckAccessOnOverride | MethodAttributes.VtableLayoutMask | MethodAttributes.NewSlot | MethodAttributes.SpecialName | MethodAttributes.PinvokeImpl | MethodAttributes.UnmanagedExport | MethodAttributes.RTSpecialName | MethodAttributes.ReservedMask | MethodAttributes.HasSecurity | MethodAttributes.RequireSecObject) | MethodAttributes.Final, method.CallingConvention, method.ReturnType, parameterType);
			ILGenerator lGenerator = methodBuilder.GetILGenerator();
			type.DefineMethodOverride(methodBuilder, method);
			return lGenerator;
		}

		private MetaType RecogniseCommonTypes(Type type)
		{
			return null;
		}

		internal void ReleaseLock(int opaqueToken)
		{
			string stackTrace;
			if (opaqueToken != 0)
			{
				Monitor.Exit(this.types);
				if (opaqueToken != this.GetContention())
				{
					LockContentedEventHandler lockContentedEventHandler = this.LockContended;
					if (lockContentedEventHandler != null)
					{
						try
						{
							throw new ProtoException();
						}
						catch (Exception exception)
						{
							stackTrace = exception.StackTrace;
						}
						lockContentedEventHandler(this, new LockContentedEventArgs(stackTrace));
					}
				}
			}
		}

		internal void ResolveListTypes(Type type, ref Type itemType, ref Type defaultType)
		{
			if (type == null)
			{
				return;
			}
			if (Helpers.GetTypeCode(type) != ProtoTypeCode.Unknown)
			{
				return;
			}
			if (this[type].IgnoreListHandling)
			{
				return;
			}
			if (type.IsArray)
			{
				if (type.GetArrayRank() != 1)
				{
					throw new NotSupportedException("Multi-dimension arrays are supported");
				}
				itemType = type.GetElementType();
				if (itemType != base.MapType(typeof(byte)))
				{
					defaultType = type;
				}
				else
				{
					object obj = null;
					Type type1 = (Type)obj;
					itemType = (Type)obj;
					defaultType = type1;
				}
			}
			if (itemType == null)
			{
				itemType = TypeModel.GetListItemType(this, type);
			}
			if (itemType != null)
			{
				Type type2 = null;
				Type type3 = null;
				this.ResolveListTypes(itemType, ref type2, ref type3);
				if (type2 != null)
				{
					throw TypeModel.CreateNestedListsNotSupported();
				}
			}
			if (itemType != null && defaultType == null)
			{
				if (type.IsClass && !type.IsAbstract && Helpers.GetConstructor(type, Helpers.EmptyTypes, true) != null)
				{
					defaultType = type;
				}
				if (defaultType == null && type.IsInterface)
				{
					if (type.IsGenericType && type.GetGenericTypeDefinition() == base.MapType(typeof(IDictionary<,>)))
					{
						Type type4 = base.MapType(typeof(KeyValuePair<,>));
						Type[] genericArguments = type.GetGenericArguments();
						Type[] typeArray = genericArguments;
						if (itemType != type4.MakeGenericType(genericArguments))
						{
							goto Label1;
						}
						defaultType = base.MapType(typeof(Dictionary<,>)).MakeGenericType(typeArray);
						goto Label0;
					}
				Label1:
					Type type5 = base.MapType(typeof(List<>));
					Type[] typeArray1 = new Type[] { itemType };
					defaultType = type5.MakeGenericType(typeArray1);
				}
			Label0:
				if (defaultType != null && !Helpers.IsAssignableFrom(type, defaultType))
				{
					defaultType = null;
				}
			}
		}

		protected internal override void Serialize(int key, object value, ProtoWriter dest)
		{
			((MetaType)this.types[key]).Serializer.Write(value, dest);
		}

		public void SetDefaultFactory(MethodInfo methodInfo)
		{
			this.VerifyFactory(methodInfo, null);
			this.defaultFactory = methodInfo;
		}

		private void SetOption(byte option, bool value)
		{
			if (value)
			{
				RuntimeTypeModel runtimeTypeModel = this;
				runtimeTypeModel.options = (byte)(runtimeTypeModel.options | option);
				return;
			}
			RuntimeTypeModel runtimeTypeModel1 = this;
			runtimeTypeModel1.options = (byte)(runtimeTypeModel1.options & (byte)(~option));
		}

		internal void TakeLock(ref int opaqueToken)
		{
			opaqueToken = 0;
			if (!Monitor.TryEnter(this.types, this.metadataTimeoutMilliseconds))
			{
				this.AddContention();
				throw new TimeoutException("Timeout while inspecting metadata; this may indicate a deadlock. This can often be avoided by preparing necessary serializers during application initialization, rather than allowing multiple threads to perform the initial metadata inspection; please also see the LockContended event");
			}
			opaqueToken = this.GetContention();
		}

		private void ThrowIfFrozen()
		{
			if (this.GetOption(4))
			{
				throw new InvalidOperationException("The model cannot be changed once frozen");
			}
		}

		internal IProtoSerializer TryGetBasicTypeSerializer(Type type)
		{
			WireType wireType;
			IProtoSerializer serializer;
			IProtoSerializer protoSerializer;
			int num = this.basicTypes.IndexOf(RuntimeTypeModel.BasicTypeFinder, type);
			if (num >= 0)
			{
				return ((RuntimeTypeModel.BasicType)this.basicTypes[num]).Serializer;
			}
			lock (this.basicTypes)
			{
				num = this.basicTypes.IndexOf(RuntimeTypeModel.BasicTypeFinder, type);
				if (num < 0)
				{
					if (MetaType.GetContractFamily(this, type, null) == MetaType.AttributeFamily.None)
					{
						protoSerializer = ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out wireType, false, false, false, false);
					}
					else
					{
						protoSerializer = null;
					}
					IProtoSerializer protoSerializer1 = protoSerializer;
					if (protoSerializer1 != null)
					{
						this.basicTypes.Add(new RuntimeTypeModel.BasicType(type, protoSerializer1));
					}
					serializer = protoSerializer1;
				}
				else
				{
					serializer = ((RuntimeTypeModel.BasicType)this.basicTypes[num]).Serializer;
				}
			}
			return serializer;
		}

		internal void VerifyFactory(MethodInfo factory, Type type)
		{
			if (factory != null)
			{
				if (type != null && Helpers.IsValueType(type))
				{
					throw new InvalidOperationException();
				}
				if (!factory.IsStatic)
				{
					throw new ArgumentException("A factory-method must be static", "factory");
				}
				if (type != null && factory.ReturnType != type && factory.ReturnType != base.MapType(typeof(object)))
				{
					throw new ArgumentException(string.Concat("The factory-method must return object", (type == null ? "" : string.Concat(" or ", type.FullName))), "factory");
				}
				if (!CallbackSet.CheckCallbackParameters(this, factory))
				{
					throw new ArgumentException(string.Concat("Invalid factory signature in ", factory.DeclaringType.FullName, ".", factory.Name), "factory");
				}
			}
		}

		private void WaitOnLock(MetaType type)
		{
			int num = 0;
			try
			{
				this.TakeLock(ref num);
			}
			finally
			{
				this.ReleaseLock(num);
			}
		}

		private void WriteAssemblyAttributes(RuntimeTypeModel.CompilerOptions options, string assemblyName, AssemblyBuilder asm)
		{
			PropertyInfo[] property;
			object[] targetFrameworkDisplayName;
			object obj;
			if (!Helpers.IsNullOrEmpty(options.TargetFrameworkName))
			{
				Type type = null;
				try
				{
					type = this.GetType("System.Runtime.Versioning.TargetFrameworkAttribute", base.MapType(typeof(string)).Assembly);
				}
				catch
				{
				}
				if (type != null)
				{
					if (!Helpers.IsNullOrEmpty(options.TargetFrameworkDisplayName))
					{
						property = new PropertyInfo[] { type.GetProperty("FrameworkDisplayName") };
						targetFrameworkDisplayName = new object[] { options.TargetFrameworkDisplayName };
					}
					else
					{
						property = new PropertyInfo[0];
						targetFrameworkDisplayName = new object[0];
					}
					Type[] typeArray = new Type[] { base.MapType(typeof(string)) };
					ConstructorInfo constructor = type.GetConstructor(typeArray);
					object[] targetFrameworkName = new object[] { options.TargetFrameworkName };
					asm.SetCustomAttribute(new CustomAttributeBuilder(constructor, targetFrameworkName, property, targetFrameworkDisplayName));
				}
			}
			Type type1 = null;
			try
			{
				type1 = base.MapType(typeof(InternalsVisibleToAttribute));
			}
			catch
			{
			}
			if (type1 != null)
			{
				BasicList basicLists = new BasicList();
				BasicList basicLists1 = new BasicList();
				BasicList.NodeEnumerator enumerator = this.types.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Assembly assembly = ((MetaType)enumerator.Current).Type.Assembly;
					if (basicLists1.IndexOfReference(assembly) >= 0)
					{
						continue;
					}
					basicLists1.Add(assembly);
					AttributeMap[] attributeMapArray = AttributeMap.Create(this, assembly);
					for (int i = 0; i < (int)attributeMapArray.Length; i++)
					{
						if (attributeMapArray[i].AttributeType == type1)
						{
							attributeMapArray[i].TryGet("AssemblyName", out obj);
							string str = obj as string;
							if (!(str == assemblyName) && !Helpers.IsNullOrEmpty(str) && basicLists.IndexOfString(str) < 0)
							{
								basicLists.Add(str);
								Type[] typeArray1 = new Type[] { base.MapType(typeof(string)) };
								ConstructorInfo constructorInfo = type1.GetConstructor(typeArray1);
								object[] objArray = new object[] { str };
								asm.SetCustomAttribute(new CustomAttributeBuilder(constructorInfo, objArray));
							}
						}
					}
				}
			}
		}

		private TypeBuilder WriteBasicTypeModel(RuntimeTypeModel.CompilerOptions options, string typeName, ModuleBuilder module)
		{
			Type type = base.MapType(typeof(TypeModel));
			TypeAttributes attributes = type.Attributes & (TypeAttributes.VisibilityMask | TypeAttributes.Public | TypeAttributes.NestedPublic | TypeAttributes.NestedPrivate | TypeAttributes.NestedFamily | TypeAttributes.NestedAssembly | TypeAttributes.NestedFamANDAssem | TypeAttributes.NestedFamORAssem | TypeAttributes.LayoutMask | TypeAttributes.SequentialLayout | TypeAttributes.ExplicitLayout | TypeAttributes.ClassSemanticsMask | TypeAttributes.Interface | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.Serializable | TypeAttributes.StringFormatMask | TypeAttributes.UnicodeClass | TypeAttributes.AutoClass | TypeAttributes.CustomFormatClass | TypeAttributes.CustomFormatMask | TypeAttributes.BeforeFieldInit | TypeAttributes.ReservedMask | TypeAttributes.RTSpecialName | TypeAttributes.HasSecurity) | TypeAttributes.Sealed;
			if (options.Accessibility == RuntimeTypeModel.Accessibility.Internal)
			{
				attributes = attributes & (TypeAttributes.NestedPublic | TypeAttributes.NestedFamily | TypeAttributes.NestedFamANDAssem | TypeAttributes.LayoutMask | TypeAttributes.SequentialLayout | TypeAttributes.ExplicitLayout | TypeAttributes.ClassSemanticsMask | TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.Serializable | TypeAttributes.StringFormatMask | TypeAttributes.UnicodeClass | TypeAttributes.AutoClass | TypeAttributes.CustomFormatClass | TypeAttributes.CustomFormatMask | TypeAttributes.BeforeFieldInit | TypeAttributes.ReservedMask | TypeAttributes.RTSpecialName | TypeAttributes.HasSecurity);
			}
			return module.DefineType(typeName, attributes, type);
		}

		private void WriteConstructors(TypeBuilder type, ref int index, RuntimeTypeModel.SerializerPair[] methodPairs, ref ILGenerator il, int knownTypesCategory, FieldBuilder knownTypes, Type knownTypesLookupType, CompilerContext ctx)
		{
			type.DefineDefaultConstructor(MethodAttributes.Public);
			il = type.DefineTypeInitializer().GetILGenerator();
			switch (knownTypesCategory)
			{
				case 1:
				{
					CompilerContext.LoadValue(il, this.types.Count);
					il.Emit(OpCodes.Newarr, ctx.MapType(typeof(Type)));
					index = 0;
					RuntimeTypeModel.SerializerPair[] serializerPairArray = methodPairs;
					for (int i = 0; i < (int)serializerPairArray.Length; i++)
					{
						RuntimeTypeModel.SerializerPair serializerPair = serializerPairArray[i];
						il.Emit(OpCodes.Dup);
						CompilerContext.LoadValue(il, index);
						il.Emit(OpCodes.Ldtoken, serializerPair.Type.Type);
						il.EmitCall(OpCodes.Call, ctx.MapType(typeof(Type)).GetMethod("GetTypeFromHandle"), null);
						il.Emit(OpCodes.Stelem_Ref);
						index++;
					}
					il.Emit(OpCodes.Stsfld, knownTypes);
					il.Emit(OpCodes.Ret);
					return;
				}
				case 2:
				{
					CompilerContext.LoadValue(il, this.types.Count);
					OpCode newobj = OpCodes.Newobj;
					Type[] typeArray = new Type[] { base.MapType(typeof(int)) };
					il.Emit(newobj, knownTypesLookupType.GetConstructor(typeArray));
					il.Emit(OpCodes.Stsfld, knownTypes);
					int num = 0;
					RuntimeTypeModel.SerializerPair[] serializerPairArray1 = methodPairs;
					for (int j = 0; j < (int)serializerPairArray1.Length; j++)
					{
						RuntimeTypeModel.SerializerPair serializerPair1 = serializerPairArray1[j];
						il.Emit(OpCodes.Ldsfld, knownTypes);
						il.Emit(OpCodes.Ldtoken, serializerPair1.Type.Type);
						il.EmitCall(OpCodes.Call, ctx.MapType(typeof(Type)).GetMethod("GetTypeFromHandle"), null);
						int num1 = num;
						num = num1 + 1;
						int num2 = num1;
						int baseKey = serializerPair1.BaseKey;
						if (baseKey != serializerPair1.MetaKey)
						{
							num2 = -1;
							int num3 = 0;
							while (num3 < (int)methodPairs.Length)
							{
								if (methodPairs[num3].BaseKey != baseKey || methodPairs[num3].MetaKey != baseKey)
								{
									num3++;
								}
								else
								{
									num2 = num3;
									break;
								}
							}
						}
						CompilerContext.LoadValue(il, num2);
						OpCode callvirt = OpCodes.Callvirt;
						Type[] typeArray1 = new Type[] { base.MapType(typeof(Type)), base.MapType(typeof(int)) };
						il.EmitCall(callvirt, knownTypesLookupType.GetMethod("Add", typeArray1), null);
					}
					il.Emit(OpCodes.Ret);
					return;
				}
				case 3:
				{
					CompilerContext.LoadValue(il, this.types.Count);
					OpCode opCode = OpCodes.Newobj;
					Type[] typeArray2 = new Type[] { base.MapType(typeof(int)) };
					il.Emit(opCode, knownTypesLookupType.GetConstructor(typeArray2));
					il.Emit(OpCodes.Stsfld, knownTypes);
					int num4 = 0;
					RuntimeTypeModel.SerializerPair[] serializerPairArray2 = methodPairs;
					for (int k = 0; k < (int)serializerPairArray2.Length; k++)
					{
						RuntimeTypeModel.SerializerPair serializerPair2 = serializerPairArray2[k];
						il.Emit(OpCodes.Ldsfld, knownTypes);
						il.Emit(OpCodes.Ldtoken, serializerPair2.Type.Type);
						il.EmitCall(OpCodes.Call, ctx.MapType(typeof(Type)).GetMethod("GetTypeFromHandle"), null);
						int num5 = num4;
						num4 = num5 + 1;
						int num6 = num5;
						int baseKey1 = serializerPair2.BaseKey;
						if (baseKey1 != serializerPair2.MetaKey)
						{
							num6 = -1;
							int num7 = 0;
							while (num7 < (int)methodPairs.Length)
							{
								if (methodPairs[num7].BaseKey != baseKey1 || methodPairs[num7].MetaKey != baseKey1)
								{
									num7++;
								}
								else
								{
									num6 = num7;
									break;
								}
							}
						}
						CompilerContext.LoadValue(il, num6);
						il.Emit(OpCodes.Box, base.MapType(typeof(int)));
						OpCode callvirt1 = OpCodes.Callvirt;
						Type[] typeArray3 = new Type[] { base.MapType(typeof(object)), base.MapType(typeof(object)) };
						il.EmitCall(callvirt1, knownTypesLookupType.GetMethod("Add", typeArray3), null);
					}
					il.Emit(OpCodes.Ret);
					return;
				}
			}
			throw new InvalidOperationException();
		}

		private void WriteGetKeyImpl(TypeBuilder type, bool hasInheritance, RuntimeTypeModel.SerializerPair[] methodPairs, CompilerContext.ILVersion ilVersion, string assemblyName, out ILGenerator il, out int knownTypesCategory, out FieldBuilder knownTypes, out Type knownTypesLookupType)
		{
			il = RuntimeTypeModel.Override(type, "GetKeyImpl");
			CompilerContext compilerContext = new CompilerContext(il, false, false, methodPairs, this, ilVersion, assemblyName, this.MapType(typeof(Type), true));
			if (this.types.Count > 20)
			{
				knownTypesLookupType = this.MapType(typeof(Dictionary<Type, int>), false);
				if (knownTypesLookupType != null)
				{
					knownTypesCategory = 2;
				}
				else
				{
					knownTypesLookupType = this.MapType(typeof(Hashtable), true);
					knownTypesCategory = 3;
				}
			}
			else
			{
				knownTypesCategory = 1;
				knownTypesLookupType = this.MapType(typeof(Type[]), true);
			}
			knownTypes = type.DefineField("knownTypes", knownTypesLookupType, FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);
			switch (knownTypesCategory)
			{
				case 1:
				{
					il.Emit(OpCodes.Ldsfld, knownTypes);
					il.Emit(OpCodes.Ldarg_1);
					OpCode callvirt = OpCodes.Callvirt;
					Type type1 = base.MapType(typeof(IList));
					Type[] typeArray = new Type[] { base.MapType(typeof(object)) };
					il.EmitCall(callvirt, type1.GetMethod("IndexOf", typeArray), null);
					if (!hasInheritance)
					{
						il.Emit(OpCodes.Ret);
						return;
					}
					il.DeclareLocal(base.MapType(typeof(int)));
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Stloc_0);
					BasicList basicLists = new BasicList();
					int baseKey = -1;
					for (int i = 0; i < (int)methodPairs.Length && methodPairs[i].MetaKey != methodPairs[i].BaseKey; i++)
					{
						if (baseKey != methodPairs[i].BaseKey)
						{
							basicLists.Add(compilerContext.DefineLabel());
							baseKey = methodPairs[i].BaseKey;
						}
						else
						{
							basicLists.Add(basicLists[basicLists.Count - 1]);
						}
					}
					CodeLabel[] codeLabelArray = new CodeLabel[basicLists.Count];
					basicLists.CopyTo(codeLabelArray, 0);
					compilerContext.Switch(codeLabelArray);
					il.Emit(OpCodes.Ldloc_0);
					il.Emit(OpCodes.Ret);
					baseKey = -1;
					for (int j = (int)codeLabelArray.Length - 1; j >= 0; j--)
					{
						if (baseKey != methodPairs[j].BaseKey)
						{
							baseKey = methodPairs[j].BaseKey;
							int num = -1;
							int length = (int)codeLabelArray.Length;
							while (length < (int)methodPairs.Length)
							{
								if (methodPairs[length].BaseKey != baseKey || methodPairs[length].MetaKey != baseKey)
								{
									length++;
								}
								else
								{
									num = length;
									break;
								}
							}
							compilerContext.MarkLabel(codeLabelArray[j]);
							CompilerContext.LoadValue(il, num);
							il.Emit(OpCodes.Ret);
						}
					}
					return;
				}
				case 2:
				{
					LocalBuilder localBuilder = il.DeclareLocal(base.MapType(typeof(int)));
					Label label = il.DefineLabel();
					il.Emit(OpCodes.Ldsfld, knownTypes);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldloca_S, localBuilder);
					il.EmitCall(OpCodes.Callvirt, knownTypesLookupType.GetMethod("TryGetValue", BindingFlags.Instance | BindingFlags.Public), null);
					il.Emit(OpCodes.Brfalse_S, label);
					il.Emit(OpCodes.Ldloc_S, localBuilder);
					il.Emit(OpCodes.Ret);
					il.MarkLabel(label);
					il.Emit(OpCodes.Ldc_I4_M1);
					il.Emit(OpCodes.Ret);
					return;
				}
				case 3:
				{
					Label label1 = il.DefineLabel();
					il.Emit(OpCodes.Ldsfld, knownTypes);
					il.Emit(OpCodes.Ldarg_1);
					il.EmitCall(OpCodes.Callvirt, knownTypesLookupType.GetProperty("Item").GetGetMethod(), null);
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Brfalse_S, label1);
					if (ilVersion != CompilerContext.ILVersion.Net1)
					{
						il.Emit(OpCodes.Unbox_Any, base.MapType(typeof(int)));
					}
					else
					{
						il.Emit(OpCodes.Unbox, base.MapType(typeof(int)));
						il.Emit(OpCodes.Ldobj, base.MapType(typeof(int)));
					}
					il.Emit(OpCodes.Ret);
					il.MarkLabel(label1);
					il.Emit(OpCodes.Pop);
					il.Emit(OpCodes.Ldc_I4_M1);
					il.Emit(OpCodes.Ret);
					return;
				}
			}
			throw new InvalidOperationException();
		}

		private CompilerContext WriteSerializeDeserialize(string assemblyName, TypeBuilder type, RuntimeTypeModel.SerializerPair[] methodPairs, CompilerContext.ILVersion ilVersion, ref ILGenerator il)
		{
			il = RuntimeTypeModel.Override(type, "Serialize");
			CompilerContext compilerContext = new CompilerContext(il, false, true, methodPairs, this, ilVersion, assemblyName, base.MapType(typeof(object)));
			CodeLabel[] codeLabelArray = new CodeLabel[this.types.Count];
			for (int i = 0; i < (int)codeLabelArray.Length; i++)
			{
				codeLabelArray[i] = compilerContext.DefineLabel();
			}
			il.Emit(OpCodes.Ldarg_1);
			compilerContext.Switch(codeLabelArray);
			compilerContext.Return();
			for (int j = 0; j < (int)codeLabelArray.Length; j++)
			{
				RuntimeTypeModel.SerializerPair serializerPair = methodPairs[j];
				compilerContext.MarkLabel(codeLabelArray[j]);
				il.Emit(OpCodes.Ldarg_2);
				compilerContext.CastFromObject(serializerPair.Type.Type);
				il.Emit(OpCodes.Ldarg_3);
				il.EmitCall(OpCodes.Call, serializerPair.Serialize, null);
				compilerContext.Return();
			}
			il = RuntimeTypeModel.Override(type, "Deserialize");
			compilerContext = new CompilerContext(il, false, false, methodPairs, this, ilVersion, assemblyName, base.MapType(typeof(object)));
			for (int k = 0; k < (int)codeLabelArray.Length; k++)
			{
				codeLabelArray[k] = compilerContext.DefineLabel();
			}
			il.Emit(OpCodes.Ldarg_1);
			compilerContext.Switch(codeLabelArray);
			compilerContext.LoadNullRef();
			compilerContext.Return();
			for (int l = 0; l < (int)codeLabelArray.Length; l++)
			{
				RuntimeTypeModel.SerializerPair serializerPair1 = methodPairs[l];
				compilerContext.MarkLabel(codeLabelArray[l]);
				Type type1 = serializerPair1.Type.Type;
				if (!type1.IsValueType)
				{
					il.Emit(OpCodes.Ldarg_2);
					compilerContext.CastFromObject(type1);
					il.Emit(OpCodes.Ldarg_3);
					il.EmitCall(OpCodes.Call, serializerPair1.Deserialize, null);
					compilerContext.Return();
				}
				else
				{
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Ldarg_3);
					il.EmitCall(OpCodes.Call, RuntimeTypeModel.EmitBoxedSerializer(type, l, type1, methodPairs, this, ilVersion, assemblyName), null);
					compilerContext.Return();
				}
			}
			return compilerContext;
		}

		private void WriteSerializers(RuntimeTypeModel.CompilerOptions options, string assemblyName, TypeBuilder type, out int index, out bool hasInheritance, out RuntimeTypeModel.SerializerPair[] methodPairs, out CompilerContext.ILVersion ilVersion)
		{
			index = 0;
			hasInheritance = false;
			methodPairs = new RuntimeTypeModel.SerializerPair[this.types.Count];
			BasicList.NodeEnumerator enumerator = this.types.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MetaType current = (MetaType)enumerator.Current;
				Type type1 = base.MapType(typeof(void));
				Type[] typeArray = new Type[] { current.Type, base.MapType(typeof(ProtoWriter)) };
				MethodBuilder methodBuilder = type.DefineMethod("Write", MethodAttributes.Private | MethodAttributes.Static, CallingConventions.Standard, type1, typeArray);
				Type type2 = current.Type;
				Type[] typeArray1 = new Type[] { current.Type, base.MapType(typeof(ProtoReader)) };
				MethodBuilder methodBuilder1 = type.DefineMethod("Read", MethodAttributes.Private | MethodAttributes.Static, CallingConventions.Standard, type2, typeArray1);
				RuntimeTypeModel.SerializerPair serializerPair = new RuntimeTypeModel.SerializerPair(this.GetKey(current.Type, true, false), this.GetKey(current.Type, true, true), current, methodBuilder, methodBuilder1, methodBuilder.GetILGenerator(), methodBuilder1.GetILGenerator());
				int num = index;
				int num1 = num;
				index = num + 1;
				methodPairs[num1] = serializerPair;
				if (serializerPair.MetaKey == serializerPair.BaseKey)
				{
					continue;
				}
				hasInheritance = true;
			}
			if (hasInheritance)
			{
				Array.Sort<RuntimeTypeModel.SerializerPair>(methodPairs);
			}
			ilVersion = CompilerContext.ILVersion.Net2;
			if (options.MetaDataVersion == 65536)
			{
				ilVersion = CompilerContext.ILVersion.Net1;
			}
			index = 0;
			while (index < (int)methodPairs.Length)
			{
				RuntimeTypeModel.SerializerPair serializerPair1 = methodPairs[index];
				CompilerContext compilerContext = new CompilerContext(serializerPair1.SerializeBody, true, true, methodPairs, this, (CompilerContext.ILVersion)((int)ilVersion), assemblyName, serializerPair1.Type.Type);
				compilerContext.CheckAccessibility(serializerPair1.Deserialize.ReturnType);
				serializerPair1.Type.Serializer.EmitWrite(compilerContext, compilerContext.InputValue);
				compilerContext.Return();
				compilerContext = new CompilerContext(serializerPair1.DeserializeBody, true, false, methodPairs, this, (CompilerContext.ILVersion)((int)ilVersion), assemblyName, serializerPair1.Type.Type);
				serializerPair1.Type.Serializer.EmitRead(compilerContext, compilerContext.InputValue);
				if (!serializerPair1.Type.Serializer.ReturnsValue)
				{
					compilerContext.LoadValue(compilerContext.InputValue);
				}
				compilerContext.Return();
				index++;
			}
		}

		public event LockContentedEventHandler LockContended;

		public enum Accessibility
		{
			Public,
			Internal
		}

		private sealed class BasicType
		{
			private readonly Type type;

			private readonly IProtoSerializer serializer;

			public IProtoSerializer Serializer
			{
				get
				{
					return this.serializer;
				}
			}

			public Type Type
			{
				get
				{
					return this.type;
				}
			}

			public BasicType(Type type, IProtoSerializer serializer)
			{
				this.type = type;
				this.serializer = serializer;
			}
		}

		public sealed class CompilerOptions
		{
			private string targetFrameworkName;

			private string targetFrameworkDisplayName;

			private string typeName;

			private string outputPath;

			private string imageRuntimeVersion;

			private int metaDataVersion;

			private RuntimeTypeModel.Accessibility accessibility;

			public RuntimeTypeModel.Accessibility Accessibility
			{
				get
				{
					return this.accessibility;
				}
				set
				{
					this.accessibility = value;
				}
			}

			public string ImageRuntimeVersion
			{
				get
				{
					return this.imageRuntimeVersion;
				}
				set
				{
					this.imageRuntimeVersion = value;
				}
			}

			public int MetaDataVersion
			{
				get
				{
					return this.metaDataVersion;
				}
				set
				{
					this.metaDataVersion = value;
				}
			}

			public string OutputPath
			{
				get
				{
					return this.outputPath;
				}
				set
				{
					this.outputPath = value;
				}
			}

			public string TargetFrameworkDisplayName
			{
				get
				{
					return this.targetFrameworkDisplayName;
				}
				set
				{
					this.targetFrameworkDisplayName = value;
				}
			}

			public string TargetFrameworkName
			{
				get
				{
					return this.targetFrameworkName;
				}
				set
				{
					this.targetFrameworkName = value;
				}
			}

			public string TypeName
			{
				get
				{
					return this.typeName;
				}
				set
				{
					this.typeName = value;
				}
			}

			public CompilerOptions()
			{
			}

			public void SetFrameworkOptions(MetaType from)
			{
				object obj;
				if (from == null)
				{
					throw new ArgumentNullException("from");
				}
				AttributeMap[] attributeMapArray = AttributeMap.Create(from.Model, from.Type.Assembly);
				AttributeMap[] attributeMapArray1 = attributeMapArray;
				int num = 0;
				while (num < (int)attributeMapArray1.Length)
				{
					AttributeMap attributeMap = attributeMapArray1[num];
					if (attributeMap.AttributeType.FullName != "System.Runtime.Versioning.TargetFrameworkAttribute")
					{
						num++;
					}
					else
					{
						if (attributeMap.TryGet("FrameworkName", out obj))
						{
							this.TargetFrameworkName = (string)obj;
						}
						if (!attributeMap.TryGet("FrameworkDisplayName", out obj))
						{
							break;
						}
						this.TargetFrameworkDisplayName = (string)obj;
						return;
					}
				}
			}
		}

		internal sealed class SerializerPair : IComparable
		{
			public readonly int MetaKey;

			public readonly int BaseKey;

			public readonly MetaType Type;

			public readonly MethodBuilder Serialize;

			public readonly MethodBuilder Deserialize;

			public readonly ILGenerator SerializeBody;

			public readonly ILGenerator DeserializeBody;

			public SerializerPair(int metaKey, int baseKey, MetaType type, MethodBuilder serialize, MethodBuilder deserialize, ILGenerator serializeBody, ILGenerator deserializeBody)
			{
				this.MetaKey = metaKey;
				this.BaseKey = baseKey;
				this.Serialize = serialize;
				this.Deserialize = deserialize;
				this.SerializeBody = serializeBody;
				this.DeserializeBody = deserializeBody;
				this.Type = type;
			}

			int System.IComparable.CompareTo(object obj)
			{
				if (obj == null)
				{
					throw new ArgumentException("obj");
				}
				RuntimeTypeModel.SerializerPair serializerPair = (RuntimeTypeModel.SerializerPair)obj;
				if (this.BaseKey == this.MetaKey)
				{
					if (serializerPair.BaseKey != serializerPair.MetaKey)
					{
						return 1;
					}
					return this.MetaKey.CompareTo(serializerPair.MetaKey);
				}
				if (serializerPair.BaseKey == serializerPair.MetaKey)
				{
					return -1;
				}
				int num = this.BaseKey.CompareTo(serializerPair.BaseKey);
				if (num == 0)
				{
					num = this.MetaKey.CompareTo(serializerPair.MetaKey);
				}
				return num;
			}
		}

		private sealed class Singleton
		{
			internal readonly static RuntimeTypeModel Value;

			static Singleton()
			{
				RuntimeTypeModel.Singleton.Value = new RuntimeTypeModel(true);
			}

			private Singleton()
			{
			}
		}
	}
}