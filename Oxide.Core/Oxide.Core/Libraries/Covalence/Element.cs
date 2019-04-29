using System;
using System.Collections.Generic;

namespace Oxide.Core.Libraries.Covalence
{
	public class Element
	{
		public ElementType Type;

		public object Val;

		public List<Element> Body = new List<Element>();

		private Element(ElementType type, object val)
		{
			this.Type = type;
			this.Val = val;
		}

		public static Element ParamTag(ElementType type, object val)
		{
			return new Element(type, val);
		}

		public static Element String(object s)
		{
			return new Element(ElementType.String, s);
		}

		public static Element Tag(ElementType type)
		{
			return new Element(type, null);
		}
	}
}