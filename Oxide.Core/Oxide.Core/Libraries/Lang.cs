using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Plugins;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Libraries
{
	public class Lang : Library
	{
		private const string defaultLang = "en";

		private readonly Lang.LangData langData;

		private readonly Dictionary<string, Dictionary<string, string>> langFiles;

		private readonly Dictionary<Plugin, Event.Callback<Plugin, PluginManager>> pluginRemovedFromManager;

		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		public Lang()
		{
			this.langFiles = new Dictionary<string, Dictionary<string, string>>();
			this.langData = ProtoStorage.Load<Lang.LangData>(new string[] { "oxide.lang" }) ?? new Lang.LangData();
			this.pluginRemovedFromManager = new Dictionary<Plugin, Event.Callback<Plugin, PluginManager>>();
		}

		private void AddLangFile(string file, Dictionary<string, string> langFile, Plugin plugin)
		{
			this.langFiles.Add(file, langFile);
			if (plugin != null && !this.pluginRemovedFromManager.ContainsKey(plugin))
			{
				this.pluginRemovedFromManager[plugin] = plugin.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.plugin_OnRemovedFromManager));
			}
		}

		[LibraryFunction("GetLanguage")]
		public string GetLanguage(string userId)
		{
			string str;
			if (!string.IsNullOrEmpty(userId) && this.langData.UserData.TryGetValue(userId, out str))
			{
				return str;
			}
			return this.langData.Lang;
		}

		[LibraryFunction("GetLanguages")]
		public string[] GetLanguages(Plugin plugin = null)
		{
			List<string> strs = new List<string>();
			string[] directories = Directory.GetDirectories(Interface.Oxide.LangDirectory);
			for (int i = 0; i < (int)directories.Length; i++)
			{
				string str = directories[i];
				if (Directory.GetFiles(str).Length != 0 && (plugin == null || plugin != null && File.Exists(Path.Combine(str, string.Concat(plugin.Name, ".json")))))
				{
					strs.Add(str.Substring(Interface.Oxide.LangDirectory.Length + 1));
				}
			}
			return strs.ToArray();
		}

		[LibraryFunction("GetMessage")]
		public string GetMessage(string key, Plugin plugin, string userId = null)
		{
			if (string.IsNullOrEmpty(key) || plugin == null)
			{
				return key;
			}
			return this.GetMessageKey(key, plugin, this.GetLanguage(userId));
		}

		private Dictionary<string, string> GetMessageFile(string plugin, string lang = "en")
		{
			if (string.IsNullOrEmpty(plugin))
			{
				return null;
			}
			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			for (int i = 0; i < (int)invalidFileNameChars.Length; i++)
			{
				lang = lang.Replace(invalidFileNameChars[i], '\u005F');
			}
			string str = string.Format("{0}{1}{2}.json", lang, Path.DirectorySeparatorChar, plugin);
			string str1 = Path.Combine(Interface.Oxide.LangDirectory, str);
			if (!File.Exists(str1))
			{
				return null;
			}
			return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(str1));
		}

		private string GetMessageKey(string key, Plugin plugin, string lang = "en")
		{
			Dictionary<string, string> messageFile;
			string str;
			string str1 = string.Format("{0}{1}{2}.json", lang, Path.DirectorySeparatorChar, plugin.Name);
			if (!this.langFiles.TryGetValue(str1, out messageFile))
			{
				messageFile = this.GetMessageFile(plugin.Name, lang) ?? (this.GetMessageFile(plugin.Name, this.langData.Lang) ?? this.GetMessageFile(plugin.Name, "en"));
				if (messageFile == null)
				{
					Interface.Oxide.LogWarning(string.Concat("Plugin '", plugin.Name, "' is using the Lang API but has no messages registered"), Array.Empty<object>());
					return key;
				}
				Dictionary<string, string> strs = this.GetMessageFile(plugin.Name, "en");
				if (strs != null && this.MergeMessages(messageFile, strs) && File.Exists(Path.Combine(Interface.Oxide.LangDirectory, str1)))
				{
					File.WriteAllText(Path.Combine(Interface.Oxide.LangDirectory, str1), JsonConvert.SerializeObject(messageFile, Formatting.Indented));
				}
				this.AddLangFile(str1, messageFile, plugin);
			}
			if (!messageFile.TryGetValue(key, out str))
			{
				return key;
			}
			return str;
		}

		[LibraryFunction("GetMessages")]
		public Dictionary<string, string> GetMessages(string lang, Plugin plugin)
		{
			Dictionary<string, string> messageFile;
			if (string.IsNullOrEmpty(lang) || plugin == null)
			{
				return null;
			}
			string str = string.Format("{0}{1}{2}.json", lang, Path.DirectorySeparatorChar, plugin.Name);
			if (!this.langFiles.TryGetValue(str, out messageFile))
			{
				messageFile = this.GetMessageFile(plugin.Name, lang);
				if (messageFile == null)
				{
					return null;
				}
				this.AddLangFile(str, messageFile, plugin);
			}
			return messageFile.ToDictionary<KeyValuePair<string, string>, string, string>((KeyValuePair<string, string> k) => k.Key, (KeyValuePair<string, string> v) => v.Value);
		}

		[LibraryFunction("GetServerLanguage")]
		public string GetServerLanguage()
		{
			return this.langData.Lang;
		}

		private bool MergeMessages(Dictionary<string, string> existingMessages, Dictionary<string, string> messages)
		{
			bool flag = false;
			foreach (KeyValuePair<string, string> message in messages)
			{
				if (existingMessages.ContainsKey(message.Key))
				{
					continue;
				}
				existingMessages.Add(message.Key, message.Value);
				flag = true;
			}
			if (existingMessages.Count > 0)
			{
				string[] array = existingMessages.Keys.ToArray<string>();
				for (int i = 0; i < (int)array.Length; i++)
				{
					string str = array[i];
					if (!messages.ContainsKey(str))
					{
						existingMessages.Remove(str);
						flag = true;
					}
				}
			}
			return flag;
		}

		private void plugin_OnRemovedFromManager(Plugin sender, PluginManager manager)
		{
			Event.Callback<Plugin, PluginManager> callback;
			if (this.pluginRemovedFromManager.TryGetValue(sender, out callback))
			{
				callback.Remove();
				this.pluginRemovedFromManager.Remove(sender);
			}
			string[] languages = this.GetLanguages(sender);
			for (int i = 0; i < (int)languages.Length; i++)
			{
				string str = languages[i];
				this.langFiles.Remove(string.Format("{0}{1}{2}.json", str, Path.DirectorySeparatorChar, sender.Name));
			}
		}

		[LibraryFunction("RegisterMessages")]
		public void RegisterMessages(Dictionary<string, string> messages, Plugin plugin, string lang = "en")
		{
			bool flag;
			if (messages == null || string.IsNullOrEmpty(lang) || plugin == null)
			{
				return;
			}
			string str = string.Format("{0}{1}{2}.json", lang, Path.DirectorySeparatorChar, plugin.Name);
			Dictionary<string, string> messageFile = this.GetMessageFile(plugin.Name, lang);
			if (messageFile != null)
			{
				flag = this.MergeMessages(messageFile, messages);
				messages = messageFile;
			}
			else
			{
				this.langFiles.Remove(str);
				this.AddLangFile(str, messages, plugin);
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			if (!Directory.Exists(Path.Combine(Interface.Oxide.LangDirectory, lang)))
			{
				Directory.CreateDirectory(Path.Combine(Interface.Oxide.LangDirectory, lang));
			}
			File.WriteAllText(Path.Combine(Interface.Oxide.LangDirectory, str), JsonConvert.SerializeObject(messages, Formatting.Indented));
		}

		private void SaveData()
		{
			ProtoStorage.Save<Lang.LangData>(this.langData, new string[] { "oxide.lang" });
		}

		[LibraryFunction("SetLanguage")]
		public void SetLanguage(string lang, string userId)
		{
			string str;
			if (string.IsNullOrEmpty(lang) || string.IsNullOrEmpty(userId))
			{
				return;
			}
			if (this.langData.UserData.TryGetValue(userId, out str) && lang.Equals(str))
			{
				return;
			}
			this.langData.UserData[userId] = lang;
			this.SaveData();
		}

		[LibraryFunction("SetServerLanguage")]
		public void SetServerLanguage(string lang)
		{
			if (string.IsNullOrEmpty(lang) || lang.Equals(this.langData.Lang))
			{
				return;
			}
			this.langData.Lang = lang;
			this.SaveData();
		}

		[ProtoContract(ImplicitFields=ImplicitFields.AllFields)]
		private class LangData
		{
			public string Lang;

			public readonly Dictionary<string, string> UserData;

			public LangData()
			{
			}
		}
	}
}