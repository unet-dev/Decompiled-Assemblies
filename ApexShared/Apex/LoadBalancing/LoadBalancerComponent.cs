using Apex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex.LoadBalancing
{
	[AddComponentMenu("Apex/Game World/Load Balancer", 1036)]
	[ApexComponent("Game World")]
	public class LoadBalancerComponent : SingleInstanceComponent<LoadBalancerComponent>
	{
		[HideInInspector]
		[SerializeField]
		private LoadBalancerConfig[] _configurations;

		[HideInInspector]
		[SerializeField]
		private int _mashallerMaxMillisecondPerFrame = 4;

		private LoadBalancedQueue[] _loadBalancers;

		private Marshaller _marshaller;

		public LoadBalancerConfig[] configurations
		{
			get
			{
				if (this._loadBalancers == null)
				{
					this.ResolveLoadBalancers();
				}
				return this._configurations;
			}
		}

		public IEnumerable<LoadBalancedQueue> loadBalancers
		{
			get
			{
				if (this._loadBalancers == null)
				{
					this.ResolveLoadBalancers();
				}
				return this._loadBalancers;
			}
		}

		public LoadBalancerComponent()
		{
		}

		protected override void OnAwake()
		{
			this.ResolveLoadBalancers();
			this._marshaller = new Marshaller(this._mashallerMaxMillisecondPerFrame);
			LoadBalancer.marshaller = this._marshaller;
		}

		private void ResolveLoadBalancers()
		{
			LoadBalancerConfig loadBalancerConfig;
			List<LoadBalancedQueue> loadBalancedQueues = new List<LoadBalancedQueue>();
			Dictionary<string, LoadBalancerConfig> strs = new Dictionary<string, LoadBalancerConfig>(StringComparer.Ordinal);
			if (this._configurations != null)
			{
				LoadBalancerConfig[] loadBalancerConfigArray = this._configurations;
				for (int i = 0; i < (int)loadBalancerConfigArray.Length; i++)
				{
					LoadBalancerConfig loadBalancerConfig1 = loadBalancerConfigArray[i];
					strs.Add(loadBalancerConfig1.targetLoadBalancer, loadBalancerConfig1);
				}
			}
			Type type = typeof(LoadBalancer);
			Type type1 = typeof(LoadBalancedQueue);
			Type type2 = typeof(ILoadBalancer);
			Type[] array = (
				from asm in (IEnumerable<Assembly>)AppDomain.CurrentDomain.GetAssemblies()
				from t in asm.GetTypes()
				select new { asm = asm, t = t }).Where((argument0) => {
				if (argument0.t == type)
				{
					return true;
				}
				return argument0.t.IsSubclassOf(type);
			}).Select((argument3) => argument3.t).ToArray<Type>();
			foreach (PropertyInfo propertyInfo in (
				from t in (IEnumerable<Type>)array
				from p in t.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public)
				select new { t = t, p = p }).Where((argument1) => {
				if (!(argument1.p.PropertyType == type1) && !(argument1.p.PropertyType == type2))
				{
					return false;
				}
				return argument1.p.CanRead;
			}).Select((argument4) => argument4.p))
			{
				string name = propertyInfo.Name;
				LoadBalancedQueue value = propertyInfo.GetValue(null, null) as LoadBalancedQueue;
				if (value == null && propertyInfo.CanWrite)
				{
					value = new LoadBalancedQueue(4);
					propertyInfo.SetValue(null, value, null);
				}
				if (value == null)
				{
					continue;
				}
				if (strs.TryGetValue(name, out loadBalancerConfig))
				{
					loadBalancerConfig.ApplyTo(value);
				}
				else
				{
					loadBalancerConfig = LoadBalancerConfig.From(name, value);
					strs.Add(name, loadBalancerConfig);
				}
				loadBalancedQueues.Add(value);
			}
			foreach (FieldInfo fieldInfo in (
				from t in (IEnumerable<Type>)array
				from f in t.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public)
				select new { t = t, f = f }).Where((argument2) => {
				if (argument2.f.FieldType == type1)
				{
					return true;
				}
				return argument2.f.FieldType == type2;
			}).Select((argument5) => argument5.f))
			{
				string str = fieldInfo.Name;
				LoadBalancedQueue loadBalancedQueue = fieldInfo.GetValue(null) as LoadBalancedQueue;
				if (loadBalancedQueue == null && !fieldInfo.IsInitOnly)
				{
					loadBalancedQueue = new LoadBalancedQueue(4);
					fieldInfo.SetValue(null, loadBalancedQueue);
				}
				if (loadBalancedQueue == null)
				{
					continue;
				}
				if (strs.TryGetValue(str, out loadBalancerConfig))
				{
					loadBalancerConfig.ApplyTo(loadBalancedQueue);
				}
				else
				{
					loadBalancerConfig = LoadBalancerConfig.From(str, loadBalancedQueue);
					strs.Add(str, loadBalancerConfig);
				}
				loadBalancedQueues.Add(loadBalancedQueue);
			}
			this._configurations = (
				from c in strs.Values
				where c.associatedLoadBalancer != null
				orderby c.targetLoadBalancer
				select c).ToArray<LoadBalancerConfig>();
			this._loadBalancers = loadBalancedQueues.ToArray();
		}

		private void Update()
		{
			for (int i = 0; i < (int)this._loadBalancers.Length; i++)
			{
				this._loadBalancers[i].Update();
			}
			this._marshaller.ProcessPending();
		}
	}
}