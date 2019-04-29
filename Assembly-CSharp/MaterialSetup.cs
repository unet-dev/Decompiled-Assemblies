using System;
using UnityEngine;

public class MaterialSetup : MonoBehaviour, IClientComponent
{
	public bool destroy = true;

	public MaterialConfig config;

	public MaterialSetup()
	{
	}
}