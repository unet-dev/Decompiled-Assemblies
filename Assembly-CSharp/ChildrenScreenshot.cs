using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ChildrenScreenshot : MonoBehaviour
{
	public Vector3 offsetAngle = new Vector3(0f, 0f, 1f);

	public int width = 512;

	public int height = 512;

	public float fieldOfView = 70f;

	[Tooltip("0 = full recursive name, 1 = object name")]
	public string folder = "screenshots/{0}.png";

	public ChildrenScreenshot()
	{
	}

	[ContextMenu("Create Screenshots")]
	public void CreateScreenshots()
	{
		RenderTexture renderTexture = new RenderTexture(this.width, this.height, 0);
		GameObject gameObject = new GameObject();
		Camera mask = gameObject.AddComponent<Camera>();
		mask.targetTexture = renderTexture;
		mask.orthographic = false;
		mask.fieldOfView = this.fieldOfView;
		mask.nearClipPlane = 0.1f;
		mask.farClipPlane = 2000f;
		mask.cullingMask = LayerMask.GetMask(new string[] { "TransparentFX" });
		mask.clearFlags = CameraClearFlags.Color;
		mask.backgroundColor = new Color(0f, 0f, 0f, 0f);
		mask.renderingPath = RenderingPath.DeferredShading;
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
		foreach (Transform transforms in base.transform.Cast<Transform>())
		{
			this.PositionCamera(mask, transforms.gameObject);
			int num = transforms.gameObject.layer;
			transforms.gameObject.SetLayerRecursive(1);
			mask.Render();
			transforms.gameObject.SetLayerRecursive(num);
			string recursiveName = transforms.GetRecursiveName("");
			recursiveName = recursiveName.Replace('/', '.');
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0, false);
			RenderTexture.active = null;
			byte[] pNG = texture2D.EncodeToPNG();
			string str = string.Format(this.folder, recursiveName, transforms.name);
			string directoryName = Path.GetDirectoryName(str);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllBytes(str, pNG);
		}
		UnityEngine.Object.DestroyImmediate(texture2D, true);
		UnityEngine.Object.DestroyImmediate(renderTexture, true);
		UnityEngine.Object.DestroyImmediate(gameObject, true);
	}

	public void PositionCamera(Camera cam, GameObject obj)
	{
		Bounds bound = new Bounds(obj.transform.position, Vector3.zero * 0.1f);
		bool flag = true;
		Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			Renderer renderer = componentsInChildren[i];
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
		Vector3 vector3 = bound.size;
		float single = vector3.magnitude * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * 0.0174532924f);
		cam.transform.position = bound.center + (obj.transform.TransformVector(this.offsetAngle.normalized) * single);
		cam.transform.LookAt(bound.center);
	}
}