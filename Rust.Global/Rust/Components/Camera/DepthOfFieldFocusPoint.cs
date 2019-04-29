using System;
using UnityEngine;

namespace Rust.Components.Camera
{
	public class DepthOfFieldFocusPoint : ListComponent<DepthOfFieldFocusPoint>
	{
		private Renderer cachedRenderer;

		public Vector3 FocusPoint
		{
			get
			{
				Vector3 vector3 = base.transform.position;
				if (this.cachedRenderer != null)
				{
					vector3 = this.cachedRenderer.bounds.center;
				}
				return vector3;
			}
		}

		public DepthOfFieldFocusPoint()
		{
		}

		public static DepthOfFieldFocusPoint Evaluate(UnityEngine.Camera cam)
		{
			if (ListComponent<DepthOfFieldFocusPoint>.InstanceList == null || ListComponent<DepthOfFieldFocusPoint>.InstanceList.Count == 0)
			{
				return null;
			}
			DepthOfFieldFocusPoint item = null;
			float single = Single.MaxValue;
			for (int i = 0; i < ListComponent<DepthOfFieldFocusPoint>.InstanceList.Count; i++)
			{
				float single1 = ListComponent<DepthOfFieldFocusPoint>.InstanceList[i].Score(cam);
				if (single1 < single)
				{
					single = single1;
					item = ListComponent<DepthOfFieldFocusPoint>.InstanceList[i];
				}
			}
			return item;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.cachedRenderer = base.GetComponentInChildren<MeshRenderer>(true);
			if (this.cachedRenderer == null)
			{
				this.cachedRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>(true);
			}
		}

		private float Score(UnityEngine.Camera cam)
		{
			Vector3 screenPoint = cam.WorldToScreenPoint(this.FocusPoint);
			if (screenPoint.z < 0f)
			{
				return Single.MaxValue;
			}
			Vector2 vector2 = new Vector2(screenPoint.x, screenPoint.y) - new Vector2((float)(cam.pixelWidth / 2), (float)(cam.pixelHeight / 2));
			return vector2.sqrMagnitude + screenPoint.z * 128f;
		}
	}
}