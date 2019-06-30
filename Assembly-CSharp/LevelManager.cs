using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelManager
{
	public static string CurrentLevelName;

	public static bool isLoaded
	{
		get
		{
			if (LevelManager.CurrentLevelName == null)
			{
				return false;
			}
			if (LevelManager.CurrentLevelName == "")
			{
				return false;
			}
			if (LevelManager.CurrentLevelName == "Empty")
			{
				return false;
			}
			if (LevelManager.CurrentLevelName == "MenuBackground")
			{
				return false;
			}
			return true;
		}
	}

	public static bool IsValid(string strName)
	{
		return Application.CanStreamedLevelBeLoaded(strName);
	}

	public static void LoadLevel(string strName, bool keepLoadingScreenOpen = true)
	{
		if (strName == "proceduralmap")
		{
			strName = "Procedural Map";
		}
		LevelManager.CurrentLevelName = strName;
		Net.sv.Reset();
		SceneManager.LoadScene(strName, LoadSceneMode.Single);
	}

	public static IEnumerator LoadLevelAsync(string strName, bool keepLoadingScreenOpen = true)
	{
		LevelManager.CurrentLevelName = strName;
		Net.sv.Reset();
		yield return null;
		yield return SceneManager.LoadSceneAsync(strName, LoadSceneMode.Single);
		yield return null;
		yield return null;
	}

	public static void UnloadLevel()
	{
		LevelManager.CurrentLevelName = null;
		SceneManager.LoadScene("Empty");
	}
}