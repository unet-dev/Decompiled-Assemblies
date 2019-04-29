using System;
using System.Runtime.CompilerServices;

namespace Oxide.Game.Rust.Cui
{
	public class CuiPanel
	{
		public bool CursorEnabled
		{
			get;
			set;
		}

		public float FadeOut
		{
			get;
			set;
		}

		public CuiImageComponent Image { get; set; } = new CuiImageComponent();

		public CuiRawImageComponent RawImage
		{
			get;
			set;
		}

		public CuiRectTransformComponent RectTransform { get; } = new CuiRectTransformComponent();

		public CuiPanel()
		{
		}
	}
}