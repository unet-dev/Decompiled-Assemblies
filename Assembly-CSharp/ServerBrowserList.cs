using System;

public class ServerBrowserList : BaseMonoBehaviour
{
	public ServerBrowserCategory categoryButton;

	public bool startActive;

	public ServerBrowserItem itemTemplate;

	public int refreshOrder;

	public bool UseOfficialServers;

	public ServerBrowserItem[] items;

	public ServerBrowserList.Rules[] rules;

	public ServerBrowserList.QueryType queryType;

	public static string VersionTag;

	public ServerBrowserList.ServerKeyvalues[] keyValues = new ServerBrowserList.ServerKeyvalues[0];

	static ServerBrowserList()
	{
		ServerBrowserList.VersionTag = string.Concat("v", 2163);
	}

	public ServerBrowserList()
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