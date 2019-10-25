using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CuttingSolids.GeometricUtilities;

namespace CuttingSolids
{
    public class MeshToCut : MonoBehaviour
    {
        public Transform CuttingPlane;

        private Mesh m_mesh;
        private List<Line> m_lines;
        private List<Vector3> m_intersectionPoints;

        private void OnDrawGizmos()
        {
            Initialize();

            //Initialize the lines
            m_lines = new List<Line>();
            m_intersectionPoints = new List<Vector3>();

            for (int i = 0; i < m_mesh.triangles.Length; i += 3)
            {
                Line tmp_1 = new Line(m_mesh.vertices[m_mesh.triangles[i]], m_mesh.vertices[m_mesh.triangles[i + 1]]);
                Line tmp_2 = new Line(m_mesh.vertices[m_mesh.triangles[i]], m_mesh.vertices[m_mesh.triangles[i + 2]]);
                Line tmp_3 = new Line(m_mesh.vertices[m_mesh.triangles[i + 1]], m_mesh.vertices[m_mesh.triangles[i + 2]]);

                //Add the lines to the list
                m_lines.Add(tmp_1);
                m_lines.Add(tmp_2);
                m_lines.Add(tmp_3);

                Vector3? intersect_1 = tmp_1.PlaneIntersection(CuttingPlane.up, CuttingPlane.position);
                Vector3? intersect_2 = tmp_2.PlaneIntersection(CuttingPlane.up, CuttingPlane.position);
                Vector3? intersect_3 = tmp_3.PlaneIntersection(CuttingPlane.up, CuttingPlane.position);

                //Add the intersection points
                if (intersect_1.HasValue)
                {
                    m_intersectionPoints.Add(intersect_1.Value);

                    HashSet<Vector3> set = new HashSet<Vector3>();
                    set.Add(intersect_1.Value);
                    set.Add(intersect_1.Value);
                }
                if (intersect_2.HasValue)
                    m_intersectionPoints.Add(intersect_2.Value);
                if (intersect_3.HasValue)
                    m_intersectionPoints.Add(intersect_3.Value);


            }

            //Delete duplicated points
            //HashSet<Vector3> set = new HashSet<Vector3>(m_intersectionPoints); 

            //Debug shapes
            Draw();
        }

        void Initialize()
        {
            m_mesh = this.GetComponent<MeshFilter>().mesh;
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
            }
        }
    } 
}