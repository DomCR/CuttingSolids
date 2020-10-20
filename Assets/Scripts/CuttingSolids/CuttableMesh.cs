using GeometricUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CuttingSolids
{
	public class CuttableMesh
	{
		private readonly Mesh m_mesh;

		public CuttableMesh(Mesh mesh)
		{
			m_mesh = mesh;
		}
		//************************************************************************************
		public void CutByPlane(Vector3 position, Plane plane, out Mesh rightMesh, out Mesh leftMesh)
		{
			Shape shape = new Shape();

			List<Triangle> leftTriangles = new List<Triangle>();
			List<Triangle> rightTriangles = new List<Triangle>();

			//Get the intersection points and the lines that generate the solid
			for (int i = 0; i < m_mesh.triangles.Length; i += 3)
			{
				List<Vector3> vertices = new List<Vector3>
				{
					m_mesh.vertices[m_mesh.triangles[i]],
					m_mesh.vertices[m_mesh.triangles[i + 1]],
					m_mesh.vertices[m_mesh.triangles[i + 2]]
				};
				List<Vector3> normals = new List<Vector3>
				{
					m_mesh.normals[m_mesh.triangles[i]],
					m_mesh.normals[m_mesh.triangles[i + 1]],
					m_mesh.normals[m_mesh.triangles[i + 2]]
				};
				List<Vector2> uvs = new List<Vector2>
				{
					m_mesh.uv[m_mesh.triangles[i]],
					m_mesh.uv[m_mesh.triangles[i + 1]],
					m_mesh.uv[m_mesh.triangles[i + 2]]
				};

				Triangle triangle = new Triangle(vertices, normals, uvs);

				var intersections = triangle.Cut(position, plane, out List<Triangle> right, out List<Triangle> left);

				if (intersections.Count == 2 && intersections[0] != intersections[1])
				{
					shape.AddEdge(new Line(intersections[0], intersections[1]));
				}

				leftTriangles.AddRange(left);
				rightTriangles.AddRange(right);
			}

			shape.SortVertices();

			leftTriangles.AddRange(shape.CreateTriangles(-plane.normal));
			rightTriangles.AddRange(shape.CreateTriangles(plane.normal));

			shape.DrawTrianglesOnDebug();

			//foreach (var item in leftTriangles)
			//{
			//	item.DrawNCross(Color.yellow);
			//}

			rightMesh = Triangle.CreateMesh(rightTriangles);
			leftMesh = Triangle.CreateMesh(leftTriangles);
		}
	}
}