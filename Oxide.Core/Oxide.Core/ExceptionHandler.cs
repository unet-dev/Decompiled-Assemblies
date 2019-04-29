using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	public class ExceptionHandler
	{
		private readonly static Dictionary<Type, Func<Exception, string>> Handlers;

		static ExceptionHandler()
		{
			ExceptionHandler.Handlers = new Dictionary<Type, Func<Exception, string>>();
		}

		public ExceptionHandler()
		{
		}

		public static string FormatException(Exception ex)
		{
			Func<Exception, string> func;
			if (!ExceptionHandler.Handlers.TryGetValue(ex.GetType(), out func))
			{
				return null;
			}
			return func(ex);
		}

		public static void RegisterType(Type ex, Func<Exception, string> handler)
		{
			ExceptionHandler.Handlers[ex] = handler;
		}
	}
}