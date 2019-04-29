using System;

public abstract class ComponentInfo<T> : ComponentInfo
{
	public T component;

	protected ComponentInfo()
	{
	}

	public void Initialize(T source)
	{
		this.component = source;
		this.Setup();
	}
}