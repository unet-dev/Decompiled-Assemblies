using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Shims;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal class ReflectionObject
	{
		public ObjectConstructor<object> Creator
		{
			get;
			private set;
		}

		public IDictionary<string, ReflectionMember> Members
		{
			get;
			private set;
		}

		public ReflectionObject()
		{
			this.Members = new Dictionary<string, ReflectionMember>();
		}

		public static ReflectionObject Create(Type t, params string[] memberNames)
		{
			return ReflectionObject.Create(t, null, memberNames);
		}

		public static ReflectionObject Create(Type t, MethodBase creator, params string[] memberNames)
		{
			MemberInfo memberInfo;
			ReflectionMember reflectionMember;
			MemberTypes memberType;
			ReflectionObject reflectionObject = new ReflectionObject();
			ReflectionDelegateFactory reflectionDelegateFactory = JsonTypeReflector.ReflectionDelegateFactory;
			if (creator != null)
			{
				reflectionObject.Creator = reflectionDelegateFactory.CreateParameterizedConstructor(creator);
			}
			else if (ReflectionUtils.HasDefaultConstructor(t, false))
			{
				Func<object> func = reflectionDelegateFactory.CreateDefaultConstructor<object>(t);
				reflectionObject.Creator = (object[] args) => func();
			}
			string[] strArrays = memberNames;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				MemberInfo[] member = t.GetMember(str, BindingFlags.Instance | BindingFlags.Public);
				if ((int)member.Length != 1)
				{
					throw new ArgumentException("Expected a single member with the name '{0}'.".FormatWith(CultureInfo.InvariantCulture, str));
				}
				memberInfo = ((IEnumerable<MemberInfo>)member).Single<MemberInfo>();
				reflectionMember = new ReflectionMember();
				memberType = memberInfo.MemberType();
				if (memberType != MemberTypes.Field)
				{
					if (memberType != MemberTypes.Method)
					{
						goto Label1;
					}
					MethodInfo methodInfo = (MethodInfo)memberInfo;
					if (methodInfo.IsPublic)
					{
						ParameterInfo[] parameters = methodInfo.GetParameters();
						if (parameters.Length == 0 && methodInfo.ReturnType != typeof(void))
						{
							MethodCall<object, object> methodCall = reflectionDelegateFactory.CreateMethodCall<object>(methodInfo);
							reflectionMember.Getter = (object target) => methodCall(target, new object[0]);
						}
						else if ((int)parameters.Length == 1 && methodInfo.ReturnType == typeof(void))
						{
							MethodCall<object, object> methodCall1 = reflectionDelegateFactory.CreateMethodCall<object>(methodInfo);
							reflectionMember.Setter = (object target, object arg) => methodCall1(target, new object[] { arg });
						}
					}
				}
				else
				{
					goto Label0;
				}
			Label2:
				if (ReflectionUtils.CanReadMemberValue(memberInfo, false))
				{
					reflectionMember.Getter = reflectionDelegateFactory.CreateGet<object>(memberInfo);
				}
				if (ReflectionUtils.CanSetMemberValue(memberInfo, false, false))
				{
					reflectionMember.Setter = reflectionDelegateFactory.CreateSet<object>(memberInfo);
				}
				reflectionMember.MemberType = ReflectionUtils.GetMemberUnderlyingType(memberInfo);
				reflectionObject.Members[str] = reflectionMember;
			}
			return reflectionObject;
		Label0:
			if (ReflectionUtils.CanReadMemberValue(memberInfo, false))
			{
				reflectionMember.Getter = reflectionDelegateFactory.CreateGet<object>(memberInfo);
			}
			if (ReflectionUtils.CanSetMemberValue(memberInfo, false, false))
			{
				reflectionMember.Setter = reflectionDelegateFactory.CreateSet<object>(memberInfo);
				goto Label2;
			}
			else
			{
				goto Label2;
			}
		Label1:
			if (memberType != MemberTypes.Property)
			{
				throw new ArgumentException("Unexpected member type '{0}' for member '{1}'.".FormatWith(CultureInfo.InvariantCulture, memberInfo.MemberType(), memberInfo.Name));
			}
			else
			{
				goto Label0;
			}
		}

		public Type GetType(string member)
		{
			return this.Members[member].MemberType;
		}

		public object GetValue(object target, string member)
		{
			return this.Members[member].Getter(target);
		}

		public void SetValue(object target, string member, object value)
		{
			this.Members[member].Setter(target, value);
		}
	}
}