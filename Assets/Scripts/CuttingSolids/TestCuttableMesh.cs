using CuttingSolids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestCuttableMesh : MonoBehaviour
{
	public Transform CuttingPlane;
	public MeshFilter RightMesh;
	public MeshFilter LeftMesh;

	private CuttableMesh m_cutter;

	private GameObject m_rightPart;
	private GameObject m_leftPart;

	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		m_cutter = new CuttableMesh(this.GetComponent<MeshFilter>().mesh);
		Plane plane = new Plane(CuttingPlane.up, CuttingPlane.position);

		m_cutter.CutByPlane(CuttingPlane.position, plane, out Mesh right, out Mesh left);

		//Tranform the mesh
		//RightMesh.mesh = right;
		//LeftMesh.mesh = left;
	}
}
