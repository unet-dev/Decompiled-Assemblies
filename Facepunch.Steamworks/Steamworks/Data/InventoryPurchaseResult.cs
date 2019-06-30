using Steamworks;
using System;

namespace Steamworks.Data
{
	public struct InventoryPurchaseResult
	{
		public Steamworks.Result Result;

		public ulong OrderID;

		public ulong TransID;
	}
}