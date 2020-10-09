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
		public List<Vector3> Vertices { get; set; } = new List<Vector3>();
		public List<Line> Edges { get; set; } = new List<Line>();
		public List<Vector2> UV { get; set; } = new List<Vector2>();
		public List<int> Triangles { get; set; } = new List<int>();

		public Shape() { }
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
				foreach (Line line in edgesCopy)
				{
					//if (l.GetPoints().Contains(currLine.EndPoint))
					if (containsPoint(line.GetPoints(), currLine.EndPoint, 5))
					{
						Vector3 vtmp = line.GetPoints().Where(o => !almostEqual(o, currLine.EndPoint, 5)).First();
						Line tmp = new Line(currLine.EndPoint, vtmp);
						toRemove = line;
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
			UV.Clear();

			foreach (Line l in Edges)
			{
				Vertices.Add(l.StartPoint);
				UV.Add(new Vector2());
			}

			Vertices.Add(Edges.Last().EndPoint);
			UV.Add(new Vector2());

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
		private bool containsPoint(Vector3[] arr, Vector3 point, int decimals)
		{
			foreach (Vector3 v in arr)
			{
				if (almostEqual(v, point, decimals))
					return true;
			}

			return false;
		}
		private bool almostEqual(Vector3 a, Vector3 b, int decimals)
		{
			if (Math.Round(a.x, decimals) == Math.Round(b.x, decimals)
					&& Math.Round(a.y, decimals) == Math.Round(b.y, decimals)
					&& Math.Round(a.z, decimals) == Math.Round(b.z, decimals))
			{
				return true;
			}
			else
				return false;
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
