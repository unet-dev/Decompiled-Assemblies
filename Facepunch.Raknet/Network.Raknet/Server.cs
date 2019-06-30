using Facepunch;
using Network;
using System;
using System.Diagnostics;
using UnityEngine;

namespace Facepunch.Network.Raknet
{
	public class Server : Network.Server
	{
		public static float MaxReceiveTime;

		public static ulong MaxPacketsPerSecond;

		public static int MaxPacketSize;

		private Peer peer;

		static Server()
		{
			Facepunch.Network.Raknet.Server.MaxReceiveTime = 20f;
			Facepunch.Network.Raknet.Server.MaxPacketsPerSecond = (ulong)1500;
			Facepunch.Network.Raknet.Server.MaxPacketSize = 5000000;
		}

		public Server()
		{
		}

		internal void ConnectedPacket(Connection connection)
		{
			if (connection.GetPacketsPerSecond(0) >= Facepunch.Network.Raknet.Server.MaxPacketsPerSecond)
			{
				this.Kick(connection, "Kicked: Packet Flooding");
				UnityEngine.Debug.LogWarning(string.Concat(connection.ToString(), " was kicked for packet flooding"));
				return;
			}
			connection.AddPacketsPerSecond(0);
			base.read.Start(this.peer.RawData(), this.peer.incomingBytesUnread);
			base.Decrypt(connection, base.read);
			byte num = base.read.PacketID();
			if (this.HandleRaknetPacket(num, connection))
			{
				return;
			}
			num = (byte)(num - 140);
			Message message = base.StartMessage((Message.Type)num, connection);
			if (this.callbackHandler != null)
			{
				this.callbackHandler.OnNetworkMessage(message);
			}
			message.Clear();
			Pool.Free<Message>(ref message);
		}

		public override void Cycle()
		{
			base.Cycle();
			if (!this.IsConnected())
			{
				return;
			}
			Stopwatch stopwatch = Pool.Get<Stopwatch>();
			stopwatch.Reset();
			stopwatch.Start();
			do
			{
			Label0:
				if (!this.peer.Receive())
				{
					break;
				}
				if (this.peer.incomingBytesUnread <= Facepunch.Network.Raknet.Server.MaxPacketSize)
				{
					Connection connection = base.FindConnection(this.peer.incomingGUID);
					if (connection != null)
					{
						using (TimeWarning timeWarning = TimeWarning.New("ConnectedPacket", (long)20))
						{
							this.ConnectedPacket(connection);
						}
					}
					else
					{
						using (timeWarning = TimeWarning.New("UnconnectedPacket", (long)20))
						{
							this.UnconnectedPacket();
						}
					}
				}
				else
				{
					goto Label0;
				}
			}
			while (stopwatch.Elapsed.TotalMilliseconds <= (double)Facepunch.Network.Raknet.Server.MaxReceiveTime);
			Pool.Free<Stopwatch>(ref stopwatch);
		}

		public override int GetAveragePing(Connection connection)
		{
			if (this.peer == null)
			{
				return 0;
			}
			return this.peer.GetPingAverage(connection.guid);
		}

		public override string GetDebug(Connection connection)
		{
			if (this.peer == null)
			{
				return string.Empty;
			}
			if (connection == null)
			{
				return this.peer.GetStatisticsString((ulong)0);
			}
			return this.peer.GetStatisticsString(connection.guid);
		}

		public override ulong GetStat(Connection connection, NetworkPeer.StatTypeLong type)
		{
			if (this.peer == null)
			{
				return (ulong)0;
			}
			return this.peer.GetStat(connection, type);
		}

		internal bool HandleRaknetPacket(byte type, Connection connection)
		{
			if (type >= 140)
			{
				return false;
			}
			switch (type)
			{
				case 19:
				{
					using (TimeWarning timeWarning = TimeWarning.New("OnNewConnection", (long)20))
					{
						this.OnNewConnection();
					}
					return true;
				}
				case 20:
				{
					return true;
				}
				case 21:
				{
					if (connection != null)
					{
						using (timeWarning = TimeWarning.New("OnDisconnected", (long)20))
						{
							base.OnDisconnected("Disconnected", connection);
						}
					}
					return true;
				}
				case 22:
				{
					if (connection != null)
					{
						using (timeWarning = TimeWarning.New("OnDisconnected (timed out)", (long)20))
						{
							base.OnDisconnected("Timed Out", connection);
						}
					}
					return true;
				}
				default:
				{
					return true;
				}
			}
		}

		public override bool IsConnected()
		{
			return this.peer != null;
		}

		public override void Kick(Connection cn, string message)
		{
			if (this.peer == null)
			{
				return;
			}
			if (this.write.Start())
			{
				this.write.PacketID(Message.Type.DisconnectReason);
				this.write.String(message);
				Write write = this.write;
				SendInfo sendInfo = new SendInfo(cn)
				{
					method = SendMethod.ReliableUnordered,
					priority = Priority.Immediate
				};
				write.Send(sendInfo);
			}
			Console.WriteLine(string.Concat(cn.ToString(), " kicked: ", message));
			this.peer.Kick(cn);
			base.OnDisconnected(string.Concat("Kicked: ", message), cn);
		}

		protected override void OnNewConnection()
		{
			ulong num = this.peer.incomingGUID;
			string str = this.peer.incomingAddress;
			if (string.IsNullOrEmpty(str) || str == "UNASSIGNED_SYSTEM_ADDRESS")
			{
				return;
			}
			Connection connection = new Connection()
			{
				guid = num,
				ipaddress = str,
				active = true
			};
			base.OnNewConnection(connection);
		}

		public override unsafe void SendUnconnected(uint netAddr, ushort netPort, byte[] data, int size)
		{
			// 
			// Current member / type: System.Void Facepunch.Network.Raknet.Server::SendUnconnected(System.UInt32,System.UInt16,System.Byte[],System.Int32)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Raknet.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void SendUnconnected(System.UInt32,System.UInt16,System.Byte[],System.Int32)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public override bool Start()
		{
			this.peer = Peer.CreateServer(this.ip, this.port, 1024);
			if (this.peer == null)
			{
				return false;
			}
			this.write = new StreamWrite(this, this.peer);
			return true;
		}

		public override void Stop(string shutdownMsg)
		{
			if (this.peer == null)
			{
				return;
			}
			Console.WriteLine(string.Concat("[Raknet] Server Shutting Down (", shutdownMsg, ")"));
			(this.write as StreamWrite).Shutdown();
			using (TimeWarning timeWarning = TimeWarning.New("ServerStop", 0.1f))
			{
				this.peer.Close();
				this.peer = null;
				base.Stop(shutdownMsg);
			}
		}

		internal void UnconnectedPacket()
		{
			base.read.Start(this.peer.RawData(), this.peer.incomingBytesUnread);
			byte num = base.read.PacketID();
			if (this.callbackHandler != null && this.callbackHandler.OnUnconnectedMessage((int)num, base.read, this.peer.incomingAddressInt, (int)this.peer.incomingPort))
			{
				return;
			}
			this.HandleRaknetPacket(num, null);
		}
	}
}