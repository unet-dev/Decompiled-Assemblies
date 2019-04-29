using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LeanTester : MonoBehaviour
{
	public float timeout = 15f;

	public LeanTester()
	{
	}

	public void Start()
	{
		base.StartCoroutine(this.timeoutCheck());
	}

	private IEnumerator timeoutCheck()
	{
		LeanTester leanTester = null;
		float single = Time.realtimeSinceStartup + leanTester.timeout;
		while (Time.realtimeSinceStartup < single)
		{
			yield return 0;
		}
		if (!LeanTest.testsFinished)
		{
			UnityEngine.Debug.Log(LeanTest.formatB("Tests timed out!"));
			LeanTest.overview();
		}
	}
}