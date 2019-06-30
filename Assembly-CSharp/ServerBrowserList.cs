using Facepunch;
using System;
using UnityEngine;

public class ServerBrowserList : BaseMonoBehaviour, Facepunch.VirtualScroll.IDataSource
{
	public ServerBrowserCategory categoryButton;

	public bool startActive;

	public Transform listTransform;

	public int refreshOrder;

	public bool UseOfficialServers;

	public Facepunch.VirtualScroll VirtualScroll;

	public ServerBrowserList.Rules[] rules;

	public ServerBrowserList.QueryType queryType;

	public static string VersionTag;

	public ServerBrowserList.ServerKeyvalues[] keyValues = new ServerBrowserList.ServerKeyvalues[0];

	static ServerBrowserList()
	{
		ServerBrowserList.VersionTag = string.Concat("v", 2177);
	}

	public ServerBrowserList()
	{
	}

	public int GetItemCount()
	{
		return 0;
	}

	public void SetItemData(int i, GameObject obj)
	{
	}

	public enum QueryType
	{
		RegularInternet,
		Friends,
		History,
		LAN,
		Favourites,
		None
	}

	[Serializable]
	public struct Rules
	{
		public string tag;

		public ServerBrowserList serverList;
	}

	[Serializable]
	public struct ServerKeyvalues
	{
		public string key;

		public string @value;
	}
}