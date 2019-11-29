using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GeometricUtilities
{
    public class Shape
    {
        public List<Vector3> Vertices { get; set; }
        public List<Line> Edges { get; set; }

        public Shape()
        {
            Edges = new List<Line>();
            Vertices = new List<Vector3>();
        }

        public void AddLine(Line edge)
        {
            Edges.Add(edge);
        }
        public void SetSortedVertices()
        {
            Vertices.Clear();
            
            foreach (Line item in Edges)
            {
                Vector3 start = item.StartPoint;

                foreach (Line lin in Edges)
                {
                    //Guard
                    if (item.Equals(lin))
                        continue;

                    if (lin.GetPoints().Contains(start))
                    {
                        //Connection between lines found
                    }
                }
            }
        }
        //*********************************************************************************
        public void DrawShape()
        {
            foreach (Line l in Edges)
            {
                l.DrawGizmos(Color.green);
            }
        }
    }
}
