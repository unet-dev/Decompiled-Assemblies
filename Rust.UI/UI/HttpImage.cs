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

		private RawImage image;

		public string Url = "";

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

		public void Load(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return;
			}
			if (this.Url == url)
			{
				return;
			}
			if (this.image == null)
			{
				this.Init();
			}
			this.Url = url;
			if (HttpImage.TextureCache.ContainsKey(url))
			{
				this.image.texture = HttpImage.TextureCache[url];
				this.image.enabled = true;
				return;
			}
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (HttpImage.RequestCache.ContainsKey(url))
			{
				this.image.enabled = false;
				Global.Runner.StartCoroutine(this.WaitForLoad(url));
				return;
			}
			this.image.enabled = false;
			Global.Runner.StartCoroutine(this.StartAndWaitForLoad(url));
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
			HttpImage item = null;
			UnityWebRequest unityWebRequest = new UnityWebRequest(url)
			{
				downloadHandler = new DownloadHandlerBuffer()
			};
			HttpImage.RequestCache.Add(url, unityWebRequest);
			yield return unityWebRequest.Send();
			HttpImage.RequestCache.Remove(url);
			Texture2D texture2D = null;
			if (unityWebRequest.isDone && !unityWebRequest.isHttpError && !unityWebRequest.isNetworkError)
			{
				texture2D = new Texture2D(16, 16);
				texture2D.LoadImage(unityWebRequest.downloadHandler.data);
			}
			if (texture2D == null)
			{
				texture2D = Texture2D.blackTexture;
			}
			HttpImage.TextureCache.Add(url, texture2D);
			unityWebRequest.Dispose();
			if (item && item.image)
			{
				item.image.texture = HttpImage.TextureCache[url];
				item.image.enabled = true;
			}
		}

		private IEnumerator WaitForLoad(string url)
		{
			HttpImage item = null;
			while (!HttpImage.TextureCache.ContainsKey(url))
			{
				yield return null;
			}
			if (item && item.image)
			{
				item.image.texture = HttpImage.TextureCache[url];
				item.image.enabled = true;
			}
		}
	}
}