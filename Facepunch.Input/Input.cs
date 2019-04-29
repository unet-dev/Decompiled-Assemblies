using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Facepunch
{
	public static class Input
	{
		private static List<Input.Button> buttons;

		private static List<Action> frameThinks;

		static Input()
		{
			Input.buttons = new List<Input.Button>();
			Input.frameThinks = new List<Action>();
		}

		public static void AddButton(string name, Func<bool> TestFunction, Action FrameThink = null)
		{
			if (Input.buttons.Any<Input.Button>((Input.Button x) => x.Name == name.ToLower()))
			{
				return;
			}
			Input.Button button = new Input.Button()
			{
				Name = name.ToLower(),
				TestFunction = TestFunction
			};
			Input.buttons.Add(button);
			if (FrameThink != null)
			{
				Input.frameThinks.Add(FrameThink);
			}
		}

		public static void ClearBinds()
		{
			foreach (Input.Button button in Input.buttons)
			{
				button.Binds = null;
			}
		}

		public static void Frame()
		{
			for (int i = 0; i < Input.frameThinks.Count; i++)
			{
				Input.frameThinks[i]();
			}
		}

		public static Dictionary<string, string[]> GetAllBinds()
		{
			return Input.buttons.Where<Input.Button>((Input.Button x) => {
				if (x.Binds == null)
				{
					return false;
				}
				return x.Binds.Length != 0;
			}).ToDictionary<Input.Button, string, string[]>((Input.Button x) => x.Name, (Input.Button x) => x.Binds);
		}

		public static string[] GetAllButtons()
		{
			return (
				from x in Input.buttons
				select x.Name).ToArray<string>();
		}

		public static string GetBind(string name)
		{
			string str;
			Func<Input.Button, bool> func = null;
			List<Input.Button> buttons = Input.buttons;
			Func<Input.Button, bool> func1 = func;
			if (func1 == null)
			{
				Func<Input.Button, bool> func2 = (Input.Button x) => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase);
				Func<Input.Button, bool> func3 = func2;
				func = func2;
				func1 = func3;
			}
			using (IEnumerator<Input.Button> enumerator = buttons.Where<Input.Button>(func1).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Input.Button current = enumerator.Current;
					str = (current.Binds != null ? string.Join(";", current.Binds) : "(nothing)");
				}
				else
				{
					return "(button not found)";
				}
			}
			return str;
		}

		public static string[] GetButtonsWithBind(string bind)
		{
			return Input.buttons.Where<Input.Button>((Input.Button x) => {
				if (x.Binds == null)
				{
					return false;
				}
				return x.Binds.Contains<string>(bind);
			}).Select<Input.Button, string>((Input.Button x) => x.Name).ToArray<string>();
		}

		public static string GetButtonWithBind(string bind)
		{
			string[] buttonsWithBind = Input.GetButtonsWithBind(bind);
			if (buttonsWithBind.Length == 0)
			{
				return "UNSET";
			}
			return buttonsWithBind[0];
		}

		public static string[] GetPressedButtons()
		{
			return (
				from x in Input.buttons
				where x.CurrentValue
				select x.Name).ToArray<string>();
		}

		public static void SetBind(string name, string bind)
		{
			Func<Input.Button, bool> func = null;
			List<Input.Button> buttons = Input.buttons;
			Func<Input.Button, bool> func1 = func;
			if (func1 == null)
			{
				Func<Input.Button, bool> func2 = (Input.Button x) => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase);
				Func<Input.Button, bool> func3 = func2;
				func = func2;
				func1 = func3;
			}
			foreach (Input.Button button in buttons.Where<Input.Button>(func1))
			{
				if (bind != null)
				{
					button.Binds = bind.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				}
				else
				{
					button.Binds = null;
				}
			}
		}

		public static void Update()
		{
			for (int i = 0; i < Input.buttons.Count; i++)
			{
				Input.buttons[i].Update();
			}
		}

		public static event Action<string, bool> RunBind;

		private class Button
		{
			public string Name;

			public Func<bool> TestFunction;

			public bool CurrentValue;

			public bool LastValue;

			public string[] Binds;

			public Button()
			{
			}

			private void RunBinds(bool Pressed)
			{
				if (this.Binds == null)
				{
					return;
				}
				if (Input.RunBind == null)
				{
					return;
				}
				for (int i = 0; i < (int)this.Binds.Length; i++)
				{
					if (this.Binds[i][0] == '+')
					{
						Input.RunBind(this.Binds[i].Substring(1), Pressed);
					}
					else if (Pressed)
					{
						Input.RunBind(this.Binds[i], Pressed);
					}
				}
			}

			public virtual void Update()
			{
				this.LastValue = this.CurrentValue;
				this.CurrentValue = this.TestFunction();
				if (this.CurrentValue && !this.LastValue)
				{
					this.RunBinds(true);
				}
				if (!this.CurrentValue && this.LastValue)
				{
					this.RunBinds(false);
				}
			}
		}
	}
}