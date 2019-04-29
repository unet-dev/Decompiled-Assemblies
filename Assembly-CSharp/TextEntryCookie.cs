using Rust;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextEntryCookie : MonoBehaviour
{
	public InputField control
	{
		get
		{
			return base.GetComponent<InputField>();
		}
	}

	public TextEntryCookie()
	{
	}

	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		PlayerPrefs.SetString(string.Concat("TextEntryCookie_", base.name), this.control.text);
	}

	private void OnEnable()
	{
		string str = PlayerPrefs.GetString(string.Concat("TextEntryCookie_", base.name));
		if (!string.IsNullOrEmpty(str))
		{
			this.control.text = str;
		}
		this.control.onValueChanged.Invoke(this.control.text);
	}
}