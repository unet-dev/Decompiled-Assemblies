using System;
using UnityEngine;

namespace AmplifyOcclusion
{
	[Serializable]
	public class VersionInfo
	{
		public const byte Major = 2;

		public const byte Minor = 0;

		public const byte Release = 0;

		private static string StageSuffix;

		[SerializeField]
		private int m_major;

		[SerializeField]
		private int m_minor;

		[SerializeField]
		private int m_release;

		public int Number
		{
			get
			{
				return this.m_major * 100 + this.m_minor * 10 + this.m_release;
			}
		}

		static VersionInfo()
		{
			VersionInfo.StageSuffix = "_dev002";
		}

		private VersionInfo()
		{
			this.m_major = 2;
			this.m_minor = 0;
			this.m_release = 0;
		}

		private VersionInfo(byte major, byte minor, byte release)
		{
			this.m_major = major;
			this.m_minor = minor;
			this.m_release = release;
		}

		public static VersionInfo Current()
		{
			return new VersionInfo(2, 0, 0);
		}

		public static bool Matches(VersionInfo version)
		{
			if (2 != version.m_major || version.m_minor != 0)
			{
				return false;
			}
			return version.m_release == 0;
		}

		public static string StaticToString()
		{
			return string.Concat(string.Format("{0}.{1}.{2}", (byte)2, (byte)0, (byte)0), VersionInfo.StageSuffix);
		}

		public override string ToString()
		{
			return string.Concat(string.Format("{0}.{1}.{2}", this.m_major, this.m_minor, this.m_release), VersionInfo.StageSuffix);
		}
	}
}