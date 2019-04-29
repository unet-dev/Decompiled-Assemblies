using System;

public class BaseVehicleSeat : BaseVehicleMountPoint
{
	public BaseVehicleSeat()
	{
	}

	public override float GetSteering(BasePlayer player)
	{
		return base.GetVehicleParent().GetSteering(player);
	}

	public override void LightToggle(BasePlayer player)
	{
		BaseVehicle vehicleParent = base.GetVehicleParent();
		if (vehicleParent == null)
		{
			return;
		}
		vehicleParent.LightToggle(player);
	}

	public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
		BaseVehicle vehicleParent = base.GetVehicleParent();
		if (vehicleParent == null)
		{
			return;
		}
		vehicleParent.MounteeTookDamage(mountee, info);
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		BaseVehicle vehicleParent = base.GetVehicleParent();
		if (vehicleParent != null)
		{
			vehicleParent.PlayerServerInput(inputState, player);
		}
		base.PlayerServerInput(inputState, player);
	}
}