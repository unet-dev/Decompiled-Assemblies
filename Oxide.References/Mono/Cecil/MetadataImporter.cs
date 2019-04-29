using Mono;
using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Mono.Cecil
{
	internal class MetadataImporter
	{
		private readonly ModuleDefinition module;

		private readonly static Dictionary<Type, ElementType> type_etype_mapping;

		static MetadataImporter()
		{
			MetadataImporter.type_etype_mapping = new Dictionary<Type, ElementType>(18)
			{
				{ typeof(void), ElementType.Void },
				{ typeof(bool), ElementType.Boolean },
				{ typeof(char), ElementType.Char },
				{ typeof(sbyte), ElementType.I1 },
				{ typeof(byte), ElementType.U1 },
				{ typeof(short), ElementType.I2 },
				{ typeof(ushort), ElementType.U2 },
				{ typeof(int), ElementType.I4 },
				{ typeof(uint), ElementType.U4 },
				{ typeof(long), ElementType.I8 },
				{ typeof(ulong), ElementType.U8 },
				{ typeof(float), ElementType.R4 },
				{ typeof(double), ElementType.R8 },
				{ typeof(string), ElementType.String },
				{ typeof(TypedReference), ElementType.TypedByRef },
				{ typeof(IntPtr), ElementType.I },
				{ typeof(UIntPtr), ElementType.U },
				{ typeof(object), ElementType.Object }
			};
		}

		public MetadataImporter(ModuleDefinition module)
		{
			this.module = module;
		}

		private static bool HasCallingConvention(MethodBase method, CallingConventions conventions)
		{
			return (int)(method.CallingConvention & conventions) != 0;
		}

		private AssemblyNameReference ImportAssemblyName(AssemblyNameReference name)
		{
			AssemblyNameReference assemblyNameReference;
			byte[] array;
			if (this.TryGetAssemblyNameReference(name, out assemblyNameReference))
			{
				return assemblyNameReference;
			}
			assemblyNameReference = new AssemblyNameReference(name.Name, name.Version)
			{
				Culture = name.Culture,
				HashAlgorithm = name.HashAlgorithm,
				IsRetargetable = name.IsRetargetable
			};
			if (!name.PublicKeyToken.IsNullOrEmpty<byte>())
			{
				array = new byte[(int)name.PublicKeyToken.Length];
			}
			else
			{
				array = Empty<byte>.Array;
			}
			byte[] numArray = array;
			if (numArray.Length != 0)
			{
				Buffer.BlockCopy(name.PublicKeyToken, 0, numArray, 0, (int)numArray.Length);
			}
			assemblyNameReference.PublicKeyToken = numArray;
			this.module.AssemblyReferences.Add(assemblyNameReference);
			return assemblyNameReference;
		}

		private static ElementType ImportElementType(Type type)
		{
			ElementType elementType;
			if (!MetadataImporter.type_etype_mapping.TryGetValue(type, out elementType))
			{
				return ElementType.None;
			}
			return elementType;
		}

		public FieldReference ImportField(FieldInfo field, ImportGenericContext context)
		{
			FieldReference fieldReference;
			TypeReference typeReference = this.ImportType(field.DeclaringType, context);
			if (MetadataImporter.IsGenericInstance(field.DeclaringType))
			{
				field = MetadataImporter.ResolveFieldDefinition(field);
			}
			context.Push(typeReference);
			try
			{
				fieldReference = new FieldReference()
				{
					Name = field.Name,
					DeclaringType = typeReference,
					FieldType = this.ImportType(field.FieldType, context)
				};
			}
			finally
			{
				context.Pop();
			}
			return fieldReference;
		}

		public FieldReference ImportField(FieldReference field, ImportGenericContext context)
		{
			FieldReference fieldReference;
			TypeReference typeReference = this.ImportType(field.DeclaringType, context);
			context.Push(typeReference);
			try
			{
				fieldReference = new FieldReference()
				{
					Name = field.Name,
					DeclaringType = typeReference,
					FieldType = this.ImportType(field.FieldType, context)
				};
			}
			finally
			{
				context.Pop();
			}
			return fieldReference;
		}

		private TypeReference ImportGenericInstance(Type type, ImportGenericContext context)
		{
			TypeReference typeReference;
			TypeReference typeReference1 = this.ImportType(type.GetGenericTypeDefinition(), context, ImportGenericKind.Definition);
			GenericInstanceType genericInstanceType = new GenericInstanceType(typeReference1);
			Type[] genericArguments = type.GetGenericArguments();
			Collection<TypeReference> typeReferences = genericInstanceType.GenericArguments;
			context.Push(typeReference1);
			try
			{
				for (int i = 0; i < (int)genericArguments.Length; i++)
				{
					typeReferences.Add(this.ImportType(genericArguments[i], context));
				}
				typeReference = genericInstanceType;
			}
			finally
			{
				context.Pop();
			}
			return typeReference;
		}

		private static TypeReference ImportGenericParameter(Type type, ImportGenericContext context)
		{
			if (context.IsEmpty)
			{
				throw new InvalidOperationException();
			}
			if (type.DeclaringMethod != null)
			{
				return context.MethodParameter(MetadataImporter.NormalizeMethodName(type.DeclaringMethod), type.GenericParameterPosition);
			}
			if (type.DeclaringType == null)
			{
				throw new InvalidOperationException();
			}
			return context.TypeParameter(MetadataImporter.NormalizeTypeFullName(type.DeclaringType), type.GenericParameterPosition);
		}

		private static void ImportGenericParameters(IGenericParameterProvider provider, Type[] arguments)
		{
			Collection<GenericParameter> genericParameters = provider.GenericParameters;
			for (int i = 0; i < (int)arguments.Length; i++)
			{
				genericParameters.Add(new GenericParameter(arguments[i].Name, provider));
			}
		}

		private static void ImportGenericParameters(IGenericParameterProvider imported, IGenericParameterProvider original)
		{
			Collection<GenericParameter> genericParameters = original.GenericParameters;
			Collection<GenericParameter> genericParameters1 = imported.GenericParameters;
			for (int i = 0; i < genericParameters.Count; i++)
			{
				genericParameters1.Add(new GenericParameter(genericParameters[i].Name, imported));
			}
		}

		public MethodReference ImportMethod(MethodBase method, ImportGenericContext context, ImportGenericKind import_kind)
		{
			MethodReference methodReference;
			if (MetadataImporter.IsMethodSpecification(method) || MetadataImporter.ImportOpenGenericMethod(method, import_kind))
			{
				return this.ImportMethodSpecification(method, context);
			}
			TypeReference typeReference = this.ImportType(method.DeclaringType, context);
			if (MetadataImporter.IsGenericInstance(method.DeclaringType))
			{
				method = method.Module.ResolveMethod(method.MetadataToken);
			}
			MethodReference methodReference1 = new MethodReference()
			{
				Name = method.Name,
				HasThis = MetadataImporter.HasCallingConvention(method, CallingConventions.HasThis),
				ExplicitThis = MetadataImporter.HasCallingConvention(method, CallingConventions.ExplicitThis),
				DeclaringType = this.ImportType(method.DeclaringType, context, ImportGenericKind.Definition)
			};
			if (MetadataImporter.HasCallingConvention(method, CallingConventions.VarArgs))
			{
				MethodReference callingConvention = methodReference1;
				callingConvention.CallingConvention = callingConvention.CallingConvention & MethodCallingConvention.VarArg;
			}
			if (method.IsGenericMethod)
			{
				MetadataImporter.ImportGenericParameters(methodReference1, method.GetGenericArguments());
			}
			context.Push(methodReference1);
			try
			{
				MethodInfo methodInfo = method as MethodInfo;
				methodReference1.ReturnType = (methodInfo != null ? this.ImportType(methodInfo.ReturnType, context) : this.ImportType(typeof(void), new ImportGenericContext()));
				ParameterInfo[] parameters = method.GetParameters();
				Collection<ParameterDefinition> parameterDefinitions = methodReference1.Parameters;
				for (int i = 0; i < (int)parameters.Length; i++)
				{
					parameterDefinitions.Add(new ParameterDefinition(this.ImportType(parameters[i].ParameterType, context)));
				}
				methodReference1.DeclaringType = typeReference;
				methodReference = methodReference1;
			}
			finally
			{
				context.Pop();
			}
			return methodReference;
		}

		public MethodReference ImportMethod(MethodReference method, ImportGenericContext context)
		{
			MethodReference methodReference;
			if (method.IsGenericInstance)
			{
				return this.ImportMethodSpecification(method, context);
			}
			TypeReference typeReference = this.ImportType(method.DeclaringType, context);
			MethodReference methodReference1 = new MethodReference()
			{
				Name = method.Name,
				HasThis = method.HasThis,
				ExplicitThis = method.ExplicitThis,
				DeclaringType = typeReference,
				CallingConvention = method.CallingConvention
			};
			if (method.HasGenericParameters)
			{
				MetadataImporter.ImportGenericParameters(methodReference1, method);
			}
			context.Push(methodReference1);
			try
			{
				methodReference1.ReturnType = this.ImportType(method.ReturnType, context);
				if (method.HasParameters)
				{
					Collection<ParameterDefinition> parameters = methodReference1.Parameters;
					Collection<ParameterDefinition> parameterDefinitions = method.Parameters;
					for (int i = 0; i < parameterDefinitions.Count; i++)
					{
						parameters.Add(new ParameterDefinition(this.ImportType(parameterDefinitions[i].ParameterType, context)));
					}
					methodReference = methodReference1;
				}
				else
				{
					methodReference = methodReference1;
				}
			}
			finally
			{
				context.Pop();
			}
			return methodReference;
		}

		private MethodReference ImportMethodSpecification(MethodBase method, ImportGenericContext context)
		{
			MethodReference methodReference;
			MethodInfo methodInfo = method as MethodInfo;
			if (methodInfo == null)
			{
				throw new InvalidOperationException();
			}
			MethodReference methodReference1 = this.ImportMethod(methodInfo.GetGenericMethodDefinition(), context, ImportGenericKind.Definition);
			GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(methodReference1);
			Type[] genericArguments = method.GetGenericArguments();
			Collection<TypeReference> typeReferences = genericInstanceMethod.GenericArguments;
			context.Push(methodReference1);
			try
			{
				for (int i = 0; i < (int)genericArguments.Length; i++)
				{
					typeReferences.Add(this.ImportType(genericArguments[i], context));
				}
				methodReference = genericInstanceMethod;
			}
			finally
			{
				context.Pop();
			}
			return methodReference;
		}

		private MethodSpecification ImportMethodSpecification(MethodReference method, ImportGenericContext context)
		{
			if (!method.IsGenericInstance)
			{
				throw new NotSupportedException();
			}
			GenericInstanceMethod genericInstanceMethod = (GenericInstanceMethod)method;
			GenericInstanceMethod genericInstanceMethod1 = new GenericInstanceMethod(this.ImportMethod(genericInstanceMethod.ElementMethod, context));
			Collection<TypeReference> genericArguments = genericInstanceMethod.GenericArguments;
			Collection<TypeReference> typeReferences = genericInstanceMethod1.GenericArguments;
			for (int i = 0; i < genericArguments.Count; i++)
			{
				typeReferences.Add(this.ImportType(genericArguments[i], context));
			}
			return genericInstanceMethod1;
		}

		private static bool ImportOpenGenericMethod(MethodBase method, ImportGenericKind import_kind)
		{
			if (!method.IsGenericMethod || !method.IsGenericMethodDefinition)
			{
				return false;
			}
			return import_kind == ImportGenericKind.Open;
		}

		private static bool ImportOpenGenericType(Type type, ImportGenericKind import_kind)
		{
			if (!type.IsGenericType || !type.IsGenericTypeDefinition)
			{
				return false;
			}
			return import_kind == ImportGenericKind.Open;
		}

		private AssemblyNameReference ImportScope(Assembly assembly)
		{
			AssemblyNameReference assemblyNameReference;
			AssemblyName name = assembly.GetName();
			if (this.TryGetAssemblyNameReference(name, out assemblyNameReference))
			{
				return assemblyNameReference;
			}
			assemblyNameReference = new AssemblyNameReference(name.Name, name.Version)
			{
				Culture = name.CultureInfo.Name,
				PublicKeyToken = name.GetPublicKeyToken(),
				HashAlgorithm = (AssemblyHashAlgorithm)name.HashAlgorithm
			};
			this.module.AssemblyReferences.Add(assemblyNameReference);
			return assemblyNameReference;
		}

		private IMetadataScope ImportScope(IMetadataScope scope)
		{
			switch (scope.MetadataScopeType)
			{
				case MetadataScopeType.AssemblyNameReference:
				{
					return this.ImportAssemblyName((AssemblyNameReference)scope);
				}
				case MetadataScopeType.ModuleReference:
				{
					throw new NotImplementedException();
				}
				case MetadataScopeType.ModuleDefinition:
				{
					if (scope == this.module)
					{
						return scope;
					}
					return this.ImportAssemblyName(((ModuleDefinition)scope).Assembly.Name);
				}
			}
			throw new NotSupportedException();
		}

		public TypeReference ImportType(Type type, ImportGenericContext context)
		{
			return this.ImportType(type, context, ImportGenericKind.Open);
		}

		public TypeReference ImportType(Type type, ImportGenericContext context, ImportGenericKind import_kind)
		{
			if (MetadataImporter.IsTypeSpecification(type) || MetadataImporter.ImportOpenGenericType(type, import_kind))
			{
				return this.ImportTypeSpecification(type, context);
			}
			TypeReference typeReference = new TypeReference(string.Empty, type.Name, this.module, this.ImportScope(type.Assembly), type.IsValueType)
			{
				etype = MetadataImporter.ImportElementType(type)
			};
			if (!MetadataImporter.IsNestedType(type))
			{
				typeReference.Namespace = type.Namespace ?? string.Empty;
			}
			else
			{
				typeReference.DeclaringType = this.ImportType(type.DeclaringType, context, import_kind);
			}
			if (type.IsGenericType)
			{
				MetadataImporter.ImportGenericParameters(typeReference, type.GetGenericArguments());
			}
			return typeReference;
		}

		public TypeReference ImportType(TypeReference type, ImportGenericContext context)
		{
			if (type.IsTypeSpecification())
			{
				return this.ImportTypeSpecification(type, context);
			}
			TypeReference typeReference = new TypeReference(type.Namespace, type.Name, this.module, this.ImportScope(type.Scope), type.IsValueType);
			MetadataSystem.TryProcessPrimitiveTypeReference(typeReference);
			if (type.IsNested)
			{
				typeReference.DeclaringType = this.ImportType(type.DeclaringType, context);
			}
			if (type.HasGenericParameters)
			{
				MetadataImporter.ImportGenericParameters(typeReference, type);
			}
			return typeReference;
		}

		private TypeReference ImportTypeSpecification(Type type, ImportGenericContext context)
		{
			if (type.IsByRef)
			{
				return new ByReferenceType(this.ImportType(type.GetElementType(), context));
			}
			if (type.IsPointer)
			{
				return new PointerType(this.ImportType(type.GetElementType(), context));
			}
			if (type.IsArray)
			{
				return new ArrayType(this.ImportType(type.GetElementType(), context), type.GetArrayRank());
			}
			if (type.IsGenericType)
			{
				return this.ImportGenericInstance(type, context);
			}
			if (!type.IsGenericParameter)
			{
				throw new NotSupportedException(type.FullName);
			}
			return MetadataImporter.ImportGenericParameter(type, context);
		}

		private TypeReference ImportTypeSpecification(TypeReference type, ImportGenericContext context)
		{
			ElementType elementType = type.etype;
			if (elementType > ElementType.CModOpt)
			{
				if (elementType == ElementType.Sentinel)
				{
					SentinelType sentinelType = (SentinelType)type;
					return new SentinelType(this.ImportType(sentinelType.ElementType, context));
				}
				if (elementType == ElementType.Pinned)
				{
					PinnedType pinnedType = (PinnedType)type;
					return new PinnedType(this.ImportType(pinnedType.ElementType, context));
				}
			}
			else
			{
				switch (elementType)
				{
					case ElementType.Ptr:
					{
						PointerType pointerType = (PointerType)type;
						return new PointerType(this.ImportType(pointerType.ElementType, context));
					}
					case ElementType.ByRef:
					{
						ByReferenceType byReferenceType = (ByReferenceType)type;
						return new ByReferenceType(this.ImportType(byReferenceType.ElementType, context));
					}
					case ElementType.ValueType:
					case ElementType.Class:
					{
						break;
					}
					case ElementType.Var:
					{
						GenericParameter genericParameter = (GenericParameter)type;
						if (genericParameter.DeclaringType == null)
						{
							throw new InvalidOperationException();
						}
						return context.TypeParameter(genericParameter.DeclaringType.FullName, genericParameter.Position);
					}
					case ElementType.Array:
					{
						ArrayType arrayType = (ArrayType)type;
						ArrayType arrayType1 = new ArrayType(this.ImportType(arrayType.ElementType, context));
						if (arrayType.IsVector)
						{
							return arrayType1;
						}
						Collection<ArrayDimension> dimensions = arrayType.Dimensions;
						Collection<ArrayDimension> arrayDimensions = arrayType1.Dimensions;
						arrayDimensions.Clear();
						for (int i = 0; i < dimensions.Count; i++)
						{
							ArrayDimension item = dimensions[i];
							arrayDimensions.Add(new ArrayDimension(item.LowerBound, item.UpperBound));
						}
						return arrayType1;
					}
					case ElementType.GenericInst:
					{
						GenericInstanceType genericInstanceType = (GenericInstanceType)type;
						GenericInstanceType genericInstanceType1 = new GenericInstanceType(this.ImportType(genericInstanceType.ElementType, context));
						Collection<TypeReference> genericArguments = genericInstanceType.GenericArguments;
						Collection<TypeReference> typeReferences = genericInstanceType1.GenericArguments;
						for (int j = 0; j < genericArguments.Count; j++)
						{
							typeReferences.Add(this.ImportType(genericArguments[j], context));
						}
						return genericInstanceType1;
					}
					default:
					{
						switch (elementType)
						{
							case ElementType.SzArray:
							{
								ArrayType arrayType2 = (ArrayType)type;
								return new ArrayType(this.ImportType(arrayType2.ElementType, context));
							}
							case ElementType.MVar:
							{
								GenericParameter genericParameter1 = (GenericParameter)type;
								if (genericParameter1.DeclaringMethod == null)
								{
									throw new InvalidOperationException();
								}
								return context.MethodParameter(context.NormalizeMethodName(genericParameter1.DeclaringMethod), genericParameter1.Position);
							}
							case ElementType.CModReqD:
							{
								RequiredModifierType requiredModifierType = (RequiredModifierType)type;
								return new RequiredModifierType(this.ImportType(requiredModifierType.ModifierType, context), this.ImportType(requiredModifierType.ElementType, context));
							}
							case ElementType.CModOpt:
							{
								OptionalModifierType optionalModifierType = (OptionalModifierType)type;
								return new OptionalModifierType(this.ImportType(optionalModifierType.ModifierType, context), this.ImportType(optionalModifierType.ElementType, context));
							}
						}
						break;
					}
				}
			}
			throw new NotSupportedException(type.etype.ToString());
		}

		private static bool IsGenericInstance(Type type)
		{
			if (!type.IsGenericType)
			{
				return false;
			}
			return !type.IsGenericTypeDefinition;
		}

		private static bool IsMethodSpecification(MethodBase method)
		{
			if (!method.IsGenericMethod)
			{
				return false;
			}
			return !method.IsGenericMethodDefinition;
		}

		private static bool IsNestedType(Type type)
		{
			return type.IsNested;
		}

		private static bool IsTypeSpecification(Type type)
		{
			if (type.HasElementType || MetadataImporter.IsGenericInstance(type))
			{
				return true;
			}
			return type.IsGenericParameter;
		}

		private static string NormalizeMethodName(MethodBase method)
		{
			return string.Concat(MetadataImporter.NormalizeTypeFullName(method.DeclaringType), ".", method.Name);
		}

		private static string NormalizeTypeFullName(Type type)
		{
			if (!MetadataImporter.IsNestedType(type))
			{
				return type.FullName;
			}
			return string.Concat(MetadataImporter.NormalizeTypeFullName(type.DeclaringType), "/", type.Name);
		}

		private static FieldInfo ResolveFieldDefinition(FieldInfo field)
		{
			return field.Module.ResolveField(field.MetadataToken);
		}

		private bool TryGetAssemblyNameReference(AssemblyName name, out AssemblyNameReference assembly_reference)
		{
			Collection<AssemblyNameReference> assemblyReferences = this.module.AssemblyReferences;
			for (int i = 0; i < assemblyReferences.Count; i++)
			{
				AssemblyNameReference item = assemblyReferences[i];
				if (name.FullName == item.FullName)
				{
					assembly_reference = item;
					return true;
				}
			}
			assembly_reference = null;
			return false;
		}

		private bool TryGetAssemblyNameReference(AssemblyNameReference name_reference, out AssemblyNameReference assembly_reference)
		{
			Collection<AssemblyNameReference> assemblyReferences = this.module.AssemblyReferences;
			for (int i = 0; i < assemblyReferences.Count; i++)
			{
				AssemblyNameReference item = assemblyReferences[i];
				if (name_reference.FullName == item.FullName)
				{
					assembly_reference = item;
					return true;
				}
			}
			assembly_reference = null;
			return false;
		}
	}
}