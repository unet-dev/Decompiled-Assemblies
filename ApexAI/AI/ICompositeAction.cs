using System;

namespace Apex.AI
{
	public interface ICompositeAction : IConnectorAction, IAction
	{
		bool isConnector
		{
			get;
		}
	}
}