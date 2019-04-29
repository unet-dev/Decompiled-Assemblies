using Mono.Unix;
using Mono.Unix.Native;
using ObjectStream;
using ObjectStream.Data;
using Oxide.Core;
using Oxide.Core.Libraries;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;

namespace Oxide.Plugins
{
	public class PluginCompiler
	{
		public static bool AutoShutdown;

		public static bool TraceRan;

		public static string FileName;

		public static string BinaryPath;

		public static string CompilerVersion;

		private static int downloadRetries;

		private Process process;

		private readonly Regex fileErrorRegex = new Regex("([\\w\\.]+)\\(\\d+\\,\\d+\\+?\\): error|error \\w+: Source file `[\\\\\\./]*([\\w\\.]+)", RegexOptions.Compiled);

		private ObjectStreamClient<CompilerMessage> client;

		private Hash<int, Compilation> compilations;

		private Queue<CompilerMessage> messageQueue;

		private volatile int lastId;

		private volatile bool ready;

		private Oxide.Core.Libraries.Timer.TimerInstance idleTimer;

		static PluginCompiler()
		{
			PluginCompiler.AutoShutdown = true;
			PluginCompiler.FileName = "basic.exe";
			PluginCompiler.downloadRetries = 0;
		}

		public PluginCompiler()
		{
			this.compilations = new Hash<int, Compilation>();
			this.messageQueue = new Queue<CompilerMessage>();
		}

