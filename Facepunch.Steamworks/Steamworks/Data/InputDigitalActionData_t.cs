using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct InputDigitalActionData_t
	{
		internal bool BState;

		internal bool BActive;

		internal static InputDigitalActionData_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (InputDigitalActionData_t)Marshal.PtrToStructure(p, typeof(InputDigitalActionData_t)) : (InputDigitalActionData_t.Pack8)Marshal.PtrToStructure(p, typeof(InputDigitalActionData_t.Pack8)));
		}

		public struct Pack8
		{
			internal bool BState;

			internal bool BActive;

			public static implicit operator InputDigitalActionData_t(InputDigitalActionData_t.Pack8 d)
			{
				InputDigitalActionData_t inputDigitalActionDataT = new InputDigitalActionData_t()
				{
					BState = d.BState,
					BActive = d.BActive
				};
				return inputDigitalActionDataT;
			}

			public static implicit operator Pack8(InputDigitalActionData_t d)
			{
				InputDigitalActionData_t.Pack8 pack8 = new InputDigitalActionData_t.Pack8()
				{
					BState = d.BState,
					BActive = d.BActive
				};
				return pack8;
			}
		}
	}
}