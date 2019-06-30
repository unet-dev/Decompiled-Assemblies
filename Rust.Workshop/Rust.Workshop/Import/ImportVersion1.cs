using Facepunch.Utility;
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
	internal class ImportVersion1 : SingletonComponent<ImportVersion1>
	{
		public ImportVersion1()
		{
		}

		internal IEnumerator DoImport(Item item, Skin skin)
		{
			WorkshopItemEditor.Loading(true, "Downloading..", "", 0f);
			if (!item.IsInstalled)
			{
				item.Download(true);
				while (item.IsDownloading)
				{
					yield return null;
				}
				WorkshopItemEditor.Loading(true, "Installing..", "", 0f);
				while (!item.IsInstalled)
				{
					yield return null;
				}
			}
			Os.OpenFolder(item.Directory);
			WorkshopItemEditor.Loading(true, "Unable To Import", "", 0f);
			yield return new WaitForSeconds(5f);
		}
	}
}