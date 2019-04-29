using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facepunch.GUI
{
	internal class TabbedPanel
	{
		private int selectedTabID;

		private List<TabbedPanel.Tab> tabs = new List<TabbedPanel.Tab>();

		public TabbedPanel.Tab selectedTab
		{
			get
			{
				return this.tabs[this.selectedTabID];
			}
		}

		public TabbedPanel()
		{
		}

		public void Add(TabbedPanel.Tab tab)
		{
			this.tabs.Add(tab);
		}

		internal void DrawContents()
		{
			if (this.selectedTabID < 0)
			{
				return;
			}
			TabbedPanel.Tab tab = this.selectedTab;
			GUILayout.BeginVertical(new GUIStyle("devtabcontents"), new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true) });
			if (tab.drawFunc != null)
			{
				tab.drawFunc();
			}
			GUILayout.EndVertical();
		}

		internal void DrawVertical(float width)
		{
			GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(width), GUILayout.ExpandHeight(true) });
			for (int i = 0; i < this.tabs.Count; i++)
			{
				if (GUILayout.Toggle(this.selectedTabID == i, this.tabs[i].name, new GUIStyle("devtab"), Array.Empty<GUILayoutOption>()))
				{
					this.selectedTabID = i;
				}
			}
			if (GUILayout.Toggle(false, "", new GUIStyle("devtab"), new GUILayoutOption[] { GUILayout.ExpandHeight(true) }))
			{
				this.selectedTabID = -1;
			}
			GUILayout.EndVertical();
		}

		public struct Tab
		{
			public string name;

			public Action drawFunc;
		}
	}
}