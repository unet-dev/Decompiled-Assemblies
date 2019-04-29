using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System;
using System.Text;

namespace Mono.Cecil
{
	internal class TypeParser
	{
		private readonly string fullname;

		private readonly int length;

		private int position;

		private TypeParser(string fullname)
		{
			this.fullname = fullname;
			this.length = fullname.Length;
		}

		private static void Add<T>(ref T[] array, T item)
		{
			if (array == null)
			{
				array = new T[] { item };
				return;
			}
			array = array.Resize<T>((int)array.Length + 1);
			array[(int)array.Length - 1] = item;
		}

		private static void AdjustGenericParameters(TypeReference type)
		{
			int num;
			if (!TypeParser.TryGetArity(type.Name, out num))
			{
				return;
			}
			for (int i = 0; i < num; i++)
			{
				type.GenericParameters.Add(new GenericParameter(type));
			}
		}

		private static void AppendNamePart(string part, StringBuilder name)
		{
			string str = part;
			for (int i = 0; i < str.Length; i++)
			{
				char chr = str[i];
				if (TypeParser.IsDelimiter(chr))
				{
					name.Append('\\');
				}
				name.Append(chr);
			}
		}

		private static void AppendType(TypeReference type, StringBuilder name, bool fq_name, bool top_level)
		{
			TypeReference declaringType = type.DeclaringType;
			if (declaringType != null)
			{
				TypeParser.AppendType(declaringType, name, false, top_level);
				name.Append('+');
			}
			string @namespace = type.Namespace;
			if (!string.IsNullOrEmpty(@namespace))
			{
				TypeParser.AppendNamePart(@namespace, name);
				name.Append('.');
			}
			TypeParser.AppendNamePart(type.GetElementType().Name, name);
			if (!fq_name)
			{
				return;
			}
			if (type.IsTypeSpecification())
			{
				TypeParser.AppendTypeSpecification((TypeSpecification)type, name);
			}
			if (TypeParser.RequiresFullyQualifiedName(type, top_level))
			{
				name.Append(", ");
				name.Append(TypeParser.GetScopeFullName(type));
			}
		}

		private static void AppendTypeSpecification(TypeSpecification type, StringBuilder name)
		{
			ArrayType arrayType;
			int i;
			StringBuilder stringBuilder;
			StringBuilder stringBuilder1;
			StringBuilder stringBuilder2;
			StringBuilder stringBuilder3;
			if (type.ElementType.IsTypeSpecification())
			{
				TypeParser.AppendTypeSpecification((TypeSpecification)type.ElementType, name);
			}
			ElementType elementType = type.etype;
			switch (elementType)
			{
				case ElementType.Ptr:
				{
					name.Append('*');
					return;
				}
				case ElementType.ByRef:
				{
					name.Append('&');
					return;
				}
				case ElementType.ValueType:
				case ElementType.Class:
				case ElementType.Var:
				{
					return;
				}
				case ElementType.Array:
				{
					arrayType = (ArrayType)type;
					if (arrayType.IsVector)
					{
						stringBuilder = name.Append("[]");
						return;
					}
					stringBuilder1 = name.Append('[');
					for (i = 1; i < arrayType.Rank; i++)
					{
						stringBuilder2 = name.Append(',');
					}
					stringBuilder3 = name.Append(']');
					return;
				}
				case ElementType.GenericInst:
				{
					Collection<TypeReference> genericArguments = ((GenericInstanceType)type).GenericArguments;
					name.Append('[');
					for (int j = 0; j < genericArguments.Count; j++)
					{
						if (j > 0)
						{
							name.Append(',');
						}
						TypeReference item = genericArguments[j];
						bool scope = item.Scope != item.Module;
						if (scope)
						{
							name.Append('[');
						}
						TypeParser.AppendType(item, name, true, false);
						if (scope)
						{
							name.Append(']');
						}
					}
					name.Append(']');
					return;
				}
				default:
				{
					if (elementType == ElementType.SzArray)
					{
						arrayType = (ArrayType)type;
						if (arrayType.IsVector)
						{
							stringBuilder = name.Append("[]");
							return;
						}
						stringBuilder1 = name.Append('[');
						for (i = 1; i < arrayType.Rank; i++)
						{
							stringBuilder2 = name.Append(',');
						}
						stringBuilder3 = name.Append(']');
						return;
					}
					return;
				}
			}
		}

