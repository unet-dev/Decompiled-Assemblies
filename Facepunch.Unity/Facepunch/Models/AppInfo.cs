using Facepunch;
using System;
using UnityEngine;

namespace Facepunch.Models
{
	public struct AppInfo
	{
		public string Arch
		{
			get
			{
				if (IntPtr.Size != 4)
				{
					return "x64";
				}
				return "x86";
			}
		}

		public BuildInfo Build
		{
			get
			{
				return BuildInfo.Current;
			}
		}

		public string Cpu
		{
			get
			{
				return SystemInfo.processorType;
			}
		}

		public int CpuCount
		{
			get
			{
				return SystemInfo.processorCount;
			}
		}

		public string Gpu
		{
			get
			{
				return SystemInfo.graphicsDeviceName;
			}
		}

		public int GpuMem
		{
			get
			{
				return SystemInfo.graphicsMemorySize;
			}
		}

		public string LevelName
		{
			get
			{
				return Facepunch.Application.Integration.LevelName;
			}
		}

		public string LevelPos
		{
			get
			{
				if (Camera.main == null)
				{
					return "0 0 0";
				}
				return Camera.main.transform.position.ToString();
			}
		}

		public string LevelRot
		{
			get
			{
				if (Camera.main == null)
				{
					return "0 0 0";
				}
				return Camera.main.transform.eulerAngles.ToString();
			}
		}

		public int Mem
		{
			get
			{
				return SystemInfo.graphicsMemorySize;
			}
		}

		public int MinutesPlayed
		{
			get
			{
				return Facepunch.Application.Integration.MinutesPlayed;
			}
		}

		public string Name
		{
			get
			{
				return SystemInfo.deviceName;
			}
		}

		public string Os
		{
			get
			{
				return SystemInfo.operatingSystem;
			}
		}

		public string ServerAddress
		{
			get
			{
				return Facepunch.Application.Integration.ServerAddress;
			}
		}

		public string ServerName
		{
			get
			{
				return Facepunch.Application.Integration.ServerName;
			}
		}

		public string UserId
		{
			get
			{
				return Facepunch.Application.Integration.UserId;
			}
		}

		public string UserName
		{
			get
			{
				return Facepunch.Application.Integration.UserName;
			}
		}

		public int Version
		{
			get
			{
				return 2;
			}
		}
	}
}