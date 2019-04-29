using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : SingletonComponent<LoadingScreen>
{
	public CanvasRenderer panel;

	public UnityEngine.UI.Text title;

	public UnityEngine.UI.Text subtitle;

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

	protected override void Awake()
	{
		base.Awake();
		LoadingScreen.HideSkip();
		LoadingScreen.Hide();
	}

	public void CancelLoading()
	{
		ConsoleSystem.Option client = ConsoleSystem.Option.Client;
		ConsoleSystem.Run(client.Quiet(), "client.disconnect", Array.Empty<object>());
	}

	public static void Hide()
	{
		if (!SingletonComponent<LoadingScreen>.Instance)
		{
			return;
		}
		if (!SingletonComponent<LoadingScreen>.Instance.panel)
		{
			return;
		}
		if (!SingletonComponent<LoadingScreen>.Instance.panel.gameObject)
		{
			return;
		}
		if (!SingletonComponent<LoadingScreen>.Instance.panel.gameObject.activeSelf)
		{
			return;
		}
		SingletonComponent<LoadingScreen>.Instance.panel.gameObject.SetActive(false);
		SingletonComponent<LoadingScreen>.Instance.gameObject.SetActive(false);
		SingletonComponent<LoadingScreen>.Instance.gameObject.SetActive(true);
		if (LevelManager.isLoaded && SingletonComponent<MusicManager>.Instance != null)
		{
			SingletonComponent<MusicManager>.Instance.StopMusic();
		}
	}

	public static void HideSkip()
	{
		LoadingScreen.WantsSkip = false;
		if (!SingletonComponent<LoadingScreen>.Instance)
		{
			return;
		}
		if (!SingletonComponent<LoadingScreen>.Instance.skipButton)
		{
			return;
		}
		SingletonComponent<LoadingScreen>.Instance.skipButton.gameObject.SetActive(false);
	}

	public static void Show()
	{
		if (!SingletonComponent<LoadingScreen>.Instance)
		{
			Debug.LogWarning("Wanted to show loading screen but not ready");
			return;
		}
		if (SingletonComponent<LoadingScreen>.Instance.panel.gameObject.activeSelf)
		{
			return;
		}
		SingletonComponent<LoadingScreen>.Instance.panel.gameObject.SetActive(true);
		SingletonComponent<LoadingScreen>.Instance.gameObject.SetActive(false);
		SingletonComponent<LoadingScreen>.Instance.gameObject.SetActive(true);
		MusicManager.RaiseIntensityTo(0.5f, 999);
	}

	public static void ShowSkip()
	{
		LoadingScreen.WantsSkip = false;
		if (!SingletonComponent<LoadingScreen>.Instance)
		{
			return;
		}
		if (!SingletonComponent<LoadingScreen>.Instance.skipButton)
		{
			return;
		}
		SingletonComponent<LoadingScreen>.Instance.skipButton.gameObject.SetActive(true);
	}

	public void SkipLoading()
	{
		LoadingScreen.WantsSkip = true;
	}

	public static void Update(string strType)
	{
		if (LoadingScreen.Text == strType)
		{
			return;
		}
		LoadingScreen.Text = strType;
		if (!SingletonComponent<LoadingScreen>.Instance)
		{
			return;
		}
		SingletonComponent<LoadingScreen>.Instance.subtitle.text = strType.ToUpper();
		GameObject gameObject = GameObject.Find("MenuMusic");
		if (gameObject)
		{
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if (component)
			{
				component.Pause();
			}
		}
	}

	public void UpdateFromServer(string strTitle, string strSubtitle)
	{
		this.title.text = strTitle;
		this.subtitle.text = strSubtitle;
	}
}