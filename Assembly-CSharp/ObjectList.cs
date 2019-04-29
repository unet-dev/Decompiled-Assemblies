using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Object List")]
public class ObjectList : ScriptableObject
{
	public UnityEngine.Object[] objects;

	public ObjectList()
	{
	}
}