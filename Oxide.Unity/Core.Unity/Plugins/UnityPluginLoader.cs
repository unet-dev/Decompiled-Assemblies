using Oxide.Core.Plugins;
using System;

namespace Oxide.Core.Unity.Plugins
{
	public class UnityPluginLoader : PluginLoader
	{
		public override Type[] CorePlugins
		{
			get
			{
				return new Type[] { typeof(UnityCore) };
			}
		}

		public UnityPluginLoader()
		{
		}
	}
}