using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	public class RenderTextureUtility
	{
		private List<RenderTexture> m_TemporaryRTs = new List<RenderTexture>();

		public RenderTextureUtility()
		{
		}

		public RenderTexture GetTemporaryRenderTexture(int width, int height, int depthBuffer = 0, RenderTextureFormat format = 2, FilterMode filterMode = 1)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, depthBuffer, format);
			temporary.filterMode = filterMode;
			temporary.wrapMode = TextureWrapMode.Clamp;
			temporary.name = "RenderTextureUtilityTempTexture";
			this.m_TemporaryRTs.Add(temporary);
			return temporary;
		}

		public void ReleaseAllTemporaryRenderTextures()
		{
			for (int i = 0; i < this.m_TemporaryRTs.Count; i++)
			{
				RenderTexture.ReleaseTemporary(this.m_TemporaryRTs[i]);
			}
			this.m_TemporaryRTs.Clear();
		}

		public void ReleaseTemporaryRenderTexture(RenderTexture rt)
		{
			if (rt == null)
			{
				return;
			}
			if (this.m_TemporaryRTs.Contains(rt))
			{
				this.m_TemporaryRTs.Remove(rt);
				RenderTexture.ReleaseTemporary(rt);
				return;
			}
			Debug.LogErrorFormat("Attempting to remove texture that was not allocated: {0}", new object[] { rt });
		}
	}
}