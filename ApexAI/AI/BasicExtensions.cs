using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	public static class BasicExtensions
	{
		internal static IEnumerable<IQualifier> AllQualifiers(Selector source)
		{
			List<IQualifier> qualifiers = source.qualifiers;
			int num = qualifiers.Count;
			for (int i = 0; i < num; i++)
			{
				yield return qualifiers[i];
			}
			yield return source.defaultQualifier;
		}

		public static bool ExecuteOnce(this IUtilityAI ai, IAIContext context)
		{
			IAction action = ai.Select(context);
			bool flag = false;
			bool flag1 = false;
			while (!flag)
			{
				ICompositeAction compositeAction = action as ICompositeAction;
				if (compositeAction != null)
				{
					action.Execute(context);
					action = compositeAction.Select(context);
					flag = action == null;
					flag1 = true;
				}
				else
				{
					IConnectorAction connectorAction = action as IConnectorAction;
					if (connectorAction != null)
					{
						action = connectorAction.Select(context);
					}
					else
					{
						flag = true;
					}
				}
			}
			if (action != null)
			{
				action.Execute(context);
				flag1 = true;
			}
			return flag1;
		}

		internal static bool IsConnectedTo(this Selector source, Selector target)
		{
			SelectorAction selectorAction;
			bool flag;
			using (IEnumerator<IQualifier> enumerator = BasicExtensions.AllQualifiers(source).GetEnumerator())
			{
				do
				{
				Label1:
					if (enumerator.MoveNext())
					{
						IQualifier current = enumerator.Current;
						selectorAction = current.action as SelectorAction;
						if (selectorAction != null)
						{
							break;
						}
						CompositeAction compositeAction = current.action as CompositeAction;
						if (compositeAction != null)
						{
							selectorAction = compositeAction.connectorAction as SelectorAction;
						}
						else
						{
							goto Label1;
						}
					}
					else
					{
						return false;
					}
				}
				while (selectorAction == null);
				flag = (selectorAction.selector != target ? selectorAction.selector.IsConnectedTo(target) : true);
			}
			return flag;
		}
	}
}