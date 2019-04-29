using System;

public class FlintStrikeWeapon : BaseProjectile
{
	public float successFraction = 0.5f;

	public RecoilProperties strikeRecoil;

	public FlintStrikeWeapon()
	{
	}

	public override RecoilProperties GetRecoil()
	{
		return this.strikeRecoil;
	}
}