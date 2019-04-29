using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;
using System;
using System.Text;

namespace Mono.Cecil
{
	internal sealed class SignatureWriter : ByteBuffer
	{
		private readonly MetadataBuilder metadata;

		public SignatureWriter(MetadataBuilder metadata) : base(6)
		{
			this.metadata = metadata;
		}

		private static int GetNamedArgumentCount(ICustomAttribute attribute)
		{
			int count = 0;
			if (attribute.HasFields)
			{
				count += attribute.Fields.Count;
			}
			if (attribute.HasProperties)
			{
				count += attribute.Properties.Count;
			}
			return count;
		}

		private static string GetXmlSecurityDeclaration(SecurityDeclaration declaration)
		{
			if (declaration.security_attributes == null || declaration.security_attributes.Count != 1)
			{
				return null;
			}
			SecurityAttribute item = declaration.security_attributes[0];
			if (!item.AttributeType.IsTypeOf("System.Security.Permissions", "PermissionSetAttribute"))
			{
				return null;
			}
			if (item.properties == null || item.properties.Count != 1)
			{
				return null;
			}
			CustomAttributeNamedArgument customAttributeNamedArgument = item.properties[0];
			if (customAttributeNamedArgument.Name != "XML")
			{
				return null;
			}
			return (string)customAttributeNamedArgument.Argument.Value;
		}

		private uint MakeTypeDefOrRefCodedRID(TypeReference type)
		{
			return CodedIndex.TypeDefOrRef.CompressMetadataToken(this.metadata.LookupToken(type));
		}

		private bool TryWriteElementType(TypeReference type)
		{
			ElementType elementType = type.etype;
			if (elementType == ElementType.None)
			{
				return false;
			}
			this.WriteElementType(elementType);
			return true;
		}

		private void WriteArrayTypeSignature(ArrayType array)
		{
			this.WriteElementType(ElementType.Array);
			this.WriteTypeSignature(array.ElementType);
			Collection<ArrayDimension> dimensions = array.Dimensions;
			int count = dimensions.Count;
			base.WriteCompressedUInt32((uint)count);
			int num = 0;
			int num1 = 0;
			for (int i = 0; i < count; i++)
			{
				ArrayDimension item = dimensions[i];
				if (item.UpperBound.HasValue)
				{
					num++;
					num1++;
				}
				else if (item.LowerBound.HasValue)
				{
					num1++;
				}
			}
			int[] value = new int[num];
			int[] valueOrDefault = new int[num1];
			for (int j = 0; j < num1; j++)
			{
				ArrayDimension arrayDimension = dimensions[j];
				int? lowerBound = arrayDimension.LowerBound;
				valueOrDefault[j] = lowerBound.GetValueOrDefault();
				if (arrayDimension.UpperBound.HasValue)
				{
					lowerBound = arrayDimension.UpperBound;
					value[j] = lowerBound.Value - valueOrDefault[j] + 1;
				}
			}
			base.WriteCompressedUInt32((uint)num);
			for (int k = 0; k < num; k++)
			{
				base.WriteCompressedUInt32((uint)value[k]);
			}
			base.WriteCompressedUInt32((uint)num1);
			for (int l = 0; l < num1; l++)
			{
				base.WriteCompressedInt32(valueOrDefault[l]);
			}
		}

		public void WriteConstantPrimitive(object value)
		{
			this.WritePrimitiveValue(value);
		}

		public void WriteConstantString(string value)
		{
			base.WriteBytes(Encoding.Unicode.GetBytes(value));
		}

		public void WriteCustomAttributeConstructorArguments(CustomAttribute attribute)
		{
			if (!attribute.HasConstructorArguments)
			{
				return;
			}
			Collection<CustomAttributeArgument> constructorArguments = attribute.ConstructorArguments;
			Collection<ParameterDefinition> parameters = attribute.Constructor.Parameters;
			if (parameters.Count != constructorArguments.Count)
			{
				throw new InvalidOperationException();
			}
			for (int i = 0; i < constructorArguments.Count; i++)
			{
				this.WriteCustomAttributeFixedArgument(parameters[i].ParameterType, constructorArguments[i]);
			}
		}

