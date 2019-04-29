namespace Apex.AI
{
	public interface IConnectorAction : IAction
	{
		IAction Select(IAIContext context);
	}
}