using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class Connection
	{
		public Connection.State state;

		public bool active;

		public bool connected;

		public uint authLevel;

		public uint encryptionLevel;

		public bool decryptIncoming;

		public bool encryptOutgoing;

		public bool rejected;

		public string authStatus;

		public byte[] token;

		public ulong guid;

		public ulong userid;

		public ulong ownerid;

		public string username;

		public string os;

		public uint protocol;

		private Connection.TimeAverageValue[] packetsPerSecond = new Connection.TimeAverageValue[24];

		public DateTime connectionTime;

		public string ipaddress;

		public MonoBehaviour player;

		public Connection.Validation validate;

		public Connection.ClientInfo info = new Connection.ClientInfo();

		public bool isAuthenticated
		{
			get
			{
				return this.authStatus == "ok";
			}
		}

		public Connection()
		{
		}

		public void AddPacketsPerSecond(Message.Type message)
		{
			this.AddPacketsPerSecond((int)message);
		}

		public void AddPacketsPerSecond(int index = 0)
		{
			if (index < 0 || index >= (int)this.packetsPerSecond.Length)
			{
				return;
			}
			this.packetsPerSecond[index].Increment();
		}

		public ulong GetPacketsPerSecond(Message.Type message)
		{
			return this.GetPacketsPerSecond((int)message);
		}

		public ulong GetPacketsPerSecond(int index = 0)
		{
			if (index < 0 || index >= (int)this.packetsPerSecond.Length)
			{
				return (ulong)0;
			}
			return this.packetsPerSecond[index].Calculate();
		}

		public float GetSecondsConnected()
		{
			return (float)DateTime.Now.Subtract(this.connectionTime).TotalSeconds;
		}

		public virtual void OnDisconnected()
		{
			this.player = null;
			this.guid = (ulong)0;
			this.ResetPacketsPerSecond();
		}

		public void ResetPacketsPerSecond()
		{
			for (int i = 0; i < (int)this.packetsPerSecond.Length; i++)
			{
				this.packetsPerSecond[i].Reset();
			}
		}

		public override string ToString()
		{
			return string.Format("{0}/{1}/{2}", this.ipaddress, this.userid, this.username);
		}

		public class ClientInfo
		{
			public Dictionary<string, string> info;

			public ClientInfo()
			{
			}

			public bool GetBool(string k, bool def = false)
			{
				bool flag;
				string str = this.GetString(k, null);
				if (str == null)
				{
					return def;
				}
				if (bool.TryParse(str, out flag))
				{
					return flag;
				}
				return def;
			}

			public float GetFloat(string k, float def = 0f)
			{
				float single;
				string str = this.GetString(k, null);
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

			public int GetInt(string k, int def = 0)
			{
				return (int)this.GetFloat(k, (float)def);
			}

			public string GetString(string k, string def = "")
			{
				string str;
				if (this.info.TryGetValue(k, out str))
				{
					return str;
				}
				return def;
			}

			public void Set(string k, string v)
			{
				this.info[k] = v;
			}
		}

		public enum State
		{
			Unconnected,
			Connecting,
			InQueue,
			Welcoming,
			Connected,
			Disconnected
		}

		public struct TimeAverageValue
		{
			private DateTime refreshTime;

			private ulong counterPrev;

			private ulong counterNext;

			public ulong Calculate()
			{
				DateTime now = DateTime.Now;
				double totalSeconds = now.Subtract(this.refreshTime).TotalSeconds;
				if (totalSeconds < 0)
				{
					totalSeconds = 0;
					this.refreshTime = now;
					this.counterNext = (ulong)0;
				}
				if (totalSeconds >= 1)
				{
					totalSeconds = 0;
					this.refreshTime = now;
					this.counterPrev = this.counterNext;
					this.counterNext = (ulong)0;
				}
				return (ulong)((double)((float)this.counterPrev) * (1 - totalSeconds)) + this.counterNext;
			}

			public void Increment()
			{
				this.counterNext += (long)1;
			}

			public void Reset()
			{
				this.counterPrev = (ulong)0;
				this.counterNext = (ulong)0;
			}
		}

		public struct Validation
		{
			public uint entityUpdates;
		}
	}
}