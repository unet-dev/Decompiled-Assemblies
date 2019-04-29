using System;
using System.Reflection;

namespace GameTips
{
	public abstract class BaseTip
	{
		public abstract bool ShouldShow
		{
			get;
		}

		public string Type
		{
			get
			{
				return this.GetType().Name;
			}
		}

		protected BaseTip()
		{
		}

		public abstract Translate.Phrase GetPhrase();
	}
}