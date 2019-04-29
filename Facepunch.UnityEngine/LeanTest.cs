using System;
using UnityEngine;

public class LeanTest
{
	public static int expected;

	private static int tests;

	private static int passes;

	public static float timeout;

	public static bool timeoutStarted;

	public static bool testsFinished;

	static LeanTest()
	{
		LeanTest.expected = 0;
		LeanTest.tests = 0;
		LeanTest.passes = 0;
		LeanTest.timeout = 15f;
		LeanTest.timeoutStarted = false;
		LeanTest.testsFinished = false;
	}

	public LeanTest()
	{
	}

	public static void debug(string name, bool didPass, string failExplaination = null)
	{
		LeanTest.expect(didPass, name, failExplaination);
	}

	public static void expect(bool didPass, string definition, string failExplaination = null)
	{
		float single = LeanTest.printOutLength(definition);
		int num = 40 - (int)(single * 1.05f);
		string str = "".PadRight(num, "_"[0]);
		string[] strArrays = new string[] { LeanTest.formatB(definition), " ", str, " [ ", null, null };
		strArrays[4] = (didPass ? LeanTest.formatC("pass", "green") : LeanTest.formatC("fail", "red"));
		strArrays[5] = " ]";
		string str1 = string.Concat(strArrays);
		if (!didPass && failExplaination != null)
		{
			str1 = string.Concat(str1, " - ", failExplaination);
		}
		Debug.Log(str1);
		if (didPass)
		{
			LeanTest.passes++;
		}
		LeanTest.tests++;
		if (LeanTest.tests == LeanTest.expected && !LeanTest.testsFinished)
		{
			LeanTest.overview();
		}
		else if (LeanTest.tests > LeanTest.expected)
		{
			Debug.Log(string.Concat(LeanTest.formatB("Too many tests for a final report!"), " set LeanTest.expected = ", LeanTest.tests));
		}
		if (!LeanTest.timeoutStarted)
		{
			LeanTest.timeoutStarted = true;
			GameObject gameObject = new GameObject()
			{
				name = "~LeanTest"
			};
			(gameObject.AddComponent(typeof(LeanTester)) as LeanTester).timeout = LeanTest.timeout;
			gameObject.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	public static string formatB(string str)
	{
		return string.Concat("<b>", str, "</b>");
	}

	public static string formatBC(string str, string color)
	{
		return LeanTest.formatC(LeanTest.formatB(str), color);
	}

	public static string formatC(string str, string color)
	{
		return string.Concat(new string[] { "<color=", color, ">", str, "</color>" });
	}

	public static void overview()
	{
		LeanTest.testsFinished = true;
		int num = LeanTest.expected - LeanTest.passes;
		string str = (num > 0 ? LeanTest.formatBC(string.Concat(num), "red") : string.Concat(num));
		Debug.Log(string.Concat(new string[] { LeanTest.formatB("Final Report:"), " _____________________ PASSED: ", LeanTest.formatBC(string.Concat(LeanTest.passes), "green"), " FAILED: ", str, " " }));
	}

	public static string padRight(int len)
	{
		string str = "";
		for (int i = 0; i < len; i++)
		{
			str = string.Concat(str, "_");
		}
		return str;
	}

	public static float printOutLength(string str)
	{
		float single = 0f;
		for (int i = 0; i < str.Length; i++)
		{
			if (str[i] == "I"[0])
			{
				single += 0.5f;
			}
			else if (str[i] != "J"[0])
			{
				single += 1f;
			}
			else
			{
				single += 0.85f;
			}
		}
		return single;
	}
}