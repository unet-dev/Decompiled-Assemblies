using ProtoBuf;
using System;
using System.Collections.Generic;

public class VendingMachineMapMarker : MapMarker
{
	public string markerShopName;

	public VendingMachine server_vendingMachine;

	public ProtoBuf.VendingMachine.SellOrderContainer client_sellOrders;

	public VendingMachineMapMarker()
	{
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vendingMachine = new ProtoBuf.VendingMachine()
		{
			shopName = this.markerShopName
		};
		if (this.server_vendingMachine != null)
		{
			info.msg.vendingMachine.sellOrderContainer = new ProtoBuf.VendingMachine.SellOrderContainer()
			{
				ShouldPool = false,
				sellOrders = new List<ProtoBuf.VendingMachine.SellOrder>()
			};
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.server_vendingMachine.sellOrders.sellOrders)
			{
				ProtoBuf.VendingMachine.SellOrder sellOrder1 = new ProtoBuf.VendingMachine.SellOrder()
				{
					ShouldPool = false
				};
				sellOrder.CopyTo(sellOrder1);
				info.msg.vendingMachine.sellOrderContainer.sellOrders.Add(sellOrder1);
			}
		}
	}
}