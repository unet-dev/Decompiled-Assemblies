using Mono;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Mono.Cecil
{
	public abstract class BaseAssemblyResolver : IAssemblyResolver
	{
		private readonly static bool on_mono;

		private readonly Collection<string> directories;

		private Collection<string> gac_paths;

		static BaseAssemblyResolver()
		{
			BaseAssemblyResolver.on_mono = Type.GetType("Mono.Runtime") != null;
		}

		protected BaseAssemblyResolver()
		{
			this.directories = new Collection<string>(2)
			{
				".",
				"bin"
			};
		}

		public void AddSearchDirectory(string directory)
		{
			this.directories.Add(directory);
		}

		private AssemblyDefinition GetAssembly(string file, ReaderParameters parameters)
		{
			if (parameters.AssemblyResolver == null)
			{
				parameters.AssemblyResolver = this;
			}
			return ModuleDefinition.ReadModule(file, parameters).Assembly;
		}

		private static string GetAssemblyFile(AssemblyNameReference reference, string prefix, string gac)
		{
			StringBuilder stringBuilder = (new StringBuilder()).Append(prefix).Append(reference.Version).Append("__");
			for (int i = 0; i < (int)reference.PublicKeyToken.Length; i++)
			{
				stringBuilder.Append(reference.PublicKeyToken[i].ToString("x2"));
			}
			return Path.Combine(Path.Combine(Path.Combine(gac, reference.Name), stringBuilder.ToString()), string.Concat(reference.Name, ".dll"));
		}

		private AssemblyDefinition GetAssemblyInGac(AssemblyNameReference reference, ReaderParameters parameters)
		{
			if (reference.PublicKeyToken == null || reference.PublicKeyToken.Length == 0)
			{
				return null;
			}
			if (this.gac_paths == null)
			{
				this.gac_paths = BaseAssemblyResolver.GetGacPaths();
			}
			if (BaseAssemblyResolver.on_mono)
			{
				return this.GetAssemblyInMonoGac(reference, parameters);
			}
			return this.GetAssemblyInNetGac(reference, parameters);
		}

		private AssemblyDefinition GetAssemblyInMonoGac(AssemblyNameReference reference, ReaderParameters parameters)
		{
			for (int i = 0; i < this.gac_paths.Count; i++)
			{
				string item = this.gac_paths[i];
				string assemblyFile = BaseAssemblyResolver.GetAssemblyFile(reference, string.Empty, item);
				if (File.Exists(assemblyFile))
				{
					return this.GetAssembly(assemblyFile, parameters);
				}
			}
			return null;
		}

		private AssemblyDefinition GetAssemblyInNetGac(AssemblyNameReference reference, ReaderParameters parameters)
		{
			string[] strArrays = new string[] { "GAC_MSIL", "GAC_32", "GAC_64", "GAC" };
			string[] empty = new string[] { string.Empty, "v4.0_" };
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < (int)strArrays.Length; j++)
				{
					string str = Path.Combine(this.gac_paths[i], strArrays[j]);
					string assemblyFile = BaseAssemblyResolver.GetAssemblyFile(reference, empty[i], str);
					if (Directory.Exists(str) && File.Exists(assemblyFile))
					{
						return this.GetAssembly(assemblyFile, parameters);
					}
				}
			}
			return null;
		}

		private AssemblyDefinition GetCorlib(AssemblyNameReference reference, ReaderParameters parameters)
		{
			Version version = reference.Version;
			if (typeof(object).Assembly.GetName().Version == version || BaseAssemblyResolver.IsZero(version))
			{
				return this.GetAssembly(typeof(object).Module.FullyQualifiedName, parameters);
			}
			string fullName = Directory.GetParent(Directory.GetParent(typeof(object).Module.FullyQualifiedName).FullName).FullName;
			if (!BaseAssemblyResolver.on_mono)
			{
				switch (version.Major)
				{
					case 1:
					{
						if (version.MajorRevision != 3300)
						{
							fullName = Path.Combine(fullName, "v1.0.5000.0");
							break;
						}
						else
						{
							fullName = Path.Combine(fullName, "v1.0.3705");
							break;
						}
					}
					case 2:
					{
						fullName = Path.Combine(fullName, "v2.0.50727");
						break;
					}
					case 3:
					{
						throw new NotSupportedException(string.Concat("Version not supported: ", version));
					}
					case 4:
					{
						fullName = Path.Combine(fullName, "v4.0.30319");
						break;
					}
					default:
					{
						throw new NotSupportedException(string.Concat("Version not supported: ", version));
					}
				}
			}
			else if (version.Major == 1)
			{
				fullName = Path.Combine(fullName, "1.0");
			}
			else if (version.Major != 2)
			{
				if (version.Major != 4)
				{
					throw new NotSupportedException(string.Concat("Version not supported: ", version));
				}
				fullName = Path.Combine(fullName, "4.0");
			}
			else
			{
				fullName = (version.MajorRevision != 5 ? Path.Combine(fullName, "2.0") : Path.Combine(fullName, "2.1"));
			}
			string str = Path.Combine(fullName, "mscorlib.dll");
			if (!File.Exists(str))
			{
				return null;
			}
			return this.GetAssembly(str, parameters);
		}

		private static string GetCurrentMonoGac()
		{
			return Path.Combine(Directory.GetParent(Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName)).FullName, "gac");
		}

		private static Collection<string> GetDefaultMonoGacPaths()
		{
			Collection<string> strs = new Collection<string>(1);
			string currentMonoGac = BaseAssemblyResolver.GetCurrentMonoGac();
			if (currentMonoGac != null)
			{
				strs.Add(currentMonoGac);
			}
			string environmentVariable = Environment.GetEnvironmentVariable("MONO_GAC_PREFIX");
			if (string.IsNullOrEmpty(environmentVariable))
			{
				return strs;
			}
			string[] strArrays = environmentVariable.Split(new char[] { Path.PathSeparator });
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (!string.IsNullOrEmpty(str))
				{
					string str1 = Path.Combine(Path.Combine(Path.Combine(str, "lib"), "mono"), "gac");
					if (Directory.Exists(str1) && !strs.Contains(currentMonoGac))
					{
						strs.Add(str1);
					}
				}
			}
			return strs;
		}

		private static Collection<string> GetGacPaths()
		{
			if (BaseAssemblyResolver.on_mono)
			{
				return BaseAssemblyResolver.GetDefaultMonoGacPaths();
			}
			Collection<string> strs = new Collection<string>(2);
			string environmentVariable = Environment.GetEnvironmentVariable("WINDIR");
			if (environmentVariable == null)
			{
				return strs;
			}
			strs.Add(Path.Combine(environmentVariable, "assembly"));
			strs.Add(Path.Combine(environmentVariable, Path.Combine("Microsoft.NET", "assembly")));
			return strs;
		}

		public string[] GetSearchDirectories()
		{
			string[] strArrays = new string[this.directories.size];
			Array.Copy(this.directories.items, strArrays, (int)strArrays.Length);
			return strArrays;
		}

		private static bool IsZero(Version version)
		{
			if (version == null)
			{
				return true;
			}
			if (version.Major != 0 || version.Minor != 0 || version.Build != 0)
			{
				return false;
			}
			return version.Revision == 0;
		}

		public void RemoveSearchDirectory(string directory)
		{
			this.directories.Remove(directory);
		}

		public virtual AssemblyDefinition Resolve(string fullName)
		{
			return this.Resolve(fullName, new ReaderParameters());
		}

		public virtual AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
		{
			if (fullName == null)
			{
				throw new ArgumentNullException("fullName");
			}
			return this.Resolve(AssemblyNameReference.Parse(fullName), parameters);
		}

		public virtual AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			return this.Resolve(name, new ReaderParameters());
		}

		public virtual AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (parameters == null)
			{
				parameters = new ReaderParameters();
			}
			AssemblyDefinition corlib = this.SearchDirectory(name, this.directories, parameters);
			if (corlib != null)
			{
				return corlib;
			}
			if (name.IsRetargetable)
			{
				name = new AssemblyNameReference(name.Name, new Version(0, 0, 0, 0))
				{
					PublicKeyToken = Empty<byte>.Array
				};
			}
			string directoryName = Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName);
			if (BaseAssemblyResolver.IsZero(name.Version))
			{
				corlib = this.SearchDirectory(name, new string[] { directoryName }, parameters);
				if (corlib != null)
				{
					return corlib;
				}
			}
			if (name.Name == "mscorlib")
			{
				corlib = this.GetCorlib(name, parameters);
				if (corlib != null)
				{
					return corlib;
				}
			}
			corlib = this.GetAssemblyInGac(name, parameters);
			if (corlib != null)
			{
				return corlib;
			}
			corlib = this.SearchDirectory(name, new string[] { directoryName }, parameters);
			if (corlib != null)
			{
				return corlib;
			}
			if (this.ResolveFailure != null)
			{
				corlib = this.ResolveFailure(this, name);
				if (corlib != null)
				{
					return corlib;
				}
			}
			throw new AssemblyResolutionException(name);
		}

		private AssemblyDefinition SearchDirectory(AssemblyNameReference name, IEnumerable<string> directories, ReaderParameters parameters)
		{
			AssemblyDefinition assembly;
			string[] strArrays = new string[] { ".exe", ".dll" };
			using (IEnumerator<string> enumerator = directories.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					string[] strArrays1 = strArrays;
					int num = 0;
					while (num < (int)strArrays1.Length)
					{
						string str = strArrays1[num];
						string str1 = Path.Combine(current, string.Concat(name.Name, str));
						if (!File.Exists(str1))
						{
							num++;
						}
						else
						{
							assembly = this.GetAssembly(str1, parameters);
							return assembly;
						}
					}
				}
				return null;
			}
			return assembly;
		}

		public event AssemblyResolveEventHandler ResolveFailure;
	}
}