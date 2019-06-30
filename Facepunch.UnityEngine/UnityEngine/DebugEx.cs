using System;

namespace UnityEngine
{
	public static class DebugEx
	{
		public static void Log(object message, StackTraceLogType stacktrace = 0)
		{
			StackTraceLogType stackTraceLogType = Application.GetStackTraceLogType(LogType.Log);
			Application.SetStackTraceLogType(LogType.Log, stacktrace);
			Debug.Log(message);
			Application.SetStackTraceLogType(LogType.Log, stackTraceLogType);
		}

		public static void Log(object message, UnityEngine.Object context, StackTraceLogType stacktrace = 0)
		{
			StackTraceLogType stackTraceLogType = Application.GetStackTraceLogType(LogType.Log);
			Application.SetStackTraceLogType(LogType.Log, stacktrace);
			Debug.Log(message, context);
			Application.SetStackTraceLogType(LogType.Log, stackTraceLogType);
		}

		public static void LogWarning(object message, StackTraceLogType stacktrace = 0)
		{
			StackTraceLogType stackTraceLogType = Application.GetStackTraceLogType(LogType.Log);
			Application.SetStackTraceLogType(LogType.Log, stacktrace);
			Debug.LogWarning(message);
			Application.SetStackTraceLogType(LogType.Log, stackTraceLogType);
		}

		public static void LogWarning(object message, UnityEngine.Object context, StackTraceLogType stacktrace = 0)
		{
			StackTraceLogType stackTraceLogType = Application.GetStackTraceLogType(LogType.Log);
			Application.SetStackTraceLogType(LogType.Log, stacktrace);
			Debug.LogWarning(message, context);
			Application.SetStackTraceLogType(LogType.Log, stackTraceLogType);
		}
	}
}