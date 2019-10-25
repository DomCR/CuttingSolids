using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingSolids.GeometricUtilities
{
    public class Line
    {
        public Vector3 StartPoint { get; set; }
        public Vector3 EndPoint { get; set; }
        public Vector3 Vector
        {
            get
            {
                return StartPoint - EndPoint;
            }
        }

        public Line(Vector3 start, Vector3 end)
        {
            StartPoint = start;
            EndPoint = end;
        }

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

        public Vector3? PlaneIntersection(Vector3 planeNormal, Vector3 planeOrigin)
        {
            if (Vector3.Dot(this.Vector.normalized, planeNormal) == 0)
                return null;

            float tp = Vector3.Dot(planeNormal, planeOrigin) - Vector3.Dot(planeNormal, this.StartPoint) /
                Vector3.Dot(planeNormal, this.Vector.normalized);

            return this.StartPoint + this.Vector.normalized * tp;
        }
    }
}