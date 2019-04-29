using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OneActiveSibling : MonoBehaviour
{
	public OneActiveSibling()
	{
	}

	[ComponentHelp("This component will disable all of its siblings when it becomes enabled. This can be useful in situations where you only ever want one of the children active - but don't want to manage turning each one off.")]
	private void OnEnable()
	{
		foreach (Transform sibling in base.transform.GetSiblings<Transform>(false))
		{
			sibling.gameObject.SetActive(false);
		}
	}
}