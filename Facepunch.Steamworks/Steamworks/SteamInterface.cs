using Steamworks.Data;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
	internal abstract class SteamInterface
	{
		public IntPtr Self;

		public IntPtr VTable;

		private static byte[] stringbuffer;

		public virtual string InterfaceName
		{
			get
			{
				return null;
			}
		}

		static SteamInterface()
		{
			SteamInterface.stringbuffer = new Byte[131072];
		}

		protected SteamInterface()
		{
		}

		internal string GetString(IntPtr p)
		{
			string str;
			if (p != IntPtr.Zero)
			{
				lock (SteamInterface.stringbuffer)
				{
					int num = 0;
					while (true)
					{
						if ((Marshal.ReadByte(p, num) == 0 ? true : num >= (int)SteamInterface.stringbuffer.Length))
						{
							break;
						}
						num++;
					}
					Marshal.Copy(p, SteamInterface.stringbuffer, 0, num);
					str = Encoding.UTF8.GetString(SteamInterface.stringbuffer, 0, num);
				}
			}
			else
			{
				str = null;
			}
			return str;
		}

		public void Init()
		{
			if (!SteamClient.IsValid)
			{
				if (!SteamServer.IsValid)
				{
					throw new Exception("Trying to initialize Steam Interface but Steam not initialized");
				}
				this.InitServer();
			}
			else
			{
				this.InitClient();
			}
		}

		public void InitClient()
		{
			this.Self = SteamInternal.CreateInterface(this.InterfaceName);
			if (this.Self == IntPtr.Zero)
			{
				HSteamUser hSteamUser = SteamAPI.GetHSteamUser();
				this.Self = SteamInternal.FindOrCreateUserInterface(hSteamUser, this.InterfaceName);
			}
			if (this.Self == IntPtr.Zero)
			{
				throw new Exception(String.Concat("Couldn't find interface ", this.InterfaceName));
			}
			this.VTable = Marshal.ReadIntPtr(this.Self, 0);
			if (this.Self == IntPtr.Zero)
			{
				throw new Exception(String.Concat("Invalid VTable for ", this.InterfaceName));
			}
			this.InitInternals();
			SteamClient.WatchInterface(this);
		}

		public abstract void InitInternals();

		public void InitServer()
		{
			HSteamUser hSteamUser = SteamGameServer.GetHSteamUser();
			this.Self = SteamInternal.FindOrCreateGameServerInterface(hSteamUser, this.InterfaceName);
			if (this.Self == IntPtr.Zero)
			{
				throw new Exception(String.Concat("Couldn't find server interface ", this.InterfaceName));
			}
			this.VTable = Marshal.ReadIntPtr(this.Self, 0);
			if (this.Self == IntPtr.Zero)
			{
				throw new Exception(String.Concat("Invalid VTable for server ", this.InterfaceName));
			}
			this.InitInternals();
			SteamServer.WatchInterface(this);
		}

		public virtual void InitUserless()
		{
			this.Self = SteamInternal.FindOrCreateUserInterface(0, this.InterfaceName);
			if (this.Self == IntPtr.Zero)
			{
				throw new Exception(String.Concat("Couldn't find interface ", this.InterfaceName));
			}
			this.VTable = Marshal.ReadIntPtr(this.Self, 0);
			if (this.Self == IntPtr.Zero)
			{
				throw new Exception(String.Concat("Invalid VTable for ", this.InterfaceName));
			}
			this.InitInternals();
		}

		internal virtual void Shutdown()
		{
			this.Self = IntPtr.Zero;
			this.VTable = IntPtr.Zero;
		}
	}
}