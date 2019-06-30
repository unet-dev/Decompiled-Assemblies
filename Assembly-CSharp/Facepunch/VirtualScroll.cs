using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Facepunch
{
	public class VirtualScroll : MonoBehaviour
	{
		public int ItemHeight = 40;

		public int ItemSpacing = 10;

		public RectOffset Padding;

		[Tooltip("Optional, we'll try to GetComponent IDataSource from this object on awake")]
		public GameObject DataSourceObject;

		public GameObject SourceObject;

		public UnityEngine.UI.ScrollRect ScrollRect;

		private VirtualScroll.IDataSource dataSource;

		private Dictionary<int, GameObject> ActivePool = new Dictionary<int, GameObject>();

		private Stack<GameObject> InactivePool = new Stack<GameObject>();

		private int BlockHeight
		{
			get
			{
				return this.ItemHeight + this.ItemSpacing;
			}
		}

		public VirtualScroll()
		{
		}

		public void Awake()
		{
			this.ScrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnScrollChanged));
			if (this.DataSourceObject != null)
			{
				this.SetDataSource(this.DataSourceObject.GetComponent<VirtualScroll.IDataSource>());
			}
		}

		private void BuildItem(int i)
		{
			if (i < 0)
			{
				return;
			}
			if (this.ActivePool.ContainsKey(i))
			{
				return;
			}
			GameObject item = this.GetItem();
			item.SetActive(true);
			this.dataSource.SetItemData(i, item);
			RectTransform vector2 = item.transform as RectTransform;
			vector2.anchorMin = new Vector2(0f, 1f);
			vector2.anchorMax = new Vector2(1f, 1f);
			vector2.pivot = new Vector2(0.5f, 1f);
			vector2.offsetMin = new Vector2(0f, 0f);
			vector2.offsetMax = new Vector2(0f, (float)this.ItemHeight);
			vector2.sizeDelta = new Vector2((float)((this.Padding.left + this.Padding.right) * -1), (float)this.ItemHeight);
			vector2.anchoredPosition = new Vector2((float)(this.Padding.left - this.Padding.right) * 0.5f, (float)(-1 * (i * this.BlockHeight + this.Padding.top)));
			this.ActivePool[i] = item;
		}

		public void DataChanged()
		{
			foreach (KeyValuePair<int, GameObject> activePool in this.ActivePool)
			{
				this.dataSource.SetItemData(activePool.Key, activePool.Value);
			}
			this.Rebuild();
		}

		public void FullRebuild()
		{
			int[] array = this.ActivePool.Keys.ToArray<int>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				this.Recycle(array[i]);
			}
			this.Rebuild();
		}

		private GameObject GetItem()
		{
			if (this.InactivePool.Count == 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.SourceObject);
				gameObject.transform.SetParent(this.ScrollRect.viewport.GetChild(0), false);
				gameObject.transform.localScale = Vector3.one;
				gameObject.SetActive(false);
				this.InactivePool.Push(gameObject);
			}
			return this.InactivePool.Pop();
		}

		public void OnDestroy()
		{
			this.ScrollRect.onValueChanged.RemoveListener(new UnityAction<Vector2>(this.OnScrollChanged));
		}

		private void OnScrollChanged(Vector2 pos)
		{
			this.Rebuild();
		}

		public void Rebuild()
		{
			if (this.dataSource == null)
			{
				return;
			}
			int itemCount = this.dataSource.GetItemCount();
			RectTransform child = this.ScrollRect.viewport.GetChild(0) as RectTransform;
			child.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)(this.BlockHeight * itemCount - this.ItemSpacing + this.Padding.top + this.Padding.bottom));
			Rect scrollRect = this.ScrollRect.viewport.rect;
			int num = Mathf.Max(2, Mathf.CeilToInt(scrollRect.height / (float)this.BlockHeight));
			int num1 = Mathf.FloorToInt((child.anchoredPosition.y - (float)this.Padding.top) / (float)this.BlockHeight);
			int num2 = num1 + num;
			this.RecycleOutOfRange(num1, (float)num2);
			for (int i = num1; i <= num2; i++)
			{
				if (i >= 0 && i < itemCount)
				{
					this.BuildItem(i);
				}
			}
		}

		private void Recycle(int key)
		{
			GameObject item = this.ActivePool[key];
			item.SetActive(false);
			this.ActivePool.Remove(key);
			this.InactivePool.Push(item);
		}

		private void RecycleOutOfRange(int startVisible, float endVisible)
		{
			int[] array = this.ActivePool.Keys.Where<int>((int x) => {
				if (x < startVisible)
				{
					return true;
				}
				return (float)x > endVisible;
			}).Select<int, int>((int x) => x).ToArray<int>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				this.Recycle(array[i]);
			}
		}

		public void SetDataSource(VirtualScroll.IDataSource source)
		{
			if (this.dataSource == source)
			{
				return;
			}
			this.dataSource = source;
			this.FullRebuild();
		}

		public interface IDataSource
		{
			int GetItemCount();

			void SetItemData(int i, GameObject obj);
		}
	}
}