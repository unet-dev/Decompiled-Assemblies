using Facepunch.Extend;
using Facepunch.Steamworks;
using Facepunch.Utility;
using Newtonsoft.Json;
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
using UnityEngine.UI;

namespace Rust.Workshop.Editor
{
	public class Publisher : MonoBehaviour
	{
		public InputField ChangeLog;

		public Button PublishButton;

		public Texture2D IconBackground;

		private Skinnable Skinnable;

		private Rust.Workshop.Skin Skin;

		private ulong ItemId;

		private string Title;

		private GameObject Prefab;

		protected WorkshopItemEditor Editor
		{
			get
			{
				return this.Interface.Editor;
			}
		}

		protected WorkshopInterface Interface
		{
			get
			{
				return base.GetComponentInParent<WorkshopInterface>();
			}
		}

		public Publisher()
		{
		}

		public bool CanPublish()
		{
			if (this.Interface.Editor.ItemTitle.Length == 0)
			{
				return false;
			}
			return true;
		}

		private void CreateWorkshopIcon(string folder)
		{
			Texture2D texture2D = new Texture2D(512, 512, TextureFormat.ARGB32, false);
			texture2D.LoadImage(File.ReadAllBytes(string.Concat(folder, "/icon.png")));
			RenderTexture renderTexture = new RenderTexture(512, 512, 0);
			renderTexture.Blit(this.IconBackground);
			renderTexture.BlitWithAlphaBlending(texture2D);
			renderTexture.ToTexture(texture2D);
			byte[] pNG = texture2D.EncodeToPNG();
			File.WriteAllBytes(string.Concat(folder, "/icon_background.png"), pNG);
			UnityEngine.Object.DestroyImmediate(texture2D);
			UnityEngine.Object.DestroyImmediate(renderTexture);
		}

		private IEnumerator DoExport(bool publishToSteam, bool OpenFolder, string forceFolderName = null)
		{
			Publisher skinnable = null;
			skinnable.Skinnable = skinnable.Editor.Skinnable;
			skinnable.Skin = skinnable.Editor.Skin;
			skinnable.ItemId = skinnable.Editor.ItemId;
			skinnable.Title = skinnable.Editor.ItemTitle;
			skinnable.Prefab = skinnable.Editor.Prefab;
			skinnable.Interface.LoadingBar.Active = true;
			skinnable.Interface.LoadingBar.Text = "Exporting";
			skinnable.Interface.LoadingBar.SubText = "";
			string tempFileName = Path.GetTempFileName();
			File.Delete(tempFileName);
			Directory.CreateDirectory(tempFileName);
			if (forceFolderName != null)
			{
				tempFileName = forceFolderName;
			}
			UnityEngine.Debug.Log(tempFileName);
			yield return skinnable.StartCoroutine(skinnable.ExportToFolder(tempFileName, OpenFolder));
			if (publishToSteam)
			{
				yield return skinnable.StartCoroutine(skinnable.PublishToSteam(tempFileName));
			}
			if (forceFolderName != tempFileName)
			{
				Directory.Delete(tempFileName, true);
			}
			skinnable.Interface.LoadingBar.Active = false;
		}

		public IEnumerator DoExport()
		{
			Publisher publisher = null;
			yield return publisher.StartCoroutine(publisher.Editor.FileDialog.Save(null, null, "SAVE FILE", null, true));
			if (publisher.Editor.FileDialog.result == null)
			{
				yield break;
			}
			if (File.Exists(publisher.Editor.FileDialog.result))
			{
				yield break;
			}
			if (!Directory.Exists(publisher.Editor.FileDialog.result))
			{
				Directory.CreateDirectory(publisher.Editor.FileDialog.result);
			}
			yield return publisher.StartCoroutine(publisher.DoExport(false, true, publisher.Editor.FileDialog.result));
		}

		public void Export()
		{
			base.StartCoroutine(this.DoExport());
		}

