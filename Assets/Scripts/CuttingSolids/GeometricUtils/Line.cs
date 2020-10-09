using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometricUtilities
{
	public class Line
	{
		public Vector3 StartPoint { get; set; }
		public Vector3 EndPoint { get; set; }
		public Vector3 Vector
		{
			get
			{
				return EndPoint - StartPoint;
			}
		}

		public Line(Vector3 start, Vector3 end)
		{
			StartPoint = start;
			EndPoint = end;
		}
		public Line(Vector3 start, Vector3 end, Transform transform)
		{
			StartPoint = transform.TransformPoint(start);
			EndPoint = transform.TransformPoint(end);
		}
		public Vector3? PlaneIntersection(Vector3 position, Plane plane, bool insideLine = true)
		{
			return PlaneIntersection(plane.normal, position, insideLine);
		}
		public Vector3? PlaneIntersection(Vector3 planeNormal, Vector3 planeOrigin, bool insideLine = true)
		{
			//Parallel to plane, does not intersect
			if (Vector3.Dot(this.Vector.normalized, planeNormal) == 0)
				return null;

			float tp = (Vector3.Dot(planeNormal, planeOrigin) - Vector3.Dot(planeNormal, this.StartPoint)) /
				Vector3.Dot(planeNormal, this.Vector.normalized);

			Vector3 intersection = this.StartPoint + this.Vector.normalized * tp;

			//Check if is inside the line 
			if (insideLine)
			{
				Vector3 min = new Vector3();
				min.x = StartPoint.x < EndPoint.x ? StartPoint.x : EndPoint.x;
				min.y = StartPoint.y < EndPoint.y ? StartPoint.y : EndPoint.y;
				min.z = StartPoint.z < EndPoint.z ? StartPoint.z : EndPoint.z;

				Vector3 max = new Vector3();
				max.x = StartPoint.x > EndPoint.x ? StartPoint.x : EndPoint.x;
				max.y = StartPoint.y > EndPoint.y ? StartPoint.y : EndPoint.y;
				max.z = StartPoint.z > EndPoint.z ? StartPoint.z : EndPoint.z;

				if (min.x <= intersection.x && intersection.x <= max.x &&
					min.y <= intersection.y && intersection.y <= max.y &&
					min.z <= intersection.z && intersection.z <= max.z)
				{
					return intersection;
				}
				else
				{
					return null;
				}
			}

			return this.StartPoint + this.Vector.normalized * tp;
		}
		public Vector3[] GetPoints()
		{
			return new Vector3[] { StartPoint, EndPoint };
		}
		//*********************************************************************************
		/// <summary>
		/// Draw the Line using Gizmos in unity interface
		/// </summary>
		/// <param name="color"></param>
		public void DrawGizmos(Color color)
		{
			Color tmpColor = Gizmos.color;
			Gizmos.color = color;
			Gizmos.DrawLine(StartPoint, EndPoint);
			Gizmos.color = tmpColor;
		}
		public void DrawOnDebug(Color color)
		{
			Debug.DrawLine(StartPoint, EndPoint, color);
		}
	}
}