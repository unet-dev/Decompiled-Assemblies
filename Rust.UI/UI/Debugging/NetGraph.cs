using Network;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.UI.Debugging
{
	public class NetGraph : SingletonComponent<NetGraph>
	{
		public CanvasGroup @group;

		private NetGraphRow[] rows;

		public bool Enabled
		{
			set
			{
				object obj;
				CanvasGroup canvasGroup = this.@group;
				if (value)
				{
					obj = 1;
				}
				else
				{
					obj = null;
				}
				canvasGroup.alpha = (float)obj;
			}
		}

		public NetGraph()
		{
		}

		public void Start()
		{
			this.rows = base.GetComponentsInChildren<NetGraphRow>(true);
		}

		public void UpdateFrom(Stats incomingStats)
		{
			for (int i = 0; i < (int)this.rows.Length; i++)
			{
				this.rows[i].Hide();
			}
			int num = 0;
			foreach (KeyValuePair<string, Stats.Node> keyValuePair in 
				from y in incomingStats.Previous.Children
				orderby y.Value.Bytes descending
				select y)
			{
				if (num < (int)this.rows.Length)
				{
					this.rows[num].UpdateFrom(keyValuePair);
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