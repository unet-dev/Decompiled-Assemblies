using Mono.Collections.Generic;
using System;

namespace Mono.Cecil
{
	public class MetadataResolver : IMetadataResolver
	{
		private readonly IAssemblyResolver assembly_resolver;

		public IAssemblyResolver AssemblyResolver
		{
			get
			{
				return this.assembly_resolver;
			}
		}

		public MetadataResolver(IAssemblyResolver assemblyResolver)
		{
			if (assemblyResolver == null)
			{
				throw new ArgumentNullException("assemblyResolver");
			}
			this.assembly_resolver = assemblyResolver;
		}

		private static bool AreSame(Collection<ParameterDefinition> a, Collection<ParameterDefinition> b)
		{
			int count = a.Count;
			if (count != b.Count)
			{
				return false;
			}
			if (count == 0)
			{
				return true;
			}
			for (int i = 0; i < count; i++)
			{
				if (!MetadataResolver.AreSame(a[i].ParameterType, b[i].ParameterType))
				{
					return false;
				}
			}
			return true;
		}

		private static bool AreSame(TypeSpecification a, TypeSpecification b)
		{
			if (!MetadataResolver.AreSame(a.ElementType, b.ElementType))
			{
				return false;
			}
			if (a.IsGenericInstance)
			{
				return MetadataResolver.AreSame((GenericInstanceType)a, (GenericInstanceType)b);
			}
			if (a.IsRequiredModifier || a.IsOptionalModifier)
			{
				return MetadataResolver.AreSame((IModifierType)a, (IModifierType)b);
			}
			if (!a.IsArray)
			{
				return true;
			}
			return MetadataResolver.AreSame((ArrayType)a, (ArrayType)b);
		}

		private static bool AreSame(ArrayType a, ArrayType b)
		{
			if (a.Rank != b.Rank)
			{
				return false;
			}
			return true;
		}

		private static bool AreSame(IModifierType a, IModifierType b)
		{
			return MetadataResolver.AreSame(a.ModifierType, b.ModifierType);
		}

