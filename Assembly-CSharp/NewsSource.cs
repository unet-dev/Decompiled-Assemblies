using Facepunch.Extend;
using Facepunch.Math;
using JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewsSource : MonoBehaviour
{
	public NewsSource.Story[] story;

	public Text title;

	public Text date;

	public Text text;

	public Text authorName;

	public RawImage image;

	public VerticalLayoutGroup layoutGroup;

	public Button button;

	public NewsSource()
	{
	}

	private IEnumerator LoadHeaderImage(string url, int i)
	{
		NewsSource newsSource = null;
		newsSource.image.enabled = false;
		WWW wWW = new WWW(url);
		yield return wWW;
		if (!string.IsNullOrEmpty(wWW.error))
		{
			UnityEngine.Debug.LogWarning(string.Concat("Couldn't load header image: ", wWW.error));
			wWW.Dispose();
			yield break;
		}
		Texture2D texture2D = wWW.textureNonReadable;
		texture2D.name = url;
		newsSource.story[i].texture = texture2D;
		newsSource.SetHeadlineTexture(newsSource.story[i].texture);
		wWW.Dispose();
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.UpdateNews());
	}

	private void SetHeadlineTexture(Texture tex)
	{
		float single = (float)tex.height / (float)tex.width;
		this.image.texture = tex;
		RectTransform vector2 = this.image.rectTransform;
		Rect rect = this.image.rectTransform.rect;
		vector2.sizeDelta = new Vector2(0f, rect.width * single);
		this.image.enabled = true;
		RectOffset rectOffset = this.layoutGroup.padding;
		rect = this.image.rectTransform.rect;
		rectOffset.top = (int)(rect.width * single) / 2;
		this.layoutGroup.padding = rectOffset;
	}

	public void SetStory(int i)
	{
		if (this.story == null)
		{
			return;
		}
		if ((int)this.story.Length <= i)
		{
			return;
		}
		base.StopAllCoroutines();
		this.title.text = this.story[i].name;
		this.date.text = ((long)(Epoch.Current - this.story[i].date)).FormatSecondsLong();
		string str = Regex.Replace(this.story[i].text, "\\[img\\].*\\[\\/img\\]", string.Empty, RegexOptions.IgnoreCase);
		str = str.Replace("\\n", "\n").Replace("\\r", "").Replace("\\\"", "\"");
		str = str.Replace("[list]", "<color=#F7EBE1aa>");
		str = str.Replace("[/list]", "</color>");
		str = str.Replace("[*]", "\t\t» ");
		str = Regex.Replace(str, "\\[(.*?)\\]", string.Empty, RegexOptions.IgnoreCase);
		str = str.Trim();
		Match match = Regex.Match(this.story[i].text, "url=(http|https):\\/\\/([\\w\\-_]+(?:(?:\\.[\\w\\-_]+)+))([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])");
		Match match1 = Regex.Match(this.story[i].text, "(http|https):\\/\\/([\\w\\-_]+(?:(?:\\.[\\w\\-_]+)+))([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])(.png|.jpg)");
		if (match == null)
		{
			this.button.gameObject.SetActive(false);
		}
		else
		{
			string str1 = match.Value.Replace("url=", "");
			if (str1 == null || str1.Trim().Length <= 0)
			{
				str1 = this.story[i].url;
			}
			this.button.gameObject.SetActive(true);
			this.button.onClick.RemoveAllListeners();
			this.button.onClick.AddListener(() => {
				UnityEngine.Debug.Log(string.Concat("Opening URL: ", str1));
				Application.OpenURL(str1);
			});
		}
		this.text.text = str;
		this.authorName.text = string.Format("posted by {0}", this.story[i].author);
		if (this.image != null)
		{
			if (this.story[i].texture)
			{
				this.SetHeadlineTexture(this.story[i].texture);
				return;
			}
			if (match1 != null)
			{
				base.StartCoroutine(this.LoadHeaderImage(match1.Value, i));
			}
		}
	}

	private IEnumerator UpdateNews()
	{
		NewsSource array = null;
		WWW wWW = new WWW("http://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=252490&count=8&format=json&feeds=steam_community_announcements");
		yield return wWW;
		JSON.Object objs = JSON.Object.Parse(wWW.text);
		wWW.Dispose();
		if (objs == null)
		{
			yield break;
		}
		JSON.Array arrays = objs.GetObject("appnews").GetArray("newsitems");
		List<NewsSource.Story> stories = new List<NewsSource.Story>();
		foreach (Value value in arrays)
		{
			string str = value.Obj.GetString("contents", "Missing URL");
			NewsSource.Story story = new NewsSource.Story()
			{
				name = value.Obj.GetString("title", "Missing Title"),
				url = value.Obj.GetString("url", "Missing URL"),
				date = value.Obj.GetInt("date", 0),
				text = str,
				author = value.Obj.GetString("author", "Missing Author")
			};
			stories.Add(story);
		}
		array.story = stories.ToArray();
		array.SetStory(0);
	}

	public struct Story
	{
		public string name;

		public string url;

		public int date;

		public string text;

		public string author;

		public Texture texture;
	}
}