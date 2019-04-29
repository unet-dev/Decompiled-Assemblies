using System;

namespace Apex.Utilities
{
	public static class Ensure
	{
		public static void ArgumentInRange(Func<bool> check, string variableName, object value, string message = null)
		{
			if (!check())
			{
				throw new ArgumentException(message, variableName);
			}
		}

		public static void ArgumentNotNull([ValidatedNotNull] object value, string variableName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(variableName);
			}
		}

		public static void ArgumentNotNullOrEmpty([ValidatedNotNull] string value, string variableName)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("Cannot be null or empty!", variableName);
			}
		}

		private sealed class ValidatedNotNullAttribute : Attribute
		{
			public ValidatedNotNullAttribute()
			{
			}
		}
	}
}