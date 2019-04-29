using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
	[SelectionBase]
	public class VolumetricLightBeam : MonoBehaviour
	{
		public bool colorFromLight = true;

		public ColorMode colorMode;

		[ColorUsage(true, true)]
		[FormerlySerializedAs("colorValue")]
		public Color color = Consts.FlatColor;

		public Gradient colorGradient;

		[Range(0f, 1f)]
		public float alphaInside = 1f;

		[FormerlySerializedAs("alpha")]
		[Range(0f, 1f)]
		public float alphaOutside = 1f;

		public BlendingMode blendingMode;

		[FormerlySerializedAs("angleFromLight")]
		public bool spotAngleFromLight = true;

		[Range(0.1f, 179.9f)]
		public float spotAngle = 35f;

		[FormerlySerializedAs("radiusStart")]
		public float coneRadiusStart = 0.1f;

		public MeshType geomMeshType;

		[FormerlySerializedAs("geomSides")]
		public int geomCustomSides = 18;

		public int geomCustomSegments = 5;

		public bool geomCap;

		public bool fadeEndFromLight = true;

		public AttenuationEquation attenuationEquation = AttenuationEquation.Quadratic;

		[Range(0f, 1f)]
		public float attenuationCustomBlending = 0.5f;

		public float fadeStart;

		public float fadeEnd = 3f;

		public float depthBlendDistance = 2f;

		public float cameraClippingDistance = 0.5f;

		[Range(0f, 1f)]
		public float glareFrontal = 0.5f;

		[Range(0f, 1f)]
		public float glareBehind = 0.5f;

		[Obsolete("Use 'glareFrontal' instead")]
		public float boostDistanceInside = 0.5f;

		[Obsolete("This property has been merged with 'fresnelPow'")]
		public float fresnelPowInside = 6f;

		[FormerlySerializedAs("fresnelPowOutside")]
		public float fresnelPow = 8f;

		public bool noiseEnabled;

		[Range(0f, 1f)]
		public float noiseIntensity = 0.5f;

		public bool noiseScaleUseGlobal = true;

		[Range(0.01f, 2f)]
		public float noiseScaleLocal = 0.5f;

		public bool noiseVelocityUseGlobal = true;

		public Vector3 noiseVelocityLocal = Consts.NoiseVelocityDefault;

		private Plane m_PlaneWS;

		[SerializeField]
		private int pluginVersion = -1;

		[FormerlySerializedAs("trackChangesDuringPlaytime")]
		[SerializeField]
		private bool _TrackChangesDuringPlaytime;

		[SerializeField]
		private int _SortingLayerID;

		[SerializeField]
		private int _SortingOrder;

		private BeamGeometry m_BeamGeom;

		private Coroutine m_CoPlaytimeUpdate;

		private Light _CachedLight;

		public float attenuationLerpLinearQuad
		{
			get
			{
				if (this.attenuationEquation == AttenuationEquation.Linear)
				{
					return 0f;
				}
				if (this.attenuationEquation == AttenuationEquation.Quadratic)
				{
					return 1f;
				}
				return this.attenuationCustomBlending;
			}
		}

		public int blendingModeAsInt
		{
			get
			{
				return Mathf.Clamp((int)this.blendingMode, 0, Enum.GetValues(typeof(BlendingMode)).Length);
			}
		}

		public Bounds bounds
		{
			get
			{
				if (this.m_BeamGeom == null)
				{
					return new Bounds(Vector3.zero, Vector3.zero);
				}
				return this.m_BeamGeom.meshRenderer.bounds;
			}
		}

		public float coneAngle
		{
			get
			{
				return Mathf.Atan2(this.coneRadiusEnd - this.coneRadiusStart, this.fadeEnd) * 57.29578f * 2f;
			}
		}

		public float coneApexOffsetZ
		{
			get
			{
				float single = this.coneRadiusStart / this.coneRadiusEnd;
				if (single == 1f)
				{
					return Single.MaxValue;
				}
				return this.fadeEnd * single / (1f - single);
			}
		}

		public float coneRadiusEnd
		{
			get
			{
				return this.fadeEnd * Mathf.Tan(this.spotAngle * 0.0174532924f * 0.5f);
			}
		}

		public float coneVolume
		{
			get
			{
				float single = this.coneRadiusStart;
				float single1 = this.coneRadiusEnd;
				return 1.04719758f * (single * single + single * single1 + single1 * single1) * this.fadeEnd;
			}
		}

		public int geomSegments
		{
			get
			{
				if (this.geomMeshType != MeshType.Custom)
				{
					return Config.Instance.sharedMeshSegments;
				}
				return this.geomCustomSegments;
			}
			set
			{
				this.geomCustomSegments = value;
				UnityEngine.Debug.LogWarning("The setter VLB.VolumetricLightBeam.geomSegments is OBSOLETE and has been renamed to geomCustomSegments.");
			}
		}

		public int geomSides
		{
			get
			{
				if (this.geomMeshType != MeshType.Custom)
				{
					return Config.Instance.sharedMeshSides;
				}
				return this.geomCustomSides;
			}
			set
			{
				this.geomCustomSides = value;
				UnityEngine.Debug.LogWarning("The setter VLB.VolumetricLightBeam.geomSides is OBSOLETE and has been renamed to geomCustomSides.");
			}
		}

		public bool hasGeometry
		{
			get
			{
				return this.m_BeamGeom != null;
			}
		}

		public bool isCurrentlyTrackingChanges
		{
			get
			{
				return this.m_CoPlaytimeUpdate != null;
			}
		}

		private Light lightSpotAttached
		{
			get
			{
				if (this._CachedLight == null)
				{
					this._CachedLight = base.GetComponent<Light>();
				}
				if (!this._CachedLight || this._CachedLight.type != LightType.Spot)
				{
					return null;
				}
				return this._CachedLight;
			}
		}

		public string meshStats
		{
			get
			{
				Mesh mBeamGeom;
				if (this.m_BeamGeom)
				{
					mBeamGeom = this.m_BeamGeom.coneMesh;
				}
				else
				{
					mBeamGeom = null;
				}
				Mesh mesh = mBeamGeom;
				if (!mesh)
				{
					return "no mesh available";
				}
				return string.Format("Cone angle: {0:0.0} degrees\nMesh: {1} vertices, {2} triangles", this.coneAngle, mesh.vertexCount, (int)mesh.triangles.Length / 3);
			}
		}

		public int meshTrianglesCount
		{
			get
			{
				if (!this.m_BeamGeom || !this.m_BeamGeom.coneMesh)
				{
					return 0;
				}
				return (int)this.m_BeamGeom.coneMesh.triangles.Length / 3;
			}
		}

		public int meshVerticesCount
		{
			get
			{
				if (!this.m_BeamGeom || !this.m_BeamGeom.coneMesh)
				{
					return 0;
				}
				return this.m_BeamGeom.coneMesh.vertexCount;
			}
		}

		public int sortingLayerID
		{
			get
			{
				return this._SortingLayerID;
			}
			set
			{
				this._SortingLayerID = value;
				if (this.m_BeamGeom)
				{
					this.m_BeamGeom.sortingLayerID = value;
				}
			}
		}

		public string sortingLayerName
		{
			get
			{
				return SortingLayer.IDToName(this.sortingLayerID);
			}
			set
			{
				this.sortingLayerID = SortingLayer.NameToID(value);
			}
		}

		public int sortingOrder
		{
			get
			{
				return this._SortingOrder;
			}
			set
			{
				this._SortingOrder = value;
				if (this.m_BeamGeom)
				{
					this.m_BeamGeom.sortingOrder = value;
				}
			}
		}

		public bool trackChangesDuringPlaytime
		{
			get
			{
				return this._TrackChangesDuringPlaytime;
			}
			set
			{
				this._TrackChangesDuringPlaytime = value;
				this.StartPlaytimeUpdateIfNeeded();
			}
		}

		public VolumetricLightBeam()
		{
		}

		private void AssignPropertiesFromSpotLight(Light lightSpot)
		{
			if (lightSpot && lightSpot.type == LightType.Spot)
			{
				if (this.fadeEndFromLight)
				{
					this.fadeEnd = lightSpot.range;
				}
				if (this.spotAngleFromLight)
				{
					this.spotAngle = lightSpot.spotAngle;
				}
				if (this.colorFromLight)
				{
					this.colorMode = ColorMode.Flat;
					this.color = lightSpot.color;
				}
			}
		}

		private void ClampProperties()
		{
			this.alphaInside = Mathf.Clamp01(this.alphaInside);
			this.alphaOutside = Mathf.Clamp01(this.alphaOutside);
			this.attenuationCustomBlending = Mathf.Clamp01(this.attenuationCustomBlending);
			this.fadeEnd = Mathf.Max(0.01f, this.fadeEnd);
			this.fadeStart = Mathf.Clamp(this.fadeStart, 0f, this.fadeEnd - 0.01f);
			this.spotAngle = Mathf.Clamp(this.spotAngle, 0.1f, 179.9f);
			this.coneRadiusStart = Mathf.Max(this.coneRadiusStart, 0f);
			this.depthBlendDistance = Mathf.Max(this.depthBlendDistance, 0f);
			this.cameraClippingDistance = Mathf.Max(this.cameraClippingDistance, 0f);
			this.geomCustomSides = Mathf.Clamp(this.geomCustomSides, 3, 256);
			this.geomCustomSegments = Mathf.Clamp(this.geomCustomSegments, 0, 64);
			this.fresnelPow = Mathf.Max(0f, this.fresnelPow);
			this.glareBehind = Mathf.Clamp01(this.glareBehind);
			this.glareFrontal = Mathf.Clamp01(this.glareFrontal);
			this.noiseIntensity = Mathf.Clamp(this.noiseIntensity, 0f, 1f);
		}

		private IEnumerator CoPlaytimeUpdate()
		{
			VolumetricLightBeam volumetricLightBeam = null;
			while (volumetricLightBeam.trackChangesDuringPlaytime && volumetricLightBeam.enabled)
			{
				volumetricLightBeam.UpdateAfterManualPropertyChange();
				yield return null;
			}
			volumetricLightBeam.m_CoPlaytimeUpdate = null;
		}

		private void DestroyBeam()
		{
			if (this.m_BeamGeom)
			{
				UnityEngine.Object.DestroyImmediate(this.m_BeamGeom.gameObject);
			}
			this.m_BeamGeom = null;
		}

		[Obsolete("Use 'GenerateGeometry()' instead")]
		public void Generate()
		{
			this.GenerateGeometry();
		}

		public virtual void GenerateGeometry()
		{
			this.HandleBackwardCompatibility(this.pluginVersion, 1510);
			this.pluginVersion = 1510;
			this.ValidateProperties();
			if (this.m_BeamGeom == null)
			{
				Shader instance = Config.Instance.beamShader;
				if (!instance)
				{
					UnityEngine.Debug.LogError("Invalid BeamShader set in VLB Config");
					return;
				}
				this.m_BeamGeom = Utils.NewWithComponent<BeamGeometry>("Beam Geometry");
				this.m_BeamGeom.Initialize(this, instance);
			}
			this.m_BeamGeom.RegenerateMesh();
			this.m_BeamGeom.visible = base.enabled;
		}

		public float GetInsideBeamFactor(Vector3 posWS)
		{
			return this.GetInsideBeamFactorFromObjectSpacePos(base.transform.InverseTransformPoint(posWS));
		}

		public float GetInsideBeamFactorFromObjectSpacePos(Vector3 posOS)
		{
			if (posOS.z < 0f)
			{
				return -1f;
			}
			Vector2 vector2 = posOS.xy();
			vector2 = new Vector2(vector2.magnitude, posOS.z + this.coneApexOffsetZ);
			Vector2 vector21 = vector2.normalized;
			return Mathf.Clamp((Mathf.Abs(Mathf.Sin(this.coneAngle * 0.0174532924f / 2f)) - Mathf.Abs(vector21.x)) / 0.1f, -1f, 1f);
		}

		private void HandleBackwardCompatibility(int serializedVersion, int newVersion)
		{
			if (serializedVersion == -1)
			{
				return;
			}
			if (serializedVersion == newVersion)
			{
				return;
			}
			if (serializedVersion < 1301)
			{
				this.attenuationEquation = AttenuationEquation.Linear;
			}
			if (serializedVersion < 1501)
			{
				this.geomMeshType = MeshType.Custom;
				this.geomCustomSegments = 5;
			}
			Utils.MarkCurrentSceneDirty();
		}

		public bool IsColliderHiddenByDynamicOccluder(Collider collider)
		{
			UnityEngine.Debug.Assert(collider, "You should pass a valid Collider to VLB.VolumetricLightBeam.IsColliderHiddenByDynamicOccluder");
			if (!this.m_PlaneWS.IsValid())
			{
				return false;
			}
			return !GeometryUtility.TestPlanesAABB(new Plane[] { this.m_PlaneWS }, collider.bounds);
		}

		private void OnDestroy()
		{
			this.DestroyBeam();
		}

		private void OnDisable()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.visible = false;
			}
			this.m_CoPlaytimeUpdate = null;
		}

		private void OnEnable()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.visible = true;
			}
			this.StartPlaytimeUpdateIfNeeded();
		}

		public void SetClippingPlane(Plane planeWS)
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.SetClippingPlane(planeWS);
			}
			this.m_PlaneWS = planeWS;
		}

		public void SetClippingPlaneOff()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.SetClippingPlaneOff();
			}
			this.m_PlaneWS = new Plane();
		}

		private void Start()
		{
			this.GenerateGeometry();
		}

		private void StartPlaytimeUpdateIfNeeded()
		{
		}

		public virtual void UpdateAfterManualPropertyChange()
		{
			this.ValidateProperties();
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.UpdateMaterialAndBounds();
			}
		}

		private void ValidateProperties()
		{
			this.AssignPropertiesFromSpotLight(this.lightSpotAttached);
			this.ClampProperties();
		}
	}
}