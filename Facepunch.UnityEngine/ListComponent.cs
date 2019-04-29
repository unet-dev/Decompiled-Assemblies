using System;

public abstract class ListComponent : FacepunchBehaviour
{
	protected ListComponent()
	{
	}

	public abstract void Clear();

	protected virtual void OnDisable()
	{
		this.Clear();
	}

	protected virtual void OnEnable()
	{
		this.Setup();
	}

	public abstract void Setup();
}