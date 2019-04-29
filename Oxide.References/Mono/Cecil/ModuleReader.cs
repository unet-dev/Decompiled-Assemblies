using Mono.Cecil.Cil;
using Mono.Cecil.PE;
using System;

namespace Mono.Cecil
{
	internal abstract class ModuleReader
	{
		protected readonly Image image;

		protected readonly ModuleDefinition module;

		protected ModuleReader(Image image, ReadingMode mode)
		{
			this.image = image;
			this.module = new ModuleDefinition(image)
			{
				ReadingMode = mode
			};
		}

		public static ModuleDefinition CreateModuleFrom(Image image, ReaderParameters parameters)
		{
			ModuleReader moduleReader = ModuleReader.CreateModuleReader(image, parameters.ReadingMode);
			ModuleDefinition assemblyResolver = moduleReader.module;
			if (parameters.AssemblyResolver != null)
			{
				assemblyResolver.assembly_resolver = parameters.AssemblyResolver;
			}
			if (parameters.MetadataResolver != null)
			{
				assemblyResolver.metadata_resolver = parameters.MetadataResolver;
			}
			moduleReader.ReadModule();
			ModuleReader.ReadSymbols(assemblyResolver, parameters);
			return assemblyResolver;
		}

		private static ModuleReader CreateModuleReader(Image image, ReadingMode mode)
		{
			if (mode == ReadingMode.Immediate)
			{
				return new ImmediateModuleReader(image);
			}
			if (mode != ReadingMode.Deferred)
			{
				throw new ArgumentException();
			}
			return new DeferredModuleReader(image);
		}

		private void ReadAssembly(MetadataReader reader)
		{
			AssemblyNameDefinition assemblyNameDefinition = reader.ReadAssemblyNameDefinition();
			if (assemblyNameDefinition == null)
			{
				this.module.kind = ModuleKind.NetModule;
				return;
			}
			AssemblyDefinition assemblyDefinition = new AssemblyDefinition()
			{
				Name = assemblyNameDefinition
			};
			this.module.assembly = assemblyDefinition;
			assemblyDefinition.main_module = this.module;
		}

		protected abstract void ReadModule();

		protected void ReadModuleManifest(MetadataReader reader)
		{
			reader.Populate(this.module);
			this.ReadAssembly(reader);
		}

		private static void ReadSymbols(ModuleDefinition module, ReaderParameters parameters)
		{
			ISymbolReaderProvider symbolReaderProvider = parameters.SymbolReaderProvider;
			if (symbolReaderProvider == null && parameters.ReadSymbols)
			{
				symbolReaderProvider = SymbolProvider.GetPlatformReaderProvider();
			}
			if (symbolReaderProvider != null)
			{
				module.SymbolReaderProvider = symbolReaderProvider;
				module.ReadSymbols((parameters.SymbolStream != null ? symbolReaderProvider.GetSymbolReader(module, parameters.SymbolStream) : symbolReaderProvider.GetSymbolReader(module, module.FullyQualifiedName)));
			}
		}
	}
}