		private IEnumerator ExportTexture(Dictionary<string, string> data, string folder, int group, string paramname, Material mat, Material defaultMat, bool isNormalMap = false)
		{
			Publisher publisher = null;
			publisher.Interface.LoadingBar.SubText = string.Concat("Exporting Texture ", paramname);
			UnityEngine.Texture texture = mat.GetTexture(paramname);
			if (texture == defaultMat.GetTexture(paramname))
			{
				yield break;
			}
			if (texture == null)
			{
				data.Add(paramname, "none");
				yield break;
			}
			texture = Facepunch.Utility.Texture.LimitSize(texture as Texture2D, publisher.Skinnable.Groups[group].MaxTextureSize, publisher.Skinnable.Groups[group].MaxTextureSize);
			if (isNormalMap)
			{
				texture = Facepunch.Utility.Texture.CreateReadableCopy(texture as Texture2D, 0, 0);
				(texture as Texture2D).DecompressNormals();
			}
			string str = string.Format("{0}{1}{2}", paramname, group, ".png");
			data.Add(paramname, str);
			texture.SaveAsPng(string.Concat(folder, "/", str));
			yield return null;
		}

		private IEnumerator ExportToFolder(string folder, bool OpenFolder)
		{
			Publisher publisher = null;
			Rust.Workshop.Skin.Manifest manifest = new Rust.Workshop.Skin.Manifest()
			{
				ItemType = publisher.Skinnable.Name,
				Version = 3,
				Groups = new Rust.Workshop.Skin.Manifest.Group[(int)publisher.Skin.Materials.Length],
				PublishDate = DateTime.UtcNow,
				AuthorId = Global.SteamClient.SteamId
			};
			Rust.Workshop.Skin.Manifest group = manifest;
			for (int i = 0; i < (int)publisher.Skin.Materials.Length; i++)
			{
				group.Groups[i] = new Rust.Workshop.Skin.Manifest.Group();
				yield return publisher.StartCoroutine(publisher.ExportTexture(group.Groups[i].Textures, folder, i, "_MainTex", publisher.Skin.Materials[i], publisher.Skin.DefaultMaterials[i], false));
				yield return publisher.StartCoroutine(publisher.ExportTexture(group.Groups[i].Textures, folder, i, "_OcclusionMap", publisher.Skin.Materials[i], publisher.Skin.DefaultMaterials[i], false));
				yield return publisher.StartCoroutine(publisher.ExportTexture(group.Groups[i].Textures, folder, i, "_SpecGlossMap", publisher.Skin.Materials[i], publisher.Skin.DefaultMaterials[i], false));
				yield return publisher.StartCoroutine(publisher.ExportTexture(group.Groups[i].Textures, folder, i, "_BumpMap", publisher.Skin.Materials[i], publisher.Skin.DefaultMaterials[i], true));
				yield return publisher.StartCoroutine(publisher.ExportTexture(group.Groups[i].Textures, folder, i, "_EmissionMap", publisher.Skin.Materials[i], publisher.Skin.DefaultMaterials[i], false));
				group.Groups[i].Floats.Add("_Cutoff", publisher.Skin.Materials[i].GetFloat("_Cutoff"));
				group.Groups[i].Floats.Add("_BumpScale", publisher.Skin.Materials[i].GetFloat("_BumpScale"));
				group.Groups[i].Floats.Add("_Glossiness", publisher.Skin.Materials[i].GetFloat("_Glossiness"));
				group.Groups[i].Floats.Add("_OcclusionStrength", publisher.Skin.Materials[i].GetFloat("_OcclusionStrength"));
				if (publisher.Skin.Materials[i].shader.name.Contains("Cloth"))
				{
					group.Groups[i].Floats.Add("_MicrofiberFuzzIntensity", publisher.Skin.Materials[i].GetFloat("_MicrofiberFuzzIntensity"));
					group.Groups[i].Floats.Add("_MicrofiberFuzzScatter", publisher.Skin.Materials[i].GetFloat("_MicrofiberFuzzScatter"));
					group.Groups[i].Floats.Add("_MicrofiberFuzzOcclusion", publisher.Skin.Materials[i].GetFloat("_MicrofiberFuzzOcclusion"));
				}
				group.Groups[i].Colors.Add("_Color", new Rust.Workshop.Skin.Manifest.ColorEntry(publisher.Skin.Materials[i].GetColor("_Color")));
				group.Groups[i].Colors.Add("_SpecColor", new Rust.Workshop.Skin.Manifest.ColorEntry(publisher.Skin.Materials[i].GetColor("_SpecColor")));
				group.Groups[i].Colors.Add("_EmissionColor", new Rust.Workshop.Skin.Manifest.ColorEntry(publisher.Skin.Materials[i].GetColor("_EmissionColor")));
				if (publisher.Skin.Materials[i].shader.name.Contains("Cloth"))
				{
					group.Groups[i].Colors.Add("_MicrofiberFuzzColor", new Rust.Workshop.Skin.Manifest.ColorEntry(publisher.Skin.Materials[i].GetColor("_MicrofiberFuzzColor")));
				}
			}
			PropRenderer.RenderScreenshot(publisher.Prefab, string.Concat(folder, "/icon.png"), 512, 512, 4);
			publisher.CreateWorkshopIcon(folder);
			string str = JsonConvert.SerializeObject(group, Formatting.Indented);
			File.WriteAllText(string.Concat(folder, "/manifest.txt"), str);
			if (OpenFolder)
			{
				Os.OpenFolder(folder);
			}
		}

