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

		[Obsolete]
		public List<Vector2> UV { get; set; } = new List<Vector2>();
		[Obsolete]
		public List<int> Triangles { get; set; } = new List<int>();

		public Shape() { }
		//*********************************************************************************
		public void AddEdge(Line edge)
		{
			Edges.Add(edge);
			Vertices.AddRange(edge.GetPoints());

			//SortVertices();
		}
		public void SortVertices()
		{
			if (!Edges.Any())
				return;

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
		[Obsolete]
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
		public List<Triangle> CreateTriangles(Vector3 normal)
		{
			if (!Edges.Any())
				return new List<Triangle>();

			List<Triangle> triangles = new List<Triangle>();

			//Clear the vertices
			Vertices.Clear();
			UV.Clear();

			//Create the new vertices with the uv
			foreach (Line l in Edges)
			{
				Vertices.Add(l.StartPoint);
				UV.Add(new Vector2());
			}

			Vertices.Add(Edges.Last().EndPoint);
			UV.Add(new Vector2());

			//Create the triangles
			for (int i = 1; i < Vertices.Count - 1; i++)
			{
				if (Vertices[0] == Vertices[i] ||
					Vertices[0] == Vertices[i + 1])
					continue;

				List<Vector3> vertices = new List<Vector3>
				{
					Vertices[0],
					Vertices[i ],
					Vertices[i +1]
				};
			List<Vector3> normals = new List<Vector3>
				{
					normal,normal,normal
				};
			List<Vector2> uvs = new List<Vector2>
				{
					UV[0],
					UV[i ],
					UV[i + 1]
				};

			triangles.Add(new Triangle(vertices, normals, uvs));
		}

			return triangles;
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
	public void DrawTrianglesOnDebug()
	{
		if (!Vertices.Any())
			return;

		Vector3 pivot = Vertices[0];
		for (int i = 1; i < Vertices.Count - 1; i++)
		{
			new Line(pivot, Vertices[i]).DrawOnDebug(Color.green);
			new Line(pivot, Vertices[i + 1]).DrawOnDebug(Color.green);
			new Line(Vertices[i], Vertices[i + 1]).DrawOnDebug(Color.green);
		}
	}
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

public class Edge
{
	public TVertex StartVertex { get; set; }
	public TVertex EndVertex { get; set; }
	public Edge(TVertex start, TVertex end)
	{
		StartVertex = start;
		EndVertex = end;
	}
	//*********************************************************************************
	public TVertex[] GetPoints()
	{
		return new TVertex[] { StartVertex, EndVertex };
	}
}

public class Section
{
	public List<Vector3> Vertices { get { return m_tvertices.Select(o => o.Vertex).ToList(); } }
	public List<Vector3> Normals { get { return m_tvertices.Select(o => o.Normal).ToList(); } }
	public List<Vector2> UVs { get { return m_tvertices.Select(o => o.UV).ToList(); } }
	public List<Edge> Edges { get; set; } = new List<Edge>();

	private List<TVertex> m_tvertices = new List<TVertex>();
	public Section() { }
	public Section(IEnumerable<TVertex> vertices)
	{
		if (vertices.Count() < 2)
			return;

		m_tvertices = new List<TVertex>(vertices);
		for (int i = 0; i < vertices.Count() - 1; i++)
		{
			Edges.Add(new Edge(vertices.ElementAt(i), vertices.ElementAt(i + 1)));
		}

		sortVertices();
	}
	//*********************************************************************************
	public void AddEdge(Edge edge)
	{
		Edges.Add(edge);
		m_tvertices.AddRange(edge.GetPoints());

		sortVertices();
	}
	//*********************************************************************************
	private void sortVertices()
	{
		if (!Edges.Any())
			return;

		List<Edge> sortedEdges = new List<Edge>();
		Queue<Edge> edgesCopy = new Queue<Edge>(Edges);

		Edge currEdge = edgesCopy.Dequeue();
		while (edgesCopy.Count > 0)
		{
			//Add the current edge
			sortedEdges.Add(currEdge);

			foreach (Edge item in edgesCopy)
			{
				if (item.GetPoints().Contains(currEdge.EndVertex))
				{

				}
			}

			currEdge = edgesCopy.Dequeue();
		}
	}
}
}
