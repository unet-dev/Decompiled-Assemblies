using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Core.Logging;
using Oxide.Core.Unity;
using Oxide.Game.Rust.Libraries;
using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Oxide.Game.Rust
{
	public class RustExtension : Extension
	{
		internal static System.Reflection.Assembly Assembly;

		internal static System.Reflection.AssemblyName AssemblyName;

		internal static VersionNumber AssemblyVersion;

		internal static string AssemblyAuthors;

		public static string[] Filter;

		public override string Author
		{
			get
			{
				return RustExtension.AssemblyAuthors;
			}
		}

		public override string Branch
		{
			get
			{
				return "public";
			}
		}

		public override string[] DefaultReferences
		{
			get
			{
				return new String[] { "ApexAI", "ApexShared", "Facepunch.Network", "Facepunch.Steamworks", "Facepunch.System", "Facepunch.UnityEngine", "NewAssembly", "Rust.Data", "Rust.Global", "Rust.Workshop", "Rust.World", "System.Drawing", "UnityEngine.AIModule", "UnityEngine.AssetBundleModule", "UnityEngine.CoreModule", "UnityEngine.GridModule", "UnityEngine.ImageConversionModule", "UnityEngine.Networking", "UnityEngine.PhysicsModule", "UnityEngine.TerrainModule", "UnityEngine.TerrainPhysicsModule", "UnityEngine.UI", "UnityEngine.UIModule", "UnityEngine.UIElementsModule", "UnityEngine.UnityWebRequestAudioModule", "UnityEngine.UnityWebRequestModule", "UnityEngine.UnityWebRequestTextureModule", "UnityEngine.UnityWebRequestWWWModule", "UnityEngine.VehiclesModule", "UnityEngine.WebModule" };
			}
		}

		public override bool IsGameExtension
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
				return "Rust";
			}
		}

		public override VersionNumber Version
		{
			get
			{
				return RustExtension.AssemblyVersion;
			}
		}

		public override string[] WhitelistAssemblies
		{
			get
			{
				return new String[] { "Assembly-CSharp", "Assembly-CSharp-firstpass", "DestMath", "Facepunch.Network", "Facepunch.System", "Facepunch.UnityEngine", "mscorlib", "Oxide.Core", "Oxide.Rust", "RustBuild", "Rust.Data", "Rust.Global", "System", "System.Core", "UnityEngine" };
			}
		}

		public override string[] WhitelistNamespaces
		{
			get
			{
				return new String[] { "ConVar", "Dest", "Facepunch", "Network", "Oxide.Game.Rust.Cui", "ProtoBuf", "PVT", "Rust", "Steamworks", "System.Collections", "System.Security.Cryptography", "System.Text", "UnityEngine" };
			}
		}

		static RustExtension()
		{
			RustExtension.Assembly = System.Reflection.Assembly.GetExecutingAssembly();
			RustExtension.AssemblyName = RustExtension.Assembly.GetName();
			RustExtension.AssemblyVersion = new VersionNumber(RustExtension.AssemblyName.Version.Major, RustExtension.AssemblyName.Version.Minor, RustExtension.AssemblyName.Version.Build);
			RustExtension.AssemblyAuthors = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(RustExtension.Assembly, typeof(AssemblyCompanyAttribute), false)).Company;
			RustExtension.Filter = new String[] { "alphamapResolution is clamped to the range of", "AngryAnt Behave version", "Floating point textures aren't supported on this device", "HDR RenderTexture format is not supported on this platform.", "Image Effects are not supported on this platform.", "Missing projectileID", "Motion vectors not supported on a platform that does not support", "The image effect Main Camera", "The image effect effect -", "Unable to find shaders", "Unsupported encoding: 'utf8'", "Warning, null renderer for ScaleRenderer!", "[AmplifyColor]", "[AmplifyOcclusion]", "[CoverageQueries] Disabled due to unsupported", "[CustomProbe]", "[Manifest] URI IS", "[SpawnHandler] populationCounts" };
		}

		public RustExtension(ExtensionManager manager) : base(manager)
		{
		}

		private static void HandleLog(string message, string stackTrace, UnityEngine.LogType logType)
		{
			if (!String.IsNullOrEmpty(message) && !RustExtension.Filter.Any<string>(new Func<string, bool>(message.Contains)))
			{
				Interface.Oxide.RootLogger.HandleMessage(message, stackTrace, logType.ToLogType());
			}
		}

		public override void Load()
		{
			base.Manager.RegisterLibrary("Rust", new Oxide.Game.Rust.Libraries.Rust());
			base.Manager.RegisterLibrary("Command", new Command());
			base.Manager.RegisterLibrary("Item", new Item());
			base.Manager.RegisterLibrary("Player", new Player());
			base.Manager.RegisterLibrary("Server", new Server());
			base.Manager.RegisterPluginLoader(new RustPluginLoader());
		}

		public override void LoadPluginWatchers(string directory)
		{
		}

		public override void OnModLoad()
		{
			CSharpPluginLoader.PluginReferences.UnionWith(this.DefaultReferences);
		}
	}
}