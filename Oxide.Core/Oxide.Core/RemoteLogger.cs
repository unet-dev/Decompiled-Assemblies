using Newtonsoft.Json;
using Oxide.Core.Extensions;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Oxide.Core
{
	public static class RemoteLogger
	{
		private const int projectId = 141692;

		private const string host = "sentry.io";

		private const string publicKey = "2d0162c790be4036a94d2d8326d7f900";

		private const string secretKey = "8a6249aad4b84e368f900b32396e8b04";

		private readonly static string Url;

		private readonly static string[][] sentryAuth;

		public static string Filename;

		private readonly static Dictionary<string, string> Tags;

		private readonly static Timer Timers;

		private readonly static WebRequests Webrequests;

		private readonly static List<RemoteLogger.QueuedReport> QueuedReports;

		private static bool submittingReports;

		public static string[] ExceptionFilter;

		static RemoteLogger()
		{
			RemoteLogger.Url = string.Concat("https://sentry.io/api/", 141692, "/store/");
			RemoteLogger.sentryAuth = new string[][] { new string[] { "sentry_version", "7" }, new string[] { "sentry_client", "MiniRaven/1.0" }, new string[] { "sentry_key", "2d0162c790be4036a94d2d8326d7f900" }, new string[] { "sentry_secret", "8a6249aad4b84e368f900b32396e8b04" } };
			RemoteLogger.Filename = Utility.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);
			Dictionary<string, string> strs = new Dictionary<string, string>()
			{
				{ "arch", (IntPtr.Size == 8 ? "x64" : "x86") }
			};
			PlatformID platform = Environment.OSVersion.Platform;
			strs.Add("platform", platform.ToString().ToLower());
			strs.Add("os version", Environment.OSVersion.Version.ToString().ToLower());
			strs.Add("game", RemoteLogger.Filename.ToLower().Replace("dedicated", "").Replace("server", "").Replace("-", "").Replace("_", ""));
			RemoteLogger.Tags = strs;
			RemoteLogger.Timers = Interface.Oxide.GetLibrary<Timer>(null);
			RemoteLogger.Webrequests = Interface.Oxide.GetLibrary<WebRequests>(null);
			RemoteLogger.QueuedReports = new List<RemoteLogger.QueuedReport>();
			RemoteLogger.ExceptionFilter = new string[] { "BadImageFormatException", "DllNotFoundException", "FileNotFoundException", "IOException", "KeyNotFoundException", "Oxide.Core.Configuration", "Oxide.Ext.", "Oxide.Plugins.<", "ReflectionTypeLoadException", "Sharing violation", "UnauthorizedAccessException", "WebException" };
		}

		private static Dictionary<string, string> BuildHeaders()
		{
			string str = string.Join(", ", (
				from x in RemoteLogger.sentryAuth
				select string.Join("=", x)).ToArray<string>());
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = utcNow.Subtract(new DateTime(1970, 1, 1));
			str = string.Concat(str, ", sentry_timestamp=", (int)timeSpan.TotalSeconds);
			return new Dictionary<string, string>()
			{
				{ "X-Sentry-Auth", string.Concat("Sentry ", str) }
			};
		}

		public static void Debug(string message)
		{
			RemoteLogger.EnqueueReport("debug", Assembly.GetCallingAssembly(), RemoteLogger.GetCurrentMethod(), message, null);
		}

		private static void EnqueueReport(string level, Assembly assembly, string culprit, string message, string exception = null)
		{
			RemoteLogger.Report report = new RemoteLogger.Report(level, culprit, message, exception);
			report.DetectModules(assembly);
			RemoteLogger.EnqueueReport(report);
		}

		private static void EnqueueReport(string level, string[] stackTrace, string culprit, string message, string exception = null)
		{
			RemoteLogger.Report report = new RemoteLogger.Report(level, culprit, message, exception);
			report.DetectModules(stackTrace);
			RemoteLogger.EnqueueReport(report);
		}

		private static void EnqueueReport(RemoteLogger.Report report)
		{
			Dictionary<string, string>.ValueCollection values = report.extra.Values;
			if (!values.Contains<string>("Oxide.Core") && !values.Contains<string>("Oxide.Plugins.Compiler"))
			{
				return;
			}
			string[] exceptionFilter = RemoteLogger.ExceptionFilter;
			for (int i = 0; i < (int)exceptionFilter.Length; i++)
			{
				string str = exceptionFilter[i];
				if (values.Contains<string>(str) || values.Contains<string>(str))
				{
					return;
				}
			}
			RemoteLogger.QueuedReports.Add(new RemoteLogger.QueuedReport(report));
			if (!RemoteLogger.submittingReports)
			{
				RemoteLogger.SubmitNextReport();
			}
		}

		public static void Error(string message)
		{
			RemoteLogger.EnqueueReport("error", Assembly.GetCallingAssembly(), RemoteLogger.GetCurrentMethod(), message, null);
		}

		public static void Exception(string message, Exception exception)
		{
			if (!exception.StackTrace.Contains("Oxide.Core") && !exception.StackTrace.Contains("Oxide.Plugins.Compiler"))
			{
				return;
			}
			string[] exceptionFilter = RemoteLogger.ExceptionFilter;
			for (int i = 0; i < (int)exceptionFilter.Length; i++)
			{
				string str = exceptionFilter[i];
				if (exception.StackTrace.Contains(str) || message.Contains(str))
				{
					return;
				}
			}
			RemoteLogger.EnqueueReport("fatal", Assembly.GetCallingAssembly(), RemoteLogger.GetCurrentMethod(), message, exception.ToString());
		}

		public static void Exception(string message, string rawStackTrace)
		{
			string[] strArrays = rawStackTrace.Split(new char[] { '\r', '\n' });
			string str = strArrays[0].Split(new char[] { '(' })[0].Trim();
			RemoteLogger.EnqueueReport("fatal", strArrays, str, message, rawStackTrace);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static string GetCurrentMethod()
		{
			string fullName;
			MethodBase method = (new StackTrace()).GetFrame(2).GetMethod();
			Type declaringType = method.DeclaringType;
			if (declaringType != null)
			{
				fullName = declaringType.FullName;
			}
			else
			{
				fullName = null;
			}
			return string.Concat(fullName, ".", method.Name);
		}

		public static string GetTag(string name)
		{
			string str;
			if (!RemoteLogger.Tags.TryGetValue(name, out str))
			{
				return "unknown";
			}
			return str;
		}

		public static void Info(string message)
		{
			RemoteLogger.EnqueueReport("info", Assembly.GetCallingAssembly(), RemoteLogger.GetCurrentMethod(), message, null);
		}

		public static void SetTag(string name, string value)
		{
			RemoteLogger.Tags[name] = value;
		}

		private static void SubmitNextReport()
		{
			if (RemoteLogger.QueuedReports.Count < 1)
			{
				return;
			}
			RemoteLogger.QueuedReport item = RemoteLogger.QueuedReports[0];
			RemoteLogger.submittingReports = true;
			RemoteLogger.Webrequests.Enqueue(RemoteLogger.Url, item.Body, (int code, string response) => {
				if (code == 200)
				{
					RemoteLogger.QueuedReports.RemoveAt(0);
					RemoteLogger.submittingReports = false;
					RemoteLogger.SubmitNextReport();
					return;
				}
				RemoteLogger.Timers.Once(5f, new Action(RemoteLogger.SubmitNextReport), null);
			}, null, RequestMethod.POST, item.Headers, 0f);
		}

		public static void Warning(string message)
		{
			RemoteLogger.EnqueueReport("warning", Assembly.GetCallingAssembly(), RemoteLogger.GetCurrentMethod(), message, null);
		}

		private class QueuedReport
		{
			public readonly Dictionary<string, string> Headers;

			public readonly string Body;

			public QueuedReport(RemoteLogger.Report report)
			{
				this.Headers = RemoteLogger.BuildHeaders();
				this.Body = JsonConvert.SerializeObject(report);
			}
		}

		public class Report
		{
			public string message;

			public string level;

			public string culprit;

			public string platform;

			public string release;

			public Dictionary<string, string> tags;

			public Dictionary<string, string> modules;

			public Dictionary<string, string> extra;

			private Dictionary<string, string> headers;

			public Report(string level, string culprit, string message, string exception = null)
			{
				this.headers = RemoteLogger.BuildHeaders();
				this.level = level;
				this.message = (message.Length > 1000 ? message.Substring(0, 1000) : message);
				this.culprit = culprit;
				this.modules = new Dictionary<string, string>();
				foreach (Extension allExtension in Interface.Oxide.GetAllExtensions())
				{
					this.modules[allExtension.GetType().Assembly.GetName().Name] = allExtension.Version.ToString();
				}
				if (exception != null)
				{
					this.extra = new Dictionary<string, string>();
					string[] array = exception.Split(new char[] { '\n' }).Take<string>(31).ToArray<string>();
					for (int i = 0; i < (int)array.Length; i++)
					{
						string str = array[i].Trim(new char[] { ' ', '\r', '\n' }).Replace('\t', ' ');
						if (str.Length > 0)
						{
							this.extra[string.Concat("line_", i.ToString("00"))] = str;
						}
					}
				}
			}

			public void DetectModules(Assembly assembly)
			{
				if (((IEnumerable<Type>)assembly.GetTypes()).FirstOrDefault<Type>((Type t) => t.BaseType == typeof(Extension)) == null)
				{
					Type type = ((IEnumerable<Type>)assembly.GetTypes()).FirstOrDefault<Type>((Type t) => RemoteLogger.Report.IsTypeDerivedFrom(t, typeof(Plugin)));
					if (type != null)
					{
						Plugin plugin = Interface.Oxide.RootPluginManager.GetPlugin(type.Name);
						if (plugin != null)
						{
							this.modules[string.Concat("Plugins.", plugin.Name)] = plugin.Version.ToString();
						}
					}
				}
			}

			public void DetectModules(string[] stackTrace)
			{
				string[] strArrays = stackTrace;
				int num = 0;
				while (num < (int)strArrays.Length)
				{
					string str = strArrays[num];
					if (!str.StartsWith("Oxide.Plugins.PluginCompiler") || !str.Contains("+"))
					{
						num++;
					}
					else
					{
						string str1 = str.Split(new char[] { '+' })[0];
						Plugin plugin = Interface.Oxide.RootPluginManager.GetPlugin(str1);
						if (plugin == null)
						{
							break;
						}
						this.modules[string.Concat("Plugins.", plugin.Name)] = plugin.Version.ToString();
						return;
					}
				}
			}

			private static bool IsTypeDerivedFrom(Type type, Type baseType)
			{
				while (type != null && type != baseType)
				{
					Type type1 = type.BaseType;
					type = type1;
					if (type1 != baseType)
					{
						continue;
					}
					return true;
				}
				return false;
			}
		}
	}
}