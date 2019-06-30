using ProtoBuf;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SleepingBagButton : MonoBehaviour
{
	public GameObject timerInfo;

	public TextMeshProUGUI BagName;

	public TextMeshProUGUI LockTime;

	internal Button button;

	internal RespawnInformation.SpawnOptions spawnOptions;

	internal float releaseTime;

	public string friendlyName
	{
		get
		{
			if (this.spawnOptions == null || string.IsNullOrEmpty(this.spawnOptions.name))
			{
				return "Null Sleeping Bag";
			}
			return this.spawnOptions.name;
		}
	}

	public float timerSeconds
	{
		get
		{
			return Mathf.Clamp(this.releaseTime - Time.realtimeSinceStartup, 0f, 216000f);
		}
	}

	public SleepingBagButton()
	{
	}

	public void DeleteBag()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "respawn_sleepingbag_remove", new object[] { this.spawnOptions.id });
	}

	public void DoSpawn()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "respawn_sleepingbag", new object[] { this.spawnOptions.id });
	}

	public void Setup(RespawnInformation.SpawnOptions options)
	{
		this.button = base.GetComponent<Button>();
		this.spawnOptions = options;
		if (options.unlockSeconds <= 0f)
		{
			this.button.interactable = true;
			this.timerInfo.SetActive(false);
			this.releaseTime = 0f;
		}
		else
		{
			this.button.interactable = false;
			this.timerInfo.SetActive(true);
			this.releaseTime = Time.realtimeSinceStartup + options.unlockSeconds;
		}
		this.BagName.text = this.friendlyName;
	}

	public void Update()
	{
		if (this.releaseTime == 0f)
		{
			return;
		}
		if (this.releaseTime >= Time.realtimeSinceStartup)
		{
			this.LockTime.text = this.timerSeconds.ToString("N0");
			return;
		}
		this.releaseTime = 0f;
		this.timerInfo.SetActive(false);
		this.button.interactable = true;
	}
}