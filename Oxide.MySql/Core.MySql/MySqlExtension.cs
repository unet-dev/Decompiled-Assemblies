using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Core.MySql.Libraries;
using System;
using System.Reflection;

namespace Oxide.Core.MySql
{
	public class MySqlExtension : Extension
	{
		internal static System.Reflection.Assembly Assembly;

		internal static System.Reflection.AssemblyName AssemblyName;

		internal static VersionNumber AssemblyVersion;

		internal static string AssemblyAuthors;

		public override string Author
		{
			get
			{
				return MySqlExtension.AssemblyAuthors;
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
				return "MySql";
			}
		}

		public override VersionNumber Version
		{
			get
			{
				return MySqlExtension.AssemblyVersion;
			}
		}

		static MySqlExtension()
		{
			MySqlExtension.Assembly = System.Reflection.Assembly.GetExecutingAssembly();
			MySqlExtension.AssemblyName = MySqlExtension.Assembly.GetName();
			MySqlExtension.AssemblyVersion = new VersionNumber(MySqlExtension.AssemblyName.Version.Major, MySqlExtension.AssemblyName.Version.Minor, MySqlExtension.AssemblyName.Version.Build);
			MySqlExtension.AssemblyAuthors = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(MySqlExtension.Assembly, typeof(AssemblyCompanyAttribute), false)).Company;
		}

		public MySqlExtension(ExtensionManager manager) : base(manager)
		{
		}

		public override void Load()
		{
			base.get_Manager().RegisterLibrary("MySql", new Oxide.Core.MySql.Libraries.MySql());
		}

		public override void LoadPluginWatchers(string pluginDirectory)
		{
		}

		public override void OnModLoad()
		{
		}
	}
}