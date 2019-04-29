using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Ambience Definition")]
public class AmbienceDefinition : ScriptableObject
{
	[Header("Sound")]
	public List<SoundDefinition> sounds;

	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange stingFrequency = new AmbienceDefinition.ValueRange(15f, 30f);

	[Header("Environment")]
	[InspectorFlags]
	public TerrainBiome.Enum biomes = TerrainBiome.Enum.Arid | TerrainBiome.Enum.Temperate | TerrainBiome.Enum.Tundra | TerrainBiome.Enum.Arctic;

	[InspectorFlags]
	public TerrainTopology.Enum topologies = TerrainTopology.Enum.Field | TerrainTopology.Enum.Cliff | TerrainTopology.Enum.Summit | TerrainTopology.Enum.Beachside | TerrainTopology.Enum.Beach | TerrainTopology.Enum.Forest | TerrainTopology.Enum.Forestside | TerrainTopology.Enum.Ocean | TerrainTopology.Enum.Oceanside | TerrainTopology.Enum.Decor | TerrainTopology.Enum.Monument | TerrainTopology.Enum.Road | TerrainTopology.Enum.Roadside | TerrainTopology.Enum.Swamp | TerrainTopology.Enum.River | TerrainTopology.Enum.Riverside | TerrainTopology.Enum.Lake | TerrainTopology.Enum.Lakeside | TerrainTopology.Enum.Offshore | TerrainTopology.Enum.Powerline | TerrainTopology.Enum.Runway | TerrainTopology.Enum.Building | TerrainTopology.Enum.Cliffside | TerrainTopology.Enum.Mountain | TerrainTopology.Enum.Clutter | TerrainTopology.Enum.Alt | TerrainTopology.Enum.Tier0 | TerrainTopology.Enum.Tier1 | TerrainTopology.Enum.Tier2 | TerrainTopology.Enum.Mainland | TerrainTopology.Enum.Hilltop;

	public EnvironmentType environmentType = EnvironmentType.Underground;

	public bool useEnvironmentType;

	public AnimationCurve time = AnimationCurve.Linear(0f, 0f, 24f, 0f);

	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange rain = new AmbienceDefinition.ValueRange(0f, 1f);

	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange wind = new AmbienceDefinition.ValueRange(0f, 1f);

	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange snow = new AmbienceDefinition.ValueRange(0f, 1f);

	public AmbienceDefinition()
	{
	}

	[Serializable]
	public class ValueRange
	{
		public float min;

		public float max;

		public ValueRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}