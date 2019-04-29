using System;

public class BaseLadder : BaseCombatEntity
{
	public BaseLadder()
	{
	}

	public override bool ShouldBlockProjectiles()
	{
		return false;
	}
}