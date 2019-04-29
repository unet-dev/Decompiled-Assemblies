using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIIntegerEntry : MonoBehaviour
{
	public InputField textEntry;

	public UIIntegerEntry()
	{
	}

	public int GetIntAmount()
	{
		int num = 0;
		int.TryParse(this.textEntry.text, out num);
		return num;
	}

	public void OnAmountTextChanged()
	{
		this.textChanged();
	}

	public void PlusMinus(int delta)
	{
		this.SetAmount(this.GetIntAmount() + delta);
	}

	public void SetAmount(int amount)
	{
		if (amount == this.GetIntAmount())
		{
			return;
		}
		this.textEntry.text = amount.ToString();
	}

	public event Action textChanged;
}