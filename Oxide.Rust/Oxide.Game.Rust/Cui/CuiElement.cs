using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Oxide.Game.Rust.Cui
{
	public class CuiElement
	{
		[JsonProperty("components")]
		public List<ICuiComponent> Components { get; } = new List<ICuiComponent>();

		[JsonProperty("fadeOut")]
		public float FadeOut
		{
			get;
			set;
		}

		[DefaultValue("AddUI CreatedPanel")]
		[JsonProperty("name")]
		public string Name { get; set; } = "AddUI CreatedPanel";

		[JsonProperty("parent")]
		public string Parent { get; set; } = "Hud";

		public CuiElement()
		{
		}
	}
}