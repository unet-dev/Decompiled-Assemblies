using System;
using UnityEngine;

public class DevMovePlayer : BaseMonoBehaviour
{
	public BasePlayer player;

	public Transform[] Waypoints;

	public bool moveRandomly;

	public Vector3 destination = Vector3.zero;

	public Vector3 lookPoint = Vector3.zero;

	private int waypointIndex;

	private float randRun;

	public DevMovePlayer()
	{
	}

	public void Awake()
	{
		this.randRun = UnityEngine.Random.Range(5f, 10f);
		this.player = base.GetComponent<BasePlayer>();
		if (this.Waypoints.Length == 0)
		{
			this.destination = base.transform.position;
		}
		else
		{
			this.destination = this.Waypoints[0].position;
		}
		if (this.player.isClient)
		{
			return;
		}
		if (this.player.eyes == null)
		{
			this.player.eyes = this.player.GetComponent<PlayerEyes>();
		}
		base.Invoke(new Action(this.LateSpawn), 1f);
	}

	public void LateSpawn()
	{
		Item item = ItemManager.CreateByName("rifle.semiauto", 1, (ulong)0);
		this.player.inventory.GiveItem(item, this.player.inventory.containerBelt);
		this.player.UpdateActiveItem(item.uid);
		this.player.health = 100f;
	}

	public void SetWaypoints(Transform[] wps)
	{
		this.Waypoints = wps;
		this.destination = wps[0].position;
	}

	public void Update()
	{
		RaycastHit raycastHit;
		if (this.player.isClient)
		{
			return;
		}
		if (!this.player.IsAlive() || this.player.IsWounded())
		{
			return;
		}
		if (Vector3.Distance(this.destination, base.transform.position) < 0.25f)
		{
			if (!this.moveRandomly)
			{
				this.waypointIndex++;
			}
			else
			{
				this.waypointIndex = UnityEngine.Random.Range(0, (int)this.Waypoints.Length);
			}
			if (this.waypointIndex >= (int)this.Waypoints.Length)
			{
				this.waypointIndex = 0;
			}
		}
		this.destination = this.Waypoints[this.waypointIndex].position;
		Vector3 vector3 = this.destination - base.transform.position;
		Vector3 vector31 = vector3.normalized;
		float single = Mathf.Sin(Time.time + this.randRun);
		float speed = this.player.GetSpeed(single, 0f);
		Vector3 vector32 = base.transform.position;
		float single1 = 1f;
		LayerMask layerMask = 1537286401;
		if (TransformUtil.GetGroundInfo(base.transform.position + ((vector31 * speed) * Time.deltaTime), out raycastHit, single1, layerMask, this.player.transform))
		{
			vector32 = raycastHit.point;
		}
		base.transform.position = vector32;
		vector3 = new Vector3(this.destination.x, 0f, this.destination.z) - new Vector3(this.player.transform.position.x, 0f, this.player.transform.position.z);
		Vector3 vector33 = vector3.normalized;
		this.player.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}
}