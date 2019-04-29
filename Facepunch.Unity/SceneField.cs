using System;
using UnityEngine;

[Serializable]
public class SceneField
{
	[SerializeField]
	private UnityEngine.Object sceneAsset;

	[SerializeField]
	private string sceneName = "";

	public string SceneName
	{
		get
		{
			return this.sceneName;
		}
	}

	public SceneField()
	{
	}

	public static implicit operator String(SceneField sceneField)
	{
		return sceneField.SceneName;
	}
}