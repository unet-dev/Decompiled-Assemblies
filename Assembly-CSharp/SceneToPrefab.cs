using System;
using UnityEngine;

public class SceneToPrefab : MonoBehaviour, IEditorComponent
{
	public bool flattenHierarchy;

	public GameObject outputPrefab;

	public SceneToPrefab()
	{
	}
}