		private bool CheckCompiler()
		{
			PluginCompiler.CheckCompilerBinary();
			Oxide.Core.Libraries.Timer.TimerInstance timerInstance = this.idleTimer;
			if (timerInstance != null)
			{
				timerInstance.Destroy();
			}
			else
			{
			}
			if (PluginCompiler.BinaryPath == null)
			{
				return false;
			}
			if (this.process != null && this.process.Handle != IntPtr.Zero && !this.process.HasExited)
			{
				return true;
			}
			PluginCompiler.SetCompilerVersion();
			PluginCompiler.PurgeOldLogs();
			this.Shutdown();
			string[] strArrays = new string[] { "/service", string.Concat("/logPath:", PluginCompiler.EscapePath(Interface.Oxide.LogDirectory)) };
			try
			{
				Process process = new Process();
				process.StartInfo.FileName = PluginCompiler.BinaryPath;
				process.StartInfo.Arguments = string.Join(" ", strArrays);
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.EnableRaisingEvents = true;
				this.process = process;
				switch (Environment.OSVersion.Platform)
				{
					case PlatformID.Win32S:
					case PlatformID.Win32Windows:
					case PlatformID.Win32NT:
					{
						string environmentVariable = Environment.GetEnvironmentVariable("PATH");
						Environment.SetEnvironmentVariable("PATH", string.Concat(environmentVariable, ";", Path.Combine(Interface.Oxide.ExtensionDirectory, "x86")));
						goto case PlatformID.Xbox;
					}
					case PlatformID.WinCE:
					case PlatformID.Xbox:
					{
						this.process.Exited += new EventHandler(this.OnProcessExited);
						this.process.Start();
						break;
					}
					case PlatformID.Unix:
					case PlatformID.MacOSX:
					{
						string str = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");
						this.process.StartInfo.EnvironmentVariables["LD_LIBRARY_PATH"] = Path.Combine(Interface.Oxide.ExtensionDirectory, (IntPtr.Size == 8 ? "x64" : "x86"));
						Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", string.Concat(str, ":", Path.Combine(Interface.Oxide.ExtensionDirectory, (IntPtr.Size == 8 ? "x64" : "x86"))));
						goto case PlatformID.Xbox;
					}
					default:
					{
						goto case PlatformID.Xbox;
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Process process1 = this.process;
				if (process1 != null)
				{
					process1.Dispose();
				}
				else
				{
				}
				this.process = null;
				Interface.Oxide.LogException(string.Concat("Exception while starting compiler version ", PluginCompiler.CompilerVersion, ": "), exception);
				if (PluginCompiler.BinaryPath.Contains("'"))
				{
					Interface.Oxide.LogWarning("Server directory path contains an apostrophe, compiler will not work until path is renamed", Array.Empty<object>());
				}
				else if (Environment.OSVersion.Platform == PlatformID.Unix)
				{
					Interface.Oxide.LogWarning("Compiler may not be set as executable; chmod +x or 0744/0755 required", Array.Empty<object>());
				}
				if (exception.GetBaseException() != exception)
				{
					Interface.Oxide.LogException("BaseException: ", exception.GetBaseException());
				}
				Win32Exception win32Exception = exception as Win32Exception;
				if (win32Exception != null)
				{
					Interface.Oxide.LogError("Win32 NativeErrorCode: {0} ErrorCode: {1} HelpLink: {2}", new object[] { win32Exception.NativeErrorCode, win32Exception.ErrorCode, win32Exception.HelpLink });
				}
			}
			if (this.process == null)
			{
				return false;
			}
			this.client = new ObjectStreamClient<CompilerMessage>(this.process.StandardOutput.BaseStream, this.process.StandardInput.BaseStream);
			this.client.Message += new ConnectionMessageEventHandler<CompilerMessage, CompilerMessage>(this.OnMessage);
			this.client.Error += new StreamExceptionEventHandler(PluginCompiler.OnError);
			this.client.Start();
			return true;
		}

		public static void CheckCompilerBinary()
		{
			PluginCompiler.BinaryPath = null;
			string rootDirectory = Interface.Oxide.RootDirectory;
			string str = Path.Combine(rootDirectory, PluginCompiler.FileName);
			if (File.Exists(str))
			{
				PluginCompiler.BinaryPath = str;
				return;
			}
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.Win32NT:
				{
					PluginCompiler.FileName = "Compiler.exe";
					str = Path.Combine(rootDirectory, PluginCompiler.FileName);
					PluginCompiler.UpdateCheck();
					PluginCompiler.BinaryPath = str;
					return;
				}
				case PlatformID.WinCE:
				case PlatformID.Xbox:
				{
					PluginCompiler.BinaryPath = str;
					return;
				}
				case PlatformID.Unix:
				case PlatformID.MacOSX:
				{
					PluginCompiler.FileName = string.Concat("Compiler.", (IntPtr.Size != 8 ? "x86" : "x86_x64"));
					str = Path.Combine(rootDirectory, PluginCompiler.FileName);
					PluginCompiler.UpdateCheck();
					try
					{
						if (Syscall.access(str, AccessModes.X_OK) == 0)
						{
							PluginCompiler.BinaryPath = str;
							return;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						Interface.Oxide.LogError(string.Concat("Unable to check ", PluginCompiler.FileName, " for executable permission"), Array.Empty<object>());
						Interface.Oxide.LogError(exception.Message, Array.Empty<object>());
						Interface.Oxide.LogError(exception.StackTrace, Array.Empty<object>());
					}
					try
					{
						Syscall.chmod(str, FilePermissions.S_IRWXU);
						PluginCompiler.BinaryPath = str;
						return;
					}
					catch (Exception exception3)
					{
						Exception exception2 = exception3;
						Interface.Oxide.LogError(string.Concat("Could not set ", PluginCompiler.FileName, " as executable, please set manually"), Array.Empty<object>());
						Interface.Oxide.LogError(exception2.Message, Array.Empty<object>());
						Interface.Oxide.LogError(exception2.StackTrace, Array.Empty<object>());
						PluginCompiler.BinaryPath = str;
						return;
					}
					break;
				}
				default:
				{
					PluginCompiler.BinaryPath = str;
					return;
				}
			}
		}

		internal void Compile(CompilablePlugin[] plugins, Action<Compilation> callback)
		{
			int num = this.lastId;
			this.lastId = num + 1;
			int num1 = num;
			Compilation compilation = new Compilation(num1, callback, plugins);
			this.compilations[num1] = compilation;
			compilation.Prepare(() => this.EnqueueCompilation(compilation));
		}

		private void DependencyTrace()
		{
			if (PluginCompiler.TraceRan || Environment.OSVersion.Platform != PlatformID.Unix)
			{
				return;
			}
			try
			{
				Interface.Oxide.LogWarning(string.Concat("Running dependency trace for ", PluginCompiler.FileName), Array.Empty<object>());
				Process process = new Process();
				process.StartInfo.WorkingDirectory = Interface.Oxide.RootDirectory;
				process.StartInfo.FileName = "/bin/bash";
				process.StartInfo.Arguments = string.Concat("-c \"LD_TRACE_LOADED_OBJECTS=1 ", PluginCompiler.BinaryPath, "\"");
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.EnableRaisingEvents = true;
				Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", string.Concat(Environment.GetEnvironmentVariable("LD_LIBRARY_PATH"), ":", Path.Combine(Interface.Oxide.ExtensionDirectory, (IntPtr.Size == 8 ? "x64" : "x86"))));
				process.StartInfo.EnvironmentVariables["LD_LIBRARY_PATH"] = Path.Combine(Interface.Oxide.ExtensionDirectory, (IntPtr.Size == 8 ? "x64" : "x86"));
				process.ErrorDataReceived += new DataReceivedEventHandler((object s, DataReceivedEventArgs e) => Interface.Oxide.LogError(e.Data.TrimStart(Array.Empty<char>()), Array.Empty<object>()));
				process.OutputDataReceived += new DataReceivedEventHandler((object s, DataReceivedEventArgs e) => Interface.Oxide.LogError(e.Data.TrimStart(Array.Empty<char>()), Array.Empty<object>()));
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
			}
			catch (Exception exception)
			{
			}
			PluginCompiler.TraceRan = true;
		}

		private static void DownloadCompiler(WebResponse response, string remoteHash)
		{
			try
			{
				Interface.Oxide.LogInfo(string.Concat("Downloading ", PluginCompiler.FileName, " for .cs (C#) plugin compilation"), Array.Empty<object>());
				Stream responseStream = response.GetResponseStream();
				FileStream fileStream = new FileStream(PluginCompiler.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
				int num = 10000;
				byte[] numArray = new byte[num];
				while (true)
				{
					int num1 = responseStream.Read(numArray, 0, num);
					if (num1 == -1 || num1 == 0)
					{
						break;
					}
					fileStream.Write(numArray, 0, num1);
				}
				fileStream.Flush();
				fileStream.Close();
				responseStream.Close();
				response.Close();
				if (PluginCompiler.downloadRetries >= 3)
				{
					Interface.Oxide.LogInfo(string.Concat("Couldn't download ", PluginCompiler.FileName, "! Please download manually from: https://github.com/OxideMod/Compiler/releases/download/latest/", PluginCompiler.FileName), Array.Empty<object>());
				}
				else if (remoteHash == (File.Exists(PluginCompiler.BinaryPath) ? PluginCompiler.GetHash(PluginCompiler.BinaryPath, PluginCompiler.Algorithms.MD5) : "0"))
				{
					Interface.Oxide.LogInfo(string.Concat("Download of ", PluginCompiler.FileName, " completed successfully"), Array.Empty<object>());
				}
				else
				{
					Interface.Oxide.LogInfo(string.Concat("Local hash did not match remote hash for ", PluginCompiler.FileName, ", attempting download again"), Array.Empty<object>());
					PluginCompiler.UpdateCheck();
					PluginCompiler.downloadRetries++;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Interface.Oxide.LogError(string.Concat("Couldn't download ", PluginCompiler.FileName, "! Please download manually from: https://github.com/OxideMod/Compiler/releases/download/latest/", PluginCompiler.FileName), Array.Empty<object>());
				Interface.Oxide.LogError(exception.Message, Array.Empty<object>());
			}
		}

		private void EnqueueCompilation(Compilation compilation)
		{
			if (compilation.plugins.Count < 1)
			{
				return;
			}
			if (!this.CheckCompiler())
			{
				this.OnCompilerFailed(string.Concat("compiler version ", PluginCompiler.CompilerVersion, " couldn't be started"));
				return;
			}
			compilation.Started();
			List<CompilerFile> list = (
				from path in compilation.plugins.SelectMany<CompilablePlugin, string>((CompilablePlugin plugin) => plugin.IncludePaths).Distinct<string>()
				select new CompilerFile(path)).ToList<CompilerFile>();
			list.AddRange(
				from plugin in compilation.plugins
				select new CompilerFile(string.Concat(plugin.ScriptName, ".cs"), plugin.ScriptSource));
			CompilerData compilerDatum = new CompilerData()
			{
				OutputFile = compilation.name,
				SourceFiles = list.ToArray(),
				ReferenceFiles = compilation.references.Values.ToArray<CompilerFile>()
			};
			CompilerMessage compilerMessage = new CompilerMessage()
			{
				Id = compilation.id,
				Data = compilerDatum,
				Type = CompilerMessageType.Compile
			};
			if (this.ready)
			{
				this.client.PushMessage(compilerMessage);
				return;
			}
			this.messageQueue.Enqueue(compilerMessage);
		}

		private static string EscapePath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return "\"\"";
			}
			path = Regex.Replace(path, "(\\\\*)\"", "$1\\$0");
			path = Regex.Replace(path, "^(.*\\s.*?)(\\\\*)$", "\"$1$2$2\"");
			return path;
		}

		private static string GetHash(string filePath, HashAlgorithm algorithm)
		{
			string lower;
			using (BufferedStream bufferedStream = new BufferedStream(File.OpenRead(filePath), 100000))
			{
				lower = BitConverter.ToString(algorithm.ComputeHash(bufferedStream)).Replace("-", string.Empty).ToLower();
			}
			return lower;
		}

		private void OnCompilerFailed(string reason)
		{
			foreach (Compilation value in this.compilations.Values)
			{
				foreach (CompilablePlugin plugin in value.plugins)
				{
					plugin.CompilerErrors = reason;
				}
				value.Completed(null);
			}
			this.compilations.Clear();
		}

		private static void OnError(Exception exception)
		{
			Interface.Oxide.LogException("Compilation error: ", exception);
		}

		private void OnMessage(ObjectStreamConnection<CompilerMessage, CompilerMessage> connection, CompilerMessage message)
		{
			Func<string, bool> func = null;
			if (message == null)
			{
				Interface.Oxide.NextTick(() => {
					this.OnCompilerFailed(string.Concat("compiler version ", PluginCompiler.CompilerVersion, " disconnected"));
					this.DependencyTrace();
					this.Shutdown();
				});
				return;
			}
			switch (message.Type)
			{
				case CompilerMessageType.Assembly:
				{
					Compilation item = this.compilations[message.Id];
					if (item == null)
					{
						Interface.Oxide.LogWarning("Compiler compiled an unknown assembly", Array.Empty<object>());
						return;
					}
					item.endedAt = Interface.Oxide.Now;
					string extraData = (string)message.ExtraData;
					if (extraData != null)
					{
						string[] strArrays = extraData.Split(new char[] { '\r', '\n' });
						for (int i = 0; i < (int)strArrays.Length; i++)
						{
							string str = strArrays[i];
							Match match = this.fileErrorRegex.Match(str.Trim());
							for (int j = 1; j < match.Groups.Count; j++)
							{
								string value = match.Groups[j].Value;
								if (value.Trim() != string.Empty)
								{
									string str1 = value.Basename(null);
									string str2 = str1.Substring(0, str1.Length - 3);
									CompilablePlugin compilablePlugin = item.plugins.SingleOrDefault<CompilablePlugin>((CompilablePlugin pl) => pl.ScriptName == str2);
									if (compilablePlugin != null)
									{
										HashSet<string> requires = compilablePlugin.Requires;
										Func<string, bool> func1 = func;
										if (func1 == null)
										{
											Func<string, bool> func2 = (string name) => !item.IncludesRequiredPlugin(name);
											Func<string, bool> func3 = func2;
											func = func2;
											func1 = func3;
										}
										IEnumerable<string> strs = requires.Where<string>(func1);
										if (!strs.Any<string>())
										{
											string str3 = str.Trim();
											string pluginDirectory = Interface.Oxide.PluginDirectory;
											char directorySeparatorChar = Path.DirectorySeparatorChar;
											compilablePlugin.CompilerErrors = str3.Replace(string.Concat(pluginDirectory, directorySeparatorChar.ToString()), string.Empty);
										}
										else
										{
											compilablePlugin.CompilerErrors = string.Concat("Missing dependencies: ", strs.ToSentence<string>());
										}
									}
									else
									{
										Interface.Oxide.LogError(string.Concat("Unable to resolve script error to plugin: ", str), Array.Empty<object>());
									}
								}
							}
						}
					}
					item.Completed((byte[])message.Data);
					this.compilations.Remove(message.Id);
					Oxide.Core.Libraries.Timer.TimerInstance timerInstance1 = this.idleTimer;
					if (timerInstance1 != null)
					{
						timerInstance1.Destroy();
					}
					else
					{
					}
					if (!PluginCompiler.AutoShutdown)
					{
						return;
					}
					Interface.Oxide.NextTick(() => {
						Oxide.Core.Libraries.Timer.TimerInstance timerInstance = this.idleTimer;
						if (timerInstance != null)
						{
							timerInstance.Destroy();
						}
						else
						{
						}
						if (PluginCompiler.AutoShutdown)
						{
							this.idleTimer = Interface.Oxide.GetLibrary<Oxide.Core.Libraries.Timer>(null).Once(60f, new Action(this.Shutdown), null);
						}
					});
					return;
				}
				case CompilerMessageType.Compile:
				case CompilerMessageType.Exit:
				{
					return;
				}
				case CompilerMessageType.Error:
				{
					Interface.Oxide.LogError("Compilation error: {0}", new object[] { message.Data });
					this.compilations[message.Id].Completed(null);
					this.compilations.Remove(message.Id);
					Oxide.Core.Libraries.Timer.TimerInstance timerInstance2 = this.idleTimer;
					if (timerInstance2 != null)
					{
						timerInstance2.Destroy();
					}
					else
					{
					}
					if (!PluginCompiler.AutoShutdown)
					{
						return;
					}
					Interface.Oxide.NextTick(() => {
						Oxide.Core.Libraries.Timer.TimerInstance timerInstance = this.idleTimer;
						if (timerInstance != null)
						{
							timerInstance.Destroy();
						}
						else
						{
						}
						this.idleTimer = Interface.Oxide.GetLibrary<Oxide.Core.Libraries.Timer>(null).Once(60f, new Action(this.Shutdown), null);
					});
					return;
				}
				case CompilerMessageType.Ready:
				{
					connection.PushMessage(message);
					if (this.ready)
					{
						return;
					}
					this.ready = true;
					while (this.messageQueue.Count > 0)
					{
						connection.PushMessage(this.messageQueue.Dequeue());
					}
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private void OnProcessExited(object sender, EventArgs eventArgs)
		{
			Interface.Oxide.NextTick(() => {
				string[] strArrays;
				int i;
				this.OnCompilerFailed(string.Concat("compiler version ", PluginCompiler.CompilerVersion, " was closed unexpectedly"));
				if (Environment.OSVersion.Platform != PlatformID.Unix)
				{
					string environmentVariable = Environment.GetEnvironmentVariable("PATH");
					string str = Path.Combine(Interface.Oxide.ExtensionDirectory, "x86");
					if (string.IsNullOrEmpty(environmentVariable) || !environmentVariable.Contains(str))
					{
						Interface.Oxide.LogWarning(string.Concat("PATH does not container path to compiler dependencies: ", str), Array.Empty<object>());
					}
					else
					{
						Interface.Oxide.LogWarning("Compiler may have been closed by interference from security software or install is missing files", Array.Empty<object>());
						string str1 = Path.Combine(Interface.Oxide.ExtensionDirectory, "x86");
						strArrays = new string[] { "mono-2.0.dll", "msvcp140.dll", "msvcr120.dll" };
						for (i = 0; i < (int)strArrays.Length; i++)
						{
							string str2 = Path.Combine(str1, strArrays[i]);
							if (!File.Exists(str2))
							{
								Interface.Oxide.LogWarning(string.Concat(str2, " is missing"), Array.Empty<object>());
							}
						}
					}
				}
				else
				{
					string environmentVariable1 = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");
					string str3 = Path.Combine(Interface.Oxide.ExtensionDirectory, (IntPtr.Size == 8 ? "x64" : "x86"));
					if (string.IsNullOrEmpty(environmentVariable1) || !environmentVariable1.Contains(str3))
					{
						Interface.Oxide.LogWarning(string.Concat("LD_LIBRARY_PATH does not container path to compiler dependencies: ", str3), Array.Empty<object>());
					}
					else
					{
						Interface.Oxide.LogWarning("User running server may not have the proper permissions or install is missing files", Array.Empty<object>());
						Interface.Oxide.LogWarning(string.Concat("User running server: ", Environment.UserName), Array.Empty<object>());
						UnixFileInfo unixFileInfo = new UnixFileInfo(PluginCompiler.BinaryPath);
						Interface.Oxide.LogWarning(string.Format("Compiler under user/group: {0}/{1}", unixFileInfo.OwnerUser, unixFileInfo.OwnerGroup), Array.Empty<object>());
						string str4 = Path.Combine(Interface.Oxide.ExtensionDirectory, (IntPtr.Size == 8 ? "x64" : "x86"));
						strArrays = new string[] { "libmonoboehm-2.0.so.1", "libMonoPosixHelper.so", "mono-2.0.dll" };
						for (i = 0; i < (int)strArrays.Length; i++)
						{
							string str5 = Path.Combine(str4, strArrays[i]);
							if (!File.Exists(str5))
							{
								Interface.Oxide.LogWarning(string.Concat(str5, " is missing"), Array.Empty<object>());
							}
						}
					}
				}
				this.Shutdown();
			});
		}

		private static void PurgeOldLogs()
		{
			try
			{
				foreach (string str in Directory.GetFiles(Interface.Oxide.LogDirectory, "*.txt").Where<string>((string f) => {
					string fileName = Path.GetFileName(f);
					if (fileName == null)
					{
						return false;
					}
					return fileName.StartsWith("compiler_");
				}))
				{
					File.Delete(str);
				}
			}
			catch (Exception exception)
			{
			}
		}

		private static void SetCompilerVersion()
		{
			PluginCompiler.CompilerVersion = (File.Exists(PluginCompiler.BinaryPath) ? FileVersionInfo.GetVersionInfo(PluginCompiler.BinaryPath).FileVersion : "Unknown");
			RemoteLogger.SetTag("compiler version", PluginCompiler.CompilerVersion);
		}

		public void Shutdown()
		{
			this.ready = false;
			Process process = this.process;
			if (process != null)
			{
				process.Exited -= new EventHandler(this.OnProcessExited);
			}
			this.process = null;
			if (this.client == null)
			{
				return;
			}
			this.client.Message -= new ConnectionMessageEventHandler<CompilerMessage, CompilerMessage>(this.OnMessage);
			this.client.Error -= new StreamExceptionEventHandler(PluginCompiler.OnError);
			this.client.PushMessage(new CompilerMessage()
			{
				Type = CompilerMessageType.Exit
			});
			this.client.Stop();
			this.client = null;
			if (process == null)
			{
				return;
			}
			ThreadPool.QueueUserWorkItem((object _) => {
				Thread.Sleep(5000);
				if (!process.HasExited)
				{
					process.Close();
				}
			});
		}

		private static void UpdateCheck()
		{
			try
			{
				string str = Path.Combine(Interface.Oxide.RootDirectory, PluginCompiler.FileName);
				HttpWebResponse response = (HttpWebResponse)((HttpWebRequest)WebRequest.Create(string.Concat("https://umod-01.nyc3.digitaloceanspaces.com/", PluginCompiler.FileName))).GetResponse();
				int statusCode = (int)response.StatusCode;
				if (statusCode != 200)
				{
					Interface.Oxide.LogWarning(string.Format("Status code from download location was not okay (code {0})", statusCode), Array.Empty<object>());
				}
				string str1 = response.Headers[HttpResponseHeader.ETag].Trim(new char[] { '\"' });
				string str2 = (File.Exists(str) ? PluginCompiler.GetHash(str, PluginCompiler.Algorithms.MD5) : "0");
				Interface.Oxide.LogInfo(string.Concat("Latest compiler MD5: ", str1), Array.Empty<object>());
				Interface.Oxide.LogInfo(string.Concat("Local compiler MD5: ", str2), Array.Empty<object>());
				if (str1 != str2)
				{
					Interface.Oxide.LogInfo("Compiler hashes did not match, downloading latest", Array.Empty<object>());
					PluginCompiler.DownloadCompiler(response, str1);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Interface.Oxide.LogError(string.Concat("Couldn't check for update to ", PluginCompiler.FileName), Array.Empty<object>());
				Interface.Oxide.LogError(exception.Message, Array.Empty<object>());
			}
		}

		private static class Algorithms
		{
			public readonly static HashAlgorithm MD5;

			public readonly static HashAlgorithm SHA1;

			public readonly static HashAlgorithm SHA256;

			public readonly static HashAlgorithm SHA384;

			public readonly static HashAlgorithm SHA512;

			public readonly static HashAlgorithm RIPEMD160;

			static Algorithms()
			{
				PluginCompiler.Algorithms.MD5 = new MD5CryptoServiceProvider();
				PluginCompiler.Algorithms.SHA1 = new SHA1Managed();
				PluginCompiler.Algorithms.SHA256 = new SHA256Managed();
				PluginCompiler.Algorithms.SHA384 = new SHA384Managed();
				PluginCompiler.Algorithms.SHA512 = new SHA512Managed();
				PluginCompiler.Algorithms.RIPEMD160 = new RIPEMD160Managed();
			}
		}
	}
}