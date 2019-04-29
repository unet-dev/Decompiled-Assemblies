using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/MaterialSound")]
public class MaterialSound : ScriptableObject
{
	public SoundDefinition DefaultSound;

	public MaterialSound.Entry[] Entries;

	public MaterialSound()
	{
	}

	[Serializable]
	public class Entry
	{
		public PhysicMaterial Material;

		public SoundDefinition Sound;

		public Entry()
		{
		}
	}
}