using System;

public class JunkPileWaterSpawner : SpawnGroup
{
	public BaseEntity attachToParent;

	public JunkPileWaterSpawner()
	{
	}

	protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
		base.PostSpawnProcess(entity, spawnPoint);
		if (this.attachToParent != null)
		{
			entity.SetParent(this.attachToParent, true, false);
		}
	}
}