using System;

namespace Mono.Cecil
{
	public interface IMetadataTokenProvider
	{
		Mono.Cecil.MetadataToken MetadataToken
		{
			get;
			set;
		}
	}
}