using System;

namespace Mono.Unix.Native
{
	internal class XPrintfFunctions
	{
		internal static XPrintfFunctions.XPrintf printf;

		internal static XPrintfFunctions.XPrintf fprintf;

		internal static XPrintfFunctions.XPrintf snprintf;

		internal static XPrintfFunctions.XPrintf syslog;

		static XPrintfFunctions()
		{
			CdeclFunction cdeclFunction = new CdeclFunction("msvcrt", "printf", typeof(int));
			XPrintfFunctions.printf = new XPrintfFunctions.XPrintf(cdeclFunction.Invoke);
			CdeclFunction cdeclFunction1 = new CdeclFunction("msvcrt", "fprintf", typeof(int));
			XPrintfFunctions.fprintf = new XPrintfFunctions.XPrintf(cdeclFunction1.Invoke);
			CdeclFunction cdeclFunction2 = new CdeclFunction("MonoPosixHelper", "Mono_Posix_Stdlib_snprintf", typeof(int));
			XPrintfFunctions.snprintf = new XPrintfFunctions.XPrintf(cdeclFunction2.Invoke);
			CdeclFunction cdeclFunction3 = new CdeclFunction("MonoPosixHelper", "Mono_Posix_Stdlib_syslog2", typeof(int));
			XPrintfFunctions.syslog = new XPrintfFunctions.XPrintf(cdeclFunction3.Invoke);
		}

		public XPrintfFunctions()
		{
		}

		internal delegate object XPrintf(object[] parameters);
	}
}