using Network;
using System;

public class EntityComponentBase : BaseMonoBehaviour
{
	public EntityComponentBase()
	{
	}

	protected virtual BaseEntity GetBaseEntity()
	{
		return null;
	}

	public virtual bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}
}