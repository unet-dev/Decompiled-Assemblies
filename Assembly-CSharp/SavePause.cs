using Rust;
using System;
using UnityEngine;

public class SavePause : MonoBehaviour, IServerComponent
{
	private bool tracked;

	public SavePause()
	{
	}

	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (!SingletonComponent<SaveRestore>.Instance || !this.tracked)
		{
			return;
		}
		this.tracked = false;
		SingletonComponent<SaveRestore>.Instance.timedSavePause--;
	}

	protected void OnEnable()
	{
		if (!SingletonComponent<SaveRestore>.Instance || this.tracked)
		{
			return;
		}
		this.tracked = true;
		SingletonComponent<SaveRestore>.Instance.timedSavePause++;
	}
}