using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

namespace Oxide.Game.Rust.Cui
{
	public class CuiButtonComponent : ICuiComponent, ICuiColor
	{
		[JsonProperty("close")]
		public string Close
		{
			get;
			set;
		}

		public string Color { get; set; } = "1.0 1.0 1.0 1.0";

		[JsonProperty("command")]
		public string Command
		{
			get;
			set;
		}

		[JsonProperty("fadeIn")]
		public float FadeIn
		{
			get;
			set;
		}

		[DefaultValue(Image.Type.Simple)]
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty("imagetype")]
		public Image.Type ImageType
		{
			get;
			set;
		}

		[DefaultValue("Assets/Icons/IconMaterial.mat")]
		[JsonProperty("material")]
		public string Material { get; set; } = "Assets/Icons/IconMaterial.mat";

		[DefaultValue("Assets/Content/UI/UI.Background.Tile.psd")]
		[JsonProperty("sprite")]
		public string Sprite { get; set; } = "Assets/Content/UI/UI.Background.Tile.psd";

		public string Type
		{
			get
			{
				return "UnityEngine.UI.Button";
			}
		}

		public CuiButtonComponent()
		{
		}
	}
}