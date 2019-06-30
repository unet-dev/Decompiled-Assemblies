using Facepunch.Utility;
using Rust;
using Steamworks.Ugc;
using System;
using System.IO;
using UnityEngine;

namespace Rust.Workshop
{
	public class WorkshopInterface : SingletonComponent<WorkshopInterface>
	{
		internal WorkshopItemEditor Editor
		{
			get
			{
				return SingletonComponent<WorkshopItemEditor>.Instance;
			}
		}

		public WorkshopInterface()
		{
		}

		public void Exit()
		{
			UnityEngine.Object.Destroy(base.gameObject);
			Global.OpenMainMenu();
		}

		public void RenderAllIcons()
		{
			this.Editor.gameObject.SetActive(true);
			if (!Directory.Exists("c:/test/icons"))
			{
				Directory.CreateDirectory("c:/test/icons");
			}
			Skinnable[] all = Skinnable.All;
			for (int i = 0; i < (int)all.Length; i++)
			{
				Skinnable skinnable = all[i];
				this.Editor.StartNewItem(skinnable.Name);
				PropRenderer.RenderScreenshot(this.Editor.Prefab, string.Concat("c:/test/icons/", skinnable.Name, ".png"), 512, 512, 4);
				int num = 0;
				UnityEngine.Mesh[] meshDownloads = skinnable.MeshDownloads;
				for (int j = 0; j < (int)meshDownloads.Length; j++)
				{
					UnityEngine.Mesh mesh = meshDownloads[j];
					if (mesh != null && mesh.isReadable)
					{
						mesh.Export(string.Format("c:/test/icons/{0}{1}.obj", skinnable.Name, num));
						num++;
					}
				}
			}
			this.Editor.gameObject.SetActive(false);
		}

		public void StartEditing(Item item)
		{
			QualitySettings.masterTextureLimit = 0;
			QualitySettings.SetQualityLevel(5, true);
			this.Editor.gameObject.SetActive(true);
			base.StartCoroutine(this.Editor.StartEditingItem(item));
		}

		public void StartViewing(Item item)
		{
			QualitySettings.masterTextureLimit = 0;
			QualitySettings.SetQualityLevel(5, true);
			this.Editor.gameObject.SetActive(true);
			base.StartCoroutine(this.Editor.StartViewingItem(item));
		}
	}
}