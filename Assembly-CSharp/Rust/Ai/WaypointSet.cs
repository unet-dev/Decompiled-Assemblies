using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class WaypointSet : MonoBehaviour, IServerComponent
	{
		[SerializeField]
		private List<WaypointSet.Waypoint> _points = new List<WaypointSet.Waypoint>();

		[SerializeField]
		private WaypointSet.NavModes navMode;

		public WaypointSet.NavModes NavMode
		{
			get
			{
				return this.navMode;
			}
		}

		public List<WaypointSet.Waypoint> Points
		{
			get
			{
				return this._points;
			}
			set
			{
				this._points = value;
			}
		}

		public WaypointSet()
		{
		}

		private void OnDrawGizmos()
		{
			for (int i = 0; i < this.Points.Count; i++)
			{
				Transform transform = this.Points[i].Transform;
				if (transform != null)
				{
					if (!this.Points[i].IsOccupied)
					{
						Gizmos.color = Color.cyan;
					}
					else
					{
						Gizmos.color = Color.red;
					}
					Gizmos.DrawSphere(transform.position, 0.25f);
					Gizmos.color = Color.cyan;
					if (i + 1 < this.Points.Count)
					{
						Gizmos.DrawLine(transform.position, this.Points[i + 1].Transform.position);
					}
					else if (this.NavMode == WaypointSet.NavModes.Loop)
					{
						Gizmos.DrawLine(transform.position, this.Points[0].Transform.position);
					}
					Gizmos.color = Color.magenta - new Color(0f, 0f, 0f, 0.5f);
					Transform[] lookatPoints = this.Points[i].LookatPoints;
					for (int j = 0; j < (int)lookatPoints.Length; j++)
					{
						Transform transforms = lookatPoints[j];
						Gizmos.DrawSphere(transforms.position, 0.1f);
						Gizmos.DrawLine(transform.position, transforms.position);
					}
				}
			}
		}

		public enum NavModes
		{
			Loop,
			PingPong
		}

		[Serializable]
		public struct Waypoint
		{
			public Transform Transform;

			public float WaitTime;

			public Transform[] LookatPoints;

			[NonSerialized]
			public bool IsOccupied;
		}
	}
}