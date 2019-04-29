using Mono;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;
using System;
using System.Text;

namespace Mono.Cecil
{
	internal sealed class SignatureReader : ByteBuffer
	{
		private readonly MetadataReader reader;

		private readonly uint start;

		private readonly uint sig_length;

		private Mono.Cecil.TypeSystem TypeSystem
		{
			get
			{
				return this.reader.module.TypeSystem;
			}
		}

		public SignatureReader(uint blob, MetadataReader reader) : base(reader.buffer)
		{
			this.reader = reader;
			this.MoveToBlob(blob);
			this.sig_length = base.ReadCompressedUInt32();
			this.start = (uint)this.position;
		}

		public bool CanReadMore()
		{
			return (long)this.position - (ulong)this.start < (ulong)this.sig_length;
		}

		private static void CheckGenericContext(IGenericParameterProvider owner, int index)
		{
			Collection<GenericParameter> genericParameters = owner.GenericParameters;
			for (int i = genericParameters.Count; i <= index; i++)
			{
				genericParameters.Add(new GenericParameter(owner));
			}
		}

		private static Collection<CustomAttributeNamedArgument> GetCustomAttributeNamedArgumentCollection(ref Collection<CustomAttributeNamedArgument> collection)
		{
			if (collection != null)
			{
				return collection;
			}
			Collection<CustomAttributeNamedArgument> customAttributeNamedArguments = new Collection<CustomAttributeNamedArgument>();
			Collection<CustomAttributeNamedArgument> customAttributeNamedArguments1 = customAttributeNamedArguments;
			collection = customAttributeNamedArguments;
			return customAttributeNamedArguments1;
		}

		private GenericParameter GetGenericParameter(GenericParameterType type, uint var)
		{
			IGenericParameterProvider method;
			IGenericContext genericContext = this.reader.context;
			int num = (int)var;
			if (genericContext == null)
			{
				return this.GetUnboundGenericParameter(type, num);
			}
			if (type == GenericParameterType.Type)
			{
				method = genericContext.Type;
			}
			else
			{
				if (type != GenericParameterType.Method)
				{
					throw new NotSupportedException();
				}
				method = genericContext.Method;
			}
			if (!genericContext.IsDefinition)
			{
				SignatureReader.CheckGenericContext(method, num);
			}
			if (num >= method.GenericParameters.Count)
			{
				return this.GetUnboundGenericParameter(type, num);
			}
			return method.GenericParameters[num];
		}

		private TypeReference GetPrimitiveType(ElementType etype)
		{
			switch (etype)
			{
				case ElementType.Boolean:
				{
					return this.TypeSystem.Boolean;
				}
				case ElementType.Char:
				{
					return this.TypeSystem.Char;
				}
				case ElementType.I1:
				{
					return this.TypeSystem.SByte;
				}
				case ElementType.U1:
				{
					return this.TypeSystem.Byte;
				}
				case ElementType.I2:
				{
					return this.TypeSystem.Int16;
				}
				case ElementType.U2:
				{
					return this.TypeSystem.UInt16;
				}
				case ElementType.I4:
				{
					return this.TypeSystem.Int32;
				}
				case ElementType.U4:
				{
					return this.TypeSystem.UInt32;
				}
				case ElementType.I8:
				{
					return this.TypeSystem.Int64;
				}
				case ElementType.U8:
				{
					return this.TypeSystem.UInt64;
				}
				case ElementType.R4:
				{
					return this.TypeSystem.Single;
				}
				case ElementType.R8:
				{
					return this.TypeSystem.Double;
				}
				case ElementType.String:
				{
					return this.TypeSystem.String;
				}
			}
			throw new NotImplementedException(etype.ToString());
		}

		private TypeReference GetTypeDefOrRef(MetadataToken token)
		{
			return this.reader.GetTypeDefOrRef(token);
		}

		private GenericParameter GetUnboundGenericParameter(GenericParameterType type, int index)
		{
			return new GenericParameter(index, type, this.reader.module);
		}

		private void MoveToBlob(uint blob)
		{
			this.position = (int)(this.reader.image.BlobHeap.Offset + blob);
		}

