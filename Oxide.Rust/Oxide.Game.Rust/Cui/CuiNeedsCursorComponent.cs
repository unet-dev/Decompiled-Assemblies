using System;

namespace Oxide.Game.Rust.Cui
{
	public class CuiNeedsCursorComponent : ICuiComponent
	{
		public string Type
		{
			get
			{
				return "NeedsCursor";
			}
		}

		public CuiNeedsCursorComponent()
		{
		}
	}
}