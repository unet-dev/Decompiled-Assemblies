using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Facepunch
{
	public class SystemInformation
	{
		public SystemInformation.Hardware hardware;

		public SystemInformation.Environment environment;

		public SystemInformation()
		{
		}

		public struct Environment
		{
			public string anisotropicFiltering
			{
				get
				{
					return QualitySettings.anisotropicFiltering.ToString();
				}
			}

			public string antiAliasing
			{
				get
				{
					return QualitySettings.antiAliasing.ToString();
				}
			}

			public string companyName
			{
				get
				{
					return Application.companyName;
				}
			}

			public Resolution currentResolution
			{
				get
				{
					return Screen.currentResolution;
				}
			}

			public bool fullScreen
			{
				get
				{
					return Screen.fullScreen;
				}
			}

			public string globalMaximumLOD
			{
				get
				{
					return Shader.globalMaximumLOD.ToString();
				}
			}

			public bool isEditor
			{
				get
				{
					return Application.isEditor;
				}
			}

			public bool isPlayer
			{
				get
				{
					return !Application.isEditor;
				}
			}

			public string lodBias
			{
				get
				{
					return QualitySettings.lodBias.ToString();
				}
			}

			public string maximumLODLevel
			{
				get
				{
					return QualitySettings.maximumLODLevel.ToString();
				}
			}

			public string particleRaycastBudget
			{
				get
				{
					return QualitySettings.particleRaycastBudget.ToString();
				}
			}

			public string pixelLightCount
			{
				get
				{
					return QualitySettings.pixelLightCount.ToString();
				}
			}

			public string qualityLevel
			{
				get
				{
					return QualitySettings.GetQualityLevel().ToString();
				}
			}

			public string realtimeReflectionProbes
			{
				get
				{
					return QualitySettings.realtimeReflectionProbes.ToString();
				}
			}

			public int screenHeight
			{
				get
				{
					return Screen.height;
				}
			}

			public int screenWidth
			{
				get
				{
					return Screen.width;
				}
			}

			public string shadowCascades
			{
				get
				{
					return QualitySettings.shadowCascades.ToString();
				}
			}

			public string shadowDistance
			{
				get
				{
					return QualitySettings.shadowDistance.ToString();
				}
			}

			public string softVegetation
			{
				get
				{
					return QualitySettings.softVegetation.ToString();
				}
			}

			public string systemLanguage
			{
				get
				{
					return Application.systemLanguage.ToString();
				}
			}

			public int targetFrameRate
			{
				get
				{
					return Application.targetFrameRate;
				}
			}

			public string unityVersion
			{
				get
				{
					return Application.unityVersion;
				}
			}

			public string version
			{
				get
				{
					return Application.version;
				}
			}

			public string vSyncCount
			{
				get
				{
					return QualitySettings.vSyncCount.ToString();
				}
			}
		}

		public struct Hardware
		{
			public string deviceName
			{
				get
				{
					return SystemInfo.deviceName;
				}
			}

			public string deviceUniqueIdentifier
			{
				get
				{
					return SystemInfo.deviceUniqueIdentifier;
				}
			}

			public string graphicsDeviceName
			{
				get
				{
					return SystemInfo.graphicsDeviceName;
				}
			}

			public string graphicsDeviceType
			{
				get
				{
					return SystemInfo.graphicsDeviceType.ToString();
				}
			}

			public string graphicsDeviceVendor
			{
				get
				{
					return SystemInfo.graphicsDeviceVendor;
				}
			}

			public string graphicsDeviceVersion
			{
				get
				{
					return SystemInfo.graphicsDeviceVersion;
				}
			}

			public string graphicsMemorySize
			{
				get
				{
					return string.Concat(SystemInfo.graphicsMemorySize, "MB");
				}
			}

			public bool graphicsMultiThreaded
			{
				get
				{
					return SystemInfo.graphicsMultiThreaded;
				}
			}

			public string graphicsShaderLevel
			{
				get
				{
					return SystemInfo.graphicsShaderLevel.ToString();
				}
			}

			public string operatingSystem
			{
				get
				{
					return SystemInfo.operatingSystem;
				}
			}

			public string processorArchitecture
			{
				get
				{
					if (IntPtr.Size == 4)
					{
						return "x86";
					}
					return "x64";
				}
			}

			public string processorCount
			{
				get
				{
					return SystemInfo.processorCount.ToString();
				}
			}

			public int processorFrequency
			{
				get
				{
					return SystemInfo.processorFrequency;
				}
			}

			public string processorType
			{
				get
				{
					return SystemInfo.processorType;
				}
			}

			public string systemMemorySize
			{
				get
				{
					return string.Concat(SystemInfo.systemMemorySize, "MB");
				}
			}
		}
	}
}