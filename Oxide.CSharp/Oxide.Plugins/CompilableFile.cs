using Oxide.Core;
using Oxide.Core.Libraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Oxide.Plugins
{
	public class CompilableFile
	{
		private static Oxide.Core.Libraries.Timer timer;

		private static object compileLock;

		public CSharpExtension Extension;

		public CSharpPluginLoader Loader;

		public string Name;

		public string Directory;

		public string ScriptName;

		public string ScriptPath;

		public string[] ScriptLines;

		public Encoding ScriptEncoding;

		public HashSet<string> Requires = new HashSet<string>();

		public HashSet<string> References = new HashSet<string>();

		public HashSet<string> IncludePaths = new HashSet<string>();

		public string CompilerErrors;

		public Oxide.Plugins.CompiledAssembly CompiledAssembly;

		public DateTime LastModifiedAt;

		public DateTime LastCachedScriptAt;

		public DateTime LastCompiledAt;

		public bool IsCompilationNeeded;

		protected Action<CSharpPlugin> LoadCallback;

		protected Action<bool> CompileCallback;

		protected float CompilationQueuedAt;

		private Oxide.Core.Libraries.Timer.TimerInstance timeoutTimer;

		public byte[] ScriptSource
		{
			get
			{
				return this.ScriptEncoding.GetBytes(string.Join(Environment.NewLine, this.ScriptLines));
			}
		}

		static CompilableFile()
		{
			CompilableFile.timer = Interface.Oxide.GetLibrary<Oxide.Core.Libraries.Timer>(null);
			CompilableFile.compileLock = new object();
		}

		public CompilableFile(CSharpExtension extension, CSharpPluginLoader loader, string directory, string name)
		{
			this.Extension = extension;
			this.Loader = loader;
			this.Directory = directory;
			this.ScriptName = name;
			this.ScriptPath = Path.Combine(this.Directory, string.Concat(this.ScriptName, ".cs"));
			this.Name = Regex.Replace(this.ScriptName, "_", "");
			this.CheckLastModificationTime();
		}

		internal void CheckLastModificationTime()
		{
			if (!File.Exists(this.ScriptPath))
			{
				this.LastModifiedAt = new DateTime();
				return;
			}
			DateTime lastModificationTime = this.GetLastModificationTime();
			if (lastModificationTime != new DateTime())
			{
				this.LastModifiedAt = lastModificationTime;
			}
		}

		internal void Compile(Action<bool> callback)
		{
			lock (CompilableFile.compileLock)
			{
				if (this.CompilationQueuedAt <= 0f)
				{
					this.OnLoadingStarted();
					if (this.CompiledAssembly != null && !this.HasBeenModified())
					{
						if (!this.CompiledAssembly.IsLoading && this.CompiledAssembly.IsBatch)
						{
							if (!((IEnumerable<CompilablePlugin>)this.CompiledAssembly.CompilablePlugins).All<CompilablePlugin>((CompilablePlugin pl) => pl.IsLoading))
							{
								goto Label1;
							}
						}
						callback(true);
						return;
					}
				Label1:
					this.IsCompilationNeeded = true;
					this.CompileCallback = callback;
					this.CompilationQueuedAt = Interface.Oxide.Now;
					this.OnCompilationRequested();
				}
				else
				{
					float now = Interface.Oxide.Now - this.CompilationQueuedAt;
					Interface.Oxide.LogDebug(string.Format("Plugin compilation is already queued: {0} ({1:0.000} ago)", this.ScriptName, now), Array.Empty<object>());
				}
			}
		}

		internal DateTime GetLastModificationTime()
		{
			DateTime lastWriteTime;
			try
			{
				lastWriteTime = File.GetLastWriteTime(this.ScriptPath);
			}
			catch (IOException oException1)
			{
				IOException oException = oException1;
				Interface.Oxide.LogError("IOException while checking plugin: {0} ({1})", new object[] { this.ScriptName, oException.Message });
				lastWriteTime = new DateTime();
			}
			return lastWriteTime;
		}

		internal bool HasBeenModified()
		{
			DateTime lastModifiedAt = this.LastModifiedAt;
			this.CheckLastModificationTime();
			return this.LastModifiedAt != lastModifiedAt;
		}

		protected virtual void InitFailed(string message = null)
		{
			if (message != null)
			{
				Interface.Oxide.LogError(message, Array.Empty<object>());
			}
			Action<CSharpPlugin> loadCallback = this.LoadCallback;
			if (loadCallback == null)
			{
				return;
			}
			loadCallback(null);
		}

		internal void OnCompilationFailed()
		{
			if (this.timeoutTimer == null)
			{
				Interface.Oxide.LogWarning(string.Concat("Ignored unexpected plugin compilation failure: ", this.Name), Array.Empty<object>());
				return;
			}
			Oxide.Core.Libraries.Timer.TimerInstance timerInstance = this.timeoutTimer;
			if (timerInstance != null)
			{
				timerInstance.Destroy();
			}
			else
			{
			}
			this.timeoutTimer = null;
			this.CompilationQueuedAt = 0f;
			this.LastCompiledAt = new DateTime();
			Action<bool> compileCallback = this.CompileCallback;
			if (compileCallback != null)
			{
				compileCallback(false);
			}
			else
			{
			}
			this.IsCompilationNeeded = false;
		}

		protected virtual void OnCompilationRequested()
		{
		}

		internal virtual void OnCompilationStarted()
		{
			this.LastCompiledAt = this.LastModifiedAt;
			Oxide.Core.Libraries.Timer.TimerInstance timerInstance1 = this.timeoutTimer;
			if (timerInstance1 != null)
			{
				timerInstance1.Destroy();
			}
			else
			{
			}
			this.timeoutTimer = null;
			Interface.Oxide.NextTick(() => {
				Oxide.Core.Libraries.Timer.TimerInstance timerInstance = this.timeoutTimer;
				if (timerInstance != null)
				{
					timerInstance.Destroy();
				}
				else
				{
				}
				this.timeoutTimer = CompilableFile.timer.Once(60f, new Action(this.OnCompilationTimeout), null);
			});
		}

		internal void OnCompilationSucceeded(Oxide.Plugins.CompiledAssembly compiledAssembly)
		{
			if (this.timeoutTimer == null)
			{
				Interface.Oxide.LogWarning(string.Concat("Ignored unexpected plugin compilation: ", this.Name), Array.Empty<object>());
				return;
			}
			Oxide.Core.Libraries.Timer.TimerInstance timerInstance = this.timeoutTimer;
			if (timerInstance != null)
			{
				timerInstance.Destroy();
			}
			else
			{
			}
			this.timeoutTimer = null;
			this.IsCompilationNeeded = false;
			this.CompilationQueuedAt = 0f;
			this.CompiledAssembly = compiledAssembly;
			Action<bool> compileCallback = this.CompileCallback;
			if (compileCallback == null)
			{
				return;
			}
			compileCallback(true);
		}

		internal void OnCompilationTimeout()
		{
			Interface.Oxide.LogError(string.Concat("Timed out waiting for plugin to be compiled: ", this.Name), Array.Empty<object>());
			this.CompilerErrors = "Timed out waiting for compilation";
			this.OnCompilationFailed();
		}

		protected virtual void OnLoadingStarted()
		{
		}
	}
}