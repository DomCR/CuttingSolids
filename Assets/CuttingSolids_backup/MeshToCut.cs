using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometricUtilities;
using System.Linq;
using UnityEditor;

namespace CuttingSolids
{
    public class MeshToCut : MonoBehaviour
    {
        public Transform CuttingPlane;
        public GameObject Section;

        private Mesh m_mesh;
        private List<Line> m_lines;
        private HashSet<Vector3> m_intersectionPoints;
        private Map2D m_plane;

        private void OnDrawGizmos()
        {
            Initialize();

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

                //Add the intersection points
                if (intersect_1.HasValue)
                    m_intersectionPoints.Add(intersect_1.Value);
                if (intersect_2.HasValue)
                    m_intersectionPoints.Add(intersect_2.Value);
                if (intersect_3.HasValue)
                    m_intersectionPoints.Add(intersect_3.Value);
            }

            //Map the points
            List<ReferencedPoint> refernces = new List<ReferencedPoint>();
            foreach (Vector3 item in m_intersectionPoints)
            {
                refernces.Add(new ReferencedPoint(item, m_plane.UVect, m_plane.VVect));
            }

            Triangulator tr = new Triangulator(refernces.Select(o => o.Reference).ToArray());
            int[] indices = tr.Triangulate();

            for (int i = 0; i < 1; i++)
            {

            }

            //*********************************************************************************
            // Create the mesh
            Mesh msh = new Mesh();
            msh.vertices = m_intersectionPoints.ToArray();
            msh.triangles = indices;
            msh.RecalculateNormals();
            //msh.RecalculateBounds();

            Section.GetComponent<MeshFilter>().mesh = msh;

            //Debug shapes
            Draw();
        }

        void Initialize()
        {
            m_mesh = this.GetComponent<MeshFilter>().mesh;
            m_lines = new List<Line>();
            m_intersectionPoints = new HashSet<Vector3>();
            m_plane = new Map2D(new Plane(CuttingPlane.forward, CuttingPlane.position));
        }

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

                //Gizmos.DrawIcon(item, "Hello");
                //Handles.Label(item, "Text");
            }
        }
    }
}