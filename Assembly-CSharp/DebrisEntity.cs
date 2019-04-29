using ConVar;
using System;

public class DebrisEntity : BaseCombatEntity
{
	public DebrisEntity()
	{
	}

	public override string Categorize()
	{
		return "debris";
	}

	public float GetRemovalTime()
	{
		return Server.debrisdespawn;
	}

	public void RemoveCorpse()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public void ResetRemovalTime(float dur)
	{
		using (TimeWarning timeWarning = TimeWarning.New("ResetRemovalTime", 0.1f))
		{
			if (base.IsInvoking(new Action(this.RemoveCorpse)))
			{
				base.CancelInvoke(new Action(this.RemoveCorpse));
			}
			base.Invoke(new Action(this.RemoveCorpse), dur);
		}
	}

	public void ResetRemovalTime()
	{
		this.ResetRemovalTime(this.GetRemovalTime());
	}

	public override void ServerInit()
	{
		this.ResetRemovalTime();
		base.ServerInit();
	}
}