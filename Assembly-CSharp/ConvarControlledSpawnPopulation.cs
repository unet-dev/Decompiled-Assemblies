using System;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(menuName="Rust/Convar Controlled Spawn Population")]
public class ConvarControlledSpawnPopulation : SpawnPopulation
{
	[Header("Convars")]
	public string PopulationConvar;

	private ConsoleSystem.Command _command;

	protected ConsoleSystem.Command Command
	{
		get
		{
			if (this._command == null)
			{
				this._command = ConsoleSystem.Index.Server.Find(this.PopulationConvar);
				Assert.IsNotNull<ConsoleSystem.Command>(this._command, string.Format("{0} has missing convar {1}", this, this.PopulationConvar));
			}
			return this._command;
		}
	}

	public override float TargetDensity
	{
		get
		{
			return this.Command.AsFloat;
		}
	}

	public ConvarControlledSpawnPopulation()
	{
	}
}