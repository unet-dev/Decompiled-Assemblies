using System;
using UnityEngine;

public class BaseCar : BaseWheeledVehicle
{
	public float brakePedal;

	public float gasPedal;

	public float steering;

	public Transform centerOfMass;

	public Transform steeringWheel;

	public float motorForceConstant = 150f;

	public float brakeForceConstant = 500f;

	public float GasLerpTime = 20f;

	public float SteeringLerpTime = 20f;

	public Transform driverEye;

	public Rigidbody myRigidBody;

	private static bool chairtest;

	public GameObjectRef chairRef;

	public Transform chairAnchorTest;

	private float throttle;

	private float brake;

	private bool lightsOn = true;

	static BaseCar()
	{
	}

	public BaseCar()
	{
	}

	private void ApplyForceAtWheels()
	{
		if (this.myRigidBody == null)
		{
			return;
		}
		Vector3 vector3 = this.myRigidBody.velocity;
		float single = vector3.magnitude * Vector3.Dot(vector3.normalized, base.transform.forward);
		float single1 = this.brakePedal;
		float single2 = this.gasPedal;
		if (single > 0f && single2 < 0f)
		{
			single1 = 100f;
		}
		else if (single < 0f && single2 > 0f)
		{
			single1 = 100f;
		}
		BaseWheeledVehicle.VehicleWheel[] vehicleWheelArray = this.wheels;
		for (int i = 0; i < (int)vehicleWheelArray.Length; i++)
		{
			BaseWheeledVehicle.VehicleWheel vehicleWheel = vehicleWheelArray[i];
			if (vehicleWheel.wheelCollider.isGrounded)
			{
				if (vehicleWheel.powerWheel)
				{
					vehicleWheel.wheelCollider.motorTorque = single2 * this.motorForceConstant;
				}
				if (vehicleWheel.brakeWheel)
				{
					vehicleWheel.wheelCollider.brakeTorque = single1 * this.brakeForceConstant;
				}
			}
		}
		base.SetFlag(BaseEntity.Flags.Reserved3, (single1 < 100f ? false : this.IsMounted()), false, true);
	}

	public void ConvertInputToThrottle()
	{
	}

	private void DoSteering()
	{
		BaseWheeledVehicle.VehicleWheel[] vehicleWheelArray = this.wheels;
		for (int i = 0; i < (int)vehicleWheelArray.Length; i++)
		{
			BaseWheeledVehicle.VehicleWheel vehicleWheel = vehicleWheelArray[i];
			if (vehicleWheel.steerWheel)
			{
				vehicleWheel.wheelCollider.steerAngle = this.steering;
			}
		}
		base.SetFlag(BaseEntity.Flags.Reserved4, this.steering < -2f, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved5, this.steering > 2f, false, true);
	}

	public void DriverInput(InputState inputState, BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.gasPedal = 100f;
			this.brakePedal = 0f;
		}
		else if (!inputState.IsDown(BUTTON.BACKWARD))
		{
			this.gasPedal = 0f;
			this.brakePedal = 30f;
		}
		else
		{
			this.gasPedal = -30f;
			this.brakePedal = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.steering = -60f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.steering = 60f;
			return;
		}
		this.steering = 0f;
	}

	public override Vector3 EyePositionForPlayer(BasePlayer player)
	{
		if (player.GetMounted() != this)
		{
			return Vector3.zero;
		}
		return this.driverEye.transform.position;
	}

	public new void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.isClient)
		{
			return;
		}
		if (!base.HasDriver())
		{
			this.NoDriverInput();
		}
		this.ConvertInputToThrottle();
		this.DoSteering();
		this.ApplyForceAtWheels();
		base.SetFlag(BaseEntity.Flags.Reserved1, this.IsMounted(), false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, (!this.IsMounted() ? false : this.lightsOn), false, true);
	}

	public override float GetComfort()
	{
		return 0f;
	}

	public override void LightToggle(BasePlayer player)
	{
		if (base.GetPlayerSeat(player) == 0)
		{
			this.lightsOn = !this.lightsOn;
		}
	}

	public override float MaxVelocity()
	{
		return 50f;
	}

	public void NoDriverInput()
	{
		if (BaseCar.chairtest)
		{
			this.gasPedal = Mathf.Sin(Time.time) * 50f;
			return;
		}
		this.gasPedal = 0f;
		this.brakePedal = Mathf.Lerp(this.brakePedal, 100f, Time.deltaTime * this.GasLerpTime / 5f);
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (base.GetPlayerSeat(player) == 0)
		{
			this.DriverInput(inputState, player);
		}
	}

	public override void ServerInit()
	{
		if (base.isClient)
		{
			return;
		}
		base.ServerInit();
		this.myRigidBody = base.GetComponent<Rigidbody>();
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.myRigidBody.isKinematic = false;
		if (BaseCar.chairtest)
		{
			this.SpawnChairTest();
		}
	}

	public void SpawnChairTest()
	{
		GameManager gameManager = GameManager.server;
		string str = this.chairRef.resourcePath;
		Vector3 vector3 = this.chairAnchorTest.transform.localPosition;
		Quaternion quaternion = new Quaternion();
		BaseEntity baseEntity = gameManager.CreateEntity(str, vector3, quaternion, true);
		baseEntity.Spawn();
		DestroyOnGroundMissing component = baseEntity.GetComponent<DestroyOnGroundMissing>();
		if (component != null)
		{
			component.enabled = false;
		}
		MeshCollider meshCollider = baseEntity.GetComponent<MeshCollider>();
		if (meshCollider)
		{
			meshCollider.convex = true;
		}
		baseEntity.SetParent(this, false, false);
	}
}