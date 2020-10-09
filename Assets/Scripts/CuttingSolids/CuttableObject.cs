using GeometricUtilities;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

//[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class CuttableObject : MonoBehaviour
{
	public Transform CuttingPlane;

	private Mesh m_mesh;
	private Shape m_shape;

	//Gizmos drawing variables
	private Dictionary<Color, List<Line>> m_linesToDraw = new Dictionary<Color, List<Line>>();
	private List<Vector3> m_pointsToDraw = new List<Vector3>();
	#region Behaviour methods
	// Start is called before the first frame update
	void Start()
	{
		m_pointsToDraw = new List<Vector3>();
		m_linesToDraw = new Dictionary<Color, List<Line>>();

		m_mesh = this.GetComponent<MeshFilter>().mesh;
		
		//Gizmos drawing variables
		m_linesToDraw = new Dictionary<Color, List<Line>>();

		m_linesToDraw.Add(Color.red, new List<Line>());
		m_linesToDraw.Add(Color.yellow, new List<Line>());
		m_linesToDraw.Add(Color.blue, new List<Line>());
	}
	// Update is called once per frame
	void Update()
	{
		Start();

		Plane cuttingPlane = new Plane(CuttingPlane.up, CuttingPlane.position);
		m_shape = new Shape();

		//Get the intersection points and the lines that generate the solid
		for (int i = 0; i < m_mesh.triangles.Length; i += 3)
		{
			Line edge1 = new Line(m_mesh.vertices[m_mesh.triangles[i]], m_mesh.vertices[m_mesh.triangles[i + 1]], this.transform);
			Line edge2 = new Line(m_mesh.vertices[m_mesh.triangles[i]], m_mesh.vertices[m_mesh.triangles[i + 2]], this.transform);
			Line edge3 = new Line(m_mesh.vertices[m_mesh.triangles[i + 1]], m_mesh.vertices[m_mesh.triangles[i + 2]], this.transform);

			List<Line> edges = new List<Line>
			{
				edge1,
				edge2,
				edge3
			};

			m_linesToDraw[Color.red].AddRange(edges);

			createReference(edge1);
			createReference(edge2);
			createReference(edge3);

		}
	}
	private void OnDrawGizmos()
	{
		foreach (var item in m_linesToDraw)
		{
			foreach (var line in item.Value)
			{
				line.DrawGizmos(item.Key);
			}
		}

		foreach (Vector3 point in m_pointsToDraw)
		{
			Gizmos.DrawWireSphere(point, 0.02f);

		}
	}
	#endregion
	//*********************************************************************************
	/// <summary>
	/// Create a reference point if this intersects with the plane.
	/// </summary>
	/// <param name="edge"></param>
	void createReference(Line edge)
	{
		Vector3? intersect = edge.PlaneIntersection(CuttingPlane.up, CuttingPlane.position, true);

		if (intersect == null)
			return;

		m_pointsToDraw.Add(intersect.Value);
		Plane cuttingPlane = new Plane(CuttingPlane.up, CuttingPlane.position);

		//Add the intersection points
		if (intersect.HasValue)
		{
			//Lines must be sorted, blue on top, yellow bottom
			if (cuttingPlane.GetSide(edge.StartPoint))
			{
				m_linesToDraw[Color.yellow].Add(new Line(edge.StartPoint, intersect.Value));
				m_linesToDraw[Color.blue].Add(new Line(edge.EndPoint, intersect.Value));
			}
			else
			{
				m_linesToDraw[Color.blue].Add(new Line(edge.StartPoint, intersect.Value));
				m_linesToDraw[Color.yellow].Add(new Line(edge.EndPoint, intersect.Value));
			}
		}
	}
}
