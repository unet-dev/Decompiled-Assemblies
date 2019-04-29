using Network;
using Network.Visibility;
using System;
using UnityEngine;

public static class EffectNetwork
{
	public static void Send(Effect effect)
	{
		if (Net.sv == null)
		{
			return;
		}
		if (!Net.sv.IsConnected())
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("EffectNetwork.Send", 0.1f))
		{
			Group group = null;
			if (!string.IsNullOrEmpty(effect.pooledString))
			{
				effect.pooledstringid = StringPool.Get(effect.pooledString);
			}
			if (effect.pooledstringid == 0)
			{
				Debug.Log(string.Concat("String ID is 0 - unknown effect ", effect.pooledString));
			}
			else if (!effect.broadcast)
			{
				if (effect.entity <= 0)
				{
					group = Net.sv.visibility.GetGroup(effect.worldPos);
				}
				else
				{
					BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(effect.entity) as BaseEntity;
					if (baseEntity.IsValid())
					{
						group = baseEntity.net.@group;
					}
					else
					{
						return;
					}
				}
				if (group != null)
				{
					Net.sv.write.Start();
					Net.sv.write.PacketID(Message.Type.Effect);
					effect.WriteToStream(Net.sv.write);
					Net.sv.write.Send(new SendInfo(group.subscribers));
				}
				else
				{
					return;
				}
			}
			else if (Net.sv.write.Start())
			{
				Net.sv.write.PacketID(Message.Type.Effect);
				effect.WriteToStream(Net.sv.write);
				Net.sv.write.Send(new SendInfo(BaseNetworkable.GlobalNetworkGroup.subscribers));
				return;
			}
		}
	}

	public static void Send(Effect effect, Connection target)
	{
		effect.pooledstringid = StringPool.Get(effect.pooledString);
		if (effect.pooledstringid == 0)
		{
			Debug.LogWarning(string.Concat("EffectNetwork.Send - unpooled effect name: ", effect.pooledString));
			return;
		}
		Net.sv.write.Start();
		Net.sv.write.PacketID(Message.Type.Effect);
		effect.WriteToStream(Net.sv.write);
		Net.sv.write.Send(new SendInfo(target));
	}
}