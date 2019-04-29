using Mono.Collections.Generic;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Mono.Cecil
{
	public sealed class AssemblyDefinition : ICustomAttributeProvider, IMetadataTokenProvider, ISecurityDeclarationProvider
	{
		private AssemblyNameDefinition name;

		internal ModuleDefinition main_module;

		private Collection<ModuleDefinition> modules;

		private Collection<CustomAttribute> custom_attributes;

		private Collection<SecurityDeclaration> security_declarations;

		public Collection<CustomAttribute> CustomAttributes
		{
			get
			{
				return this.custom_attributes ?? this.GetCustomAttributes(ref this.custom_attributes, this.main_module);
			}
		}

		public MethodDefinition EntryPoint
		{
			get
			{
				return this.main_module.EntryPoint;
			}
			set
			{
				this.main_module.EntryPoint = value;
			}
		}

		public string FullName
		{
			get
			{
				if (this.name == null)
				{
					return string.Empty;
				}
				return this.name.FullName;
			}
		}

		public bool HasCustomAttributes
		{
			get
			{
				if (this.custom_attributes == null)
				{
					return this.GetHasCustomAttributes(this.main_module);
				}
				return this.custom_attributes.Count > 0;
			}
		}

		public bool HasSecurityDeclarations
		{
			get
			{
				if (this.security_declarations == null)
				{
					return this.GetHasSecurityDeclarations(this.main_module);
				}
				return this.security_declarations.Count > 0;
			}
		}

		public ModuleDefinition MainModule
		{
			get
			{
				return this.main_module;
			}
		}

		public Mono.Cecil.MetadataToken MetadataToken
		{
			get
			{
				return new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.Assembly, 1);
			}
			set
			{
			}
		}

		public Collection<ModuleDefinition> Modules
		{
			get
			{
				if (this.modules != null)
				{
					return this.modules;
				}
				if (!this.main_module.HasImage)
				{
					Collection<ModuleDefinition> moduleDefinitions = new Collection<ModuleDefinition>(1)
					{
						this.main_module
					};
					Collection<ModuleDefinition> moduleDefinitions1 = moduleDefinitions;
					this.modules = moduleDefinitions;
					return moduleDefinitions1;
				}
				return this.main_module.Read<AssemblyDefinition, Collection<ModuleDefinition>>(ref this.modules, this, (AssemblyDefinition _, MetadataReader reader) => reader.ReadModules());
			}
		}

		public AssemblyNameDefinition Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public Collection<SecurityDeclaration> SecurityDeclarations
		{
			get
			{
				return this.security_declarations ?? this.GetSecurityDeclarations(ref this.security_declarations, this.main_module);
			}
		}

		internal AssemblyDefinition()
		{
		}

		public static AssemblyDefinition CreateAssembly(AssemblyNameDefinition assemblyName, string moduleName, ModuleKind kind)
		{
			return AssemblyDefinition.CreateAssembly(assemblyName, moduleName, new ModuleParameters()
			{
				Kind = kind
			});
		}

		public static AssemblyDefinition CreateAssembly(AssemblyNameDefinition assemblyName, string moduleName, ModuleParameters parameters)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			if (moduleName == null)
			{
				throw new ArgumentNullException("moduleName");
			}
			Mixin.CheckParameters(parameters);
			if (parameters.Kind == ModuleKind.NetModule)
			{
				throw new ArgumentException("kind");
			}
			AssemblyDefinition assembly = ModuleDefinition.CreateModule(moduleName, parameters).Assembly;
			assembly.Name = assemblyName;
			return assembly;
		}

		public static AssemblyDefinition ReadAssembly(string fileName)
		{
			return AssemblyDefinition.ReadAssembly(ModuleDefinition.ReadModule(fileName));
		}

		public static AssemblyDefinition ReadAssembly(string fileName, ReaderParameters parameters)
		{
			return AssemblyDefinition.ReadAssembly(ModuleDefinition.ReadModule(fileName, parameters));
		}

		public static AssemblyDefinition ReadAssembly(Stream stream)
		{
			return AssemblyDefinition.ReadAssembly(ModuleDefinition.ReadModule(stream));
		}

		public static AssemblyDefinition ReadAssembly(Stream stream, ReaderParameters parameters)
		{
			return AssemblyDefinition.ReadAssembly(ModuleDefinition.ReadModule(stream, parameters));
		}

		private static AssemblyDefinition ReadAssembly(ModuleDefinition module)
		{
			AssemblyDefinition assembly = module.Assembly;
			if (assembly == null)
			{
				throw new ArgumentException();
			}
			return assembly;
		}

		public override string ToString()
		{
			return this.FullName;
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
			this.main_module.Write(fileName, parameters);
		}

		public void Write(Stream stream, WriterParameters parameters)
		{
			this.main_module.Write(stream, parameters);
		}
	}
}