		private ArrayType ReadArrayTypeSignature()
		{
			int? nullable;
			ArrayType arrayType = new ArrayType(this.ReadTypeSignature());
			uint num = base.ReadCompressedUInt32();
			uint[] numArray = new uint[base.ReadCompressedUInt32()];
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				numArray[i] = base.ReadCompressedUInt32();
			}
			int[] numArray1 = new int[base.ReadCompressedUInt32()];
			for (int j = 0; j < (int)numArray1.Length; j++)
			{
				numArray1[j] = base.ReadCompressedInt32();
			}
			arrayType.Dimensions.Clear();
			for (int k = 0; (long)k < (ulong)num; k++)
			{
				int? nullable1 = null;
				int? nullable2 = null;
				if (k < (int)numArray1.Length)
				{
					nullable1 = new int?(numArray1[k]);
				}
				if (k < (int)numArray.Length)
				{
					int? nullable3 = nullable1;
					int num1 = (int)numArray[k];
					if (nullable3.HasValue)
					{
						nullable = new int?(nullable3.GetValueOrDefault() + num1 - 1);
					}
					else
					{
						nullable = null;
					}
					nullable2 = nullable;
				}
				arrayType.Dimensions.Add(new ArrayDimension(nullable1, nullable2));
			}
			return arrayType;
		}

		public object ReadConstantSignature(ElementType type)
		{
			return this.ReadPrimitiveValue(type);
		}

		public void ReadCustomAttributeConstructorArguments(CustomAttribute attribute, Collection<ParameterDefinition> parameters)
		{
			int count = parameters.Count;
			if (count == 0)
			{
				return;
			}
			attribute.arguments = new Collection<CustomAttributeArgument>(count);
			for (int i = 0; i < count; i++)
			{
				attribute.arguments.Add(this.ReadCustomAttributeFixedArgument(parameters[i].ParameterType));
			}
		}

		private CustomAttributeArgument ReadCustomAttributeElement(TypeReference type)
		{
			if (type.IsArray)
			{
				return this.ReadCustomAttributeFixedArrayArgument((ArrayType)type);
			}
			return new CustomAttributeArgument(type, (type.etype == ElementType.Object ? this.ReadCustomAttributeElement(this.ReadCustomAttributeFieldOrPropType()) : this.ReadCustomAttributeElementValue(type)));
		}

		private object ReadCustomAttributeElementValue(TypeReference type)
		{
			ElementType elementType = type.etype;
			if (elementType != ElementType.None)
			{
				if (elementType == ElementType.String)
				{
					return this.ReadUTF8String();
				}
				return this.ReadPrimitiveValue(elementType);
			}
			if (type.IsTypeOf("System", "Type"))
			{
				return this.ReadTypeReference();
			}
			return this.ReadCustomAttributeEnum(type);
		}

		private object ReadCustomAttributeEnum(TypeReference enum_type)
		{
			TypeDefinition typeDefinition = enum_type.CheckedResolve();
			if (!typeDefinition.IsEnum)
			{
				throw new ArgumentException();
			}
			return this.ReadCustomAttributeElementValue(typeDefinition.GetEnumUnderlyingType());
		}

		private TypeReference ReadCustomAttributeFieldOrPropType()
		{
			ElementType elementType = (ElementType)base.ReadByte();
			if (elementType > ElementType.Type)
			{
				if (elementType == ElementType.Boxed)
				{
					return this.TypeSystem.Object;
				}
				if (elementType == ElementType.Enum)
				{
					return this.ReadTypeReference();
				}
			}
			else
			{
				if (elementType == ElementType.SzArray)
				{
					return new ArrayType(this.ReadCustomAttributeFieldOrPropType());
				}
				if (elementType == ElementType.Type)
				{
					return this.TypeSystem.LookupType("System", "Type");
				}
			}
			return this.GetPrimitiveType(elementType);
		}

		private CustomAttributeArgument ReadCustomAttributeFixedArgument(TypeReference type)
		{
			if (!type.IsArray)
			{
				return this.ReadCustomAttributeElement(type);
			}
			return this.ReadCustomAttributeFixedArrayArgument((ArrayType)type);
		}