		private static TypeReference CreateReference(TypeParser.Type type_info, ModuleDefinition module, IMetadataScope scope)
		{
			string str;
			string str1;
			TypeParser.SplitFullName(type_info.type_fullname, out str, out str1);
			TypeReference typeReference = new TypeReference(str, str1, module, scope);
			MetadataSystem.TryProcessPrimitiveTypeReference(typeReference);
			TypeParser.AdjustGenericParameters(typeReference);
			string[] nestedNames = type_info.nested_names;
			if (nestedNames.IsNullOrEmpty<string>())
			{
				return typeReference;
			}
			for (int i = 0; i < (int)nestedNames.Length; i++)
			{
				typeReference = new TypeReference(string.Empty, nestedNames[i], module, null)
				{
					DeclaringType = typeReference
				};
				TypeParser.AdjustGenericParameters(typeReference);
			}
			return typeReference;
		}

		private static TypeReference CreateSpecs(TypeReference type, TypeParser.Type type_info)
		{
			type = TypeParser.TryCreateGenericInstanceType(type, type_info);
			int[] typeInfo = type_info.specs;
			if (typeInfo.IsNullOrEmpty<int>())
			{
				return type;
			}
			for (int i = 0; i < (int)typeInfo.Length; i++)
			{
				switch (typeInfo[i])
				{
					case -3:
					{
						type = new ArrayType(type);
						break;
					}
					case -2:
					{
						type = new ByReferenceType(type);
						break;
					}
					case -1:
					{
						type = new PointerType(type);
						break;
					}
					default:
					{
						ArrayType arrayType = new ArrayType(type);
						arrayType.Dimensions.Clear();
						for (int j = 0; j < typeInfo[i]; j++)
						{
							arrayType.Dimensions.Add(new ArrayDimension());
						}
						type = arrayType;
						break;
					}
				}
			}
			return type;
		}

		private static IMetadataScope GetMetadataScope(ModuleDefinition module, TypeParser.Type type_info)
		{
			if (string.IsNullOrEmpty(type_info.assembly))
			{
				return module.TypeSystem.Corlib;
			}
			return TypeParser.MatchReference(module, AssemblyNameReference.Parse(type_info.assembly));
		}

		private static string GetScopeFullName(TypeReference type)
		{
			IMetadataScope scope = type.Scope;
			MetadataScopeType metadataScopeType = scope.MetadataScopeType;
			if (metadataScopeType == MetadataScopeType.AssemblyNameReference)
			{
				return ((AssemblyNameReference)scope).FullName;
			}
			if (metadataScopeType != MetadataScopeType.ModuleDefinition)
			{
				throw new ArgumentException();
			}
			return ((ModuleDefinition)scope).Assembly.Name.FullName;
		}

		private static TypeReference GetTypeReference(ModuleDefinition module, TypeParser.Type type_info)
		{
			TypeReference typeReference;
			if (!TypeParser.TryGetDefinition(module, type_info, out typeReference))
			{
				typeReference = TypeParser.CreateReference(type_info, module, TypeParser.GetMetadataScope(module, type_info));
			}
			return TypeParser.CreateSpecs(typeReference, type_info);
		}

		private static bool IsDelimiter(char chr)
		{
			return "+,[]*&".IndexOf(chr) != -1;
		}

