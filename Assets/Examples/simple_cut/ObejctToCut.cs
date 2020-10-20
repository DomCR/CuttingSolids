using CuttingSolids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SectionType
{
	right,
	left
}

public class ObejctToCut : MonoBehaviour
{
	public Transform CuttingPlane;
	public SectionType Section;

	private Mesh m_mesh;
	private MeshFilter m_filterer;
	private CuttableMesh m_cutter;

	// Start is called before the first frame update
	void Start()
	{
		m_filterer = this.GetComponent<MeshFilter>();
		m_cutter = new CuttableMesh(m_filterer.mesh);
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			cut();
	}

	void cut()
	{
		Plane plane = new Plane(CuttingPlane.up, CuttingPlane.position);

		m_cutter.CutByPlane(CuttingPlane.position, plane, out Mesh right, out Mesh left);


		switch (Section)
		{
			case SectionType.right:
				m_filterer.mesh = right;
				break;
			case SectionType.left:
				m_filterer.mesh = left;
				break;
			default:
				break;
		}
	}
}
