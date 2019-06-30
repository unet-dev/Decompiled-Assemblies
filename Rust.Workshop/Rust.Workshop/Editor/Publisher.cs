using Facepunch.Extend;
using Facepunch.Utility;
using Newtonsoft.Json;
using Rust;
using Rust.UI;
using Rust.Workshop;
using Steamworks;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
				return SingletonComponent<WorkshopItemEditor>.Instance;
			}
		}

		protected WorkshopInterface Interface
		{
			get
			{
				return SingletonComponent<WorkshopInterface>.Instance;
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

		private async Task DoExport(bool publishToSteam, bool OpenFolder, string forceFolderName = null)
		{
			Publisher.<DoExport>d__15 openFolder = new Publisher.<DoExport>d__15();
			openFolder.<>4__this = this;
			openFolder.publishToSteam = publishToSteam;
			openFolder.OpenFolder = OpenFolder;
			openFolder.forceFolderName = forceFolderName;
			openFolder.<>t__builder = AsyncTaskMethodBuilder.Create();
			openFolder.<>1__state = -1;
			openFolder.<>t__builder.Start<Publisher.<DoExport>d__15>(ref openFolder);
			return openFolder.<>t__builder.Task;
		}

		public async Task DoExport()
		{
			string str = await this.Editor.FileDialog.SaveAsync(null, null, "SAVE FILE", null, true);
			if (str != null)
			{
				if (!File.Exists(str))
				{
					if (!Directory.Exists(str))
					{
						Directory.CreateDirectory(str);
					}
					await this.DoExport(false, true, str);
				}
			}
		}

		public void Export()
		{
			this.DoExport();
		}

		private async Task ExportTexture(Dictionary<string, string> data, string folder, int group, string paramname, Material mat, Material defaultMat, bool isNormalMap = false)
		{
			WorkshopItemEditor.Loading(true, string.Concat("Exporting Texture ", paramname), "", 0f);
			UnityEngine.Texture texture = mat.GetTexture(paramname);
			if (texture != defaultMat.GetTexture(paramname))
			{
				if (texture != null)
				{
					texture = Facepunch.Utility.Texture.LimitSize(texture as Texture2D, this.Skinnable.Groups[group].MaxTextureSize, this.Skinnable.Groups[group].MaxTextureSize);
					if (isNormalMap)
					{
						texture = Facepunch.Utility.Texture.CreateReadableCopy(texture as Texture2D, 0, 0);
						(texture as Texture2D).DecompressNormals();
					}
					string str = string.Format("{0}{1}{2}", paramname, group, ".png");
					data.Add(paramname, str);
					texture.SaveAsPng(string.Concat(folder, "/", str));
					await Task.Delay(1);
				}
				else
				{
					data.Add(paramname, "none");
				}
			}
		}

		private async Task ExportToFolder(string folder, bool OpenFolder)
		{
			Rust.Workshop.Skin.Manifest manifest = new Rust.Workshop.Skin.Manifest()
			{
				ItemType = this.Skinnable.Name,
				Version = 3,
				Groups = new Rust.Workshop.Skin.Manifest.Group[(int)this.Skin.Materials.Length],
				PublishDate = DateTime.UtcNow,
				AuthorId = SteamClient.SteamId
			};
			Rust.Workshop.Skin.Manifest group = manifest;
			for (int i = 0; i < (int)this.Skin.Materials.Length; i++)
			{
				group.Groups[i] = new Rust.Workshop.Skin.Manifest.Group();
				await this.ExportTexture(group.Groups[i].Textures, folder, i, "_MainTex", this.Skin.Materials[i], this.Skin.DefaultMaterials[i], false);
				await this.ExportTexture(group.Groups[i].Textures, folder, i, "_OcclusionMap", this.Skin.Materials[i], this.Skin.DefaultMaterials[i], false);
				await this.ExportTexture(group.Groups[i].Textures, folder, i, "_SpecGlossMap", this.Skin.Materials[i], this.Skin.DefaultMaterials[i], false);
				await this.ExportTexture(group.Groups[i].Textures, folder, i, "_BumpMap", this.Skin.Materials[i], this.Skin.DefaultMaterials[i], true);
				await this.ExportTexture(group.Groups[i].Textures, folder, i, "_EmissionMap", this.Skin.Materials[i], this.Skin.DefaultMaterials[i], false);
				group.Groups[i].Floats.Add("_Cutoff", this.Skin.Materials[i].GetFloat("_Cutoff"));
				group.Groups[i].Floats.Add("_BumpScale", this.Skin.Materials[i].GetFloat("_BumpScale"));
				group.Groups[i].Floats.Add("_Glossiness", this.Skin.Materials[i].GetFloat("_Glossiness"));
				group.Groups[i].Floats.Add("_OcclusionStrength", this.Skin.Materials[i].GetFloat("_OcclusionStrength"));
				if (this.Skin.Materials[i].shader.name.Contains("Cloth"))
				{
					group.Groups[i].Floats.Add("_MicrofiberFuzzIntensity", this.Skin.Materials[i].GetFloat("_MicrofiberFuzzIntensity"));
					group.Groups[i].Floats.Add("_MicrofiberFuzzScatter", this.Skin.Materials[i].GetFloat("_MicrofiberFuzzScatter"));
					group.Groups[i].Floats.Add("_MicrofiberFuzzOcclusion", this.Skin.Materials[i].GetFloat("_MicrofiberFuzzOcclusion"));
				}
				group.Groups[i].Colors.Add("_Color", new Rust.Workshop.Skin.Manifest.ColorEntry(this.Skin.Materials[i].GetColor("_Color")));
				group.Groups[i].Colors.Add("_SpecColor", new Rust.Workshop.Skin.Manifest.ColorEntry(this.Skin.Materials[i].GetColor("_SpecColor")));
				group.Groups[i].Colors.Add("_EmissionColor", new Rust.Workshop.Skin.Manifest.ColorEntry(this.Skin.Materials[i].GetColor("_EmissionColor")));
				if (this.Skin.Materials[i].shader.name.Contains("Cloth"))
				{
					group.Groups[i].Colors.Add("_MicrofiberFuzzColor", new Rust.Workshop.Skin.Manifest.ColorEntry(this.Skin.Materials[i].GetColor("_MicrofiberFuzzColor")));
				}
			}
			PropRenderer.RenderScreenshot(this.Prefab, string.Concat(folder, "/icon.png"), 512, 512, 4);
			this.CreateWorkshopIcon(folder);
			string str = JsonConvert.SerializeObject(group, Formatting.Indented);
			File.WriteAllText(string.Concat(folder, "/manifest.txt"), str);
			if (OpenFolder)
			{
				Os.OpenFolder(folder);
			}
		}

		private async Task PublishToSteam(string folder)
		{
			Steamworks.Ugc.Editor editor = new Steamworks.Ugc.Editor();
			editor = (this.ItemId != 0 ? new Steamworks.Ugc.Editor(this.ItemId) : Steamworks.Ugc.Editor.NewMicrotransactionFile);
			Steamworks.Ugc.Editor editor1 = editor.WithContent(folder);
			editor1 = editor1.WithPreviewFile(string.Concat(folder, "/icon_background.png"));
			editor1 = editor1.WithTitle(this.Title);
			editor1 = editor1.WithTag("Version3");
			editor1 = editor1.WithTag(this.Skinnable.Name);
			editor1 = editor1.WithTag("Skin");
			editor = editor1.WithPublicVisibility();
			if (!string.IsNullOrEmpty(this.ChangeLog.text))
			{
				editor = editor.WithChangeLog(this.ChangeLog.text);
			}
			WorkshopItemEditor.Loading(true, "Publishing To Steam", "", 0f);
			PublishResult publishResult = await editor.SubmitAsync(null);
			if (publishResult.Success)
			{
				UnityEngine.Debug.Log(string.Concat("Published File: ", publishResult.FileId));
			}
			else
			{
				UnityEngine.Debug.Log(string.Concat("Error: ", publishResult.Result));
			}
			Item? nullable = await SteamUGC.QueryFileAsync(publishResult.FileId);
			if (nullable.HasValue)
			{
				WorkshopItemEditor id = this.Editor;
				Item value = nullable.Value;
				id.ItemId = value.Id;
				WorkshopItemEditor title = this.Editor;
				value = nullable.Value;
				title.ItemTitle = value.Title;
				this.ChangeLog.text = "";
				value = nullable.Value;
				UnityEngine.Application.OpenURL(value.Url);
				WorkshopItemList.RefreshAll();
			}
			else
			{
				UnityEngine.Debug.Log("Error Retrieving item information!");
				WorkshopItemList.RefreshAll();
			}
		}

		public void StartExport()
		{
			this.DoExport(true, false, null);
		}

		public void Update()
		{
			this.PublishButton.interactable = this.CanPublish();
		}
	}
}