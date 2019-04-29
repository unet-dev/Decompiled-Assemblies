using Mono.Collections.Generic;
using System;

namespace Mono.Cecil
{
	public interface IMethodSignature : IMetadataTokenProvider
	{
		MethodCallingConvention CallingConvention
		{
			get;
			set;
		}

		bool ExplicitThis
		{
			get;
			set;
		}

		bool HasParameters
		{
			get;
		}

		bool HasThis
		{
			get;
			set;
		}

		Mono.Cecil.MethodReturnType MethodReturnType
		{
			get;
		}

		Collection<ParameterDefinition> Parameters
		{
			get;
		}

		TypeReference ReturnType
		{
			get;
			set;
		}
	}
}