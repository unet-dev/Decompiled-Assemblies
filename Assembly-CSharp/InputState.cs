using System;
using UnityEngine;

public class InputState
{
	public InputMessage current = new InputMessage()
	{
		ShouldPool = false
	};

	public InputMessage previous = new InputMessage()
	{
		ShouldPool = false
	};

	private int SwallowedButtons;

	public InputState()
	{
	}

	private Quaternion AimAngle()
	{
		if (this.current == null)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler(this.current.aimAngles);
	}

	public void Clear()
	{
		this.current.buttons = 0;
		this.previous.buttons = 0;
		this.SwallowedButtons = 0;
	}

	public void Flip(InputMessage newcurrent)
	{
		this.SwallowedButtons = 0;
		this.previous.aimAngles = this.current.aimAngles;
		this.previous.buttons = this.current.buttons;
		this.previous.mouseDelta = this.current.mouseDelta;
		this.current.aimAngles = newcurrent.aimAngles;
		this.current.buttons = newcurrent.buttons;
		this.current.mouseDelta = newcurrent.mouseDelta;
	}

	public bool IsDown(BUTTON btn)
	{
		if (this.current == null)
		{
			return false;
		}
		if ((this.SwallowedButtons & (int)btn) == (int)btn)
		{
			return false;
		}
		return (this.current.buttons & (int)btn) == (int)btn;
	}

	public void SwallowButton(BUTTON btn)
	{
		if (this.current == null)
		{
			return;
		}
		this.SwallowedButtons |= (int)btn;
	}

	public bool WasDown(BUTTON btn)
	{
		if (this.previous == null)
		{
			return false;
		}
		return (this.previous.buttons & (int)btn) == (int)btn;
	}

	public bool WasJustPressed(BUTTON btn)
	{
		if (!this.IsDown(btn))
		{
			return false;
		}
		return !this.WasDown(btn);
	}

	public bool WasJustReleased(BUTTON btn)
	{
		if (this.IsDown(btn))
		{
			return false;
		}
		return this.WasDown(btn);
	}
}