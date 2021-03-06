﻿using System;
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

			//Make sure they are in clockwise for the indexes
			if (Vector3.Dot(Vector3.Cross(m_tvertices[1].Vertex - m_tvertices[0].Vertex, m_tvertices[2].Vertex - m_tvertices[0].Vertex), m_tvertices[0].Normal) < 0)
			{
				m_tvertices.Reverse();
			}

			createEdges();
		}
		private Triangle(TVertex vertex1, TVertex vertex2, TVertex vertex3)
		{
			m_tvertices.Add(vertex1);
			m_tvertices.Add(vertex2);
			m_tvertices.Add(vertex3);

			//Make sure they are in clockwise for the indexes
			if (Vector3.Dot(Vector3.Cross(m_tvertices[1].Vertex - m_tvertices[0].Vertex, m_tvertices[2].Vertex - m_tvertices[0].Vertex), m_tvertices[0].Normal) < 0)
			{
				m_tvertices.Reverse();
			}

			createEdges();
		}
		private Triangle(List<TVertex> vertices) : this(vertices[0], vertices[1], vertices[2]) { }
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

			//Get intersections
			List<Vector3> intersections = new List<Vector3>();

			for (int i = 0, j = 1, k = 2; i < 3; i++, j++, k++)
			{
				int first = i;
				int second = j % 3;
				int third = k % 3;

				var intersection = getIntersection(m_tvertices[first], m_tvertices[second], position, plane,
					leftSide, rightSide);

				if (intersection == null)
					continue;

				intersections.Add(intersection.Value);

				if (leftSide.Count == 3)
				{
					left.AddRange(createTriangles(leftSide));

					//Remove triangle vertex
					if (leftSide.Contains(m_tvertices[first]))
					{
						leftSide.Remove(m_tvertices[first]);
					}
					if (leftSide.Contains(m_tvertices[second]))
					{
						leftSide.Remove(m_tvertices[second]);
					}
				}
				if (rightSide.Count == 3)
				{
					right.AddRange(createTriangles(rightSide));

					//Remove triangle vertex
					if (rightSide.Contains(m_tvertices[first]))
					{
						rightSide.Remove(m_tvertices[first]);
					}
					if (rightSide.Contains(m_tvertices[second]))
					{
						rightSide.Remove(m_tvertices[second]);
					}
				}

			}

			//if (intersections.Count > 1)
			//{
			//	this.DrawOnDebug(Color.green);

			//	//new Line(m_tvertices[0].Vertex, m_tvertices[1].Vertex).DrawOnDebug(Color.blue);
			//	//new Line(m_tvertices[1].Vertex, m_tvertices[2].Vertex).DrawOnDebug(Color.blue);
			//	//new Line(m_tvertices[2].Vertex, m_tvertices[0].Vertex).DrawOnDebug(Color.blue);

			//	left.ForEach(o => o.DrawNCross(Color.yellow));
			//	//right.ForEach(o => o.DrawNCross(Color.blue));
			//}

			return intersections;
		}
		public Mesh AddToMesh(Mesh source)
		{
			Mesh dest = new Mesh();

			int[] indexes = new int[3];
			for (int i = 0; i < 3; i++)
			{
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
			//dest.RecalculateNormals();
			//dest.Optimize();

			return dest;
		}
		public void DrawOnDebug(Color color)
		{
			foreach (var item in Edges)
			{
				item.DrawOnDebug(color);
			}
		}
		public void DrawNCross(Color color)
		{
			foreach (var item in Edges)
			{
				item.DrawOnDebug(color);
			}

			for (int i = 0; i < 2; i++)
			{
				new Line(Edges[i].StartPoint, Edges[i + 1].MidPoint()).DrawOnDebug(Color.red);
			}
		}
		//************************************************************************************
		public static Mesh CreateMesh(List<Triangle> input)
		{
			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();
			List<int> triangles = new List<int>();

			foreach (Triangle item in input)
			{
				triangles.AddRange(new int[]
				{
					vertices.Count,
					vertices.Count + 1,
					vertices.Count + 2
				});

				vertices.AddRange(item.Vertices);
				normals.AddRange(item.Normals);
				uvs.AddRange(item.UVs);
			}

			Mesh mesh = new Mesh();

			mesh.SetVertices(vertices);
			mesh.SetNormals(normals);
			mesh.uv = uvs.ToArray();
			mesh.triangles = triangles.ToArray();

			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
			mesh.Optimize();

			return mesh;
		}
		//************************************************************************************
		private void createEdges()
		{
			Edges.Add(new Line(Vertices[0], Vertices[1]));
			Edges.Add(new Line(Vertices[1], Vertices[2]));
			Edges.Add(new Line(Vertices[2], Vertices[0]));
		}
		private Vector3? getIntersection(TVertex start, TVertex end, Vector3 position, Plane plane, List<TVertex> leftSide, List<TVertex> rightSide)
		{
			Line edge = new Line(start.Vertex, end.Vertex);
			Vector3? intersection = edge.PlaneIntersection(position, plane);

			if (intersection != null)
			{
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

			return intersection;
		}
		private Vector3? getIntersection(TVertex start, TVertex end, Vector3 position, Plane plane, out TVertex? leftSide, out TVertex? rightSide)
		{
			Line edge = new Line(start.Vertex, end.Vertex);
			Vector3? intersection = edge.PlaneIntersection(position, plane);

			leftSide = null;
			rightSide = null;

			if (intersection != null)
			{
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

				leftSide = left;
				rightSide = right;
			}

			return intersection;
		}
		//************************************************************************************
		private static List<Triangle> createTriangles(List<TVertex> vertices)
		{
			List<Triangle> triangles = new List<Triangle>();

			//Create as many triangles as need
			for (int i = 0; i + 2 < vertices.Count; i++)
			{
				if (vertices[i].Vertex == vertices[i + 1].Vertex ||
					vertices[i].Vertex == vertices[i + 2].Vertex ||
					vertices[i + 1].Vertex == vertices[i + 2].Vertex)
					continue;

				triangles.Add(new Triangle(vertices[i], vertices[i + 1], vertices[i + 2]));
			}

			return triangles;
		}
	}
}
