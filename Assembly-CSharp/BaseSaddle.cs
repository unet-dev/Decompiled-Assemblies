using System;

public class BaseSaddle : BaseMountable
{
	public BaseRidableAnimal animal;

	public BaseSaddle()
	{
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (player != this._mounted)
		{
			return;
		}
		if (this.animal)
		{
			this.animal.RiderInput(inputState, player);
		}
	}

	public void SetAnimal(BaseRidableAnimal newAnimal)
	{
		this.animal = newAnimal;
	}
}