using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameAnalyticsSDK.Setup
{
	public class Settings : ScriptableObject
	{
		[HideInInspector]
		public static string VERSION;

		[HideInInspector]
		public static bool CheckingForUpdates;

		public int TotalMessagesSubmitted;

		public int TotalMessagesFailed;

		public int DesignMessagesSubmitted;

		public int DesignMessagesFailed;

		public int QualityMessagesSubmitted;

		public int QualityMessagesFailed;

		public int ErrorMessagesSubmitted;

		public int ErrorMessagesFailed;

		public int BusinessMessagesSubmitted;

		public int BusinessMessagesFailed;

		public int UserMessagesSubmitted;

		public int UserMessagesFailed;

		public string CustomArea = string.Empty;

		[SerializeField]
		private List<string> gameKey = new List<string>();

		[SerializeField]
		private List<string> secretKey = new List<string>();

		[SerializeField]
		public List<string> Build = new List<string>();

		[SerializeField]
		public List<string> SelectedPlatformStudio = new List<string>();

		[SerializeField]
		public List<string> SelectedPlatformGame = new List<string>();

		[SerializeField]
		public List<int> SelectedPlatformGameID = new List<int>();

		[SerializeField]
		public List<int> SelectedStudio = new List<int>();

		[SerializeField]
		public List<int> SelectedGame = new List<int>();

		public string NewVersion = "";

		public string Changes = "";

		public bool SignUpOpen = true;

		public string StudioName = "";

		public string GameName = "";

		public string EmailGA = "";

		[NonSerialized]
		public string PasswordGA = "";

		[NonSerialized]
		public string TokenGA = "";

		[NonSerialized]
		public string ExpireTime = "";

		[NonSerialized]
		public string LoginStatus = "Not logged in.";

		[NonSerialized]
		public bool JustSignedUp;

		[NonSerialized]
		public bool HideSignupWarning;

		public bool IntroScreen = true;

		[NonSerialized]
		public List<Studio> Studios;

		public bool InfoLogEditor = true;

		public bool InfoLogBuild = true;

		public bool VerboseLogBuild;

		public bool UseManualSessionHandling;

		public bool SendExampleGameDataToMyGame;

		public bool InternetConnectivity;

		public List<string> CustomDimensions01 = new List<string>();

		public List<string> CustomDimensions02 = new List<string>();

		public List<string> CustomDimensions03 = new List<string>();

		public List<string> ResourceItemTypes = new List<string>();

		public List<string> ResourceCurrencies = new List<string>();

		public RuntimePlatform LastCreatedGamePlatform;

		public List<RuntimePlatform> Platforms = new List<RuntimePlatform>();

		public Settings.InspectorStates CurrentInspectorState;

		public List<Settings.HelpTypes> ClosedHints = new List<Settings.HelpTypes>();

		public bool DisplayHints;

		public Vector2 DisplayHintsScrollState;

		public Texture2D Logo;

		public Texture2D UpdateIcon;

		public Texture2D InfoIcon;

		public Texture2D DeleteIcon;

		public Texture2D GameIcon;

		public Texture2D HomeIcon;

		public Texture2D InstrumentIcon;

		public Texture2D QuestionIcon;

		public Texture2D UserIcon;

		public Texture2D AmazonIcon;

		public Texture2D GooglePlayIcon;

		public Texture2D iosIcon;

		public Texture2D macIcon;

		public Texture2D windowsPhoneIcon;

		[NonSerialized]
		public GUIStyle SignupButton;

		public bool UsePlayerSettingsBuildNumber;

		public bool SubmitErrors = true;

		public int MaxErrorCount = 10;

		public bool SubmitFpsAverage = true;

		public bool SubmitFpsCritical = true;

		public bool IncludeGooglePlay = true;

		public int FpsCriticalThreshold = 20;

		public int FpsCirticalSubmitInterval = 1;

		public List<bool> PlatformFoldOut = new List<bool>();

		public bool CustomDimensions01FoldOut;

		public bool CustomDimensions02FoldOut;

		public bool CustomDimensions03FoldOut;

		public bool ResourceItemTypesFoldOut;

		public bool ResourceCurrenciesFoldOut;

		public readonly static RuntimePlatform[] AvailablePlatforms;

		static Settings()
		{
			Settings.VERSION = "5.0.5";
			Settings.CheckingForUpdates = false;
			Settings.AvailablePlatforms = new RuntimePlatform[] { typeof(<PrivateImplementationDetails>).GetField("6E5E6830F0A82B8603B122C02111F18D3639978F").FieldHandle };
		}

		public Settings()
		{
		}

		public void AddPlatform(RuntimePlatform platform)
		{
			this.gameKey.Add("");
			this.secretKey.Add("");
			this.Build.Add("0.1");
			this.SelectedPlatformStudio.Add("");
			this.SelectedPlatformGame.Add("");
			this.SelectedPlatformGameID.Add(-1);
			this.SelectedStudio.Add(0);
			this.SelectedGame.Add(0);
			this.PlatformFoldOut.Add(true);
			this.Platforms.Add(platform);
		}

		public string[] GetAvailablePlatforms()
		{
			List<string> strs = new List<string>();
			for (int i = 0; i < (int)Settings.AvailablePlatforms.Length; i++)
			{
				RuntimePlatform availablePlatforms = Settings.AvailablePlatforms[i];
				if (availablePlatforms == RuntimePlatform.IPhonePlayer)
				{
					if (!this.Platforms.Contains(RuntimePlatform.tvOS) && !this.Platforms.Contains(availablePlatforms))
					{
						strs.Add(availablePlatforms.ToString());
					}
					else if (!this.Platforms.Contains(availablePlatforms))
					{
						strs.Add(availablePlatforms.ToString());
					}
				}
				else if (availablePlatforms == RuntimePlatform.tvOS)
				{
					if (!this.Platforms.Contains(RuntimePlatform.IPhonePlayer) && !this.Platforms.Contains(availablePlatforms))
					{
						strs.Add(availablePlatforms.ToString());
					}
					else if (!this.Platforms.Contains(availablePlatforms))
					{
						strs.Add(availablePlatforms.ToString());
					}
				}
				else if (availablePlatforms == RuntimePlatform.MetroPlayerARM)
				{
					if (!this.Platforms.Contains(availablePlatforms))
					{
						strs.Add("WSA");
					}
				}
				else if (!this.Platforms.Contains(availablePlatforms))
				{
					strs.Add(availablePlatforms.ToString());
				}
			}
			return strs.ToArray();
		}

		public string GetGameKey(int index)
		{
			return this.gameKey[index];
		}

		public string GetSecretKey(int index)
		{
			return this.secretKey[index];
		}

		public bool IsGameKeyValid(int index, string value)
		{
			bool flag = true;
			int num = 0;
			while (num < this.Platforms.Count)
			{
				if (index == num || !value.Equals(this.gameKey[num]))
				{
					num++;
				}
				else
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		public bool IsSecretKeyValid(int index, string value)
		{
			bool flag = true;
			int num = 0;
			while (num < this.Platforms.Count)
			{
				if (index == num || !value.Equals(this.secretKey[num]))
				{
					num++;
				}
				else
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		public void RemovePlatformAtIndex(int index)
		{
			if (index >= 0 && index < this.Platforms.Count)
			{
				this.gameKey.RemoveAt(index);
				this.secretKey.RemoveAt(index);
				this.Build.RemoveAt(index);
				this.SelectedPlatformStudio.RemoveAt(index);
				this.SelectedPlatformGame.RemoveAt(index);
				this.SelectedPlatformGameID.RemoveAt(index);
				this.SelectedStudio.RemoveAt(index);
				this.SelectedGame.RemoveAt(index);
				this.PlatformFoldOut.RemoveAt(index);
				this.Platforms.RemoveAt(index);
			}
		}

		public void SetCustomArea(string customArea)
		{
		}

		public void SetCustomUserID(string customID)
		{
			bool flag = customID != string.Empty;
		}

		public void SetKeys(string gamekey, string secretkey)
		{
		}

		public void UpdateGameKey(int index, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this.gameKey[index] = value;
			}
			else
			{
				if (this.IsGameKeyValid(index, value))
				{
					this.gameKey[index] = value;
					return;
				}
				if (this.gameKey[index].Equals(value))
				{
					this.gameKey[index] = "";
					return;
				}
			}
		}

		public void UpdateSecretKey(int index, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this.secretKey[index] = value;
			}
			else
			{
				if (this.IsSecretKeyValid(index, value))
				{
					this.secretKey[index] = value;
					return;
				}
				if (this.secretKey[index].Equals(value))
				{
					this.secretKey[index] = "";
					return;
				}
			}
		}

		public struct HelpInfo
		{
			public string Message;

			public Settings.MessageTypes MsgType;

			public Settings.HelpTypes HelpType;
		}

		public enum HelpTypes
		{
			None,
			IncludeSystemSpecsHelp,
			ProvideCustomUserID
		}

		public enum InspectorStates
		{
			Account,
			Basic,
			Debugging,
			Pref
		}

		public enum MessageTypes
		{
			None,
			Error,
			Info,
			Warning
		}
	}
}