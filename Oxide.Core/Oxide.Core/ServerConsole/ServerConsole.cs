using Oxide.Core;
using Oxide.Core.Configuration;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Oxide.Core.ServerConsole
{
	public class ServerConsole
	{
		private readonly ConsoleWindow console = new ConsoleWindow();

		private readonly ConsoleInput input = new ConsoleInput();

		private bool init;

		private float nextUpdate;

		private float nextTitleUpdate;

		public Func<string> Title;

		public Func<string> Status1Left;

		public Func<string> Status1Right;

		public Func<string> Status2Left;

		public Func<string> Status2Right;

		public Func<string> Status3Left;

		public Func<string> Status3Right;

		public Func<string, string[]> Completion
		{
			get
			{
				return this.input.Completion;
			}
			set
			{
				this.input.Completion = value;
			}
		}

		private string status1Left
		{
			get
			{
				return Oxide.Core.ServerConsole.ServerConsole.GetStatusValue(this.Status1Left);
			}
		}

		public ConsoleColor Status1LeftColor
		{
			get
			{
				return this.input.StatusTextLeftColor[1];
			}
			set
			{
				this.input.StatusTextLeftColor[1] = value;
			}
		}

		private string status1Right
		{
			get
			{
				return Oxide.Core.ServerConsole.ServerConsole.GetStatusValue(this.Status1Right).PadLeft(this.input.LineWidth - 1);
			}
		}

		public ConsoleColor Status1RightColor
		{
			get
			{
				return this.input.StatusTextRightColor[1];
			}
			set
			{
				this.input.StatusTextRightColor[1] = value;
			}
		}

		private string status2Left
		{
			get
			{
				return Oxide.Core.ServerConsole.ServerConsole.GetStatusValue(this.Status2Left);
			}
		}

		public ConsoleColor Status2LeftColor
		{
			get
			{
				return this.input.StatusTextLeftColor[2];
			}
			set
			{
				this.input.StatusTextLeftColor[2] = value;
			}
		}

		private string status2Right
		{
			get
			{
				return Oxide.Core.ServerConsole.ServerConsole.GetStatusValue(this.Status2Right).PadLeft(this.input.LineWidth - 1);
			}
		}

		public ConsoleColor Status2RightColor
		{
			get
			{
				return this.input.StatusTextRightColor[2];
			}
			set
			{
				this.input.StatusTextRightColor[2] = value;
			}
		}

		private string status3Left
		{
			get
			{
				return Oxide.Core.ServerConsole.ServerConsole.GetStatusValue(this.Status3Left);
			}
		}

		public ConsoleColor Status3LeftColor
		{
			get
			{
				return this.input.StatusTextLeftColor[3];
			}
			set
			{
				this.input.StatusTextLeftColor[3] = value;
			}
		}

		private string status3Right
		{
			get
			{
				return Oxide.Core.ServerConsole.ServerConsole.GetStatusValue(this.Status3Right).PadLeft(this.input.LineWidth - 1);
			}
		}

		public ConsoleColor Status3RightColor
		{
			get
			{
				return this.input.StatusTextRightColor[3];
			}
			set
			{
				this.input.StatusTextRightColor[3] = value;
			}
		}

		private string title
		{
			get
			{
				Func<string> title = this.Title;
				if (title != null)
				{
					return title();
				}
				return null;
			}
		}

		public ServerConsole()
		{
		}

		public void AddMessage(string message, ConsoleColor color = 7)
		{
			Console.ForegroundColor = color;
			int num = message.Split(new char[] { '\n' }).Aggregate<string, int>(0, (int sum, string line) => sum + (int)Math.Ceiling((double)line.Length / (double)Console.BufferWidth));
			this.input.ClearLine((Interface.Oxide.Config.Console.ShowStatusBar ? (int)this.input.StatusTextLeft.Length : 0) + num);
			Console.WriteLine(message);
			this.input.RedrawInputLine();
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		private static string GetStatusRight(int leftLength, string right)
		{
			if (leftLength >= right.Length)
			{
				return string.Empty;
			}
			return right.Substring(leftLength);
		}

		private static string GetStatusValue(Func<string> status)
		{
			if (status == null)
			{
				return "";
			}
			return status() ?? string.Empty;
		}

		public void OnDisable()
		{
			if (!this.init)
			{
				return;
			}
			this.input.OnInputText -= new Action<string>(this.OnInputText);
			this.console.Shutdown();
		}

		public void OnEnable()
		{
			if (!this.console.Initialize())
			{
				return;
			}
			this.init = true;
			this.input.OnInputText += new Action<string>(this.OnInputText);
			this.input.ClearLine(1);
			this.input.ClearLine(Console.WindowHeight);
			for (int i = 0; i < Console.WindowHeight; i++)
			{
				Console.WriteLine();
			}
		}

		private void OnInputText(string obj)
		{
			try
			{
				Action<string> action = this.Input;
				if (action != null)
				{
					action(obj);
				}
				else
				{
				}
			}
			catch (Exception exception)
			{
				Interface.Oxide.LogException("OnInputText: ", exception);
			}
		}

		public static void PrintColored(params object[] objects)
		{
			if (Interface.Oxide.ServerConsole == null)
			{
				return;
			}
			Interface.Oxide.ServerConsole.input.ClearLine((Interface.Oxide.Config.Console.ShowStatusBar ? (int)Interface.Oxide.ServerConsole.input.StatusTextLeft.Length : 1));
			for (int i = 0; i < (int)objects.Length; i++)
			{
				if (i % 2 == 0)
				{
					Console.ForegroundColor = (ConsoleColor)((int)objects[i]);
				}
				else
				{
					Console.Write((string)objects[i]);
				}
			}
			if (Console.CursorLeft != 0)
			{
				Console.CursorTop = Console.CursorTop + 1;
			}
			Interface.Oxide.ServerConsole.input.RedrawInputLine();
		}

		public void Update()
		{
			if (!this.init)
			{
				return;
			}
			if (Interface.Oxide.Config.Console.ShowStatusBar)
			{
				this.UpdateStatus();
			}
			this.input.Update();
			if (this.nextTitleUpdate > Interface.Oxide.Now)
			{
				return;
			}
			this.nextTitleUpdate = Interface.Oxide.Now + 1f;
			this.console.SetTitle(this.title);
		}

		private void UpdateStatus()
		{
			if (this.nextUpdate > Interface.Oxide.Now)
			{
				return;
			}
			this.nextUpdate = Interface.Oxide.Now + 0.66f;
			if (!this.input.Valid)
			{
				return;
			}
			string str = this.status1Left;
			string str1 = this.status2Left;
			string str2 = this.status3Left;
			this.input.StatusTextLeft[1] = str;
			this.input.StatusTextLeft[2] = str1;
			this.input.StatusTextLeft[3] = str2;
			this.input.StatusTextRight[1] = Oxide.Core.ServerConsole.ServerConsole.GetStatusRight(str.Length, this.status1Right);
			this.input.StatusTextRight[2] = Oxide.Core.ServerConsole.ServerConsole.GetStatusRight(str1.Length, this.status2Right);
			this.input.StatusTextRight[3] = Oxide.Core.ServerConsole.ServerConsole.GetStatusRight(str2.Length, this.status3Right);
		}

		public event Action<string> Input;
	}
}