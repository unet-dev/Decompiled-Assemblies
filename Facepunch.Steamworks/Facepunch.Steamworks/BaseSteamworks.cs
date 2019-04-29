using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Facepunch.Steamworks
{
	public class BaseSteamworks : IDisposable
	{
		internal NativeInterface native;

		private List<CallbackHandle> CallbackHandles = new List<CallbackHandle>();

		private List<CallResult> CallResults = new List<CallResult>();

		protected bool disposed;

		public Action<object> OnAnyCallback;

		private Dictionary<Type, List<Action<object>>> Callbacks = new Dictionary<Type, List<Action<object>>>();

		public uint AppId
		{
			get;
			internal set;
		}

		public Facepunch.Steamworks.Inventory Inventory
		{
			get;
			internal set;
		}

		internal virtual bool IsGameServer
		{
			get
			{
				return false;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.native != null;
			}
		}

		public Facepunch.Steamworks.Networking Networking
		{
			get;
			internal set;
		}

		public Facepunch.Steamworks.Workshop Workshop
		{
			get;
			internal set;
		}

		protected BaseSteamworks(uint appId)
		{
			this.AppId = appId;
			uint num = this.AppId;
			Environment.SetEnvironmentVariable("SteamAppId", num.ToString());
			num = this.AppId;
			Environment.SetEnvironmentVariable("SteamGameId", num.ToString());
		}

		internal List<Action<object>> CallbackList(Type T)
		{
			List<Action<object>> actions = null;
			if (!this.Callbacks.TryGetValue(T, out actions))
			{
				actions = new List<Action<object>>();
				this.Callbacks[T] = actions;
			}
			return actions;
		}

		public virtual void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.Callbacks.Clear();
			foreach (CallbackHandle callbackHandle in this.CallbackHandles)
			{
				callbackHandle.Dispose();
			}
			this.CallbackHandles.Clear();
			foreach (CallResult callResult in this.CallResults)
			{
				callResult.Dispose();
			}
			this.CallResults.Clear();
			if (this.Workshop != null)
			{
				this.Workshop.Dispose();
				this.Workshop = null;
			}
			if (this.Inventory != null)
			{
				this.Inventory.Dispose();
				this.Inventory = null;
			}
			if (this.Networking != null)
			{
				this.Networking.Dispose();
				this.Networking = null;
			}
			if (this.native != null)
			{
				this.native.Dispose();
				this.native = null;
			}
			Environment.SetEnvironmentVariable("SteamAppId", null);
			Environment.SetEnvironmentVariable("SteamGameId", null);
			this.disposed = true;
		}

		~BaseSteamworks()
		{
			this.Dispose();
		}

		internal void OnCallback<T>(T data)
		{
			foreach (Action<object> action in this.CallbackList(typeof(T)))
			{
				action(data);
			}
			if (this.OnAnyCallback != null)
			{
				this.OnAnyCallback(data);
			}
		}

		internal void RegisterCallback<T>(Action<T> func)
		{
			this.CallbackList(typeof(T)).Add(new Action<object>((object o) => func((T)o)));
		}

		internal void RegisterCallbackHandle(CallbackHandle handle)
		{
			this.CallbackHandles.Add(handle);
		}

		internal void RegisterCallResult(CallResult handle)
		{
			this.CallResults.Add(handle);
		}

		public void RunUpdateCallbacks()
		{
			if (this.OnUpdate != null)
			{
				this.OnUpdate();
			}
			for (int i = 0; i < this.CallResults.Count; i++)
			{
				this.CallResults[i].Try();
			}
			SourceServerQuery.Cycle();
		}

		protected void SetupCommonInterfaces()
		{
			this.Networking = new Facepunch.Steamworks.Networking(this, this.native.networking);
			this.Inventory = new Facepunch.Steamworks.Inventory(this, this.native.inventory, this.IsGameServer);
			this.Workshop = new Facepunch.Steamworks.Workshop(this, this.native.ugc, this.native.remoteStorage);
		}

		internal void UnregisterCallResult(CallResult handle)
		{
			this.CallResults.Remove(handle);
		}

		public virtual void Update()
		{
			this.Networking.Update();
			this.RunUpdateCallbacks();
		}

		public void UpdateWhile(Func<bool> func)
		{
			while (func())
			{
				this.Update();
				Thread.Sleep(1);
			}
		}

		internal event Action OnUpdate;
	}
}