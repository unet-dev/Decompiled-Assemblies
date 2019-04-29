using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChildrenFromScene : MonoBehaviour
{
	public string SceneName;

	public bool StartChildrenDisabled;

	public ChildrenFromScene()
	{
	}

	private IEnumerator Start()
	{
		ChildrenFromScene childrenFromScene = null;
		Scene sceneByName = SceneManager.GetSceneByName(childrenFromScene.SceneName);
		if (!sceneByName.isLoaded)
		{
			yield return SceneManager.LoadSceneAsync(childrenFromScene.SceneName, LoadSceneMode.Additive);
		}
		sceneByName = SceneManager.GetSceneByName(childrenFromScene.SceneName);
		GameObject[] rootGameObjects = sceneByName.GetRootGameObjects();
		for (int i = 0; i < (int)rootGameObjects.Length; i++)
		{
			GameObject gameObject = rootGameObjects[i];
			gameObject.transform.SetParent(childrenFromScene.transform, false);
			gameObject.Identity();
			RectTransform rectTransform = gameObject.transform as RectTransform;
			if (rectTransform)
			{
				rectTransform.pivot = Vector2.zero;
				rectTransform.anchoredPosition = Vector2.zero;
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.sizeDelta = Vector2.one;
			}
			SingletonComponent[] componentsInChildren = gameObject.GetComponentsInChildren<SingletonComponent>(true);
			for (int j = 0; j < (int)componentsInChildren.Length; j++)
			{
				componentsInChildren[j].Setup();
			}
			if (childrenFromScene.StartChildrenDisabled)
			{
				gameObject.SetActive(false);
			}
		}
		SceneManager.UnloadSceneAsync(sceneByName);
	}
}