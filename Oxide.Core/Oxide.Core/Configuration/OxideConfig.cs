using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Configuration
{
	public class OxideConfig : ConfigFile
	{
		[JsonProperty(PropertyName="OxideConsole")]
		public OxideConfig.OxideConsole Console
		{
			get;
			set;
		}

		public OxideConfig.OxideOptions Options
		{
			get;
			set;
		}

		[JsonProperty(PropertyName="OxideRcon")]
		public OxideConfig.OxideRcon Rcon
		{
			get;
			set;
		}

		public OxideConfig(string filename) : base(filename)
		{
			this.Options = new OxideConfig.OxideOptions()
			{
				Modded = true,
				PluginWatchers = true,
				DefaultGroups = new OxideConfig.DefaultGroups()
				{
					Administrators = "admin",
					Players = "default"
				}
			};
			this.Console = new OxideConfig.OxideConsole()
			{
				Enabled = true,
				MinimalistMode = true,
				ShowStatusBar = true
			};
			this.Rcon = new OxideConfig.OxideRcon()
			{
				Enabled = false,
				ChatPrefix = "[Server Console]",
				Port = 25580,
				Password = string.Empty
			};
		}

		[JsonObject]
		public class DefaultGroups : IEnumerable<string>, IEnumerable
		{
			public string Players;

			public string Administrators;

			public DefaultGroups()
			{
			}

			public IEnumerator<string> GetEnumerator()
			{
				OxideConfig.DefaultGroups defaultGroups = null;
				yield return defaultGroups.Players;
				yield return defaultGroups.Administrators;
			}

			IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}

		public class OxideConsole
		{
			public bool Enabled
			{
				get;
				set;
			}

			public bool MinimalistMode
			{
				get;
				set;
			}

			public bool ShowStatusBar
			{
				get;
				set;
			}

			public OxideConsole()
			{
			}
		}

		public class OxideOptions
		{
			public bool Modded;

			public bool PluginWatchers;

			public OxideConfig.DefaultGroups DefaultGroups;

			public OxideOptions()
			{
			}
		}

		public class OxideRcon
		{
			public string ChatPrefix
			{
				get;
				set;
			}

			public bool Enabled
			{
				get;
				set;
			}

			public string Password
			{
				get;
				set;
			}

			public int Port
			{
				get;
				set;
			}

			public OxideRcon()
			{
			}
		}
	}
}