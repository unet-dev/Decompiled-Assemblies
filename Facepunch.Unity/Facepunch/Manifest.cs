using Facepunch.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace Facepunch
{
	public static class Manifest
	{
		public static string Contents
		{
			get;
			internal set;
		}

		public static DateTime LastDownloaded
		{
			get;
			internal set;
		}

		internal static void Download()
		{
			if (!string.IsNullOrEmpty(Facepunch.Application.Integration.PublicKey))
			{
				Facepunch.Application.Controller.StartCoroutine(Facepunch.Manifest.UpdateManifest());
				return;
			}
			if (Facepunch.Application.Integration.DebugOutput)
			{
				UnityEngine.Debug.LogWarning("[manifest] Not downloading manifest - no public key");
			}
		}

		private static void LoadManifest(string text)
		{
			Facepunch.Manifest.LastDownloaded = DateTime.UtcNow;
			Facepunch.Manifest.Contents = text;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (Facepunch.Application.Integration.DebugOutput)
			{
				UnityEngine.Debug.Log("[Manifest] Loading Manifest..");
			}
			try
			{
				Facepunch.Application.Manifest = Facepunch.Models.Manifest.FromJson(text);
				Facepunch.Manifest.OnManifestLoaded(Facepunch.Application.Manifest);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				UnityEngine.Debug.LogWarning(string.Concat("Exception when reading manifest (", exception.Message, ")"));
			}
		}

		private static void OnManifestLoaded(Facepunch.Models.Manifest manifest)
		{
			UnityEngine.Assertions.Assert.IsNotNull<Facepunch.Models.Manifest>(manifest);
			Facepunch.Application.Integration.OnManifestFile(manifest);
			if ((!UnityEngine.Application.isEditor || !Facepunch.Application.Integration.RestrictEditorFunctionality) && Facepunch.Application.Analytics == null && !string.IsNullOrEmpty(manifest.AnalyticUrl))
			{
				if (string.IsNullOrEmpty(Facepunch.Application.Integration.UserId))
				{
					if (Facepunch.Application.Integration.DebugOutput)
					{
						UnityEngine.Debug.LogWarning("[Analytics] Skipping Analytics because userid is unset");
					}
					return;
				}
				Facepunch.Application.Analytics = new Analytics(manifest.AnalyticUrl);
			}
		}

		private static IEnumerator UpdateManifest()
		{
			string str1 = string.Concat(Facepunch.Application.Integration.ApiUrl, "public/manifest/?public_key=", Facepunch.Application.Integration.PublicKey);
			if (Facepunch.Application.Integration.LocalApi)
			{
				str1 = string.Concat("http://localhost:555/api/public/manifest/?public_key=", Facepunch.Application.Integration.PublicKey);
			}
			while (true)
			{
				if (Facepunch.Application.Integration.DebugOutput)
				{
					UnityEngine.Debug.Log(string.Concat("[Manifest] Fetching from \"", str1, "\""));
				}
				Uri uri = new Uri(str1, UriKind.Absolute);
				UnityEngine.Debug.Log(string.Format("[Manifest] URI IS \"{0}\"", uri));
				string str2 = str1;
				WebUtil.Get(str2, (string str) => {
					if (!string.IsNullOrEmpty(str))
					{
						Facepunch.Manifest.LoadManifest(str);
						return;
					}
					if (Facepunch.Application.Integration.DebugOutput)
					{
						UnityEngine.Debug.Log("[Manifest] Empty Response, bailing.");
					}
				});
				yield return new WaitForSecondsRealtime(3600f);
			}
		}
	}
}