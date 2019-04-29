using Facepunch;
using System;

namespace ConVar
{
	public class Manifest
	{
		public Manifest()
		{
		}

		[ClientVar]
		[ServerVar]
		public static object PrintManifest()
		{
			return Application.Manifest;
		}

		[ClientVar]
		[ServerVar]
		public static object PrintManifestRaw()
		{
			return Facepunch.Manifest.Contents;
		}
	}
}