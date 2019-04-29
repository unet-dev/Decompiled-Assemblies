using Facepunch.Extend;
using Rust;
using Rust.Workshop;
using System;
using UnityEngine;

namespace Rust.Workshop.Editor
{
	internal class WorkshopPlayerPreview : MonoBehaviour
	{
		internal GameObject Instance;

		protected WorkshopInterface Interface
		{
			get
			{
				return base.GetComponentInParent<WorkshopInterface>();
			}
		}

		public WorkshopPlayerPreview()
		{
		}

		public void Cleanup()
		{
			if (this.Instance != null)
			{
				UnityEngine.Object.Destroy(this.Instance);
				this.Instance = null;
			}
		}

		private void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			this.Cleanup();
		}

		public void Setup(GameObject ClothingPrefab, ulong id, bool focus, bool IsClothing)
		{
			this.Cleanup();
			this.Instance = Global.CreatePrefab("assets/prefabs/player/player_model.prefab");
			this.Instance.transform.position = Vector3.zero;
			this.Instance.transform.rotation = Quaternion.identity;
			this.Instance.SetActive(true);
			ClothingPrefab.transform.SetParent(this.Instance.transform);
			ClothingPrefab.SetActive(false);
			this.Instance.SendMessage("ForceModelSeed", id);
			this.Instance.SendMessage("WorkshopPreviewSetup", new GameObject[] { ClothingPrefab });
			this.Instance.transform.position = this.Interface.item_position_b.transform.position;
			this.Instance.transform.rotation = this.Interface.item_position_b.transform.rotation;
			if (focus)
			{
				Camera.main.FocusOnRenderer(this.Instance, new Vector3(0.3f, 0.1f, 1f), Vector3.up, -1);
			}
		}
	}
}