using System;
using System.Collections.Generic;
using UnityEngine;

public class GameContentList : MonoBehaviour
{
	public GameContentList.ResourceType resourceType;

	public List<UnityEngine.Object> foundObjects;

	public GameContentList()
	{
	}

	public enum ResourceType
	{
		Audio,
		Textures,
		Models
	}
}