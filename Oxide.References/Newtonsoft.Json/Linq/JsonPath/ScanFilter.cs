using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal class ScanFilter : PathFilter
	{
		public string Name
		{
			get;
			set;
		}

		public ScanFilter()
		{
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken jTokens in current)
			{
				if (this.Name == null)
				{
					yield return jTokens;
				}
				JToken parent = jTokens;
				JToken jTokens1 = jTokens;
				while (true)
				{
					if (jTokens1 == null || !jTokens1.HasValues)
					{
						while (parent != null && parent != jTokens && parent == parent.Parent.Last)
						{
							parent = parent.Parent;
						}
						if (parent == null || parent == jTokens)
						{
							break;
						}
						parent = parent.Next;
					}
					else
					{
						parent = jTokens1.First;
					}
					JProperty jProperty = parent as JProperty;
					if (jProperty != null)
					{
						if (jProperty.Name == this.Name)
						{
							yield return jProperty.Value;
						}
					}
					else if (this.Name == null)
					{
						yield return parent;
					}
					jTokens1 = parent as JContainer;
				}
				parent = null;
			}
		}
	}
}