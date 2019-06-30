using Rust.Workshop;
using Steamworks.Ugc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Workshop.Import
{
	internal class ImportVersion3 : SingletonComponent<ImportVersion3>
	{
		public ImportVersion3()
		{
		}

		internal IEnumerator DoImport(Item item, Skin skin)
		{
			ImportVersion3 importVersion3 = null;
			WorkshopItemEditor.Loading(true, "Downloading..", "", 0f);
			yield return importVersion3.StartCoroutine(importVersion3.DownloadFromWorkshop(item));
			if (!item.IsInstalled || item.Directory == null)
			{
				UnityEngine.Debug.Log("Error opening item, not downloaded properly.");
				UnityEngine.Debug.Log(string.Concat("item.Directory: ", item.Directory));
				bool isInstalled = item.IsInstalled;
				UnityEngine.Debug.Log(string.Concat("item.Installed: ", isInstalled.ToString()));
				yield break;
			}
			WorkshopItemEditor.Loading(true, "Loading..", "Reloading Textures", 0f);
			yield return importVersion3.StartCoroutine(skin.LoadIcon(item.Id, item.Directory, null));
			yield return importVersion3.StartCoroutine(skin.LoadAssets(item.Id, item.Directory, null));
		}

		private IEnumerator DownloadFromWorkshop(Item item)
		{
			item.Download(true);
			while (item.IsDownloading)
			{
				yield return null;
			}
			while (!item.IsInstalled)
			{
				yield return null;
			}
		}
	}
}