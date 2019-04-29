using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Oxide.Game.Rust.Cui
{
	public class CuiOutlineComponent : ICuiComponent, ICuiColor
	{
		public string Color { get; set; } = "1.0 1.0 1.0 1.0";

		[DefaultValue("1.0 -1.0")]
		[JsonProperty("distance")]
		public string Distance { get; set; } = "1.0 -1.0";

		public string Type
		{
			get
			{
				return "UnityEngine.UI.Outline";
			}
		}

		[DefaultValue(false)]
		[JsonProperty("useGraphicAlpha")]
		public bool UseGraphicAlpha
		{
			get;
			set;
		}

		public CuiOutlineComponent()
		{
		}
	}
}