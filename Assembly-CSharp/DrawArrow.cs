using System;
using UnityEngine;

public class DrawArrow : MonoBehaviour
{
	public Color color = new Color(1f, 1f, 1f, 1f);

	public float length = 0.2f;

	public float arrowLength = 0.02f;

	public DrawArrow()
	{
	}

	private void OnDrawGizmos()
	{
		Vector3 vector3 = base.transform.forward;
		Vector3 vector31 = Camera.current.transform.up;
		Vector3 vector32 = base.transform.position;
		Vector3 vector33 = base.transform.position + (vector3 * this.length);
		Gizmos.color = this.color;
		Gizmos.DrawLine(vector32, vector33);
		Gizmos.DrawLine(vector33, (vector33 + (vector31 * this.arrowLength)) - (vector3 * this.arrowLength));
		Gizmos.DrawLine(vector33, (vector33 - (vector31 * this.arrowLength)) - (vector3 * this.arrowLength));
		Gizmos.DrawLine((vector33 + (vector31 * this.arrowLength)) - (vector3 * this.arrowLength), (vector33 - (vector31 * this.arrowLength)) - (vector3 * this.arrowLength));
	}
}