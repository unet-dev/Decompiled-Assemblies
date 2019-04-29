using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
	[HelpURL("http://saladgamer.com/vlb-doc/config/")]
	public class Config : ScriptableObject
	{
		public int geometryLayerID = 1;

		public string geometryTag = "Untagged";

		public int geometryRenderQueue = 3000;

		public bool forceSinglePass;

		[HighlightNull]
		[SerializeField]
		private Shader beamShader1Pass;

		[FormerlySerializedAs("beamShader")]
		[FormerlySerializedAs("BeamShader")]
		[HighlightNull]
		[SerializeField]
		private Shader beamShader2Pass;

		public int sharedMeshSides = 24;

		public int sharedMeshSegments = 5;

		[Range(0.01f, 2f)]
		public float globalNoiseScale = 0.5f;

		public Vector3 globalNoiseVelocity = Consts.NoiseVelocityDefault;

		[HighlightNull]
		public TextAsset noise3DData;

		public int noise3DSize = 64;

		[HighlightNull]
		public ParticleSystem dustParticlesPrefab;

		private static Config m_Instance;

		public Shader beamShader
		{
			get
			{
				if (!this.forceSinglePass)
				{
					return this.beamShader2Pass;
				}
				return this.beamShader1Pass;
			}
		}

		public Vector4 globalNoiseParam
		{
			get
			{
				return new Vector4(this.globalNoiseVelocity.x, this.globalNoiseVelocity.y, this.globalNoiseVelocity.z, this.globalNoiseScale);
			}
		}

		public static Config Instance
		{
			get
			{
				if (Config.m_Instance == null)
				{
					Config[] configArray = Resources.LoadAll<Config>("Config");
					Debug.Assert(configArray.Length != 0, string.Format("Can't find any resource of type '{0}'. Make sure you have a ScriptableObject of this type in a 'Resources' folder.", typeof(Config)));
					Config.m_Instance = configArray[0];
				}
				return Config.m_Instance;
			}
		}

		static Config()
		{
		}

		public Config()
		{
		}

		public ParticleSystem NewVolumetricDustParticles()
		{
			if (!this.dustParticlesPrefab)
			{
				if (Application.isPlaying)
				{
					Debug.LogError("Failed to instantiate VolumetricDustParticles prefab.");
				}
				return null;
			}
			ParticleSystem proceduralObjectsHideFlags = UnityEngine.Object.Instantiate<ParticleSystem>(this.dustParticlesPrefab);
			proceduralObjectsHideFlags.useAutoRandomSeed = false;
			proceduralObjectsHideFlags.name = "Dust Particles";
			proceduralObjectsHideFlags.gameObject.hideFlags = Consts.ProceduralObjectsHideFlags;
			proceduralObjectsHideFlags.gameObject.SetActive(true);
			return proceduralObjectsHideFlags;
		}

		public void Reset()
		{
			this.geometryLayerID = 1;
			this.geometryTag = "Untagged";
			this.geometryRenderQueue = 3000;
			this.beamShader1Pass = Shader.Find("Hidden/VolumetricLightBeam1Pass");
			this.beamShader2Pass = Shader.Find("Hidden/VolumetricLightBeam2Pass");
			this.sharedMeshSides = 24;
			this.sharedMeshSegments = 5;
			this.globalNoiseScale = 0.5f;
			this.globalNoiseVelocity = Consts.NoiseVelocityDefault;
			this.noise3DData = Resources.Load("Noise3D_64x64x64") as TextAsset;
			this.noise3DSize = 64;
			this.dustParticlesPrefab = Resources.Load("DustParticles", typeof(ParticleSystem)) as ParticleSystem;
		}
	}
}