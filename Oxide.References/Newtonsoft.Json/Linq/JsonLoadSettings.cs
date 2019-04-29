using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public class JsonLoadSettings
	{
		private Newtonsoft.Json.Linq.CommentHandling _commentHandling;

		private Newtonsoft.Json.Linq.LineInfoHandling _lineInfoHandling;

		public Newtonsoft.Json.Linq.CommentHandling CommentHandling
		{
			get
			{
				return this._commentHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.Linq.CommentHandling.Ignore || value > Newtonsoft.Json.Linq.CommentHandling.Load)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._commentHandling = value;
			}
		}

		public Newtonsoft.Json.Linq.LineInfoHandling LineInfoHandling
		{
			get
			{
				return this._lineInfoHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.Linq.LineInfoHandling.Ignore || value > Newtonsoft.Json.Linq.LineInfoHandling.Load)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._lineInfoHandling = value;
			}
		}

		public JsonLoadSettings()
		{
		}
	}
}