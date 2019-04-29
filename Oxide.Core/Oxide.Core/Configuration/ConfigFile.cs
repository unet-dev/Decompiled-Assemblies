using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Configuration
{
	public abstract class ConfigFile
	{
		[JsonIgnore]
		public string Filename
		{
			get;
			private set;
		}

		protected ConfigFile(string filename)
		{
			this.Filename = filename;
		}

		public static T Load<T>(string filename)
		where T : ConfigFile
		{
			T t = (T)Activator.CreateInstance(typeof(T), new object[] { filename });
			t.Load(null);
			return t;
		}

		public virtual void Load(string filename = null)
		{
			JsonConvert.PopulateObject(File.ReadAllText(filename ?? this.Filename), this);
		}

		public virtual void Save(string filename = null)
		{
			string str = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(filename ?? this.Filename, str);
		}
	}
}