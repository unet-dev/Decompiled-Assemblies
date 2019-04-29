using System;

public abstract class Definition<T> : BaseScriptableObject
where T : Definition<T>
{
	protected Definition()
	{
	}
}