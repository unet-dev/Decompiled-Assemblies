using Network;
using System;
using UnityEngine;

public class SantaSleigh : BaseEntity
{
	public GameObjectRef prefabDrop;

	public SpawnFilter filter;

	public Transform dropOrigin;

	[ServerVar]
	public static float altitudeAboveTerrain;

	[ServerVar]
	public static float desiredAltitude;

	public Light bigLight;

	public SoundPlayer hohoho;

	public float hohohospacing = 4f;

	public float hohoho_additional_spacing = 2f;

	private Vector3 startPos;

	private Vector3 endPos;

	private float secondsToTake;

	private float secondsTaken;

	private bool dropped;

	private Vector3 dropPosition = Vector3.zero;

	public Vector3 swimScale;

	public Vector3 swimSpeed;

	private float swimRandom;

	public float appliedSwimScale = 1f;

	public float appliedSwimRotation = 20f;

	private const string path = "assets/prefabs/misc/xmas/sleigh/santasleigh.prefab";

	static SantaSleigh()
	{
		SantaSleigh.altitudeAboveTerrain = 50f;
		SantaSleigh.desiredAltitude = 60f;
	}

	public SantaSleigh()
	{
	}

	[ServerVar]
	public static void drop(ConsoleSystem.Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (!basePlayer)
		{
			return;
		}
		Debug.Log("Santa Inbound");
		GameManager gameManager = GameManager.server;
		Vector3 vector3 = new Vector3();
		Quaternion quaternion = new Quaternion();
		BaseEntity baseEntity = gameManager.CreateEntity("assets/prefabs/misc/xmas/sleigh/santasleigh.prefab", vector3, quaternion, true);
		if (baseEntity)
		{
			baseEntity.GetComponent<SantaSleigh>().InitDropPosition(basePlayer.transform.position + new Vector3(0f, 10f, 0f));
			baseEntity.Spawn();
		}
	}

	private void FixedUpdate()
	{
		if (!base.isServer)
		{
			return;
		}
		this.secondsTaken += Time.deltaTime;
		float single = Mathf.InverseLerp(0f, this.secondsToTake, this.secondsTaken);
		if (!this.dropped && single >= 0.5f)
		{
			this.dropped = true;
			GameManager gameManager = GameManager.server;
			string str = this.prefabDrop.resourcePath;
			Vector3 vector3 = this.dropOrigin.transform.position;
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity(str, vector3, quaternion, true);
			if (baseEntity)
			{
				baseEntity.globalBroadcast = true;
				baseEntity.Spawn();
			}
		}
		base.transform.position = Vector3.Lerp(this.startPos, this.endPos, single);
		Vector3 vector31 = (this.endPos - this.startPos).normalized;
		Vector3 vector32 = Vector3.zero;
		if (this.swimScale != Vector3.zero)
		{
			if (this.swimRandom == 0f)
			{
				this.swimRandom = UnityEngine.Random.Range(0f, 20f);
			}
			float single1 = Time.time + this.swimRandom;
			vector32 = new Vector3(Mathf.Sin(single1 * this.swimSpeed.x) * this.swimScale.x, Mathf.Cos(single1 * this.swimSpeed.y) * this.swimScale.y, Mathf.Sin(single1 * this.swimSpeed.z) * this.swimScale.z);
			vector32 = base.transform.InverseTransformDirection(vector32);
			Transform transforms = base.transform;
			transforms.position = transforms.position + (vector32 * this.appliedSwimScale);
		}
		base.transform.rotation = Quaternion.LookRotation(vector31) * Quaternion.Euler(Mathf.Cos(Time.time * this.swimSpeed.y) * this.appliedSwimRotation, 0f, Mathf.Sin(Time.time * this.swimSpeed.x) * this.appliedSwimRotation);
		Vector3 vector33 = base.transform.position;
		float height = TerrainMeta.HeightMap.GetHeight(base.transform.position + (base.transform.forward * 30f));
		float height1 = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		float single2 = Mathf.Max(height, height1);
		vector33.y = Mathf.Max(SantaSleigh.desiredAltitude, single2 + SantaSleigh.altitudeAboveTerrain);
		base.transform.position = vector33;
		base.transform.hasChanged = true;
		if (single >= 1f)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	public void InitDropPosition(Vector3 newDropPosition)
	{
		this.dropPosition = newDropPosition;
		this.dropPosition.y = 0f;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("SantaSleigh.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override bool PhysicsDriven()
	{
		return true;
	}

	public Vector3 RandomDropPosition()
	{
		float single;
		Vector3 vector3 = Vector3.zero;
		float single1 = 100f;
		float size = TerrainMeta.Size.x;
		do
		{
			vector3 = Vector3Ex.Range(-(size / 3f), size / 3f);
			if (this.filter.GetFactor(vector3) != 0f)
			{
				break;
			}
			single = single1 - 1f;
			single1 = single;
		}
		while (single > 0f);
		vector3.y = 0f;
		return vector3;
	}

	public void SendHoHoHo()
	{
		base.Invoke(new Action(this.SendHoHoHo), this.hohohospacing + UnityEngine.Random.Range(0f, this.hohoho_additional_spacing));
		base.ClientRPC(null, "ClientPlayHoHoHo");
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (this.dropPosition == Vector3.zero)
		{
			this.dropPosition = this.RandomDropPosition();
		}
		this.UpdateDropPosition(this.dropPosition);
		base.Invoke(new Action(this.SendHoHoHo), 0f);
	}

	public void UpdateDropPosition(Vector3 newDropPosition)
	{
		float size = TerrainMeta.Size.x;
		float single = SantaSleigh.altitudeAboveTerrain;
		this.startPos = Vector3Ex.Range(-1f, 1f);
		this.startPos.y = 0f;
		this.startPos.Normalize();
		this.startPos = this.startPos * (size * 1.25f);
		this.startPos.y = single;
		this.endPos = this.startPos * -1f;
		this.endPos.y = this.startPos.y;
		this.startPos += newDropPosition;
		this.endPos += newDropPosition;
		this.secondsToTake = Vector3.Distance(this.startPos, this.endPos) / 25f;
		this.secondsToTake *= UnityEngine.Random.Range(0.95f, 1.05f);
		base.transform.position = this.startPos;
		base.transform.rotation = Quaternion.LookRotation(this.endPos - this.startPos);
		this.dropPosition = newDropPosition;
	}
}