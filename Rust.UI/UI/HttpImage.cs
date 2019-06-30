using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Rust.UI
{
	[RequireComponent(typeof(RawImage))]
	public class HttpImage : MonoBehaviour
	{
		public static Dictionary<string, UnityWebRequest> RequestCache;

		public static Dictionary<string, Texture2D> TextureCache;

		public Texture2D LoadingImage;

		public Texture2D MissingImage;

		private RawImage image;

		public string Url = "";

		public bool PreserveAspectHeight;

		static HttpImage()
		{
			HttpImage.RequestCache = new Dictionary<string, UnityWebRequest>();
			HttpImage.TextureCache = new Dictionary<string, Texture2D>();
		}

		public HttpImage()
		{
		}

		private void Init()
		{
			this.image = base.GetComponent<RawImage>();
			this.image.enabled = false;
		}

		public bool Load(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return false;
			}
			if (this.Url == url)
			{
				return false;
			}
			if (this.image == null)
			{
				this.Init();
			}
			this.UpdateImageTexture(this.LoadingImage);
			this.Url = url;
			if (HttpImage.TextureCache.ContainsKey(url))
			{
				this.image.texture = HttpImage.TextureCache[url];
				this.image.enabled = true;
				return false;
			}
			if (!base.isActiveAndEnabled)
			{
				return false;
			}
			if (HttpImage.RequestCache.ContainsKey(url))
			{
				this.image.enabled = false;
				Global.Runner.StartCoroutine(this.WaitForLoad(url));
				return false;
			}
			this.image.enabled = false;
			Global.Runner.StartCoroutine(this.StartAndWaitForLoad(url));
			return true;
		}

		private void OnEnable()
		{
			this.Init();
			if (this.Url != string.Empty)
			{
				string url = this.Url;
				this.Url = string.Empty;
				this.Load(url);
			}
		}

		private IEnumerator StartAndWaitForLoad(string url)
		{
			HttpImage httpImage = null;
			UnityWebRequest unityWebRequest = new UnityWebRequest(url)
			{
				downloadHandler = new DownloadHandlerBuffer()
			};
			HttpImage.RequestCache.Add(url, unityWebRequest);
			yield return unityWebRequest.SendWebRequest();
			HttpImage.RequestCache.Remove(url);
			Texture2D texture2D = null;
			if (unityWebRequest.isDone && !unityWebRequest.isHttpError && !unityWebRequest.isNetworkError)
			{
				texture2D = new Texture2D(16, 16)
				{
					name = url
				};
				if (!ImageConversion.LoadImage(texture2D, unityWebRequest.downloadHandler.data, true))
				{
					UnityEngine.Object.DestroyImmediate(texture2D);
					texture2D = null;
				}
			}
			if (texture2D == null)
			{
				texture2D = httpImage.MissingImage;
				if (texture2D == null)
				{
					texture2D = Texture2D.blackTexture;
				}
			}
			HttpImage.TextureCache.Add(url, texture2D);
			unityWebRequest.Dispose();
			if (httpImage && httpImage.image)
			{
				httpImage.UpdateImageTexture(texture2D);
				if (httpImage.PreserveAspectHeight)
				{
					float single = (float)texture2D.height / (float)texture2D.width;
					RectTransform vector2 = httpImage.image.rectTransform;
					Rect rect = httpImage.image.rectTransform.rect;
					vector2.sizeDelta = new Vector2(0f, rect.width * single);
					httpImage.image.enabled = true;
				}
			}
		}

		private void UpdateImageTexture(Texture2D tex)
		{
			if (tex == null)
			{
				return;
			}
			if (this && this.image)
			{
				this.image.texture = tex;
				this.image.enabled = true;
			}
		}

		private IEnumerator WaitForLoad(string url)
		{
			HttpImage httpImage = null;
			while (!HttpImage.TextureCache.ContainsKey(url))
			{
				yield return null;
			}
			if (httpImage && httpImage.image)
			{
				httpImage.UpdateImageTexture(HttpImage.TextureCache[url]);
			}
		}
	}
}