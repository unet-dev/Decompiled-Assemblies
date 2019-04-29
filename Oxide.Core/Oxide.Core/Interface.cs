using System;
using System.Runtime.CompilerServices;

namespace Oxide.Core
{
	public static class Interface
	{
		public static NativeDebugCallback DebugCallback
		{
			get;
			set;
		}

		public static OxideMod Oxide
		{
			get;
			private set;
		}

		public static object Call(string hook, params object[] args)
		{
			return Interface.CallHook(hook, args);
		}

		public static T Call<T>(string hook, params object[] args)
		{
			return (T)Convert.ChangeType(Interface.CallHook(hook, args), typeof(T));
		}

		public static object CallDeprecated(string oldHook, string newHook, DateTime expireDate, params object[] args)
		{
			return Interface.CallDeprecatedHook(oldHook, newHook, expireDate, args);
		}

		public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, params object[] args)
		{
			return Interface.Oxide.CallDeprecatedHook(oldHook, newHook, expireDate, args);
		}

		public static object CallHook(string hook, object[] args)
		{
			OxideMod oxide = Interface.Oxide;
			if (oxide == null)
			{
				return null;
			}
			return oxide.CallHook(hook, args);
		}

		public static object CallHook(string hook)
		{
			return Interface.CallHook(hook, (object[])null);
		}

		public static object CallHook(string hook, object obj1)
		{
			object[] objArray = ArrayPool.Get(1);
			objArray[0] = obj1;
			object obj = Interface.CallHook(hook, objArray);
			ArrayPool.Free(objArray);
			return obj;
		}

		public static object CallHook(string hook, object obj1, object obj2)
		{
			object[] objArray = ArrayPool.Get(2);
			objArray[0] = obj1;
			objArray[1] = obj2;
			object obj = Interface.CallHook(hook, objArray);
			ArrayPool.Free(objArray);
			return obj;
		}

		public static object CallHook(string hook, object obj1, object obj2, object obj3)
		{
			object[] objArray = ArrayPool.Get(3);
			objArray[0] = obj1;
			objArray[1] = obj2;
			objArray[2] = obj3;
			object obj = Interface.CallHook(hook, objArray);
			ArrayPool.Free(objArray);
			return obj;
		}

		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4)
		{
			object[] objArray = ArrayPool.Get(4);
			objArray[0] = obj1;
			objArray[1] = obj2;
			objArray[2] = obj3;
			objArray[3] = obj4;
			object obj = Interface.CallHook(hook, objArray);
			ArrayPool.Free(objArray);
			return obj;
		}

		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4, object obj5)
		{
			object[] objArray = ArrayPool.Get(5);
			objArray[0] = obj1;
			objArray[1] = obj2;
			objArray[2] = obj3;
			objArray[3] = obj4;
			objArray[4] = obj5;
			object obj = Interface.CallHook(hook, objArray);
			ArrayPool.Free(objArray);
			return obj;
		}

		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6)
		{
			object[] objArray = ArrayPool.Get(6);
			objArray[0] = obj1;
			objArray[1] = obj2;
			objArray[2] = obj3;
			objArray[3] = obj4;
			objArray[4] = obj5;
			objArray[5] = obj6;
			object obj = Interface.CallHook(hook, objArray);
			ArrayPool.Free(objArray);
			return obj;
		}

		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7)
		{
			object[] objArray = ArrayPool.Get(7);
			objArray[0] = obj1;
			objArray[1] = obj2;
			objArray[2] = obj3;
			objArray[3] = obj4;
			objArray[4] = obj5;
			objArray[5] = obj6;
			objArray[6] = obj7;
			object obj = Interface.CallHook(hook, objArray);
			ArrayPool.Free(objArray);
			return obj;
		}

		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8)
		{
			object[] objArray = ArrayPool.Get(8);
			objArray[0] = obj1;
			objArray[1] = obj2;
			objArray[2] = obj3;
			objArray[3] = obj4;
			objArray[4] = obj5;
			objArray[5] = obj6;
			objArray[6] = obj7;
			objArray[7] = obj8;
			object obj = Interface.CallHook(hook, objArray);
			ArrayPool.Free(objArray);
			return obj;
		}

		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8, object obj9)
		{
			object[] objArray = ArrayPool.Get(9);
			objArray[0] = obj1;
			objArray[1] = obj2;
			objArray[2] = obj3;
			objArray[3] = obj4;
			objArray[4] = obj5;
			objArray[5] = obj6;
			objArray[6] = obj7;
			objArray[7] = obj8;
			objArray[8] = obj9;
			object obj = Interface.CallHook(hook, objArray);
			ArrayPool.Free(objArray);
			return obj;
		}

		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8, object obj9, object obj10)
		{
			object[] objArray = ArrayPool.Get(10);
			objArray[0] = obj1;
			objArray[1] = obj2;
			objArray[2] = obj3;
			objArray[3] = obj4;
			objArray[4] = obj5;
			objArray[5] = obj6;
			objArray[6] = obj7;
			objArray[7] = obj8;
			objArray[8] = obj9;
			objArray[9] = obj10;
			object obj = Interface.CallHook(hook, objArray);
			ArrayPool.Free(objArray);
			return obj;
		}

		public static OxideMod GetMod()
		{
			return Interface.Oxide;
		}

		public static void Initialize()
		{
			if (Interface.Oxide != null)
			{
				return;
			}
			Interface.Oxide = new OxideMod(Interface.DebugCallback);
			Interface.Oxide.Load();
		}
	}
}