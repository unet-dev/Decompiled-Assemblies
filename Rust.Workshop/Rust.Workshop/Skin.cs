using Newtonsoft.Json;
using Rust;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Rust.Workshop
{
	public class Skin
	{
		private string manifestName;

		private string manifestContent;

		private TextAsset manifestAsset;

		private Skin.Manifest manifest;

		private string iconName;

		public Sprite sprite;

		public int references;

		public Skinnable Skinnable;

		public Material[] Materials;

		public Material[] DefaultMaterials;

		public List<Texture> TextureAssets;

		public List<Texture> TextureObjects;

		public Action OnLoaded;

		public Action OnIconLoaded;

		public bool AssetsLoaded
		{
			get;
			internal set;
		}

		public bool AssetsRequested
		{
			get;
			set;
		}

		public bool IconLoaded
		{
			get;
			internal set;
		}

		public bool IconRequested
		{
			get;
			set;
		}

		public Skin()
		{
		}

		internal void Apply(GameObject gameObject)
		{
			Skin.Apply(gameObject, this.Skinnable, this.Materials);
		}

		public static void Apply(GameObject obj, Skinnable skinnable, Material[] Materials)
		{
			TimeWarning.BeginSample("Skin.Apply");
			if (Materials == null)
			{
				TimeWarning.EndSample();
				return;
			}
			if (obj == null)
			{
				TimeWarning.EndSample();
				return;
			}
			MaterialReplacement.ReplaceRecursive(obj, skinnable.SourceMaterials, Materials);
			TimeWarning.EndSample();
		}

		private void DeserializeManifest()
		{
			this.manifest = JsonConvert.DeserializeObject<Skin.Manifest>(this.manifestContent);
		}

		public int GetSizeInBytes()
		{
			int sizeInBytes = 0;
			if (this.sprite != null)
			{
				sizeInBytes += this.sprite.texture.GetSizeInBytes();
			}
			if (this.TextureAssets != null)
			{
				foreach (Texture textureAsset in this.TextureAssets)
				{
					sizeInBytes += textureAsset.GetSizeInBytes();
				}
			}
			if (this.TextureObjects != null)
			{
				foreach (Texture textureObject in this.TextureObjects)
				{
					sizeInBytes += textureObject.GetSizeInBytes();
				}
			}
			return sizeInBytes;
		}

		public IEnumerator LoadAssets(ulong workshopId, string directory = null, AssetBundle bundle = null)
		{
			// 
			// Current member / type: System.Collections.IEnumerator Rust.Workshop.Skin::LoadAssets(System.UInt64,System.String,UnityEngine.AssetBundle)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Rust.Workshop.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Collections.IEnumerator LoadAssets(System.UInt64,System.String,UnityEngine.AssetBundle)
			// 
			// Invalid state value
			//    at ..( , Queue`1 , ILogicalConstruct ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\YieldGuardedBlocksBuilder.cs:line 203
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\YieldGuardedBlocksBuilder.cs:line 187
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\YieldGuardedBlocksBuilder.cs:line 129
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\YieldGuardedBlocksBuilder.cs:line 76
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\LogicalFlowBuilderStep.cs:line 126
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\LogicalFlowBuilderStep.cs:line 51
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , DecompilationContext ,  , Func`2 , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 104
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , DecompilationContext , & ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 139
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildYieldStatementsStep.cs:line 134
			//    at ..Match( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildYieldStatementsStep.cs:line 49
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildYieldStatementsStep.cs:line 20
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public IEnumerator LoadIcon(ulong workshopId, string directory = null, AssetBundle bundle = null)
		{
			Skin str = null;
			AssetBundleRequest assetBundleRequest;
			TimeWarning.BeginSample("Skin.LoadIcon");
			if (bundle != null)
			{
				TimeWarning.BeginSample("ManifestName");
				str.manifestName = string.Concat("Assets/Skins/", workshopId, "/manifest.txt");
				TimeWarning.EndSample();
				TimeWarning.BeginSample("LoadAssetAsync");
				assetBundleRequest = bundle.LoadAssetAsync<TextAsset>(str.manifestName);
				TimeWarning.EndSample();
				TimeWarning.EndSample();
				yield return assetBundleRequest;
				TimeWarning.BeginSample("Skin.LoadIcon");
				TimeWarning.BeginSample("AssetBundleRequest");
				str.manifestAsset = assetBundleRequest.asset as TextAsset;
				TimeWarning.EndSample();
				if (str.manifestAsset != null)
				{
					TimeWarning.BeginSample("TextAsset");
					str.manifestContent = str.manifestAsset.text;
					TimeWarning.EndSample();
				}
				assetBundleRequest = null;
			}
			if (str.manifestContent == null && directory != null)
			{
				TimeWarning.BeginSample("ManifestName");
				str.manifestName = string.Concat(directory, "/manifest.txt");
				TimeWarning.EndSample();
				TimeWarning.BeginSample("File.Exists");
				bool flag = File.Exists(str.manifestName);
				TimeWarning.EndSample();
				if (flag)
				{
					TimeWarning.EndSample();
					yield return Global.Runner.StartCoroutine(Parallel.Coroutine(new Action(str.LoadManifestFromFile)));
					TimeWarning.BeginSample("Skin.LoadIcon");
				}
			}
			if (str.manifestContent != null)
			{
				TimeWarning.EndSample();
				yield return Global.Runner.StartCoroutine(Parallel.Coroutine(new Action(str.DeserializeManifest)));
				TimeWarning.BeginSample("Skin.LoadIcon");
			}
			if (str.manifest == null)
			{
				UnityEngine.Debug.LogWarning(string.Concat("Invalid skin manifest: ", str.manifestName));
				TimeWarning.EndSample();
				yield break;
			}
			TimeWarning.BeginSample("Skinnable.FindForItem");
			str.Skinnable = Skinnable.FindForItem(str.manifest.ItemType);
			TimeWarning.EndSample();
			if (bundle != null)
			{
				TimeWarning.BeginSample("IconName");
				str.iconName = string.Concat("Assets/Skins/", workshopId, "/icon.png");
				TimeWarning.EndSample();
				TimeWarning.BeginSample("LoadAssetAsync");
				assetBundleRequest = bundle.LoadAssetAsync<Sprite>(str.iconName);
				TimeWarning.EndSample();
				TimeWarning.EndSample();
				yield return assetBundleRequest;
				TimeWarning.BeginSample("Skin.LoadIcon");
				TimeWarning.BeginSample("AssetBundleRequest");
				Sprite sprite = assetBundleRequest.asset as Sprite;
				TimeWarning.EndSample();
				if (sprite != null)
				{
					TimeWarning.BeginSample("Sprite");
					str.sprite = sprite;
					TimeWarning.EndSample();
				}
				assetBundleRequest = null;
			}
			if (str.sprite == null && SteamClient.IsValid)
			{
				string empty = string.Empty;
				InventoryDef[] definitions = SteamInventory.Definitions;
				TimeWarning.BeginSample("IconName");
				str.iconName = workshopId.ToString();
				TimeWarning.EndSample();
				if (definitions != null)
				{
					TimeWarning.BeginSample("FindItemDefinition");
					int length = (int)definitions.Length - 1;
					while (length >= 0)
					{
						InventoryDef inventoryDef = definitions[length];
						string property = inventoryDef.GetProperty("workshopdownload");
						if (str.iconName != property)
						{
							length--;
						}
						else
						{
							empty = inventoryDef.IconUrlLarge;
							break;
						}
					}
					TimeWarning.EndSample();
				}
				if (!string.IsNullOrEmpty(empty))
				{
					TimeWarning.BeginSample("UnityWebRequestTexture.GetTexture");
					UnityWebRequest texture = UnityWebRequestTexture.GetTexture(empty);
					texture.timeout = Mathf.CeilToInt(WorkshopSkin.DownloadTimeout);
					TimeWarning.EndSample();
					TimeWarning.EndSample();
					yield return texture.SendWebRequest();
					TimeWarning.BeginSample("Skin.LoadIcon");
					if (texture.isDone && !texture.isHttpError && !texture.isNetworkError)
					{
						TimeWarning.BeginSample("DownloadHandlerTexture.GetContent");
						Texture2D content = DownloadHandlerTexture.GetContent(texture);
						TimeWarning.EndSample();
						TimeWarning.BeginSample("Sprite");
						str.sprite = Sprite.Create(content, new Rect(0f, 0f, 512f, 512f), Vector2.zero, 100f, 0, SpriteMeshType.FullRect);
						TimeWarning.EndSample();
					}
					TimeWarning.BeginSample("UnityWebRequest.Dispose");
					texture.Dispose();
					TimeWarning.EndSample();
					texture = null;
				}
			}
			if (str.sprite == null && directory != null)
			{
				TimeWarning.BeginSample("IconName");
				str.iconName = string.Concat(directory, "/icon.png");
				TimeWarning.EndSample();
				TimeWarning.BeginSample("File.Exists");
				bool flag1 = File.Exists(str.iconName);
				TimeWarning.EndSample();
				if (flag1)
				{
					TimeWarning.BeginSample("AsyncTextureLoad.Invoke");
					AsyncTextureLoad asyncTextureLoad = new AsyncTextureLoad(str.iconName, false, false, true, false);
					TimeWarning.EndSample();
					TimeWarning.EndSample();
					yield return asyncTextureLoad;
					TimeWarning.BeginSample("Skin.LoadIcon");
					TimeWarning.BeginSample("AsyncTextureLoad.Texture");
					Texture2D texture2D = asyncTextureLoad.texture;
					TimeWarning.EndSample();
					TimeWarning.BeginSample("Sprite");
					str.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, 512f, 512f), Vector2.zero, 100f, 0, SpriteMeshType.FullRect);
					TimeWarning.EndSample();
					asyncTextureLoad = null;
				}
			}
			if (str.sprite != null)
			{
				str.IconLoaded = true;
				if (str.OnIconLoaded != null)
				{
					str.OnIconLoaded();
				}
			}
			TimeWarning.EndSample();
		}

		private void LoadManifestFromFile()
		{
			this.manifestContent = File.ReadAllText(this.manifestName);
		}

		internal void ReadDefaults()
		{
			TimeWarning.BeginSample("Skin.ReadDefaults");
			if (this.AssetsLoaded)
			{
				this.UnloadAssets();
			}
			if (this.Skinnable != null && this.Skinnable.Groups != null)
			{
				if (this.DefaultMaterials == null || (int)this.DefaultMaterials.Length != (int)this.Skinnable.Groups.Length)
				{
					this.DefaultMaterials = new Material[(int)this.Skinnable.Groups.Length];
				}
				if (this.Materials == null || (int)this.Materials.Length != (int)this.Skinnable.Groups.Length)
				{
					this.Materials = new Material[(int)this.Skinnable.Groups.Length];
				}
				for (int i = 0; i < (int)this.DefaultMaterials.Length; i++)
				{
					Skinnable.Group groups = this.Skinnable.Groups[i];
					if (groups != null)
					{
						this.DefaultMaterials[i] = groups.Material;
					}
				}
				for (int j = 0; j < (int)this.Materials.Length; j++)
				{
					if (this.DefaultMaterials[j] != null)
					{
						Material[] materials = this.Materials;
						Material material = new Material(this.DefaultMaterials[j]);
						Material material1 = material;
						materials[j] = material;
						Material material2 = material1;
						material2.DisableKeyword("_COLORIZELAYER_ON");
						material2.SetInt("_COLORIZELAYER_ON", 0);
						material2.name = string.Concat(this.DefaultMaterials[j].name, " (Editing)");
					}
					else
					{
						UnityEngine.Debug.LogWarning(string.Concat("Missing skin for ", this.Skinnable.ItemName));
					}
				}
			}
			TimeWarning.EndSample();
		}

		public void UnloadAssets()
		{
			if (this.Materials != null)
			{
				for (int i = 0; i < (int)this.Materials.Length; i++)
				{
					Material materials = this.Materials[i];
					if (materials != null)
					{
						UnityEngine.Object.DestroyImmediate(materials);
						this.Materials[i] = null;
					}
				}
			}
			if (this.TextureObjects != null)
			{
				for (int j = 0; j < this.TextureObjects.Count; j++)
				{
					Texture item = this.TextureObjects[j];
					if (item != null)
					{
						UnityEngine.Object.DestroyImmediate(item);
					}
				}
				this.TextureObjects.Clear();
			}
			if (this.TextureAssets != null)
			{
				for (int k = 0; k < this.TextureAssets.Count; k++)
				{
					Texture texture = this.TextureAssets[k];
					if (texture != null)
					{
						Resources.UnloadAsset(texture);
					}
				}
				this.TextureAssets.Clear();
			}
			this.AssetsLoaded = false;
		}

		private void UpdateTextureMetadata(Texture2D texture, string textureName, bool anisoFiltering, bool trilinearFiltering)
		{
			texture.name = textureName;
			texture.anisoLevel = (anisoFiltering ? 16 : 1);
			texture.filterMode = (trilinearFiltering ? FilterMode.Trilinear : FilterMode.Bilinear);
		}

		public class Manifest
		{
			public ulong AuthorId
			{
				get;
				set;
			}

			public Skin.Manifest.Group[] Groups
			{
				get;
				set;
			}

			public string ItemType
			{
				get;
				set;
			}

			public DateTime PublishDate
			{
				get;
				set;
			}

			public int Version
			{
				get;
				set;
			}

			public Manifest()
			{
			}

			public class ColorEntry
			{
				public float b
				{
					get;
					set;
				}

				public float g
				{
					get;
					set;
				}

				public float r
				{
					get;
					set;
				}

				public ColorEntry(Color c)
				{
					this.r = c.r;
					this.g = c.g;
					this.b = c.b;
				}
			}

			public class Group
			{
				public Dictionary<string, Skin.Manifest.ColorEntry> Colors
				{
					get;
					set;
				}

				public Dictionary<string, float> Floats
				{
					get;
					set;
				}

				public Dictionary<string, string> Textures
				{
					get;
					set;
				}

				public Group()
				{
				}
			}
		}
	}
}