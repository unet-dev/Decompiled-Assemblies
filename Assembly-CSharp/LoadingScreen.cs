using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : SingletonComponent<LoadingScreen>
{
	public CanvasRenderer panel;

	public TextMeshProUGUI title;

	public TextMeshProUGUI subtitle;

	public Button skipButton;

	public AudioSource music;

	public static bool isOpen
	{
		get
		{
			if (!SingletonComponent<LoadingScreen>.Instance || !SingletonComponent<LoadingScreen>.Instance.panel)
			{
				return false;
			}
			return SingletonComponent<LoadingScreen>.Instance.panel.gameObject.activeSelf;
		}
	}

	public static string Text
	{
		get;
		private set;
	}

	public static bool WantsSkip
	{
		get;
		private set;
	}

	public LoadingScreen()
	{
	}

	public static void Update(string strType)
	{
	}

	public static void Update(string strType, string strSubtitle)
	{
	}
}