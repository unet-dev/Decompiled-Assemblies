using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using Mono.Security.Cryptography;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Mono.Cecil
{
	internal static class Mixin
	{
		internal static object NoValue;

		internal static object NotResolved;

		public const int NotResolvedMarker = -2;

		public const int NoDataMarker = -1;

		static Mixin()
		{
			Mixin.NoValue = new object();
			Mixin.NotResolved = new object();
		}

		public static TypeDefinition CheckedResolve(this TypeReference self)
		{
			TypeDefinition typeDefinition = self.Resolve();
			if (typeDefinition == null)
			{
				throw new ResolutionException(self);
			}
			return typeDefinition;
		}

		public static void CheckModifier(TypeReference modifierType, TypeReference type)
		{
			if (modifierType == null)
			{
				throw new ArgumentNullException("modifierType");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
		}

		public static void CheckName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("Empty name");
			}
		}

		public static void CheckParameters(object parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
		}

		public static void CheckType(TypeReference type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
		}

		public static uint CompressMetadataToken(this CodedIndex self, Mono.Cecil.MetadataToken token)
		{
			Mono.Cecil.TokenType tokenType;
			uint rID = 0;
			if (token.RID == 0)
			{
				return rID;
			}
			switch (self)
			{
				case CodedIndex.TypeDefOrRef:
				{
					rID = token.RID << 2;
					tokenType = token.TokenType;
					if (tokenType == Mono.Cecil.TokenType.TypeRef)
					{
						return rID | 1;
					}
					if (tokenType == Mono.Cecil.TokenType.TypeDef)
					{
						return rID | 0;
					}
					if (tokenType == Mono.Cecil.TokenType.TypeSpec)
					{
						return rID | 2;
					}
					break;
				}
				case CodedIndex.HasConstant:
				{
					rID = token.RID << 2;
					tokenType = token.TokenType;
					if (tokenType == Mono.Cecil.TokenType.Field)
					{
						return rID | 0;
					}
					if (tokenType == Mono.Cecil.TokenType.Param)
					{
						return rID | 1;
					}
					if (tokenType == Mono.Cecil.TokenType.Property)
					{
						return rID | 2;
					}
					break;
				}
				case CodedIndex.HasCustomAttribute:
				{
					rID = token.RID << 5;
					tokenType = token.TokenType;
					if (tokenType <= Mono.Cecil.TokenType.Signature)
					{
						if (tokenType <= Mono.Cecil.TokenType.Method)
						{
							if (tokenType > Mono.Cecil.TokenType.TypeRef)
							{
								if (tokenType == Mono.Cecil.TokenType.TypeDef)
								{
									return rID | 3;
								}
								if (tokenType == Mono.Cecil.TokenType.Field)
								{
									return rID | 1;
								}
								if (tokenType == Mono.Cecil.TokenType.Method)
								{
									return rID | 0;
								}
								break;
							}
							else
							{
								if (tokenType == Mono.Cecil.TokenType.Module)
								{
									return rID | 7;
								}
								if (tokenType == Mono.Cecil.TokenType.TypeRef)
								{
									return rID | 2;
								}
								break;
							}
						}
						else if (tokenType > Mono.Cecil.TokenType.InterfaceImpl)
						{
							if (tokenType == Mono.Cecil.TokenType.MemberRef)
							{
								return rID | 6;
							}
							if (tokenType == Mono.Cecil.TokenType.Permission)
							{
								return rID | 8;
							}
							if (tokenType == Mono.Cecil.TokenType.Signature)
							{
								return rID | 11;
							}
							break;
						}
						else
						{
							if (tokenType == Mono.Cecil.TokenType.Param)
							{
								return rID | 4;
							}
							if (tokenType == Mono.Cecil.TokenType.InterfaceImpl)
							{
								return rID | 5;
							}
							break;
						}
					}
					else if (tokenType <= Mono.Cecil.TokenType.Assembly)
					{
						if (tokenType > Mono.Cecil.TokenType.Property)
						{
							if (tokenType == Mono.Cecil.TokenType.ModuleRef)
							{
								return rID | 12;
							}
							if (tokenType == Mono.Cecil.TokenType.TypeSpec)
							{
								return rID | 13;
							}
							if (tokenType == Mono.Cecil.TokenType.Assembly)
							{
								return rID | 14;
							}
							break;
						}
						else
						{
							if (tokenType == Mono.Cecil.TokenType.Event)
							{
								return rID | 10;
							}
							if (tokenType == Mono.Cecil.TokenType.Property)
							{
								return rID | 9;
							}
							break;
						}
					}
					else if (tokenType > Mono.Cecil.TokenType.File)
					{
						if (tokenType == Mono.Cecil.TokenType.ExportedType)
						{
							return rID | 17;
						}
						if (tokenType == Mono.Cecil.TokenType.ManifestResource)
						{
							return rID | 18;
						}
						if (tokenType == Mono.Cecil.TokenType.GenericParam)
						{
							return rID | 19;
						}
						break;
					}
					else
					{
						if (tokenType == Mono.Cecil.TokenType.AssemblyRef)
						{
							return rID | 15;
						}
						if (tokenType == Mono.Cecil.TokenType.File)
						{
							return rID | 16;
						}
						break;
					}
				}
				case CodedIndex.HasFieldMarshal:
				{
					rID = token.RID << 1;
					tokenType = token.TokenType;
					if (tokenType == Mono.Cecil.TokenType.Field)
					{
						return rID | 0;
					}
					if (tokenType == Mono.Cecil.TokenType.Param)
					{
						return rID | 1;
					}
					break;
				}
				case CodedIndex.HasDeclSecurity:
				{
					rID = token.RID << 2;
					tokenType = token.TokenType;
					if (tokenType == Mono.Cecil.TokenType.TypeDef)
					{
						return rID | 0;
					}
					if (tokenType == Mono.Cecil.TokenType.Method)
					{
						return rID | 1;
					}
					if (tokenType == Mono.Cecil.TokenType.Assembly)
					{
						return rID | 2;
					}
					break;
				}
				case CodedIndex.MemberRefParent:
				{
					rID = token.RID << 3;
					tokenType = token.TokenType;
					if (tokenType > Mono.Cecil.TokenType.TypeDef)
					{
						if (tokenType == Mono.Cecil.TokenType.Method)
						{
							return rID | 3;
						}
						if (tokenType == Mono.Cecil.TokenType.ModuleRef)
						{
							return rID | 2;
						}
						if (tokenType == Mono.Cecil.TokenType.TypeSpec)
						{
							return rID | 4;
						}
						break;
					}
					else
					{
						if (tokenType == Mono.Cecil.TokenType.TypeRef)
						{
							return rID | 1;
						}
						if (tokenType == Mono.Cecil.TokenType.TypeDef)
						{
							return rID | 0;
						}
						break;
					}
				}
				case CodedIndex.HasSemantics:
				{
					rID = token.RID << 1;
					tokenType = token.TokenType;
					if (tokenType == Mono.Cecil.TokenType.Event)
					{
						return rID | 0;
					}
					if (tokenType == Mono.Cecil.TokenType.Property)
					{
						return rID | 1;
					}
					break;
				}
				case CodedIndex.MethodDefOrRef:
				{
					rID = token.RID << 1;
					tokenType = token.TokenType;
					if (tokenType == Mono.Cecil.TokenType.Method)
					{
						return rID | 0;
					}
					if (tokenType == Mono.Cecil.TokenType.MemberRef)
					{
						return rID | 1;
					}
					break;
				}
				case CodedIndex.MemberForwarded:
				{
					rID = token.RID << 1;
					tokenType = token.TokenType;
					if (tokenType == Mono.Cecil.TokenType.Field)
					{
						return rID | 0;
					}
					if (tokenType == Mono.Cecil.TokenType.Method)
					{
						return rID | 1;
					}
					break;
				}
				case CodedIndex.Implementation:
				{
					rID = token.RID << 2;
					tokenType = token.TokenType;
					if (tokenType == Mono.Cecil.TokenType.AssemblyRef)
					{
						return rID | 1;
					}
					if (tokenType == Mono.Cecil.TokenType.File)
					{
						return rID | 0;
					}
					if (tokenType == Mono.Cecil.TokenType.ExportedType)
					{
						return rID | 2;
					}
					break;
				}
				case CodedIndex.CustomAttributeType:
				{
					rID = token.RID << 3;
					tokenType = token.TokenType;
					if (tokenType == Mono.Cecil.TokenType.Method)
					{
						return rID | 2;
					}
					if (tokenType == Mono.Cecil.TokenType.MemberRef)
					{
						return rID | 3;
					}
					break;
				}
				case CodedIndex.ResolutionScope:
				{
					rID = token.RID << 2;
					tokenType = token.TokenType;
					if (tokenType > Mono.Cecil.TokenType.TypeRef)
					{
						if (tokenType == Mono.Cecil.TokenType.ModuleRef)
						{
							return rID | 1;
						}
						if (tokenType == Mono.Cecil.TokenType.AssemblyRef)
						{
							return rID | 2;
						}
						break;
					}
					else
					{
						if (tokenType == Mono.Cecil.TokenType.Module)
						{
							return rID | 0;
						}
						if (tokenType == Mono.Cecil.TokenType.TypeRef)
						{
							return rID | 3;
						}
						break;
					}
				}
				case CodedIndex.TypeOrMethodDef:
				{
					rID = token.RID << 1;
					tokenType = token.TokenType;
					if (tokenType == Mono.Cecil.TokenType.TypeDef)
					{
						return rID | 0;
					}
					if (tokenType == Mono.Cecil.TokenType.Method)
					{
						return rID | 1;
					}
					break;
				}
			}
			throw new ArgumentException();
		}

		public static bool ContainsGenericParameter(this IGenericInstance self)
		{
			Collection<TypeReference> genericArguments = self.GenericArguments;
			for (int i = 0; i < genericArguments.Count; i++)
			{
				if (genericArguments[i].ContainsGenericParameter)
				{
					return true;
				}
			}
			return false;
		}

		public static RSA CreateRSA(this StrongNameKeyPair key_pair)
		{
			byte[] numArray;
			string str;
			if (!Mixin.TryGetKeyContainer(key_pair, out numArray, out str))
			{
				return CryptoConvert.FromCapiKeyBlob(numArray);
			}
			return new RSACryptoServiceProvider(new CspParameters()
			{
				Flags = CspProviderFlags.UseMachineKeyStore,
				KeyContainerName = str,
				KeyNumber = 2
			});
		}

		public static void GenericInstanceFullName(this IGenericInstance self, StringBuilder builder)
		{
			builder.Append("<");
			Collection<TypeReference> genericArguments = self.GenericArguments;
			for (int i = 0; i < genericArguments.Count; i++)
			{
				if (i > 0)
				{
					builder.Append(",");
				}
				builder.Append(genericArguments[i].FullName);
			}
			builder.Append(">");
		}

		public static bool GetAttributes(this uint self, uint attributes)
		{
			return (self & attributes) != 0;
		}

		public static bool GetAttributes(this ushort self, ushort attributes)
		{
			return (self & attributes) != 0;
		}

		public static Collection<Mono.Cecil.CustomAttribute> GetCustomAttributes(this Mono.Cecil.ICustomAttributeProvider self, ref Collection<Mono.Cecil.CustomAttribute> variable, ModuleDefinition module)
		{
			if (!module.HasImage())
			{
				Collection<Mono.Cecil.CustomAttribute> customAttributes = new Collection<Mono.Cecil.CustomAttribute>();
				Collection<Mono.Cecil.CustomAttribute> customAttributes1 = customAttributes;
				variable = customAttributes;
				return customAttributes1;
			}
			return module.Read<Mono.Cecil.ICustomAttributeProvider, Collection<Mono.Cecil.CustomAttribute>>(ref variable, self, (Mono.Cecil.ICustomAttributeProvider provider, MetadataReader reader) => reader.ReadCustomAttributes(provider));
		}

		public static TypeReference GetEnumUnderlyingType(this TypeDefinition self)
		{
			Collection<FieldDefinition> fields = self.Fields;
			for (int i = 0; i < fields.Count; i++)
			{
				FieldDefinition item = fields[i];
				if (!item.IsStatic)
				{
					return item.FieldType;
				}
			}
			throw new ArgumentException();
		}

		public static string GetFullyQualifiedName(this Stream self)
		{
			FileStream fileStream = self as FileStream;
			if (fileStream == null)
			{
				return string.Empty;
			}
			return Path.GetFullPath(fileStream.Name);
		}

		public static Collection<GenericParameter> GetGenericParameters(this IGenericParameterProvider self, ref Collection<GenericParameter> collection, ModuleDefinition module)
		{
			if (!module.HasImage())
			{
				GenericParameterCollection genericParameterCollection = new GenericParameterCollection(self);
				Collection<GenericParameter> genericParameters = genericParameterCollection;
				collection = genericParameterCollection;
				return genericParameters;
			}
			return module.Read<IGenericParameterProvider, Collection<GenericParameter>>(ref collection, self, (IGenericParameterProvider provider, MetadataReader reader) => reader.ReadGenericParameters(provider));
		}

		public static bool GetHasCustomAttributes(this Mono.Cecil.ICustomAttributeProvider self, ModuleDefinition module)
		{
			if (!module.HasImage())
			{
				return false;
			}
			return module.Read<Mono.Cecil.ICustomAttributeProvider, bool>(self, (Mono.Cecil.ICustomAttributeProvider provider, MetadataReader reader) => reader.HasCustomAttributes(provider));
		}

		public static bool GetHasGenericParameters(this IGenericParameterProvider self, ModuleDefinition module)
		{
			if (!module.HasImage())
			{
				return false;
			}
			return module.Read<IGenericParameterProvider, bool>(self, (IGenericParameterProvider provider, MetadataReader reader) => reader.HasGenericParameters(provider));
		}

		public static bool GetHasMarshalInfo(this IMarshalInfoProvider self, ModuleDefinition module)
		{
			if (!module.HasImage())
			{
				return false;
			}
			return module.Read<IMarshalInfoProvider, bool>(self, (IMarshalInfoProvider provider, MetadataReader reader) => reader.HasMarshalInfo(provider));
		}

		public static bool GetHasSecurityDeclarations(this ISecurityDeclarationProvider self, ModuleDefinition module)
		{
			if (!module.HasImage())
			{
				return false;
			}
			return module.Read<ISecurityDeclarationProvider, bool>(self, (ISecurityDeclarationProvider provider, MetadataReader reader) => reader.HasSecurityDeclarations(provider));
		}

		public static MarshalInfo GetMarshalInfo(this IMarshalInfoProvider self, ref MarshalInfo variable, ModuleDefinition module)
		{
			if (!module.HasImage())
			{
				return null;
			}
			return module.Read<IMarshalInfoProvider, MarshalInfo>(ref variable, self, (IMarshalInfoProvider provider, MetadataReader reader) => reader.ReadMarshalInfo(provider));
		}

		public static bool GetMaskedAttributes(this uint self, uint mask, uint attributes)
		{
			return (self & mask) == attributes;
		}

		public static bool GetMaskedAttributes(this ushort self, ushort mask, uint attributes)
		{
			return (long)(self & mask) == (ulong)attributes;
		}

		public static Mono.Cecil.MetadataToken GetMetadataToken(this CodedIndex self, uint data)
		{
			uint num;
			Mono.Cecil.TokenType tokenType;
			uint num1;
			switch (self)
			{
				case CodedIndex.TypeDefOrRef:
				{
					num = data >> 2;
					num1 = data & 3;
					switch (num1)
					{
						case 0:
						{
							tokenType = Mono.Cecil.TokenType.TypeDef;
							break;
						}
						case 1:
						{
							tokenType = Mono.Cecil.TokenType.TypeRef;
							break;
						}
						case 2:
						{
							tokenType = Mono.Cecil.TokenType.TypeSpec;
							break;
						}
						default:
						{
							return Mono.Cecil.MetadataToken.Zero;
						}
					}
					break;
				}
				case CodedIndex.HasConstant:
				{
					num = data >> 2;
					num1 = data & 3;
					switch (num1)
					{
						case 0:
						{
							tokenType = Mono.Cecil.TokenType.Field;
							break;
						}
						case 1:
						{
							tokenType = Mono.Cecil.TokenType.Param;
							break;
						}
						case 2:
						{
							tokenType = Mono.Cecil.TokenType.Property;
							break;
						}
						default:
						{
							return Mono.Cecil.MetadataToken.Zero;
						}
					}
					break;
				}
				case CodedIndex.HasCustomAttribute:
				{
					num = data >> 5;
					num1 = data & 31;
					switch (num1)
					{
						case 0:
						{
							tokenType = Mono.Cecil.TokenType.Method;
							break;
						}
						case 1:
						{
							tokenType = Mono.Cecil.TokenType.Field;
							break;
						}
						case 2:
						{
							tokenType = Mono.Cecil.TokenType.TypeRef;
							break;
						}
						case 3:
						{
							tokenType = Mono.Cecil.TokenType.TypeDef;
							break;
						}
						case 4:
						{
							tokenType = Mono.Cecil.TokenType.Param;
							break;
						}
						case 5:
						{
							tokenType = Mono.Cecil.TokenType.InterfaceImpl;
							break;
						}
						case 6:
						{
							tokenType = Mono.Cecil.TokenType.MemberRef;
							break;
						}
						case 7:
						{
							tokenType = Mono.Cecil.TokenType.Module;
							break;
						}
						case 8:
						{
							tokenType = Mono.Cecil.TokenType.Permission;
							break;
						}
						case 9:
						{
							tokenType = Mono.Cecil.TokenType.Property;
							break;
						}
						case 10:
						{
							tokenType = Mono.Cecil.TokenType.Event;
							break;
						}
						case 11:
						{
							tokenType = Mono.Cecil.TokenType.Signature;
							break;
						}
						case 12:
						{
							tokenType = Mono.Cecil.TokenType.ModuleRef;
							break;
						}
						case 13:
						{
							tokenType = Mono.Cecil.TokenType.TypeSpec;
							break;
						}
						case 14:
						{
							tokenType = Mono.Cecil.TokenType.Assembly;
							break;
						}
						case 15:
						{
							tokenType = Mono.Cecil.TokenType.AssemblyRef;
							break;
						}
						case 16:
						{
							tokenType = Mono.Cecil.TokenType.File;
							break;
						}
						case 17:
						{
							tokenType = Mono.Cecil.TokenType.ExportedType;
							break;
						}
						case 18:
						{
							tokenType = Mono.Cecil.TokenType.ManifestResource;
							break;
						}
						case 19:
						{
							tokenType = Mono.Cecil.TokenType.GenericParam;
							break;
						}
						default:
						{
							return Mono.Cecil.MetadataToken.Zero;
						}
					}
					break;
				}
				case CodedIndex.HasFieldMarshal:
				{
					num = data >> 1;
					num1 = data & 1;
					if (num1 == 0)
					{
						tokenType = Mono.Cecil.TokenType.Field;
						break;
					}
					else if (num1 == 1)
					{
						tokenType = Mono.Cecil.TokenType.Param;
						break;
					}
					else
					{
						return Mono.Cecil.MetadataToken.Zero;
					}
				}
				case CodedIndex.HasDeclSecurity:
				{
					num = data >> 2;
					num1 = data & 3;
					switch (num1)
					{
						case 0:
						{
							tokenType = Mono.Cecil.TokenType.TypeDef;
							break;
						}
						case 1:
						{
							tokenType = Mono.Cecil.TokenType.Method;
							break;
						}
						case 2:
						{
							tokenType = Mono.Cecil.TokenType.Assembly;
							break;
						}
						default:
						{
							return Mono.Cecil.MetadataToken.Zero;
						}
					}
					break;
				}
				case CodedIndex.MemberRefParent:
				{
					num = data >> 3;
					num1 = data & 7;
					switch (num1)
					{
						case 0:
						{
							tokenType = Mono.Cecil.TokenType.TypeDef;
							break;
						}
						case 1:
						{
							tokenType = Mono.Cecil.TokenType.TypeRef;
							break;
						}
						case 2:
						{
							tokenType = Mono.Cecil.TokenType.ModuleRef;
							break;
						}
						case 3:
						{
							tokenType = Mono.Cecil.TokenType.Method;
							break;
						}
						case 4:
						{
							tokenType = Mono.Cecil.TokenType.TypeSpec;
							break;
						}
						default:
						{
							return Mono.Cecil.MetadataToken.Zero;
						}
					}
					break;
				}
				case CodedIndex.HasSemantics:
				{
					num = data >> 1;
					num1 = data & 1;
					if (num1 == 0)
					{
						tokenType = Mono.Cecil.TokenType.Event;
						break;
					}
					else if (num1 == 1)
					{
						tokenType = Mono.Cecil.TokenType.Property;
						break;
					}
					else
					{
						return Mono.Cecil.MetadataToken.Zero;
					}
				}
				case CodedIndex.MethodDefOrRef:
				{
					num = data >> 1;
					num1 = data & 1;
					if (num1 == 0)
					{
						tokenType = Mono.Cecil.TokenType.Method;
						break;
					}
					else if (num1 == 1)
					{
						tokenType = Mono.Cecil.TokenType.MemberRef;
						break;
					}
					else
					{
						return Mono.Cecil.MetadataToken.Zero;
					}
				}
				case CodedIndex.MemberForwarded:
				{
					num = data >> 1;
					num1 = data & 1;
					if (num1 == 0)
					{
						tokenType = Mono.Cecil.TokenType.Field;
						break;
					}
					else if (num1 == 1)
					{
						tokenType = Mono.Cecil.TokenType.Method;
						break;
					}
					else
					{
						return Mono.Cecil.MetadataToken.Zero;
					}
				}
				case CodedIndex.Implementation:
				{
					num = data >> 2;
					num1 = data & 3;
					switch (num1)
					{
						case 0:
						{
							tokenType = Mono.Cecil.TokenType.File;
							break;
						}
						case 1:
						{
							tokenType = Mono.Cecil.TokenType.AssemblyRef;
							break;
						}
						case 2:
						{
							tokenType = Mono.Cecil.TokenType.ExportedType;
							break;
						}
						default:
						{
							return Mono.Cecil.MetadataToken.Zero;
						}
					}
					break;
				}
				case CodedIndex.CustomAttributeType:
				{
					num = data >> 3;
					num1 = data & 7;
					if (num1 == 2)
					{
						tokenType = Mono.Cecil.TokenType.Method;
						break;
					}
					else if (num1 == 3)
					{
						tokenType = Mono.Cecil.TokenType.MemberRef;
						break;
					}
					else
					{
						return Mono.Cecil.MetadataToken.Zero;
					}
				}
				case CodedIndex.ResolutionScope:
				{
					num = data >> 2;
					num1 = data & 3;
					switch (num1)
					{
						case 0:
						{
							tokenType = Mono.Cecil.TokenType.Module;
							break;
						}
						case 1:
						{
							tokenType = Mono.Cecil.TokenType.ModuleRef;
							break;
						}
						case 2:
						{
							tokenType = Mono.Cecil.TokenType.AssemblyRef;
							break;
						}
						case 3:
						{
							tokenType = Mono.Cecil.TokenType.TypeRef;
							break;
						}
						default:
						{
							return Mono.Cecil.MetadataToken.Zero;
						}
					}
					break;
				}
				case CodedIndex.TypeOrMethodDef:
				{
					num = data >> 1;
					num1 = data & 1;
					if (num1 == 0)
					{
						tokenType = Mono.Cecil.TokenType.TypeDef;
						break;
					}
					else if (num1 == 1)
					{
						tokenType = Mono.Cecil.TokenType.Method;
						break;
					}
					else
					{
						return Mono.Cecil.MetadataToken.Zero;
					}
				}
				default:
				{
					return Mono.Cecil.MetadataToken.Zero;
				}
			}
			return new Mono.Cecil.MetadataToken(tokenType, num);
		}

		public static TypeDefinition GetNestedType(this TypeDefinition self, string fullname)
		{
			if (!self.HasNestedTypes)
			{
				return null;
			}
			Collection<TypeDefinition> nestedTypes = self.NestedTypes;
			for (int i = 0; i < nestedTypes.Count; i++)
			{
				TypeDefinition item = nestedTypes[i];
				if (item.TypeFullName() == fullname)
				{
					return item;
				}
			}
			return null;
		}

		public static ParameterDefinition GetParameter(this Mono.Cecil.Cil.MethodBody self, int index)
		{
			MethodDefinition methodDefinition = self.method;
			if (methodDefinition.HasThis)
			{
				if (index == 0)
				{
					return self.ThisParameter;
				}
				index--;
			}
			Collection<ParameterDefinition> parameters = methodDefinition.Parameters;
			if (index < 0 || index >= parameters.size)
			{
				return null;
			}
			return parameters[index];
		}

		public static Collection<SecurityDeclaration> GetSecurityDeclarations(this ISecurityDeclarationProvider self, ref Collection<SecurityDeclaration> variable, ModuleDefinition module)
		{
			if (!module.HasImage())
			{
				Collection<SecurityDeclaration> securityDeclarations = new Collection<SecurityDeclaration>();
				Collection<SecurityDeclaration> securityDeclarations1 = securityDeclarations;
				variable = securityDeclarations;
				return securityDeclarations1;
			}
			return module.Read<ISecurityDeclarationProvider, Collection<SecurityDeclaration>>(ref variable, self, (ISecurityDeclarationProvider provider, MetadataReader reader) => reader.ReadSecurityDeclarations(provider));
		}

		public static bool GetSemantics(this MethodDefinition self, Mono.Cecil.MethodSemanticsAttributes semantics)
		{
			return (self.SemanticsAttributes & semantics) != Mono.Cecil.MethodSemanticsAttributes.None;
		}

		public static int GetSentinelPosition(this IMethodSignature self)
		{
			if (!self.HasParameters)
			{
				return -1;
			}
			Collection<ParameterDefinition> parameters = self.Parameters;
			for (int i = 0; i < parameters.Count; i++)
			{
				if (parameters[i].ParameterType.IsSentinel)
				{
					return i;
				}
			}
			return -1;
		}

		public static int GetSize(this CodedIndex self, Func<Table, int> counter)
		{
			int num;
			Table[] tableArray;
			switch (self)
			{
				case CodedIndex.TypeDefOrRef:
				{
					num = 2;
					tableArray = new Table[] { Table.TypeDef, Table.TypeRef, Table.TypeSpec };
					break;
				}
				case CodedIndex.HasConstant:
				{
					num = 2;
					tableArray = new Table[] { Table.Field, Table.Param, Table.Property };
					break;
				}
				case CodedIndex.HasCustomAttribute:
				{
					num = 5;
					tableArray = new Table[] { Table.Method, Table.Field, Table.TypeRef, Table.TypeDef, Table.Param, Table.InterfaceImpl, Table.MemberRef, Table.Module, Table.DeclSecurity, Table.Property, Table.Event, Table.StandAloneSig, Table.ModuleRef, Table.TypeSpec, Table.Assembly, Table.AssemblyRef, Table.File, Table.ExportedType, Table.ManifestResource, Table.GenericParam };
					break;
				}
				case CodedIndex.HasFieldMarshal:
				{
					num = 1;
					tableArray = new Table[] { Table.Field, Table.Param };
					break;
				}
				case CodedIndex.HasDeclSecurity:
				{
					num = 2;
					tableArray = new Table[] { Table.TypeDef, Table.Method, Table.Assembly };
					break;
				}
				case CodedIndex.MemberRefParent:
				{
					num = 3;
					tableArray = new Table[] { Table.TypeDef, Table.TypeRef, Table.ModuleRef, Table.Method, Table.TypeSpec };
					break;
				}
				case CodedIndex.HasSemantics:
				{
					num = 1;
					tableArray = new Table[] { Table.Event, Table.Property };
					break;
				}
				case CodedIndex.MethodDefOrRef:
				{
					num = 1;
					tableArray = new Table[] { Table.Method, Table.MemberRef };
					break;
				}
				case CodedIndex.MemberForwarded:
				{
					num = 1;
					tableArray = new Table[] { Table.Field, Table.Method };
					break;
				}
				case CodedIndex.Implementation:
				{
					num = 2;
					tableArray = new Table[] { Table.File, Table.AssemblyRef, Table.ExportedType };
					break;
				}
				case CodedIndex.CustomAttributeType:
				{
					num = 3;
					tableArray = new Table[] { Table.Method, Table.MemberRef };
					break;
				}
				case CodedIndex.ResolutionScope:
				{
					num = 2;
					tableArray = new Table[] { Table.Module, Table.ModuleRef, Table.AssemblyRef, Table.TypeRef };
					break;
				}
				case CodedIndex.TypeOrMethodDef:
				{
					num = 1;
					tableArray = new Table[] { Table.TypeDef, Table.Method };
					break;
				}
				default:
				{
					throw new ArgumentException();
				}
			}
			int num1 = 0;
			for (int i = 0; i < (int)tableArray.Length; i++)
			{
				num1 = System.Math.Max(counter(tableArray[i]), num1);
			}
			if (num1 >= 1 << (16 - num & 31))
			{
				return 4;
			}
			return 2;
		}

		public static VariableDefinition GetVariable(this Mono.Cecil.Cil.MethodBody self, int index)
		{
			Collection<VariableDefinition> variables = self.Variables;
			if (index < 0 || index >= variables.size)
			{
				return null;
			}
			return variables[index];
		}

		public static bool HasImage(this ModuleDefinition self)
		{
			if (self == null)
			{
				return false;
			}
			return self.HasImage;
		}

		public static bool HasImplicitThis(this IMethodSignature self)
		{
			if (!self.HasThis)
			{
				return false;
			}
			return !self.ExplicitThis;
		}

		public static bool IsCorlib(this ModuleDefinition module)
		{
			if (module.Assembly == null)
			{
				return false;
			}
			return module.Assembly.Name.Name == "mscorlib";
		}

		public static bool IsNullOrEmpty<T>(this T[] self)
		{
			if (self == null)
			{
				return true;
			}
			return self.Length == 0;
		}

		public static bool IsNullOrEmpty<T>(this Collection<T> self)
		{
			if (self == null)
			{
				return true;
			}
			return self.size == 0;
		}

		public static bool IsPrimitive(this ElementType self)
		{
			switch (self)
			{
				case ElementType.Boolean:
				case ElementType.Char:
				case ElementType.I1:
				case ElementType.U1:
				case ElementType.I2:
				case ElementType.U2:
				case ElementType.I4:
				case ElementType.U4:
				case ElementType.I8:
				case ElementType.U8:
				case ElementType.R4:
				case ElementType.R8:
				case ElementType.I:
				case ElementType.U:
				{
					return true;
				}
				case ElementType.String:
				case ElementType.Ptr:
				case ElementType.ByRef:
				case ElementType.ValueType:
				case ElementType.Class:
				case ElementType.Var:
				case ElementType.Array:
				case ElementType.GenericInst:
				case ElementType.TypedByRef:
				case ElementType.Void | ElementType.Boolean | ElementType.Char | ElementType.I1 | ElementType.U1 | ElementType.I2 | ElementType.U2 | ElementType.ByRef | ElementType.ValueType | ElementType.Class | ElementType.Var | ElementType.Array | ElementType.GenericInst | ElementType.TypedByRef:
				{
					return false;
				}
				default:
				{
					return false;
				}
			}
		}

		public static bool IsTypeOf(this TypeReference self, string @namespace, string name)
		{
			if (self.Name != name)
			{
				return false;
			}
			return self.Namespace == @namespace;
		}

		public static bool IsTypeSpecification(this TypeReference type)
		{
			ElementType elementType = type.etype;
			switch (elementType)
			{
				case ElementType.Ptr:
				case ElementType.ByRef:
				case ElementType.Var:
				case ElementType.Array:
				case ElementType.GenericInst:
				case ElementType.FnPtr:
				case ElementType.SzArray:
				case ElementType.MVar:
				case ElementType.CModReqD:
				case ElementType.CModOpt:
				{
					return true;
				}
				case ElementType.ValueType:
				case ElementType.Class:
				case ElementType.TypedByRef:
				case ElementType.Void | ElementType.Boolean | ElementType.Char | ElementType.I1 | ElementType.U1 | ElementType.I2 | ElementType.U2 | ElementType.ByRef | ElementType.ValueType | ElementType.Class | ElementType.Var | ElementType.Array | ElementType.GenericInst | ElementType.TypedByRef:
				case ElementType.I:
				case ElementType.U:
				case ElementType.Boolean | ElementType.I4 | ElementType.I8 | ElementType.ByRef | ElementType.Class | ElementType.I:
				case ElementType.Object:
				{
					return false;
				}
				default:
				{
					if (elementType == ElementType.Sentinel || elementType == ElementType.Pinned)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
		}

		public static bool IsVarArg(this IMethodSignature self)
		{
			return (self.CallingConvention & MethodCallingConvention.VarArg) != MethodCallingConvention.Default;
		}

		public static void MethodSignatureFullName(this IMethodSignature self, StringBuilder builder)
		{
			builder.Append("(");
			if (self.HasParameters)
			{
				Collection<ParameterDefinition> parameters = self.Parameters;
				for (int i = 0; i < parameters.Count; i++)
				{
					ParameterDefinition item = parameters[i];
					if (i > 0)
					{
						builder.Append(",");
					}
					if (item.ParameterType.IsSentinel)
					{
						builder.Append("...,");
					}
					builder.Append(item.ParameterType.FullName);
				}
			}
			builder.Append(")");
		}

		public static TargetRuntime ParseRuntime(this string self)
		{
			switch (self[1])
			{
				case '1':
				{
					if (self[3] != '0')
					{
						return TargetRuntime.Net_1_1;
					}
					return TargetRuntime.Net_1_0;
				}
				case '2':
				{
					return TargetRuntime.Net_2_0;
				}
				case '3':
				case '4':
				{
					return TargetRuntime.Net_4_0;
				}
				default:
				{
					return TargetRuntime.Net_4_0;
				}
			}
		}

		public static uint ReadCompressedUInt32(this byte[] data, ref int position)
		{
			uint num;
			if ((data[position] & 128) == 0)
			{
				num = data[position];
				position++;
			}
			else if ((data[position] & 64) != 0)
			{
				num = (uint)((data[position] & -193) << 24);
				num = num | data[position + 1] << 16;
				num = num | data[position + 2] << 8;
				num |= data[position + 3];
				position += 4;
			}
			else
			{
				num = (uint)((data[position] & -129) << 8);
				num |= data[position + 1];
				position += 2;
			}
			return num;
		}

		public static T[] Resize<T>(this T[] self, int length)
		{
			Array.Resize<T>(ref self, length);
			return self;
		}

		public static void ResolveConstant(this IConstantProvider self, ref object constant, ModuleDefinition module)
		{
			if (module == null)
			{
				constant = Mixin.NoValue;
				return;
			}
			lock (module.SyncRoot)
			{
				if (constant == Mixin.NotResolved)
				{
					if (!module.HasImage())
					{
						constant = Mixin.NoValue;
					}
					else
					{
						constant = module.Read<IConstantProvider, object>(self, (IConstantProvider provider, MetadataReader reader) => reader.ReadConstant(provider));
					}
				}
			}
		}

		public static string RuntimeVersionString(this TargetRuntime runtime)
		{
			switch (runtime)
			{
				case TargetRuntime.Net_1_0:
				{
					return "v1.0.3705";
				}
				case TargetRuntime.Net_1_1:
				{
					return "v1.1.4322";
				}
				case TargetRuntime.Net_2_0:
				{
					return "v2.0.50727";
				}
				case TargetRuntime.Net_4_0:
				{
					return "v4.0.30319";
				}
				default:
				{
					return "v4.0.30319";
				}
			}
		}

		public static uint SetAttributes(this uint self, uint attributes, bool value)
		{
			if (value)
			{
				return self | attributes;
			}
			return self & ~attributes;
		}

		public static ushort SetAttributes(this ushort self, ushort attributes, bool value)
		{
			if (value)
			{
				return (ushort)(self | attributes);
			}
			return (ushort)(self & ~attributes);
		}

		public static uint SetMaskedAttributes(this uint self, uint mask, uint attributes, bool value)
		{
			if (!value)
			{
				return self & ~(mask & attributes);
			}
			self &= ~mask;
			return self | attributes;
		}

		public static ushort SetMaskedAttributes(this ushort self, ushort mask, uint attributes, bool value)
		{
			if (!value)
			{
				return (ushort)(self & ~(mask & attributes));
			}
			self = (ushort)(self & ~mask);
			return (ushort)(self | attributes);
		}

		public static void SetSemantics(this MethodDefinition self, Mono.Cecil.MethodSemanticsAttributes semantics, bool value)
		{
			if (value)
			{
				MethodDefinition semanticsAttributes = self;
				semanticsAttributes.SemanticsAttributes = semanticsAttributes.SemanticsAttributes | semantics;
				return;
			}
			MethodDefinition methodDefinition = self;
			methodDefinition.SemanticsAttributes = (Mono.Cecil.MethodSemanticsAttributes)((ushort)methodDefinition.SemanticsAttributes & (ushort)(~semantics));
		}

		private static bool TryGetKeyContainer(ISerializable key_pair, out byte[] key, out string key_container)
		{
			SerializationInfo serializationInfo = new SerializationInfo(typeof(StrongNameKeyPair), new FormatterConverter());
			key_pair.GetObjectData(serializationInfo, new StreamingContext());
			key = (byte[])serializationInfo.GetValue("_keyPairArray", typeof(byte[]));
			key_container = serializationInfo.GetString("_keyPairContainer");
			return key_container != null;
		}

		public static string TypeFullName(this TypeReference self)
		{
			if (string.IsNullOrEmpty(self.Namespace))
			{
				return self.Name;
			}
			return string.Concat(self.Namespace, ".", self.Name);
		}
	}
}