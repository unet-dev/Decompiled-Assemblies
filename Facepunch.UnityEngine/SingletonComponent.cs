using System;

public abstract class SingletonComponent : FacepunchBehaviour
{
	protected SingletonComponent()
	{
	}

	protected virtual void Awake()
	{
		this.Setup();
	}

	public abstract void Clear();

	protected virtual void OnDestroy()
	{
		this.Clear();
	}

	public abstract void Setup();
}