		private CustomAttributeArgument ReadCustomAttributeFixedArrayArgument(ArrayType type)
		{
			uint num = base.ReadUInt32();
			if (num == -1)
			{
				return new CustomAttributeArgument(type, null);
			}
			if (num == 0)
			{
				return new CustomAttributeArgument(type, Empty<CustomAttributeArgument>.Array);
			}
			CustomAttributeArgument[] customAttributeArgumentArray = new CustomAttributeArgument[num];
			TypeReference elementType = type.ElementType;
			for (int i = 0; (long)i < (ulong)num; i++)
			{
				customAttributeArgumentArray[i] = this.ReadCustomAttributeElement(elementType);
			}
			return new CustomAttributeArgument(type, customAttributeArgumentArray);
		}

		private void ReadCustomAttributeNamedArgument(ref Collection<CustomAttributeNamedArgument> fields, ref Collection<CustomAttributeNamedArgument> properties)
		{
			Collection<CustomAttributeNamedArgument> customAttributeNamedArgumentCollection;
			byte num = base.ReadByte();
			TypeReference typeReference = this.ReadCustomAttributeFieldOrPropType();
			string str = this.ReadUTF8String();
			if (num == 83)
			{
				customAttributeNamedArgumentCollection = SignatureReader.GetCustomAttributeNamedArgumentCollection(ref fields);
			}
			else
			{
				if (num != 84)
				{
					throw new NotSupportedException();
				}
				customAttributeNamedArgumentCollection = SignatureReader.GetCustomAttributeNamedArgumentCollection(ref properties);
			}
			customAttributeNamedArgumentCollection.Add(new CustomAttributeNamedArgument(str, this.ReadCustomAttributeFixedArgument(typeReference)));
		}

		public void ReadCustomAttributeNamedArguments(ushort count, ref Collection<CustomAttributeNamedArgument> fields, ref Collection<CustomAttributeNamedArgument> properties)
		{
			for (int i = 0; i < count; i++)
			{
				this.ReadCustomAttributeNamedArgument(ref fields, ref properties);
			}
		}

		public void ReadGenericInstanceSignature(IGenericParameterProvider provider, IGenericInstance instance)
		{
			uint num = base.ReadCompressedUInt32();
			if (!provider.IsDefinition)
			{
				SignatureReader.CheckGenericContext(provider, (int)(num - 1));
			}
			Collection<TypeReference> genericArguments = instance.GenericArguments;
			for (int i = 0; (long)i < (ulong)num; i++)
			{
				genericArguments.Add(this.ReadTypeSignature());
			}
		}

		public MarshalInfo ReadMarshalInfo()
		{
			NativeType nativeType = this.ReadNativeType();
			if (nativeType > NativeType.SafeArray)
			{
				if (nativeType == NativeType.FixedArray)
				{
					FixedArrayMarshalInfo fixedArrayMarshalInfo = new FixedArrayMarshalInfo();
					if (this.CanReadMore())
					{
						fixedArrayMarshalInfo.size = (int)base.ReadCompressedUInt32();
					}
					if (this.CanReadMore())
					{
						fixedArrayMarshalInfo.element_type = this.ReadNativeType();
					}
					return fixedArrayMarshalInfo;
				}
				if (nativeType == NativeType.Array)
				{
					ArrayMarshalInfo arrayMarshalInfo = new ArrayMarshalInfo();
					if (this.CanReadMore())
					{
						arrayMarshalInfo.element_type = this.ReadNativeType();
					}
					if (this.CanReadMore())
					{
						arrayMarshalInfo.size_parameter_index = (int)base.ReadCompressedUInt32();
					}
					if (this.CanReadMore())
					{
						arrayMarshalInfo.size = (int)base.ReadCompressedUInt32();
					}
					if (this.CanReadMore())
					{
						arrayMarshalInfo.size_parameter_multiplier = (int)base.ReadCompressedUInt32();
					}
					return arrayMarshalInfo;
				}
				if (nativeType == NativeType.CustomMarshaler)
				{
					CustomMarshalInfo customMarshalInfo = new CustomMarshalInfo();
					string str = this.ReadUTF8String();
					customMarshalInfo.guid = (!string.IsNullOrEmpty(str) ? new Guid(str) : Guid.Empty);
					customMarshalInfo.unmanaged_type = this.ReadUTF8String();
					customMarshalInfo.managed_type = this.ReadTypeReference();
					customMarshalInfo.cookie = this.ReadUTF8String();
					return customMarshalInfo;
				}
			}
			else
			{
				if (nativeType == NativeType.FixedSysString)
				{
					FixedSysStringMarshalInfo fixedSysStringMarshalInfo = new FixedSysStringMarshalInfo();
					if (this.CanReadMore())
					{
						fixedSysStringMarshalInfo.size = (int)base.ReadCompressedUInt32();
					}
					return fixedSysStringMarshalInfo;
				}
				if (nativeType == NativeType.SafeArray)
				{
					SafeArrayMarshalInfo safeArrayMarshalInfo = new SafeArrayMarshalInfo();
					if (this.CanReadMore())
					{
						safeArrayMarshalInfo.element_type = this.ReadVariantType();
					}
					return safeArrayMarshalInfo;
				}
			}
			return new MarshalInfo(nativeType);
		}

