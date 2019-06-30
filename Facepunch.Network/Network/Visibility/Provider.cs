using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Visibility
{
	public interface Provider
	{
		Group GetGroup(Vector3 vPos);

		void GetVisibleFrom(Group group, List<Group> groups);

		bool IsInside(Group group, Vector3 vPos);

		void OnGroupAdded(Group group);
	}
}