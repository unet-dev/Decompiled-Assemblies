using System;

namespace Apex.AI
{
	public interface IHaveFriendlyName
	{
		string description
		{
			get;
		}

		string friendlyName
		{
			get;
		}
	}
}