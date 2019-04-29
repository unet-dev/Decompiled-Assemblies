using Mono.Cecil.PE;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil
{
	internal sealed class ImmediateModuleReader : ModuleReader
	{
		public ImmediateModuleReader(Image image) : base(image, ReadingMode.Immediate)
		{
		}

		private static void Read(object collection)
		{
		}

		private static void ReadCustomAttributes(ICustomAttributeProvider provider)
		{
			if (!provider.HasCustomAttributes)
			{
				return;
			}
			Collection<CustomAttribute> customAttributes = provider.CustomAttributes;
			for (int i = 0; i < customAttributes.Count; i++)
			{
				ImmediateModuleReader.Read(customAttributes[i].ConstructorArguments);
			}
		}

		private static void ReadEvents(TypeDefinition type)
		{
			Collection<EventDefinition> events = type.Events;
			for (int i = 0; i < events.Count; i++)
			{
				EventDefinition item = events[i];
				ImmediateModuleReader.Read(item.AddMethod);
				ImmediateModuleReader.ReadCustomAttributes(item);
			}
		}

		private static void ReadFields(TypeDefinition type)
		{
			Collection<FieldDefinition> fields = type.Fields;
			for (int i = 0; i < fields.Count; i++)
			{
				FieldDefinition item = fields[i];
				if (item.HasConstant)
				{
					ImmediateModuleReader.Read(item.Constant);
				}
				if (item.HasLayoutInfo)
				{
					ImmediateModuleReader.Read(item.Offset);
				}
				if (item.RVA > 0)
				{
					ImmediateModuleReader.Read(item.InitialValue);
				}
				if (item.HasMarshalInfo)
				{
					ImmediateModuleReader.Read(item.MarshalInfo);
				}
				ImmediateModuleReader.ReadCustomAttributes(item);
			}
		}

		private static void ReadGenericParameters(IGenericParameterProvider provider)
		{
			if (!provider.HasGenericParameters)
			{
				return;
			}
			Collection<GenericParameter> genericParameters = provider.GenericParameters;
			for (int i = 0; i < genericParameters.Count; i++)
			{
				GenericParameter item = genericParameters[i];
				if (item.HasConstraints)
				{
					ImmediateModuleReader.Read(item.Constraints);
				}
				ImmediateModuleReader.ReadCustomAttributes(item);
			}
		}

		private static void ReadMethods(TypeDefinition type)
		{
			Collection<MethodDefinition> methods = type.Methods;
			for (int i = 0; i < methods.Count; i++)
			{
				MethodDefinition item = methods[i];
				ImmediateModuleReader.ReadGenericParameters(item);
				if (item.HasParameters)
				{
					ImmediateModuleReader.ReadParameters(item);
				}
				if (item.HasOverrides)
				{
					ImmediateModuleReader.Read(item.Overrides);
				}
				if (item.IsPInvokeImpl)
				{
					ImmediateModuleReader.Read(item.PInvokeInfo);
				}
				ImmediateModuleReader.ReadSecurityDeclarations(item);
				ImmediateModuleReader.ReadCustomAttributes(item);
				MethodReturnType methodReturnType = item.MethodReturnType;
				if (methodReturnType.HasConstant)
				{
					ImmediateModuleReader.Read(methodReturnType.Constant);
				}
				if (methodReturnType.HasMarshalInfo)
				{
					ImmediateModuleReader.Read(methodReturnType.MarshalInfo);
				}
				ImmediateModuleReader.ReadCustomAttributes(methodReturnType);
			}
		}

		protected override void ReadModule()
		{
			this.module.Read<ModuleDefinition, ModuleDefinition>(this.module, (ModuleDefinition module, MetadataReader reader) => {
				base.ReadModuleManifest(reader);
				ImmediateModuleReader.ReadModule(module);
				return module;
			});
		}

		public static void ReadModule(ModuleDefinition module)
		{
			if (module.HasAssemblyReferences)
			{
				ImmediateModuleReader.Read(module.AssemblyReferences);
			}
			if (module.HasResources)
			{
				ImmediateModuleReader.Read(module.Resources);
			}
			if (module.HasModuleReferences)
			{
				ImmediateModuleReader.Read(module.ModuleReferences);
			}
			if (module.HasTypes)
			{
				ImmediateModuleReader.ReadTypes(module.Types);
			}
			if (module.HasExportedTypes)
			{
				ImmediateModuleReader.Read(module.ExportedTypes);
			}
			if (module.HasCustomAttributes)
			{
				ImmediateModuleReader.Read(module.CustomAttributes);
			}
			AssemblyDefinition assembly = module.Assembly;
			if (assembly == null)
			{
				return;
			}
			if (assembly.HasCustomAttributes)
			{
				ImmediateModuleReader.ReadCustomAttributes(assembly);
			}
			if (assembly.HasSecurityDeclarations)
			{
				ImmediateModuleReader.Read(assembly.SecurityDeclarations);
			}
		}

		private static void ReadParameters(MethodDefinition method)
		{
			Collection<ParameterDefinition> parameters = method.Parameters;
			for (int i = 0; i < parameters.Count; i++)
			{
				ParameterDefinition item = parameters[i];
				if (item.HasConstant)
				{
					ImmediateModuleReader.Read(item.Constant);
				}
				if (item.HasMarshalInfo)
				{
					ImmediateModuleReader.Read(item.MarshalInfo);
				}
				ImmediateModuleReader.ReadCustomAttributes(item);
			}
		}

		private static void ReadProperties(TypeDefinition type)
		{
			Collection<PropertyDefinition> properties = type.Properties;
			for (int i = 0; i < properties.Count; i++)
			{
				PropertyDefinition item = properties[i];
				ImmediateModuleReader.Read(item.GetMethod);
				if (item.HasConstant)
				{
					ImmediateModuleReader.Read(item.Constant);
				}
				ImmediateModuleReader.ReadCustomAttributes(item);
			}
		}

		private static void ReadSecurityDeclarations(ISecurityDeclarationProvider provider)
		{
			if (!provider.HasSecurityDeclarations)
			{
				return;
			}
			Collection<SecurityDeclaration> securityDeclarations = provider.SecurityDeclarations;
			for (int i = 0; i < securityDeclarations.Count; i++)
			{
				ImmediateModuleReader.Read(securityDeclarations[i].SecurityAttributes);
			}
		}

		private static void ReadType(TypeDefinition type)
		{
			ImmediateModuleReader.ReadGenericParameters(type);
			if (type.HasInterfaces)
			{
				ImmediateModuleReader.Read(type.Interfaces);
			}
			if (type.HasNestedTypes)
			{
				ImmediateModuleReader.ReadTypes(type.NestedTypes);
			}
			if (type.HasLayoutInfo)
			{
				ImmediateModuleReader.Read(type.ClassSize);
			}
			if (type.HasFields)
			{
				ImmediateModuleReader.ReadFields(type);
			}
			if (type.HasMethods)
			{
				ImmediateModuleReader.ReadMethods(type);
			}
			if (type.HasProperties)
			{
				ImmediateModuleReader.ReadProperties(type);
			}
			if (type.HasEvents)
			{
				ImmediateModuleReader.ReadEvents(type);
			}
			ImmediateModuleReader.ReadSecurityDeclarations(type);
			ImmediateModuleReader.ReadCustomAttributes(type);
		}

		private static void ReadTypes(Collection<TypeDefinition> types)
		{
			for (int i = 0; i < types.Count; i++)
			{
				ImmediateModuleReader.ReadType(types[i]);
			}
		}
	}
}