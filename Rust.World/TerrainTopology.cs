using System;

public static class TerrainTopology
{
	public const int COUNT = 31;

	public const int EVERYTHING = -1;

	public const int NOTHING = 0;

	public const int FIELD = 1;

	public const int CLIFF = 2;

	public const int SUMMIT = 4;

	public const int BEACHSIDE = 8;

	public const int BEACH = 16;

	public const int FOREST = 32;

	public const int FORESTSIDE = 64;

	public const int OCEAN = 128;

	public const int OCEANSIDE = 256;

	public const int DECOR = 512;

	public const int MONUMENT = 1024;

	public const int ROAD = 2048;

	public const int ROADSIDE = 4096;

	public const int SWAMP = 8192;

	public const int RIVER = 16384;

	public const int RIVERSIDE = 32768;

	public const int LAKE = 65536;

	public const int LAKESIDE = 131072;

	public const int OFFSHORE = 262144;

	public const int POWERLINE = 524288;

	public const int RUNWAY = 1048576;

	public const int BUILDING = 2097152;

	public const int CLIFFSIDE = 4194304;

	public const int MOUNTAIN = 8388608;

	public const int CLUTTER = 16777216;

	public const int ALT = 33554432;

	public const int TIER0 = 67108864;

	public const int TIER1 = 134217728;

	public const int TIER2 = 268435456;

	public const int MAINLAND = 536870912;

	public const int HILLTOP = 1073741824;

	public const int WATER = 82048;

	public const int WATERSIDE = 164096;

	public const int SAND = 197016;

	public enum Enum
	{
		Field = 1,
		Cliff = 2,
		Summit = 4,
		Beachside = 8,
		Beach = 16,
		Forest = 32,
		Forestside = 64,
		Ocean = 128,
		Oceanside = 256,
		Decor = 512,
		Monument = 1024,
		Road = 2048,
		Roadside = 4096,
		Swamp = 8192,
		River = 16384,
		Riverside = 32768,
		Lake = 65536,
		Lakeside = 131072,
		Offshore = 262144,
		Powerline = 524288,
		Runway = 1048576,
		Building = 2097152,
		Cliffside = 4194304,
		Mountain = 8388608,
		Clutter = 16777216,
		Alt = 33554432,
		Tier0 = 67108864,
		Tier1 = 134217728,
		Tier2 = 268435456,
		Mainland = 536870912,
		Hilltop = 1073741824
	}
}