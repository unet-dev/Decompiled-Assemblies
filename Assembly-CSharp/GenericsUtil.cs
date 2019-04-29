using System;

public static class GenericsUtil
{
	public static TDst Cast<TSrc, TDst>(TSrc obj)
	{
		GenericsUtil.CastImpl<TSrc, TDst>.Value = obj;
		return GenericsUtil.CastImpl<TDst, TSrc>.Value;
	}

	public static void Swap<T>(ref T a, ref T b)
	{
		T t = a;
		a = b;
		b = t;
	}

	private static class CastImpl<TSrc, TDst>
	{
		[ThreadStatic]
		public static TSrc Value;

		static CastImpl()
		{
			if (typeof(TSrc) != typeof(TDst))
			{
				throw new InvalidCastException();
			}
		}
	}
}