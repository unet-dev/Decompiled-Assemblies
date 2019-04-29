using Mono.Collections.Generic;
using System;

namespace Mono.Cecil
{
	public interface IGenericParameterProvider : IMetadataTokenProvider
	{
		Collection<GenericParameter> GenericParameters
		{
			get;
		}

		Mono.Cecil.GenericParameterType GenericParameterType
		{
			get;
		}

		bool HasGenericParameters
		{
			get;
		}

		bool IsDefinition
		{
			get;
		}

		ModuleDefinition Module
		{
			get;
		}
	}
}