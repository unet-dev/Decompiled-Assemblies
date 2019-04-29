using System;
using System.Runtime.CompilerServices;

namespace Oxide.Game.Rust.Cui
{
	public class CuiLabel
	{
		public float FadeOut
		{
			get;
			set;
		}

		public CuiRectTransformComponent RectTransform { get; } = new CuiRectTransformComponent();

		public CuiTextComponent Text { get; } = new CuiTextComponent();

		public CuiLabel()
		{
		}
	}
}