using System;
using System.IO;
using System.Reflection;

namespace Mono.Cecil.Cil
{
	internal static class SymbolProvider
	{
		private readonly static string symbol_kind;

		private static ISymbolReaderProvider reader_provider;

		private static ISymbolWriterProvider writer_provider;

		static SymbolProvider()
		{
			SymbolProvider.symbol_kind = (Type.GetType("Mono.Runtime") != null ? "Mdb" : "Pdb");
		}

		public static ISymbolReaderProvider GetPlatformReaderProvider()
		{
			if (SymbolProvider.reader_provider != null)
			{
				return SymbolProvider.reader_provider;
			}
			Type platformType = SymbolProvider.GetPlatformType(SymbolProvider.GetProviderTypeName("ReaderProvider"));
			if (platformType == null)
			{
				return null;
			}
			ISymbolReaderProvider symbolReaderProvider = (ISymbolReaderProvider)Activator.CreateInstance(platformType);
			SymbolProvider.reader_provider = symbolReaderProvider;
			return symbolReaderProvider;
		}

		private static AssemblyName GetPlatformSymbolAssemblyName()
		{
			AssemblyName name = typeof(SymbolProvider).Assembly.GetName();
			AssemblyName assemblyName = new AssemblyName()
			{
				Name = string.Concat("Mono.Cecil.", SymbolProvider.symbol_kind),
				Version = name.Version
			};
			assemblyName.SetPublicKeyToken(name.GetPublicKeyToken());
			return assemblyName;
		}

		private static Type GetPlatformType(string fullname)
		{
			Type type = Type.GetType(fullname);
			if (type != null)
			{
				return type;
			}
			AssemblyName platformSymbolAssemblyName = SymbolProvider.GetPlatformSymbolAssemblyName();
			type = Type.GetType(string.Concat(fullname, ", ", platformSymbolAssemblyName.FullName));
			if (type != null)
			{
				return type;
			}
			try
			{
				Assembly assembly = Assembly.Load(platformSymbolAssemblyName);
				if (assembly != null)
				{
					return assembly.GetType(fullname);
				}
			}
			catch (FileNotFoundException fileNotFoundException)
			{
			}
			catch (FileLoadException fileLoadException)
			{
			}
			return null;
		}

		public static ISymbolWriterProvider GetPlatformWriterProvider()
		{
			if (SymbolProvider.writer_provider != null)
			{
				return SymbolProvider.writer_provider;
			}
			Type platformType = SymbolProvider.GetPlatformType(SymbolProvider.GetProviderTypeName("WriterProvider"));
			if (platformType == null)
			{
				return null;
			}
			ISymbolWriterProvider symbolWriterProvider = (ISymbolWriterProvider)Activator.CreateInstance(platformType);
			SymbolProvider.writer_provider = symbolWriterProvider;
			return symbolWriterProvider;
		}

		private static string GetProviderTypeName(string name)
		{
			return string.Concat(new string[] { "Mono.Cecil.", SymbolProvider.symbol_kind, ".", SymbolProvider.symbol_kind, name });
		}
	}
}