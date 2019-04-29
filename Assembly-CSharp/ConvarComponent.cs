using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConvarComponent : MonoBehaviour
{
	public bool runOnServer = true;

	public bool runOnClient = true;

	public List<ConvarComponent.ConvarEvent> List = new List<ConvarComponent.ConvarEvent>();

	public ConvarComponent()
	{
	}

	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (!this.ShouldRun())
		{
			return;
		}
		foreach (ConvarComponent.ConvarEvent list in this.List)
		{
			list.OnDisable();
		}
	}

	protected void OnEnable()
	{
		if (!this.ShouldRun())
		{
			return;
		}
		foreach (ConvarComponent.ConvarEvent list in this.List)
		{
			list.OnEnable();
		}
	}

	private bool ShouldRun()
	{
		if (!this.runOnServer)
		{
			return false;
		}
		return true;
	}

	[Serializable]
	public class ConvarEvent
	{
		public string convar;

		public string on;

		public MonoBehaviour component;

		internal ConsoleSystem.Command cmd;

		public ConvarEvent()
		{
		}

		private void cmd_OnValueChanged(ConsoleSystem.Command obj)
		{
			if (this.component == null)
			{
				return;
			}
			bool str = obj.String == this.on;
			if (this.component.enabled == str)
			{
				return;
			}
			this.component.enabled = str;
		}

		public void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			if (this.cmd == null)
			{
				return;
			}
			this.cmd.OnValueChanged -= new Action<ConsoleSystem.Command>(this.cmd_OnValueChanged);
		}

		public void OnEnable()
		{
			this.cmd = ConsoleSystem.Index.Client.Find(this.convar);
			if (this.cmd == null)
			{
				this.cmd = ConsoleSystem.Index.Server.Find(this.convar);
			}
			if (this.cmd == null)
			{
				return;
			}
			this.cmd.OnValueChanged += new Action<ConsoleSystem.Command>(this.cmd_OnValueChanged);
			this.cmd_OnValueChanged(this.cmd);
		}
	}
}