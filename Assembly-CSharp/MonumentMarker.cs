using System;
using UnityEngine;
using UnityEngine.UI;

public class MonumentMarker : MonoBehaviour
{
	public Text text;

	public MonumentMarker()
	{
	}

	public void Setup(MonumentInfo info)
	{
		string str = info.displayPhrase.translated;
		this.text.text = (string.IsNullOrEmpty(str) ? "Monument" : str);
	}
}