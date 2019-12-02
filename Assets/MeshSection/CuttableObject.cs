using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GeometricUtilities;

namespace MeshSection
{
    public class CuttableObject : MonoBehaviour
    {
        public bool CreateSection;
        public Transform CuttingPlane;
        public GameObject Section;

        private Mesh m_mesh;
        private List<Line> m_lines;
        private List<Vector3> m_intersectionPoints;
        private List<ReferencedPoint> m_projections;
        private Map2D m_plane;

        #region Behaviour methods
        // Start is called before the first frame update
        void Start()
        {

        }
        // Update is called once per frame
        void Update()
        {

        }
        #endregion
        void OnDrawGizmos() //Change for update when stop debugging
        {
            Initialize();

            Plane cuttingPlane = new Plane(CuttingPlane.up, CuttingPlane.position);
            Shape section = new Shape();

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
                    reference_1 = new ReferencedPoint(intersect_1.Value, m_plane);
                    references.Add(reference_1);

                    if (!m_intersectionPoints.Contains(intersect_1.Value))
                        m_intersectionPoints.Add(intersect_1.Value);
                }
                if (intersect_2.HasValue)
                {
                    reference_2 = new ReferencedPoint(intersect_2.Value, m_plane);
                    references.Add(reference_2);

                    if (!m_intersectionPoints.Contains(intersect_2.Value))
                        m_intersectionPoints.Add(intersect_2.Value);
                }
                if (intersect_3.HasValue)
                {
                    reference_3 = new ReferencedPoint(intersect_3.Value, m_plane);
                    references.Add(reference_3);

                    if (!m_intersectionPoints.Contains(intersect_3.Value))
                        m_intersectionPoints.Add(intersect_3.Value);
                }

                //Setup connections
                //reference_1.AddConnection(reference_2);
                //reference_1.AddConnection(reference_3);
                //reference_2.AddConnection(reference_1);
                //reference_2.AddConnection(reference_3);
                //reference_3.AddConnection(reference_1);
                //reference_3.AddConnection(reference_2);

                //Get the first reference not null
                if (references.Count > 0)
                {
                    //set a new line
                    Line lin = new Line(references[0].Original, references[1].Original);
                    section.AddLine(lin);
                    //lin.DrawGizmos(Color.blue);

                    if (m_projections.Where(o => o.Original == references[0].Original).Count() > 0)
                    {
                        m_projections.Where(o => o.Original == references[0].Original).First().AddConnection(references[1]);
                    }
                    else if (m_projections.Where(o => o.Original == references[1].Original).Count() > 0)
                    {
                        m_projections.Where(o => o.Original == references[1].Original).First().AddConnection(references[0]);
                    }
                    else
                    {
                        references[0].AddConnection(references[1]);
                        m_projections.Add(references[0]);
                    }
                }
            }

            section.SetSortedVertices();

            //for (int i = 0; i < section.Vertices.Count - 1; i++)
            //{
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawLine(section.Vertices[i], section.Vertices[i + 1]);
            //}

            section.ComputeMesh();
            //section.DrawShape();
            //section.DrawTriangles();
            //*********************************************************************************
            if (true)
            {
                Mesh section_mesh = Section.GetComponent<MeshFilter>().mesh;
                section_mesh.Clear();
                section_mesh.vertices = section.Vertices.ToArray();
                section_mesh.triangles = section.Triangles.ToArray();
                section_mesh.uv = section.UV.ToArray();
                section_mesh.RecalculateTangents();
                section_mesh.RecalculateNormals();
                section_mesh.Optimize();

                CreateSection = false;
            }
            //*********************************************************************************
            //Debug shapes
            Draw();
        }
        //*********************************************************************************
        void Initialize()
        {
            m_mesh = this.GetComponent<MeshFilter>().mesh;
            m_lines = new List<Line>();
            m_intersectionPoints = new List<Vector3>();
            m_projections = new List<ReferencedPoint>();
            m_plane = new Map2D(new Plane(CuttingPlane.forward, CuttingPlane.position));
        }
        //*********************************************************************************
        void Draw()
        {
            foreach (Line line in m_lines)
            {
                line.DrawGizmos(Color.yellow);
            }
            foreach (Vector3 item in m_intersectionPoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(item, 0.02f);

                //UnityEditor.Handles.Label(item, item.ToString());
            }
        }
    }
}