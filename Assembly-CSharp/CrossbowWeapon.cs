using System;

public class CrossbowWeapon : BaseProjectile
{
	public CrossbowWeapon()
	{
	}

	public override bool ForceSendMagazine()
	{
		return true;
	}
}