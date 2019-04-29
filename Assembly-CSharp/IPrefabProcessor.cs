using System;
using UnityEngine;

public interface IPrefabProcessor
{
	void NominateForDeletion(GameObject obj);

	void RemoveComponent(Component component);
}