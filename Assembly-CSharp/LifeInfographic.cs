using ProtoBuf;
using System;
using UnityEngine;

public class LifeInfographic : MonoBehaviour
{
	[NonSerialized]
	public PlayerLifeStory life;

	public GameObject container;

	public LifeInfographic()
	{
	}
}