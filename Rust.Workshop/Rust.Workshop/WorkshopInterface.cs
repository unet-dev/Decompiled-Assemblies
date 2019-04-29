using Facepunch;
using Facepunch.Steamworks;
using Facepunch.Utility;
using Rust;
using Rust.UI;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rust.Workshop
{
	public class WorkshopInterface : MonoBehaviour
	{
		public GameObject item_position_a;

		public GameObject item_position_b;

		private string _backgroundScene;

		public string BackgroundScene
		{
			get
			{
				return this._backgroundScene;
			}
			set
			{
				if (this._backgroundScene != null)
				{
					SceneManager.UnloadScene(this._backgroundScene);
				}
				this._backgroundScene = value;
				SceneManager.LoadScene(this._backgroundScene, LoadSceneMode.Additive);
			}
		}

		internal WorkshopItemEditor Editor
		{
			get
			{
				return base.GetComponentInChildren<WorkshopItemEditor>(true);
			}
		}

		internal Rust.UI.LoadingBar LoadingBar
		{
			get
			{
				return base.GetComponentInChildren<Rust.UI.LoadingBar>(true);
			}
		}

		internal WorkshopMenu Menu
		{
			get
			{
				return base.GetComponentInChildren<WorkshopMenu>(true);
			}
		}

		public WorkshopInterface()
		{
		}

		public void Awake()
		{
			this.BackgroundScene = "Skybox";
		}

		public void CreateNewItem()
		{
			QualitySettings.masterTextureLimit = 0;
			QualitySettings.SetQualityLevel(5, true);
			this.Editor.gameObject.SetActive(true);
			this.Menu.gameObject.SetActive(false);
			this.Editor.StartNewItem("TShirt");
		}

		public void Exit()
		{
			UnityEngine.Object.Destroy(base.gameObject);
			Global.OpenMainMenu();
		}

		public void OpenMenu()
		{
			this.Editor.gameObject.SetActive(false);
			this.Menu.gameObject.SetActive(true);
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

		public void StartEditing(Facepunch.Steamworks.Workshop.Item item)
		{
			QualitySettings.masterTextureLimit = 0;
			QualitySettings.SetQualityLevel(5, true);
			this.Editor.gameObject.SetActive(true);
			this.Menu.gameObject.SetActive(false);
			this.Editor.StartEditingItem(item);
		}

		public void StartViewing(Facepunch.Steamworks.Workshop.Item item)
		{
			QualitySettings.masterTextureLimit = 0;
			QualitySettings.SetQualityLevel(5, true);
			this.Editor.gameObject.SetActive(true);
			this.Menu.gameObject.SetActive(false);
			this.Editor.StartViewingItem(item);
		}

		public void Update()
		{
			this.item_position_a = GameObject.Find("item_position_a");
			this.item_position_b = GameObject.Find("item_position_b");
			Facepunch.Input.Frame();
			Facepunch.Input.Update();
		}
	}
}