		private void WriteCustomAttributeElement(TypeReference type, CustomAttributeArgument argument)
		{
			if (type.IsArray)
			{
				this.WriteCustomAttributeFixedArrayArgument((ArrayType)type, argument);
				return;
			}
			if (type.etype != ElementType.Object)
			{
				this.WriteCustomAttributeValue(type, argument.Value);
				return;
			}
			argument = (CustomAttributeArgument)argument.Value;
			type = argument.Type;
			this.WriteCustomAttributeFieldOrPropType(type);
			this.WriteCustomAttributeElement(type, argument);
		}

		private void WriteCustomAttributeEnumValue(TypeReference enum_type, object value)
		{
			TypeDefinition typeDefinition = enum_type.CheckedResolve();
			if (!typeDefinition.IsEnum)
			{
				throw new ArgumentException();
			}
			this.WriteCustomAttributeValue(typeDefinition.GetEnumUnderlyingType(), value);
		}

		private void WriteCustomAttributeFieldOrPropType(TypeReference type)
		{
			if (type.IsArray)
			{
				ArrayType arrayType = (ArrayType)type;
				this.WriteElementType(ElementType.SzArray);
				this.WriteCustomAttributeFieldOrPropType(arrayType.ElementType);
				return;
			}
			ElementType elementType = type.etype;
			if (elementType != ElementType.None)
			{
				if (elementType == ElementType.Object)
				{
					this.WriteElementType(ElementType.Boxed);
					return;
				}
				this.WriteElementType(elementType);
				return;
			}
			if (type.IsTypeOf("System", "Type"))
			{
				this.WriteElementType(ElementType.Type);
				return;
			}
			this.WriteElementType(ElementType.Enum);
			this.WriteTypeReference(type);
		}

		private void WriteCustomAttributeFixedArgument(TypeReference type, CustomAttributeArgument argument)
		{
			if (!type.IsArray)
			{
				this.WriteCustomAttributeElement(type, argument);
				return;
			}
			this.WriteCustomAttributeFixedArrayArgument((ArrayType)type, argument);
		}

		private void WriteCustomAttributeFixedArrayArgument(ArrayType type, CustomAttributeArgument argument)
		{
			CustomAttributeArgument[] value = argument.Value as CustomAttributeArgument[];
			if (value == null)
			{
				base.WriteUInt32(-1);
				return;
			}
			base.WriteInt32((int)value.Length);
			if (value.Length == 0)
			{
				return;
			}
			TypeReference elementType = type.ElementType;
			for (int i = 0; i < (int)value.Length; i++)
			{
				this.WriteCustomAttributeElement(elementType, value[i]);
			}
		}

		private void WriteCustomAttributeNamedArgument(byte kind, CustomAttributeNamedArgument named_argument)
		{
			CustomAttributeArgument argument = named_argument.Argument;
			base.WriteByte(kind);
			this.WriteCustomAttributeFieldOrPropType(argument.Type);
			this.WriteUTF8String(named_argument.Name);
			this.WriteCustomAttributeFixedArgument(argument.Type, argument);
		}

		public void WriteCustomAttributeNamedArguments(CustomAttribute attribute)
		{
			int namedArgumentCount = SignatureWriter.GetNamedArgumentCount(attribute);
			base.WriteUInt16((ushort)namedArgumentCount);
			if (namedArgumentCount == 0)
			{
				return;
			}
			this.WriteICustomAttributeNamedArguments(attribute);
		}

		private void WriteCustomAttributeNamedArguments(byte kind, Collection<CustomAttributeNamedArgument> named_arguments)
		{
			for (int i = 0; i < named_arguments.Count; i++)
			{
				this.WriteCustomAttributeNamedArgument(kind, named_arguments[i]);
			}
		}

		private void WriteCustomAttributeValue(TypeReference type, object value)
		{
			ElementType elementType = type.etype;
			if (elementType == ElementType.None)
			{
				if (type.IsTypeOf("System", "Type"))
				{
					this.WriteTypeReference((TypeReference)value);
					return;
				}
				this.WriteCustomAttributeEnumValue(type, value);
				return;
			}
			if (elementType != ElementType.String)
			{
				this.WritePrimitiveValue(value);
				return;
			}
			string str = (string)value;
			if (str == null)
			{
				base.WriteByte(255);
				return;
			}
			this.WriteUTF8String(str);
		}

