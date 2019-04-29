using System;
using System.Collections.Generic;
using UnityEngine;

public class ModelConditionTest_WallTriangleLeft : ModelConditionTest
{
	private const string socket_1 = "wall/sockets/wall-female";

	private const string socket_2 = "wall/sockets/floor-female/1";

	private const string socket_3 = "wall/sockets/floor-female/2";

	private const string socket_4 = "wall/sockets/stability/1";

	private const string socket = "wall/sockets/neighbour/1";

	public ModelConditionTest_WallTriangleLeft()
	{
	}

	public static bool CheckCondition(BaseEntity ent)
	{
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/wall-female"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/1"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/2"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/stability/1"))
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink("wall/sockets/neighbour/1");
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock item = entityLink.connections[i].owner as BuildingBlock;
			if (!(item == null) && !(item.blockDefinition.info.name.token != "roof") && Vector3.Angle(ent.transform.forward, item.transform.forward) <= 10f)
			{
				return true;
			}
		}
		return false;
	}

	private static bool CheckSocketOccupied(BaseEntity ent, string socket)
	{
		EntityLink entityLink = ent.FindLink(socket);
		if (entityLink == null)
		{
			return false;
		}
		return !entityLink.IsEmpty();
	}

	public override bool DoTest(BaseEntity ent)
	{
		return ModelConditionTest_WallTriangleLeft.CheckCondition(ent);
	}
}