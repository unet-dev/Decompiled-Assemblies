using Facepunch.Extend;
using Facepunch.Math;
using GameAnalyticsSDK;
using Rust.UI;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewsSource : MonoBehaviour
{
	public TextMeshProUGUI title;

	public TextMeshProUGUI date;

	public TextMeshProUGUI text;

	public TextMeshProUGUI authorName;

	public HttpImage image;

	public VerticalLayoutGroup layoutGroup;

	public Button button;

	public NewsSource()
	{
	}

	private void Awake()
	{
		GameAnalytics.NewDesignEvent("news:view");
	}

	private void OnEnable()
	{
		if (SteamNewsSource.Stories == null || SteamNewsSource.Stories.Length == 0)
		{
			return;
		}
		this.SetStory(SteamNewsSource.Stories[0]);
	}

	public void SetStory(SteamNewsSource.Story story)
	{
		PlayerPrefs.SetInt("lastNewsDate", story.date);
		this.title.text = story.name;
		string str = ((long)(Epoch.Current - story.date)).FormatSecondsLong();
		this.date.text = string.Concat("Posted ", str, " ago");
		string str1 = Regex.Replace(story.text, "\\[img\\].*\\[\\/img\\]", string.Empty, RegexOptions.IgnoreCase);
		str1 = str1.Replace("\\n", "\n").Replace("\\r", "").Replace("\\\"", "\"");
		str1 = str1.Replace("[list]", "<color=#F7EBE1aa>");
		str1 = str1.Replace("[/list]", "</color>");
		str1 = str1.Replace("[*]", "\t\t» ");
		str1 = Regex.Replace(str1, "\\[(.*?)\\]", string.Empty, RegexOptions.IgnoreCase);
		str1 = str1.Trim();
		Match match = Regex.Match(story.text, "url=(http|https):\\/\\/([\\w\\-_]+(?:(?:\\.[\\w\\-_]+)+))([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])");
		Match match1 = Regex.Match(story.text, "(http|https):\\/\\/([\\w\\-_]+(?:(?:\\.[\\w\\-_]+)+))([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])(.png|.jpg)");
		if (match == null)
		{
			this.button.gameObject.SetActive(false);
		}
		else
		{
			string str2 = match.Value.Replace("url=", "");
			if (str2 == null || str2.Trim().Length <= 0)
			{
				str2 = story.url;
			}
			this.button.gameObject.SetActive(true);
			this.button.onClick.RemoveAllListeners();
			this.button.onClick.AddListener(() => {
				Debug.Log(string.Concat("Opening URL: ", str2));
				Application.OpenURL(str2);
			});
		}
		this.text.text = str1;
		this.authorName.text = string.Format("by {0}", story.author);
		if (match1 != null)
		{
			this.image.Load(match1.Value);
		}
	}
}