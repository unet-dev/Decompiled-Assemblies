using System;

public class BaseChair : BaseMountable
{
	public BaseChair()
	{
	}

	public override float GetComfort()
	{
		return 1f;
	}
}