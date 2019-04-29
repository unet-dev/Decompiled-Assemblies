using System;
using UnityEngine;

public class Buoyancy : BaseMonoBehaviour
{
	public BuoyancyPoint[] points;

	public GameObjectRef[] waterImpacts;

	public Rigidbody rigidBody;

	public float buoyancyScale = 1f;

	public float submergedFraction;

	public bool doEffects = true;

	public bool clientSide;

	public Action<bool> SubmergedChanged;

	private float timeOutOfWater;

	private float timeInWater;

	private Buoyancy.BuoyancyPointData[] pointData;

	private Vector2[] pointPositionArray;

	private Vector2[] pointPositionUVArray;

	private float[] pointTerrainHeightArray;

	private float[] pointWaterHeightArray;

	public float waveHeightScale = 0.5f;

	public Buoyancy()
	{
	}

	public void BuoyancyFixedUpdate()
	{
		if (TerrainMeta.WaterMap == null)
		{
			return;
		}
		this.EnsurePointsInitialized();
		if (this.rigidBody == null)
		{
			return;
		}
		if (this.rigidBody.IsSleeping())
		{
			this.Sleep();
			return;
		}
		if (this.buoyancyScale == 0f)
		{
			this.Sleep();
			return;
		}
		float single = Time.time;
		float position = TerrainMeta.Position.x;
		float position1 = TerrainMeta.Position.z;
		float oneOverSize = TerrainMeta.OneOverSize.x;
		float oneOverSize1 = TerrainMeta.OneOverSize.z;
		Matrix4x4 matrix4x4 = base.transform.localToWorldMatrix;
		for (int i = 0; i < (int)this.pointData.Length; i++)
		{
			BuoyancyPoint buoyancyPoint = this.points[i];
			Vector3 vector3 = matrix4x4.MultiplyPoint3x4(this.pointData[i].rootToPoint);
			this.pointData[i].position = vector3;
			float single1 = (vector3.x - position) * oneOverSize;
			float single2 = (vector3.z - position1) * oneOverSize1;
			this.pointPositionArray[i] = new Vector2(vector3.x, vector3.z);
			this.pointPositionUVArray[i] = new Vector2(single1, single2);
		}
		WaterSystem.GetHeight(this.pointPositionArray, this.pointPositionUVArray, this.pointTerrainHeightArray, this.pointWaterHeightArray);
		int num = 0;
		for (int j = 0; j < (int)this.points.Length; j++)
		{
			BuoyancyPoint buoyancyPoint1 = this.points[j];
			Vector3 vector31 = this.pointData[j].position;
			Vector3 vector32 = this.pointData[j].localPosition;
			Vector2 vector2 = this.pointPositionUVArray[j];
			float single3 = this.pointTerrainHeightArray[j];
			float single4 = this.pointWaterHeightArray[j];
			WaterLevel.WaterInfo buoyancyWaterInfo = WaterLevel.GetBuoyancyWaterInfo(vector31, vector2, single3, single4);
			bool flag = false;
			if (vector31.y < buoyancyWaterInfo.surfaceLevel)
			{
				flag = true;
				num++;
				float single5 = buoyancyWaterInfo.currentDepth;
				float single6 = Mathf.InverseLerp(0f, buoyancyPoint1.size, single5);
				float single7 = 1f + Mathf.PerlinNoise(buoyancyPoint1.randomOffset + single * buoyancyPoint1.waveFrequency, 0f) * buoyancyPoint1.waveScale;
				float single8 = buoyancyPoint1.buoyancyForce * this.buoyancyScale;
				Vector3 vector33 = new Vector3(0f, single7 * single6 * single8, 0f);
				Vector3 flowDirection = this.GetFlowDirection(vector2);
				if (flowDirection.y < 0.9999f && flowDirection != Vector3.up)
				{
					single8 *= 0.25f;
					ref float singlePointer = ref vector33.x;
					singlePointer = singlePointer + flowDirection.x * single8;
					ref float singlePointer1 = ref vector33.y;
					singlePointer1 = singlePointer1 + flowDirection.y * single8;
					ref float singlePointer2 = ref vector33.z;
					singlePointer2 = singlePointer2 + flowDirection.z * single8;
				}
				this.rigidBody.AddForceAtPosition(vector33, vector31, ForceMode.Force);
			}
			if (buoyancyPoint1.doSplashEffects && (!buoyancyPoint1.wasSubmergedLastFrame & flag || !flag && buoyancyPoint1.wasSubmergedLastFrame) && this.doEffects && this.rigidBody.GetRelativePointVelocity(vector32).magnitude > 1f)
			{
				string str = (this.waterImpacts == null || this.waterImpacts.Length == 0 || !this.waterImpacts[0].isValid ? Buoyancy.DefaultWaterImpact() : this.waterImpacts[0].resourcePath);
				Vector3 vector34 = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0f, UnityEngine.Random.Range(-0.25f, 0.25f));
				if (!this.clientSide)
				{
					Effect.server.Run(str, vector31 + vector34, Vector3.up, null, false);
				}
				else
				{
					Effect.client.Run(str, vector31 + vector34, Vector3.up, new Vector3());
				}
				buoyancyPoint1.nexSplashTime = Time.time + 0.25f;
			}
			buoyancyPoint1.wasSubmergedLastFrame = flag;
		}
		if (this.points.Length != 0)
		{
			this.submergedFraction = (float)num / (float)((int)this.points.Length);
		}
		if (this.submergedFraction <= 0f)
		{
			this.timeOutOfWater += Time.fixedDeltaTime;
			this.timeInWater = 0f;
		}
		else
		{
			this.timeInWater += Time.fixedDeltaTime;
			this.timeOutOfWater = 0f;
		}
		if (this.timeOutOfWater > 3f && base.enabled)
		{
			this.Sleep();
		}
	}

	public void CheckForWake()
	{
		bool flag = !this.rigidBody.IsSleeping();
		if (WaterSystem.GetHeight(base.transform.position) > (base.transform.position - (Vector3.up * 0.2f)).y & flag)
		{
			this.Wake();
		}
	}

	public static string DefaultWaterImpact()
	{
		return "assets/bundled/prefabs/fx/impacts/physics/water-enter-exit.prefab";
	}

	public void EnsurePointsInitialized()
	{
		if (this.points == null || this.points.Length == 0)
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component != null)
			{
				GameObject gameObject = new GameObject("BuoyancyPoint");
				gameObject.transform.parent = component.gameObject.transform;
				gameObject.transform.localPosition = component.centerOfMass;
				BuoyancyPoint buoyancyPoint = gameObject.AddComponent<BuoyancyPoint>();
				buoyancyPoint.buoyancyForce = component.mass * -Physics.gravity.y;
				buoyancyPoint.buoyancyForce *= 1.32f;
				buoyancyPoint.size = 0.2f;
				this.points = new BuoyancyPoint[] { buoyancyPoint };
			}
		}
		if (this.pointData == null || (int)this.pointData.Length != (int)this.points.Length)
		{
			this.pointData = new Buoyancy.BuoyancyPointData[(int)this.points.Length];
			this.pointPositionArray = new Vector2[(int)this.points.Length];
			this.pointPositionUVArray = new Vector2[(int)this.points.Length];
			this.pointTerrainHeightArray = new float[(int)this.points.Length];
			this.pointWaterHeightArray = new float[(int)this.points.Length];
			for (int i = 0; i < (int)this.points.Length; i++)
			{
				Transform transforms = this.points[i].transform;
				Transform transforms1 = transforms.parent;
				transforms.SetParent(base.transform);
				Vector3 vector3 = transforms.localPosition;
				transforms.SetParent(transforms1);
				this.pointData[i].transform = transforms;
				this.pointData[i].localPosition = transforms.localPosition;
				this.pointData[i].rootToPoint = vector3;
			}
		}
	}

	public void FixedUpdate()
	{
		bool flag = this.submergedFraction > 0f;
		this.BuoyancyFixedUpdate();
		bool flag1 = this.submergedFraction > 0f;
		if (this.SubmergedChanged != null && flag != flag1)
		{
			this.SubmergedChanged(flag1);
		}
	}

	public Vector3 GetFlowDirection(Vector2 posUV)
	{
		if (TerrainMeta.WaterMap == null)
		{
			return Vector3.zero;
		}
		Vector3 normalFast = TerrainMeta.WaterMap.GetNormalFast(posUV);
		float single = Mathf.Clamp01(Mathf.Abs(normalFast.y));
		normalFast.y = 0f;
		normalFast.FastRenormalize(single);
		return normalFast;
	}

	public void Sleep()
	{
		base.enabled = false;
		base.InvokeRandomized(new Action(this.CheckForWake), 0.5f, 0.5f, 0.1f);
	}

	public void Wake()
	{
		base.enabled = true;
		base.CancelInvoke(new Action(this.CheckForWake));
	}

	private struct BuoyancyPointData
	{
		public Transform transform;

		public Vector3 localPosition;

		public Vector3 rootToPoint;

		public Vector3 position;
	}
}