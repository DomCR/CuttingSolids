using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GeometricUtilities;

namespace MeshSection
{
	[RequireComponent(typeof(MeshFilter))]
	public class SectionCreator : MonoBehaviour
	{
		public bool CreateSection;
		public Transform CuttingPlane;

		private Mesh m_mesh;
		private List<Line> m_lines;
		private List<Vector3> m_intersectionPoints;
		private List<ReferencedPoint> m_projections;
		private Map2D m_map;
		private Shape m_shape;
		private GameObject m_section;
		//*********************************************************************************
		#region Behaviour methods
		// Start is called before the first frame update
		void Start()
		{
			Initialize();
		}
		// Update is called once per frame
		void Update()
		{
			if (CreateSection)
			{
				ComputeSection();
				CreateSection = false;
			}
		}
		#endregion
		//*********************************************************************************
		void Initialize()
		{
			m_mesh = this.GetComponent<MeshFilter>().mesh;
			m_lines = new List<Line>();
			m_intersectionPoints = new List<Vector3>();
			//m_projections = new List<ReferencedPoint>();
			m_map = new Map2D(new Plane(CuttingPlane.forward, CuttingPlane.position));
			m_section = null;
		}
		void ComputeSection()
		{
			Plane cuttingPlane = new Plane(CuttingPlane.up, CuttingPlane.position);
			m_shape = new Shape();

			//Get the intersection points and the lines that generate the solid
			for (int i = 0; i < m_mesh.triangles.Length; i += 3)
			{
				Line tmp_1 = new Line(m_mesh.vertices[m_mesh.triangles[i]], m_mesh.vertices[m_mesh.triangles[i + 1]], this.transform);
				Line tmp_2 = new Line(m_mesh.vertices[m_mesh.triangles[i]], m_mesh.vertices[m_mesh.triangles[i + 2]], this.transform);
				Line tmp_3 = new Line(m_mesh.vertices[m_mesh.triangles[i + 1]], m_mesh.vertices[m_mesh.triangles[i + 2]], this.transform);

				//Add the lines to the list
				m_lines.Add(tmp_1);
				m_lines.Add(tmp_2);
				m_lines.Add(tmp_3);

				Vector3? intersect_1 = tmp_1.PlaneIntersection(CuttingPlane.up, CuttingPlane.position, true);
				Vector3? intersect_2 = tmp_2.PlaneIntersection(CuttingPlane.up, CuttingPlane.position, true);
				Vector3? intersect_3 = tmp_3.PlaneIntersection(CuttingPlane.up, CuttingPlane.position, true);

				//Setup the reference points
				ReferencedPoint reference_1 = null;
				ReferencedPoint reference_2 = null;
				ReferencedPoint reference_3 = null;
				List<ReferencedPoint> references = new List<ReferencedPoint>();

				//Add the intersection points
				if (intersect_1.HasValue)
				{
					reference_1 = new ReferencedPoint(intersect_1.Value, m_map);
					references.Add(reference_1);

					if (!m_intersectionPoints.Contains(intersect_1.Value))
						m_intersectionPoints.Add(intersect_1.Value);
				}
				if (intersect_2.HasValue)
				{
					reference_2 = new ReferencedPoint(intersect_2.Value, m_map);
					references.Add(reference_2);

					if (!m_intersectionPoints.Contains(intersect_2.Value))
						m_intersectionPoints.Add(intersect_2.Value);
				}
				if (intersect_3.HasValue)
				{
					reference_3 = new ReferencedPoint(intersect_3.Value, m_map);
					references.Add(reference_3);

					if (!m_intersectionPoints.Contains(intersect_3.Value))
						m_intersectionPoints.Add(intersect_3.Value);
				}

				//Get the first reference not null
				if (references.Count > 1)
				{
					//set a new line
					Line lin = new Line(references[0].Original, references[1].Original);
					m_shape.AddEdge(lin);
				}
			}

			if (m_shape.Edges.Count == 0)
				return;

			//Sort the points and generate a closed shape
			m_shape.SortVertices();
			//Compute the mesh, triangles and vertices
			m_shape.ComputeMesh();

			//Instantiate the section
			if (m_section == null)
			{
				m_section = new GameObject(this.name + "_section");
				m_section = GameObject.Instantiate(m_section, this.transform);
				m_section.AddComponent<MeshRenderer>();
				m_section.AddComponent<MeshFilter>();
			}

			Mesh sectionedMesh = m_section.GetComponent<MeshFilter>().mesh;
			sectionedMesh.Clear();
			sectionedMesh.vertices = m_shape.Vertices.ToArray();
			sectionedMesh.triangles = m_shape.Triangles.ToArray();
			sectionedMesh.uv = m_shape.UV.ToArray();
			sectionedMesh.RecalculateTangents();
			sectionedMesh.RecalculateNormals();
			sectionedMesh.Optimize();
		}
	}
}
