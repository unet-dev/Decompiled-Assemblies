using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ConVar
{
	[Factory("hierarchy")]
	public class Hierarchy : ConsoleSystem
	{
		private static GameObject currentDir;

		public Hierarchy()
		{
		}

		[ServerVar]
		public static void cd(ConsoleSystem.Arg args)
		{
			GameObject gameObject;
			if (args.FullString == ".")
			{
				Hierarchy.currentDir = null;
				args.ReplyWith("Changed to .");
				return;
			}
			if (args.FullString == "..")
			{
				if (Hierarchy.currentDir)
				{
					if (Hierarchy.currentDir.transform.parent)
					{
						gameObject = Hierarchy.currentDir.transform.parent.gameObject;
					}
					else
					{
						gameObject = null;
					}
					Hierarchy.currentDir = gameObject;
				}
				Hierarchy.currentDir = null;
				if (!Hierarchy.currentDir)
				{
					args.ReplyWith("Changed to .");
					return;
				}
				args.ReplyWith(string.Concat("Changed to ", Hierarchy.currentDir.transform.GetRecursiveName("")));
				return;
			}
			Transform transforms = Hierarchy.GetCurrent().FirstOrDefault<Transform>((Transform x) => x.name.ToLower() == args.FullString.ToLower());
			if (transforms == null)
			{
				transforms = Hierarchy.GetCurrent().FirstOrDefault<Transform>((Transform x) => x.name.StartsWith(args.FullString, StringComparison.CurrentCultureIgnoreCase));
			}
			if (!transforms)
			{
				args.ReplyWith(string.Concat("Couldn't find \"", args.FullString, "\""));
				return;
			}
			Hierarchy.currentDir = transforms.gameObject;
			args.ReplyWith(string.Concat("Changed to ", Hierarchy.currentDir.transform.GetRecursiveName("")));
		}

		[ServerVar]
		public static void del(ConsoleSystem.Arg args)
		{
			if (!args.HasArgs(1))
			{
				return;
			}
			IEnumerable<Transform> current = 
				from x in Hierarchy.GetCurrent()
				where x.name.ToLower() == args.FullString.ToLower()
				select x;
			if (current.Count<Transform>() == 0)
			{
				current = 
					from x in Hierarchy.GetCurrent()
					where x.name.StartsWith(args.FullString, StringComparison.CurrentCultureIgnoreCase)
					select x;
			}
			if (current.Count<Transform>() == 0)
			{
				args.ReplyWith(string.Concat("Couldn't find  ", args.FullString));
				return;
			}
			foreach (Transform transforms in current)
			{
				BaseEntity baseEntity = transforms.gameObject.ToBaseEntity();
				if (!baseEntity.IsValid())
				{
					GameManager.Destroy(transforms.gameObject, 0f);
				}
				else
				{
					if (!baseEntity.isServer)
					{
						continue;
					}
					baseEntity.Kill(BaseNetworkable.DestroyMode.None);
				}
			}
			args.ReplyWith(string.Concat("Deleted ", current.Count<Transform>(), " objects"));
		}

		private static Transform[] GetCurrent()
		{
			if (Hierarchy.currentDir == null)
			{
				return TransformUtil.GetRootObjects().ToArray<Transform>();
			}
			List<Transform> transforms = new List<Transform>();
			for (int i = 0; i < Hierarchy.currentDir.transform.childCount; i++)
			{
				transforms.Add(Hierarchy.currentDir.transform.GetChild(i));
			}
			return transforms.ToArray();
		}

		[ServerVar]
		public static void ls(ConsoleSystem.Arg args)
		{
			Func<Transform, bool> func = null;
			string str = "";
			string str1 = args.GetString(0, "");
			str = (!Hierarchy.currentDir ? string.Concat(str, "Listing .\n\n") : string.Concat(str, "Listing ", Hierarchy.currentDir.transform.GetRecursiveName(""), "\n\n"));
			Transform[] current = Hierarchy.GetCurrent();
			Func<Transform, bool> func1 = func;
			if (func1 == null)
			{
				Func<Transform, bool> func2 = (Transform x) => {
					if (string.IsNullOrEmpty(str1))
					{
						return true;
					}
					return x.name.Contains(str1);
				};
				Func<Transform, bool> func3 = func2;
				func = func2;
				func1 = func3;
			}
			foreach (Transform transforms in ((IEnumerable<Transform>)current).Where<Transform>(func1).Take<Transform>(40))
			{
				str = string.Concat(str, string.Format("   {0} [{1}]\n", transforms.name, transforms.childCount));
			}
			str = string.Concat(str, "\n");
			args.ReplyWith(str);
		}
	}
}