		public void WriteElementType(ElementType element_type)
		{
			base.WriteByte((byte)element_type);
		}

		public void WriteGenericInstanceSignature(IGenericInstance instance)
		{
			Collection<TypeReference> genericArguments = instance.GenericArguments;
			int count = genericArguments.Count;
			base.WriteCompressedUInt32((uint)count);
			for (int i = 0; i < count; i++)
			{
				this.WriteTypeSignature(genericArguments[i]);
			}
		}

		private void WriteICustomAttributeNamedArguments(ICustomAttribute attribute)
		{
			if (attribute.HasFields)
			{
				this.WriteCustomAttributeNamedArguments(83, attribute.Fields);
			}
			if (attribute.HasProperties)
			{
				this.WriteCustomAttributeNamedArguments(84, attribute.Properties);
			}
		}

		public void WriteMarshalInfo(MarshalInfo marshal_info)
		{
			this.WriteNativeType(marshal_info.native);
			NativeType marshalInfo = marshal_info.native;
			if (marshalInfo <= NativeType.SafeArray)
			{
				if (marshalInfo == NativeType.FixedSysString)
				{
					FixedSysStringMarshalInfo fixedSysStringMarshalInfo = (FixedSysStringMarshalInfo)marshal_info;
					if (fixedSysStringMarshalInfo.size > -1)
					{
						base.WriteCompressedUInt32((uint)fixedSysStringMarshalInfo.size);
					}
					return;
				}
				if (marshalInfo != NativeType.SafeArray)
				{
					return;
				}
				SafeArrayMarshalInfo safeArrayMarshalInfo = (SafeArrayMarshalInfo)marshal_info;
				if (safeArrayMarshalInfo.element_type != VariantType.None)
				{
					this.WriteVariantType(safeArrayMarshalInfo.element_type);
				}
				return;
			}
			if (marshalInfo == NativeType.FixedArray)
			{
				FixedArrayMarshalInfo fixedArrayMarshalInfo = (FixedArrayMarshalInfo)marshal_info;
				if (fixedArrayMarshalInfo.size > -1)
				{
					base.WriteCompressedUInt32((uint)fixedArrayMarshalInfo.size);
				}
				if (fixedArrayMarshalInfo.element_type != NativeType.None)
				{
					this.WriteNativeType(fixedArrayMarshalInfo.element_type);
				}
				return;
			}
			if (marshalInfo != NativeType.Array)
			{
				if (marshalInfo != NativeType.CustomMarshaler)
				{
					return;
				}
				CustomMarshalInfo customMarshalInfo = (CustomMarshalInfo)marshal_info;
				this.WriteUTF8String((customMarshalInfo.guid != Guid.Empty ? customMarshalInfo.guid.ToString() : string.Empty));
				this.WriteUTF8String(customMarshalInfo.unmanaged_type);
				this.WriteTypeReference(customMarshalInfo.managed_type);
				this.WriteUTF8String(customMarshalInfo.cookie);
				return;
			}
			ArrayMarshalInfo arrayMarshalInfo = (ArrayMarshalInfo)marshal_info;
			if (arrayMarshalInfo.element_type != NativeType.None)
			{
				this.WriteNativeType(arrayMarshalInfo.element_type);
			}
			if (arrayMarshalInfo.size_parameter_index > -1)
			{
				base.WriteCompressedUInt32((uint)arrayMarshalInfo.size_parameter_index);
			}
			if (arrayMarshalInfo.size > -1)
			{
				base.WriteCompressedUInt32((uint)arrayMarshalInfo.size);
			}
			if (arrayMarshalInfo.size_parameter_multiplier > -1)
			{
				base.WriteCompressedUInt32((uint)arrayMarshalInfo.size_parameter_multiplier);
			}
		}

