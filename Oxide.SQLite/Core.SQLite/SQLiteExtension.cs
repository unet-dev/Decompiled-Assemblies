using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Core.SQLite.Libraries;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Oxide.Core.SQLite
{
	public class SQLiteExtension : Extension
	{
		internal static System.Reflection.Assembly Assembly;

		internal static System.Reflection.AssemblyName AssemblyName;

		internal static VersionNumber AssemblyVersion;

		internal static string AssemblyAuthors;

		public override string Author
		{
			get
			{
				return SQLiteExtension.AssemblyAuthors;
			}
		}

		public override bool IsCoreExtension
		{
			get
			{
				return true;
			}
		}

		public override string Name
		{
			get
			{
				return "SQLite";
			}
		}

		public override VersionNumber Version
		{
			get
			{
				return SQLiteExtension.AssemblyVersion;
			}
		}

		static SQLiteExtension()
		{
			SQLiteExtension.Assembly = System.Reflection.Assembly.GetExecutingAssembly();
			SQLiteExtension.AssemblyName = SQLiteExtension.Assembly.GetName();
			SQLiteExtension.AssemblyVersion = new VersionNumber(SQLiteExtension.AssemblyName.Version.Major, SQLiteExtension.AssemblyName.Version.Minor, SQLiteExtension.AssemblyName.Version.Build);
			SQLiteExtension.AssemblyAuthors = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(SQLiteExtension.Assembly, typeof(AssemblyCompanyAttribute), false)).Company;
		}

		public SQLiteExtension(ExtensionManager manager) : base(manager)
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				string extensionDirectory = Interface.get_Oxide().get_ExtensionDirectory();
				string str = Path.Combine(extensionDirectory, "System.Data.SQLite.dll.config");
				if (File.Exists(str))
				{
					if (!(new string[] { "target=\"x64", "target=\"./x64" }).Any<string>(new Func<string, bool>(File.ReadAllText(str).Contains)))
					{
						return;
					}
				}
				File.WriteAllText(str, string.Concat(string.Format("<configuration>\n<dllmap dll=\"sqlite3\" target=\"{0}/x86/libsqlite3.so\" os=\"!windows,osx\" cpu=\"x86\" />\n", extensionDirectory), string.Format("<dllmap dll=\"sqlite3\" target=\"{0}/x64/libsqlite3.so\" os=\"!windows,osx\" cpu=\"x86-64\" />\n</configuration>", extensionDirectory)));
			}
		}

		public override void Load()
		{
			base.get_Manager().RegisterLibrary("SQLite", new Oxide.Core.SQLite.Libraries.SQLite());
		}

		public override void LoadPluginWatchers(string pluginDirectory)
		{
		}

		public override void OnModLoad()
		{
		}
	}
}