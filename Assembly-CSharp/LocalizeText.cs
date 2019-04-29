using System;
using UnityEngine;

public class LocalizeText : MonoBehaviour, IClientComponent, ILanguageChanged
{
	public string token;

	[TextArea]
	public string english;

	public string append;

	public LocalizeText.SpecialMode specialMode;

	public LocalizeText()
	{
	}

	public enum SpecialMode
	{
		None,
		AllUppercase,
		AllLowercase
	}
}