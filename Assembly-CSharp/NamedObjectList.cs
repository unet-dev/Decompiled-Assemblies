using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Named Object List")]
public class NamedObjectList : ScriptableObject
{
	public NamedObjectList.NamedObject[] objects;

	public NamedObjectList()
	{
	}

	[Serializable]
	public struct NamedObject
	{
		public string name;

		public UnityEngine.Object obj;
	}
}