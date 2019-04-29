using System;

public class SocketMod : PrefabAttribute
{
	[NonSerialized]
	public Socket_Base baseSocket;

	public Translate.Phrase FailedPhrase;

	public SocketMod()
	{
	}

	public virtual bool DoCheck(Construction.Placement place)
	{
		return false;
	}

	protected override Type GetIndexedType()
	{
		return typeof(SocketMod);
	}

	public virtual void ModifyPlacement(Construction.Placement place)
	{
	}
}