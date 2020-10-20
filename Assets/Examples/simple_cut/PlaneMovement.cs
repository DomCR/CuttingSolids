using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMovement : MonoBehaviour
{
	public float MoveSpeed = 0.005f;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.UpArrow))
		{
		this.transform.Translate(0, MoveSpeed, 0, Space.Self);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			this.transform.Translate(0, -MoveSpeed, 0, Space.Self);
		}
	}
}
