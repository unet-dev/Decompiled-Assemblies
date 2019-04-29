using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal class SteamController : IDisposable
	{
		internal Platform.Interface platform;

		internal BaseSteamworks steamworks;

		public bool IsValid
		{
			get
			{
				if (this.platform == null)
				{
					return false;
				}
				return this.platform.IsValid;
			}
		}

		internal SteamController(BaseSteamworks steamworks, IntPtr pointer)
		{
			this.steamworks = steamworks;
			if (Platform.IsWindows64)
			{
				this.platform = new Platform.Win64(pointer);
				return;
			}
			if (Platform.IsWindows32)
			{
				this.platform = new Platform.Win32(pointer);
				return;
			}
			if (Platform.IsLinux32)
			{
				this.platform = new Platform.Linux32(pointer);
				return;
			}
			if (Platform.IsLinux64)
			{
				this.platform = new Platform.Linux64(pointer);
				return;
			}
			if (Platform.IsOsx)
			{
				this.platform = new Platform.Mac(pointer);
			}
		}

		public void ActivateActionSet(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle)
		{
			this.platform.ISteamController_ActivateActionSet(controllerHandle.Value, actionSetHandle.Value);
		}

		public void ActivateActionSetLayer(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle)
		{
			this.platform.ISteamController_ActivateActionSetLayer(controllerHandle.Value, actionSetLayerHandle.Value);
		}

		public void DeactivateActionSetLayer(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle)
		{
			this.platform.ISteamController_DeactivateActionSetLayer(controllerHandle.Value, actionSetLayerHandle.Value);
		}

		public void DeactivateAllActionSetLayers(ControllerHandle_t controllerHandle)
		{
			this.platform.ISteamController_DeactivateAllActionSetLayers(controllerHandle.Value);
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public ControllerActionSetHandle_t GetActionSetHandle(string pszActionSetName)
		{
			return this.platform.ISteamController_GetActionSetHandle(pszActionSetName);
		}

		public int GetActiveActionSetLayers(ControllerHandle_t controllerHandle, IntPtr handlesOut)
		{
			return this.platform.ISteamController_GetActiveActionSetLayers(controllerHandle.Value, handlesOut);
		}

		public ControllerAnalogActionData_t GetAnalogActionData(ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t analogActionHandle)
		{
			return this.platform.ISteamController_GetAnalogActionData(controllerHandle.Value, analogActionHandle.Value);
		}

		public ControllerAnalogActionHandle_t GetAnalogActionHandle(string pszActionName)
		{
			return this.platform.ISteamController_GetAnalogActionHandle(pszActionName);
		}

		public int GetAnalogActionOrigins(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerAnalogActionHandle_t analogActionHandle, out ControllerActionOrigin originsOut)
		{
			return this.platform.ISteamController_GetAnalogActionOrigins(controllerHandle.Value, actionSetHandle.Value, analogActionHandle.Value, out originsOut);
		}

		public int GetConnectedControllers(IntPtr handlesOut)
		{
			return this.platform.ISteamController_GetConnectedControllers(handlesOut);
		}

		public ControllerHandle_t GetControllerForGamepadIndex(int nIndex)
		{
			return this.platform.ISteamController_GetControllerForGamepadIndex(nIndex);
		}

		public ControllerActionSetHandle_t GetCurrentActionSet(ControllerHandle_t controllerHandle)
		{
			return this.platform.ISteamController_GetCurrentActionSet(controllerHandle.Value);
		}

		public ControllerDigitalActionData_t GetDigitalActionData(ControllerHandle_t controllerHandle, ControllerDigitalActionHandle_t digitalActionHandle)
		{
			return this.platform.ISteamController_GetDigitalActionData(controllerHandle.Value, digitalActionHandle.Value);
		}

		public ControllerDigitalActionHandle_t GetDigitalActionHandle(string pszActionName)
		{
			return this.platform.ISteamController_GetDigitalActionHandle(pszActionName);
		}

		public int GetDigitalActionOrigins(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerDigitalActionHandle_t digitalActionHandle, out ControllerActionOrigin originsOut)
		{
			return this.platform.ISteamController_GetDigitalActionOrigins(controllerHandle.Value, actionSetHandle.Value, digitalActionHandle.Value, out originsOut);
		}

		public int GetGamepadIndexForController(ControllerHandle_t ulControllerHandle)
		{
			return this.platform.ISteamController_GetGamepadIndexForController(ulControllerHandle.Value);
		}

		public string GetGlyphForActionOrigin(ControllerActionOrigin eOrigin)
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamController_GetGlyphForActionOrigin(eOrigin));
		}

		public SteamInputType GetInputTypeForHandle(ControllerHandle_t controllerHandle)
		{
			return this.platform.ISteamController_GetInputTypeForHandle(controllerHandle.Value);
		}

		public ControllerMotionData_t GetMotionData(ControllerHandle_t controllerHandle)
		{
			return this.platform.ISteamController_GetMotionData(controllerHandle.Value);
		}

		public string GetStringForActionOrigin(ControllerActionOrigin eOrigin)
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamController_GetStringForActionOrigin(eOrigin));
		}

		public bool Init()
		{
			return this.platform.ISteamController_Init();
		}

		public void RunFrame()
		{
			this.platform.ISteamController_RunFrame();
		}

		public void SetLEDColor(ControllerHandle_t controllerHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags)
		{
			this.platform.ISteamController_SetLEDColor(controllerHandle.Value, nColorR, nColorG, nColorB, nFlags);
		}

		public bool ShowAnalogActionOrigins(ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t analogActionHandle, float flScale, float flXPosition, float flYPosition)
		{
			return this.platform.ISteamController_ShowAnalogActionOrigins(controllerHandle.Value, analogActionHandle.Value, flScale, flXPosition, flYPosition);
		}

		public bool ShowBindingPanel(ControllerHandle_t controllerHandle)
		{
			return this.platform.ISteamController_ShowBindingPanel(controllerHandle.Value);
		}

		public bool ShowDigitalActionOrigins(ControllerHandle_t controllerHandle, ControllerDigitalActionHandle_t digitalActionHandle, float flScale, float flXPosition, float flYPosition)
		{
			return this.platform.ISteamController_ShowDigitalActionOrigins(controllerHandle.Value, digitalActionHandle.Value, flScale, flXPosition, flYPosition);
		}

		public bool Shutdown()
		{
			return this.platform.ISteamController_Shutdown();
		}

		public void StopAnalogActionMomentum(ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t eAction)
		{
			this.platform.ISteamController_StopAnalogActionMomentum(controllerHandle.Value, eAction.Value);
		}

		public void TriggerHapticPulse(ControllerHandle_t controllerHandle, SteamControllerPad eTargetPad, ushort usDurationMicroSec)
		{
			this.platform.ISteamController_TriggerHapticPulse(controllerHandle.Value, eTargetPad, usDurationMicroSec);
		}

		public void TriggerRepeatedHapticPulse(ControllerHandle_t controllerHandle, SteamControllerPad eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags)
		{
			this.platform.ISteamController_TriggerRepeatedHapticPulse(controllerHandle.Value, eTargetPad, usDurationMicroSec, usOffMicroSec, unRepeat, nFlags);
		}

		public void TriggerVibration(ControllerHandle_t controllerHandle, ushort usLeftSpeed, ushort usRightSpeed)
		{
			this.platform.ISteamController_TriggerVibration(controllerHandle.Value, usLeftSpeed, usRightSpeed);
		}
	}
}