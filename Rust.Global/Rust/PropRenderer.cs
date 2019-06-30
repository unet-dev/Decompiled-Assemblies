using Facepunch.Extend;
using Rust.Components.Camera;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rust
{
	public class PropRenderer : MonoBehaviour, IClientComponent
	{
		public bool HideLowLods = true;

		public bool HideUnskinnable = true;

		public bool Outline = true;

		[Header("Rotation")]
		public Vector3 Rotation = Vector3.zero;

		public Vector3 PostRotation = Vector3.zero;

		[Header("Position Offset")]
		public Vector3 PositionalTweak = Vector3.zero;

		[Header("Misc")]
		public float FieldOfView = 40f;

		public Vector3 LookDirection = new Vector3(0.2f, 0.4f, 1f);

		public Vector3 UpDirection = Vector3.up;

		public PropRenderer()
		{
		}

		[ContextMenu("Create 512x512 Icon")]
		public void CreateIcon()
		{
			GameObject gameObject = base.gameObject;
			Debug.Log(string.Concat(new string[] { "Saving To ", UnityEngine.Application.dataPath, "/", gameObject.name, ".icon.png" }));
			this.CreateScreenshot(string.Concat(UnityEngine.Application.dataPath, "/", gameObject.name, ".icon.png"), 512, 512, 4, null);
		}

		[ContextMenu("Create Large Render")]
		public void CreateRender()
		{
			GameObject gameObject = base.gameObject;
			Debug.Log(string.Concat(new string[] { "Saving To ", UnityEngine.Application.dataPath, "/", gameObject.name, ".large.png" }));
			this.CreateScreenshot(string.Concat(UnityEngine.Application.dataPath, "/", gameObject.name, ".large.png"), 4096, 4096, 1, null);
		}

		public void CreateScreenshot(string filename, int width, int height, int SuperSampleSize = 1, PropRenderer.LightIntensityScale lightIntensityScale = null)
		{
			if (lightIntensityScale == null)
			{
				lightIntensityScale = new PropRenderer.LightIntensityScale(PropRenderer.DefaultLightIntensityScale);
			}
			GameObject gameObject = new GameObject("Temporary Camera");
			Camera color = gameObject.AddComponent<Camera>();
			color.clearFlags = CameraClearFlags.Depth;
			color.backgroundColor = new Color(0f, 0f, 0f, 0f);
			color.allowHDR = true;
			color.allowMSAA = false;
			Type type = Type.GetType("DeferredExtension,Assembly-CSharp");
			if (type != null)
			{
				gameObject.AddComponent(type);
			}
			if (RenderSettings.customReflection != null)
			{
				Shader.SetGlobalTexture("global_SkyReflection", RenderSettings.customReflection);
				Shader.SetGlobalVector("global_SkyReflection_HDR", new Vector2(0.2f, 0.01f));
			}
			if (this.Outline)
			{
				gameObject.AddComponent<IconOutline>();
			}
			LightingOverride lightingOverride = gameObject.AddComponent<LightingOverride>();
			lightingOverride.overrideAmbientLight = true;
			lightingOverride.ambientMode = AmbientMode.Flat;
			lightingOverride.ambientLight = new Color(0.4f, 0.4f, 0.4f, 1f);
			GameObject gameObject1 = new GameObject("Temporary Light");
			gameObject1.transform.SetParent(color.transform);
			gameObject1.transform.localRotation = Quaternion.Euler(115f, 60f, 0f);
			Light light = gameObject1.AddComponent<Light>();
			light.type = LightType.Directional;
			light.color = new Color(1f, 1f, 0.96f);
			light.intensity = 2f;
			light.cullingMask = 524288;
			light.shadows = LightShadows.Soft;
			light.shadowBias = 0.01f;
			light.shadowNormalBias = 0.01f;
			light.shadowStrength = 0.9f;
			if (lightIntensityScale != null)
			{
				light.intensity = lightIntensityScale(light.intensity);
			}
			GameObject gameObject2 = new GameObject("Temporary Light");
			gameObject2.transform.SetParent(color.transform);
			gameObject2.transform.localRotation = Quaternion.Euler(5f, -35f, 0f);
			Light color1 = gameObject2.AddComponent<Light>();
			color1.type = LightType.Directional;
			color1.color = new Color(1f, 1f, 1f);
			color1.intensity = 1f;
			color1.cullingMask = 524288;
			color1.shadows = LightShadows.Soft;
			color1.shadowBias = 0.01f;
			color1.shadowNormalBias = 0.01f;
			color1.shadowStrength = 0.9f;
			if (lightIntensityScale != null)
			{
				color1.intensity = lightIntensityScale(color1.intensity);
			}
			this.PreRender();
			try
			{
				color.cullingMask = 524288;
				this.PositionCamera(color);
				color.SaveScreenshot(filename, width, height, true, SuperSampleSize);
			}
			finally
			{
				UnityEngine.Object.DestroyImmediate(gameObject);
				this.PostRender();
			}
		}

		public void DebugAlign()
		{
			this.PreRender();
			Camera fieldOfView = Camera.main;
			fieldOfView.fieldOfView = this.FieldOfView;
			this.PositionCamera(fieldOfView);
			this.PostRender();
		}

		private static float DefaultLightIntensityScale(float intensity)
		{
			return Mathf.GammaToLinearSpace(intensity) * 3.14159274f;
		}

		public void PositionCamera(Camera cam)
		{
			Vector3 vector3 = Quaternion.Euler(this.Rotation) * this.LookDirection.normalized;
			Vector3 vector31 = Quaternion.Euler(this.Rotation) * this.UpDirection.normalized;
			vector3 = Quaternion.Euler(this.PostRotation) * vector3;
			vector31 = Quaternion.Euler(this.PostRotation) * vector31;
			cam.fieldOfView = this.FieldOfView;
			cam.nearClipPlane = 0.01f;
			cam.farClipPlane = 100f;
			cam.FocusOnRenderer(base.gameObject, vector3, vector31, 524288);
			Transform positionalTweak = cam.transform;
			positionalTweak.position = positionalTweak.position + ((this.PositionalTweak.x * cam.transform.right) * 0.01f);
			Transform transforms = cam.transform;
			transforms.position = transforms.position + ((this.PositionalTweak.y * cam.transform.up) * 0.01f);
			Transform positionalTweak1 = cam.transform;
			positionalTweak1.position = positionalTweak1.position + ((this.PositionalTweak.z * cam.transform.forward) * 0.1f);
		}

		public void PostRender()
		{
			Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i];
				if (!(renderer is ParticleSystemRenderer))
				{
					renderer.gameObject.layer = 0;
					SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
					if (skinnedMeshRenderer)
					{
						skinnedMeshRenderer.updateWhenOffscreen = false;
					}
				}
			}
		}

		public void PreRender()
		{
			Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i];
				if (!(renderer is ParticleSystemRenderer) && (!this.HideLowLods || !renderer.gameObject.name.EndsWith("lod01", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod02", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod03", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod04", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod1", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod2", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod3", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod4", StringComparison.InvariantCultureIgnoreCase)))
				{
					bool hideUnskinnable = this.HideUnskinnable;
					renderer.gameObject.layer = 19;
					SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
					if (skinnedMeshRenderer)
					{
						skinnedMeshRenderer.updateWhenOffscreen = true;
					}
				}
			}
		}

		public static bool RenderScreenshot(GameObject prefab, string filename, int width, int height, int SuperSampleSize = 1)
		{
			if (prefab == null)
			{
				Debug.Log("RenderScreenshot - prefab is null", prefab);
				return false;
			}
			PropRenderer propRenderer = null;
			PropRenderer component = prefab.GetComponent<PropRenderer>();
			if (component == null)
			{
				propRenderer = prefab.AddComponent<PropRenderer>();
				component = propRenderer;
			}
			component.CreateScreenshot(filename, width, height, SuperSampleSize, null);
			if (propRenderer != null)
			{
				UnityEngine.Object.DestroyImmediate(propRenderer);
			}
			return true;
		}

		public delegate float LightIntensityScale(float intensity);
	}
}