		private static bool AreSame(GenericInstanceType a, GenericInstanceType b)
		{
			if (a.GenericArguments.Count != b.GenericArguments.Count)
			{
				return false;
			}
			for (int i = 0; i < a.GenericArguments.Count; i++)
			{
				if (!MetadataResolver.AreSame(a.GenericArguments[i], b.GenericArguments[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static bool AreSame(GenericParameter a, GenericParameter b)
		{
			return a.Position == b.Position;
		}

		private static bool AreSame(TypeReference a, TypeReference b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			if (a.etype != b.etype)
			{
				return false;
			}
			if (a.IsGenericParameter)
			{
				return MetadataResolver.AreSame((GenericParameter)a, (GenericParameter)b);
			}
			if (a.IsTypeSpecification())
			{
				return MetadataResolver.AreSame((TypeSpecification)a, (TypeSpecification)b);
			}
			if (a.Name != b.Name || a.Namespace != b.Namespace)
			{
				return false;
			}
			return MetadataResolver.AreSame(a.DeclaringType, b.DeclaringType);
		}

		private FieldDefinition GetField(TypeDefinition type, FieldReference reference)
		{
			while (type != null)
			{
				FieldDefinition field = MetadataResolver.GetField(type.Fields, reference);
				if (field != null)
				{
					return field;
				}
				if (type.BaseType == null)
				{
					return null;
				}
				type = this.Resolve(type.BaseType);
			}
			return null;
		}

		private static FieldDefinition GetField(Collection<FieldDefinition> fields, FieldReference reference)
		{
			for (int i = 0; i < fields.Count; i++)
			{
				FieldDefinition item = fields[i];
				if (!(item.Name != reference.Name) && MetadataResolver.AreSame(item.FieldType, reference.FieldType))
				{
					return item;
				}
			}
			return null;
		}

		private MethodDefinition GetMethod(TypeDefinition type, MethodReference reference)
		{
			while (type != null)
			{
				MethodDefinition method = MetadataResolver.GetMethod(type.Methods, reference);
				if (method != null)
				{
					return method;
				}
				if (type.BaseType == null)
				{
					return null;
				}
				type = this.Resolve(type.BaseType);
			}
			return null;
		}

		public static MethodDefinition GetMethod(Collection<MethodDefinition> methods, MethodReference reference)
		{
			for (int i = 0; i < methods.Count; i++)
			{
				MethodDefinition item = methods[i];
				if (!(item.Name != reference.Name) && item.HasGenericParameters == reference.HasGenericParameters && (!item.HasGenericParameters || item.GenericParameters.Count == reference.GenericParameters.Count) && MetadataResolver.AreSame(item.ReturnType, reference.ReturnType) && item.HasParameters == reference.HasParameters)
				{
					if (!item.HasParameters && !reference.HasParameters)
					{
						return item;
					}
					if (MetadataResolver.AreSame(item.Parameters, reference.Parameters))
					{
						return item;
					}
				}
			}
			return null;
		}

		private static TypeDefinition GetType(ModuleDefinition module, TypeReference reference)
		{
			TypeDefinition typeDefinition = MetadataResolver.GetTypeDefinition(module, reference);
			if (typeDefinition != null)
			{
				return typeDefinition;
			}
			if (!module.HasExportedTypes)
			{
				return null;
			}
			Collection<ExportedType> exportedTypes = module.ExportedTypes;
			for (int i = 0; i < exportedTypes.Count; i++)
			{
				ExportedType item = exportedTypes[i];
				if (!(item.Name != reference.Name) && !(item.Namespace != reference.Namespace))
				{
					return item.Resolve();
				}
			}
			return null;
		}

		private static TypeDefinition GetTypeDefinition(ModuleDefinition module, TypeReference type)
		{
			if (!type.IsNested)
			{
				return module.GetType(type.Namespace, type.Name);
			}
			TypeDefinition typeDefinition = type.DeclaringType.Resolve();
			if (typeDefinition == null)
			{
				return null;
			}
			return typeDefinition.GetNestedType(type.TypeFullName());
		}

		public virtual TypeDefinition Resolve(TypeReference type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			type = type.GetElementType();
			IMetadataScope scope = type.Scope;
			if (scope == null)
			{
				return null;
			}
			switch (scope.MetadataScopeType)
			{
				case MetadataScopeType.AssemblyNameReference:
				{
					AssemblyDefinition assemblyDefinition = this.assembly_resolver.Resolve((AssemblyNameReference)scope);
					if (assemblyDefinition == null)
					{
						return null;
					}
					return MetadataResolver.GetType(assemblyDefinition.MainModule, type);
				}
				case MetadataScopeType.ModuleReference:
				{
					Collection<ModuleDefinition> modules = type.Module.Assembly.Modules;
					ModuleReference moduleReference = (ModuleReference)scope;
					for (int i = 0; i < modules.Count; i++)
					{
						ModuleDefinition item = modules[i];
						if (item.Name == moduleReference.Name)
						{
							return MetadataResolver.GetType(item, type);
						}
					}
					break;
				}
				case MetadataScopeType.ModuleDefinition:
				{
					return MetadataResolver.GetType((ModuleDefinition)scope, type);
				}
			}
			throw new NotSupportedException();
		}

		public virtual FieldDefinition Resolve(FieldReference field)
		{
			if (field == null)
			{
				throw new ArgumentNullException("field");
			}
			TypeDefinition typeDefinition = this.Resolve(field.DeclaringType);
			if (typeDefinition == null)
			{
				return null;
			}
			if (!typeDefinition.HasFields)
			{
				return null;
			}
			return this.GetField(typeDefinition, field);
		}

		public virtual MethodDefinition Resolve(MethodReference method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			TypeDefinition typeDefinition = this.Resolve(method.DeclaringType);
			if (typeDefinition == null)
			{
				return null;
			}
			method = method.GetElementMethod();
			if (!typeDefinition.HasMethods)
			{
				return null;
			}
			return this.GetMethod(typeDefinition, method);
		}
	}
}