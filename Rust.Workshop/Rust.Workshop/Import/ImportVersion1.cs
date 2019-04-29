using Facepunch.Extend;
using Facepunch.Steamworks;
using Facepunch.Utility;
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
	internal class ImportVersion1 : SingletonComponent<ImportVersion1>
	{
		protected WorkshopInterface Interface
		{
			get
			{
				return base.GetComponentInParent<WorkshopInterface>();
			}
		}

		public ImportVersion1()
		{
		}

		internal void DoImport(Facepunch.Steamworks.Workshop.Item item, Skin skin, Action onImportFinished)
		{
			base.StartCoroutine(this.RunImport(item, skin, onImportFinished));
		}

		private IEnumerator RunImport(Facepunch.Steamworks.Workshop.Item item, Skin skin, Action onImportFinished)
		{
			ImportVersion1 downloadProgress = null;
			downloadProgress.Interface.LoadingBar.Active = true;
			downloadProgress.Interface.LoadingBar.Text = "Downloading..";
			downloadProgress.Interface.LoadingBar.SubText = "";
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
			Os.OpenFolder(item.Directory.FullName);
			downloadProgress.Interface.LoadingBar.Text = "Unable To Import";
			downloadProgress.Interface.LoadingBar.SubText = "Sorry, you need to convert and import this item manually.";
			yield return new WaitForSeconds(5f);
			downloadProgress.Interface.LoadingBar.Active = false;
			onImportFinished();
		}
	}
}