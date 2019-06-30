using Facepunch;
using System;
using UnityEngine;

public static class Translate
{
	public static string TranslateMouseButton(string mouseButton)
	{
		if (mouseButton == "mouse0")
		{
			return "Left Mouse";
		}
		if (mouseButton == "mouse1")
		{
			return "Right Mouse";
		}
		if (mouseButton == "mouse2")
		{
			return "Center Mouse";
		}
		return mouseButton;
	}

	[Serializable]
	public class Phrase
	{
		public string token;

		[TextArea]
		public string english;

		public virtual string translated
		{
			get
			{
				string.IsNullOrEmpty(this.token);
				return this.english;
			}
		}

		public Phrase(string t = "", string eng = "")
		{
			this.token = t;
			this.english = eng;
		}

		internal string GetWithParam(string param)
		{
			return string.Format(this.translated, param);
		}

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(this.token);
		}
	}

	[Serializable]
	public class TokenisedPhrase : Translate.Phrase
	{
		public override string translated
		{
			get
			{
				return base.translated.Replace("[inventory.toggle]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("inventory.toggle").ToUpper())).Replace("[inventory.togglecrafting]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("inventory.togglecrafting").ToUpper())).Replace("[+map]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("+map").ToUpper())).Replace("[inventory.examineheld]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("inventory.examineheld").ToUpper())).Replace("[slot2]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("+slot2").ToUpper())).Replace("[attack]", string.Format("[{0}]", Translate.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+attack")).ToUpper())).Replace("[attack2]", string.Format("[{0}]", Translate.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+attack2")).ToUpper())).Replace("[+use]", string.Format("[{0}]", Translate.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+use")).ToUpper())).Replace("[+altlook]", string.Format("[{0}]", Translate.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+altlook")).ToUpper())).Replace("[+reload]", string.Format("[{0}]", Translate.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+reload")).ToUpper())).Replace("[+voice]", string.Format("[{0}]", Translate.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+voice")).ToUpper()));
			}
		}

		public TokenisedPhrase(string t = "", string eng = "") : base(t, eng)
		{
		}
	}
}