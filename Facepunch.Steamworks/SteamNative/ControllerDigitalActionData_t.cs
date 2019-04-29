using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct ControllerDigitalActionData_t
	{
		internal bool BState;

		internal bool BActive;

		internal static ControllerDigitalActionData_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (ControllerDigitalActionData_t)Marshal.PtrToStructure(p, typeof(ControllerDigitalActionData_t));
			}
			return (ControllerDigitalActionData_t.PackSmall)Marshal.PtrToStructure(p, typeof(ControllerDigitalActionData_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(ControllerDigitalActionData_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(ControllerDigitalActionData_t));
		}

		internal struct PackSmall
		{
			internal bool BState;

			internal bool BActive;

			public static implicit operator ControllerDigitalActionData_t(ControllerDigitalActionData_t.PackSmall d)
			{
				ControllerDigitalActionData_t controllerDigitalActionDataT = new ControllerDigitalActionData_t()
				{
					BState = d.BState,
					BActive = d.BActive
				};
				return controllerDigitalActionDataT;
			}
		}
	}
}