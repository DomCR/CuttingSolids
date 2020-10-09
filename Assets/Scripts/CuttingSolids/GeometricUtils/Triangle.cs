using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;

namespace GeometricUtilities
{

	public class Triangle
	{
		public List<Vector3> Vertices { get { return m_tvertices.Select(o => o.Vertex).ToList(); } }
		public List<Vector3> Normals { get { return m_tvertices.Select(o => o.Normal).ToList(); } }
		public List<Vector2> UVs { get { return m_tvertices.Select(o => o.UV).ToList(); } }

		private List<TVertex> m_tvertices = new List<TVertex>();
		public int SubmeshIndex = 0;

		public List<Line> Edges { get; } = new List<Line>(3);
		public Triangle(IEnumerable<Vector3> vertices, IEnumerable<Vector3> normals, IEnumerable<Vector2> uvs)
		{
			for (int i = 0; i < 3; i++)
			{
				m_tvertices.Add(new TVertex
				{
					Vertex = vertices.ElementAt(i),
					Normal = normals.ElementAt(i),
					UV = uvs.ElementAt(i),
				});
			}

			createEdges();
		}
		private Triangle(TVertex vertex1, TVertex vertex2, TVertex vertex3)
		{
			m_tvertices.Add(vertex1);
			m_tvertices.Add(vertex2);
			m_tvertices.Add(vertex3);

			createEdges();
		}
		//************************************************************************************
		public List<Vector3> Cut(Vector3 position, Plane plane, out List<Triangle> right, out List<Triangle> left)
		{
			//Initialize list
			right = new List<Triangle>();
			left = new List<Triangle>();

			List<TVertex> leftSide = new List<TVertex>();
			List<TVertex> rightSide = new List<TVertex>();

			//Split the vertex by the side of the plane
			foreach (TVertex t in m_tvertices)
			{
				if (plane.GetSide(t.Vertex))
				{
					leftSide.Add(t);
				}
				else
				{
					rightSide.Add(t);
				}
			}

			//The triangle does not cut with the plane
			if (rightSide.Count == 0 || leftSide.Count == 0)
			{
				if (rightSide.Count == 0)
				{
					left.Add(this);
				}
				else
				{
					right.Add(this);
				}
				//no intersections, return empty list
				return new List<Vector3>();
			}

			List<Vector3> intersections = new List<Vector3>();
			intersections.AddRange(getIntersection(m_tvertices[0], m_tvertices[1], position, plane, leftSide, rightSide));
			intersections.AddRange(getIntersection(m_tvertices[1], m_tvertices[2], position, plane, leftSide, rightSide));
			intersections.AddRange(getIntersection(m_tvertices[2], m_tvertices[0], position, plane, leftSide, rightSide));

			//foreach (Line edge in Edges)
			//{
			//	Vector3? intersection = edge.PlaneIntersection(position, plane);

			//	if (intersection != null)
			//	{
			//		//Save the intersection
			//		intersections.Add(intersection.Value);

			//		TVertex tv = new TVertex();
			//		tv.Vertex = intersection.Value;

			//		//Add the intersection to create the new triangles
			//		if (plane.GetSide(edge.StartPoint))
			//		{
			//			tv.UV = edge
			//		}

			//		leftSide.Add(intersection.Value);
			//		rightSide.Add(intersection.Value);
			//	}
			//}

			//Create the triangles
			right.AddRange(createTriangles(rightSide));
			left.AddRange(createTriangles(leftSide));

			return intersections;
		}
		public Mesh AddToMesh(Mesh source)
		{
			Mesh dest = new Mesh();

			int[] indexes = new int[3];
			for (int i = 0; i < 3; i++)
			{
				//if (mesh.vertices.Contains(Vertices[i]))
				//{
				//	indexes[i] = Array.IndexOf(mesh.vertices, Vertices[i]);
				//}
				//else
				//{
				//	List<Vector3> vertices = new List<Vector3>(mesh.vertices);
				//	vertices.Add(Vertices[i]);
				//	mesh.vertices = vertices.ToArray();

				//	indexes[i] = mesh.vertices.Length - 1;
				//}


				indexes[i] = source.vertices.Length + i;
			}

			List<Vector3> vertices = new List<Vector3>(source.vertices);
			vertices.AddRange(Vertices);
			dest.SetVertices(vertices);

			List<Vector3> normals = new List<Vector3>(source.normals);
			normals.AddRange(this.Normals);
			dest.SetNormals(normals);

			List<Vector2> uv = new List<Vector2>(source.uv);
			uv.AddRange(this.UVs);
			dest.uv = uv.ToArray();

			List<int> triangles = new List<int>(source.triangles);
			triangles.AddRange(indexes);
			dest.SetTriangles(triangles, 0);

			//dest.RecalculateTangents();
			dest.RecalculateNormals();
			dest.Optimize();

			return dest;
		}
		public void DrawOnDebug(Color color)
		{
			foreach (var item in Edges)
			{
				item.DrawOnDebug(color);
			}
		}
		//************************************************************************************
		public static Mesh CreateMesh(List<Triangle> triangles)
		{

			throw new NotImplementedException();
		}
		//************************************************************************************
		private void createEdges()
		{
			Edges.Add(new Line(Vertices[0], Vertices[1]));
			Edges.Add(new Line(Vertices[1], Vertices[2]));
			Edges.Add(new Line(Vertices[2], Vertices[0]));
		}
		private List<Vector3> getIntersection(TVertex start, TVertex end, Vector3 position, Plane plane, List<TVertex> leftSide, List<TVertex> rightSide)
		{
			List<Vector3> intersections = new List<Vector3>();

			Line edge = new Line(start.Vertex, end.Vertex);
			Vector3? intersection = edge.PlaneIntersection(position, plane);

			if (intersection != null)
			{
				//Save the intersection
				intersections.Add(intersection.Value);

				TVertex left = new TVertex();
				TVertex right = new TVertex();
				left.Vertex = intersection.Value;
				right.Vertex = intersection.Value;


				//Add the intersection to create the new triangles
				if (plane.GetSide(edge.StartPoint))
				{
					left.UV = end.UV;
					left.Normal = end.Normal;

					right.UV = start.UV;
					right.Normal = start.Normal;
				}
				else
				{
					right.UV = end.UV;
					right.Normal = end.Normal;

					left.UV = start.UV;
					left.Normal = start.Normal;
				}

				leftSide.Add(left);
				rightSide.Add(right);
			}

			return intersections;
		}
		//************************************************************************************
		private static List<Triangle> createTriangles(List<TVertex> vertices)
		{
			List<Triangle> triangles = new List<Triangle>();

			if (vertices.Count == 3)
			{
				triangles.Add(new Triangle(vertices[0], vertices[1], vertices[2]));
			}
			else
			{
				//Create as many triangles as need
				for (int i = 0; i + 2 < vertices.Count; i++)
				{
					triangles.Add(new Triangle(vertices[i], vertices[i + 1], vertices[i + 2]));
				}
			}

			return triangles;
		}
	}
}
