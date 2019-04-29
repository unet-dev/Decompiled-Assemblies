using Apex.DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex
{
	[AddComponentMenu("Apex/Common/Apex Component Master", 1000)]
	public class ApexComponentMaster : MonoBehaviour
	{
		private Dictionary<int, ApexComponentMaster.ComponentInfo> _components = new Dictionary<int, ApexComponentMaster.ComponentInfo>();

		private Dictionary<string, ApexComponentMaster.ComponentCategory> _categories = new Dictionary<string, ApexComponentMaster.ComponentCategory>();

		[HideInInspector]
		[SerializeField]
		private bool _firstTime = true;

		[HideInInspector]
		[SerializeField]
		private int _hiddenComponents;

		public IEnumerable<ApexComponentMaster.ComponentCategory> componentCategories
		{
			get
			{
				ApexComponentMaster apexComponentMaster = null;
				Dictionary<!0, !1>.KeyCollection keys = apexComponentMaster._categories.Keys;
				IOrderedEnumerable<string> strs = 
					from s in keys
					orderby s
					select s;
				foreach (string str in strs)
				{
					yield return apexComponentMaster._categories[str];
				}
			}
		}

		public ApexComponentMaster()
		{
		}

		private void AddHidden(ApexComponentMaster.ComponentInfo c)
		{
			if (c.idx > 29)
			{
				UnityEngine.Debug.LogWarning("Apex Component Master cannot manage more than 30 components.");
				return;
			}
			this._hiddenComponents = this._hiddenComponents | 1 << (c.idx & 31);
		}

		public void Cleanup()
		{
			ApexComponentMaster.ComponentInfo componentInfo = null;
			foreach (ApexComponentMaster.ComponentInfo value in this._components.Values)
			{
				if (!value.component.Equals(null))
				{
					continue;
				}
				componentInfo = value;
			}
			if (componentInfo != null)
			{
				this.RemoveHidden(componentInfo);
				this._components.Remove(componentInfo.id);
				componentInfo.category.Remove(componentInfo);
				if (componentInfo.category.count == 0)
				{
					this._categories.Remove(componentInfo.category.name);
				}
			}
		}

		public bool Init(IEnumerable<ApexComponentMaster.ComponentCandidate> candidates)
		{
			ApexComponentMaster.ComponentInfo componentInfo;
			ApexComponentMaster.ComponentCategory componentCategory;
			this.Cleanup();
			bool flag = false;
			int num = 0;
			foreach (ApexComponentMaster.ComponentCandidate candidate in candidates)
			{
				int instanceID = candidate.component.GetInstanceID();
				bool flag1 = this._components.TryGetValue(instanceID, out componentInfo);
				if ((candidate.component.hideFlags & HideFlags.HideInInspector) == HideFlags.None && (this._hiddenComponents & 1 << (num & 31)) > 0)
				{
					MonoBehaviour monoBehaviour = candidate.component;
					monoBehaviour.hideFlags = monoBehaviour.hideFlags | HideFlags.HideInInspector;
					flag = true;
				}
				if (!flag1)
				{
					ApexComponentMaster.ComponentInfo componentInfo1 = new ApexComponentMaster.ComponentInfo()
					{
						component = candidate.component,
						id = instanceID
					};
					int num1 = num;
					num = num1 + 1;
					componentInfo1.idx = num1;
					componentInfo1.name = candidate.component.GetType().Name.Replace("Component", string.Empty).ExpandFromPascal();
					componentInfo1.isVisible = (candidate.component.hideFlags & HideFlags.HideInInspector) == HideFlags.None;
					componentInfo = componentInfo1;
					this._components.Add(instanceID, componentInfo);
					if (!this._categories.TryGetValue(candidate.categoryName, out componentCategory))
					{
						componentCategory = new ApexComponentMaster.ComponentCategory()
						{
							name = candidate.categoryName
						};
						this._categories.Add(candidate.categoryName, componentCategory);
					}
					componentCategory.Add(componentInfo);
					componentInfo.category = componentCategory;
				}
				else
				{
					num++;
					componentInfo.isVisible = (candidate.component.hideFlags & HideFlags.HideInInspector) == HideFlags.None;
				}
			}
			FunctionComparer<ApexComponentMaster.ComponentInfo> functionComparer = new FunctionComparer<ApexComponentMaster.ComponentInfo>((ApexComponentMaster.ComponentInfo a, ApexComponentMaster.ComponentInfo b) => a.name.CompareTo(b.name));
			foreach (ApexComponentMaster.ComponentCategory value in this._categories.Values)
			{
				value.Sort(functionComparer);
			}
			if (!this._firstTime)
			{
				return flag;
			}
			this.ToggleAll();
			this._firstTime = false;
			return true;
		}

		private void RemoveHidden(ApexComponentMaster.ComponentInfo c)
		{
			if (c.idx > 29)
			{
				UnityEngine.Debug.LogWarning("Apex Component Master cannot manage more than 30 components.");
				return;
			}
			this._hiddenComponents &= ~(1 << (c.idx & 31));
		}

		public void Toggle(ApexComponentMaster.ComponentInfo cinfo)
		{
			cinfo.isVisible = !cinfo.isVisible;
			HideFlags hideFlag = cinfo.component.hideFlags;
			if (cinfo.isVisible)
			{
				this.RemoveHidden(cinfo);
				cinfo.component.hideFlags = hideFlag & (HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset | HideFlags.DontSave | HideFlags.HideAndDontSave);
				return;
			}
			this.AddHidden(cinfo);
			cinfo.component.hideFlags = hideFlag | HideFlags.HideInInspector;
		}

		public void Toggle(string componentName, bool visible)
		{
			(
				from c in this._components.Values
				where c.name == componentName
				select c).Apply<ApexComponentMaster.ComponentInfo>((ApexComponentMaster.ComponentInfo c) => {
				c.isVisible = visible;
				HideFlags hideFlag = c.component.hideFlags;
				if (visible)
				{
					this.RemoveHidden(c);
					c.component.hideFlags = hideFlag & (HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset | HideFlags.DontSave | HideFlags.HideAndDontSave);
					return;
				}
				this.AddHidden(c);
				c.component.hideFlags = hideFlag | HideFlags.HideInInspector;
			});
		}

		public void ToggleAll()
		{
			bool flag = !this._components.Values.Any<ApexComponentMaster.ComponentInfo>((ApexComponentMaster.ComponentInfo c) => c.isVisible);
			if (!flag)
			{
				this._hiddenComponents = 2147483647 >> (31 - this._components.Values.Count & 31);
			}
			else
			{
				this._hiddenComponents = 0;
			}
			foreach (ApexComponentMaster.ComponentInfo value in this._components.Values)
			{
				value.isVisible = flag;
				HideFlags hideFlag = value.component.hideFlags;
				value.component.hideFlags = (flag ? hideFlag & (HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset | HideFlags.DontSave | HideFlags.HideAndDontSave) : hideFlag | HideFlags.HideInInspector);
			}
		}

		public class ComponentCandidate
		{
			public MonoBehaviour component;

			public string categoryName;

			public ComponentCandidate()
			{
			}
		}

		public class ComponentCategory : DynamicArray<ApexComponentMaster.ComponentInfo>
		{
			public bool isOpen;

			public string name;

			public ComponentCategory() : base(5)
			{
			}
		}

		public class ComponentInfo
		{
			public MonoBehaviour component;

			public ApexComponentMaster.ComponentCategory category;

			public string name;

			public int id;

			public int idx;

			public bool isVisible;

			public ComponentInfo()
			{
			}
		}
	}
}