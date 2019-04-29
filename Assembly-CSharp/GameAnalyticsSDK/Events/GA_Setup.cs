using GameAnalyticsSDK;
using GameAnalyticsSDK.Utilities;
using GameAnalyticsSDK.Validators;
using GameAnalyticsSDK.Wrapper;
using System;
using System.Collections.Generic;

namespace GameAnalyticsSDK.Events
{
	public static class GA_Setup
	{
		public static void SetAvailableCustomDimensions01(List<string> customDimensions)
		{
			if (GAValidator.ValidateCustomDimensions(customDimensions.ToArray()))
			{
				GA_Wrapper.SetAvailableCustomDimensions01(GA_MiniJSON.Serialize(customDimensions));
			}
		}

		public static void SetAvailableCustomDimensions02(List<string> customDimensions)
		{
			if (GAValidator.ValidateCustomDimensions(customDimensions.ToArray()))
			{
				GA_Wrapper.SetAvailableCustomDimensions02(GA_MiniJSON.Serialize(customDimensions));
			}
		}

		public static void SetAvailableCustomDimensions03(List<string> customDimensions)
		{
			if (GAValidator.ValidateCustomDimensions(customDimensions.ToArray()))
			{
				GA_Wrapper.SetAvailableCustomDimensions03(GA_MiniJSON.Serialize(customDimensions));
			}
		}

		public static void SetAvailableResourceCurrencies(List<string> resourceCurrencies)
		{
			if (GAValidator.ValidateResourceCurrencies(resourceCurrencies.ToArray()))
			{
				GA_Wrapper.SetAvailableResourceCurrencies(GA_MiniJSON.Serialize(resourceCurrencies));
			}
		}

		public static void SetAvailableResourceItemTypes(List<string> resourceItemTypes)
		{
			if (GAValidator.ValidateResourceItemTypes(resourceItemTypes.ToArray()))
			{
				GA_Wrapper.SetAvailableResourceItemTypes(GA_MiniJSON.Serialize(resourceItemTypes));
			}
		}

		public static void SetBirthYear(int birthYear)
		{
			GA_Wrapper.SetBirthYear(birthYear);
		}

		public static void SetCustomDimension01(string customDimension)
		{
			GA_Wrapper.SetCustomDimension01(customDimension);
		}

		public static void SetCustomDimension02(string customDimension)
		{
			GA_Wrapper.SetCustomDimension02(customDimension);
		}

		public static void SetCustomDimension03(string customDimension)
		{
			GA_Wrapper.SetCustomDimension03(customDimension);
		}

		public static void SetFacebookId(string facebookId)
		{
			GA_Wrapper.SetFacebookId(facebookId);
		}

		public static void SetGender(GAGender gender)
		{
			if (gender == GAGender.male)
			{
				GA_Wrapper.SetGender(GAGender.male.ToString());
				return;
			}
			if (gender != GAGender.female)
			{
				return;
			}
			GA_Wrapper.SetGender(GAGender.female.ToString());
		}

		public static void SetInfoLog(bool enabled)
		{
			GA_Wrapper.SetInfoLog(enabled);
		}

		public static void SetVerboseLog(bool enabled)
		{
			GA_Wrapper.SetVerboseLog(enabled);
		}
	}
}