		public void ReadMethodSignature(IMethodSignature method)
		{
			Collection<ParameterDefinition> parameters;
			byte num = base.ReadByte();
			if ((num & 32) != 0)
			{
				method.HasThis = true;
				num = (byte)(num & -33);
			}
			if ((num & 64) != 0)
			{
				method.ExplicitThis = true;
				num = (byte)(num & -65);
			}
			method.CallingConvention = (MethodCallingConvention)num;
			MethodReference methodReference = method as MethodReference;
			if (methodReference != null && !methodReference.DeclaringType.IsArray)
			{
				this.reader.context = methodReference;
			}
			if ((num & 16) != 0)
			{
				uint num1 = base.ReadCompressedUInt32();
				if (methodReference != null && !methodReference.IsDefinition)
				{
					SignatureReader.CheckGenericContext(methodReference, (int)(num1 - 1));
				}
			}
			uint num2 = base.ReadCompressedUInt32();
			method.MethodReturnType.ReturnType = this.ReadTypeSignature();
			if (num2 == 0)
			{
				return;
			}
			MethodReference methodReference1 = method as MethodReference;
			if (methodReference1 == null)
			{
				parameters = method.Parameters;
			}
			else
			{
				ParameterDefinitionCollection parameterDefinitionCollection = new ParameterDefinitionCollection(method, (int)num2);
				ParameterDefinitionCollection parameterDefinitionCollection1 = parameterDefinitionCollection;
				methodReference1.parameters = parameterDefinitionCollection;
				parameters = parameterDefinitionCollection1;
			}
			for (int i = 0; (long)i < (ulong)num2; i++)
			{
				parameters.Add(new ParameterDefinition(this.ReadTypeSignature()));
			}
		}

		private NativeType ReadNativeType()
		{
			return (NativeType)base.ReadByte();
		}

		private object ReadPrimitiveValue(ElementType type)
		{
			switch (type)
			{
				case ElementType.Boolean:
				{
					return base.ReadByte() == 1;
				}
				case ElementType.Char:
				{
					return (char)base.ReadUInt16();
				}
				case ElementType.I1:
				{
					return (sbyte)base.ReadByte();
				}
				case ElementType.U1:
				{
					return base.ReadByte();
				}
				case ElementType.I2:
				{
					return base.ReadInt16();
				}
				case ElementType.U2:
				{
					return base.ReadUInt16();
				}
				case ElementType.I4:
				{
					return base.ReadInt32();
				}
				case ElementType.U4:
				{
					return base.ReadUInt32();
				}
				case ElementType.I8:
				{
					return base.ReadInt64();
				}
				case ElementType.U8:
				{
					return base.ReadUInt64();
				}
				case ElementType.R4:
				{
					return base.ReadSingle();
				}
				case ElementType.R8:
				{
					return base.ReadDouble();
				}
			}
			throw new NotImplementedException(type.ToString());
		}

		public SecurityAttribute ReadSecurityAttribute()
		{
			SecurityAttribute securityAttribute = new SecurityAttribute(this.ReadTypeReference());
			base.ReadCompressedUInt32();
			this.ReadCustomAttributeNamedArguments((ushort)base.ReadCompressedUInt32(), ref securityAttribute.fields, ref securityAttribute.properties);
			return securityAttribute;
		}

		public TypeReference ReadTypeReference()
		{
			return TypeParser.ParseType(this.reader.module, this.ReadUTF8String());
		}

		public TypeReference ReadTypeSignature()
		{
			return this.ReadTypeSignature((ElementType)base.ReadByte());
		}

