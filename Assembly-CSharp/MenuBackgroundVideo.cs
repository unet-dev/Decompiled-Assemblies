using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Video;

public class MenuBackgroundVideo : SingletonComponent<MenuBackgroundVideo>
{
	private string[] videos;

	private int index;

	private bool errored;

	public MenuBackgroundVideo()
	{
	}

	protected override void Awake()
	{
		base.Awake();
		this.LoadVideoList();
		this.NextVideo();
		base.GetComponent<VideoPlayer>().errorReceived += new VideoPlayer.ErrorEventHandler(this.OnVideoError);
	}

	public void LoadVideoList()
	{
		this.videos = Directory.EnumerateFiles(string.Concat(Application.streamingAssetsPath, "/MenuVideo/")).Where<string>((string x) => {
			if (x.EndsWith(".mp4"))
			{
				return true;
			}
			return x.EndsWith(".webm");
		}).OrderBy<string, Guid>((string x) => Guid.NewGuid()).ToArray<string>();
	}

	private void NextVideo()
	{
		string[] strArrays = this.videos;
		int num = this.index;
		this.index = num + 1;
		string str = strArrays[num % (int)this.videos.Length];
		this.errored = false;
		UnityEngine.Debug.Log(string.Concat("Playing Video ", str));
		VideoPlayer component = base.GetComponent<VideoPlayer>();
		component.url = str;
		component.Play();
	}

	private void OnVideoError(VideoPlayer source, string message)
	{
		this.errored = true;
	}

	internal IEnumerator ReadyVideo()
	{
		MenuBackgroundVideo menuBackgroundVideo = null;
		if (menuBackgroundVideo.errored)
		{
			yield break;
		}
		VideoPlayer component = menuBackgroundVideo.GetComponent<VideoPlayer>();
		while (!component.isPrepared)
		{
			if (menuBackgroundVideo.errored)
			{
				yield break;
			}
			yield return null;
		}
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			this.LoadVideoList();
		}
		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			this.NextVideo();
		}
	}
}