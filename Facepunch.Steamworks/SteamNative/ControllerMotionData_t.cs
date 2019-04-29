using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct ControllerMotionData_t
	{
		internal float RotQuatX;

		internal float RotQuatY;

		internal float RotQuatZ;

		internal float RotQuatW;

		internal float PosAccelX;

		internal float PosAccelY;

		internal float PosAccelZ;

		internal float RotVelX;

		internal float RotVelY;

		internal float RotVelZ;

		internal static ControllerMotionData_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (ControllerMotionData_t)Marshal.PtrToStructure(p, typeof(ControllerMotionData_t));
			}
			return (ControllerMotionData_t.PackSmall)Marshal.PtrToStructure(p, typeof(ControllerMotionData_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(ControllerMotionData_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(ControllerMotionData_t));
		}

		internal struct PackSmall
		{
			internal float RotQuatX;

			internal float RotQuatY;

			internal float RotQuatZ;

			internal float RotQuatW;

			internal float PosAccelX;

			internal float PosAccelY;

			internal float PosAccelZ;

			internal float RotVelX;

			internal float RotVelY;

			internal float RotVelZ;

			public static implicit operator ControllerMotionData_t(ControllerMotionData_t.PackSmall d)
			{
				ControllerMotionData_t controllerMotionDataT = new ControllerMotionData_t()
				{
					RotQuatX = d.RotQuatX,
					RotQuatY = d.RotQuatY,
					RotQuatZ = d.RotQuatZ,
					RotQuatW = d.RotQuatW,
					PosAccelX = d.PosAccelX,
					PosAccelY = d.PosAccelY,
					PosAccelZ = d.PosAccelZ,
					RotVelX = d.RotVelX,
					RotVelY = d.RotVelY,
					RotVelZ = d.RotVelZ
				};
				return controllerMotionDataT;
			}
		}
	}
}