using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	public static class Localization
	{
		public readonly static Dictionary<string, Dictionary<string, string>> languages;

		static Localization()
		{
			Dictionary<string, Dictionary<string, string>> strs = new Dictionary<string, Dictionary<string, string>>();
			Dictionary<string, string> strs1 = new Dictionary<string, string>();
			strs1["CommandUsageExtLoad"] = "Usage: oxide.ext.load <extname>+";
			strs1["CommandUsageExtUnload"] = "Usage: oxide.ext.unload <extname>+";
			strs1["CommandUsageExtReload"] = "Usage: oxide.ext.reload <extname>+";
			strs1["CommandUsageGrant"] = "Usage: oxide.grant <group|user> <name|id> <permission>";
			strs1["CommandUsageGroup"] = "Usage: oxide.group <add|set> <name> [title] [rank]";
			strs1["CommandUsageGroupParent"] = "Usage: oxide.group <parent> <name> <parentName>";
			strs1["CommandUsageGroupRemove"] = "Usage: oxide.group <remove> <name>";
			strs1["CommandUsageLang"] = "Usage: oxide.lang <two-digit language code>";
			strs1["CommandUsageLoad"] = "Usage: oxide.load *|<pluginname>+";
			strs1["CommandUsageReload"] = "Usage: oxide.reload *|<pluginname>+";
			strs1["CommandUsageRevoke"] = "Usage: oxide.revoke <group|user> <name|id> <permission>";
			strs1["CommandUsageShow"] = "Usage: oxide.show <groups|perms>";
			strs1["CommandUsageShowName"] = "Usage: oxide.show <group|user> <name>";
			strs1["CommandUsageUnload"] = "Usage: oxide.unload *|<pluginname>+";
			strs1["CommandUsageUserGroup"] = "Usage: oxide.usergroup <add|remove> <username> <groupname>";
			strs1["ConnectionRejected"] = "Connection was rejected";
			strs1["DataSaved"] = "Saving Oxide data...";
			strs1["GroupAlreadyExists"] = "Group '{0}' already exists";
			strs1["GroupAlreadyHasPermission"] = "Group '{0}' already has permission '{1}'";
			strs1["GroupDoesNotHavePermission"] = "Group '{0}' does not have permission '{1}'";
			strs1["GroupChanged"] = "Group '{0}' changed";
			strs1["GroupCreated"] = "Group '{0}' created";
			strs1["GroupDeleted"] = "Group '{0}' deleted";
			strs1["GroupNotFound"] = "Group '{0}' doesn't exist";
			strs1["GroupParentChanged"] = "Group '{0}' parent changed to '{1}'";
			strs1["GroupParentNotChanged"] = "Group '{0}' parent was not changed";
			strs1["GroupParentNotFound"] = "Group parent '{0}' doesn't exist";
			strs1["GroupPermissionGranted"] = "Group '{0}' granted permission '{1}'";
			strs1["GroupPermissionRevoked"] = "Group '{0}' revoked permission '{1}'";
			strs1["GroupPermissions"] = "Group '{0}' permissions";
			strs1["GroupPlayers"] = "Group '{0}' players";
			strs1["Groups"] = "Groups";
			strs1["NoGroupPermissions"] = "No permissions currently granted";
			strs1["NoPermissionGroups"] = "No groups with this permission";
			strs1["NoPermissionPlayers"] = "No players with this permission";
			strs1["NoPluginsFound"] = "No plugins are currently available";
			strs1["NoPlayerGroups"] = "Player is not assigned to any groups";
			strs1["NoPlayerPermissions"] = "No permissions currently granted";
			strs1["NoPlayersInGroup"] = "No players currently in group";
			strs1["NotAllowed"] = "You are not allowed to use the '{0}' command";
			strs1["ParentGroupPermissions"] = "Parent group '{0}' permissions";
			strs1["PermissionGroups"] = "Permission '{0}' Groups";
			strs1["PermissionPlayers"] = "Permission '{0}' Players";
			strs1["PermissionNotFound"] = "Permission '{0}' doesn't exist";
			strs1["Permissions"] = "Permissions";
			strs1["PermissionsNotLoaded"] = "Unable to load permission files! Permissions will not work until resolved.\n => {0}";
			strs1["PlayerLanguage"] = "Player language set to {0}";
			strs1["PluginNotLoaded"] = "Plugin '{0}' not loaded.";
			strs1["PluginReloaded"] = "Reloaded plugin {0} v{1} by {2}";
			strs1["PluginUnloaded"] = "Unloaded plugin {0} v{1} by {2}";
			strs1["ServerLanguage"] = "Server language set to {0}";
			strs1["Unknown"] = "Unknown";
			strs1["UnknownCommand"] = "Unknown command: {0}";
			strs1["PlayerAddedToGroup"] = "Player '{0}' added to group: {1}";
			strs1["PlayerAlreadyHasPermission"] = "Player '{0}' already has permission '{1}'";
			strs1["PlayerDoesNotHavePermission"] = "Player '{0}' does not have permission '{1}'";
			strs1["PlayerNotFound"] = "Player '{0}' not found";
			strs1["PlayerGroups"] = "Player '{0}' groups";
			strs1["PlayerPermissions"] = "Player '{0}' permissions";
			strs1["PlayerPermissionGranted"] = "Player '{0}' granted permission '{1}'";
			strs1["PlayerPermissionRevoked"] = "Player '{0}' revoked permission '{1}'";
			strs1["PlayerRemovedFromGroup"] = "Player '{0}' removed from group '{1}'";
			strs1["PlayersFound"] = "Multiple players were found, please specify: {0}";
			strs1["Version"] = "Server is running [#ffb658]Oxide {0}[/#] and [#ee715c]{1} {2} ({3})[/#]";
			strs1["YouAreNotAdmin"] = "You are not an admin";
			strs["en"] = strs1;
			Localization.languages = strs;
		}
	}
}