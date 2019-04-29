using System;

namespace Mono.Cecil
{
	public interface IMemberDefinition : ICustomAttributeProvider, IMetadataTokenProvider
	{
		TypeDefinition DeclaringType
		{
			get;
			set;
		}

		string FullName
		{
			get;
		}

		bool IsRuntimeSpecialName
		{
			get;
			set;
		}

		bool IsSpecialName
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}
	}
}