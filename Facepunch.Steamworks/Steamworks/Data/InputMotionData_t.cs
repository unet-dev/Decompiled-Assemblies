using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct InputMotionData_t
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

		internal static InputMotionData_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (InputMotionData_t)Marshal.PtrToStructure(p, typeof(InputMotionData_t)) : (InputMotionData_t.Pack8)Marshal.PtrToStructure(p, typeof(InputMotionData_t.Pack8)));
		}

		public struct Pack8
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

			public static implicit operator InputMotionData_t(InputMotionData_t.Pack8 d)
			{
				InputMotionData_t inputMotionDataT = new InputMotionData_t()
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
				return inputMotionDataT;
			}

			public static implicit operator Pack8(InputMotionData_t d)
			{
				InputMotionData_t.Pack8 pack8 = new InputMotionData_t.Pack8()
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
				return pack8;
			}
		}
	}
}