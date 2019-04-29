namespace Mono.Cecil
{
	public interface IModifierType
	{
		TypeReference ElementType
		{
			get;
		}

		TypeReference ModifierType
		{
			get;
		}
	}
}