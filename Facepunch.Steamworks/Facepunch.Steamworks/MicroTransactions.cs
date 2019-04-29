using SteamNative;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Facepunch.Steamworks
{
	public class MicroTransactions : IDisposable
	{
		internal Client client;

		internal MicroTransactions(Client c)
		{
			this.client = c;
			this.client.RegisterCallback<MicroTxnAuthorizationResponse_t>(new Action<MicroTxnAuthorizationResponse_t>(this.onMicroTxnAuthorizationResponse));
		}

		public void Dispose()
		{
			this.client = null;
		}

		private void onMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t arg1)
		{
			if (this.OnAuthorizationResponse != null)
			{
				this.OnAuthorizationResponse(arg1.Authorized == 1, arg1.AppID, arg1.OrderID);
			}
		}

		public event MicroTransactions.AuthorizationResponse OnAuthorizationResponse;

		public delegate void AuthorizationResponse(bool authorized, int appId, ulong orderId);
	}
}