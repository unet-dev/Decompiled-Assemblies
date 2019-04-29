using System;
using UnityEngine;

public abstract class BaseSpawnPoint : MonoBehaviour, IServerComponent
{
	protected BaseSpawnPoint()
	{
	}

	protected void DropToGround(ref Vector3 pos, ref Quaternion rot)
	{
		RaycastHit raycastHit;
		if (TerrainMeta.HeightMap && TerrainMeta.Collision && !TerrainMeta.Collision.GetIgnore(pos, 0.01f))
		{
			float height = TerrainMeta.HeightMap.GetHeight(pos);
			pos.y = Mathf.Max(pos.y, height);
		}
		if (TransformUtil.GetGroundInfo(pos, out raycastHit, 20f, 1235288065, null))
		{
			pos = raycastHit.point;
			rot = Quaternion.LookRotation(rot * Vector3.forward, raycastHit.normal);
		}
	}

	public abstract void GetLocation(out Vector3 pos, out Quaternion rot);

	public abstract void ObjectRetired(SpawnPointInstance instance);

	public abstract void ObjectSpawned(SpawnPointInstance instance);
}