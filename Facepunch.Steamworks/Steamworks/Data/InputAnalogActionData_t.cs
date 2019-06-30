using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct InputAnalogActionData_t
	{
		internal InputSourceMode EMode;

		internal float X;

		internal float Y;

		internal bool BActive;

		internal static InputAnalogActionData_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (InputAnalogActionData_t)Marshal.PtrToStructure(p, typeof(InputAnalogActionData_t)) : (InputAnalogActionData_t.Pack8)Marshal.PtrToStructure(p, typeof(InputAnalogActionData_t.Pack8)));
		}

		public struct Pack8
		{
			internal InputSourceMode EMode;

			internal float X;

			internal float Y;

			internal bool BActive;

			public static implicit operator InputAnalogActionData_t(InputAnalogActionData_t.Pack8 d)
			{
				InputAnalogActionData_t inputAnalogActionDataT = new InputAnalogActionData_t()
				{
					EMode = d.EMode,
					X = d.X,
					Y = d.Y,
					BActive = d.BActive
				};
				return inputAnalogActionDataT;
			}

			public static implicit operator Pack8(InputAnalogActionData_t d)
			{
				InputAnalogActionData_t.Pack8 pack8 = new InputAnalogActionData_t.Pack8()
				{
					EMode = d.EMode,
					X = d.X,
					Y = d.Y,
					BActive = d.BActive
				};
				return pack8;
			}
		}
	}
}