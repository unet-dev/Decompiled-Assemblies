using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Rocks
{
	public static class TypeReferenceRocks
	{
		public static ArrayType MakeArrayType(this TypeReference self)
		{
			return new ArrayType(self);
		}

		public static ArrayType MakeArrayType(this TypeReference self, int rank)
		{
			if (rank == 0)
			{
				throw new ArgumentOutOfRangeException("rank");
			}
			ArrayType arrayType = new ArrayType(self);
			for (int i = 1; i < rank; i++)
			{
				arrayType.Dimensions.Add(new ArrayDimension());
			}
			return arrayType;
		}

		public static ByReferenceType MakeByReferenceType(this TypeReference self)
		{
			return new ByReferenceType(self);
		}

		public static GenericInstanceType MakeGenericInstanceType(this TypeReference self, params TypeReference[] arguments)
		{
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}
			if (arguments.Length == 0)
			{
				throw new ArgumentException();
			}
			if (self.GenericParameters.Count != (int)arguments.Length)
			{
				throw new ArgumentException();
			}
			GenericInstanceType genericInstanceType = new GenericInstanceType(self);
			TypeReference[] typeReferenceArray = arguments;
			for (int i = 0; i < (int)typeReferenceArray.Length; i++)
			{
				TypeReference typeReference = typeReferenceArray[i];
				genericInstanceType.GenericArguments.Add(typeReference);
			}
			return genericInstanceType;
		}

		public static OptionalModifierType MakeOptionalModifierType(this TypeReference self, TypeReference modifierType)
		{
			return new OptionalModifierType(modifierType, self);
		}

		public static PinnedType MakePinnedType(this TypeReference self)
		{
			return new PinnedType(self);
		}

		public static PointerType MakePointerType(this TypeReference self)
		{
			return new PointerType(self);
		}

		public static RequiredModifierType MakeRequiredModifierType(this TypeReference self, TypeReference modifierType)
		{
			return new RequiredModifierType(modifierType, self);
		}

		public static SentinelType MakeSentinelType(this TypeReference self)
		{
			return new SentinelType(self);
		}
	}
}