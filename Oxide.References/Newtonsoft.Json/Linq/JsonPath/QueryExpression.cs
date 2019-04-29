using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal abstract class QueryExpression
	{
		public QueryOperator Operator
		{
			get;
			set;
		}

		protected QueryExpression()
		{
		}

		public abstract bool IsMatch(JToken t);
	}
}