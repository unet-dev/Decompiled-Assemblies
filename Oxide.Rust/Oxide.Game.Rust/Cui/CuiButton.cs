using System;
using System.Runtime.CompilerServices;

namespace Oxide.Game.Rust.Cui
{
	public class CuiButton
	{
		public CuiButtonComponent Button { get; } = new CuiButtonComponent();

		public float FadeOut
		{
			get;
			set;
		}

		public CuiRectTransformComponent RectTransform { get; } = new CuiRectTransformComponent();

		public CuiTextComponent Text { get; } = new CuiTextComponent();

		public CuiButton()
		{
		}
	}
}