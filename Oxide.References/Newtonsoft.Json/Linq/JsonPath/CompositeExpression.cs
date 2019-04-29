using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal class CompositeExpression : QueryExpression
	{
		public List<QueryExpression> Expressions
		{
			get;
			set;
		}

		public CompositeExpression()
		{
			this.Expressions = new List<QueryExpression>();
		}

		public override bool IsMatch(JToken t)
		{
			bool flag;
			QueryOperator @operator = base.Operator;
			if (@operator == QueryOperator.And)
			{
				foreach (QueryExpression expression in this.Expressions)
				{
					if (expression.IsMatch(t))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			else
			{
				if (@operator != QueryOperator.Or)
				{
					throw new ArgumentOutOfRangeException();
				}
				foreach (QueryExpression queryExpression in this.Expressions)
				{
					if (!queryExpression.IsMatch(t))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}
	}
}