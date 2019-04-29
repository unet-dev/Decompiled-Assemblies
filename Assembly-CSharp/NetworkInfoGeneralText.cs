using Network;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NetworkInfoGeneralText : MonoBehaviour
{
	public Text text;

	public NetworkInfoGeneralText()
	{
	}

	private static string ChannelStat(int window, int left)
	{
		return string.Format("{0}/{1}", left, window);
	}

	private void Update()
	{
		this.UpdateText();
	}

	private void UpdateText()
	{
		string str = "";
		if (Net.sv != null)
		{
			str = string.Concat(str, "Server\n");
			str = string.Concat(str, Net.sv.GetDebug(null));
			str = string.Concat(str, "\n");
		}
		this.text.text = str;
	}
}