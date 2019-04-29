using System;

namespace Rust
{
	[Flags]
	public enum AmmoTypes
	{
		PISTOL_9MM = 1,
		RIFLE_556MM = 2,
		SHOTGUN_12GUAGE = 4,
		BOW_ARROW = 8,
		HANDMADE_SHELL = 16,
		ROCKET = 32,
		NAILS = 64,
		AMMO_40MM = 128
	}
}