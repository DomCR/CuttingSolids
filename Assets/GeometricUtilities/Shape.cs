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
        public List<int> Triangles { get; set; }

        public Shape()
        {
            Edges = new List<Line>();
            Vertices = new List<Vector3>();
            Triangles = new List<int>();
        }
        //*********************************************************************************
        public void AddLine(Line edge)
        {
            Edges.Add(edge);
        }
        public void SetSortedVertices()
        {
            Vertices.Clear();
            List<Line> sortedLines = new List<Line>();
            List<Line> edgesCopy = new List<Line>();
            foreach (Line e in Edges)
            {
                Line tmp = new Line(e.StartPoint, e.EndPoint);
                edgesCopy.Add(tmp);
            }

            bool closed = false;
            Line currLine = edgesCopy.First();
            Line toRemove = null;
            while (!closed)
            {
                foreach (Line l in edgesCopy)
                {
                    if (l.GetPoints().Contains(currLine.EndPoint))
                    {
                        Line tmp = new Line(currLine.EndPoint, l.GetPoints().Where(p => !p.Equals(currLine.EndPoint)).First());
                        toRemove = l;
                        sortedLines.Add(tmp);
                        currLine = tmp;
                        break;
                    }
                    else
                    {
                        toRemove = null;
                    }
                }

                if (toRemove == null)
                    break;
                else
                    edgesCopy.Remove(toRemove);

                if (edgesCopy.Count == 0)
                    closed = true;
            }

            Edges = sortedLines;
        }
        public void ComputeMesh()
        {
            Vertices.Clear();
            foreach (Line l in Edges)
            {
                Vertices.Add(l.StartPoint);
            }
            Vertices.Add(Edges.Last().EndPoint);

            Triangles.Clear();
            for (int i = 1; i < Vertices.Count - 1; i++)
            {
                int[] tmp = new int[3];
                tmp[0] = 0;
                tmp[1] = i;
                tmp[2] = i + 1;

                Triangles.AddRange(tmp);
            }
        }
        //*********************************************************************************
        public void DrawTriangles()
        {
            Gizmos.color = Color.blue;
            //Gizmos.DrawLine(Vertices[0], Vertices[1]);
            //Gizmos.DrawLine(Vertices[1], Vertices[2]);
            //Gizmos.DrawLine(Vertices[2], Vertices[0]);

            Vector3 pivot = Vertices[0];
            for (int i = 1; i < Vertices.Count - 1; i++)
            {
                int[] tmp = new int[3];
                tmp[0] = 0;
                tmp[1] = i;
                tmp[2] = i + 1;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(pivot, Vertices[i]);
                Gizmos.DrawLine(pivot, Vertices[i + 1]);
                Gizmos.DrawLine(Vertices[i], Vertices[i + 1]);
            }
        }
        public void DrawShape()
        {
            foreach (Line l in Edges)
            {
                l.DrawGizmos(Color.green);
            }
        }
    }
}
