using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Oxide.Game.Rust.Cui
{
	public class CuiRawImageComponent : ICuiComponent, ICuiColor
	{
		public string Color { get; set; } = "1.0 1.0 1.0 1.0";

		[JsonProperty("fadeIn")]
		public float FadeIn
		{
			get;
			set;
		}

		[JsonProperty("material")]
		public string Material
		{
			get;
			set;
		}

		[JsonProperty("png")]
		public string Png
		{
			get;
			set;
		}

		[DefaultValue("Assets/Icons/rust.png")]
		[JsonProperty("sprite")]
		public string Sprite { get; set; } = "Assets/Icons/rust.png";

		public string Type
		{
			get
			{
				return "UnityEngine.UI.RawImage";
			}
		}

		[JsonProperty("url")]
		public string Url
		{
			get;
			set;
		}

		public CuiRawImageComponent()
		{
		}
	}
}