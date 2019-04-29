using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace VLB
{
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
	public class BeamGeometry : MonoBehaviour
	{
		private VolumetricLightBeam m_Master;

		private Matrix4x4 m_ColorGradientMatrix;

		private MeshType m_CurrentMeshType;

		public Mesh coneMesh
		{
			get;
			private set;
		}

		public Material material
		{
			get;
			private set;
		}

		public MeshFilter meshFilter
		{
			get;
			private set;
		}

		public MeshRenderer meshRenderer
		{
			get;
			private set;
		}

		public int sortingLayerID
		{
			get
			{
				return this.meshRenderer.sortingLayerID;
			}
			set
			{
				this.meshRenderer.sortingLayerID = value;
			}
		}

		public int sortingOrder
		{
			get
			{
				return this.meshRenderer.sortingOrder;
			}
			set
			{
				this.meshRenderer.sortingOrder = value;
			}
		}

		public bool visible
		{
			get
			{
				return this.meshRenderer.enabled;
			}
			set
			{
				this.meshRenderer.enabled = value;
			}
		}

		public BeamGeometry()
		{
		}

		private void ComputeLocalMatrix()
		{
			float single = Mathf.Max(this.m_Master.coneRadiusStart, this.m_Master.coneRadiusEnd);
			base.transform.localScale = new Vector3(single, single, this.m_Master.fadeEnd);
		}

		public void Initialize(VolumetricLightBeam master, Shader shader)
		{
			HideFlags proceduralObjectsHideFlags = Consts.ProceduralObjectsHideFlags;
			this.m_Master = master;
			base.transform.SetParent(master.transform, false);
			this.material = new Material(shader)
			{
				hideFlags = proceduralObjectsHideFlags
			};
			this.meshRenderer = base.gameObject.GetOrAddComponent<MeshRenderer>();
			this.meshRenderer.hideFlags = proceduralObjectsHideFlags;
			this.meshRenderer.material = this.material;
			this.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			this.meshRenderer.receiveShadows = false;
			this.meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			if (!SortingLayer.IsValid(this.m_Master.sortingLayerID))
			{
				Debug.LogError(string.Format("Beam '{0}' has an invalid sortingLayerID ({1}). Please fix it by setting a valid layer.", Utils.GetPath(this.m_Master.transform), this.m_Master.sortingLayerID));
			}
			else
			{
				this.sortingLayerID = this.m_Master.sortingLayerID;
			}
			this.sortingOrder = this.m_Master.sortingOrder;
			this.meshFilter = base.gameObject.GetOrAddComponent<MeshFilter>();
			this.meshFilter.hideFlags = proceduralObjectsHideFlags;
			base.gameObject.hideFlags = proceduralObjectsHideFlags;
		}

		private static bool IsUsingCustomRenderPipeline()
		{
			if (RenderPipelineManager.currentPipeline != null)
			{
				return true;
			}
			return GraphicsSettings.renderPipelineAsset != null;
		}

		private void OnBeginCameraRendering(Camera cam)
		{
			this.UpdateCameraRelatedProperties(cam);
		}

		private void OnDestroy()
		{
			if (this.material)
			{
				UnityEngine.Object.DestroyImmediate(this.material);
				this.material = null;
			}
		}

		private void OnDisable()
		{
			if (BeamGeometry.IsUsingCustomRenderPipeline())
			{
				RenderPipeline.beginCameraRendering -= new Action<Camera>(this.OnBeginCameraRendering);
			}
		}

		private void OnEnable()
		{
			if (BeamGeometry.IsUsingCustomRenderPipeline())
			{
				RenderPipeline.beginCameraRendering += new Action<Camera>(this.OnBeginCameraRendering);
			}
		}

		private void OnWillRenderObject()
		{
			if (!BeamGeometry.IsUsingCustomRenderPipeline())
			{
				Camera camera = Camera.current;
				if (camera != null)
				{
					this.UpdateCameraRelatedProperties(camera);
				}
			}
		}

		public void RegenerateMesh()
		{
			Debug.Assert(this.m_Master);
			base.gameObject.layer = Config.Instance.geometryLayerID;
			base.gameObject.tag = Config.Instance.geometryTag;
			if (this.coneMesh && this.m_CurrentMeshType == MeshType.Custom)
			{
				UnityEngine.Object.DestroyImmediate(this.coneMesh);
			}
			this.m_CurrentMeshType = this.m_Master.geomMeshType;
			MeshType mMaster = this.m_Master.geomMeshType;
			if (mMaster == MeshType.Shared)
			{
				this.coneMesh = GlobalMesh.mesh;
				this.meshFilter.sharedMesh = this.coneMesh;
			}
			else if (mMaster != MeshType.Custom)
			{
				Debug.LogError("Unsupported MeshType");
			}
			else
			{
				this.coneMesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, this.m_Master.geomCustomSides, this.m_Master.geomCustomSegments, this.m_Master.geomCap);
				this.coneMesh.hideFlags = Consts.ProceduralObjectsHideFlags;
				this.meshFilter.mesh = this.coneMesh;
			}
			this.UpdateMaterialAndBounds();
		}

		public void SetClippingPlane(Plane planeWS)
		{
			Vector3 vector3 = planeWS.normal;
			this.material.EnableKeyword("VLB_CLIPPING_PLANE");
			this.material.SetVector("_ClippingPlaneWS", new Vector4(vector3.x, vector3.y, vector3.z, planeWS.distance));
		}

		public void SetClippingPlaneOff()
		{
			this.material.DisableKeyword("VLB_CLIPPING_PLANE");
		}

		private void Start()
		{
		}

		private void UpdateCameraRelatedProperties(Camera cam)
		{
			if (cam && this.m_Master)
			{
				if (this.material)
				{
					Vector3 vector3 = this.m_Master.transform.InverseTransformPoint(cam.transform.position);
					this.material.SetVector("_CameraPosObjectSpace", vector3);
					Vector3 vector31 = base.transform.InverseTransformDirection(cam.transform.forward);
					Vector3 vector32 = vector31.normalized;
					float single = (cam.orthographic ? -1f : this.m_Master.GetInsideBeamFactorFromObjectSpacePos(vector3));
					this.material.SetVector("_CameraParams", new Vector4(vector32.x, vector32.y, vector32.z, single));
					if (this.m_Master.colorMode == ColorMode.Gradient)
					{
						this.material.SetMatrix("_ColorGradientMatrix", this.m_ColorGradientMatrix);
					}
				}
				if (this.m_Master.depthBlendDistance > 0f)
				{
					Camera camera = cam;
					camera.depthTextureMode = camera.depthTextureMode | DepthTextureMode.Depth;
				}
			}
		}

		public void UpdateMaterialAndBounds()
		{
			object obj;
			Debug.Assert(this.m_Master);
			this.material.renderQueue = Config.Instance.geometryRenderQueue;
			float mMaster = this.m_Master.coneAngle * 0.0174532924f / 2f;
			this.material.SetVector("_ConeSlopeCosSin", new Vector2(Mathf.Cos(mMaster), Mathf.Sin(mMaster)));
			Vector2 vector2 = new Vector2(Mathf.Max(this.m_Master.coneRadiusStart, 0.0001f), Mathf.Max(this.m_Master.coneRadiusEnd, 0.0001f));
			this.material.SetVector("_ConeRadius", vector2);
			float single = Mathf.Sign(this.m_Master.coneApexOffsetZ) * Mathf.Max(Mathf.Abs(this.m_Master.coneApexOffsetZ), 0.0001f);
			this.material.SetFloat("_ConeApexOffsetZ", single);
			if (this.m_Master.colorMode != ColorMode.Gradient)
			{
				this.material.DisableKeyword("VLB_COLOR_GRADIENT_MATRIX_HIGH");
				this.material.DisableKeyword("VLB_COLOR_GRADIENT_MATRIX_LOW");
				this.material.SetColor("_ColorFlat", this.m_Master.color);
			}
			else
			{
				Utils.FloatPackingPrecision floatPackingPrecision = Utils.GetFloatPackingPrecision();
				this.material.EnableKeyword((floatPackingPrecision == Utils.FloatPackingPrecision.High ? "VLB_COLOR_GRADIENT_MATRIX_HIGH" : "VLB_COLOR_GRADIENT_MATRIX_LOW"));
				this.m_ColorGradientMatrix = this.m_Master.colorGradient.SampleInMatrix((int)floatPackingPrecision);
			}
			if (!Consts.BlendingMode_AlphaAsBlack[this.m_Master.blendingModeAsInt])
			{
				this.material.DisableKeyword("ALPHA_AS_BLACK");
			}
			else
			{
				this.material.EnableKeyword("ALPHA_AS_BLACK");
			}
			this.material.SetInt("_BlendSrcFactor", (int)Consts.BlendingMode_SrcFactor[this.m_Master.blendingModeAsInt]);
			this.material.SetInt("_BlendDstFactor", (int)Consts.BlendingMode_DstFactor[this.m_Master.blendingModeAsInt]);
			this.material.SetFloat("_AlphaInside", this.m_Master.alphaInside);
			this.material.SetFloat("_AlphaOutside", this.m_Master.alphaOutside);
			this.material.SetFloat("_AttenuationLerpLinearQuad", this.m_Master.attenuationLerpLinearQuad);
			this.material.SetFloat("_DistanceFadeStart", this.m_Master.fadeStart);
			this.material.SetFloat("_DistanceFadeEnd", this.m_Master.fadeEnd);
			this.material.SetFloat("_DistanceCamClipping", this.m_Master.cameraClippingDistance);
			this.material.SetFloat("_FresnelPow", Mathf.Max(0.001f, this.m_Master.fresnelPow));
			this.material.SetFloat("_GlareBehind", this.m_Master.glareBehind);
			this.material.SetFloat("_GlareFrontal", this.m_Master.glareFrontal);
			Material material = this.material;
			if (this.m_Master.geomCap)
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			material.SetFloat("_DrawCap", (float)obj);
			if (this.m_Master.depthBlendDistance <= 0f)
			{
				this.material.DisableKeyword("VLB_DEPTH_BLEND");
			}
			else
			{
				this.material.EnableKeyword("VLB_DEPTH_BLEND");
				this.material.SetFloat("_DepthBlendDistance", this.m_Master.depthBlendDistance);
			}
			if (!this.m_Master.noiseEnabled || this.m_Master.noiseIntensity <= 0f || !Noise3D.isSupported)
			{
				this.material.DisableKeyword("VLB_NOISE_3D");
			}
			else
			{
				Noise3D.LoadIfNeeded();
				this.material.EnableKeyword("VLB_NOISE_3D");
				this.material.SetVector("_NoiseLocal", new Vector4(this.m_Master.noiseVelocityLocal.x, this.m_Master.noiseVelocityLocal.y, this.m_Master.noiseVelocityLocal.z, this.m_Master.noiseScaleLocal));
				this.material.SetVector("_NoiseParam", new Vector3(this.m_Master.noiseIntensity, (this.m_Master.noiseVelocityUseGlobal ? 1f : 0f), (this.m_Master.noiseScaleUseGlobal ? 1f : 0f)));
			}
			this.ComputeLocalMatrix();
		}
	}
}