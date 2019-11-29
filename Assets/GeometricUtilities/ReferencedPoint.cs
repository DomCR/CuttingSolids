using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GeometricUtilities
{
    public class ReferencedPoint
    {
        public Vector3 Original { get; set; }
        public Vector2 Reference { get; set; }

        public bool IsConnected
        {
            get
            {
                return (Connections[0] != null && Connections[1] != null);
            }
        }
        public ReferencedPoint[] Connections { get; set; }

        public ReferencedPoint()
        {
            Connections = new ReferencedPoint[2];
        }

        public ReferencedPoint(Vector3 original, Vector3 uVect, Vector3 vVect) : this()
        {
            this.Original = original;
            this.Reference = new Vector2(Vector3.Dot(Original, uVect), Vector3.Dot(Original, vVect));
        }

        public ReferencedPoint(Vector3 original, Map2D map) : this(original, map.UVect, map.VVect)
        {

        }
        public Vector3[] GetTriangle()
        {
            Vector3[] triangle = new Vector3[3];

            triangle[0] = Original;
            triangle[1] = Connections[0].Original;
            triangle[2] = Connections[1].Original;

            return triangle;
        }
        public bool SetConnection(ReferencedPoint point)
        {
            Vector3[] tri_1 = point.GetTriangle();

            if (tri_1.Contains(this.Original))
            {
                this.AddConnection(point);
                return true;
            }

            return false;
        }
        public void AddConnection(ReferencedPoint point)
        {
            if (point == null)
                return;

            if (Connections.Contains(point))
                return;

            for (int i = 0; i < Connections.Length; i++)
            {
                if (Connections[i] == null)
                {
                    Connections[i] = point;
                    break;
                }
            }
        }
        //*********************************************************************************
        public void DrawConnections()
        {
            Gizmos.color = Color.white;
            //Gizmos.DrawLine(Original, Connections.Where(o => o != null).FirstOrDefault().Original);

            Vector3[] tri = GetTriangle();
            Gizmos.DrawLine(tri[0], tri[1]);
            Gizmos.DrawLine(tri[1], tri[2]);
            try
            {
                //if (this.IsConnected)
                //{
                //    Vector3[] tri = GetTriangle();
                //    Gizmos.color = Color.green;
                //    Gizmos.DrawLine(tri[0], tri[1]);
                //    Gizmos.DrawLine(tri[1], tri[2]);
                //    Gizmos.DrawLine(tri[2], tri[0]);
                //}
            }
            catch (Exception ex)
            {

            }
        }
    }
}
