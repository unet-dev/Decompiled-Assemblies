using System;

namespace Mono.Cecil
{
	public interface IConstantProvider : IMetadataTokenProvider
	{
		object Constant
		{
			get;
			set;
		}

		bool HasConstant
		{
			get;
			set;
		}
	}
}