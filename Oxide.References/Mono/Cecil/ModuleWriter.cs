using Mono.Cecil.Cil;
using Mono.Cecil.PE;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mono.Cecil
{
	internal static class ModuleWriter
	{
		private static void BuildMetadata(ModuleDefinition module, MetadataBuilder metadata)
		{
			if (!module.HasImage)
			{
				metadata.BuildMetadata();
				return;
			}
			module.Read<MetadataBuilder, MetadataBuilder>(metadata, (MetadataBuilder builder, MetadataReader _) => {
				builder.BuildMetadata();
				return builder;
			});
		}

		private static ISymbolWriter GetSymbolWriter(ModuleDefinition module, string fq_name, ISymbolWriterProvider symbol_writer_provider)
		{
			if (symbol_writer_provider == null)
			{
				return null;
			}
			return symbol_writer_provider.GetSymbolWriter(module, fq_name);
		}

		public static void WriteModuleTo(ModuleDefinition module, Stream stream, WriterParameters parameters)
		{
			AssemblyNameDefinition name;
			if ((int)(module.Attributes & ModuleAttributes.ILOnly) == 0)
			{
				throw new NotSupportedException("Writing mixed-mode assemblies is not supported");
			}
			if (module.HasImage && module.ReadingMode == ReadingMode.Deferred)
			{
				ImmediateModuleReader.ReadModule(module);
			}
			module.MetadataSystem.Clear();
			if (module.assembly != null)
			{
				name = module.assembly.Name;
			}
			else
			{
				name = null;
			}
			AssemblyNameDefinition publicKey = name;
			string fullyQualifiedName = stream.GetFullyQualifiedName();
			ISymbolWriterProvider symbolWriterProvider = parameters.SymbolWriterProvider;
			if (symbolWriterProvider == null && parameters.WriteSymbols)
			{
				symbolWriterProvider = SymbolProvider.GetPlatformWriterProvider();
			}
			ISymbolWriter symbolWriter = ModuleWriter.GetSymbolWriter(module, fullyQualifiedName, symbolWriterProvider);
			if (parameters.StrongNameKeyPair != null && publicKey != null)
			{
				publicKey.PublicKey = parameters.StrongNameKeyPair.PublicKey;
				ModuleDefinition attributes = module;
				attributes.Attributes = attributes.Attributes | ModuleAttributes.StrongNameSigned;
			}
			MetadataBuilder metadataBuilder = new MetadataBuilder(module, fullyQualifiedName, symbolWriterProvider, symbolWriter);
			ModuleWriter.BuildMetadata(module, metadataBuilder);
			if (module.symbol_reader != null)
			{
				module.symbol_reader.Dispose();
			}
			ImageWriter imageWriter = ImageWriter.CreateWriter(module, metadataBuilder, stream);
			imageWriter.WriteImage();
			if (parameters.StrongNameKeyPair != null)
			{
				CryptoService.StrongName(stream, imageWriter, parameters.StrongNameKeyPair);
			}
			if (symbolWriter != null)
			{
				symbolWriter.Dispose();
			}
		}
	}
}