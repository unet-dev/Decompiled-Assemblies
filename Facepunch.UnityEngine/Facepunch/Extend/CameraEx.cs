using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch.Extend
{
	public static class CameraEx
	{
		public static void FocusOnRenderer(this Camera cam, GameObject obj, Vector3 lookDirection, Vector3 Up, int layerMask = -1)
		{
			Vector3 vector3 = obj.transform.position;
			Quaternion quaternion = obj.transform.rotation;
			obj.transform.SetPositionAndRotation(Vector3.one, Quaternion.identity);
			obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			Bounds bound = new Bounds(Vector3.zero, Vector3.one * 0.01f);
			bool flag = true;
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i];
				if (renderer.enabled && renderer.gameObject.activeInHierarchy && !(renderer is ParticleSystemRenderer) && !renderer.gameObject.name.EndsWith("lod01", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod02", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod03", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod04", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod1", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod2", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod3", StringComparison.InvariantCultureIgnoreCase) && !renderer.gameObject.name.EndsWith("lod4", StringComparison.InvariantCultureIgnoreCase) && (layerMask & 1 << (renderer.gameObject.layer & 31)) != 0)
				{
					if (!flag)
					{
						bound.Encapsulate(renderer.bounds);
					}
					else
					{
						bound = renderer.bounds;
						flag = false;
					}
				}
			}
			Vector3 vector31 = bound.size;
			float single = vector31.magnitude * 0.33f / Mathf.Tan(cam.fieldOfView * 0.5f * 0.0174532924f);
			Matrix4x4 matrix4x4 = obj.transform.worldToLocalMatrix;
			Vector3 vector32 = matrix4x4.MultiplyPoint(bound.center);
			obj.transform.SetPositionAndRotation(vector3, quaternion);
			matrix4x4 = obj.transform.localToWorldMatrix;
			vector32 = matrix4x4.MultiplyPoint(vector32);
			cam.transform.position = vector32 + (obj.transform.TransformDirection(lookDirection.normalized) * single);
			cam.transform.LookAt(vector32, obj.transform.TransformDirection(Up.normalized));
		}

		public static void SaveScreenshot(this Camera cam, string name, int width, int height, bool transparent, int SuperSampleSize)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(width * SuperSampleSize, height * SuperSampleSize, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
			Color color = cam.backgroundColor;
			CameraClearFlags cameraClearFlag = cam.clearFlags;
			RenderTexture renderTexture = cam.targetTexture;
			int num = QualitySettings.antiAliasing;
			AnisotropicFiltering anisotropicFiltering = QualitySettings.anisotropicFiltering;
			bool flag = GL.sRGBWrite;
			GameObject gameObject = new GameObject();
			cam.forceIntoRenderTexture = true;
			cam.targetTexture = temporary;
			cam.aspect = 1f;
			cam.renderingPath = RenderingPath.UsePlayerSettings;
			cam.rect = new Rect(0f, 0f, 1f, 1f);
			cam.allowHDR = true;
			Texture.SetGlobalAnisotropicFilteringLimits(16, 16);
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			QualitySettings.antiAliasing = 8;
			if (transparent)
			{
				cam.clearFlags = CameraClearFlags.Depth;
				cam.backgroundColor = new Color(0f, 0f, 0f, 0f);
			}
			RenderTexture.active = temporary;
			GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
			GL.sRGBWrite = true;
			cam.Render();
			RenderTexture.active = null;
			RenderTexture.active = temporary;
			Texture2D texture2D = new Texture2D(temporary.width, temporary.height, TextureFormat.ARGB32, true);
			texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0, true);
			texture2D.filterMode = FilterMode.Trilinear;
			texture2D.anisoLevel = 32;
			RenderTexture.active = null;
			cam.targetTexture = renderTexture;
			QualitySettings.antiAliasing = num;
			QualitySettings.anisotropicFiltering = anisotropicFiltering;
			Texture.SetGlobalAnisotropicFilteringLimits(1, 16);
			if (SuperSampleSize != 1)
			{
				texture2D.Apply();
				RenderTexture temporary1 = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
				RenderTexture.active = temporary1;
				GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
				GL.sRGBWrite = true;
				Graphics.Blit(texture2D, temporary1);
				texture2D.Resize(width, height);
				texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
				RenderTexture.active = null;
				texture2D.Apply();
				RenderTexture.ReleaseTemporary(temporary1);
			}
			byte[] pNG = texture2D.EncodeToPNG();
			string directoryName = Path.GetDirectoryName(name);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllBytes(name, pNG);
			UnityEngine.Object.DestroyImmediate(texture2D, true);
			RenderTexture.ReleaseTemporary(temporary);
			UnityEngine.Object.DestroyImmediate(gameObject, true);
			if (transparent)
			{
				cam.clearFlags = cameraClearFlag;
				cam.backgroundColor = color;
			}
			GL.sRGBWrite = flag;
		}
	}
}