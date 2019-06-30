using System;
using UnityEngine;

public class Achievements : SingletonComponent<Achievements>
{
	public SoundDefinition listComplete;

	public SoundDefinition itemComplete;

	public SoundDefinition popup;

	public UnityEngine.Canvas Canvas;

	public Achievements()
	{
	}
}