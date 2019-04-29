using Oxide.Core;
using Oxide.Core.Logging;
using Oxide.Core.Plugins;
using Oxide.Core.Unity;
using Oxide.Core.Unity.Logging;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oxide.Core.Unity.Plugins
{
	public class UnityCore : CSPlugin
	{
		private UnityLogger logger;

		public UnityCore()
		{
			base.set_Title("Unity");
			base.set_Author(UnityExtension.AssemblyAuthors);
			base.set_Version(UnityExtension.AssemblyVersion);
		}

		[HookMethod("InitLogging")]
		private void InitLogging()
		{
			Interface.get_Oxide().NextTick(() => {
				this.logger = new UnityLogger();
				Interface.get_Oxide().get_RootLogger().AddLogger(this.logger);
				Interface.get_Oxide().get_RootLogger().DisableCache();
			});
		}

		public void Print(string message)
		{
			Debug.Log(message);
		}

		public void PrintError(string message)
		{
			Debug.LogError(message);
		}

		public void PrintWarning(string message)
		{
			Debug.LogWarning(message);
		}
	}
}