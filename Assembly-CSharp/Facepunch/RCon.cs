using ConVar;
using Facepunch.Rcon;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Facepunch
{
	public class RCon
	{
		public static string Password;

		[ServerVar]
		public static int Port;

		[ServerVar]
		public static string Ip;

		[ServerVar(Help="If set to true, use websocket rcon. If set to false use legacy, source engine rcon.")]
		public static bool Web;

		[ServerVar(Help="If true, rcon commands etc will be printed in the console")]
		public static bool Print;

		internal static RCon.RConListener listener;

		internal static Listener listenerNew;

		private static Queue<RCon.Command> Commands;

		private static float lastRunTime;

		internal static List<RCon.BannedAddresses> bannedAddresses;

		private static int responseIdentifier;

		private static string responseConnection;

		private static bool isInput;

		internal static int SERVERDATA_AUTH;

		internal static int SERVERDATA_EXECCOMMAND;

		internal static int SERVERDATA_AUTH_RESPONSE;

		internal static int SERVERDATA_RESPONSE_VALUE;

		internal static int SERVERDATA_CONSOLE_LOG;

		internal static int SERVERDATA_SWITCH_UTF8;

		static RCon()
		{
			RCon.Password = "";
			RCon.Port = 0;
			RCon.Ip = "";
			RCon.Web = true;
			RCon.Print = false;
			RCon.listener = null;
			RCon.listenerNew = null;
			RCon.Commands = new Queue<RCon.Command>();
			RCon.lastRunTime = 0f;
			RCon.bannedAddresses = new List<RCon.BannedAddresses>();
			RCon.SERVERDATA_AUTH = 3;
			RCon.SERVERDATA_EXECCOMMAND = 2;
			RCon.SERVERDATA_AUTH_RESPONSE = 2;
			RCon.SERVERDATA_RESPONSE_VALUE = 0;
			RCon.SERVERDATA_CONSOLE_LOG = 4;
			RCon.SERVERDATA_SWITCH_UTF8 = 5;
		}

		public RCon()
		{
		}

		public static void BanIP(IPAddress addr, float seconds)
		{
			RCon.bannedAddresses.RemoveAll((RCon.BannedAddresses x) => x.addr == addr);
			RCon.BannedAddresses bannedAddress = new RCon.BannedAddresses()
			{
				addr = addr,
				banTime = UnityEngine.Time.realtimeSinceStartup + seconds
			};
		}

		public static void Broadcast(RCon.LogType type, object obj)
		{
			if (RCon.listenerNew == null)
			{
				return;
			}
			RCon.Response response = new RCon.Response()
			{
				Identifier = -1,
				Message = JsonConvert.SerializeObject(obj, Formatting.Indented),
				Type = type
			};
			if (string.IsNullOrEmpty(RCon.responseConnection))
			{
				RCon.listenerNew.BroadcastMessage(JsonConvert.SerializeObject(response, Formatting.Indented));
				return;
			}
			RCon.listenerNew.SendMessage(RCon.responseConnection, JsonConvert.SerializeObject(response, Formatting.Indented));
		}

		public static void Initialize()
		{
			if (Interface.CallHook("IOnRconInitialize") != null)
			{
				return;
			}
			if (RCon.Port == 0)
			{
				RCon.Port = Server.port;
			}
			RCon.Password = Facepunch.CommandLine.GetSwitch("-rcon.password", Facepunch.CommandLine.GetSwitch("+rcon.password", ""));
			if (RCon.Password == "password")
			{
				return;
			}
			if (RCon.Password == "")
			{
				return;
			}
			Output.OnMessage += new Action<string, string, UnityEngine.LogType>(RCon.OnMessage);
			if (!RCon.Web)
			{
				RCon.listener = new RCon.RConListener();
				Debug.Log(string.Concat("RCon Started on ", RCon.Port));
				Debug.Log("Source style TCP Rcon is deprecated. Please switch to Websocket Rcon before it goes away.");
				return;
			}
			RCon.listenerNew = new Listener();
			if (!string.IsNullOrEmpty(RCon.Ip))
			{
				RCon.listenerNew.Address = RCon.Ip;
			}
			RCon.listenerNew.Password = RCon.Password;
			RCon.listenerNew.Port = RCon.Port;
			RCon.listenerNew.SslCertificate = Facepunch.CommandLine.GetSwitch("-rcon.ssl", null);
			RCon.listenerNew.SslCertificatePassword = Facepunch.CommandLine.GetSwitch("-rcon.sslpwd", null);
			RCon.listenerNew.OnMessage = (IPEndPoint ip, string id, string msg) => {
				lock (RCon.Commands)
				{
					RCon.Command command = JsonConvert.DeserializeObject<RCon.Command>(msg);
					command.Ip = ip;
					command.ConnectionId = id;
					RCon.Commands.Enqueue(command);
				}
			};
			RCon.listenerNew.Start();
			Debug.Log(string.Concat("WebSocket RCon Started on ", RCon.Port));
		}

		public static bool IsBanned(IPAddress addr)
		{
			return RCon.bannedAddresses.Count<RCon.BannedAddresses>((RCon.BannedAddresses x) => {
				if (x.addr != addr)
				{
					return false;
				}
				return x.banTime > UnityEngine.Time.realtimeSinceStartup;
			}) > 0;
		}

		private static void OnCommand(RCon.Command cmd)
		{
			try
			{
				RCon.responseIdentifier = cmd.Identifier;
				RCon.responseConnection = cmd.ConnectionId;
				RCon.isInput = true;
				if (RCon.Print)
				{
					Debug.Log(string.Concat(new object[] { "[rcon] ", cmd.Ip, ": ", cmd.Message }));
				}
				RCon.isInput = false;
				ConsoleSystem.Option server = ConsoleSystem.Option.Server;
				string str = ConsoleSystem.Run(server.Quiet(), cmd.Message, Array.Empty<object>());
				if (str != null)
				{
					RCon.OnMessage(str, string.Empty, UnityEngine.LogType.Log);
				}
			}
			finally
			{
				RCon.responseIdentifier = 0;
				RCon.responseConnection = string.Empty;
			}
		}

		private static void OnMessage(string message, string stacktrace, UnityEngine.LogType type)
		{
			if (RCon.isInput)
			{
				return;
			}
			if (RCon.listenerNew != null)
			{
				RCon.Response response = new RCon.Response()
				{
					Identifier = RCon.responseIdentifier,
					Message = message,
					Stacktrace = stacktrace,
					Type = RCon.LogType.Generic
				};
				if (type == UnityEngine.LogType.Error || type == UnityEngine.LogType.Exception)
				{
					response.Type = RCon.LogType.Error;
				}
				if (type == UnityEngine.LogType.Assert || type == UnityEngine.LogType.Warning)
				{
					response.Type = RCon.LogType.Warning;
				}
				if (string.IsNullOrEmpty(RCon.responseConnection))
				{
					RCon.listenerNew.BroadcastMessage(JsonConvert.SerializeObject(response, Formatting.Indented));
					return;
				}
				RCon.listenerNew.SendMessage(RCon.responseConnection, JsonConvert.SerializeObject(response, Formatting.Indented));
			}
		}

		public static void Shutdown()
		{
			if (RCon.listenerNew != null)
			{
				RCon.listenerNew.Shutdown();
				RCon.listenerNew = null;
			}
			if (RCon.listener != null)
			{
				RCon.listener.Shutdown();
				RCon.listener = null;
			}
		}

		public static void Update()
		{
			lock (RCon.Commands)
			{
				while (RCon.Commands.Count > 0)
				{
					RCon.OnCommand(RCon.Commands.Dequeue());
				}
			}
			if (RCon.listener == null)
			{
				return;
			}
			if (RCon.lastRunTime + 0.02f >= UnityEngine.Time.realtimeSinceStartup)
			{
				return;
			}
			RCon.lastRunTime = UnityEngine.Time.realtimeSinceStartup;
			try
			{
				RCon.bannedAddresses.RemoveAll((RCon.BannedAddresses x) => x.banTime < UnityEngine.Time.realtimeSinceStartup);
				RCon.listener.Cycle();
			}
			catch (Exception exception)
			{
				Debug.LogWarning("Rcon Exception");
				Debug.LogException(exception);
			}
		}

		internal struct BannedAddresses
		{
			public IPAddress addr;

			public float banTime;
		}

		public struct Command
		{
			public IPEndPoint Ip;

			public string ConnectionId;

			public string Name;

			public string Message;

			public int Identifier;
		}

		public enum LogType
		{
			Generic,
			Error,
			Warning,
			Chat
		}

		internal class RConClient
		{
			private Socket socket;

			private bool isAuthorised;

			private string connectionName;

			private int lastMessageID;

			private bool runningConsoleCommand;

			private bool utf8Mode;

			internal RConClient(Socket cl)
			{
				this.socket = cl;
				this.socket.NoDelay = true;
				this.connectionName = this.socket.RemoteEndPoint.ToString();
			}

			internal void Close(string strReasn)
			{
				Output.OnMessage -= new Action<string, string, UnityEngine.LogType>(this.Output_OnMessage);
				if (this.socket == null)
				{
					return;
				}
				Debug.Log(string.Concat("[RCON][", this.connectionName, "] Disconnected: ", strReasn));
				this.socket.Close();
				this.socket = null;
			}

			internal bool HandleMessage(int type, string msg)
			{
				if (!this.isAuthorised)
				{
					return this.HandleMessage_UnAuthed(type, msg);
				}
				if (type == RCon.SERVERDATA_SWITCH_UTF8)
				{
					this.utf8Mode = true;
					return true;
				}
				if (type == RCon.SERVERDATA_EXECCOMMAND)
				{
					Debug.Log(string.Concat("[RCON][", this.connectionName, "] ", msg));
					this.runningConsoleCommand = true;
					ConsoleSystem.Run(ConsoleSystem.Option.Server, msg, Array.Empty<object>());
					this.runningConsoleCommand = false;
					this.Reply(-1, RCon.SERVERDATA_RESPONSE_VALUE, "");
					return true;
				}
				if (type == RCon.SERVERDATA_RESPONSE_VALUE)
				{
					this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, "");
					return true;
				}
				Debug.Log(string.Concat(new object[] { "[RCON][", this.connectionName, "] Unhandled: ", this.lastMessageID, " -> ", type, " -> ", msg }));
				return false;
			}

			internal bool HandleMessage_UnAuthed(int type, string msg)
			{
				if (type != RCon.SERVERDATA_AUTH)
				{
					RCon.BanIP((this.socket.RemoteEndPoint as IPEndPoint).Address, 60f);
					this.Close("Invalid Command - Not Authed");
					return false;
				}
				this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, "");
				this.isAuthorised = RCon.Password == msg;
				if (!this.isAuthorised)
				{
					this.Reply(-1, RCon.SERVERDATA_AUTH_RESPONSE, "");
					RCon.BanIP((this.socket.RemoteEndPoint as IPEndPoint).Address, 60f);
					this.Close("Invalid Password");
					return true;
				}
				this.Reply(this.lastMessageID, RCon.SERVERDATA_AUTH_RESPONSE, "");
				Debug.Log(string.Concat("[RCON] Auth: ", this.connectionName));
				Output.OnMessage += new Action<string, string, UnityEngine.LogType>(this.Output_OnMessage);
				return true;
			}

			internal bool IsValid()
			{
				return this.socket != null;
			}

			private void Output_OnMessage(string message, string stacktrace, UnityEngine.LogType type)
			{
				if (!this.isAuthorised)
				{
					return;
				}
				if (!this.IsValid())
				{
					return;
				}
				if (this.lastMessageID != -1 && this.runningConsoleCommand)
				{
					this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, message);
				}
				this.Reply(0, RCon.SERVERDATA_CONSOLE_LOG, message);
			}

			internal string ReadNullTerminatedString(BinaryReader read)
			{
				string str = "";
				do
				{
					if (read.BaseStream.Position == read.BaseStream.Length)
					{
						return "";
					}
					char chr = read.ReadChar();
					if (chr == 0)
					{
						return str;
					}
					str = string.Concat(str, chr.ToString());
				}
				while (str.Length <= 8192);
				return string.Empty;
			}

			internal void Reply(int id, int type, string msg)
			{
				MemoryStream memoryStream = new MemoryStream(1024);
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					if (!this.utf8Mode)
					{
						binaryWriter.Write(10 + msg.Length);
						binaryWriter.Write(id);
						binaryWriter.Write(type);
						string str = msg;
						for (int i = 0; i < str.Length; i++)
						{
							binaryWriter.Write((sbyte)str[i]);
						}
					}
					else
					{
						byte[] bytes = Encoding.UTF8.GetBytes(msg);
						binaryWriter.Write(10 + (int)bytes.Length);
						binaryWriter.Write(id);
						binaryWriter.Write(type);
						binaryWriter.Write(bytes);
					}
					binaryWriter.Write((sbyte)0);
					binaryWriter.Write((sbyte)0);
					binaryWriter.Flush();
					try
					{
						this.socket.Send(memoryStream.GetBuffer(), (int)memoryStream.Position, SocketFlags.None);
					}
					catch (Exception exception)
					{
						Debug.LogWarning(string.Concat("Error sending rcon reply: ", exception));
						this.Close("Exception");
					}
				}
			}

			internal void Update()
			{
				if (this.socket == null)
				{
					return;
				}
				if (!this.socket.Connected)
				{
					this.Close("Disconnected");
					return;
				}
				int available = this.socket.Available;
				if (available < 14)
				{
					return;
				}
				if (available > 4096)
				{
					this.Close("overflow");
					return;
				}
				byte[] numArray = new byte[available];
				this.socket.Receive(numArray);
				using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(numArray, false), (this.utf8Mode ? Encoding.UTF8 : Encoding.ASCII)))
				{
					if (available >= binaryReader.ReadInt32())
					{
						this.lastMessageID = binaryReader.ReadInt32();
						int num = binaryReader.ReadInt32();
						string str = this.ReadNullTerminatedString(binaryReader);
						this.ReadNullTerminatedString(binaryReader);
						if (this.HandleMessage(num, str))
						{
							this.lastMessageID = -1;
						}
						else
						{
							this.Close("invalid packet");
						}
					}
					else
					{
						this.Close("invalid packet");
					}
				}
			}
		}

		internal class RConListener
		{
			private TcpListener server;

			private List<RCon.RConClient> clients;

			internal RConListener()
			{
				IPAddress any = IPAddress.Any;
				if (!IPAddress.TryParse(RCon.Ip, out any))
				{
					any = IPAddress.Any;
				}
				this.server = new TcpListener(any, RCon.Port);
				try
				{
					this.server.Start();
				}
				catch (Exception exception)
				{
					Debug.LogWarning(string.Concat("Couldn't start RCON Listener: ", exception.Message));
					this.server = null;
				}
			}

			internal void Cycle()
			{
				if (this.server == null)
				{
					return;
				}
				this.ProcessConnections();
				this.RemoveDeadClients();
				this.UpdateClients();
			}

			private void ProcessConnections()
			{
				if (!this.server.Pending())
				{
					return;
				}
				Socket socket = this.server.AcceptSocket();
				if (socket == null)
				{
					return;
				}
				IPEndPoint remoteEndPoint = socket.RemoteEndPoint as IPEndPoint;
				if (Interface.CallHook("OnRconConnection", remoteEndPoint) != null)
				{
					socket.Close();
					return;
				}
				if (!RCon.IsBanned(remoteEndPoint.Address))
				{
					this.clients.Add(new RCon.RConClient(socket));
					return;
				}
				Debug.Log(string.Concat("[RCON] Ignoring connection - banned. ", remoteEndPoint.Address.ToString()));
				socket.Close();
			}

			private void RemoveDeadClients()
			{
				this.clients.RemoveAll((RCon.RConClient x) => !x.IsValid());
			}

			internal void Shutdown()
			{
				if (this.server != null)
				{
					this.server.Stop();
					this.server = null;
				}
			}

			private void UpdateClients()
			{
				foreach (RCon.RConClient client in this.clients)
				{
					client.Update();
				}
			}
		}

		public struct Response
		{
			public string Message;

			public int Identifier;

			[JsonConverter(typeof(StringEnumConverter))]
			public RCon.LogType Type;

			public string Stacktrace;
		}
	}
}