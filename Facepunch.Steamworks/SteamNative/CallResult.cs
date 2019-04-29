using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;

namespace SteamNative
{
	internal abstract class CallResult : CallbackHandle
	{
		internal SteamAPICall_t Call;

		public override bool IsValid
		{
			get
			{
				return this.Call != (long)0;
			}
		}

		internal CallResult(BaseSteamworks steamworks, SteamAPICall_t call) : base(steamworks)
		{
			this.Call = call;
		}

		internal abstract void RunCallback();

		internal void Try()
		{
			bool flag = false;
			if (!this.Steamworks.native.utils.IsAPICallCompleted(this.Call, ref flag))
			{
				return;
			}
			this.Steamworks.UnregisterCallResult(this);
			this.RunCallback();
		}
	}
}