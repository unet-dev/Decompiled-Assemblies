using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Oxide.Game.Rust.Cui
{
	public interface ICuiColor
	{
		[DefaultValue("1.0 1.0 1.0 1.0")]
		[JsonProperty("color")]
		string Color
		{
			get;
			set;
		}
	}
}