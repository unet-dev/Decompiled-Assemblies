using System;
using UnityEngine;

public class RadialSpawnPoint : BaseSpawnPoint
{
	public float radius = 10f;

	public RadialSpawnPoint()
	{
	}

	public override void GetLocation(out Vector3 pos, out Quaternion rot)
	{
		Vector2 vector2 = UnityEngine.Random.insideUnitCircle * this.radius;
		pos = base.transform.position + new Vector3(vector2.x, 0f, vector2.y);
		rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		base.DropToGround(ref pos, ref rot);
	}

	public override void ObjectRetired(SpawnPointInstance instance)
	{
	}

	public override void ObjectSpawned(SpawnPointInstance instance)
	{
	}
}