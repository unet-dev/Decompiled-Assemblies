using JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class SteamNewsSource
{
	public static SteamNewsSource.Story[] Stories;

	public static IEnumerator GetStories()
	{
		WWW wWW = new WWW("http://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=252490&count=8&format=json&feeds=steam_community_announcements");
		yield return wWW;
		JSON.Object objs = JSON.Object.Parse(wWW.text);
		wWW.Dispose();
		if (objs == null)
		{
			yield break;
		}
		JSON.Array array = objs.GetObject("appnews").GetArray("newsitems");
		List<SteamNewsSource.Story> stories = new List<SteamNewsSource.Story>();
		foreach (Value value in array)
		{
			string str = value.Obj.GetString("contents", "Missing URL");
			SteamNewsSource.Story story = new SteamNewsSource.Story()
			{
				name = value.Obj.GetString("title", "Missing Title"),
				url = value.Obj.GetString("url", "Missing URL"),
				date = value.Obj.GetInt("date", 0),
				text = str,
				author = value.Obj.GetString("author", "Missing Author")
			};
			stories.Add(story);
		}
		SteamNewsSource.Stories = stories.ToArray();
	}

	public struct Story
	{
		public string name;

		public string url;

		public int date;

		public string text;

		public string author;
	}
}