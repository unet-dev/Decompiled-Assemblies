using System;

public class SoundManager : SingletonComponent<SoundManager>, IClientComponent
{
	public SoundClass defaultSoundClass;

	public SoundManager()
	{
	}
}