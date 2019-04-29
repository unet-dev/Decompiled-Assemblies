using System;
using UnityEngine;

namespace Smaa
{
	[AddComponentMenu("Image Effects/Subpixel Morphological Antialiasing")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class SMAA : MonoBehaviour
	{
		public Smaa.DebugPass DebugPass;

		public QualityPreset Quality = QualityPreset.High;

		public EdgeDetectionMethod DetectionMethod = EdgeDetectionMethod.Luma;

		public bool UsePredication;

		public Preset CustomPreset;

		public PredicationPreset CustomPredicationPreset;

		public UnityEngine.Shader Shader;

		public Texture2D AreaTex;

		public Texture2D SearchTex;

		protected Camera m_Camera;

		protected Preset m_LowPreset;

		protected Preset m_MediumPreset;

		protected Preset m_HighPreset;

		protected Preset m_UltraPreset;

		protected UnityEngine.Material m_Material;

		public UnityEngine.Material Material
		{
			get
			{
				if (this.m_Material == null)
				{
					this.m_Material = new UnityEngine.Material(this.Shader)
					{
						hideFlags = HideFlags.HideAndDontSave
					};
				}
				return this.m_Material;
			}
		}

		public SMAA()
		{
		}
	}
}