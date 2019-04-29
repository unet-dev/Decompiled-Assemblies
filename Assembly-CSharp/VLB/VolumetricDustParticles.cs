using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace VLB
{
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-dustparticles/")]
	[RequireComponent(typeof(VolumetricLightBeam))]
	public class VolumetricDustParticles : MonoBehaviour
	{
		[Range(0f, 1f)]
		public float alpha = 0.5f;

		[Range(0.0001f, 0.1f)]
		public float size = 0.01f;

		public VolumetricDustParticles.Direction direction = VolumetricDustParticles.Direction.Random;

		public float speed = 0.03f;

		public float density = 5f;

		[Range(0f, 1f)]
		public float spawnMaxDistance = 0.7f;

		public bool cullingEnabled = true;

		public float cullingMaxDistance = 10f;

		public static bool isFeatureSupported;

		private ParticleSystem m_Particles;

		private ParticleSystemRenderer m_Renderer;

		private static bool ms_NoMainCameraLogged;

		private static Camera ms_MainCamera;

		private VolumetricLightBeam m_Master;

		public bool isCulled
		{
			get;
			private set;
		}

		public Camera mainCamera
		{
			get
			{
				if (!VolumetricDustParticles.ms_MainCamera)
				{
					VolumetricDustParticles.ms_MainCamera = Camera.main;
					if (!VolumetricDustParticles.ms_MainCamera && !VolumetricDustParticles.ms_NoMainCameraLogged)
					{
						Debug.LogErrorFormat(base.gameObject, "In order to use 'VolumetricDustParticles' culling, you must have a MainCamera defined in your scene.", Array.Empty<object>());
						VolumetricDustParticles.ms_NoMainCameraLogged = true;
					}
				}
				return VolumetricDustParticles.ms_MainCamera;
			}
		}

		public bool particlesAreInstantiated
		{
			get
			{
				return this.m_Particles;
			}
		}

		public int particlesCurrentCount
		{
			get
			{
				if (!this.m_Particles)
				{
					return 0;
				}
				return this.m_Particles.particleCount;
			}
		}

		public int particlesMaxCount
		{
			get
			{
				if (!this.m_Particles)
				{
					return 0;
				}
				return this.m_Particles.main.maxParticles;
			}
		}

		static VolumetricDustParticles()
		{
			VolumetricDustParticles.isFeatureSupported = true;
			VolumetricDustParticles.ms_NoMainCameraLogged = false;
			VolumetricDustParticles.ms_MainCamera = null;
		}

		public VolumetricDustParticles()
		{
		}

		private void InstantiateParticleSystem()
		{
			ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>(true);
			for (int i = (int)componentsInChildren.Length - 1; i >= 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
			}
			this.m_Particles = Config.Instance.NewVolumetricDustParticles();
			if (this.m_Particles)
			{
				this.m_Particles.transform.SetParent(base.transform, false);
				this.m_Renderer = this.m_Particles.GetComponent<ParticleSystemRenderer>();
			}
		}

		private void OnDestroy()
		{
			if (this.m_Particles)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Particles.gameObject);
			}
			this.m_Particles = null;
		}

		private void OnDisable()
		{
			if (this.m_Particles)
			{
				this.m_Particles.gameObject.SetActive(false);
			}
		}

		private void OnEnable()
		{
			this.SetActiveAndPlay();
		}

		private void SetActiveAndPlay()
		{
			if (this.m_Particles)
			{
				this.m_Particles.gameObject.SetActive(true);
				this.SetParticleProperties();
				this.m_Particles.Play(true);
			}
		}

		private void SetParticleProperties()
		{
			if (this.m_Particles && this.m_Particles.gameObject.activeSelf)
			{
				float single = Mathf.Clamp01(1f - this.m_Master.fresnelPow / 10f);
				float mMaster = this.m_Master.fadeEnd * this.spawnMaxDistance;
				float single1 = mMaster * this.density;
				int num = (int)(single1 * 4f);
				ParticleSystem.MainModule mParticles = this.m_Particles.main;
				ParticleSystem.MinMaxCurve minMaxCurve = mParticles.startLifetime;
				minMaxCurve.mode = ParticleSystemCurveMode.TwoConstants;
				minMaxCurve.constantMin = 4f;
				minMaxCurve.constantMax = 6f;
				mParticles.startLifetime = minMaxCurve;
				ParticleSystem.MinMaxCurve minMaxCurve1 = mParticles.startSize;
				minMaxCurve1.mode = ParticleSystemCurveMode.TwoConstants;
				minMaxCurve1.constantMin = this.size * 0.9f;
				minMaxCurve1.constantMax = this.size * 1.1f;
				mParticles.startSize = minMaxCurve1;
				ParticleSystem.MinMaxGradient minMaxGradient = mParticles.startColor;
				if (this.m_Master.colorMode != ColorMode.Flat)
				{
					minMaxGradient.mode = ParticleSystemGradientMode.Gradient;
					Gradient gradient = this.m_Master.colorGradient;
					GradientColorKey[] gradientColorKeyArray = gradient.colorKeys;
					GradientAlphaKey[] gradientAlphaKeyArray = gradient.alphaKeys;
					for (int i = 0; i < (int)gradientAlphaKeyArray.Length; i++)
					{
						gradientAlphaKeyArray[i].alpha *= this.alpha;
					}
					Gradient gradient1 = new Gradient();
					gradient1.SetKeys(gradientColorKeyArray, gradientAlphaKeyArray);
					minMaxGradient.gradient = gradient1;
				}
				else
				{
					minMaxGradient.mode = ParticleSystemGradientMode.Color;
					Color color = this.m_Master.color;
					color.a *= this.alpha;
					minMaxGradient.color = color;
				}
				mParticles.startColor = minMaxGradient;
				ParticleSystem.MinMaxCurve minMaxCurve2 = mParticles.startSpeed;
				minMaxCurve2.constant = this.speed;
				mParticles.startSpeed = minMaxCurve2;
				mParticles.maxParticles = num;
				ParticleSystem.ShapeModule shapeModule = this.m_Particles.shape;
				shapeModule.shapeType = ParticleSystemShapeType.ConeVolume;
				shapeModule.radius = this.m_Master.coneRadiusStart * Mathf.Lerp(0.3f, 1f, single);
				shapeModule.angle = this.m_Master.coneAngle * 0.5f * Mathf.Lerp(0.7f, 1f, single);
				shapeModule.length = mMaster;
				shapeModule.arc = 360f;
				shapeModule.randomDirectionAmount = (this.direction == VolumetricDustParticles.Direction.Random ? 1f : 0f);
				ParticleSystem.EmissionModule emissionModule = this.m_Particles.emission;
				ParticleSystem.MinMaxCurve minMaxCurve3 = emissionModule.rateOverTime;
				minMaxCurve3.constant = single1;
				emissionModule.rateOverTime = minMaxCurve3;
				if (this.m_Renderer)
				{
					this.m_Renderer.sortingLayerID = this.m_Master.sortingLayerID;
					this.m_Renderer.sortingOrder = this.m_Master.sortingOrder;
				}
			}
		}

		private void Start()
		{
			this.isCulled = false;
			this.m_Master = base.GetComponent<VolumetricLightBeam>();
			Debug.Assert(this.m_Master);
			this.InstantiateParticleSystem();
			this.SetActiveAndPlay();
		}

		private void Update()
		{
			if (Application.isPlaying)
			{
				this.UpdateCulling();
			}
			this.SetParticleProperties();
		}

		private void UpdateCulling()
		{
			if (this.m_Particles)
			{
				bool flag = true;
				if (this.cullingEnabled && this.m_Master.hasGeometry)
				{
					if (!this.mainCamera)
					{
						this.cullingEnabled = false;
					}
					else
					{
						float single = this.cullingMaxDistance * this.cullingMaxDistance;
						Bounds mMaster = this.m_Master.bounds;
						flag = mMaster.SqrDistance(this.mainCamera.transform.position) <= single;
					}
				}
				if (this.m_Particles.gameObject.activeSelf != flag)
				{
					this.m_Particles.gameObject.SetActive(flag);
					this.isCulled = !flag;
				}
				if (flag && !this.m_Particles.isPlaying)
				{
					this.m_Particles.Play();
				}
			}
		}

		public enum Direction
		{
			Beam,
			Random
		}
	}
}