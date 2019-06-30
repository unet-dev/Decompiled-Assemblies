using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Windows
{
	public class ConsoleInput
	{
		public string inputString = "";

		public string[] statusText = new string[] { "", "", "" };

		internal float nextUpdate;

		public int lineWidth
		{
			get
			{
				return Console.BufferWidth;
			}
		}

		public bool valid
		{
			get
			{
				return Console.BufferWidth > 0;
			}
		}

		public ConsoleInput()
		{
		}

		public void ClearLine(int numLines)
		{
			Console.CursorLeft = 0;
			Console.Write(new string(' ', this.lineWidth * numLines));
			Console.CursorTop = Console.CursorTop - numLines;
			Console.CursorLeft = 0;
		}

		internal void OnBackspace()
		{
			if (this.inputString.Length < 1)
			{
				return;
			}
			this.inputString = this.inputString.Substring(0, this.inputString.Length - 1);
			this.RedrawInputLine();
		}

		internal void OnEnter()
		{
			this.ClearLine((int)this.statusText.Length);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(string.Concat("> ", this.inputString));
			string str = this.inputString;
			this.inputString = "";
			if (this.OnInputText != null)
			{
				this.OnInputText(str);
			}
			this.RedrawInputLine();
		}

		internal void OnEscape()
		{
			this.inputString = "";
			this.RedrawInputLine();
		}

		public void RedrawInputLine()
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			ConsoleColor foregroundColor = Console.ForegroundColor;
			try
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.CursorTop = Console.CursorTop + 1;
				for (int i = 0; i < (int)this.statusText.Length; i++)
				{
					Console.CursorLeft = 0;
					Console.Write(this.statusText[i].PadRight(this.lineWidth));
				}
				Console.CursorTop = Console.CursorTop - ((int)this.statusText.Length + 1);
				Console.CursorLeft = 0;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Green;
				this.ClearLine(1);
				if (this.inputString.Length == 0)
				{
					Console.BackgroundColor = backgroundColor;
					Console.ForegroundColor = foregroundColor;
					return;
				}
				else if (this.inputString.Length >= this.lineWidth - 2)
				{
					Console.Write(this.inputString.Substring(this.inputString.Length - (this.lineWidth - 2)));
				}
				else
				{
					Console.Write(this.inputString);
				}
			}
			catch (Exception exception)
			{
			}
			Console.BackgroundColor = backgroundColor;
			Console.ForegroundColor = foregroundColor;
		}

		public void Update()
		{
			if (!this.valid)
			{
				return;
			}
			if (this.nextUpdate < Time.realtimeSinceStartup)
			{
				this.RedrawInputLine();
				this.nextUpdate = Time.realtimeSinceStartup + 0.5f;
			}
			try
			{
				if (!Console.KeyAvailable)
				{
					return;
				}
			}
			catch (Exception exception)
			{
				return;
			}
			ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
			if (consoleKeyInfo.Key == ConsoleKey.Enter)
			{
				this.OnEnter();
				return;
			}
			if (consoleKeyInfo.Key == ConsoleKey.Backspace)
			{
				this.OnBackspace();
				return;
			}
			if (consoleKeyInfo.Key == ConsoleKey.Escape)
			{
				this.OnEscape();
				return;
			}
			if (consoleKeyInfo.KeyChar != 0)
			{
				string str = this.inputString;
				char keyChar = consoleKeyInfo.KeyChar;
				this.inputString = string.Concat(str, keyChar.ToString());
				this.RedrawInputLine();
				return;
			}
		}

		public event Action<string> OnInputText;
	}
}