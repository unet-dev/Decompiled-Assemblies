using System;
using UnityEngine;

public class BaseWheeledVehicle : BaseVehicle
{
	[Header("Wheels")]
	public BaseWheeledVehicle.VehicleWheel[] wheels;

	public BaseWheeledVehicle()
	{
	}

	[Serializable]
	public class VehicleWheel
	{
		public Transform shock;

		public WheelCollider wheelCollider;

		public Transform wheel;

		public Transform axle;

		public bool steerWheel;

		public bool brakeWheel;

		public bool powerWheel;

		public VehicleWheel()
		{
		}
	}
}