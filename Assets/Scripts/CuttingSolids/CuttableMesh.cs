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
			rightMesh = new Mesh();
			leftMesh = new Mesh();

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

				triangle.Cut(position, plane, out List<Triangle> right, out List<Triangle> left);

				foreach (var item in right)
				{
					rightMesh = item.AddToMesh(rightMesh);
					item.DrawOnDebug(Color.yellow);
				}

				foreach (var item in left)
				{
					leftMesh = item.AddToMesh(leftMesh);

					item.DrawOnDebug(Color.green);
				}

			}
		}
		//************************************************************************************
		/// <summary>
		/// Create a reference point if this intersects with the plane.
		/// </summary>
		/// <param name="edge"></param>
		void createReference(Line edge, Vector3 position, Plane plane)
		{
			Vector3? intersection = edge.PlaneIntersection(position, plane);

		}
	}
}