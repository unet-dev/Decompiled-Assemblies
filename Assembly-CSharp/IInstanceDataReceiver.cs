using ProtoBuf;
using System;

public interface IInstanceDataReceiver
{
	void ReceiveInstanceData(ProtoBuf.Item.InstanceData data);
}