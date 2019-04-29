using Facepunch.Extend;
using Facepunch.Steamworks;
using Rust.UI;
using Rust.Workshop;
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
		protected WorkshopInterface Interface
		{
			get
			{
				return base.GetComponentInParent<WorkshopInterface>();
			}
		}

		public ImportVersion3()
		{
		}

		internal void DoImport(Facepunch.Steamworks.Workshop.Item item, Skin skin, Action OnImportFinished)
		{
			base.StartCoroutine(this.StartImport(item, skin, OnImportFinished));
		}

		private IEnumerator DownloadFromWorkshop(Facepunch.Steamworks.Workshop.Item item)
		{
			ImportVersion3 importVersion3 = null;
			item.Download(true);
			float single = 0f;
			float single1 = 0f;
			while (item.Downloading)
			{
				single = Mathf.Lerp(single, (float)item.DownloadProgress, Time.deltaTime * 7f);
				importVersion3.Interface.LoadingBar.Progress = single;
				ulong bytesDownloaded = item.BytesDownloaded;
				ulong bytesTotalDownload = item.BytesTotalDownload;
				single1 = Mathf.Lerp(single1, (float)((float)item.BytesDownloaded), Time.deltaTime * 7f);
				if (bytesTotalDownload <= (long)0)
				{
					importVersion3.Interface.LoadingBar.SubText = "";
				}
				else
				{
					importVersion3.Interface.LoadingBar.SubText = string.Format("{0} / {1}", ((ulong)single1).FormatBytes<ulong>(false), bytesTotalDownload.FormatBytes<ulong>(false));
				}
				yield return null;
			}
			while (!item.Installed)
			{
				importVersion3.Interface.LoadingBar.SubText = "Installing";
				yield return null;
			}
		}

		private IEnumerator StartImport(Facepunch.Steamworks.Workshop.Item item, Skin skin, Action OnImportFinished)
		{
			ImportVersion3 importVersion3 = null;
			importVersion3.Interface.LoadingBar.Active = true;
			importVersion3.Interface.LoadingBar.Text = "Downloading..";
			importVersion3.Interface.LoadingBar.SubText = "";
			importVersion3.Interface.LoadingBar.Progress = 0f;
			yield return importVersion3.StartCoroutine(importVersion3.DownloadFromWorkshop(item));
			if (!item.Installed || item.Directory == null)
			{
				UnityEngine.Debug.Log("Error opening item, not downloaded properly.");
				UnityEngine.Debug.Log(string.Concat("item.Directory: ", item.Directory));
				bool installed = item.Installed;
				UnityEngine.Debug.Log(string.Concat("item.Installed: ", installed.ToString()));
				importVersion3.Interface.OpenMenu();
				importVersion3.Interface.LoadingBar.Active = false;
				yield break;
			}
			importVersion3.Interface.LoadingBar.Text = "Loading..";
			importVersion3.Interface.LoadingBar.SubText = "Reading Textures";
			yield return importVersion3.StartCoroutine(skin.FromFolder(item.Id, item.Directory, null));
			importVersion3.Interface.LoadingBar.Active = false;
			OnImportFinished();
		}
	}
}