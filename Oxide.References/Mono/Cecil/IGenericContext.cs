using System;

namespace Mono.Cecil
{
	internal interface IGenericContext
	{
		bool IsDefinition
		{
			get;
		}

		IGenericParameterProvider Method
		{
			get;
		}

		IGenericParameterProvider Type
		{
			get;
		}
	}
}