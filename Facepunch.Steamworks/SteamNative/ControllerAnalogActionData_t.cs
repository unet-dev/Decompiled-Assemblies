using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct ControllerAnalogActionData_t
	{
		internal ControllerSourceMode EMode;

		internal float X;

		internal float Y;

		internal bool BActive;

		internal static ControllerAnalogActionData_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (ControllerAnalogActionData_t)Marshal.PtrToStructure(p, typeof(ControllerAnalogActionData_t));
			}
			return (ControllerAnalogActionData_t.PackSmall)Marshal.PtrToStructure(p, typeof(ControllerAnalogActionData_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(ControllerAnalogActionData_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(ControllerAnalogActionData_t));
		}

		internal struct PackSmall
		{
			internal ControllerSourceMode EMode;

			internal float X;

			internal float Y;

			internal bool BActive;

			public static implicit operator ControllerAnalogActionData_t(ControllerAnalogActionData_t.PackSmall d)
			{
				ControllerAnalogActionData_t controllerAnalogActionDataT = new ControllerAnalogActionData_t()
				{
					EMode = d.EMode,
					X = d.X,
					Y = d.Y,
					BActive = d.BActive
				};
				return controllerAnalogActionDataT;
			}
		}
	}
}