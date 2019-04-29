using Facepunch.Steamworks;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Rust.Workshop
{
	public class WorkshopSkin : MonoBehaviour
	{
		public static bool AllowApply;

		public static float DownloadTimeout;

		private static WaitForSeconds waitForSeconds;

		private static AssetBundleCreateRequest[] bundleRequests;

		private static AssetBundle[] bundles;

		private static ListDictionary<ulong, ListHashSet<WorkshopSkin>> RefreshQueue;

		private static Queue<ulong> ItemQueue;

		private static Queue<ulong> SkinQueue;

		private static ListDictionary<ulong, Facepunch.Steamworks.Workshop.Item> ItemCache;

		private static ListDictionary<ulong, Skin> SkinCache;

		private ulong WorkshopID;

		private Action OnRefresh;

		public static int LoadedCount
		{
			get
			{
				return WorkshopSkin.ItemCache.Count + WorkshopSkin.SkinCache.Count;
			}
		}

		public static int QueuedCount
		{
			get
			{
				return WorkshopSkin.ItemQueue.Count + WorkshopSkin.SkinQueue.Count;
			}
		}

		static WorkshopSkin()
		{
			WorkshopSkin.AllowApply = true;
			WorkshopSkin.DownloadTimeout = 60f;
			WorkshopSkin.waitForSeconds = new WaitForSeconds(1f);
			WorkshopSkin.bundleRequests = new AssetBundleCreateRequest[10];
			WorkshopSkin.bundles = new AssetBundle[10];
			WorkshopSkin.RefreshQueue = new ListDictionary<ulong, ListHashSet<WorkshopSkin>>(8);
			WorkshopSkin.ItemQueue = new Queue<ulong>();
			WorkshopSkin.SkinQueue = new Queue<ulong>();
			WorkshopSkin.ItemCache = new ListDictionary<ulong, Facepunch.Steamworks.Workshop.Item>(8);
			WorkshopSkin.SkinCache = new ListDictionary<ulong, Skin>(8);
		}

		public WorkshopSkin()
		{
		}

		public static void Apply(GameObject gameobj, ulong workshopId, Action callback = null)
		{
			if (Global.SteamClient == null)
			{
				return;
			}
			WorkshopSkin component = gameobj.GetComponent<WorkshopSkin>();
			if (component == null)
			{
				component = gameobj.AddComponent<WorkshopSkin>();
			}
			component.Apply(workshopId, callback);
		}

		private void Apply(ulong workshopId, Action callback = null)
		{
			this.DequeueSkinRefresh(this.WorkshopID);
			this.WorkshopID = workshopId;
			this.OnRefresh = callback;
			Skin skin = null;
			if (WorkshopSkin.SkinCache.TryGetValue(this.WorkshopID, out skin) && skin.Loaded)
			{
				this.ApplySkin(skin);
				return;
			}
			this.EnqueueSkinRefresh(workshopId);
		}

		private void ApplySkin(Skin skin)
		{
			TimeWarning.BeginSample("WorkshopSkin.ApplySkin");
			if (!this)
			{
				TimeWarning.EndSample();
				return;
			}
			if (!base.gameObject)
			{
				TimeWarning.EndSample();
				return;
			}
			if (WorkshopSkin.AllowApply)
			{
				skin.Apply(base.gameObject);
			}
			if (this.OnRefresh != null)
			{
				this.OnRefresh();
			}
			TimeWarning.EndSample();
		}

		private void DequeueSkinRefresh(ulong workshopId)
		{
			if (workshopId == 0)
			{
				return;
			}
			ListHashSet<WorkshopSkin> workshopSkins = null;
			if (WorkshopSkin.RefreshQueue.TryGetValue(workshopId, out workshopSkins))
			{
				workshopSkins.Remove(this);
				if (workshopSkins.Count == 0)
				{
					WorkshopSkin.RefreshQueue.Remove(workshopId);
				}
			}
		}

		private void EnqueueSkinRefresh(ulong workshopId)
		{
			if (workshopId == 0)
			{
				return;
			}
			ListHashSet<WorkshopSkin> workshopSkins = null;
			if (!WorkshopSkin.RefreshQueue.TryGetValue(workshopId, out workshopSkins))
			{
				ListDictionary<ulong, ListHashSet<WorkshopSkin>> refreshQueue = WorkshopSkin.RefreshQueue;
				ListHashSet<WorkshopSkin> workshopSkins1 = new ListHashSet<WorkshopSkin>(8);
				workshopSkins = workshopSkins1;
				refreshQueue.Add(workshopId, workshopSkins1);
			}
			workshopSkins.Add(this);
			WorkshopSkin.LoadFromWorkshop(workshopId);
		}

		public static Facepunch.Steamworks.Workshop.Item GetItem(ulong workshopId)
		{
			WorkshopSkin.LoadFromWorkshop(workshopId);
			return WorkshopSkin.ItemCache[workshopId];
		}

		public static Skin GetSkin(ulong workshopId)
		{
			WorkshopSkin.LoadFromWorkshop(workshopId);
			return WorkshopSkin.SkinCache[workshopId];
		}

		public static string GetStatus()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = WorkshopSkin.ItemCache.Count;
			int num = WorkshopSkin.ItemQueue.Count;
			int count1 = WorkshopSkin.SkinCache.Count;
			int num1 = WorkshopSkin.SkinQueue.Count;
			if (WorkshopSkin.ItemCache.Count > 0)
			{
				TextTable textTable = new TextTable();
				textTable.AddColumn("id");
				textTable.AddColumn("status");
				foreach (KeyValuePair<ulong, Facepunch.Steamworks.Workshop.Item> itemCache in WorkshopSkin.ItemCache)
				{
					ulong key = itemCache.Key;
					string str = (itemCache.Value.Installed ? "loaded" : "loading");
					textTable.AddRow(new string[] { key.ToString(), str });
				}
				stringBuilder.Append(textTable.ToString());
				stringBuilder.AppendLine();
			}
			stringBuilder.Append("Items: ");
			stringBuilder.Append(count);
			stringBuilder.Append(" in cache + ");
			stringBuilder.Append(num);
			stringBuilder.Append(" loading");
			stringBuilder.AppendLine();
			stringBuilder.Append("Skins: ");
			stringBuilder.Append(count1);
			stringBuilder.Append(" in cache + ");
			stringBuilder.Append(num1);
			stringBuilder.Append(" loading");
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		private static IEnumerator ItemQueueCoroutine()
		{
			while (WorkshopSkin.ItemQueue.Count > 0)
			{
				ulong num = WorkshopSkin.ItemQueue.Peek();
				yield return Global.Runner.StartCoroutine(WorkshopSkin.LoadItem(num));
				WorkshopSkin.ItemQueue.Dequeue();
			}
		}

		private static void LoadFromWorkshop(ulong workshopId)
		{
			if (!WorkshopSkin.ItemCache.Contains(workshopId))
			{
				TimeWarning.BeginSample("Workshop.GetItem");
				Facepunch.Steamworks.Workshop.Item item = Global.SteamClient.Workshop.GetItem(workshopId);
				TimeWarning.EndSample();
				TimeWarning.BeginSample("ItemCache.Add");
				WorkshopSkin.ItemCache.Add(workshopId, item);
				TimeWarning.EndSample();
				TimeWarning.BeginSample("Skin.New");
				Skin skin = new Skin();
				TimeWarning.EndSample();
				TimeWarning.BeginSample("SkinCache.Add");
				WorkshopSkin.SkinCache.Add(workshopId, skin);
				TimeWarning.EndSample();
				WorkshopSkin.ItemQueue.Enqueue(workshopId);
				if (WorkshopSkin.ItemQueue.Count == 1)
				{
					Global.Runner.StartCoroutine(WorkshopSkin.ItemQueueCoroutine());
				}
			}
		}

		private static IEnumerator LoadItem(ulong workshopId)
		{
			int num = (int)(workshopId / (long)100000000 % (long)10);
			AssetBundle assetBundle = WorkshopSkin.bundles[num];
			if (assetBundle == null)
			{
				AssetBundleCreateRequest assetBundleCreateRequest = WorkshopSkin.bundleRequests[num];
				if (assetBundleCreateRequest == null)
				{
					TimeWarning.BeginSample("AssetBundle.LoadFromFileAsync");
					AssetBundleCreateRequest[] assetBundleCreateRequestArray = WorkshopSkin.bundleRequests;
					int num1 = num;
					AssetBundleCreateRequest assetBundleCreateRequest1 = AssetBundle.LoadFromFileAsync(string.Concat("Bundles/textures/textures.", num, ".bundle"));
					AssetBundleCreateRequest assetBundleCreateRequest2 = assetBundleCreateRequest1;
					assetBundleCreateRequestArray[num1] = assetBundleCreateRequest1;
					assetBundleCreateRequest = assetBundleCreateRequest2;
					TimeWarning.EndSample();
				}
				yield return assetBundleCreateRequest;
				TimeWarning.BeginSample("BundleRequest");
				AssetBundle[] assetBundleArray = WorkshopSkin.bundles;
				int num2 = num;
				AssetBundle assetBundle1 = WorkshopSkin.bundleRequests[num].assetBundle;
				AssetBundle assetBundle2 = assetBundle1;
				assetBundleArray[num2] = assetBundle1;
				assetBundle = assetBundle2;
				TimeWarning.EndSample();
			}
			TimeWarning.BeginSample("WorkshopSkin.LoadItem");
			TimeWarning.BeginSample("ItemCache.Get");
			Facepunch.Steamworks.Workshop.Item item = WorkshopSkin.ItemCache[workshopId];
			TimeWarning.EndSample();
			TimeWarning.BeginSample("Item.Installed");
			bool installed = item.Installed;
			TimeWarning.EndSample();
			if (!installed && assetBundle != null)
			{
				TimeWarning.BeginSample("Bundle.Contains");
				installed = assetBundle.Contains(string.Concat("Assets/Skins/", workshopId, "/manifest.txt"));
				TimeWarning.EndSample();
			}
			if (!installed)
			{
				TimeWarning.BeginSample("Item.Download");
				bool flag = item.Download(true);
				TimeWarning.EndSample();
				if (flag)
				{
					TimeWarning.BeginSample("Stopwatch.StartNew");
					Stopwatch stopwatch = Stopwatch.StartNew();
					TimeWarning.EndSample();
					TimeWarning.BeginSample("Item.Installed");
					while (!item.Installed && stopwatch.Elapsed.TotalSeconds < (double)WorkshopSkin.DownloadTimeout)
					{
						TimeWarning.EndSample();
						TimeWarning.EndSample();
						yield return WorkshopSkin.waitForSeconds;
						TimeWarning.BeginSample("WorkshopSkin.LoadItem");
						TimeWarning.BeginSample("Item.Installed");
					}
					TimeWarning.EndSample();
					TimeWarning.BeginSample("Item.Installed");
					installed = item.Installed;
					TimeWarning.EndSample();
					stopwatch = null;
				}
				if (!flag)
				{
					UnityEngine.Debug.LogWarning(string.Concat("Skin download failed: ", workshopId));
				}
				else if (!installed)
				{
					UnityEngine.Debug.LogWarning(string.Concat("Skin download timed out: ", workshopId));
				}
			}
			if (installed)
			{
				WorkshopSkin.SkinQueue.Enqueue(workshopId);
				if (WorkshopSkin.SkinQueue.Count == 1)
				{
					Global.Runner.StartCoroutine(WorkshopSkin.SkinQueueCoroutine());
				}
			}
			TimeWarning.EndSample();
		}

		private static IEnumerator LoadSkin(ulong workshopId)
		{
			int num = (int)(workshopId / (long)100000000 % (long)10);
			AssetBundle assetBundle = WorkshopSkin.bundles[num];
			TimeWarning.BeginSample("WorkshopSkin.LoadSkin");
			TimeWarning.BeginSample("ItemCache.Get");
			Facepunch.Steamworks.Workshop.Item item = WorkshopSkin.ItemCache[workshopId];
			TimeWarning.EndSample();
			TimeWarning.BeginSample("SkinCache.Get");
			Skin skin = WorkshopSkin.SkinCache[workshopId];
			TimeWarning.EndSample();
			TimeWarning.EndSample();
			yield return Global.Runner.StartCoroutine(skin.FromFolder(workshopId, item.Directory, assetBundle));
			TimeWarning.BeginSample("WorkshopSkin.LoadSkin");
			if (WorkshopSkin.RefreshQueue.Contains(workshopId))
			{
				ListHashSet<WorkshopSkin> workshopSkins = WorkshopSkin.RefreshQueue[workshopId];
				while (workshopSkins.Count > 0)
				{
					workshopSkins[0].ApplySkin(skin);
					workshopSkins.RemoveAt(0);
					TimeWarning.EndSample();
					yield return null;
					TimeWarning.BeginSample("WorkshopSkin.LoadSkin");
				}
				WorkshopSkin.RefreshQueue.Remove(workshopId);
				workshopSkins = null;
			}
			TimeWarning.EndSample();
		}

		protected void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			this.DequeueSkinRefresh(this.WorkshopID);
		}

		public static void Prepare(GameObject gameobj)
		{
			if (gameobj.GetComponent<WorkshopSkin>() == null)
			{
				gameobj.AddComponent<WorkshopSkin>();
			}
		}

		private static IEnumerator SkinQueueCoroutine()
		{
			while (WorkshopSkin.SkinQueue.Count > 0)
			{
				ulong num = WorkshopSkin.SkinQueue.Peek();
				yield return Global.Runner.StartCoroutine(WorkshopSkin.LoadSkin(num));
				WorkshopSkin.SkinQueue.Dequeue();
			}
		}
	}
}