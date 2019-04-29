using ConVar;
using Facepunch.Extend;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using Windows;

public class ServerConsole : SingletonComponent<ServerConsole>
{
	private ConsoleWindow console;

	private ConsoleInput input;

	private float nextUpdate;

	private int currentEntityCount
	{
		get
		{
			return BaseNetworkable.serverEntities.Count;
		}
	}

	private DateTime currentGameTime
	{
		get
		{
			if (!TOD_Sky.Instance)
			{
				return DateTime.Now;
			}
			return TOD_Sky.Instance.Cycle.DateTime;
		}
	}

	private int currentPlayerCount
	{
		get
		{
			return BasePlayer.activePlayerList.Count;
		}
	}

	private int currentSleeperCount
	{
		get
		{
			return BasePlayer.sleepingPlayerList.Count;
		}
	}

	private int maxPlayerCount
	{
		get
		{
			return ConVar.Server.maxplayers;
		}
	}

	public ServerConsole()
	{
		if (Environment.OSVersion.Platform != PlatformID.Unix)
		{
			this.console = new ConsoleWindow();
			this.input = new ConsoleInput();
			base();
		}
	}

	private void HandleLog(string message, string stackTrace, LogType type)
	{
		if (Environment.OSVersion.Platform != PlatformID.Unix)
		{
			if (message.StartsWith("[CHAT]"))
			{
				return;
			}
			if (message.StartsWith("[Physics.PhysX] RigidBody::setRigidBodyFlag"))
			{
				return;
			}
			if (type == LogType.Warning)
			{
				if (message.StartsWith("HDR RenderTexture format is not") || message.StartsWith("The image effect") || message.StartsWith("Image Effects are not supported on this platform") || message.StartsWith("[AmplifyColor]") || message.StartsWith("Skipping profile frame."))
				{
					return;
				}
				System.Console.ForegroundColor = ConsoleColor.Yellow;
			}
			else if (type == LogType.Error)
			{
				System.Console.ForegroundColor = ConsoleColor.Red;
			}
			else if (type == LogType.Exception)
			{
				System.Console.ForegroundColor = ConsoleColor.Red;
			}
			else if (type != LogType.Assert)
			{
				System.Console.ForegroundColor = ConsoleColor.Gray;
			}
			else
			{
				System.Console.ForegroundColor = ConsoleColor.Red;
			}
			this.input.ClearLine((int)this.input.statusText.Length);
			System.Console.WriteLine(message);
			this.input.RedrawInputLine();
		}
	}

	private void OnDisable()
	{
		Output.OnMessage -= new Action<string, string, LogType>(this.HandleLog);
		if (Environment.OSVersion.Platform != PlatformID.Unix)
		{
			this.input.OnInputText -= new Action<string>(this.OnInputText);
			this.console.Shutdown();
		}
	}

	public void OnEnable()
	{
		if (Environment.OSVersion.Platform != PlatformID.Unix)
		{
			this.console.Initialize();
			this.input.OnInputText += new Action<string>(this.OnInputText);
			Output.OnMessage += new Action<string, string, LogType>(this.HandleLog);
			this.input.ClearLine(System.Console.WindowHeight);
			for (int i = 0; i < System.Console.WindowHeight; i++)
			{
				System.Console.WriteLine("");
			}
		}
	}

	private void OnInputText(string obj)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Server, obj, Array.Empty<object>());
	}

	public static void PrintColoured(params object[] objects)
	{
		if (Environment.OSVersion.Platform != PlatformID.Unix)
		{
			if (SingletonComponent<ServerConsole>.Instance == null)
			{
				return;
			}
			SingletonComponent<ServerConsole>.Instance.input.ClearLine((int)SingletonComponent<ServerConsole>.Instance.input.statusText.Length);
			for (int i = 0; i < (int)objects.Length; i++)
			{
				if (i % 2 != 0)
				{
					System.Console.Write((string)objects[i]);
				}
				else
				{
					System.Console.ForegroundColor = (ConsoleColor)objects[i];
				}
			}
			if (System.Console.CursorLeft != 0)
			{
				System.Console.CursorTop = System.Console.CursorTop + 1;
			}
			SingletonComponent<ServerConsole>.Instance.input.RedrawInputLine();
		}
	}

	private void Update()
	{
		if (Environment.OSVersion.Platform != PlatformID.Unix)
		{
			this.UpdateStatus();
			this.input.Update();
		}
	}

	private void UpdateStatus()
	{
		if (this.nextUpdate > UnityEngine.Time.realtimeSinceStartup)
		{
			return;
		}
		if (Network.Net.sv == null || !Network.Net.sv.IsConnected())
		{
			return;
		}
		this.nextUpdate = UnityEngine.Time.realtimeSinceStartup + 0.33f;
		if (!this.input.valid)
		{
			return;
		}
		string str = ((long)UnityEngine.Time.realtimeSinceStartup).FormatSeconds();
		string str1 = this.currentGameTime.ToString("[H:mm]");
		string str2 = string.Concat(new object[] { " ", str1, " [", this.currentPlayerCount, "/", this.maxPlayerCount, "] ", ConVar.Server.hostname, " [", ConVar.Server.level, "]" });
		string str3 = string.Concat(new object[] { Performance.current.frameRate, "fps ", Performance.current.memoryCollections, "gc ", str }) ?? "";
		string str4 = string.Concat(Network.Net.sv.GetStat(null, NetworkPeer.StatTypeLong.BytesReceived_LastSecond).FormatBytes<ulong>(true), "/s in, ", Network.Net.sv.GetStat(null, NetworkPeer.StatTypeLong.BytesSent_LastSecond).FormatBytes<ulong>(true), "/s out");
		string str5 = str3.PadLeft(this.input.lineWidth - 1);
		str5 = string.Concat(str2, (str2.Length < str5.Length ? str5.Substring(str2.Length) : ""));
		string[] strArrays = new string[] { " ", null, null, null, null };
		int num = this.currentEntityCount;
		strArrays[1] = num.ToString("n0");
		strArrays[2] = " ents, ";
		num = this.currentSleeperCount;
		strArrays[3] = num.ToString("n0");
		strArrays[4] = " slprs";
		string str6 = string.Concat(strArrays);
		string str7 = str4.PadLeft(this.input.lineWidth - 1);
		str7 = string.Concat(str6, (str6.Length < str7.Length ? str7.Substring(str6.Length) : ""));
		this.input.statusText[0] = "";
		this.input.statusText[1] = str5;
		this.input.statusText[2] = str7;
	}
}