using Facepunch;
using Facepunch.Extend;
using Network;
using Newtonsoft.Json;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class ConsoleSystem
{
	public static bool HasChanges;

	public static Func<bool> ClientCanRunAdminCommands;

	public static Func<string, bool> OnSendToServer;

	public static string LastError;

	public static ConsoleSystem.Arg CurrentArgs;

	static ConsoleSystem()
	{
	}

	public ConsoleSystem()
	{
	}

	public static string BuildCommand(string strCommand, params object[] args)
	{
		if (args == null || args.Length == 0)
		{
			return strCommand;
		}
		object[] objArray = args;
		for (int i = 0; i < (int)objArray.Length; i++)
		{
			object obj = objArray[i];
			if (obj == null)
			{
				strCommand = string.Concat(strCommand, " \"\"");
			}
			else if (obj is Color)
			{
				Color color = (Color)obj;
				strCommand = string.Concat(strCommand, " ", string.Format("{0},{1},{2},{3}", new object[] { color.r, color.g, color.b, color.a }).QuoteSafe());
			}
			else if (!(obj is Vector3))
			{
				strCommand = string.Concat(strCommand, " ", obj.ToString().QuoteSafe());
			}
			else
			{
				Vector3 vector3 = (Vector3)obj;
				strCommand = string.Concat(strCommand, " ", string.Format("{0},{1},{2}", vector3.x, vector3.y, vector3.z).QuoteSafe());
			}
		}
		return strCommand;
	}

	private static bool Internal(ConsoleSystem.Arg arg)
	{
		bool flag;
		if (arg.Invalid)
		{
			return false;
		}
		object obj = Interface.CallHook("IOnServerCommand", arg);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (!arg.HasPermission())
		{
			arg.ReplyWith("You cannot run this command");
			return false;
		}
		try
		{
			using (TimeWarning timeWarning = TimeWarning.New(string.Concat("ConsoleSystem: ", arg.cmd.FullName), 0.1f))
			{
				arg.cmd.Call(arg);
			}
			if (arg.cmd.Variable && arg.cmd.GetOveride != null)
			{
				string str = arg.cmd.String;
				string str1 = (arg.cmd.Variable ? arg.cmd.String : "");
				if (str1 == str)
				{
					arg.ReplyWith(string.Format("{0}: {1}", arg.cmd.FullName, str.QuoteSafe()));
				}
				else
				{
					arg.ReplyWith(string.Format("{0}: changed from {1} to {2}", arg.cmd.FullName, str1.QuoteSafe(), str.QuoteSafe()));
				}
			}
			return true;
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			arg.ReplyWith(string.Concat(new string[] { "Error: ", arg.cmd.FullName, " - ", exception.Message, " (", exception.Source, ")" }));
			Debug.LogException(exception);
			flag = false;
		}
		return flag;
	}

	public static string Run(ConsoleSystem.Option options, string strCommand, params object[] args)
	{
		ConsoleSystem.LastError = null;
		string str = ConsoleSystem.BuildCommand(strCommand, args);
		ConsoleSystem.Arg arg = new ConsoleSystem.Arg(options, str);
		bool flag = arg.HasPermission();
		if (!arg.Invalid & flag)
		{
			ConsoleSystem.Arg currentArgs = ConsoleSystem.CurrentArgs;
			ConsoleSystem.CurrentArgs = arg;
			bool flag1 = ConsoleSystem.Internal(arg);
			ConsoleSystem.CurrentArgs = currentArgs;
			if (options.PrintOutput & flag1 && arg.Reply != null && arg.Reply.Length > 0)
			{
				DebugEx.Log(arg.Reply, StackTraceLogType.None);
			}
			return arg.Reply;
		}
		ConsoleSystem.LastError = "Command not found";
		if (!flag)
		{
			ConsoleSystem.LastError = "Permission denied";
		}
		if (options.IsServer || options.ForwardtoServerOnMissing && ConsoleSystem.SendToServer(str))
		{
			if (options.IsServer && options.PrintOutput)
			{
				ConsoleSystem.LastError = string.Concat("Command '", strCommand, "' not found");
				DebugEx.Log(ConsoleSystem.LastError, StackTraceLogType.None);
			}
			return null;
		}
		ConsoleSystem.LastError = string.Concat("Command '", strCommand, "' not found");
		if (options.PrintOutput)
		{
			DebugEx.Log(ConsoleSystem.LastError, StackTraceLogType.None);
		}
		return null;
	}

	public static void RunFile(ConsoleSystem.Option options, string strFile)
	{
		string[] strArrays = strFile.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < (int)strArrays.Length; i++)
		{
			string str = strArrays[i];
			if (str[0] != '#')
			{
				ConsoleSystem.Run(options, str, Array.Empty<object>());
			}
		}
		ConsoleSystem.HasChanges = false;
	}

	public static string SaveToConfigString(bool bServer)
	{
		IEnumerable<ConsoleSystem.Command> commands;
		string str = "";
		commands = (!bServer ? ((IEnumerable<ConsoleSystem.Command>)ConsoleSystem.Index.All).Where<ConsoleSystem.Command>((ConsoleSystem.Command x) => {
			if (!x.Saved)
			{
				return false;
			}
			return x.Client;
		}) : ((IEnumerable<ConsoleSystem.Command>)ConsoleSystem.Index.All).Where<ConsoleSystem.Command>((ConsoleSystem.Command x) => {
			if (!x.Saved)
			{
				return false;
			}
			return x.ServerAdmin;
		}));
		foreach (ConsoleSystem.Command command in commands)
		{
			if (command.GetOveride == null)
			{
				continue;
			}
			str = string.Concat(str, command.FullName, " ", command.String.QuoteSafe());
			str = string.Concat(str, Environment.NewLine);
		}
		return str;
	}

	internal static bool SendToServer(string command)
	{
		if (ConsoleSystem.OnSendToServer == null)
		{
			return false;
		}
		return ConsoleSystem.OnSendToServer(command);
	}

	public static void UpdateValuesFromCommandLine()
	{
		if (Interface.CallHook("IOnRunCommandLine") != null)
		{
			return;
		}
		foreach (KeyValuePair<string, string> @switch in Facepunch.CommandLine.GetSwitches())
		{
			string value = @switch.Value;
			if (value == "")
			{
				value = "1";
			}
			string str = @switch.Key.Substring(1);
			ConsoleSystem.Run(ConsoleSystem.Option.Unrestricted, str, new object[] { value });
		}
	}

	public class Arg
	{
		public ConsoleSystem.Option Option;

		public ConsoleSystem.Command cmd;

		public string FullString;

		public string[] Args;

		public bool Invalid;

		public string Reply;

		public Connection Connection
		{
			get
			{
				return this.Option.Connection;
			}
		}

		public bool IsAdmin
		{
			get
			{
				if (this.IsConnectionAdmin)
				{
					return true;
				}
				return this.IsRcon;
			}
		}

		public bool IsClientside
		{
			get
			{
				return this.Option.IsClient;
			}
		}

		public bool IsConnectionAdmin
		{
			get
			{
				if (this.Option.Connection == null || !this.Option.Connection.connected)
				{
					return false;
				}
				return this.Option.Connection.authLevel != 0;
			}
		}

		public bool IsRcon
		{
			get
			{
				return this.Option.FromRcon;
			}
		}

		public bool IsServerside
		{
			get
			{
				return this.Option.IsServer;
			}
		}

		internal Arg(ConsoleSystem.Option options, string rconCommand)
		{
			this.Option = options;
			this.BuildCommand(rconCommand);
		}

		internal void BuildCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				this.Invalid = true;
				return;
			}
			if (command.IndexOf('.') <= 0 || command.IndexOf(' ', 0, command.IndexOf('.')) != -1)
			{
				command = string.Concat("global.", command);
			}
			int num = command.IndexOf('.');
			if (num <= 0)
			{
				return;
			}
			string lower = command.Substring(0, num);
			if (lower.Length < 1)
			{
				return;
			}
			lower = lower.Trim().ToLower();
			string str = command.Substring(num + 1);
			if (str.Length < 1)
			{
				return;
			}
			int num1 = str.IndexOf(' ');
			if (num1 > 0)
			{
				this.FullString = str.Substring(num1 + 1);
				this.FullString = this.FullString.Trim();
				this.Args = this.FullString.SplitQuotesStrings();
				str = str.Substring(0, num1);
			}
			str = str.Trim().ToLower();
			if (this.cmd == null && this.Option.IsClient)
			{
				this.cmd = ConsoleSystem.Index.Client.Find(string.Concat(lower, ".", str));
			}
			if (this.cmd == null && this.Option.IsServer)
			{
				this.cmd = ConsoleSystem.Index.Server.Find(string.Concat(lower, ".", str));
			}
			this.Invalid = this.cmd == null;
		}

		internal bool CanSeeInFind(ConsoleSystem.Command command)
		{
			if (command == null)
			{
				return false;
			}
			if (this.Option.IsUnrestricted)
			{
				return true;
			}
			if (this.IsClientside)
			{
				return command.Client;
			}
			if (!this.IsServerside)
			{
				return false;
			}
			return command.Server;
		}

		public bool GetBool(int iArg, bool def = false)
		{
			string str = this.GetString(iArg, null);
			if (str == null)
			{
				return def;
			}
			if (str == string.Empty || str == "0")
			{
				return false;
			}
			if (str.Equals("false", StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			if (str.Equals("no", StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			if (str.Equals("none", StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			if (str.Equals("null", StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			return true;
		}

		public Color GetColor(int iArg, Color def = null)
		{
			string str = this.GetString(iArg, null);
			if (str == null)
			{
				return def;
			}
			return str.ToColor();
		}

		public float GetFloat(int iArg, float def = 0f)
		{
			float single;
			string str = this.GetString(iArg, null);
			if (str == null)
			{
				return def;
			}
			if (float.TryParse(str, out single))
			{
				return single;
			}
			return def;
		}

		public int GetInt(int iArg, int def = 0)
		{
			int num;
			string str = this.GetString(iArg, null);
			if (str == null)
			{
				return def;
			}
			if (int.TryParse(str, out num))
			{
				return num;
			}
			return def;
		}

		public string GetString(int iArg, string def = "")
		{
			if (!this.HasArgs(iArg + 1))
			{
				return def;
			}
			return this.Args[iArg];
		}

		public TimeSpan GetTimeSpan(int iArg)
		{
			TimeSpan timeSpan;
			string str = this.GetString(iArg, null);
			if (str == null)
			{
				return TimeSpan.FromSeconds(0);
			}
			if (TimeSpan.TryParse(str, out timeSpan))
			{
				return timeSpan;
			}
			return TimeSpan.FromSeconds(0);
		}

		public uint GetUInt(int iArg, uint def = 0)
		{
			uint num;
			string str = this.GetString(iArg, null);
			if (str == null)
			{
				return def;
			}
			if (uint.TryParse(str, out num))
			{
				return num;
			}
			return def;
		}

		public ulong GetUInt64(int iArg, ulong def = 0L)
		{
			ulong num;
			string str = this.GetString(iArg, null);
			if (str == null)
			{
				return def;
			}
			if (ulong.TryParse(str, out num))
			{
				return num;
			}
			return def;
		}

		public ulong GetULong(int iArg, ulong def = 0L)
		{
			ulong num;
			string str = this.GetString(iArg, null);
			if (str == null)
			{
				return def;
			}
			if (ulong.TryParse(str, out num))
			{
				return num;
			}
			return def;
		}

		public Vector3 GetVector3(int iArg, Vector3 def = null)
		{
			string str = this.GetString(iArg, null);
			if (str == null)
			{
				return def;
			}
			return str.ToVector3();
		}

		public bool HasArgs(int iMinimum = 1)
		{
			if (this.Args == null)
			{
				return false;
			}
			return (int)this.Args.Length >= iMinimum;
		}

		internal bool HasPermission()
		{
			if (this.cmd == null)
			{
				return false;
			}
			if (this.Option.IsUnrestricted)
			{
				return true;
			}
			if (!this.IsClientside)
			{
				if (this.cmd.ServerAdmin)
				{
					if (this.IsRcon)
					{
						return true;
					}
					if (this.IsAdmin)
					{
						return true;
					}
				}
				if (this.cmd.ServerUser && this.Connection != null)
				{
					return true;
				}
				return false;
			}
			if (this.cmd.ClientAdmin)
			{
				if (ConsoleSystem.ClientCanRunAdminCommands == null)
				{
					return false;
				}
				return ConsoleSystem.ClientCanRunAdminCommands();
			}
			if (!this.Option.IsFromServer || this.cmd.AllowRunFromServer)
			{
				return this.cmd.Client;
			}
			Debug.Log(string.Concat("Server tried to run command \"", this.FullString, "\", but we blocked it."));
			return false;
		}

		public void ReplyWith(string strValue)
		{
			this.Reply = strValue;
		}

		public void ReplyWithObject(object rval)
		{
			if (rval == null)
			{
				return;
			}
			if (rval is string)
			{
				this.ReplyWith((string)rval);
				return;
			}
			this.ReplyWith(JsonConvert.SerializeObject(rval, Formatting.Indented));
		}
	}

	public class Command
	{
		public string Name;

		public string Parent;

		public string FullName;

		public Func<string> GetOveride;

		public Action<string> SetOveride;

		public Action<ConsoleSystem.Arg> Call;

		public bool Variable;

		public bool Saved;

		public bool ServerAdmin;

		public bool ServerUser;

		public bool ClientAdmin;

		public bool Client;

		public bool ClientInfo;

		public bool AllowRunFromServer;

		public string Description;

		public string Arguments;

		public bool AsBool
		{
			get
			{
				return this.String.ToBool();
			}
		}

		public float AsFloat
		{
			get
			{
				return this.String.ToFloat(0f);
			}
		}

		public int AsInt
		{
			get
			{
				return this.String.ToInt(0);
			}
		}

		public Vector3 AsVector3
		{
			get
			{
				return this.String.ToVector3();
			}
		}

		public bool Server
		{
			get
			{
				if (this.ServerAdmin)
				{
					return true;
				}
				return this.ServerUser;
			}
		}

		public string String
		{
			get
			{
				return this.GetOveride();
			}
		}

		public Command()
		{
			this.Call = new Action<ConsoleSystem.Arg>(this.DefaultCall);
		}

		private void DefaultCall(ConsoleSystem.Arg arg)
		{
			if (this.SetOveride == null)
			{
				return;
			}
			if (!arg.HasArgs(1))
			{
				return;
			}
			string str = this.String;
			this.Set(arg.Args[0]);
			if (str != this.String)
			{
				this.ValueChanged();
			}
		}

		public void Set(string value)
		{
			if (this.SetOveride == null)
			{
				return;
			}
			this.SetOveride(value);
		}

		public void Set(float f)
		{
			string str = f.ToString("0.00");
			if (this.String == str)
			{
				return;
			}
			this.Set(str);
		}

		public void Set(bool val)
		{
			if (this.AsBool == val)
			{
				return;
			}
			this.Set((val ? "1" : "0"));
		}

		private void ValueChanged()
		{
			if (this.Saved)
			{
				ConsoleSystem.HasChanges = true;
			}
			if (this.ClientInfo)
			{
				ConsoleSystem.SendToServer(ConsoleSystem.BuildCommand("setinfo", new object[] { this.FullName, this.String }));
			}
			if (this.OnValueChanged != null)
			{
				this.OnValueChanged(this);
			}
		}

		public event Action<ConsoleSystem.Command> OnValueChanged;
	}

	public class Factory : Attribute
	{
		public string Name;

		public Factory(string systemName)
		{
			this.Name = systemName;
		}
	}

	public interface IConsoleCommand
	{
		void Call(ConsoleSystem.Arg arg);
	}

	public static class Index
	{
		public static ConsoleSystem.Command[] All
		{
			get;
			set;
		}

		public static void Initialize(ConsoleSystem.Command[] Commands)
		{
			ConsoleSystem.Index.All = Commands;
			ConsoleSystem.Index.Server.Dict = new Dictionary<string, ConsoleSystem.Command>();
			ConsoleSystem.Index.Client.Dict = new Dictionary<string, ConsoleSystem.Command>();
			ConsoleSystem.Command[] all = ConsoleSystem.Index.All;
			for (int i = 0; i < (int)all.Length; i++)
			{
				ConsoleSystem.Command command = all[i];
				if (command.Server)
				{
					if (!ConsoleSystem.Index.Server.Dict.ContainsKey(command.FullName))
					{
						ConsoleSystem.Index.Server.Dict.Add(command.FullName, command);
					}
					else
					{
						Debug.LogWarning(string.Concat("Server Vars have multiple entries for ", command.FullName));
					}
					if (command.Parent != "global" && !ConsoleSystem.Index.Server.GlobalDict.ContainsKey(command.Name))
					{
						ConsoleSystem.Index.Server.GlobalDict.Add(command.Name, command);
					}
				}
				if (command.Client)
				{
					if (!ConsoleSystem.Index.Client.Dict.ContainsKey(command.FullName))
					{
						ConsoleSystem.Index.Client.Dict.Add(command.FullName, command);
					}
					else
					{
						Debug.LogWarning(string.Concat("Client Vars have multiple entries for ", command.FullName));
					}
					if (command.Parent != "global" && !ConsoleSystem.Index.Client.GlobalDict.ContainsKey(command.Name))
					{
						ConsoleSystem.Index.Client.GlobalDict.Add(command.Name, command);
					}
				}
			}
			Facepunch.Input.RunBind += new Action<string, bool>((string strCommand, bool pressed) => ConsoleSystem.Run(ConsoleSystem.Option.Client, string.Format("{0} {1}", strCommand, pressed), Array.Empty<object>()));
		}

		public static class Client
		{
			public static Dictionary<string, ConsoleSystem.Command> Dict;

			public static Dictionary<string, ConsoleSystem.Command> GlobalDict;

			static Client()
			{
				ConsoleSystem.Index.Client.Dict = new Dictionary<string, ConsoleSystem.Command>(StringComparer.OrdinalIgnoreCase);
				ConsoleSystem.Index.Client.GlobalDict = new Dictionary<string, ConsoleSystem.Command>(StringComparer.OrdinalIgnoreCase);
			}

			public static ConsoleSystem.Command Find(string strName)
			{
				ConsoleSystem.Command command;
				if (!strName.Contains("."))
				{
					strName = string.Concat("global.", strName);
				}
				if (ConsoleSystem.Index.Client.Dict.TryGetValue(strName, out command))
				{
					return command;
				}
				ConsoleSystem.Index.Client.GlobalDict.TryGetValue(strName.Replace("global.", ""), out command);
				return command;
			}
		}

		public static class Server
		{
			public static Dictionary<string, ConsoleSystem.Command> Dict;

			public static Dictionary<string, ConsoleSystem.Command> GlobalDict;

			static Server()
			{
				ConsoleSystem.Index.Server.Dict = new Dictionary<string, ConsoleSystem.Command>(StringComparer.OrdinalIgnoreCase);
				ConsoleSystem.Index.Server.GlobalDict = new Dictionary<string, ConsoleSystem.Command>(StringComparer.OrdinalIgnoreCase);
			}

			public static ConsoleSystem.Command Find(string strName)
			{
				ConsoleSystem.Command command;
				if (!strName.Contains("."))
				{
					strName = string.Concat("global.", strName);
				}
				if (ConsoleSystem.Index.Server.Dict.TryGetValue(strName, out command))
				{
					return command;
				}
				ConsoleSystem.Index.Server.GlobalDict.TryGetValue(strName.Replace("global.", ""), out command);
				return command;
			}
		}
	}

	public struct Option
	{
		public static ConsoleSystem.Option Client
		{
			get
			{
				ConsoleSystem.Option option = new ConsoleSystem.Option()
				{
					IsClient = true,
					ForwardtoServerOnMissing = true,
					PrintOutput = true
				};
				return option;
			}
		}

		public Connection Connection
		{
			get;
			set;
		}

		public bool ForwardtoServerOnMissing
		{
			get;
			set;
		}

		public bool FromRcon
		{
			get;
			set;
		}

		public bool IsClient
		{
			get;
			set;
		}

		public bool IsFromServer
		{
			get;
			set;
		}

		public bool IsServer
		{
			get;
			set;
		}

		public bool IsUnrestricted
		{
			get;
			set;
		}

		public bool PrintOutput
		{
			get;
			set;
		}

		public static ConsoleSystem.Option Server
		{
			get
			{
				ConsoleSystem.Option option = new ConsoleSystem.Option()
				{
					IsServer = true,
					PrintOutput = true,
					FromRcon = true
				};
				return option;
			}
		}

		public static ConsoleSystem.Option Unrestricted
		{
			get
			{
				ConsoleSystem.Option option = new ConsoleSystem.Option()
				{
					IsServer = true,
					IsClient = true,
					ForwardtoServerOnMissing = true,
					PrintOutput = true,
					IsUnrestricted = true
				};
				return option;
			}
		}

		public ConsoleSystem.Option FromConnection(Connection connection)
		{
			this.FromRcon = false;
			this.Connection = connection;
			return this;
		}

		public ConsoleSystem.Option FromServer()
		{
			this.IsFromServer = true;
			return this;
		}

		public ConsoleSystem.Option Quiet()
		{
			this.PrintOutput = false;
			return this;
		}
	}
}