		private static AssemblyNameReference MatchReference(ModuleDefinition module, AssemblyNameReference pattern)
		{
			Collection<AssemblyNameReference> assemblyReferences = module.AssemblyReferences;
			for (int i = 0; i < assemblyReferences.Count; i++)
			{
				AssemblyNameReference item = assemblyReferences[i];
				if (item.FullName == pattern.FullName)
				{
					return item;
				}
			}
			return pattern;
		}

		private string ParseAssemblyName()
		{
			if (!this.TryParse(','))
			{
				return string.Empty;
			}
			this.TryParseWhiteSpace();
			int num = this.position;
			while (this.position < this.length)
			{
				char chr = this.fullname[this.position];
				if (chr == '[' || chr == ']')
				{
					break;
				}
				this.position++;
			}
			return this.fullname.Substring(num, this.position - num);
		}

		private TypeParser.Type[] ParseGenericArguments(int arity)
		{
			TypeParser.Type[] typeArray = null;
			if (this.position == this.length || this.fullname[this.position] != '[')
			{
				return typeArray;
			}
			this.TryParse('[');
			for (int i = 0; i < arity; i++)
			{
				bool flag = this.TryParse('[');
				TypeParser.Add<TypeParser.Type>(ref typeArray, this.ParseType(flag));
				if (flag)
				{
					this.TryParse(']');
				}
				this.TryParse(',');
				this.TryParseWhiteSpace();
			}
			this.TryParse(']');
			return typeArray;
		}

		private static bool ParseInt32(string value, out int result)
		{
			return int.TryParse(value, out result);
		}

		private string[] ParseNestedNames()
		{
			string[] strArrays = null;
			while (this.TryParse('+'))
			{
				TypeParser.Add<string>(ref strArrays, this.ParsePart());
			}
			return strArrays;
		}

		private string ParsePart()
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (this.position < this.length && !TypeParser.IsDelimiter(this.fullname[this.position]))
			{
				if (this.fullname[this.position] == '\\')
				{
					this.position++;
				}
				string str = this.fullname;
				int num = this.position;
				this.position = num + 1;
				stringBuilder.Append(str[num]);
			}
			return stringBuilder.ToString();
		}

		private int[] ParseSpecs()
		{
			int[] numArray = null;
			while (this.position < this.length)
			{
				char chr = this.fullname[this.position];
				if (chr == '&')
				{
					this.position++;
					TypeParser.Add<int>(ref numArray, -2);
				}
				else if (chr == '*')
				{
					this.position++;
					TypeParser.Add<int>(ref numArray, -1);
				}
				else
				{
					if (chr != '[')
					{
						return numArray;
					}
					this.position++;
					chr = this.fullname[this.position];
					if (chr == '*')
					{
						this.position++;
						TypeParser.Add<int>(ref numArray, 1);
					}
					else if (chr != ']')
					{
						int num = 1;
						while (this.TryParse(','))
						{
							num++;
						}
						TypeParser.Add<int>(ref numArray, num);
						this.TryParse(']');
					}
					else
					{
						this.position++;
						TypeParser.Add<int>(ref numArray, -3);
					}
				}
			}
			return numArray;
		}

		private TypeParser.Type ParseType(bool fq_name)
		{
			TypeParser.Type type = new TypeParser.Type()
			{
				type_fullname = this.ParsePart(),
				nested_names = this.ParseNestedNames()
			};
			if (TypeParser.TryGetArity(type))
			{
				type.generic_arguments = this.ParseGenericArguments(type.arity);
			}
			type.specs = this.ParseSpecs();
			if (fq_name)
			{
				type.assembly = this.ParseAssemblyName();
			}
			return type;
		}

		public static TypeReference ParseType(ModuleDefinition module, string fullname)
		{
			if (string.IsNullOrEmpty(fullname))
			{
				return null;
			}
			TypeParser typeParser = new TypeParser(fullname);
			return TypeParser.GetTypeReference(module, typeParser.ParseType(true));
		}

		private static bool RequiresFullyQualifiedName(TypeReference type, bool top_level)
		{
			if (type.Scope == type.Module)
			{
				return false;
			}
			if ((type.Scope.Name == "mscorlib") & top_level)
			{
				return false;
			}
			return true;
		}