		private TypeReference ReadTypeSignature(ElementType etype)
		{
			switch (etype)
			{
				case ElementType.Void:
				{
					return this.TypeSystem.Void;
				}
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
				case ElementType.String:
				case ElementType.Void | ElementType.Boolean | ElementType.Char | ElementType.I1 | ElementType.U1 | ElementType.I2 | ElementType.U2 | ElementType.ByRef | ElementType.ValueType | ElementType.Class | ElementType.Var | ElementType.Array | ElementType.GenericInst | ElementType.TypedByRef:
				case ElementType.Boolean | ElementType.I4 | ElementType.I8 | ElementType.ByRef | ElementType.Class | ElementType.I:
				{
					return this.GetPrimitiveType(etype);
				}
				case ElementType.Ptr:
				{
					return new PointerType(this.ReadTypeSignature());
				}
				case ElementType.ByRef:
				{
					return new ByReferenceType(this.ReadTypeSignature());
				}
				case ElementType.ValueType:
				{
					TypeReference typeDefOrRef = this.GetTypeDefOrRef(this.ReadTypeTokenSignature());
					typeDefOrRef.IsValueType = true;
					return typeDefOrRef;
				}
				case ElementType.Class:
				{
					return this.GetTypeDefOrRef(this.ReadTypeTokenSignature());
				}
				case ElementType.Var:
				{
					return this.GetGenericParameter(GenericParameterType.Type, base.ReadCompressedUInt32());
				}
				case ElementType.Array:
				{
					return this.ReadArrayTypeSignature();
				}
				case ElementType.GenericInst:
				{
					TypeReference typeReference = this.GetTypeDefOrRef(this.ReadTypeTokenSignature());
					GenericInstanceType genericInstanceType = new GenericInstanceType(typeReference);
					this.ReadGenericInstanceSignature(typeReference, genericInstanceType);
					if (base.ReadByte() == 17)
					{
						genericInstanceType.IsValueType = true;
						typeReference.GetElementType().IsValueType = true;
					}
					return genericInstanceType;
				}
				case ElementType.TypedByRef:
				{
					return this.TypeSystem.TypedReference;
				}
				case ElementType.I:
				{
					return this.TypeSystem.IntPtr;
				}
				case ElementType.U:
				{
					return this.TypeSystem.UIntPtr;
				}
				case ElementType.FnPtr:
				{
					FunctionPointerType functionPointerType = new FunctionPointerType();
					this.ReadMethodSignature(functionPointerType);
					return functionPointerType;
				}
				case ElementType.Object:
				{
					return this.TypeSystem.Object;
				}
				case ElementType.SzArray:
				{
					return new ArrayType(this.ReadTypeSignature());
				}
				case ElementType.MVar:
				{
					return this.GetGenericParameter(GenericParameterType.Method, base.ReadCompressedUInt32());
				}
				case ElementType.CModReqD:
				{
					return new RequiredModifierType(this.GetTypeDefOrRef(this.ReadTypeTokenSignature()), this.ReadTypeSignature());
				}
				case ElementType.CModOpt:
				{
					return new OptionalModifierType(this.GetTypeDefOrRef(this.ReadTypeTokenSignature()), this.ReadTypeSignature());
				}
				default:
				{
					if (etype == ElementType.Sentinel)
					{
						return new SentinelType(this.ReadTypeSignature());
					}
					if (etype == ElementType.Pinned)
					{
						return new PinnedType(this.ReadTypeSignature());
					}
					return this.GetPrimitiveType(etype);
				}
			}
		}

		private MetadataToken ReadTypeTokenSignature()
		{
			return CodedIndex.TypeDefOrRef.GetMetadataToken(base.ReadCompressedUInt32());
		}

		private string ReadUTF8String()
		{
			int num;
			if (this.buffer[this.position] == 255)
			{
				this.position++;
				return null;
			}
			int num1 = (int)base.ReadCompressedUInt32();
			if (num1 == 0)
			{
				return string.Empty;
			}
			Encoding uTF8 = Encoding.UTF8;
			byte[] numArray = this.buffer;
			int num2 = this.position;
			num = (this.buffer[this.position + num1 - 1] == 0 ? num1 - 1 : num1);
			this.position += num1;
			return uTF8.GetString(numArray, num2, num);
		}

		private VariantType ReadVariantType()
		{
			return (VariantType)base.ReadByte();
		}
	}
}