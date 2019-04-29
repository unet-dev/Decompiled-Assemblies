using Network;
using System;

public class CommunityEntity : PointEntity
{
	public static CommunityEntity ServerInstance;

	public static CommunityEntity ClientInstance;

	static CommunityEntity()
	{
	}

	public CommunityEntity()
	{
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			CommunityEntity.ServerInstance = null;
			return;
		}
		CommunityEntity.ClientInstance = null;
	}

	public override void InitShared()
	{
		if (!base.isServer)
		{
			CommunityEntity.ClientInstance = this;
		}
		else
		{
			CommunityEntity.ServerInstance = this;
		}
		base.InitShared();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("CommunityEntity.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}
}