		public void WriteMethodSignature(IMethodSignature method)
		{
			byte callingConvention = (byte)method.CallingConvention;
			if (method.HasThis)
			{
				callingConvention = (byte)(callingConvention | 32);
			}
			if (method.ExplicitThis)
			{
				callingConvention = (byte)(callingConvention | 64);
			}
			IGenericParameterProvider genericParameterProvider = method as IGenericParameterProvider;
			int num = (genericParameterProvider == null || !genericParameterProvider.HasGenericParameters ? 0 : genericParameterProvider.GenericParameters.Count);
			if (num > 0)
			{
				callingConvention = (byte)(callingConvention | 16);
			}
			int num1 = (method.HasParameters ? method.Parameters.Count : 0);
			base.WriteByte(callingConvention);
			if (num > 0)
			{
				base.WriteCompressedUInt32((uint)num);
			}
			base.WriteCompressedUInt32((uint)num1);
			this.WriteTypeSignature(method.ReturnType);
			if (num1 == 0)
			{
				return;
			}
			Collection<ParameterDefinition> parameters = method.Parameters;
			for (int i = 0; i < num1; i++)
			{
				this.WriteTypeSignature(parameters[i].ParameterType);
			}
		}

		private void WriteModifierSignature(ElementType element_type, IModifierType type)
		{
			this.WriteElementType(element_type);
			base.WriteCompressedUInt32(this.MakeTypeDefOrRefCodedRID(type.ModifierType));
			this.WriteTypeSignature(type.ElementType);
		}

		private void WriteNativeType(NativeType native)
		{
			base.WriteByte((byte)native);
		}

		private void WritePrimitiveValue(object value)
		{
			object obj;
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			switch (Type.GetTypeCode(value.GetType()))
			{
				case TypeCode.Boolean:
				{
					if ((bool)value)
					{
						obj = 1;
					}
					else
					{
						obj = null;
					}
					base.WriteByte((byte)obj);
					return;
				}
				case TypeCode.Char:
				{
					base.WriteInt16((short)((char)value));
					return;
				}
				case TypeCode.SByte:
				{
					base.WriteSByte((sbyte)value);
					return;
				}
				case TypeCode.Byte:
				{
					base.WriteByte((byte)value);
					return;
				}
				case TypeCode.Int16:
				{
					base.WriteInt16((short)value);
					return;
				}
				case TypeCode.UInt16:
				{
					base.WriteUInt16((ushort)value);
					return;
				}
				case TypeCode.Int32:
				{
					base.WriteInt32((int)value);
					return;
				}
				case TypeCode.UInt32:
				{
					base.WriteUInt32((uint)value);
					return;
				}
				case TypeCode.Int64:
				{
					base.WriteInt64((long)value);
					return;
				}
				case TypeCode.UInt64:
				{
					base.WriteUInt64((ulong)value);
					return;
				}
				case TypeCode.Single:
				{
					base.WriteSingle((float)value);
					return;
				}
				case TypeCode.Double:
				{
					base.WriteDouble((double)value);
					return;
				}
			}
			throw new NotSupportedException(value.GetType().FullName);
		}

		private void WriteSecurityAttribute(SecurityAttribute attribute)
		{
			this.WriteTypeReference(attribute.AttributeType);
			int namedArgumentCount = SignatureWriter.GetNamedArgumentCount(attribute);
			if (namedArgumentCount == 0)
			{
				base.WriteCompressedUInt32(1);
				base.WriteCompressedUInt32(0);
				return;
			}
			SignatureWriter signatureWriter = new SignatureWriter(this.metadata);
			signatureWriter.WriteCompressedUInt32((uint)namedArgumentCount);
			signatureWriter.WriteICustomAttributeNamedArguments(attribute);
			base.WriteCompressedUInt32((uint)signatureWriter.length);
			base.WriteBytes(signatureWriter);
		}

		public void WriteSecurityDeclaration(SecurityDeclaration declaration)
		{
			base.WriteByte(46);
			Collection<SecurityAttribute> securityAttributes = declaration.security_attributes;
			if (securityAttributes == null)
			{
				throw new NotSupportedException();
			}
			base.WriteCompressedUInt32((uint)securityAttributes.Count);
			for (int i = 0; i < securityAttributes.Count; i++)
			{
				this.WriteSecurityAttribute(securityAttributes[i]);
			}
		}

