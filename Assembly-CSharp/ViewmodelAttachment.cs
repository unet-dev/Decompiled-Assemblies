using System;

public class ViewmodelAttachment : EntityComponent<BaseEntity>, IClientComponent, IViewModeChanged, IViewModelUpdated
{
	public GameObjectRef modelObject;

	public string targetBone;

	public bool hideViewModelIronSights;

	public ViewmodelAttachment()
	{
	}
}