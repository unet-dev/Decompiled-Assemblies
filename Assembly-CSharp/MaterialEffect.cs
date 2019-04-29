using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/MaterialEffect")]
public class MaterialEffect : ScriptableObject
{
	public GameObjectRef DefaultEffect;

	public MaterialEffect.Entry[] Entries;

	public MaterialEffect()
	{
	}

	public void SpawnOnRay(Ray ray, int mask, float length = 0.5f, Vector3 forward = null)
	{
		RaycastHit raycastHit;
		if (GamePhysics.Trace(ray, 0f, out raycastHit, length, mask, QueryTriggerInteraction.UseGlobal))
		{
			Effect.client.Run(this.DefaultEffect.resourcePath, raycastHit.point, raycastHit.normal, forward);
			return;
		}
		Effect.client.Run(this.DefaultEffect.resourcePath, ray.origin, ray.direction * -1f, forward);
	}

	[Serializable]
	public class Entry
	{
		public PhysicMaterial Material;

		public GameObjectRef Effect;

		public Entry()
		{
		}
	}
}