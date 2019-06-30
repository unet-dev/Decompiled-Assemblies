using Facepunch.Extend;
using Network;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.Debugging
{
	public class NetGraphRow : MonoBehaviour
	{
		public CanvasGroup @group;

		public Text countTotal;

		public Text countUnique;

		public Text label;

		public Text bytes;

		public GameObject RowContainer;

		public NetGraphItem[] Items;

		public GameObject RowSpacer;

		public NetGraphRow()
		{
		}

		private void Awake()
		{
			this.Items = base.GetComponentsInChildren<NetGraphItem>();
		}

		internal void Hide()
		{
			this.@group.alpha = 0f;
		}

		internal void UpdateFrom(KeyValuePair<string, Stats.Node> node)
		{
			int count;
			string str;
			this.@group.alpha = 1f;
			this.label.text = node.Key;
			this.bytes.text = node.Value.Bytes.FormatBytes<long>(false);
			this.countTotal.text = node.Value.Count.ToString("N0");
			Text text = this.countUnique;
			if (node.Value.Children == null)
			{
				str = "0";
			}
			else
			{
				count = node.Value.Children.Count;
				str = count.ToString("N0");
			}
			text.text = str;
			NetGraphItem[] items = this.Items;
			for (count = 0; count < (int)items.Length; count++)
			{
				items[count].Hide();
			}
			if (node.Value.Children == null)
			{
				return;
			}
			int num = 0;
			foreach (KeyValuePair<string, Stats.Node> keyValuePair in 
				from y in node.Value.Children
				orderby y.Value.Bytes descending
				select y)
			{
				if (num < (int)this.Items.Length)
				{
					this.Items[num].UpdateFrom(keyValuePair);
					num++;
				}
				else
				{
					return;
				}
			}
		}
	}
}