using System;

public abstract class SingletonComponent : FacepunchBehaviour
{
	protected SingletonComponent()
	{
	}

	protected virtual void Awake()
	{
		this.SingletonSetup();
	}

	protected virtual void OnDestroy()
	{
		this.SingletonClear();
	}

	public abstract void SingletonClear();

	public abstract void SingletonSetup();
}