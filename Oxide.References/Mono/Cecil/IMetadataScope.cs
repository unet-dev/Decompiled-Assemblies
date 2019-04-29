using System;

namespace Mono.Cecil
{
	public interface IMetadataScope : IMetadataTokenProvider
	{
		Mono.Cecil.MetadataScopeType MetadataScopeType
		{
			get;
		}

		string Name
		{
			get;
			set;
		}
	}
}