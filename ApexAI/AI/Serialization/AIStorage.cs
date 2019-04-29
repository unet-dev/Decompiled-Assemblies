using Apex.AI;
using System;
using UnityEngine;

namespace Apex.AI.Serialization
{
	public sealed class AIStorage : ScriptableObject, IHaveFriendlyName
	{
		[TextArea(1, 20)]
		public string description;

		[HideInInspector]
		public int version;

		[HideInInspector]
		public string aiId;

		[HideInInspector]
		public string configuration;

		[HideInInspector]
		public string editorConfiguration;

		string Apex.AI.IHaveFriendlyName.description
		{
			get
			{
				return this.description;
			}
		}

		string Apex.AI.IHaveFriendlyName.friendlyName
		{
			get
			{
				return base.name;
			}
		}

		public AIStorage()
		{
		}

		public static AIStorage Create(string aiId, string aiName)
		{
			AIStorage aIStorage = ScriptableObject.CreateInstance<AIStorage>();
			aIStorage.name = aiName;
			aIStorage.aiId = aiId;
			return aIStorage;
		}
	}
}