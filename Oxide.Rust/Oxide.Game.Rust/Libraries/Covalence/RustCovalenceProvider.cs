using Oxide.Core.Libraries.Covalence;
using System;
using System.Runtime.CompilerServices;

namespace Oxide.Game.Rust.Libraries.Covalence
{
	public class RustCovalenceProvider : ICovalenceProvider
	{
		public uint ClientAppId
		{
			get
			{
				return (uint)252490;
			}
		}

		public RustCommandSystem CommandSystem
		{
			get;
			private set;
		}

		public string GameName
		{
			get
			{
				return "Rust";
			}
		}

		internal static RustCovalenceProvider Instance
		{
			get;
			private set;
		}

		public RustPlayerManager PlayerManager
		{
			get;
			private set;
		}

		public uint ServerAppId
		{
			get
			{
				return (uint)258550;
			}
		}

		public RustCovalenceProvider()
		{
			RustCovalenceProvider.Instance = this;
		}

		public ICommandSystem CreateCommandSystemProvider()
		{
			RustCommandSystem rustCommandSystem = new RustCommandSystem();
			RustCommandSystem rustCommandSystem1 = rustCommandSystem;
			this.CommandSystem = rustCommandSystem;
			return rustCommandSystem1;
		}

		public IPlayerManager CreatePlayerManager()
		{
			this.PlayerManager = new RustPlayerManager();
			this.PlayerManager.Initialize();
			return this.PlayerManager;
		}

		public IServer CreateServer()
		{
			return new RustServer();
		}

		public string FormatText(string text)
		{
			return Formatter.ToUnity(text);
		}
	}
}