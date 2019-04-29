using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Facepunch
{
	public class Report
	{
		public string event_id
		{
			get;
			set;
		}

		public string message
		{
			get;
			set;
		}

		public string platform
		{
			get;
			set;
		}

		public string release
		{
			get;
			set;
		}

		public Report.StackTrace stacktrace
		{
			get;
			set;
		}

		public Dictionary<string, string> tags
		{
			get;
			set;
		}

		public Report()
		{
		}

		public class StackTrace
		{
			public Report.StackTrace.StackFrame[] frames
			{
				get;
				set;
			}

			public StackTrace()
			{
				this.frames = (
					from x in (IEnumerable<StackFrame>)(new System.Diagnostics.StackTrace(0, true)).GetFrames()
					select new Report.StackTrace.StackFrame(x)).ToArray<Report.StackTrace.StackFrame>();
			}

			public StackTrace(string unityStack)
			{
				this.frames = (
					from x in unityStack.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Reverse<string>()
					select new Report.StackTrace.StackFrame(x)).ToArray<Report.StackTrace.StackFrame>();
			}

			public class StackFrame
			{
				public int colno
				{
					get;
					set;
				}

				public string context_line
				{
					get;
					private set;
				}

				public string filename
				{
					get;
					private set;
				}

				public string function
				{
					get;
					private set;
				}

				public bool in_app
				{
					get;
					private set;
				}

				public int lineno
				{
					get;
					private set;
				}

				public string module
				{
					get;
					private set;
				}

				public StackFrame(StackFrame x)
				{
					string fullName;
					if (x == null)
					{
						return;
					}
					this.lineno = x.GetFileLineNumber();
					this.colno = x.GetFileColumnNumber();
					if (this.lineno == 0)
					{
						this.lineno = x.GetILOffset();
					}
					MethodBase method = x.GetMethod();
					if (method == null)
					{
						this.module = "(unknown)";
						this.function = "(unknown)";
						this.context_line = "(unknown)";
					}
					else
					{
						if (method.DeclaringType != null)
						{
							fullName = method.DeclaringType.FullName;
						}
						else
						{
							fullName = null;
						}
						this.module = fullName;
						this.function = method.Name;
						this.context_line = method.ToString();
					}
					this.filename = x.GetFileName();
					this.in_app = !Report.StackTrace.StackFrame.IsSystemModuleName(this.module);
				}

				public StackFrame(string x)
				{
					this.function = x;
				}

				private static bool IsSystemModuleName(string moduleName)
				{
					if (string.IsNullOrEmpty(moduleName))
					{
						return false;
					}
					if (moduleName.StartsWith("System.", StringComparison.Ordinal))
					{
						return true;
					}
					return moduleName.StartsWith("Microsoft.", StringComparison.Ordinal);
				}
			}
		}
	}
}