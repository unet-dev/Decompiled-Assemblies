using ConVar;
using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseBoat : BaseVehicle
{
	public float engineThrust = 10f;

	public float steeringScale = 0.1f;

	public float gasPedal;

	public float steering;

	public Rigidbody myRigidBody;

	public Transform thrustPoint;

	public Transform centerOfMass;

	public Buoyancy buoyancy;

	public GameObject clientCollider;

	public GameObject serverCollider;

	[ServerVar]
	public static bool generate_paths;

	static BaseBoat()
	{
		BaseBoat.generate_paths = true;
	}

	public BaseBoat()
	{
	}

	public virtual void DriverInput(InputState inputState, BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.gasPedal = 1f;
		}
		else if (!inputState.IsDown(BUTTON.BACKWARD))
		{
			this.gasPedal = 0f;
		}
		else
		{
			this.gasPedal = -0.5f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.steering = 1f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.steering = -1f;
			return;
		}
		this.steering = 0f;
	}

	public virtual bool EngineInWater()
	{
		return TerrainMeta.WaterMap.GetHeight(this.thrustPoint.position) > this.thrustPoint.position.y;
	}

	public virtual bool EngineOn()
	{
		if (!base.HasDriver())
		{
			return false;
		}
		return !this.IsFlipped();
	}

	public static List<Vector3> GenerateOceanPatrolPath(float minDistanceFromShore = 50f, float minWaterDepth = 8f)
	{
		RaycastHit raycastHit;
		object obj = Interface.CallHook("OnBoatPathGenerate");
		if (obj is List<Vector3>)
		{
			return (List<Vector3>)obj;
		}
		float size = TerrainMeta.Size.x;
		float single = 30f;
		int num = Mathf.CeilToInt(size * 2f * 3.14159274f / single);
		List<Vector3> vector3s = new List<Vector3>();
		float single1 = size;
		float single2 = 0f;
		for (int i = 0; i < num; i++)
		{
			float single3 = (float)i / (float)num * 360f;
			vector3s.Add(new Vector3(Mathf.Sin(single3 * 0.0174532924f) * single1, single2, Mathf.Cos(single3 * 0.0174532924f) * single1));
		}
		float single4 = 4f;
		float single5 = 200f;
		bool flag = true;
		for (int j = 0; j < AI.ocean_patrol_path_iterations & flag; j++)
		{
			flag = false;
			for (int k = 0; k < num; k++)
			{
				Vector3 item = vector3s[k];
				int num1 = (k == 0 ? num - 1 : k - 1);
				Vector3 vector3 = vector3s[(k == num - 1 ? 0 : k + 1)];
				Vector3 item1 = vector3s[num1];
				Vector3 vector31 = item;
				Vector3 vector32 = Vector3.zero - item;
				Vector3 vector33 = vector32.normalized;
				Vector3 vector34 = item + (vector33 * single4);
				if (Vector3.Distance(vector34, vector3) <= single5 && Vector3.Distance(vector34, item1) <= single5)
				{
					bool flag1 = true;
					int num2 = 16;
					int num3 = 0;
					while (num3 < num2)
					{
						float single6 = (float)num3 / (float)num2 * 360f;
						vector32 = new Vector3(Mathf.Sin(single6 * 0.0174532924f), single2, Mathf.Cos(single6 * 0.0174532924f));
						Vector3 vector35 = vector34 + (vector32.normalized * 1f);
						BaseBoat.GetWaterDepth(vector35);
						Vector3 vector36 = vector33;
						if (vector35 != Vector3.zero)
						{
							vector32 = vector35 - vector34;
							vector36 = vector32.normalized;
						}
						if (!UnityEngine.Physics.SphereCast(vector31, 3f, vector36, out raycastHit, minDistanceFromShore, 1218511105))
						{
							num3++;
						}
						else
						{
							flag1 = false;
							break;
						}
					}
					if (flag1)
					{
						flag = true;
						vector3s[k] = vector34;
					}
				}
			}
		}
		if (flag)
		{
			Debug.LogWarning("Failed to generate ocean patrol path");
			return null;
		}
		List<int> nums = new List<int>();
		LineUtility.Simplify(vector3s, 5f, nums);
		List<Vector3> vector3s1 = vector3s;
		vector3s = new List<Vector3>();
		foreach (int num4 in nums)
		{
			vector3s.Add(vector3s1[num4]);
		}
		Debug.Log(string.Concat("Generated ocean patrol path with node count: ", vector3s.Count));
		return vector3s;
	}

	public static float GetWaterDepth(Vector3 pos)
	{
		RaycastHit raycastHit;
		if (Application.isPlaying && !(TerrainMeta.WaterMap == null))
		{
			return TerrainMeta.WaterMap.GetDepth(pos);
		}
		if (!UnityEngine.Physics.Raycast(pos, Vector3.down, out raycastHit, 100f, 8388608))
		{
			return 100f;
		}
		return raycastHit.distance;
	}

	public bool InDryDock()
	{
		return base.GetParentEntity() != null;
	}

	public bool IsFlipped()
	{
		return Vector3.Dot(Vector3.up, base.transform.up) <= 0f;
	}

	public override float MaxVelocity()
	{
		return 25f;
	}

	public override bool PhysicsDriven()
	{
		return true;
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
		base.ServerInit();
		this.myRigidBody.isKinematic = false;
		if (this.myRigidBody == null)
		{
			Debug.LogWarning("Boat rigidbody null");
			return;
		}
		if (this.centerOfMass == null)
		{
			Debug.LogWarning("boat COM null");
			return;
		}
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
	}

	public override void VehicleFixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.EngineOn())
		{
			this.gasPedal = 0f;
			this.steering = 0f;
		}
		base.VehicleFixedUpdate();
		bool height = WaterSystem.GetHeight(this.thrustPoint.position) >= this.thrustPoint.position.y;
		if (this.gasPedal != 0f & height && this.buoyancy.submergedFraction > 0.3f)
		{
			Vector3 vector3 = base.transform.forward + ((base.transform.right * this.steering) * this.steeringScale);
			Vector3 vector31 = (vector3.normalized * this.gasPedal) * this.engineThrust;
			this.myRigidBody.AddForceAtPosition(vector31, this.thrustPoint.position, ForceMode.Force);
		}
	}

	public override float WaterFactorForPlayer(BasePlayer player)
	{
		if (TerrainMeta.WaterMap.GetHeight(player.eyes.position) >= player.eyes.position.y)
		{
			return 1f;
		}
		return 0f;
	}
}