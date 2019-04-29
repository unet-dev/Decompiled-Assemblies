using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rust.Components.Camera
{
	[ExecuteInEditMode]
	public class LightingOverride : MonoBehaviour
	{
		public bool overrideAmbientLight;

		public AmbientMode ambientMode;

		public Color ambientGroundColor;

		public Color ambientEquatorColor;

		public Color ambientLight;

		public float ambientIntensity;

		internal Color old_ambientLight;

		internal Color old_ambientGroundColor;

		internal Color old_ambientEquatorColor;

		internal float old_ambientIntensity;

		internal AmbientMode old_ambientMode;

		public float aspect;

		public LightingOverride()
		{
		}

		private void OnPostRender()
		{
			if (this.overrideAmbientLight)
			{
				RenderSettings.ambientMode = this.ambientMode;
				RenderSettings.ambientLight = this.old_ambientLight;
				RenderSettings.ambientIntensity = this.old_ambientIntensity;
				RenderSettings.ambientMode = this.old_ambientMode;
				RenderSettings.ambientGroundColor = this.old_ambientGroundColor;
				RenderSettings.ambientEquatorColor = this.old_ambientEquatorColor;
				RenderSettings.ambientGroundColor = this.old_ambientGroundColor;
			}
		}

		private void OnPreRender()
		{
			if (this.overrideAmbientLight)
			{
				this.old_ambientLight = RenderSettings.ambientLight;
				this.old_ambientIntensity = RenderSettings.ambientIntensity;
				this.old_ambientMode = RenderSettings.ambientMode;
				this.old_ambientGroundColor = RenderSettings.ambientGroundColor;
				this.old_ambientEquatorColor = RenderSettings.ambientEquatorColor;
				this.old_ambientGroundColor = RenderSettings.ambientGroundColor;
				RenderSettings.ambientMode = this.ambientMode;
				RenderSettings.ambientLight = this.ambientLight;
				RenderSettings.ambientIntensity = this.ambientIntensity;
				RenderSettings.ambientGroundColor = this.ambientLight;
				RenderSettings.ambientEquatorColor = this.ambientEquatorColor;
				RenderSettings.ambientGroundColor = this.ambientGroundColor;
			}
			if (this.aspect > 0f)
			{
				base.GetComponent<UnityEngine.Camera>().aspect = this.aspect;
			}
		}
	}
}