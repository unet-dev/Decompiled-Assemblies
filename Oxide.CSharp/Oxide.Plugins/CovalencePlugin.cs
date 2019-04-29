using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Oxide.Plugins
{
	public class CovalencePlugin : CSharpPlugin
	{
		private readonly static Covalence covalence;

		protected string game = CovalencePlugin.covalence.Game;

		protected IPlayerManager players = CovalencePlugin.covalence.Players;

		protected IServer server = CovalencePlugin.covalence.Server;

		static CovalencePlugin()
		{
			CovalencePlugin.covalence = Interface.Oxide.GetLibrary<Covalence>(null);
		}

		public CovalencePlugin()
		{
		}

		public override void HandleAddedToManager(PluginManager manager)
		{
			PermissionAttribute permissionAttribute;
			string[] permission;
			MethodInfo[] methods = base.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
			for (int i = 0; i < (int)methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				object[] customAttributes = methodInfo.GetCustomAttributes(typeof(CommandAttribute), true);
				object[] objArray = methodInfo.GetCustomAttributes(typeof(PermissionAttribute), true);
				if (customAttributes.Length != 0)
				{
					CommandAttribute commandAttribute = customAttributes[0] as CommandAttribute;
					if (objArray.Length == 0)
					{
						permissionAttribute = null;
					}
					else
					{
						permissionAttribute = objArray[0] as PermissionAttribute;
					}
					PermissionAttribute permissionAttribute1 = permissionAttribute;
					if (commandAttribute != null)
					{
						string[] commands = commandAttribute.Commands;
						if (permissionAttribute1 != null)
						{
							permission = permissionAttribute1.Permission;
						}
						else
						{
							permission = null;
						}
						base.AddCovalenceCommand(commands, permission, (IPlayer caller, string command, string[] args) => {
							base.CallHook(methodInfo.Name, new object[] { caller, command, args });
							return true;
						});
					}
				}
			}
			base.HandleAddedToManager(manager);
		}

		protected void Log(string format, params object[] args)
		{
			OxideMod oxide = Interface.Oxide;
			object[] title = new object[] { base.Title, null };
			title[1] = (args.Length != 0 ? string.Format(format, args) : format);
			oxide.LogInfo("[{0}] {1}", title);
		}

		protected void LogError(string format, params object[] args)
		{
			OxideMod oxide = Interface.Oxide;
			object[] title = new object[] { base.Title, null };
			title[1] = (args.Length != 0 ? string.Format(format, args) : format);
			oxide.LogError("[{0}] {1}", title);
		}

		protected void LogWarning(string format, params object[] args)
		{
			OxideMod oxide = Interface.Oxide;
			object[] title = new object[] { base.Title, null };
			title[1] = (args.Length != 0 ? string.Format(format, args) : format);
			oxide.LogWarning("[{0}] {1}", title);
		}
	}
}