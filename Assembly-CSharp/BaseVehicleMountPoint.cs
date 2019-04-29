using System;

public class BaseVehicleMountPoint : BaseMountable
{
	public BaseVehicleMountPoint()
	{
	}

	public override bool DirectlyMountable()
	{
		return false;
	}

	public BaseVehicle GetVehicleParent()
	{
		return base.GetParentEntity() as BaseVehicle;
	}

	public override float WaterFactorForPlayer(BasePlayer player)
	{
		BaseVehicle vehicleParent = this.GetVehicleParent();
		if (vehicleParent == null)
		{
			return 0f;
		}
		return vehicleParent.WaterFactorForPlayer(player);
	}
}