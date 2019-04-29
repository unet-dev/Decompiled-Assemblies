using Facepunch.Extend;
using Facepunch.Steamworks;
using Facepunch.Utility;
using Rust;
using Rust.UI;
using Rust.Workshop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Workshop.Import
{
	internal class ImportVersion2 : SingletonComponent<ImportVersion2>
	{
		private AssetBundle Bundle;

		protected WorkshopInterface Interface
		{
			get
			{
				return base.GetComponentInParent<WorkshopInterface>();
			}
		}

		public ImportVersion2()
		{
		}

		private Texture2D ConvertMetalToSpec(Texture2D tex, Material outputMaterial)
		{
			tex = Facepunch.Utility.Texture.CreateReadableCopy(tex, 0, 0);
			for (int i = 0; i < tex.height; i++)
			{
				for (int j = 0; j < tex.width; j++)
				{
					UnityEngine.Color pixel = tex.GetPixel(j, i);
					if (pixel.a == 0f)
					{
						pixel.a = 0.007843138f;
					}
					UnityEngine.Color color = new UnityEngine.Color(pixel.r, pixel.r, pixel.r, pixel.a);
					tex.SetPixel(j, i, color);
				}
			}
			tex.Apply();
			outputMaterial.SetTexture("_SpecGlossMap", tex);
			outputMaterial.SetFloat("_Glossiness", 1f);
			outputMaterial.SetColor("_SpecColor", UnityEngine.Color.white);
			return tex;
		}

		internal void DoImport(Facepunch.Steamworks.Workshop.Item item, Skin skin, Action onImportFinished)
		{
			base.StartCoroutine(this.RunImport(item, skin, onImportFinished));
		}

		private Texture2D ImportTexture(string name, Material inputMaterial, Material outputMaterial, bool normal, Skin skin, int group, string targetName = null)
		{
			if (targetName == null)
			{
				targetName = name;
			}
			UnityEngine.Texture texture = outputMaterial.GetTexture(name);
			UnityEngine.Texture texture1 = inputMaterial.GetTexture(name);
			if (texture1 == null)
			{
				return null;
			}
			if (texture == null || texture1.name == texture.name)
			{
				return null;
			}
			texture1 = Facepunch.Utility.Texture.LimitSize(texture1 as Texture2D, skin.Skinnable.Groups[group].MaxTextureSize, skin.Skinnable.Groups[group].MaxTextureSize);
			outputMaterial.SetTexture(targetName, texture1);
			return texture1 as Texture2D;
		}

		public IEnumerator LoadItem(string Folder, string BundleName, Skin skin)
		{
			ImportVersion2 importVersion2 = null;
			AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(BundleName);
			importVersion2.Interface.LoadingBar.Text = "Loading Asset Bundle";
			yield return new WaitUntil(() => assetBundleCreateRequest.isDone);
			if (assetBundleCreateRequest.assetBundle == null)
			{
				UnityEngine.Debug.LogWarning("Asset bundle is null!");
				yield break;
			}
			importVersion2.Bundle = assetBundleCreateRequest.assetBundle;
			WorkshopSkinBase workshopSkinBase = assetBundleCreateRequest.assetBundle.LoadAsset<WorkshopSkinBase>("Meta.asset");
			if (workshopSkinBase == null)
			{
				string[] allAssetNames = assetBundleCreateRequest.assetBundle.GetAllAssetNames();
				for (int i = 0; i < (int)allAssetNames.Length; i++)
				{
					string str = allAssetNames[i];
					workshopSkinBase = assetBundleCreateRequest.assetBundle.LoadAsset<WorkshopSkinBase>(str);
					if (workshopSkinBase != null)
					{
						break;
					}
				}
			}
			if (workshopSkinBase == null)
			{
				Os.OpenFolder(Folder);
				importVersion2.Interface.LoadingBar.Text = "Couldn't Load";
				importVersion2.Interface.LoadingBar.SubText = "So we're going to open the folder so you can update it manually";
				yield return new WaitForSeconds(5f);
				yield break;
			}
			importVersion2.Interface.LoadingBar.Text = "SettingSkin";
			yield return importVersion2.StartCoroutine(importVersion2.ProcessMaterial(0, workshopSkinBase.skinMaterial0, skin));
			yield return importVersion2.StartCoroutine(importVersion2.ProcessMaterial(1, workshopSkinBase.skinMaterial1, skin));
			yield return importVersion2.StartCoroutine(importVersion2.ProcessMaterial(2, workshopSkinBase.skinMaterial2, skin));
			yield return importVersion2.StartCoroutine(importVersion2.ProcessMaterial(3, workshopSkinBase.skinMaterial3, skin));
		}

		public void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			if (this.Bundle != null)
			{
				this.Bundle.Unload(true);
				this.Bundle = null;
				UnityEngine.Debug.Log("CLEANUP BUNDLE");
			}
		}

		private IEnumerator ProcessMaterial(int v, Material inputMaterial, Skin skin)
		{
			ImportVersion2 importVersion2 = null;
			if (inputMaterial == null)
			{
				yield break;
			}
			if ((int)skin.Materials.Length <= v)
			{
				yield break;
			}
			importVersion2.Interface.LoadingBar.Text = string.Concat("Loading Material ", v + 1);
			yield return null;
			importVersion2.ImportTexture("_MainTex", inputMaterial, skin.Materials[v], false, skin, v, null);
			importVersion2.ImportTexture("_BumpMap", inputMaterial, skin.Materials[v], true, skin, v, null);
			importVersion2.ImportTexture("_OcclusionMap", inputMaterial, skin.Materials[v], false, skin, v, null);
			if (importVersion2.ImportTexture("_SpecGlossMap", inputMaterial, skin.Materials[v], false, skin, v, null) == null)
			{
				Texture2D spec = importVersion2.ImportTexture("_MetallicGlossMap", inputMaterial, skin.Materials[v], false, skin, v, "_SpecGlossMap");
				if (spec != null)
				{
					spec = importVersion2.ConvertMetalToSpec(spec, skin.Materials[v]);
				}
			}
		}

		private IEnumerator RunImport(Facepunch.Steamworks.Workshop.Item item, Skin skin, Action onImportFinished)
		{
			ImportVersion2 downloadProgress = null;
			if (downloadProgress.Bundle != null)
			{
				downloadProgress.Bundle.Unload(true);
				downloadProgress.Bundle = null;
			}
			downloadProgress.Interface.LoadingBar.Active = true;
			downloadProgress.Interface.LoadingBar.Text = "Downloading..";
			downloadProgress.Interface.LoadingBar.Progress = 0f;
			if (!item.Installed)
			{
				item.Download(true);
				while (item.Downloading)
				{
					downloadProgress.Interface.LoadingBar.Progress = (float)item.DownloadProgress;
					downloadProgress.Interface.LoadingBar.SubText = string.Format("{0} / {1}", item.BytesDownloaded.FormatBytes<ulong>(false), item.BytesTotalDownload.FormatBytes<ulong>(false));
					yield return null;
				}
				downloadProgress.Interface.LoadingBar.SubText = "";
				downloadProgress.Interface.LoadingBar.Text = "Installing..";
				while (!item.Installed)
				{
					downloadProgress.Interface.LoadingBar.Text = "Installing";
					yield return null;
				}
			}
			string str = string.Concat(item.Directory.FullName, "/bundle");
			if (!File.Exists(str))
			{
				UnityEngine.Debug.LogWarning("No Bundle Found!");
				Os.OpenFolder(item.Directory.FullName);
				downloadProgress.Interface.LoadingBar.Text = "Couldn't Load";
				downloadProgress.Interface.LoadingBar.SubText = "So we're going to open the folder so you can update it manually";
				yield return new WaitForSeconds(5f);
			}
			else
			{
				downloadProgress.Interface.LoadingBar.Text = "Loading Item";
				yield return downloadProgress.StartCoroutine(downloadProgress.LoadItem(item.Directory.FullName, str, skin));
			}
			onImportFinished();
			downloadProgress.Interface.LoadingBar.Active = false;
		}
	}
}