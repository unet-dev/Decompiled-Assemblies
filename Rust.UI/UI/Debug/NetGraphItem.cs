using Facepunch.Extend;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.Debug
{
	public class NetGraphItem : MonoBehaviour
	{
		public CanvasGroup @group;

		public LayoutElement element;

		public Text countTotal;

		public Text label;

		public Text bytes;

		public NetGraphItem()
		{
		}

		internal void Hide()
		{
			this.@group.alpha = 0f;
			this.element.ignoreLayout = true;
		}

		internal void UpdateFrom(KeyValuePair<string, Stats.Node> node)
		{
			this.element.ignoreLayout = false;
			this.@group.alpha = 1f;
			this.element.flexibleWidth = (float)node.Value.Bytes / 2048f;
			this.label.text = node.Key;
			this.bytes.text = node.Value.Bytes.FormatBytes<long>(false);
			this.countTotal.text = node.Value.Count.ToString("N0");
		}
	}
}