		private void WriteTypeReference(TypeReference type)
		{
			this.WriteUTF8String(TypeParser.ToParseable(type));
		}

		public void WriteTypeSignature(TypeReference type)
		{
			int position;
			TypeSpecification typeSpecification;
			GenericParameter genericParameter;
			if (type == null)
			{
				throw new ArgumentNullException();
			}
			ElementType elementType = type.etype;
			if (elementType > ElementType.GenericInst)
			{
				switch (elementType)
				{
					case ElementType.FnPtr:
					{
						FunctionPointerType functionPointerType = (FunctionPointerType)type;
						this.WriteElementType(ElementType.FnPtr);
						this.WriteMethodSignature(functionPointerType);
						return;
					}
					case ElementType.Object:
					case ElementType.SzArray:
					{
						break;
					}
					case ElementType.MVar:
					{
						genericParameter = (GenericParameter)type;
						this.WriteElementType(elementType);
						position = genericParameter.Position;
						if (position == -1)
						{
							throw new NotSupportedException();
						}
						base.WriteCompressedUInt32((uint)position);
						return;
					}
					case ElementType.CModReqD:
					case ElementType.CModOpt:
					{
						this.WriteModifierSignature(elementType, (IModifierType)type);
						return;
					}
					default:
					{
						if (elementType == ElementType.Sentinel || elementType == ElementType.Pinned)
						{
							typeSpecification = (TypeSpecification)type;
							this.WriteElementType(elementType);
							this.WriteTypeSignature(typeSpecification.ElementType);
							return;
						}
						break;
					}
				}
			}
			else
			{
				if (elementType == ElementType.None)
				{
					this.WriteElementType((type.IsValueType ? ElementType.ValueType : ElementType.Class));
					base.WriteCompressedUInt32(this.MakeTypeDefOrRefCodedRID(type));
					return;
				}
				switch (elementType)
				{
					case ElementType.Ptr:
					case ElementType.ByRef:
					{
						typeSpecification = (TypeSpecification)type;
						this.WriteElementType(elementType);
						this.WriteTypeSignature(typeSpecification.ElementType);
						return;
					}
					case ElementType.Var:
					{
						genericParameter = (GenericParameter)type;
						this.WriteElementType(elementType);
						position = genericParameter.Position;
						if (position == -1)
						{
							throw new NotSupportedException();
						}
						base.WriteCompressedUInt32((uint)position);
						return;
					}
					case ElementType.Array:
					{
						ArrayType arrayType = (ArrayType)type;
						if (!arrayType.IsVector)
						{
							this.WriteArrayTypeSignature(arrayType);
							return;
						}
						this.WriteElementType(ElementType.SzArray);
						this.WriteTypeSignature(arrayType.ElementType);
						return;
					}
					case ElementType.GenericInst:
					{
						GenericInstanceType genericInstanceType = (GenericInstanceType)type;
						this.WriteElementType(ElementType.GenericInst);
						this.WriteElementType((genericInstanceType.IsValueType ? ElementType.ValueType : ElementType.Class));
						base.WriteCompressedUInt32(this.MakeTypeDefOrRefCodedRID(genericInstanceType.ElementType));
						this.WriteGenericInstanceSignature(genericInstanceType);
						return;
					}
				}
			}
			if (!this.TryWriteElementType(type))
			{
				throw new NotSupportedException();
			}
		}

		public void WriteUTF8String(string @string)
		{
			if (@string == null)
			{
				base.WriteByte(255);
				return;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(@string);
			base.WriteCompressedUInt32((uint)bytes.Length);
			base.WriteBytes(bytes);
		}

		private void WriteVariantType(VariantType variant)
		{
			base.WriteByte((byte)variant);
		}

		public void WriteXmlSecurityDeclaration(SecurityDeclaration declaration)
		{
			string xmlSecurityDeclaration = SignatureWriter.GetXmlSecurityDeclaration(declaration);
			if (xmlSecurityDeclaration == null)
			{
				throw new NotSupportedException();
			}
			base.WriteBytes(Encoding.Unicode.GetBytes(xmlSecurityDeclaration));
		}
	}
}