using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Buttons
{
	public Buttons()
	{
	}

	public class ConButton : ConsoleSystem.IConsoleCommand
	{
		private int frame;

		public bool IsDown
		{
			get;
			set;
		}

		public bool JustPressed
		{
			get
			{
				if (!this.IsDown)
				{
					return false;
				}
				return this.frame == Time.frameCount;
			}
		}

		public bool JustReleased
		{
			get
			{
				if (this.IsDown)
				{
					return false;
				}
				return this.frame == Time.frameCount;
			}
		}

		public ConButton()
		{
		}

		public void Call(ConsoleSystem.Arg arg)
		{
			this.IsDown = arg.GetBool(0, false);
			this.frame = Time.frameCount;
		}
	}
}