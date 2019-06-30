using Facepunch;
using Facepunch.Extend;
using Facepunch.Utility;
using Rust;
using Rust.UI;
using Rust.Workshop.Editor;
using Rust.Workshop.Import;
using Steamworks;
using Steamworks.Ugc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.Workshop
{
	public class WorkshopItemEditor : SingletonComponent<WorkshopItemEditor>
	{
		public static Action<bool, string> OnLoading;

		public Dropdown ItemTypeSelector;

		public InputField ItemTitleLabel;

		public WorkshopViewmodelControls ViewmodelControls;

		public GameObject MaterialTabHolder;

		public GameObject FileDialogObject;

		public GameObject[] EditorElements;

		public GameObject[] ClothOnlyElements;

		public Toggle[] MaterialTabs;

		public int EditingMaterial;

		public GameObject item_position_a;

		public GameObject item_position_b;

		public string ChangeLog
		{
			get
			{
				return this.Publisher.ChangeLog.text;
			}
			set
			{
				this.Publisher.ChangeLog.text = value;
			}
		}

		public Rust.UI.FileDialog FileDialog
		{
			get
			{
				return this.FileDialogObject.GetComponent<Rust.UI.FileDialog>();
			}
		}

		protected WorkshopInterface Interface
		{
			get
			{
				return base.GetComponentInParent<WorkshopInterface>();
			}
		}

		internal ulong ItemId
		{
			get;
			set;
		}

		public string ItemTitle
		{
			get
			{
				return this.ItemTitleLabel.text;
			}
			set
			{
				this.ItemTitleLabel.text = value;
			}
		}

		internal GameObject Prefab
		{
			get;
			set;
		}

		internal Rust.Workshop.Editor.Publisher Publisher
		{
			get
			{
				return base.GetComponentInChildren<Rust.Workshop.Editor.Publisher>(true);
			}
		}

		internal Rust.Workshop.Skin Skin
		{
			get;
			set;
		}

		internal Skinnable Skinnable
		{
			get;
			set;
		}

		internal GameObject ViewModel
		{
			get;
			set;
		}

		public WorkshopItemEditor()
		{
		}

		private void ClearEditor()
		{
			if (this.Prefab != null)
			{
				UnityEngine.Object.Destroy(this.Prefab);
				this.Prefab = null;
			}
			if (this.ViewModel != null)
			{
				UnityEngine.Object.Destroy(this.ViewModel);
				this.ViewModel = null;
			}
			this.ItemId = (ulong)0;
			base.GetComponent<WorkshopPlayerPreview>().Cleanup();
			this.ItemTitle = "";
			this.Skinnable = null;
			this.ChangeLog = "";
			base.GetComponentInChildren<WorkshopView>(true).Clear();
		}

		private IEnumerator DoDownloadModel()
		{
			WorkshopItemEditor workshopItemEditor = null;
			yield return workshopItemEditor.StartCoroutine(workshopItemEditor.FileDialog.Save(null, ".obj", "SAVE FILE", null, true));
			if (string.IsNullOrEmpty(workshopItemEditor.FileDialog.result))
			{
				yield break;
			}
			UnityEngine.Debug.Log(string.Concat("Save Obj to ", workshopItemEditor.FileDialog.result));
			for (int i = 0; i < (int)workshopItemEditor.Skinnable.MeshDownloads.Length; i++)
			{
				UnityEngine.Mesh meshDownloads = workshopItemEditor.Skinnable.MeshDownloads[i];
				if (meshDownloads != null && meshDownloads.isReadable)
				{
					string fileDialog = workshopItemEditor.FileDialog.result;
					if (i > 0)
					{
						fileDialog = fileDialog.Replace(".obj", string.Format("_{0}.obj", i));
					}
					meshDownloads.Export(fileDialog);
				}
			}
		}

		public void DownloadModel()
		{
			base.StartCoroutine(this.DoDownloadModel());
		}

		private void HideEditor()
		{
			GameObject[] editorElements = this.EditorElements;
			for (int i = 0; i < (int)editorElements.Length; i++)
			{
				editorElements[i].SetActive(false);
			}
		}

		private void InitPlayerPreview(ulong playerid, bool focus)
		{
			GameObject createPrefab = Global.CreatePrefab(this.Skinnable.EntityPrefabName);
			createPrefab.AddComponent<DepthOfFieldFocusPoint>();
			WorkshopItemEditor.RemoveLODs(createPrefab);
			createPrefab.SetActive(true);
			this.Skin.Skinnable = this.Skinnable;
			this.Skin.Apply(createPrefab);
			base.GetComponent<WorkshopPlayerPreview>().Setup(createPrefab, playerid, focus, (this.Skinnable.Category == Category.Weapon || this.Skinnable.Category == Category.Misc ? false : this.Skinnable.Category != Category.Deployable));
		}

		private void InitScene()
		{
			if (this.Skinnable.Category != Category.Deployable)
			{
				this.InitPlayerPreview((ulong)585364905, true);
				if (this.Prefab != null)
				{
					this.Prefab.transform.position = new Vector3(0f, 500f, 0f);
				}
			}
			if (this.Skinnable.ViewmodelPrefab)
			{
				this.ViewModel = Global.CreatePrefab(this.Skinnable.ViewmodelPrefabName);
				this.ViewModel.transform.position = Camera.main.transform.position;
				this.ViewModel.transform.rotation = Camera.main.transform.rotation;
				this.ViewModel.SetActive(true);
				this.ViewModel.BroadcastMessage("WorkshopMode", SendMessageOptions.DontRequireReceiver);
				this.Skin.Apply(this.ViewModel);
			}
		}

		public static bool IsLesserLOD(string name)
		{
			if (name.EndsWith("lod01", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (name.EndsWith("lod02", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (name.EndsWith("lod03", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (name.EndsWith("lod04", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (name.EndsWith("lod1", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (name.EndsWith("lod2", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (name.EndsWith("lod3", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (name.EndsWith("lod4", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			return false;
		}

		private void LateUpdate()
		{
			this.ViewmodelControls.DoUpdate(this.ViewModel);
		}

		internal static void Loading(bool v1, string v2, string v3, float v4)
		{
			Action<bool, string> onLoading = WorkshopItemEditor.OnLoading;
			if (onLoading == null)
			{
				return;
			}
			onLoading(v1, v2);
		}

		private bool LoadItemType(string[] tags)
		{
			string[] strArrays = tags;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				if (this.LoadItemType(strArrays[i]))
				{
					return true;
				}
			}
			return false;
		}

		private bool LoadItemType(string v)
		{
			this.ClearEditor();
			this.Skinnable = Skinnable.FindForItem(v);
			if (this.Skinnable == null)
			{
				return false;
			}
			this.ItemTypeSelector.@value = this.ItemTypeSelector.options.IndexOf(this.ItemTypeSelector.options.First<Dropdown.OptionData>((Dropdown.OptionData x) => x.text == this.Skinnable.Name));
			this.Prefab = Global.CreatePrefab(this.Skinnable.EntityPrefabName);
			WorkshopItemEditor.RemoveLODs(this.Prefab);
			this.Prefab.transform.position = this.item_position_a.transform.position;
			this.Prefab.transform.rotation = this.item_position_a.transform.rotation;
			this.Prefab.SetActive(true);
			this.Prefab.AddComponent<DepthOfFieldFocusPoint>();
			this.Prefab.BroadcastMessage("BuildRig", SendMessageOptions.DontRequireReceiver);
			this.Prefab.BroadcastMessage("WorkshopMode", SendMessageOptions.DontRequireReceiver);
			Camera.main.FocusOnRenderer(this.Prefab, new Vector3(0.3f, 0.5f, 1f), Vector3.up, -1);
			if (this.Skin == null)
			{
				this.Skin = new Rust.Workshop.Skin();
			}
			this.Skin.Skinnable = this.Skinnable;
			this.Skin.ReadDefaults();
			return true;
		}

		public void OnChangedItemType(int type)
		{
			Dropdown.OptionData item = this.ItemTypeSelector.options[type];
			if (this.Skinnable != null && this.Skinnable.Name == item.text)
			{
				return;
			}
			this.LoadItemType(item.text);
			this.EditingMaterial = 0;
			this.UpdateMaterialRows();
			this.InitScene();
		}

		private void OnEnable()
		{
			this.ItemTypeSelector.ClearOptions();
			this.ItemTypeSelector.AddOptions((
				from x in (IEnumerable<Skinnable>)Skinnable.All
				select x.Name into x
				orderby x
				select x).ToList<string>());
			this.MaterialTabs = this.MaterialTabHolder.GetComponentsInChildren<Toggle>(true);
		}

		private void OnImportFinished()
		{
			this.EditingMaterial = 0;
			this.Skin.Skinnable = this.Skinnable;
			this.Skin.Apply(this.Prefab);
			this.UpdateMaterialRows();
			this.InitScene();
		}

		internal IEnumerator OpenItem(Item item)
		{
			WorkshopItemEditor title = null;
			if (!title.LoadItemType(item.Tags))
			{
				UnityEngine.Debug.Log(string.Concat("Couldn't LoadItemType (", string.Join(";", item.Tags), ")"));
				title.ClearEditor();
				WorkshopItemEditor.Loading(false, "", "", 0f);
				yield break;
			}
			Task task = item.Owner.RequestInfoAsync(5000);
			while (!task.IsCompleted)
			{
				yield return null;
			}
			title.GetComponentInChildren<WorkshopView>(true).UpdateFrom(item);
			title.ItemTitle = item.Title;
			title.ItemId = item.Id;
			if (item.Tags.Contains<string>("version3"))
			{
				yield return title.StartCoroutine(SingletonComponent<ImportVersion3>.Instance.DoImport(item, title.Skin));
				yield break;
			}
			if (item.Tags.Contains<string>("version2"))
			{
				yield return title.StartCoroutine(SingletonComponent<ImportVersion2>.Instance.DoImport(item, title.Skin));
				yield break;
			}
			UnityEngine.Debug.Log(string.Concat("Unhandled Item version (", string.Join(";", item.Tags), ")"));
			yield return title.StartCoroutine(SingletonComponent<ImportVersion1>.Instance.DoImport(item, title.Skin));
		}

		public void RandomizePlayerPreview()
		{
			this.InitPlayerPreview((ulong)UnityEngine.Random.Range(0, 2147483647), false);
		}

		public static void RemoveLODs(GameObject prefab)
		{
		}

		internal void SetColor(string paramName, Color val)
		{
			this.Skin.Materials[this.EditingMaterial].SetColor(paramName, val);
		}

		internal void SetFloat(string paramName, float value)
		{
			this.Skin.Materials[this.EditingMaterial].SetFloat(paramName, value);
			if (paramName == "_Cutoff")
			{
				if (value <= 0.1f)
				{
					this.Skin.Materials[this.EditingMaterial].SetOverrideTag("RenderType", "");
					this.Skin.Materials[this.EditingMaterial].DisableKeyword("_ALPHATEST_ON");
					this.Skin.Materials[this.EditingMaterial].renderQueue = -1;
				}
				else
				{
					this.Skin.Materials[this.EditingMaterial].SetOverrideTag("RenderType", "TransparentCutout");
					this.Skin.Materials[this.EditingMaterial].EnableKeyword("_ALPHATEST_ON");
					this.Skin.Materials[this.EditingMaterial].renderQueue = 2450;
				}
			}
			if (paramName == "_MicrofiberFuzzIntensity")
			{
				if (value > 0.1f)
				{
					this.Skin.Materials[this.EditingMaterial].EnableKeyword("_MICROFIBERFUZZLAYER_ON");
					return;
				}
				this.Skin.Materials[this.EditingMaterial].DisableKeyword("_MICROFIBERFUZZLAYER_ON");
			}
		}

		internal Texture2D SetTexture(string paramName, string fullName, bool isNormalMap)
		{
			byte[] numArray = File.ReadAllBytes(fullName);
			if (numArray == null)
			{
				throw new Exception("Couldn't Load Data");
			}
			Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, true, isNormalMap);
			if (!texture2D.LoadImage(numArray))
			{
				throw new Exception("Couldn't Load Image");
			}
			texture2D.name = fullName;
			texture2D = Facepunch.Utility.Texture.LimitSize(texture2D, this.Skinnable.Groups[this.EditingMaterial].MaxTextureSize, this.Skinnable.Groups[this.EditingMaterial].MaxTextureSize);
			if (isNormalMap)
			{
				texture2D.CompressNormals();
			}
			texture2D.anisoLevel = 16;
			texture2D.filterMode = FilterMode.Trilinear;
			this.SetTexture(paramName, texture2D);
			return texture2D;
		}

		internal void SetTexture(string paramName, UnityEngine.Texture tex)
		{
			this.Skin.Materials[this.EditingMaterial].SetTexture(paramName, tex);
			if (paramName == "_EmissionMap" && tex != null)
			{
				this.Skin.Materials[this.EditingMaterial].EnableKeyword("_EMISSION");
			}
		}

		private void ShowEditor()
		{
			GameObject[] editorElements = this.EditorElements;
			for (int i = 0; i < (int)editorElements.Length; i++)
			{
				editorElements[i].SetActive(true);
			}
		}

		public IEnumerator StartEditingItem(Item item)
		{
			WorkshopItemEditor workshopItemEditor = null;
			workshopItemEditor.Skin = null;
			workshopItemEditor.ClearEditor();
			workshopItemEditor.ShowEditor();
			yield return workshopItemEditor.StartCoroutine(workshopItemEditor.OpenItem(item));
			workshopItemEditor.OnImportFinished();
		}

		public void StartNewItem(string type = "TShirt")
		{
			this.Skin = null;
			this.ClearEditor();
			this.LoadItemType(type);
			this.OnImportFinished();
			this.ShowEditor();
		}

		public IEnumerator StartViewingItem(Item item)
		{
			WorkshopItemEditor workshopItemEditor = null;
			workshopItemEditor.Skin = null;
			workshopItemEditor.ClearEditor();
			workshopItemEditor.HideEditor();
			yield return workshopItemEditor.StartCoroutine(workshopItemEditor.OpenItem(item));
			workshopItemEditor.OnImportFinished();
		}

		public void SwitchMaterial(int i)
		{
			this.EditingMaterial = i;
			this.UpdateMaterialRows();
		}

		private void Update()
		{
			Facepunch.Input.Frame();
			Facepunch.Input.Update();
		}

		private void UpdateMaterialRows()
		{
			int i;
			this.UpdateMaterialTabs();
			Material materials = this.Skin.Materials[this.EditingMaterial];
			if (materials == null)
			{
				return;
			}
			Material defaultMaterials = this.Skin.DefaultMaterials[this.EditingMaterial];
			if (defaultMaterials == null)
			{
				return;
			}
			if (!materials.IsKeywordEnabled("_ALPHATEST_ON"))
			{
				materials.SetFloat("_Cutoff", 0f);
			}
			if (!defaultMaterials.IsKeywordEnabled("_ALPHATEST_ON"))
			{
				defaultMaterials.SetFloat("_Cutoff", 0f);
			}
			MaterialRow[] componentsInChildren = base.GetComponentsInChildren<MaterialRow>(true);
			for (i = 0; i < (int)componentsInChildren.Length; i++)
			{
				MaterialRow materialRow = componentsInChildren[i];
				if (materials.HasProperty(materialRow.ParamName))
				{
					materialRow.Read(materials, defaultMaterials);
				}
			}
			bool flag = materials.shader.name.Contains("Cloth");
			GameObject[] clothOnlyElements = this.ClothOnlyElements;
			for (i = 0; i < (int)clothOnlyElements.Length; i++)
			{
				clothOnlyElements[i].SetActive(flag);
			}
		}

		private void UpdateMaterialTabs()
		{
			for (int i = 0; i < (int)this.MaterialTabs.Length; i++)
			{
				if ((int)this.Skinnable.Groups.Length >= i + 1)
				{
					this.MaterialTabs[i].gameObject.SetActive(true);
					Text[] componentsInChildren = this.MaterialTabs[i].gameObject.GetComponentsInChildren<Text>(true);
					for (int j = 0; j < (int)componentsInChildren.Length; j++)
					{
						componentsInChildren[j].text = this.Skinnable.Groups[i].Name;
					}
				}
				else
				{
					this.MaterialTabs[i].gameObject.SetActive(false);
				}
			}
		}
	}
}