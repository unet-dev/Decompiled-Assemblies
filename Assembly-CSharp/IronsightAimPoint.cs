using System;
using UnityEngine;

public class IronsightAimPoint : MonoBehaviour
{
	public Transform targetPoint;

	public IronsightAimPoint()
	{
	}

	private void DrawArrow(Vector3 start, Vector3 end, float arrowLength)
	{
		Vector3 vector3 = (end - start).normalized;
		Vector3 vector31 = Camera.current.transform.up;
		Gizmos.DrawLine(start, end);
		Gizmos.DrawLine(end, (end + (vector31 * arrowLength)) - (vector3 * arrowLength));
		Gizmos.DrawLine(end, (end - (vector31 * arrowLength)) - (vector3 * arrowLength));
		Gizmos.DrawLine((end + (vector31 * arrowLength)) - (vector3 * arrowLength), (end - (vector31 * arrowLength)) - (vector3 * arrowLength));
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Vector3 vector3 = this.targetPoint.position - base.transform.position;
		Vector3 vector31 = vector3.normalized;
		Gizmos.color = Color.red;
		this.DrawArrow(base.transform.position, base.transform.position + (vector31 * 0.1f), 0.1f);
		Gizmos.color = Color.cyan;
		this.DrawArrow(base.transform.position, this.targetPoint.position, 0.02f);
		Gizmos.color = Color.yellow;
		this.DrawArrow(this.targetPoint.position, this.targetPoint.position + (vector31 * 3f), 0.02f);
	}
}