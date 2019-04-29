using System;

public class UIDialog : ListComponent<UIDialog>
{
	public static bool isOpen
	{
		get
		{
			return ListComponent<UIDialog>.InstanceList.Count > 0;
		}
	}

	public UIDialog()
	{
	}
}