using System;
using UnityEngine;

namespace VLB
{
	[DisallowMultipleComponent]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-triggerzone/")]
	[RequireComponent(typeof(VolumetricLightBeam))]
	public class TriggerZone : MonoBehaviour
	{
		public bool setIsTrigger = true;

		public float rangeMultiplier = 1f;

		private const int kMeshColliderNumSides = 8;

		private Mesh m_Mesh;

		public TriggerZone()
		{
		}

		private void Update()
		{
			VolumetricLightBeam component = base.GetComponent<VolumetricLightBeam>();
			if (component)
			{
				MeshCollider orAddComponent = base.gameObject.GetOrAddComponent<MeshCollider>();
				Debug.Assert(orAddComponent);
				float single = component.fadeEnd * this.rangeMultiplier;
				float single1 = Mathf.LerpUnclamped(component.coneRadiusStart, component.coneRadiusEnd, this.rangeMultiplier);
				this.m_Mesh = MeshGenerator.GenerateConeZ_Radius(single, component.coneRadiusStart, single1, 8, 0, false);
				this.m_Mesh.hideFlags = Consts.ProceduralObjectsHideFlags;
				orAddComponent.sharedMesh = this.m_Mesh;
				if (this.setIsTrigger)
				{
					orAddComponent.convex = true;
					orAddComponent.isTrigger = true;
				}
				UnityEngine.Object.Destroy(this);
			}
		}
	}
}