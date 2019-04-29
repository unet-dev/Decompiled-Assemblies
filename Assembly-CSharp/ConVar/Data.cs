using System;
using System.IO;
using UnityEngine;

namespace ConVar
{
	[Factory("data")]
	public class Data : ConsoleSystem
	{
		public Data()
		{
		}

		[ClientVar]
		[ServerVar]
		public static void export(ConsoleSystem.Arg args)
		{
			string str = args.GetString(0, "none");
			string str1 = Path.Combine(Application.persistentDataPath, string.Concat(str, ".raw"));
			if (str == "splatmap")
			{
				if (TerrainMeta.SplatMap)
				{
					RawWriter.Write(TerrainMeta.SplatMap.ToEnumerable(), str1);
				}
			}
			else if (str == "heightmap")
			{
				if (TerrainMeta.HeightMap)
				{
					RawWriter.Write(TerrainMeta.HeightMap.ToEnumerable(), str1);
				}
			}
			else if (str == "biomemap")
			{
				if (TerrainMeta.BiomeMap)
				{
					RawWriter.Write(TerrainMeta.BiomeMap.ToEnumerable(), str1);
				}
			}
			else if (str == "topologymap")
			{
				if (TerrainMeta.TopologyMap)
				{
					RawWriter.Write(TerrainMeta.TopologyMap.ToEnumerable(), str1);
				}
			}
			else if (str != "alphamap")
			{
				if (str != "watermap")
				{
					args.ReplyWith(string.Concat("Unknown export source: ", str));
					return;
				}
				if (TerrainMeta.WaterMap)
				{
					RawWriter.Write(TerrainMeta.WaterMap.ToEnumerable(), str1);
				}
			}
			else if (TerrainMeta.AlphaMap)
			{
				RawWriter.Write(TerrainMeta.AlphaMap.ToEnumerable(), str1);
			}
			args.ReplyWith(string.Concat("Export written to ", str1));
		}
	}
}