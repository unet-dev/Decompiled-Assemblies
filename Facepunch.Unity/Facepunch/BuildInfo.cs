using Facepunch.Math;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	public class BuildInfo
	{
		private static BuildInfo _current;

		public BuildInfo.BuildDesc Build
		{
			get;
			set;
		}

		[JsonIgnore]
		public DateTime BuildDate
		{
			get
			{
				return Epoch.ToDateTime(this.Date).ToLocalTime();
			}
		}

		public static BuildInfo Current
		{
			get
			{
				if (BuildInfo._current == null)
				{
					BuildInfo._current = new BuildInfo()
					{
						Scm = new BuildInfo.ScmInfo(),
						Build = new BuildInfo.BuildDesc()
					};
					TextAsset textAsset = Resources.Load<TextAsset>("BuildInfo");
					if (textAsset)
					{
						BuildInfo._current = JsonConvert.DeserializeObject<BuildInfo>(textAsset.text);
						BuildInfo._current.Valid = true;
					}
				}
				return BuildInfo._current;
			}
		}

		public int Date
		{
			get;
			set;
		}

		public BuildInfo.ScmInfo Scm
		{
			get;
			set;
		}

		public bool Valid
		{
			get;
			set;
		}

		public BuildInfo()
		{
		}

		public class BuildDesc
		{
			public string Id
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			public string Node
			{
				get;
				set;
			}

			public string Number
			{
				get;
				set;
			}

			public string Tag
			{
				get;
				set;
			}

			public string Url
			{
				get;
				set;
			}

			public BuildDesc()
			{
			}
		}

		public class ScmInfo
		{
			public string Author
			{
				get;
				set;
			}

			public string Branch
			{
				get;
				set;
			}

			public string ChangeId
			{
				get;
				set;
			}

			public string Comment
			{
				get;
				set;
			}

			public string Date
			{
				get;
				set;
			}

			public string Items
			{
				get;
				set;
			}

			public string Repo
			{
				get;
				set;
			}

			public string Type
			{
				get;
				set;
			}

			public ScmInfo()
			{
			}
		}
	}
}