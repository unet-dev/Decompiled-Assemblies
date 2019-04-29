using Mono.Collections.Generic;
using System;

namespace Mono.Cecil
{
	public interface IGenericInstance : IMetadataTokenProvider
	{
		Collection<TypeReference> GenericArguments
		{
			get;
		}

		bool HasGenericArguments
		{
			get;
		}
	}
}