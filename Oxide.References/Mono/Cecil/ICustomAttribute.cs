using Mono.Collections.Generic;
using System;

namespace Mono.Cecil
{
	public interface ICustomAttribute
	{
		TypeReference AttributeType
		{
			get;
		}

		Collection<CustomAttributeNamedArgument> Fields
		{
			get;
		}

		bool HasFields
		{
			get;
		}

		bool HasProperties
		{
			get;
		}

		Collection<CustomAttributeNamedArgument> Properties
		{
			get;
		}
	}
}