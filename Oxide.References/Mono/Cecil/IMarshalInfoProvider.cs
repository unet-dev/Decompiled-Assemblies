using System;

namespace Mono.Cecil
{
	public interface IMarshalInfoProvider : IMetadataTokenProvider
	{
		bool HasMarshalInfo
		{
			get;
		}

		Mono.Cecil.MarshalInfo MarshalInfo
		{
			get;
			set;
		}
	}
}