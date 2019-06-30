using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class Event : IDisposable
	{
		internal static List<IDisposable> AllClient;

		internal static List<IDisposable> AllServer;

		private bool IsAllocated;

		private List<GCHandle> Allocations = new List<GCHandle>();

		internal IntPtr vTablePtr;

		internal GCHandle PinnedCallback;

		static Event()
		{
			Event.AllClient = new List<IDisposable>();
			Event.AllServer = new List<IDisposable>();
		}

		public Event()
		{
		}

		private static IntPtr BuildVTable(Callback.Run run, List<GCHandle> allocations)
		{
			Callback.RunCall runCall = new Callback.RunCall(Callback.RunStub);
			Callback.GetCallbackSizeBytes getCallbackSizeByte = new Callback.GetCallbackSizeBytes(Callback.SizeStub);
			allocations.Add(GCHandle.Alloc(run));
			allocations.Add(GCHandle.Alloc(runCall));
			allocations.Add(GCHandle.Alloc(getCallbackSizeByte));
			IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate<Callback.Run>(run);
			IntPtr intPtr = Marshal.GetFunctionPointerForDelegate<Callback.RunCall>(runCall);
			IntPtr functionPointerForDelegate1 = Marshal.GetFunctionPointerForDelegate<Callback.GetCallbackSizeBytes>(getCallbackSizeByte);
			IntPtr intPtr1 = Marshal.AllocHGlobal(IntPtr.Size * 3);
			if (Config.Os != OsType.Windows)
			{
				int size = IntPtr.Size;
				Marshal.WriteIntPtr(intPtr1, 0, functionPointerForDelegate);
				Marshal.WriteIntPtr(intPtr1, IntPtr.Size, intPtr);
				Marshal.WriteIntPtr(intPtr1, IntPtr.Size * 2, functionPointerForDelegate1);
			}
			else
			{
				int num = IntPtr.Size;
				Marshal.WriteIntPtr(intPtr1, 0, intPtr);
				Marshal.WriteIntPtr(intPtr1, IntPtr.Size, functionPointerForDelegate);
				Marshal.WriteIntPtr(intPtr1, IntPtr.Size * 2, functionPointerForDelegate1);
			}
			return intPtr1;
		}

		public void Dispose()
		{
			if (this.IsAllocated)
			{
				this.IsAllocated = false;
				if (!this.PinnedCallback.IsAllocated)
				{
					throw new Exception("Callback isn't allocated!?");
				}
				SteamClient.UnregisterCallback(this.PinnedCallback.AddrOfPinnedObject());
				foreach (GCHandle allocation in this.Allocations)
				{
					if (allocation.IsAllocated)
					{
						allocation.Free();
					}
				}
				this.Allocations = null;
				this.PinnedCallback.Free();
				if (this.vTablePtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(this.vTablePtr);
					this.vTablePtr = IntPtr.Zero;
				}
			}
		}

		internal static void DisposeAllClient()
		{
			IDisposable[] array = Event.AllClient.ToArray();
			for (int i = 0; i < (int)array.Length; i++)
			{
				array[i].Dispose();
			}
			Event.AllClient.Clear();
		}

		internal static void DisposeAllServer()
		{
			IDisposable[] array = Event.AllServer.ToArray();
			for (int i = 0; i < (int)array.Length; i++)
			{
				array[i].Dispose();
			}
			Event.AllServer.Clear();
		}

		~Event()
		{
			this.Dispose();
		}

		internal static void Register(Callback.Run func, int size, int callbackId, bool gameserver)
		{
			Event @event = new Event()
			{
				vTablePtr = Event.BuildVTable(func, @event.Allocations)
			};
			Callback callback = new Callback()
			{
				vTablePtr = @event.vTablePtr,
				CallbackFlags = (byte)((gameserver ? 2 : 0)),
				CallbackId = callbackId
			};
			@event.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			SteamClient.RegisterCallback(@event.PinnedCallback.AddrOfPinnedObject(), callback.CallbackId);
			@event.IsAllocated = true;
			if (!gameserver)
			{
				Event.AllClient.Add(@event);
			}
			else
			{
				Event.AllServer.Add(@event);
			}
		}
	}
}