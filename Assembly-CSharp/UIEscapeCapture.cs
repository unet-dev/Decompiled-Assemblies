using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class UIEscapeCapture : ListComponent<UIEscapeCapture>
{
	public UnityEvent onEscape;

	public UIEscapeCapture()
	{
	}

	public static bool EscapePressed()
	{
		bool flag;
		using (IEnumerator<UIEscapeCapture> enumerator = ListComponent<UIEscapeCapture>.InstanceList.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				enumerator.Current.onEscape.Invoke();
				flag = true;
			}
			else
			{
				return false;
			}
		}
		return flag;
	}
}