using Rust;
using System;

public class PlayerInput : EntityComponent<BasePlayer>
{
	public InputState state = new InputState();

	[NonSerialized]
	public bool hadInputBuffer = true;

	public PlayerInput()
	{
	}

	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.state.Clear();
	}
}