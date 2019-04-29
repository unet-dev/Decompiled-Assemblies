using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oxide.Game.Rust.Cui
{
	public class CuiTextComponent : ICuiComponent, ICuiColor
	{
		[DefaultValue(TextAnchor.UpperLeft)]
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty("align")]
		public TextAnchor Align
		{
			get;
			set;
		}

		public string Color { get; set; } = "1.0 1.0 1.0 1.0";

		[JsonProperty("fadeIn")]
		public float FadeIn
		{
			get;
			set;
		}

		[DefaultValue("RobotoCondensed-Bold.ttf")]
		[JsonProperty("font")]
		public string Font { get; set; } = "RobotoCondensed-Bold.ttf";

		[DefaultValue(14)]
		[JsonProperty("fontSize")]
		public int FontSize { get; set; } = 14;

		[DefaultValue("Text")]
		[JsonProperty("text")]
		public string Text { get; set; } = "Text";

		public string Type
		{
			get
			{
				return "UnityEngine.UI.Text";
			}
		}

		public CuiTextComponent()
		{
		}
	}
}