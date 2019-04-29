using Newtonsoft.Json;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

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

		public Skinnable Skinnable;

		public Material[] Materials;

		public Material[] DefaultMaterials;

		public Action OnLoaded;

		public Action OnIconLoaded;

		public bool IconLoaded
		{
			get;
			internal set;
		}

		public bool Loaded
		{
			get;
			internal set;
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

		private bool CompareMaterials(Material a, Material b)
		{
			if (a == b)
			{
				return true;
			}
			if (a.GetTexture("_MainTex") != b.GetTexture("_MainTex"))
			{
				return false;
			}
			return true;
		}

		private void DeserializeManifest()
		{
			this.manifest = JsonConvert.DeserializeObject<Skin.Manifest>(this.manifestContent);
		}

		public IEnumerator FromFolder(ulong workshopId, DirectoryInfo directory = null, AssetBundle bundle = null)
		{
			// 
			// Current member / type: System.Collections.IEnumerator Rust.Workshop.Skin::FromFolder(System.UInt64,System.IO.DirectoryInfo,UnityEngine.AssetBundle)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Rust.Workshop.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Collections.IEnumerator FromFolder(System.UInt64,System.IO.DirectoryInfo,UnityEngine.AssetBundle)
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

		private bool IsSkinnable(string name)
		{
			if (name.Contains("PlayerSkin"))
			{
				return false;
			}
			if (name.StartsWith("Female."))
			{
				return false;
			}
			if (name.StartsWith("Male."))
			{
				return false;
			}
			return true;
		}

		private void LoadManifestFromFile()
		{
			this.manifestContent = File.ReadAllText(this.manifestName);
		}

		internal void ReadDefaults()
		{
			TimeWarning.BeginSample("Skin.ReadDefaults");
			if (this.DefaultMaterials != null && this.Materials != null)
			{
				for (int i = 0; i < (int)this.Materials.Length; i++)
				{
					if (this.CompareMaterials(this.Materials[i], this.DefaultMaterials[i]))
					{
						this.Materials[i] = null;
					}
				}
			}
			this.DefaultMaterials = new Material[(int)this.Skinnable.Groups.Length];
			for (int j = 0; j < (int)this.Skinnable.Groups.Length; j++)
			{
				this.DefaultMaterials[j] = this.Skinnable.Groups[j].Material;
			}
			if (this.Materials == null || (int)this.Materials.Length != (int)this.Skinnable.Groups.Length)
			{
				this.Materials = new Material[(int)this.Skinnable.Groups.Length];
			}
			for (int k = 0; k < (int)this.Materials.Length; k++)
			{
				if (this.DefaultMaterials[k] != null)
				{
					Material materials = this.Materials[k];
					this.Materials[k] = new Material(this.DefaultMaterials[k]);
					this.Materials[k].DisableKeyword("_COLORIZELAYER_ON");
					this.Materials[k].SetInt("_COLORIZELAYER_ON", 0);
					this.Materials[k].name = string.Concat(this.DefaultMaterials[k].name, " (Editing)");
				}
				else
				{
					UnityEngine.Debug.LogWarning(string.Concat("Missing skin for ", this.Skinnable.ItemName));
				}
			}
			TimeWarning.EndSample();
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