using Oxide.Core;
using Oxide.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Oxide.Core.ServerConsole
{
	public class ConsoleInput
	{
		private string inputString = string.Empty;

		private readonly List<string> inputHistory = new List<string>();

		private int inputHistoryIndex;

		private float nextUpdate;

		internal readonly string[] StatusTextLeft = new string[] { string.Empty, string.Empty, string.Empty, string.Empty };

		internal readonly string[] StatusTextRight = new string[] { string.Empty, string.Empty, string.Empty, string.Empty };

		internal readonly ConsoleColor[] StatusTextLeftColor = new ConsoleColor[] { typeof(<PrivateImplementationDetails>).GetField("0F8F44850955F18920CC2979A5C1127AA14D0CB6").FieldHandle };

		internal readonly ConsoleColor[] StatusTextRightColor = new ConsoleColor[] { typeof(<PrivateImplementationDetails>).GetField("0F8F44850955F18920CC2979A5C1127AA14D0CB6").FieldHandle };

		public Func<string, string[]> Completion;

		public int LineWidth
		{
			get
			{
				return Console.BufferWidth;
			}
		}

		public bool Valid
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
			Console.Write(new string(' ', this.LineWidth * numLines));
			Console.CursorTop = Console.CursorTop - numLines;
			Console.CursorLeft = 0;
		}

		private static int GetFirstDiffIndex(string str1, string str2)
		{
			if (str1 == null || str2 == null)
			{
				return -1;
			}
			int num = Math.Min(str1.Length, str2.Length);
			for (int i = 0; i < num; i++)
			{
				if (str1[i] != str2[i])
				{
					return i;
				}
			}
			return num;
		}

		public void RedrawInputLine()
		{
			if (this.nextUpdate - 0.45f > Interface.Oxide.Now || this.LineWidth <= 0)
			{
				return;
			}
			try
			{
				Console.CursorTop = Console.CursorTop + 1;
				for (int i = 0; i < (int)this.StatusTextLeft.Length && Interface.Oxide.Config.Console.ShowStatusBar; i++)
				{
					Console.CursorLeft = 0;
					Console.ForegroundColor = this.StatusTextLeftColor[i];
					Console.Write(this.StatusTextLeft[i].Substring(0, Math.Min(this.StatusTextLeft[i].Length, this.LineWidth - 1)));
					Console.ForegroundColor = this.StatusTextRightColor[i];
					Console.Write(this.StatusTextRight[i].PadRight(this.LineWidth));
				}
				Console.CursorTop = Console.CursorTop - (Interface.Oxide.Config.Console.ShowStatusBar ? (int)this.StatusTextLeft.Length + 1 : 1);
				Console.CursorLeft = 0;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Green;
				this.ClearLine(1);
				if (this.inputString.Length != 0)
				{
					Console.Write((this.inputString.Length >= this.LineWidth - 2 ? this.inputString.Substring(this.inputString.Length - (this.LineWidth - 2)) : this.inputString));
					Console.ForegroundColor = ConsoleColor.Gray;
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Gray;
				}
			}
			catch (Exception exception)
			{
				Interface.Oxide.LogException("RedrawInputLine: ", exception);
			}
		}

		public void Update()
		{
			int num;
			string empty;
			string[] strArrays;
			if (!this.Valid)
			{
				return;
			}
			if (this.nextUpdate < Interface.Oxide.Now)
			{
				this.RedrawInputLine();
				this.nextUpdate = Interface.Oxide.Now + 0.5f;
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
			if (consoleKeyInfo.Key != ConsoleKey.DownArrow && consoleKeyInfo.Key != ConsoleKey.UpArrow)
			{
				this.inputHistoryIndex = 0;
			}
			ConsoleKey key = consoleKeyInfo.Key;
			if (key > ConsoleKey.Enter)
			{
				if (key == ConsoleKey.Escape)
				{
					this.inputString = string.Empty;
					this.RedrawInputLine();
					return;
				}
				if (key == ConsoleKey.UpArrow)
				{
					if (this.inputHistory.Count == 0)
					{
						return;
					}
					if (this.inputHistoryIndex < 0)
					{
						this.inputHistoryIndex = 0;
					}
					if (this.inputHistoryIndex >= this.inputHistory.Count - 1)
					{
						this.inputHistoryIndex = this.inputHistory.Count - 1;
						this.inputString = this.inputHistory[this.inputHistoryIndex];
						this.RedrawInputLine();
						return;
					}
					List<string> strs = this.inputHistory;
					num = this.inputHistoryIndex;
					this.inputHistoryIndex = num + 1;
					this.inputString = strs[num];
					this.RedrawInputLine();
					return;
				}
				if (key == ConsoleKey.DownArrow)
				{
					if (this.inputHistory.Count == 0)
					{
						return;
					}
					if (this.inputHistoryIndex >= this.inputHistory.Count - 1)
					{
						this.inputHistoryIndex = this.inputHistory.Count - 2;
					}
					if (this.inputHistoryIndex < 0)
					{
						empty = string.Empty;
					}
					else
					{
						List<string> strs1 = this.inputHistory;
						num = this.inputHistoryIndex;
						this.inputHistoryIndex = num - 1;
						empty = strs1[num];
					}
					this.inputString = empty;
					this.RedrawInputLine();
					return;
				}
			}
			else
			{
				if (key == ConsoleKey.Backspace)
				{
					if (this.inputString.Length < 1)
					{
						return;
					}
					this.inputString = this.inputString.Substring(0, this.inputString.Length - 1);
					this.RedrawInputLine();
					return;
				}
				if (key == ConsoleKey.Tab)
				{
					Func<string, string[]> completion = this.Completion;
					if (completion != null)
					{
						strArrays = completion(this.inputString);
					}
					else
					{
						strArrays = null;
					}
					string[] strArrays1 = strArrays;
					if (strArrays1 == null || strArrays1.Length == 0)
					{
						return;
					}
					if ((int)strArrays1.Length <= 1)
					{
						this.inputString = strArrays1[0];
						this.RedrawInputLine();
						return;
					}
					this.ClearLine((Interface.Oxide.Config.Console.ShowStatusBar ? (int)this.StatusTextLeft.Length + 1 : 1));
					Console.ForegroundColor = ConsoleColor.Yellow;
					int num1 = strArrays1.Max<string>((string r) => r.Length);
					for (int i = 0; i < (int)strArrays1.Length; i++)
					{
						string str = strArrays1[i];
						if (i > 0)
						{
							int firstDiffIndex = ConsoleInput.GetFirstDiffIndex(strArrays1[0], str);
							if (firstDiffIndex > 0 && firstDiffIndex < num1)
							{
								num1 = firstDiffIndex;
							}
						}
						Console.WriteLine(str);
					}
					if (num1 > 0)
					{
						this.inputString = strArrays1[0].Substring(0, num1);
					}
					this.RedrawInputLine();
					return;
				}
				if (key == ConsoleKey.Enter)
				{
					this.ClearLine((Interface.Oxide.Config.Console.ShowStatusBar ? (int)this.StatusTextLeft.Length : 1));
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine(string.Concat("> ", this.inputString));
					this.inputHistory.Insert(0, this.inputString);
					if (this.inputHistory.Count > 50)
					{
						this.inputHistory.RemoveRange(50, this.inputHistory.Count - 50);
					}
					string str1 = this.inputString;
					this.inputString = string.Empty;
					Action<string> action = this.OnInputText;
					if (action != null)
					{
						action(str1);
					}
					else
					{
					}
					this.RedrawInputLine();
					return;
				}
			}
			if (consoleKeyInfo.KeyChar == 0)
			{
				return;
			}
			this.inputString = string.Concat(this.inputString, consoleKeyInfo.KeyChar);
			this.RedrawInputLine();
		}

		internal event Action<string> OnInputText;
	}
}