		public static void SplitFullName(string fullname, out string @namespace, out string name)
		{
			int num = fullname.LastIndexOf('.');
			if (num == -1)
			{
				@namespace = string.Empty;
				name = fullname;
				return;
			}
			@namespace = fullname.Substring(0, num);
			name = fullname.Substring(num + 1);
		}

		public static string ToParseable(TypeReference type)
		{
			if (type == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			TypeParser.AppendType(type, stringBuilder, true, true);
			return stringBuilder.ToString();
		}

		private static void TryAddArity(string name, ref int arity)
		{
			int num;
			if (!TypeParser.TryGetArity(name, out num))
			{
				return;
			}
			arity += num;
		}

		private static TypeReference TryCreateGenericInstanceType(TypeReference type, TypeParser.Type type_info)
		{
			TypeParser.Type[] genericArguments = type_info.generic_arguments;
			if (genericArguments.IsNullOrEmpty<TypeParser.Type>())
			{
				return type;
			}
			GenericInstanceType genericInstanceType = new GenericInstanceType(type);
			Collection<TypeReference> typeReferences = genericInstanceType.GenericArguments;
			for (int i = 0; i < (int)genericArguments.Length; i++)
			{
				typeReferences.Add(TypeParser.GetTypeReference(type.Module, genericArguments[i]));
			}
			return genericInstanceType;
		}

		private static bool TryCurrentModule(ModuleDefinition module, TypeParser.Type type_info)
		{
			if (string.IsNullOrEmpty(type_info.assembly))
			{
				return true;
			}
			if (module.assembly != null && module.assembly.Name.FullName == type_info.assembly)
			{
				return true;
			}
			return false;
		}

		private static bool TryGetArity(TypeParser.Type type)
		{
			int num = 0;
			TypeParser.TryAddArity(type.type_fullname, ref num);
			string[] nestedNames = type.nested_names;
			if (!nestedNames.IsNullOrEmpty<string>())
			{
				for (int i = 0; i < (int)nestedNames.Length; i++)
				{
					TypeParser.TryAddArity(nestedNames[i], ref num);
				}
			}
			type.arity = num;
			return num > 0;
		}

		private static bool TryGetArity(string name, out int arity)
		{
			arity = 0;
			int num = name.LastIndexOf('\u0060');
			if (num == -1)
			{
				return false;
			}
			return TypeParser.ParseInt32(name.Substring(num + 1), out arity);
		}

		private static bool TryGetDefinition(ModuleDefinition module, TypeParser.Type type_info, out TypeReference type)
		{
			type = null;
			if (!TypeParser.TryCurrentModule(module, type_info))
			{
				return false;
			}
			TypeDefinition nestedType = module.GetType(type_info.type_fullname);
			if (nestedType == null)
			{
				return false;
			}
			string[] nestedNames = type_info.nested_names;
			if (!nestedNames.IsNullOrEmpty<string>())
			{
				for (int i = 0; i < (int)nestedNames.Length; i++)
				{
					nestedType = nestedType.GetNestedType(nestedNames[i]);
				}
			}
			type = nestedType;
			return true;
		}

		private bool TryParse(char chr)
		{
			if (this.position >= this.length || this.fullname[this.position] != chr)
			{
				return false;
			}
			this.position++;
			return true;
		}

		private void TryParseWhiteSpace()
		{
			while (this.position < this.length && char.IsWhiteSpace(this.fullname[this.position]))
			{
				this.position++;
			}
		}

		private class Type
		{
			public const int Ptr = -1;

			public const int ByRef = -2;

			public const int SzArray = -3;

			public string type_fullname;

			public string[] nested_names;

			public int arity;

			public int[] specs;

			public TypeParser.Type[] generic_arguments;

			public string assembly;

			public Type()
			{
			}
		}
	}
}