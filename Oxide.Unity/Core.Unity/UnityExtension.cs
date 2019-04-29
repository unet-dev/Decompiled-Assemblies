using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Core.Unity.Plugins;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oxide.Core.Unity
{
	public class UnityExtension : Extension
	{
		internal static System.Reflection.Assembly Assembly;

		internal static System.Reflection.AssemblyName AssemblyName;

		internal static VersionNumber AssemblyVersion;

		internal static string AssemblyAuthors;

		public override string Author
		{
			get
			{
				return UnityExtension.AssemblyAuthors;
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
				return "Unity";
			}
		}

		public override VersionNumber Version
		{
			get
			{
				return UnityExtension.AssemblyVersion;
			}
		}

		static UnityExtension()
		{
			UnityExtension.Assembly = System.Reflection.Assembly.GetExecutingAssembly();
			UnityExtension.AssemblyName = UnityExtension.Assembly.GetName();
			UnityExtension.AssemblyVersion = new VersionNumber(UnityExtension.AssemblyName.Version.Major, UnityExtension.AssemblyName.Version.Minor, UnityExtension.AssemblyName.Version.Build);
			UnityExtension.AssemblyAuthors = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(UnityExtension.Assembly, typeof(AssemblyCompanyAttribute), false)).Company;
		}

		public UnityExtension(ExtensionManager manager) : base(manager)
		{
		}

		public override void Load()
		{
			base.get_Manager().RegisterPluginLoader(new UnityPluginLoader());
			Interface.get_Oxide().RegisterEngineClock(() => Time.realtimeSinceStartup);
			UnityScript.Create();
		}

		public override void LoadPluginWatchers(string pluginDirectory)
		{
		}

		public override void OnModLoad()
		{
		}
	}
}