		private IEnumerator PublishToSteam(string folder)
		{
			Publisher progress = null;
			Facepunch.Steamworks.Workshop.Editor title = null;
			title = (progress.ItemId != 0 ? Global.SteamClient.Workshop.EditItem(progress.ItemId) : Global.SteamClient.Workshop.CreateItem(Facepunch.Steamworks.Workshop.ItemType.Microtransaction));
			if (title == null)
			{
				throw new Exception("No Editor");
			}
			title.Folder = folder;
			title.PreviewImage = string.Concat(folder, "/icon_background.png");
			title.Title = progress.Title;
			title.Tags.Add("Version3");
			title.Tags.Add(progress.Skinnable.Name);
			title.Tags.Add("Skin");
			title.Visibility = new Facepunch.Steamworks.Workshop.Editor.VisibilityType?(Facepunch.Steamworks.Workshop.Editor.VisibilityType.Public);
			if (!string.IsNullOrEmpty(progress.ChangeLog.text))
			{
				title.ChangeNote = progress.ChangeLog.text;
			}
			title.Publish();
			progress.Interface.LoadingBar.Text = "Publishing To Steam";
			while (title.Publishing)
			{
				progress.Interface.LoadingBar.Progress = (float)title.Progress;
				progress.Interface.LoadingBar.SubText = string.Format("{0} / {1}", title.BytesUploaded.FormatBytes<int>(false), title.BytesTotal.FormatBytes<int>(false));
				yield return null;
			}
			if (title.Error == null)
			{
				UnityEngine.Debug.Log(string.Concat("Published File: ", title.Id));
			}
			else
			{
				UnityEngine.Debug.Log(string.Concat("Error: ", title.Error));
			}
			progress.Interface.LoadingBar.SubText = "";
			Facepunch.Steamworks.Workshop.Query query = Global.SteamClient.Workshop.CreateQuery();
			query.FileId.Add(title.Id);
			progress.Interface.LoadingBar.Text = "Fetching Item Information";
			query.Run();
			while (query.IsRunning)
			{
				yield return null;
			}
			if (query.Items.Length == 0)
			{
				UnityEngine.Debug.Log("Error Retrieving item information!");
				WorkshopItemList.RefreshAll();
				yield break;
			}
			Facepunch.Steamworks.Workshop.Item items = query.Items[0];
			progress.Editor.ItemId = items.Id;
			progress.Editor.ItemTitle = items.Title;
			progress.ChangeLog.text = "";
			UnityEngine.Application.OpenURL(query.Items[0].Url);
			WorkshopItemList.RefreshAll();
		}

		public void StartExport()
		{
			base.StartCoroutine(this.DoExport(true, false, null));
		}

		public void Update()
		{
			this.PublishButton.interactable = this.CanPublish();
		}
	}
}