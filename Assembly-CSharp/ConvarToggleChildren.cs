using System;
using System.Collections;
using UnityEngine;

public class ConvarToggleChildren : MonoBehaviour
{
	public string ConvarName;

	public string ConvarEnabled = "True";

	private bool state;

	private ConsoleSystem.Command Command;

	public ConvarToggleChildren()
	{
	}

	protected void Awake()
	{
		this.Command = ConsoleSystem.Index.Client.Find(this.ConvarName);
		if (this.Command == null)
		{
			this.Command = ConsoleSystem.Index.Server.Find(this.ConvarName);
		}
		if (this.Command != null)
		{
			this.SetState(this.Command.String == this.ConvarEnabled);
		}
	}

	private void SetState(bool newState)
	{
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(newState);
		}
		this.state = newState;
	}

	protected void Update()
	{
		if (this.Command != null)
		{
			bool str = this.Command.String == this.ConvarEnabled;
			if (this.state != str)
			{
				this.SetState(str);
			}
		}
	}
}