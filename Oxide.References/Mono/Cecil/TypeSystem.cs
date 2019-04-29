using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil
{
	public abstract class TypeSystem
	{
		private readonly ModuleDefinition module;

		private TypeReference type_object;

		private TypeReference type_void;

		private TypeReference type_bool;

		private TypeReference type_char;

		private TypeReference type_sbyte;

		private TypeReference type_byte;

		private TypeReference type_int16;

		private TypeReference type_uint16;

		private TypeReference type_int32;

		private TypeReference type_uint32;

		private TypeReference type_int64;

		private TypeReference type_uint64;

		private TypeReference type_single;

		private TypeReference type_double;

		private TypeReference type_intptr;

		private TypeReference type_uintptr;

		private TypeReference type_string;

		private TypeReference type_typedref;

		public TypeReference Boolean
		{
			get
			{
				return this.type_bool ?? this.LookupSystemValueType(ref this.type_bool, "Boolean", ElementType.Boolean);
			}
		}

		public TypeReference Byte
		{
			get
			{
				return this.type_byte ?? this.LookupSystemValueType(ref this.type_byte, "Byte", ElementType.U1);
			}
		}

		public TypeReference Char
		{
			get
			{
				return this.type_char ?? this.LookupSystemValueType(ref this.type_char, "Char", ElementType.Char);
			}
		}

		public IMetadataScope Corlib
		{
			get
			{
				TypeSystem.CommonTypeSystem commonTypeSystem = this as TypeSystem.CommonTypeSystem;
				if (commonTypeSystem == null)
				{
					return this.module;
				}
				return commonTypeSystem.GetCorlibReference();
			}
		}

		public TypeReference Double
		{
			get
			{
				return this.type_double ?? this.LookupSystemValueType(ref this.type_double, "Double", ElementType.R8);
			}
		}

		public TypeReference Int16
		{
			get
			{
				return this.type_int16 ?? this.LookupSystemValueType(ref this.type_int16, "Int16", ElementType.I2);
			}
		}

		public TypeReference Int32
		{
			get
			{
				return this.type_int32 ?? this.LookupSystemValueType(ref this.type_int32, "Int32", ElementType.I4);
			}
		}

		public TypeReference Int64
		{
			get
			{
				return this.type_int64 ?? this.LookupSystemValueType(ref this.type_int64, "Int64", ElementType.I8);
			}
		}

		public TypeReference IntPtr
		{
			get
			{
				return this.type_intptr ?? this.LookupSystemValueType(ref this.type_intptr, "IntPtr", ElementType.I);
			}
		}

		public TypeReference Object
		{
			get
			{
				return this.type_object ?? this.LookupSystemType(ref this.type_object, "Object", ElementType.Object);
			}
		}

		public TypeReference SByte
		{
			get
			{
				return this.type_sbyte ?? this.LookupSystemValueType(ref this.type_sbyte, "SByte", ElementType.I1);
			}
		}

		public TypeReference Single
		{
			get
			{
				return this.type_single ?? this.LookupSystemValueType(ref this.type_single, "Single", ElementType.R4);
			}
		}

		public TypeReference String
		{
			get
			{
				return this.type_string ?? this.LookupSystemType(ref this.type_string, "String", ElementType.String);
			}
		}

		public TypeReference TypedReference
		{
			get
			{
				return this.type_typedref ?? this.LookupSystemValueType(ref this.type_typedref, "TypedReference", ElementType.TypedByRef);
			}
		}

		public TypeReference UInt16
		{
			get
			{
				return this.type_uint16 ?? this.LookupSystemValueType(ref this.type_uint16, "UInt16", ElementType.U2);
			}
		}

		public TypeReference UInt32
		{
			get
			{
				return this.type_uint32 ?? this.LookupSystemValueType(ref this.type_uint32, "UInt32", ElementType.U4);
			}
		}

		public TypeReference UInt64
		{
			get
			{
				return this.type_uint64 ?? this.LookupSystemValueType(ref this.type_uint64, "UInt64", ElementType.U8);
			}
		}

		public TypeReference UIntPtr
		{
			get
			{
				return this.type_uintptr ?? this.LookupSystemValueType(ref this.type_uintptr, "UIntPtr", ElementType.U);
			}
		}

		public TypeReference Void
		{
			get
			{
				return this.type_void ?? this.LookupSystemType(ref this.type_void, "Void", ElementType.Void);
			}
		}

		private TypeSystem(ModuleDefinition module)
		{
			this.module = module;
		}

		internal static TypeSystem CreateTypeSystem(ModuleDefinition module)
		{
			if (module.IsCorlib())
			{
				return new TypeSystem.CoreTypeSystem(module);
			}
			return new TypeSystem.CommonTypeSystem(module);
		}

		private TypeReference LookupSystemType(ref TypeReference reference, string name, ElementType element_type)
		{
			TypeReference typeReference;
			lock (this.module.SyncRoot)
			{
				if (reference == null)
				{
					TypeReference elementType = this.LookupType("System", name);
					elementType.etype = element_type;
					TypeReference typeReference1 = elementType;
					TypeReference typeReference2 = typeReference1;
					reference = typeReference1;
					typeReference = typeReference2;
				}
				else
				{
					typeReference = reference;
				}
			}
			return typeReference;
		}

		private TypeReference LookupSystemValueType(ref TypeReference typeRef, string name, ElementType element_type)
		{
			TypeReference typeReference;
			lock (this.module.SyncRoot)
			{
				if (typeRef == null)
				{
					TypeReference elementType = this.LookupType("System", name);
					elementType.etype = element_type;
					elementType.IsValueType = true;
					TypeReference typeReference1 = elementType;
					TypeReference typeReference2 = typeReference1;
					typeRef = typeReference1;
					typeReference = typeReference2;
				}
				else
				{
					typeReference = typeRef;
				}
			}
			return typeReference;
		}

		internal abstract TypeReference LookupType(string @namespace, string name);

		private sealed class CommonTypeSystem : TypeSystem
		{
			private AssemblyNameReference corlib;

			public CommonTypeSystem(ModuleDefinition module) : base(module)
			{
			}

			private TypeReference CreateTypeReference(string @namespace, string name)
			{
				return new TypeReference(@namespace, name, this.module, this.GetCorlibReference());
			}

			public AssemblyNameReference GetCorlibReference()
			{
				AssemblyNameReference assemblyNameReference;
				if (this.corlib != null)
				{
					return this.corlib;
				}
				Collection<AssemblyNameReference> assemblyReferences = this.module.AssemblyReferences;
				for (int i = 0; i < assemblyReferences.Count; i++)
				{
					AssemblyNameReference item = assemblyReferences[i];
					if (item.Name == "mscorlib")
					{
						AssemblyNameReference assemblyNameReference1 = item;
						assemblyNameReference = assemblyNameReference1;
						this.corlib = assemblyNameReference1;
						return assemblyNameReference;
					}
				}
				assemblyNameReference = new AssemblyNameReference()
				{
					Name = "mscorlib",
					Version = this.GetCorlibVersion(),
					PublicKeyToken = new byte[] { 183, 122, 92, 86, 25, 52, 224, 137 }
				};
				this.corlib = assemblyNameReference;
				assemblyReferences.Add(this.corlib);
				return this.corlib;
			}

			private Version GetCorlibVersion()
			{
				switch (this.module.Runtime)
				{
					case TargetRuntime.Net_1_0:
					case TargetRuntime.Net_1_1:
					{
						return new Version(1, 0, 0, 0);
					}
					case TargetRuntime.Net_2_0:
					{
						return new Version(2, 0, 0, 0);
					}
					case TargetRuntime.Net_4_0:
					{
						return new Version(4, 0, 0, 0);
					}
				}
				throw new NotSupportedException();
			}

			internal override TypeReference LookupType(string @namespace, string name)
			{
				return this.CreateTypeReference(@namespace, name);
			}
		}

		private sealed class CoreTypeSystem : TypeSystem
		{
			public CoreTypeSystem(ModuleDefinition module) : base(module)
			{
			}

			private static void Initialize(object obj)
			{
			}

			internal override TypeReference LookupType(string @namespace, string name)
			{
				TypeReference typeReference = this.LookupTypeDefinition(@namespace, name) ?? this.LookupTypeForwarded(@namespace, name);
				if (typeReference == null)
				{
					throw new NotSupportedException();
				}
				return typeReference;
			}

			private TypeReference LookupTypeDefinition(string @namespace, string name)
			{
				if (this.module.MetadataSystem.Types == null)
				{
					TypeSystem.CoreTypeSystem.Initialize(this.module.Types);
				}
				return this.module.Read<Row<string, string>, TypeDefinition>(new Row<string, string>(@namespace, name), (Row<string, string> row, MetadataReader reader) => {
					TypeDefinition[] types = reader.metadata.Types;
					for (int i = 0; i < (int)types.Length; i++)
					{
						if (types[i] == null)
						{
							types[i] = reader.GetTypeDefinition((uint)(i + 1));
						}
						TypeDefinition typeDefinition = types[i];
						if (typeDefinition.Name == row.Col2 && typeDefinition.Namespace == row.Col1)
						{
							return typeDefinition;
						}
					}
					return null;
				});
			}

			private TypeReference LookupTypeForwarded(string @namespace, string name)
			{
				if (!this.module.HasExportedTypes)
				{
					return null;
				}
				Collection<ExportedType> exportedTypes = this.module.ExportedTypes;
				for (int i = 0; i < exportedTypes.Count; i++)
				{
					ExportedType item = exportedTypes[i];
					if (item.Name == name && item.Namespace == @namespace)
					{
						return item.CreateReference();
					}
				}
				return null;
			}
		}
	}
}