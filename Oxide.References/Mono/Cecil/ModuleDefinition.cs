using Mono;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Mono.Cecil
{
	public sealed class ModuleDefinition : ModuleReference, Mono.Cecil.ICustomAttributeProvider, IMetadataTokenProvider
	{
		internal Mono.Cecil.PE.Image Image;

		internal Mono.Cecil.MetadataSystem MetadataSystem;

		internal Mono.Cecil.ReadingMode ReadingMode;

		internal ISymbolReaderProvider SymbolReaderProvider;

		internal ISymbolReader symbol_reader;

		internal IAssemblyResolver assembly_resolver;

		internal IMetadataResolver metadata_resolver;

		internal Mono.Cecil.TypeSystem type_system;

		private readonly MetadataReader reader;

		private readonly string fq_name;

		internal string runtime_version;

		internal ModuleKind kind;

		private TargetRuntime runtime;

		private TargetArchitecture architecture;

		private ModuleAttributes attributes;

		private ModuleCharacteristics characteristics;

		private Guid mvid;

		internal AssemblyDefinition assembly;

		private MethodDefinition entry_point;

		private Mono.Cecil.MetadataImporter importer;

		private Collection<Mono.Cecil.CustomAttribute> custom_attributes;

		private Collection<AssemblyNameReference> references;

		private Collection<ModuleReference> modules;

		private Collection<Resource> resources;

		private Collection<ExportedType> exported_types;

		private TypeDefinitionCollection types;

		private readonly object module_lock = new object();

		public TargetArchitecture Architecture
		{
			get
			{
				return this.architecture;
			}
			set
			{
				this.architecture = value;
			}
		}

		public AssemblyDefinition Assembly
		{
			get
			{
				return this.assembly;
			}
		}

		public Collection<AssemblyNameReference> AssemblyReferences
		{
			get
			{
				if (this.references != null)
				{
					return this.references;
				}
				if (!this.HasImage)
				{
					Collection<AssemblyNameReference> assemblyNameReferences = new Collection<AssemblyNameReference>();
					Collection<AssemblyNameReference> assemblyNameReferences1 = assemblyNameReferences;
					this.references = assemblyNameReferences;
					return assemblyNameReferences1;
				}
				return this.Read<ModuleDefinition, Collection<AssemblyNameReference>>(ref this.references, this, (ModuleDefinition _, MetadataReader reader) => reader.ReadAssemblyReferences());
			}
		}

		public IAssemblyResolver AssemblyResolver
		{
			get
			{
				if (this.assembly_resolver == null)
				{
					Interlocked.CompareExchange<IAssemblyResolver>(ref this.assembly_resolver, new DefaultAssemblyResolver(), null);
				}
				return this.assembly_resolver;
			}
		}

		public ModuleAttributes Attributes
		{
			get
			{
				return this.attributes;
			}
			set
			{
				this.attributes = value;
			}
		}

		public ModuleCharacteristics Characteristics
		{
			get
			{
				return this.characteristics;
			}
			set
			{
				this.characteristics = value;
			}
		}

		public Collection<Mono.Cecil.CustomAttribute> CustomAttributes
		{
			get
			{
				return this.custom_attributes ?? this.GetCustomAttributes(ref this.custom_attributes, this);
			}
		}

		public MethodDefinition EntryPoint
		{
			get
			{
				if (this.entry_point != null)
				{
					return this.entry_point;
				}
				if (!this.HasImage)
				{
					object obj = null;
					MethodDefinition methodDefinition = (MethodDefinition)obj;
					this.entry_point = (MethodDefinition)obj;
					return methodDefinition;
				}
				return this.Read<ModuleDefinition, MethodDefinition>(ref this.entry_point, this, (ModuleDefinition _, MetadataReader reader) => reader.ReadEntryPoint());
			}
			set
			{
				this.entry_point = value;
			}
		}

		public Collection<ExportedType> ExportedTypes
		{
			get
			{
				if (this.exported_types != null)
				{
					return this.exported_types;
				}
				if (!this.HasImage)
				{
					Collection<ExportedType> exportedTypes = new Collection<ExportedType>();
					Collection<ExportedType> exportedTypes1 = exportedTypes;
					this.exported_types = exportedTypes;
					return exportedTypes1;
				}
				return this.Read<ModuleDefinition, Collection<ExportedType>>(ref this.exported_types, this, (ModuleDefinition _, MetadataReader reader) => reader.ReadExportedTypes());
			}
		}

		public string FullyQualifiedName
		{
			get
			{
				return this.fq_name;
			}
		}

		public bool HasAssemblyReferences
		{
			get
			{
				if (this.references != null)
				{
					return this.references.Count > 0;
				}
				if (!this.HasImage)
				{
					return false;
				}
				return this.Image.HasTable(Table.AssemblyRef);
			}
		}

		public bool HasCustomAttributes
		{
			get
			{
				if (this.custom_attributes == null)
				{
					return this.GetHasCustomAttributes(this);
				}
				return this.custom_attributes.Count > 0;
			}
		}

		public bool HasDebugHeader
		{
			get
			{
				if (this.Image == null)
				{
					return false;
				}
				return !this.Image.Debug.IsZero;
			}
		}

		public bool HasExportedTypes
		{
			get
			{
				if (this.exported_types != null)
				{
					return this.exported_types.Count > 0;
				}
				if (!this.HasImage)
				{
					return false;
				}
				return this.Image.HasTable(Table.ExportedType);
			}
		}

		internal bool HasImage
		{
			get
			{
				return this.Image != null;
			}
		}

		public bool HasModuleReferences
		{
			get
			{
				if (this.modules != null)
				{
					return this.modules.Count > 0;
				}
				if (!this.HasImage)
				{
					return false;
				}
				return this.Image.HasTable(Table.ModuleRef);
			}
		}

		public bool HasResources
		{
			get
			{
				if (this.resources != null)
				{
					return this.resources.Count > 0;
				}
				if (!this.HasImage)
				{
					return false;
				}
				if (this.Image.HasTable(Table.ManifestResource))
				{
					return true;
				}
				return this.Read<ModuleDefinition, bool>(this, (ModuleDefinition _, MetadataReader reader) => reader.HasFileResource());
			}
		}

		public bool HasSymbols
		{
			get
			{
				return this.symbol_reader != null;
			}
		}

		public bool HasTypes
		{
			get
			{
				if (this.types != null)
				{
					return this.types.Count > 0;
				}
				if (!this.HasImage)
				{
					return false;
				}
				return this.Image.HasTable(Table.TypeDef);
			}
		}

		public bool IsMain
		{
			get
			{
				return this.kind != ModuleKind.NetModule;
			}
		}

		public ModuleKind Kind
		{
			get
			{
				return this.kind;
			}
			set
			{
				this.kind = value;
			}
		}

		internal Mono.Cecil.MetadataImporter MetadataImporter
		{
			get
			{
				if (this.importer == null)
				{
					Interlocked.CompareExchange<Mono.Cecil.MetadataImporter>(ref this.importer, new Mono.Cecil.MetadataImporter(this), null);
				}
				return this.importer;
			}
		}

		public IMetadataResolver MetadataResolver
		{
			get
			{
				if (this.metadata_resolver == null)
				{
					Interlocked.CompareExchange<IMetadataResolver>(ref this.metadata_resolver, new Mono.Cecil.MetadataResolver(this.AssemblyResolver), null);
				}
				return this.metadata_resolver;
			}
		}

		public override Mono.Cecil.MetadataScopeType MetadataScopeType
		{
			get
			{
				return Mono.Cecil.MetadataScopeType.ModuleDefinition;
			}
		}

		public Collection<ModuleReference> ModuleReferences
		{
			get
			{
				if (this.modules != null)
				{
					return this.modules;
				}
				if (!this.HasImage)
				{
					Collection<ModuleReference> moduleReferences = new Collection<ModuleReference>();
					Collection<ModuleReference> moduleReferences1 = moduleReferences;
					this.modules = moduleReferences;
					return moduleReferences1;
				}
				return this.Read<ModuleDefinition, Collection<ModuleReference>>(ref this.modules, this, (ModuleDefinition _, MetadataReader reader) => reader.ReadModuleReferences());
			}
		}

		public Guid Mvid
		{
			get
			{
				return this.mvid;
			}
			set
			{
				this.mvid = value;
			}
		}

		public Collection<Resource> Resources
		{
			get
			{
				if (this.resources != null)
				{
					return this.resources;
				}
				if (!this.HasImage)
				{
					Collection<Resource> resources = new Collection<Resource>();
					Collection<Resource> resources1 = resources;
					this.resources = resources;
					return resources1;
				}
				return this.Read<ModuleDefinition, Collection<Resource>>(ref this.resources, this, (ModuleDefinition _, MetadataReader reader) => reader.ReadResources());
			}
		}

		public TargetRuntime Runtime
		{
			get
			{
				return this.runtime;
			}
			set
			{
				this.runtime = value;
				this.runtime_version = this.runtime.RuntimeVersionString();
			}
		}

		public string RuntimeVersion
		{
			get
			{
				return this.runtime_version;
			}
			set
			{
				this.runtime_version = value;
				this.runtime = this.runtime_version.ParseRuntime();
			}
		}

		public ISymbolReader SymbolReader
		{
			get
			{
				return this.symbol_reader;
			}
		}

		internal object SyncRoot
		{
			get
			{
				return this.module_lock;
			}
		}

		public Collection<TypeDefinition> Types
		{
			get
			{
				if (this.types != null)
				{
					return this.types;
				}
				if (!this.HasImage)
				{
					TypeDefinitionCollection typeDefinitionCollection = new TypeDefinitionCollection(this);
					TypeDefinitionCollection typeDefinitionCollection1 = typeDefinitionCollection;
					this.types = typeDefinitionCollection;
					return typeDefinitionCollection1;
				}
				return this.Read<ModuleDefinition, TypeDefinitionCollection>(ref this.types, this, (ModuleDefinition _, MetadataReader reader) => reader.ReadTypes());
			}
		}

		public Mono.Cecil.TypeSystem TypeSystem
		{
			get
			{
				if (this.type_system == null)
				{
					Interlocked.CompareExchange<Mono.Cecil.TypeSystem>(ref this.type_system, Mono.Cecil.TypeSystem.CreateTypeSystem(this), null);
				}
				return this.type_system;
			}
		}

		internal ModuleDefinition()
		{
			this.MetadataSystem = new Mono.Cecil.MetadataSystem();
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.Module, 1);
		}

		internal ModuleDefinition(Mono.Cecil.PE.Image image) : this()
		{
			this.Image = image;
			this.kind = image.Kind;
			this.RuntimeVersion = image.RuntimeVersion;
			this.architecture = image.Architecture;
			this.attributes = image.Attributes;
			this.characteristics = image.Characteristics;
			this.fq_name = image.FileName;
			this.reader = new MetadataReader(this);
		}

		private static void CheckContext(IGenericParameterProvider context, ModuleDefinition module)
		{
			if (context == null)
			{
				return;
			}
			if (context.Module != module)
			{
				throw new ArgumentException();
			}
		}

		private static void CheckField(object field)
		{
			if (field == null)
			{
				throw new ArgumentNullException("field");
			}
		}

		private static void CheckFullName(string fullName)
		{
			if (fullName == null)
			{
				throw new ArgumentNullException("fullName");
			}
			if (fullName.Length == 0)
			{
				throw new ArgumentException();
			}
		}

		private static void CheckMethod(object method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
		}

		private static void CheckStream(object stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
		}

		private static void CheckType(object type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
		}

		private static AssemblyNameDefinition CreateAssemblyName(string name)
		{
			if (name.EndsWith(".dll") || name.EndsWith(".exe"))
			{
				name = name.Substring(0, name.Length - 4);
			}
			return new AssemblyNameDefinition(name, new Version(0, 0, 0, 0));
		}

		public static ModuleDefinition CreateModule(string name, ModuleKind kind)
		{
			return ModuleDefinition.CreateModule(name, new ModuleParameters()
			{
				Kind = kind
			});
		}

		public static ModuleDefinition CreateModule(string name, ModuleParameters parameters)
		{
			Mixin.CheckName(name);
			Mixin.CheckParameters(parameters);
			ModuleDefinition moduleDefinition = new ModuleDefinition()
			{
				Name = name,
				kind = parameters.Kind,
				Runtime = parameters.Runtime,
				architecture = parameters.Architecture,
				mvid = Guid.NewGuid(),
				Attributes = ModuleAttributes.ILOnly,
				Characteristics = ModuleCharacteristics.DynamicBase | ModuleCharacteristics.NoSEH | ModuleCharacteristics.NXCompat | ModuleCharacteristics.TerminalServerAware
			};
			if (parameters.AssemblyResolver != null)
			{
				moduleDefinition.assembly_resolver = parameters.AssemblyResolver;
			}
			if (parameters.MetadataResolver != null)
			{
				moduleDefinition.metadata_resolver = parameters.MetadataResolver;
			}
			if (parameters.Kind != ModuleKind.NetModule)
			{
				AssemblyDefinition assemblyDefinition = new AssemblyDefinition();
				moduleDefinition.assembly = assemblyDefinition;
				moduleDefinition.assembly.Name = ModuleDefinition.CreateAssemblyName(name);
				assemblyDefinition.main_module = moduleDefinition;
			}
			moduleDefinition.Types.Add(new TypeDefinition(string.Empty, "<Module>", Mono.Cecil.TypeAttributes.NotPublic));
			return moduleDefinition;
		}

		private static ImportGenericContext GenericContextFor(IGenericParameterProvider context)
		{
			if (context != null)
			{
				return new ImportGenericContext(context);
			}
			return new ImportGenericContext();
		}

		public ImageDebugDirectory GetDebugHeader(out byte[] header)
		{
			if (!this.HasDebugHeader)
			{
				throw new InvalidOperationException();
			}
			return this.Image.GetDebugHeader(out header);
		}

		private static Stream GetFileStream(string fileName, FileMode mode, FileAccess access, FileShare share)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			if (fileName.Length == 0)
			{
				throw new ArgumentException();
			}
			return new FileStream(fileName, mode, access, share);
		}

		public IEnumerable<MemberReference> GetMemberReferences()
		{
			if (!this.HasImage)
			{
				return Empty<MemberReference>.Array;
			}
			return this.Read<ModuleDefinition, IEnumerable<MemberReference>>(this, (ModuleDefinition _, MetadataReader reader) => reader.GetMemberReferences());
		}

		private TypeDefinition GetNestedType(string fullname)
		{
			string[] strArrays = fullname.Split(new char[] { '/' });
			TypeDefinition type = this.GetType(strArrays[0]);
			if (type == null)
			{
				return null;
			}
			for (int i = 1; i < (int)strArrays.Length; i++)
			{
				TypeDefinition nestedType = type.GetNestedType(strArrays[i]);
				if (nestedType == null)
				{
					return null;
				}
				type = nestedType;
			}
			return type;
		}

		public TypeReference GetType(string fullName, bool runtimeName)
		{
			if (!runtimeName)
			{
				return this.GetType(fullName);
			}
			return TypeParser.ParseType(this, fullName);
		}

		public TypeDefinition GetType(string fullName)
		{
			ModuleDefinition.CheckFullName(fullName);
			if (fullName.IndexOf('/') > 0)
			{
				return this.GetNestedType(fullName);
			}
			return ((TypeDefinitionCollection)this.Types).GetType(fullName);
		}

		public TypeDefinition GetType(string @namespace, string name)
		{
			Mixin.CheckName(name);
			return ((TypeDefinitionCollection)this.Types).GetType(@namespace ?? string.Empty, name);
		}

		private TypeReference GetTypeReference(string scope, string fullname)
		{
			return this.Read<Row<string, string>, TypeReference>(new Row<string, string>(scope, fullname), (Row<string, string> row, MetadataReader reader) => reader.GetTypeReference(row.Col1, row.Col2));
		}

		public IEnumerable<TypeReference> GetTypeReferences()
		{
			if (!this.HasImage)
			{
				return Empty<TypeReference>.Array;
			}
			return this.Read<ModuleDefinition, IEnumerable<TypeReference>>(this, (ModuleDefinition _, MetadataReader reader) => reader.GetTypeReferences());
		}

		public IEnumerable<TypeDefinition> GetTypes()
		{
			return ModuleDefinition.GetTypes(this.Types);
		}

		private static IEnumerable<TypeDefinition> GetTypes(Collection<TypeDefinition> types)
		{
			for (int i = 0; i < types.Count; i++)
			{
				TypeDefinition item = types[i];
				yield return item;
				if (item.HasNestedTypes)
				{
					foreach (TypeDefinition typeDefinition in ModuleDefinition.GetTypes(item.NestedTypes))
					{
						yield return typeDefinition;
					}
					item = null;
				}
			}
		}

		public bool HasTypeReference(string fullName)
		{
			return this.HasTypeReference(string.Empty, fullName);
		}

		public bool HasTypeReference(string scope, string fullName)
		{
			ModuleDefinition.CheckFullName(fullName);
			if (!this.HasImage)
			{
				return false;
			}
			return this.GetTypeReference(scope, fullName) != null;
		}

		public TypeReference Import(Type type)
		{
			return this.Import(type, null);
		}

		public TypeReference Import(Type type, IGenericParameterProvider context)
		{
			ModuleDefinition.CheckType(type);
			ModuleDefinition.CheckContext(context, this);
			return this.MetadataImporter.ImportType(type, ModuleDefinition.GenericContextFor(context), (context != null ? ImportGenericKind.Open : ImportGenericKind.Definition));
		}

		public FieldReference Import(FieldInfo field)
		{
			return this.Import(field, null);
		}

		public FieldReference Import(FieldInfo field, IGenericParameterProvider context)
		{
			ModuleDefinition.CheckField(field);
			ModuleDefinition.CheckContext(context, this);
			return this.MetadataImporter.ImportField(field, ModuleDefinition.GenericContextFor(context));
		}

		public MethodReference Import(MethodBase method)
		{
			ModuleDefinition.CheckMethod(method);
			return this.MetadataImporter.ImportMethod(method, new ImportGenericContext(), ImportGenericKind.Definition);
		}

		public MethodReference Import(MethodBase method, IGenericParameterProvider context)
		{
			ModuleDefinition.CheckMethod(method);
			ModuleDefinition.CheckContext(context, this);
			return this.MetadataImporter.ImportMethod(method, ModuleDefinition.GenericContextFor(context), (context != null ? ImportGenericKind.Open : ImportGenericKind.Definition));
		}

		public TypeReference Import(TypeReference type)
		{
			ModuleDefinition.CheckType(type);
			if (type.Module == this)
			{
				return type;
			}
			return this.MetadataImporter.ImportType(type, new ImportGenericContext());
		}

		public TypeReference Import(TypeReference type, IGenericParameterProvider context)
		{
			ModuleDefinition.CheckType(type);
			if (type.Module == this)
			{
				return type;
			}
			ModuleDefinition.CheckContext(context, this);
			return this.MetadataImporter.ImportType(type, ModuleDefinition.GenericContextFor(context));
		}

		public FieldReference Import(FieldReference field)
		{
			ModuleDefinition.CheckField(field);
			if (field.Module == this)
			{
				return field;
			}
			return this.MetadataImporter.ImportField(field, new ImportGenericContext());
		}

		public FieldReference Import(FieldReference field, IGenericParameterProvider context)
		{
			ModuleDefinition.CheckField(field);
			if (field.Module == this)
			{
				return field;
			}
			ModuleDefinition.CheckContext(context, this);
			return this.MetadataImporter.ImportField(field, ModuleDefinition.GenericContextFor(context));
		}

		public MethodReference Import(MethodReference method)
		{
			return this.Import(method, null);
		}

		public MethodReference Import(MethodReference method, IGenericParameterProvider context)
		{
			ModuleDefinition.CheckMethod(method);
			if (method.Module == this)
			{
				return method;
			}
			ModuleDefinition.CheckContext(context, this);
			return this.MetadataImporter.ImportMethod(method, ModuleDefinition.GenericContextFor(context));
		}

		public IMetadataTokenProvider LookupToken(int token)
		{
			return this.LookupToken(new Mono.Cecil.MetadataToken((uint)token));
		}

		public IMetadataTokenProvider LookupToken(Mono.Cecil.MetadataToken token)
		{
			return this.Read<Mono.Cecil.MetadataToken, IMetadataTokenProvider>(token, (Mono.Cecil.MetadataToken t, MetadataReader reader) => reader.LookupToken(t));
		}

		private void ProcessDebugHeader()
		{
			byte[] numArray;
			if (!this.HasDebugHeader)
			{
				return;
			}
			ImageDebugDirectory debugHeader = this.GetDebugHeader(out numArray);
			if (!this.symbol_reader.ProcessDebugHeader(debugHeader, numArray))
			{
				throw new InvalidOperationException();
			}
		}

		internal TRet Read<TItem, TRet>(TItem item, Func<TItem, MetadataReader, TRet> read)
		{
			TRet tRet;
			lock (this.module_lock)
			{
				int num = this.reader.position;
				IGenericContext genericContext = this.reader.context;
				this.reader.position = num;
				this.reader.context = genericContext;
				tRet = read(item, this.reader);
			}
			return tRet;
		}

		internal TRet Read<TItem, TRet>(ref TRet variable, TItem item, Func<TItem, MetadataReader, TRet> read)
		where TRet : class
		{
			TRet tRet;
			lock (this.module_lock)
			{
				if (variable == null)
				{
					int num = this.reader.position;
					IGenericContext genericContext = this.reader.context;
					TRet tRet1 = read(item, this.reader);
					this.reader.position = num;
					this.reader.context = genericContext;
					TRet tRet2 = tRet1;
					TRet tRet3 = tRet2;
					variable = tRet2;
					tRet = tRet3;
				}
				else
				{
					tRet = variable;
				}
			}
			return tRet;
		}

		public static ModuleDefinition ReadModule(string fileName)
		{
			return ModuleDefinition.ReadModule(fileName, new ReaderParameters(Mono.Cecil.ReadingMode.Deferred));
		}

		public static ModuleDefinition ReadModule(Stream stream)
		{
			return ModuleDefinition.ReadModule(stream, new ReaderParameters(Mono.Cecil.ReadingMode.Deferred));
		}

		public static ModuleDefinition ReadModule(string fileName, ReaderParameters parameters)
		{
			ModuleDefinition moduleDefinition;
			using (Stream fileStream = ModuleDefinition.GetFileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				moduleDefinition = ModuleDefinition.ReadModule(fileStream, parameters);
			}
			return moduleDefinition;
		}

		public static ModuleDefinition ReadModule(Stream stream, ReaderParameters parameters)
		{
			ModuleDefinition.CheckStream(stream);
			if (!stream.CanRead || !stream.CanSeek)
			{
				throw new ArgumentException();
			}
			Mixin.CheckParameters(parameters);
			return ModuleReader.CreateModuleFrom(ImageReader.ReadImageFrom(stream), parameters);
		}

		public void ReadSymbols()
		{
			if (string.IsNullOrEmpty(this.fq_name))
			{
				throw new InvalidOperationException();
			}
			ISymbolReaderProvider platformReaderProvider = SymbolProvider.GetPlatformReaderProvider();
			if (platformReaderProvider == null)
			{
				throw new InvalidOperationException();
			}
			this.ReadSymbols(platformReaderProvider.GetSymbolReader(this, this.fq_name));
		}

		public void ReadSymbols(ISymbolReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.symbol_reader = reader;
			this.ProcessDebugHeader();
		}

		internal FieldDefinition Resolve(FieldReference field)
		{
			return this.MetadataResolver.Resolve(field);
		}

		internal MethodDefinition Resolve(MethodReference method)
		{
			return this.MetadataResolver.Resolve(method);
		}

		internal TypeDefinition Resolve(TypeReference type)
		{
			return this.MetadataResolver.Resolve(type);
		}

		public bool TryGetTypeReference(string fullName, out TypeReference type)
		{
			return this.TryGetTypeReference(string.Empty, fullName, out type);
		}

		public bool TryGetTypeReference(string scope, string fullName, out TypeReference type)
		{
			ModuleDefinition.CheckFullName(fullName);
			if (!this.HasImage)
			{
				type = null;
				return false;
			}
			TypeReference typeReference = this.GetTypeReference(scope, fullName);
			TypeReference typeReference1 = typeReference;
			type = typeReference;
			return typeReference1 != null;
		}

		public void Write(string fileName)
		{
			this.Write(fileName, new WriterParameters());
		}

		public void Write(Stream stream)
		{
			this.Write(stream, new WriterParameters());
		}

		public void Write(string fileName, WriterParameters parameters)
		{
			using (Stream fileStream = ModuleDefinition.GetFileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
			{
				this.Write(fileStream, parameters);
			}
		}

		public void Write(Stream stream, WriterParameters parameters)
		{
			ModuleDefinition.CheckStream(stream);
			if (!stream.CanWrite || !stream.CanSeek)
			{
				throw new ArgumentException();
			}
			Mixin.CheckParameters(parameters);
			ModuleWriter.WriteModuleTo(this, stream, parameters);
		}
	}
}