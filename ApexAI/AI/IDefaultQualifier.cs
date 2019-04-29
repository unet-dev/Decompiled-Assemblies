using System;

namespace Apex.AI
{
	public interface IDefaultQualifier : IQualifier, ICanBeDisabled
	{
		float score
		{
			get;
			set;
		}
	}
}